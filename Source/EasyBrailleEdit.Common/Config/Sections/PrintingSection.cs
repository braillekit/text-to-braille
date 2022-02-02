using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Common.Config.Sections
{
    /// <summary>
    /// 區段 [Printing]
    /// </summary>
    public record PrintingSection
    {
        public const string Name = "Printing";

        public string BraillePrinterName { get; set; } = String.Empty;
        public string BraillePrinterPort { get; set; } = "LPT1";
        public string DefaultTextPrinter { get; set; } = String.Empty;
        public bool PrintBrailleSendPageBreakAtEndOfDoc { get; set; } = false;
        public bool PrintBrailleToBrailler { get; set; } = true;
        public bool PrintBrailleToFile { get; set; } = false;
        public string PrintBrailleToFileName { get; set; } = String.Empty;
        public bool PrintPageFoot { get; set; } = true;

        // 一方點字的寬度。此參數會用來計算每個明眼字的字寬（會影響字距）
        public double BrailleCellWidth { get; set; } = Constant.DefaultBrailleWidth;

        public string PrintTextFontName { get; set; } = Constant.DefaultPrintTextFontName;

        //[Option(DefaultValue = Constant.DefaultPrintTextFontSize)]
        public double PrintTextFontSize { get; set; } = Constant.DefaultPrintTextFontSize;

        public double PrintTextLineHeight { get; set; } = Constant.DefaultPrintTextLineHeight;

        public int PrintTextMarginLeft { get; set; } = Constant.DefaultPrintTextMarginLeft;

        public int PrintTextMarginTop { get; set; } = Constant.DefaultPrintTextMarginTop;

        public int PrintTextMarginRight { get; set; } = Constant.DefaultPrintTextMarginRight;

        public int PrintTextMarginBottom { get; set; } = Constant.DefaultPrintTextMarginBottom;

        #region 偶數頁的明眼字列印邊界

        public int PrintTextMarginLeft2 { get; set; } = Constant.DefaultPrintTextMarginLeft2;

        public int PrintTextMarginTop2 { get; set; } = Constant.DefaultPrintTextMarginTop2;

        public int PrintTextMarginRight2 { get; set; } = Constant.DefaultPrintTextMarginRight2;

        public int PrintTextMarginBottom2 { get; set; } = Constant.DefaultPrintTextMarginBottom2;

        #endregion

        public string PrintTextPaperName { get; set; } = string.Empty;

        public string PrintTextPaperSourceName { get; set; } = string.Empty;
    }

}
