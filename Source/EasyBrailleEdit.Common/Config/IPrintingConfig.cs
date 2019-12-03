using Config.Net;

namespace EasyBrailleEdit.Common.Config
{
    public interface IPrintingConfig
    {
        [Option(DefaultValue = "")]
        string BraillePrinterName { get; set; }

        [Option(DefaultValue = "LPT1")]
        string BraillePrinterPort { get; set; }

        [Option(DefaultValue = "")]
        string DefaultTextPrinter { get; set; }

        [Option(DefaultValue = false)]
        bool PrintBrailleSendPageBreakAtEndOfDoc { get; set; }

        [Option(DefaultValue = true)]
        bool PrintBrailleToBrailler { get; set; }

        [Option(DefaultValue = false)]
        bool PrintBrailleToFile { get; set; }

        [Option(DefaultValue = "")]
        string PrintBrailleToFileName { get; set; }

        [Option(DefaultValue = true)]
        bool PrintPageFoot { get; set; }

        // 一方點字的寬度。此參數會用來計算每個明眼字的字寬（會影響字距）
        [Option(DefaultValue = Constant.DefaultBrailleWidth)]
        double BrailleCellWidth { get; set; }

        [Option(DefaultValue = "新細明體")]
        string PrintTextFontName { get; set; }

        [Option(DefaultValue = Constant.DefaultPrintTextFontSize)]
        double PrintTextFontSize { get; set; }

        [Option(DefaultValue = 40.0975)]
        double PrintTextLineHeight { get; set; }

        [Option(DefaultValue = Constant.DefaultPrintTextMarginLeft)]
        int PrintTextMarginLeft { get; set; }

        [Option(DefaultValue = Constant.DefaultPrintTextMarginTop)]
        int PrintTextMarginTop { get; set; }

        [Option(DefaultValue = Constant.DefaultPrintTextMarginRight)]
        int PrintTextMarginRight { get; set; }

        [Option(DefaultValue = Constant.DefaultPrintTextMarginBottom)]
        int PrintTextMarginBottom { get; set; }

        #region 偶數頁的明眼字列印邊界

        [Option(DefaultValue = Constant.DefaultPrintTextMarginLeft2)]
        int PrintTextMarginLeft2 { get; set; }

        [Option(DefaultValue = Constant.DefaultPrintTextMarginTop2)]
        int PrintTextMarginTop2 { get; set; }

        [Option(DefaultValue = Constant.DefaultPrintTextMarginRight2)]
        int PrintTextMarginRight2 { get; set; }

        [Option(DefaultValue = Constant.DefaultPrintTextMarginBottom2)]
        int PrintTextMarginBottom2 { get; set; }

        #endregion

        [Option(DefaultValue = "")]
        string PrintTextPaperName { get; set; }

        [Option(DefaultValue = "")]
        string PrintTextPaperSourceName { get; set; }
    }
}
