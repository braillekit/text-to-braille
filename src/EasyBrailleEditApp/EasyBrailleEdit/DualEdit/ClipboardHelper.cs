using BrailleToolkit;
using EasyBrailleEdit.Common.Utilities;

namespace EasyBrailleEdit.DualEdit
{
    internal static class ClipboardHelper
    {

        static string ClipboardObjectFormatForWords = typeof(BrailleWord).FullName + "_List";
        static string ClipboardObjectFormatForLines = typeof(BrailleLine).FullName + "_List";

        public static void SetWords(IReadOnlyList<BrailleWord> brWords)
        {
            var words = new List<BrailleWord>();
            foreach (var brWord in brWords)
            {
                words.Add(brWord);
            }

            var s = JsonHelper.Serialize(words);

            Clipboard.Clear();
            Clipboard.SetData(ClipboardObjectFormatForWords, s);

            // 注意：這裡不使用 Clipboard 內建的序列化，是因為它會遺漏 BrailleWord 的 PhoneticCode 屬性
            //Clipboard.SetData(ClipboardObjectFormat, brLines);
        }


        public static void SetLines(IReadOnlyList<BrailleLine> brLines)
        {
            var lines = new List<BrailleLine>();
            foreach (var brLine in brLines)
            {
                lines.Add(brLine);
            }

            var s = JsonHelper.Serialize(lines);

            Clipboard.Clear();
            Clipboard.SetData(ClipboardObjectFormatForLines, s);

            // 注意：這裡不使用 Clipboard 內建的序列化，是因為它會遺漏 BrailleWord 的 PhoneticCode 屬性
            //Clipboard.SetData(ClipboardObjectFormat, brLines);
        }

        public static List<BrailleWord>? GetWords()
        {
            List<BrailleWord>? result = null;
            if (Clipboard.TryGetData(ClipboardObjectFormatForWords, out string? data))
            {
                if (data != null)
                {
                    result = JsonHelper.Deserialize<List<BrailleWord>>(data);
                }
            }
            return result;
        }

        public static List<BrailleLine>? GetLines()
        {
            List<BrailleLine>? result = null;
            if (Clipboard.TryGetData(ClipboardObjectFormatForLines, out string? data))
            {
                if (data != null)
                {
                    result = JsonHelper.Deserialize<List<BrailleLine>>(data);
                }
            }
            return result;
        }


        public static void ClearData()
        {
            if (Clipboard.ContainsData(ClipboardObjectFormatForLines) ||
                Clipboard.ContainsData(ClipboardObjectFormatForWords))
            {
                Clipboard.Clear();
            }
        }

    }
}
