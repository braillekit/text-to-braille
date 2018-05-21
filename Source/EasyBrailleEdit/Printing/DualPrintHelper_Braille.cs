using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BrailleToolkit;
using BrailleToolkit.Converters;
using BrailleToolkit.Helpers;
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
            if (!toBrailler && !toFile)
                return;

            string msg = "請按「確定」鈕開始列印點字。";
            if (toFile)
            {
                msg = "請按「確定」鈕開始將點字資料輸出至以下檔案:\r\n" + fileName;
            }
            if (MsgBoxHelper.ShowOkCancel(msg) != DialogResult.OK)
                return;

            // 起始列印工作
            BeginPrintBraille();

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

        private void BeginPrintBraille()
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

                bool isFirstLineOfCurrentPage = lineCnt % realLinesPerPage == 0;

                // 設定起始/終止原書頁碼。
                BrailleDocumentHelper.SetBeginEndOrgPageNumber(
                    brLine,
                    isFirstLineOfCurrentPage,
                    ref m_BeginOrgPageNumber,
                    ref m_EndOrgPageNumber);

                sb.Append(BrailleCharConverter.ToString(brLine));
                sb.Append(NewLine);     // 每一列後面附加一個換行符號。

                lineCnt++;

                // 列印頁尾資訊：文件標題、原書頁碼、點字頁碼。
                if (lineCnt % realLinesPerPage == 0)    // 已經印滿一頁了？
                {
                    if (m_PrintOptions.PrintPageFoot)  // 是否要印頁尾？
                    {
                        pageNum++;

                        sb.Append(BrailleDocumentHelper.GetBraillePageFoot(m_BrDoc, lineIdx, m_DisplayedPageNum, m_BeginOrgPageNumber, m_EndOrgPageNumber));

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

                    sb.Append(BrailleDocumentHelper.GetBraillePageFoot(m_BrDoc, lineIdx, m_DisplayedPageNum, m_BeginOrgPageNumber, m_EndOrgPageNumber));

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
