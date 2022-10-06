using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Converters;
using BrailleToolkit.Helpers;
using EasyBrailleEdit.Common;

namespace BrailleToolkit
{
    /// <summary>
    /// 此類別的程式碼，最初是從 DualPrintHelper_Braille.cs 中複製過來，因為匯出點字檔跟
    /// 列印點字的處理方式很接近，只有某些字元需要替換掉（參閱 FixBrailleDataForWCBE 方法）。
    /// </summary>
    public sealed class BrailleDataExporter
    {
        const string NewLine = "\n";
        const string NewPage = "\f";

        private BrailleDocument _brDoc;

        public BrailleDataExporter(BrailleDocument brDoc, int linesPerPage, int startPageNum = 1, bool needPageFooter = true)
        {
            _brDoc = brDoc ?? throw new ArgumentNullException(nameof(brDoc));
            LinesPerPage = linesPerPage;
            StartPageNumber = startPageNum;
            NeedPageFooter = needPageFooter;
            AddPageBreakAtEndOfFile = false;
            UseNewLineForPageBreak = true; // 匯出點字檔時，不要使用跳頁字元，因為超點的「中英文點字編輯器」不認得跳頁符號。
        }

        public int LinesPerPage { get; }

        public int StartPageNumber { get; } // 起始頁碼

        public bool NeedPageFooter { get; }

        public bool AddPageBreakAtEndOfFile { get; }

        public bool UseNewLineForPageBreak { get; }

        /// <summary>
        /// 匯出點字檔（副檔名為 .BRL）。可供列印或者給超點的 WCBE（中英文點字編輯器）讀取。
        /// </summary>
        /// <param name="fileName"></param>
        public int SaveBrailleFile(string fileName)
        {
            int endPageNumber ;

            string brailleText = GetAllBrailleText(out endPageNumber);

            Encoding enc = Encoding.UTF8;
            File.WriteAllText(fileName, brailleText, enc);

            return endPageNumber;
        }

        public string GetAllBrailleText(out int endPageNumber)
        {
            // 1.產生用來列印的點字資料。
            var brailleData = GetBrailleDataForPrint(out endPageNumber);

            // 2.修正點字資料，以便供列印或者給超點的 WCBE（中英文點字編輯器）使用。
            FixBrailleDataForPrint(ref brailleData);

            return brailleData.Replace("\n", "\r\n").ToString();
        }

        /// <summary>
        ///       1.全部的英文改成小寫(  A B C D  改成  a b c d)。
        ///       2.  ^  符號改成 ~ 符號。
        ///       3.  @  符號改成 ` 符號。
        ///       4.  [  符號改成 { 符號。
        ///       5.  ]  符號改成 } 符號。
        ///       6.  \  符號改成 | 符號。 
        /// </summary>
        /// <param name="brailleData">可列印的點字資料。</param>
        private void FixBrailleDataForPrint(ref StringBuilder brailleData)
        {
            string oldChars = @"^@[]\";
            string newChars = @"~`{}|";

            for (int i = 0; i < oldChars.Length; i++)
            {
                brailleData.Replace(oldChars[i], newChars[i]);
            }
            brailleData = new StringBuilder(brailleData.ToString().ToLower());
        }

        /// <summary>
        /// 產生給點字打印機的點字資料。
        /// 此方法是修改自 DualPrintHelper_Braille.cs 中的 GenerateOutputData 方法。
        /// </summary>
        /// <param name="endPageNumber">輸出終止頁碼。</param>
        /// <returns></returns>
        private StringBuilder GetBrailleDataForPrint(out int endPageNumber)
        {
            int lineCnt = 0;
            int pageNum = 0;			// 程式內部處理的頁碼
            BrailleLine brLine;
            StringBuilder sb = new StringBuilder();

            int realLinesPerPage = LinesPerPage;

            int maxPages = AppGlobals.UserLicense.GetMaxPages();

            if (NeedPageFooter)  // 如需輸出頁碼，每頁可印列數便少一列。
            {
                realLinesPerPage--;
            }

            // 計算起始列索引
            int lineIdx = 0;
            string beginOrgPageNum = null;
            string endOrgPageNum = null;
            int displayedPageNum = StartPageNumber;
            endPageNumber = displayedPageNum;

            // 準備輸出至點字印表機的資料
            while (lineIdx < _brDoc.LineCount)
            {
                if (pageNum >= maxPages) // 試用版的列印限制
                {
                    break;
                }

                brLine = _brDoc.Lines[lineIdx];
                bool isFirstLineOfCurrentPage = lineCnt % realLinesPerPage == 0;
                BrailleDocumentHelper.SetBeginEndOrgPageNumber(
                    brLine,
                    isFirstLineOfCurrentPage,
                    ref beginOrgPageNum,
                    ref endOrgPageNum);	// 設定起始/終止原書頁碼。

                sb.Append(BrailleCharConverter.ToString(brLine));
                sb.Append(NewLine);     // 每一列後面附加一個換行符號。

                lineCnt++;

                // 列印頁尾資訊：文件標題、原書頁碼、點字頁碼。
                if (lineCnt % realLinesPerPage == 0)    // 已經印滿一頁了？
                {
                    if (NeedPageFooter)  // 是否要印頁尾？
                    {
                        sb.Append(BrailleDocumentHelper.GetBraillePageFoot(
                            _brDoc, lineIdx, displayedPageNum, beginOrgPageNum, endOrgPageNum));

                        endPageNumber = displayedPageNum;

                        AddPageBreak(sb);
                    }

                    pageNum++;
                    displayedPageNum++;

                    // 每一頁開始列印時，都要把上一頁的終止原書頁碼指定給本頁的起始原書頁碼。
                    if (!String.IsNullOrEmpty(endOrgPageNum))
                    {
                        beginOrgPageNum = endOrgPageNum;
                    }
                }

                lineIdx++;
            }

            // 補印頁碼
            if (lineCnt % realLinesPerPage != 0)
            {
                pageNum++;

                if (NeedPageFooter)
                {
                    // 用空白列補滿剩餘的頁面
                    int n = realLinesPerPage - (lineCnt % realLinesPerPage);
                    for (int i = 0; i < n; i++)
                        sb.Append(NewLine);

                    sb.Append(BrailleDocumentHelper.GetBraillePageFoot(
                        _brDoc, lineIdx, displayedPageNum, beginOrgPageNum, endOrgPageNum));

                    endPageNumber = displayedPageNum;

                    AddPageBreak(sb);
                }
                displayedPageNum++;
            }

            if (AddPageBreakAtEndOfFile)
            {
                AddPageBreak(sb);
            }

            return sb;
        }

        private StringBuilder AddPageBreak(StringBuilder sb)
        {
            if (UseNewLineForPageBreak)
            {
                // 不使用跳頁符號換頁，而是採取印滿整頁的方式，讓印表機自動跳新頁。
                sb.Append(NewLine);
            }
            else
            {
                sb.Append(NewPage);
            }
            return sb;
        }
    }
}
