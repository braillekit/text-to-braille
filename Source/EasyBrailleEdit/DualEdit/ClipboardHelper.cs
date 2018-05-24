using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrailleToolkit;
using SourceGrid;

namespace EasyBrailleEdit.DualEdit
{
    internal static class ClipboardHelper
    {

        static string ClipboardObjectFormat = typeof(BrailleWord).FullName + "_List";

        public static void SetData(List<BrailleWord> words)
        {
            Clipboard.Clear();
            Clipboard.SetData(ClipboardObjectFormat, words);
        }

        public static List<BrailleWord> GetData()
        {
            List<BrailleWord> result = null;
            if (Clipboard.ContainsData(ClipboardObjectFormat))
            {
                result = (List<BrailleWord>) Clipboard.GetData(ClipboardObjectFormat);
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
