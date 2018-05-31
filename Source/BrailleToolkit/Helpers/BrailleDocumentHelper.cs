using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Converters;
using BrailleToolkit.Tags;

namespace BrailleToolkit.Helpers
{
    public static class BrailleDocumentHelper
    {

        /// <summary>
        /// 傳回點字頁尾。內容包括：頁標題、點字頁碼、原書頁碼。
        /// </summary>
        /// <param name="lineIdx">目前列印的列索引。用來計算頁尾的文件標題。</param>
        /// <param name="pageNum">頁碼。</param>
        /// <param name="beginOrgPageNum">起始原書頁碼。</param>
        /// <param name="endOrgPageNum">終止原書頁碼。</param>
        /// <returns></returns>
        /// <remarks>注意：點字頁碼的 # 號要固定印在第 37 方的位置（requested by 秋華）</remarks>
        public static string GetBraillePageFoot(BrailleDocument brDoc,
            int lineIdx, int pageNum, string beginOrgPageNum, string endOrgPageNum)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbPageNum = new StringBuilder();

            // 標題
            BrailleLine titleLine = brDoc.GetPageTitle(lineIdx);
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
                        var hyphen = BrailleCharConverter.ToChar(BrailleCell.PositionNumbersToByte(3, 6).ToString("X2"));
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
            int roomForTitle = brDoc.CellsPerLine - sbPageNum.Length - 1;  // 多留一個空白

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


        /// <summary>
        /// 從傳入的字串中取出原書頁碼。
        /// </summary>
        /// <param name="line"></param>
        /// <returns>若傳入的字串不是原書頁碼，則傳回空字串。否則傳回原書頁碼的文字（必須是字串，因為頁碼可能是羅馬數字）。</returns>
        public static string GetOrgPageNumber(string line)
        {
            if (String.IsNullOrEmpty(line)) return String.Empty;

            var pageNumberText = String.Empty;

            var endTagName = XmlTagHelper.GetEndTagName(ContextTagNames.OrgPageNumber);
            line = line.Trim();
            if (line.StartsWith(ContextTagNames.OrgPageNumber) && line.EndsWith(endTagName))
            {
                pageNumberText =
                    line.Replace(ContextTagNames.OrgPageNumber, String.Empty)
                        .Replace(endTagName, String.Empty)
                        .Replace(OrgPageNumberContextTag.LeadingUnderline, String.Empty)
                        .Trim();

            }
            else if (line.StartsWith(OrgPageNumberContextTag.LeadingUnderlines))
            {
                pageNumberText =
                    line.Remove(0, OrgPageNumberContextTag.LeadingUnderlines.Length)
                        .Replace(endTagName, String.Empty);
            }

            return pageNumberText;
        }


        /// <summary>
        /// 判斷傳入的列是否為原書頁碼，如果是，則設定起始或（與）終止原書頁碼。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="isFirstLineOfPage">是否為該頁的第一列。</param>
        /// <param name="beginOrgPageNumber">若有發現原書頁碼，而且 beginOrgPageNumber 尚未設定過，便會設定此參數值。</param>
        /// <param name="endOrgPageNumber">若有發現原書頁碼，便會設定此參數值。
        /// 也就是說，所以反覆呼叫此方法時，endOrgPageNumber 是會持續變動的；而 beginOrgPageNumber 只會設定一次。</param>
        public static void SetBeginEndOrgPageNumber(
            BrailleLine brLine, 
            bool isFirstLineOfPage,
            ref string beginOrgPageNumber, 
            ref string endOrgPageNumber)
        {
            string line = brLine.ToString();

            string orgPageNum = GetOrgPageNumber(line);
            if (String.IsNullOrEmpty(orgPageNum))
            {
                return;
            }

            if (String.IsNullOrEmpty(beginOrgPageNumber))
            {
                beginOrgPageNumber = orgPageNum;
            }
            if (isFirstLineOfPage)	// 如果該頁的第一列就是原書頁碼
            {
                beginOrgPageNumber = orgPageNum;	// 則起始原書頁碼應該直接使用此頁碼。
            }
            endOrgPageNumber = orgPageNum;
        }

        public static int RemoveSharpSymbolFromPageNumbers(BrailleDocument doc)
        {
            int removedCount = 0;
            foreach(var brLine in doc.Lines)
            {
                var text = brLine.ToOriginalTextString(null);
                int wordIdx = text.IndexOf("<P>#");
                if (wordIdx >= 0)
                {
                    removedCount += FindAndRemoveSharpSymbol(brLine, wordIdx + 1);
                }
                else
                {
                    // 若沒有 <P> 標籤，那麼以連續 36 個底線符號開頭的 BrailleLine 也視為原書頁碼
                    text = brLine.ToString();
                    wordIdx = text.IndexOf(OrgPageNumberContextTag.LeadingUnderlines);
                    if (wordIdx >= 0)
                    {
                        removedCount += FindAndRemoveSharpSymbol(brLine, wordIdx + 1);
                    }
                }
            }
            return removedCount;

            // local function
            int FindAndRemoveSharpSymbol(BrailleLine brLine, int startWordIdx)
            {
                for (int i = startWordIdx + 1; i < brLine.WordCount; i++)
                {
                    var brWord = brLine[i];
                    if (brWord.Text == "#" && brWord.CellCount < 1)
                    {
                        brLine.RemoveAt(i);
                        return 1;
                    }
                }
                return 0;
            }
        }

    }
}
