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

        static string ClipboardObjectFormat = typeof(BrailleWord).FullName + "_List";

        public static void SetData(List<BrailleWord> words)
        {
            var s = JsonHelper.Serialize(words);

            Clipboard.Clear();
            Clipboard.SetData(ClipboardObjectFormat, s);

            // 注意：這裡不使用 Clipboard 內建的序列化，是因為它會遺漏 BrailleWord 的 PhoneticCode 屬性
            //Clipboard.SetData(ClipboardObjectFormat, words);
        }

        public static List<BrailleWord> GetData()
        {
            List<BrailleWord> result = null;
            if (Clipboard.ContainsData(ClipboardObjectFormat))
            {
                var s = (string) Clipboard.GetData(ClipboardObjectFormat);
                result = JsonHelper.Deserialize<List<BrailleWord>>(s);
            }
            return result;
        }

        public static void ClearData()
        {
            if (Clipboard.ContainsData(ClipboardObjectFormat))
            {
                Clipboard.Clear();
            }
        }

    }
}
