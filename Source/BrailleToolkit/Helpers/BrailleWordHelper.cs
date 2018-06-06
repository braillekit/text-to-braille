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

        public static string ToString(List<BrailleWord> brWordList)
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

        public static string ToDotNumberString(List<BrailleWord> brWordList)
        {
            var sb = new StringBuilder();
            foreach (var brWord in brWordList)
            {
                sb.Append(brWord.ToPositionNumberString(useParenthesis: true));
            }
            return sb.ToString();
        }

        public static string ToOriginalTextString(List<BrailleWord> words)
        {
            var sb = new StringBuilder();
            int index = 0;
            while (index < words.Count)
            {
                var brWord = words[index];
                if (brWord.IsContextTag)
                {
                    sb.Append(brWord.Text); // 輸出標籤名稱（可能為起始標籤或結束標籤）。
                    index++;
                    continue;
                }
                if (brWord.IsConvertedFromTag) // 只要是由 context tag 所衍生的文字都不儲存。
                {
                    index++;
                    continue;
                }

                // 一般文字，或曾被替換過的文字。
                if (!String.IsNullOrEmpty(brWord.OriginalText))
                    sb.Append(brWord.OriginalText);
                else
                    sb.Append(brWord.Text);
                index++;
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

        public static bool RemoveDigitSymbol(BrailleWord brWord)
        {
            if (brWord.CellCount <= 1)
            {
                // 至少要有兩個 cells 才能執行刪除操作。
                return false;
            }

            if ((int)brWord.Cells[0].Value == (int)BrailleCellCode.Digit)
            {
                brWord.Cells.RemoveAt(0);
                return true;
            }
            return false;
        }
    }
}
