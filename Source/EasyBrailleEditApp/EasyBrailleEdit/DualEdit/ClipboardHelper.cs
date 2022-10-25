using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrailleToolkit;
using Huanlin.Common.Helpers;
using SourceGrid;

namespace EasyBrailleEdit.DualEdit
{
    internal static class ClipboardHelper
    {

        static string ClipboardObjectFormatForWords = typeof(BrailleWord).FullName + "_List";
        static string ClipboardObjectFormatForLines = typeof(BrailleLine).FullName + "_List";

        public static void SetWords(List<BrailleWord> brWords)
        {
            var s = JsonHelper.Serialize(brWords);

            Clipboard.Clear();
            Clipboard.SetData(ClipboardObjectFormatForWords, s);

            // 注意：這裡不使用 Clipboard 內建的序列化，是因為它會遺漏 BrailleWord 的 PhoneticCode 屬性
            //Clipboard.SetData(ClipboardObjectFormat, brLines);
        }


        public static void SetLines(List<BrailleLine> brLines)
        {
            var s = JsonHelper.Serialize(brLines);

            Clipboard.Clear();
            Clipboard.SetData(ClipboardObjectFormatForLines, s);

            // 注意：這裡不使用 Clipboard 內建的序列化，是因為它會遺漏 BrailleWord 的 PhoneticCode 屬性
            //Clipboard.SetData(ClipboardObjectFormat, brLines);
        }

        public static List<BrailleWord> GetWords()
        {
            List<BrailleWord> result = null;
            if (Clipboard.ContainsData(ClipboardObjectFormatForWords))
            {
                var s = (string) Clipboard.GetData(ClipboardObjectFormatForWords);
                result = JsonHelper.Deserialize<List<BrailleWord>>(s);
            }
            return result;
        }

        public static List<BrailleLine> GetLines()
        {
            List<BrailleLine> result = null;
            if (Clipboard.ContainsData(ClipboardObjectFormatForLines))
            {
                var s = (string)Clipboard.GetData(ClipboardObjectFormatForLines);
                result = JsonHelper.Deserialize<List<BrailleLine>>(s);
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
