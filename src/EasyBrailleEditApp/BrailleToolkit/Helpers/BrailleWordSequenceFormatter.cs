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
            var sb = new StringBuilder();
            foreach (var brWord in words)
            {
                foreach (var cell in brWord.Cells)
                {
                    sb.Append(cell.ToHexString());
                }
            }
            return sb.ToString();
        }

        internal static string ToBrailleCellHexString(IReadOnlyList<IBrailleWordResult> words)
        {
            var sb = new StringBuilder();
            foreach (var brWord in words)
            {
                var cells = brWord.Cells.Span;
                for (int i = 0; i < cells.Length; i++)
                {
                    sb.Append(cells[i].ToHexString());
                }
            }
            return sb.ToString();
        }

        public static string ToPositionNumberString(IReadOnlyList<BrailleWord> words)
        {
            var sb = new StringBuilder();
            foreach (var brWord in words)
            {
                sb.Append(brWord.ToPositionNumberString(useParenthesis: true));
            }
            return sb.ToString();
        }

        internal static string ToPositionNumberString(IReadOnlyList<IBrailleWordResult> words)
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

        internal static string ToHtmlString(
            IReadOnlyList<IBrailleWordResult> words,
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
