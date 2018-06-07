using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.DualEdit
{

    public enum ViewMode { All, BrailleOnly, TextAndZhuyin };

    /// <summary>
    /// 儲存格內容修改的情況。
    /// </summary>
    internal enum CellChangedType
    {
        None,       // 完全沒有變動。
        Text,       // 修改了明眼字。
        Phonetic,   // 只修改點字的注音碼。
        Braille     // 修改點字。
    };

    internal interface IBrailleGridForm
    {
        string StatusText { get; set; }
        int StatusProgress { get; set; }
        string CurrentWordStatusText { get; set; }
        string CurrentLineStatusText { get; set; }
        string CurrentPageTitleStatusText { get; set; }
        string PageNumberText { get; set; }
    }
}
