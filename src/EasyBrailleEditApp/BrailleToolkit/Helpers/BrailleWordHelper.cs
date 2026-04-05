using BrailleToolkit.Tags;
using Huanlin.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrailleToolkit.Helpers
{
    /// <summary>
    /// 提供處理點字詞 (BrailleWord) 的靜態輔助函式。
    /// </summary>
    public static class BrailleWordHelper
    {
        private static void AppendDebugString(StringBuilder sb, string text, string? phoneticCode, string brailleHex)
        {
            sb.Append("{");
            sb.Append($"\"{text}\",");
            sb.Append($"\"{phoneticCode ?? String.Empty}\",");
            sb.Append($"\"{brailleHex}\"");
            sb.Append("},");
        }

        private static void AppendOriginalText(StringBuilder sb, string text, string originalText, bool isContextTag, bool isConvertedFromTag)
        {
            if (isContextTag)
            {
                sb.Append(text);
                return;
            }

            if (isConvertedFromTag)
            {
                return;
            }

            if (!String.IsNullOrEmpty(originalText))
                sb.Append(originalText);
            else
                sb.Append(text);
        }

        /// <summary>
        /// 判斷是否為中文標點符號。
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static bool IsChinesePunctuation(BrailleWord brWord)
        {
            if (brWord == null)
            {
                throw new ArgumentNullException("呼叫 IsChinesePunctuation() 時傳入了 null 參數", nameof(brWord));
            }
            return (BrailleGlobals.ChinesePunctuations.IndexOf(brWord.Text) >= 0);
        }

        /// <summary>
        /// 將 BrailleWord 串列轉換成字串表示（包含 Text, PhoneticCode, CellList）。
        /// </summary>
        /// <param name="brWordList"></param>
        /// <returns></returns>
        public static string ToString(IReadOnlyList<BrailleWord> brWordList)
        {
            return ToString((IReadOnlyList<IBrailleWordView>)brWordList);
        }

        internal static string ToString(IReadOnlyList<IBrailleWordView> brWordList)
        {
            var sb = new StringBuilder();
            foreach (var brWord in brWordList)
            {
                AppendDebugString(sb, brWord.Text, brWord.PhoneticCode, BrailleCellListToHexString(brWord));
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將 BrailleWord 串列轉換成純文字字串。
        /// </summary>
        /// <param name="brWordList"></param>
        /// <returns></returns>
        public static string ToTextString(IReadOnlyList<BrailleWord> brWordList)
        {
            return ToTextString((IReadOnlyList<IBrailleWordView>)brWordList);
        }

        internal static string ToTextString(IReadOnlyList<IBrailleWordView> brWordList)
        {
            var sb = new StringBuilder();
            foreach (var brWord in brWordList)
            {
                sb.Append(brWord.Text);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將 BrailleWord 串列轉換成點位字串。
        /// </summary>
        /// <param name="brWordList"></param>
        /// <returns></returns>
        public static string ToDotNumberString(IReadOnlyList<BrailleWord> brWordList)
        {
            return ToDotNumberString((IReadOnlyList<IBrailleWordView>)brWordList);
        }

        internal static string ToDotNumberString(IReadOnlyList<IBrailleWordView> brWordList)
        {
            var sb = new StringBuilder();
            foreach (var brWord in brWordList)
            {
                sb.Append(BrailleWordViewToDotNumberString(brWord));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 注意：此函式其實無法完全呈現原始的明眼字，因為有些標籤是在轉成點字之後就被刪除了。
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public static string ToOriginalTextString(IReadOnlyList<BrailleWord> words)
        {
            return ToOriginalTextString((IReadOnlyList<IBrailleWordView>)words);
        }

        internal static string ToOriginalTextString(IReadOnlyList<IBrailleWordView> words)
        {
            var sb = new StringBuilder();
            int index = 0;
            while (index < words.Count)
            {
                var brWord = words[index];
                AppendOriginalText(sb, brWord.Text, brWord.OriginalText, brWord.IsContextTag, brWord.IsConvertedFromTag);
                index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 計算 BrailleWord 串列的總方數。
        /// </summary>
        /// <param name="brWordList"></param>
        /// <returns></returns>
        public static int GetCellCount(this IReadOnlyList<BrailleWord> brWordList)
        {
            return GetCellCount((IReadOnlyList<IBrailleWordView>)brWordList);
        }

        internal static int GetCellCount(this IReadOnlyList<IBrailleWordView> brWordList)
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
        public static bool ContainsTitleTag(IReadOnlyList<BrailleWord> brWordList)
        {
            return ContainsTitleTag((IReadOnlyList<IBrailleWordView>)brWordList);
        }

        internal static bool ContainsTitleTag(IReadOnlyList<IBrailleWordView> brWordList)
        {
            if (brWordList.Count > 0 && brWordList[0].Text.Equals(ContextTagNames.Title))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除數字記號（數字符號）。
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns>若有移除則傳回 true。</returns>
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

        private static string BrailleCellListToHexString(IBrailleWordView brWord)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < brWord.CellCount; i++)
            {
                sb.Append(brWord.GetCell(i).ToString());
            }
            return sb.ToString();
        }

        private static string BrailleWordViewToDotNumberString(IBrailleWordView brWord)
        {
            if (brWord.IsContextTag)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();
            sb.Append("(");
            for (int i = 0; i < brWord.CellCount; i++)
            {
                sb.Append(brWord.GetCell(i).ToPositionNumberString());
                sb.Append(" ");
            }

            if (brWord.CellCount > 0)
            {
                sb.Length--;
            }

            sb.Append(")");
            return sb.ToString();
        }
    }
}
