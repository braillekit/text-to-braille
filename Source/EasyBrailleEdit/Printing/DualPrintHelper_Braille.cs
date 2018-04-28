using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Converters;
using EasyBrailleEdit.Common;
using Huanlin.Common.Helpers;
using Huanlin.Windows.Forms;
using Huanlin.Windows.WinApi;

namespace EasyBrailleEdit
{
    /// <summary>
    /// 局部類別：處理點字的列印。
    /// </summary>
    public partial class DualPrintHelper
    {
        const string NewLine = "\n";
        const string NewPage = "\f";

        /// <summary>
        /// 列印點字。
        /// </summary>
        public void PrintBraille(bool toBrailler, bool toFile, string fileName)
        {
            if (toBrailler == false && toFile == false)
                return;

            string msg = "請按「確定」鈕開始列印點字。";
            if (toFile)
            {
                msg = "請按「確定」鈕開始將點字資料輸出至以下檔案:\r\n" + fileName;
            }
            if (MsgBoxHelper.ShowOkCancel(msg) != DialogResult.OK)
                return;

            bool cancel = false;

            // 起始列印工作
            BeginPrintBraille(ref cancel);
            if (cancel)
            {
                return;
            }

            StringBuilder brailleData = GenerateOutputData();
            if (toBrailler)
            {
                WriteToBrailler(brailleData);   // 輸出至點字印表機
            }
            if (toFile)
            {
                WriteToFile(brailleData, fileName); // 輸出至檔案
            }

            // 收尾列印工作
            EndPrintBraille(toBrailler, toFile);
        }

        private void BeginPrintBraille(ref bool cancel)
        {
            InitializePrintParameters();
        }

        private void EndPrintBraille(bool toBrailler, bool toFile)
        {
            StringBuilder sb = new StringBuilder("點字已輸出至指定的");
            if (toBrailler && toFile)
            {
                sb.Append("點字印表機和檔案。");
            }
            else
            {
                sb.Append(toBrailler ? "點字印表機。" : "檔案。");
            }
            MsgBoxHelper.ShowInfo(sb.ToString());
        }

        private StringBuilder AddPageBreak(StringBuilder sb)
        {
            if (m_PrintOptions.BrUseNewLineForPageBreak)
            {
                // 注意：這裡不利用跳頁符號換頁，而是採取印滿整頁的方式，讓印表機自動跳新頁。
                sb.Append(NewLine);
            }
            else
            {
                sb.Append(NewPage);
            }
            return sb;
        }

        /// <summary>
        /// 產生輸出的點字資料。
        /// </summary>
        /// <returns></returns>
        private StringBuilder GenerateOutputData()
        {
            int lineCnt = 0;
            int pageNum = 0;			// 程式內部處理的頁碼
            BrailleLine brLine;
            StringBuilder sb = new StringBuilder();

            int realLinesPerPage = m_PrintOptions.LinesPerPage;

            if (m_PrintOptions.PrintPageFoot)  // 如需列印頁碼，每頁可印列數便少一列。
            {
                realLinesPerPage--;
            }

            // 計算起始列索引
            int lineIdx = 0;
            if (!m_PrintOptions.AllPages)
            {
                lineIdx = CalcTextLineIndex(m_PrintOptions.FromPage - 1);
            }

            // 準備輸出至點字印表機的資料
            while (lineIdx < m_BrDoc.LineCount)
            {
                brLine = m_BrDoc.Lines[lineIdx];

                SetOrgPageNumber(brLine, (lineCnt % realLinesPerPage == 0));	// 設定起始/終止原書頁碼。

                sb.Append(BrailleCharConverter.ToString(brLine));
                sb.Append(NewLine);     // 每一列後面附加一個換行符號。

                lineCnt++;

                // 列印頁尾資訊：文件標題、原書頁碼、點字頁碼。
                if (lineCnt % realLinesPerPage == 0)    // 已經印滿一頁了？
                {
                    if (m_PrintOptions.PrintPageFoot)  // 是否要印頁尾？
                    {
                        pageNum++;

                        sb.Append(GetBraillePageFoot(lineIdx, m_DisplayedPageNum, m_BeginOrgPageNumber, m_EndOrgPageNumber));

                        AddPageBreak(sb);
                    }

                    m_DisplayedPageNum++;

                    // 每一頁開始列印時，都要把上一頁的終止原書頁碼指定給本頁的起始原書頁碼。
                    if (!String.IsNullOrEmpty(m_EndOrgPageNumber))
                    {
                        m_BeginOrgPageNumber = m_EndOrgPageNumber;
                    }

                    if (pageNum >= m_PrintOptions.ToPage)
                        break;
                }

                lineIdx++;
            }

            // 補印頁碼
            if (lineCnt % realLinesPerPage != 0)
            {
                pageNum++;

                if (m_PrintOptions.PrintPageFoot)
                {
                    // 用空白列補滿剩餘的頁面
                    int n = realLinesPerPage - (lineCnt % realLinesPerPage);
                    for (int i = 0; i < n; i++)
                        sb.Append(NewLine);

                    sb.Append(GetBraillePageFoot(lineIdx, m_DisplayedPageNum, m_BeginOrgPageNumber, m_EndOrgPageNumber));

                    AddPageBreak(sb);
                }
                m_DisplayedPageNum++;
            }

            if (m_PrintOptions.BrSendPageBreakAtEndOfDoc)
            {
                sb.Append(NewPage);
            }

            return sb;
        }

        /// <summary>
        /// 傳回點字頁尾。
        /// </summary>
        /// <param name="lineIdx">目前列印的列索引。用來計算頁尾的文件標題。</param>
        /// <param name="pageNum">頁碼。</param>
        /// <param name="beginOrgPageNum">起始原書頁碼。</param>
        /// <param name="endOrgPageNum">終止原書頁碼。</param>
        /// <returns></returns>
        /// <remarks>注意：點字頁碼的 # 號要固定印在第 37 方的位置（requested by 秋華）</remarks>
        private string GetBraillePageFoot(int lineIdx, int pageNum, string beginOrgPageNum, string endOrgPageNum)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbPageNum = new StringBuilder();

            // 標題
            BrailleLine titleLine = m_BrDoc.GetPageTitle(lineIdx);
            string title = BrailleCharConverter.ToString(titleLine);

            // 原書頁碼
            if (!String.IsNullOrEmpty(beginOrgPageNum))
            {
                int beginNumber = -1;
                Int32.TryParse(beginOrgPageNum, out beginNumber);

                string orgPageNum = "";
                if (String.IsNullOrEmpty(endOrgPageNum))
                {
                    if (beginNumber >= 0)
                    {
                        orgPageNum = BrailleCharConverter.GetDigitCharCode(beginNumber, true);
                    }
                    else
                    {
                        // 如果原書頁碼不是阿拉伯數字，就直接使用原始文字（不用轉換）。
                        orgPageNum = beginOrgPageNum;
                    }                    
                }
                else // 起始原書頁碼和終止原書頁碼都有指定。
                {                    
                    if (beginOrgPageNum == endOrgPageNum)
                    {
                        if (beginNumber >= 0)
                        {
                            orgPageNum = BrailleCharConverter.GetDigitCharCode(beginNumber, true);
                        }
                        else
                        {
                            // 如果原書頁碼不是阿拉伯數字，就直接使用原始文字（不用轉換）。
                            orgPageNum = beginOrgPageNum;
                        }                            
                    }
                    else
                    {
                        var hyphen = BrailleCharConverter.ToChar(BrailleCell.DotsToByte(3, 6).ToString("X2"));
                        int endNumber = -1;
                        Int32.TryParse(endOrgPageNum, out endNumber);

                        if (endNumber >= 0)
                        {
                            orgPageNum = BrailleCharConverter.GetDigitCharCode(beginNumber, true) 
                                + hyphen + BrailleCharConverter.GetDigitCharCode(endNumber, true);
                        }
                        else
                        {
                            // 如果原書頁碼不是阿拉伯數字，就直接使用原始文字（不用轉換）。
                            orgPageNum = beginOrgPageNum + hyphen + endOrgPageNum;
                        }
                    }
                }
                sbPageNum.Append('#');			// 數字點
                sbPageNum.Append(orgPageNum);	// 原書頁碼
                sbPageNum.Append(' ');			// 空方
            }

            sbPageNum.Append('#');	// 數字點
            string pageNumStr = BrailleCharConverter.GetDigitCharCode(pageNum, true);
            sbPageNum.Append(pageNumStr.PadRight(3));	// 點字頁碼的數字部分固定佔三方，亦即 # 固定在第 37 方的位置

            // 計算剩餘可容納標題的空間。
            int roomForTitle = m_BrDoc.CellsPerLine - sbPageNum.Length - 1;  // 多留一個空白

            if (title.Length > roomForTitle)
            {
                title = title.Substring(0, roomForTitle);
            }
            else
            {
                title = title.PadRight(roomForTitle);
            }
            sb.Append(title);		// 標題
            sb.Append(' ');			// 空方
            sb.Append(sbPageNum.ToString());	// 原書頁碼、點字頁碼

            return sb.ToString();
        }

        private void WriteToBrailler(StringBuilder brailleData)
        {
            if (m_PrintOptions.PrinterNameForBraille.Equals(AppGlobals.Config.Printing.BraillePrinterPort))
            {
                // 輸出至 LPT port
                LptPrintHelper lpt = new LptPrintHelper();
                lpt.OpenPrinter(AppGlobals.Config.Printing.BraillePrinterPort);
                lpt.Print(brailleData.ToString());
                lpt.ClosePrinter();
            }
            else
            {
                // 輸出至 Windows 印表機
                RawPrinterHelper.SendStringToPrinter(m_PrintOptions.PrinterNameForBraille, brailleData.ToString());
            }

            // 同時將列印的內容輸出至檔案。
            //WriteToFile(brailleData, @"c:\SentToBrailler.txt");
        }

        private void WriteToFile(StringBuilder brailleData, string fileName)
        {
            // 將列印的內容輸出至檔案。
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.Write(brailleData.ToString());
                sw.Flush();
            }
        }
    }
}
