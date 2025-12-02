using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    /// <summary>
    /// Defines the names of context tags used in the application.
    /// </summary>
    public static class ContextTagNames
    {
        /// <summary>Title tag.</summary>
        public const string Title = "<標題>";
        /// <summary>Indent tag.</summary>
        public const string Indent = "<縮排>";
        /// <summary>Math tag.</summary>
        public const string Math = "<數學>";
        /// <summary>Coordinate tag.</summary>
        public const string Coordinate = "<座標>";
        /// <summary>Fraction tag.</summary>
        public const string Fraction = "<分數>";
        /// <summary>Table tag.</summary>
        public const string Table = "<表格>";
        /// <summary>Table top line 1 tag.</summary>
        public const string TableTopLine1 = "<上表格線1>";
        /// <summary>Table bottom line 1 tag.</summary>
        public const string TableBottomLine1 = "<下表格線1>";
        /// <summary>Table top line 2 tag.</summary>
        public const string TableTopLine2 = "<上表格線2>";
        /// <summary>Table bottom line 2 tag.</summary>
        public const string TableBottomLine2 = "<下表格線2>";
        /// <summary>Time tag.</summary>
        public const string Time = "<時間>";
        /// <summary>Delete tag.</summary>
        public const string Delete = "<刪>";
        /// <summary>Phonetic tag.</summary>
        public const string Phonetic = "<音標>";
        /// <summary>Specific name tag.</summary>
        public const string SpecificName = "<私名號>";
        /// <summary>Book name tag.</summary>
        public const string BookName = "<書名號>";
        /// <summary>Braille translator note tag.</summary>
        public const string BrailleTranslatorNote = "<點譯者註>";
        /// <summary>Original page number tag.</summary>
        public const string OrgPageNumber = "<P>";
        /// <summary>Choice tag.</summary>
        public const string Choice = "<選項>"; // 選項裡面的 "ㄅ." 之間不空方，且小數點的點字為 6。
        /// <summary>Upper position tag.</summary>
        public const string UpperPosition = "<上位點>";  // 數字一律使用上位點。
        /// <summary>URL tag.</summary>
        public const string Url = "<URL>";
        /// <summary>Quotation mark 1 tag.</summary>
        public const string QuotationMark1 = "<引號一>"; // (236)(356)
        /// <summary>Quotation mark 2 tag.</summary>
        public const string QuotationMark2 = "<引號二>"; // (236)(456 356)
        /// <summary>Separator line tag.</summary>
        public const string SeparatorLine = "<分隔線>";  // (135 246)
        /// <summary>No digit symbol tag.</summary>
        public const string NoDigitSymbol = "<無數符>"; // 這是暫時性的控制標籤，只在轉點字過程中存在。

        // NOTE: 每當有變動時，必須同步修改 AllTagNames 的內容。

        /// <summary>
        /// Gets the collection of all context tag names.
        /// </summary>
        public static HashSet<string> Collection =
            new HashSet<string>()
            {
                Title,
                Indent,
                Indent,
                Math,
                Coordinate,
                Fraction,
                Table,
                TableTopLine1,
                TableBottomLine1,
                TableTopLine2,
                TableBottomLine2,
                Time,
                Delete,
                Phonetic,
                SpecificName,
                BookName,
                BrailleTranslatorNote,
                OrgPageNumber,          // 原書頁碼
                Choice,
                UpperPosition,
                Url,
                QuotationMark1,
                QuotationMark2,
                SeparatorLine,
                NoDigitSymbol
            };


        /// <summary>
        /// Checks if the given string is a title tag.
        /// </summary>
        /// <param name="s">The string to check.</param>
        /// <returns>True if it is a title tag; otherwise, false.</returns>
        public static bool IsTitleTag(string s) => XmlTagHelper.IsTag(s, Title);
        

        /// <summary>
        /// 檢查傳入的字串是否是以語境標籤（含起始及結束標籤）開頭。
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool StartsWithContextTag(string s)
        {
            string beginTag;
            string endTag;
            bool found = false;

            foreach (string tagName in Collection)
            {
                beginTag = tagName;
                endTag = beginTag.Insert(1, "/");
                if (s.StartsWith(beginTag) || s.StartsWith(endTag))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

    }
}
