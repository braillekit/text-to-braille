using BrailleToolkit.Tags;
using Huanlin.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Helpers
{
    public static class BrailleWordHelper
    {

        public static bool IsChinesePunctuation(BrailleWord brWord)
        {
            if (brWord == null)
            {
                throw new ArgumentNullException("呼叫 IsChinesePunctuation() 時傳入了 null 參數", nameof(brWord));
            }
            return (BrailleGlobals.ChinesePunctuations.IndexOf(brWord.Text) >= 0);
        }

        public static string ToString(this List<BrailleWord> brWordList)
        {
            var sb = new StringBuilder();
            foreach (var brWord in brWordList)
            {            
                sb.Append("{");
                sb.Append($"\"{brWord.Text}\",");
                sb.Append($"\"{brWord.PhoneticCode}\",");
                sb.Append($"\"{brWord.CellList.ToString()}\"");
                sb.Append("},");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public static string ToDotNumberString(this List<BrailleWord> brWordList)
        {
            var sb = new StringBuilder();
            foreach (var brWord in brWordList)
            {
                sb.Append(brWord.ToPositionNumberString(useParenthesis: true));
            }
            return sb.ToString();
        }

        public static int GetCellCount(this List<BrailleWord> brWordList)
        {
            int count = 0;
            foreach (var brWord in brWordList)
            {
                count += brWord.CellCount;
            }
            return count;
        }

        /// <summary>
        /// 是否包含標題情境標籤。
        /// </summary>
        /// <returns></returns>
        public static bool ContainsTitleTag(List<BrailleWord> brWordList)
        {
            if (brWordList.Count > 0 && brWordList[0].Text.Equals(ContextTagNames.Title))
            {
                return true;
            }
            return false;
        }
    }
}
