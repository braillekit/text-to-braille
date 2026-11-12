using System;
using System.Collections.Generic;
using System.Text;
using BrailleToolkit.Converters;

namespace BrailleToolkit.Helpers
{
    /// <summary>
    /// 提供點字詞序列的唯讀格式化功能。
    /// </summary>
    internal static class BrailleWordSequenceFormatter
    {
        public static string ToBrailleCellHexString(IReadOnlyList<BrailleWord> words)
        {
            return ToBrailleCellHexString((IReadOnlyList<IBrailleWordView>)words);
        }

        internal static string ToBrailleCellHexString(IReadOnlyList<IBrailleWordView> words)
        {
            var sb = new StringBuilder();
            foreach (var brWord in words)
            {
                for (int i = 0; i < brWord.CellCount; i++)
                {
                    sb.Append(brWord.GetCell(i).ToHexString());
                }
            }
            return sb.ToString();
        }

        public static string ToPositionNumberString(IReadOnlyList<BrailleWord> words)
        {
            return ToPositionNumberString((IReadOnlyList<IBrailleWordView>)words);
        }

        internal static string ToPositionNumberString(IReadOnlyList<IBrailleWordView> words)
        {
            return BrailleWordHelper.ToDotNumberString(words);
        }

        public static string ToHtmlString(
            IReadOnlyList<BrailleWord> words,
            string leadingSpaces,
            string cssClassTd,
            string cssClassBraille,
            string cssClassText)
        {
            return ToHtmlString((IReadOnlyList<IBrailleWordView>)words, leadingSpaces, cssClassTd, cssClassBraille, cssClassText);
        }

        internal static string ToHtmlString(
            IReadOnlyList<IBrailleWordView> words,
            string leadingSpaces,
            string cssClassTd,
            string cssClassBraille,
            string cssClassText)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{leadingSpaces}<tr>");

            foreach (var brWord in words)
            {
                if (brWord.IsContextTag || brWord.CellCount < 1)
                    continue;

                string brFontText = BrailleFontConverter.ToString(brWord);

                if (String.IsNullOrEmpty(brFontText))
                {
                    sb.AppendLine($"無法轉換成對應的點字字型: {brWord.Text}。");
                    break;
                }

                sb.AppendLine($"{leadingSpaces}  <td colspan='{brFontText.Length}' class='{cssClassTd}'>");
                sb.AppendLine($"{leadingSpaces}    <div class='{cssClassBraille}'>{brFontText}</div>");
                sb.AppendLine($"{leadingSpaces}    <div class='{cssClassText}'>{brWord.Text}</div>");
                sb.AppendLine($"{leadingSpaces}  </td>");
            }

            sb.AppendLine($"{leadingSpaces}</tr>");
            return sb.ToString();
        }
    }
}
