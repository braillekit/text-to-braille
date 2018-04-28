using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    public static class ContextTagNames
    {
        public const string Title = "<標題>";
        public const string Indent = "<縮排>";
        public const string Math = "<數學>";
        public const string Coordinate = "<座標>";
        public const string Fraction = "<分數>";
        public const string Table = "<表格>";
        public const string Time = "<時間>";
        public const string Delete = "<刪>";
        public const string Phonetic = "<音標>";
        public const string SpecificName = "<私名號>";
        public const string BookName = "<書名號>";
        public const string BrailleTranslatorNote = "<點譯者註>";
        public const string OrgPageNumber = "<P>";

        // NOTE: 每當有變動時，必須同步修改 AllTagNames 的內容。

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
                Time,
                Delete,
                Phonetic,
                SpecificName,
                BookName,
                BrailleTranslatorNote,
                OrgPageNumber          // 原書頁碼

            };


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
