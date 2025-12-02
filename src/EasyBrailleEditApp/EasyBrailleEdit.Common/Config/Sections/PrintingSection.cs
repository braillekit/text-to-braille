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
    public sealed record PrintingSection
    {
        /// <summary>
        /// The section name.
        /// </summary>
        public const string Name = "Printing";

        /// <summary>
        /// Gets or sets the braille printer name.
        /// </summary>
        public string BraillePrinterName { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the braille printer port.
        /// </summary>
        public string BraillePrinterPort { get; set; } = "LPT1";

        /// <summary>
        /// Gets or sets the default text printer.
        /// </summary>
        public string DefaultTextPrinter { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to send page break at end of document.
        /// </summary>
        public bool PrintBrailleSendPageBreakAtEndOfDoc { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to print braille to brailler.
        /// </summary>
        public bool PrintBrailleToBrailler { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to print braille to file.
        /// </summary>
        public bool PrintBrailleToFile { get; set; } = false;

        /// <summary>
        /// Gets or sets the filename for braille output.
        /// </summary>
        public string PrintBrailleToFileName { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to print page footer.
        /// </summary>
        public bool PrintPageFoot { get; set; } = true;

        /// <summary>
        /// Gets or sets the braille cell width.
        /// </summary>
        public double BrailleCellWidth { get; set; } = Constant.DefaultBrailleWidth;

        /// <summary>
        /// Gets or sets the paper name for text printing.
        /// </summary>
        public string PrintTextPaperName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the paper source name for text printing.
        /// </summary>
        public string PrintTextPaperSourceName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the font name for text printing.
        /// </summary>
        public string PrintTextFontName { get; set; } = Constant.DefaultPrintTextFontName;

        /// <summary>
        /// Gets or sets the font size for text printing.
        /// </summary>
        public double PrintTextFontSize { get; set; } = Constant.DefaultPrintTextFontSize;

        /// <summary>
        /// Gets or sets the line height for text printing.
        /// </summary>
        public double PrintTextLineHeight { get; set; } = Constant.DefaultPrintTextLineHeight;

        /// <summary>
        /// Gets or sets the left margin for text printing.
        /// </summary>
        public int PrintTextMarginLeft { get; set; } = Constant.DefaultPrintTextMarginLeft;

        /// <summary>
        /// Gets or sets the top margin for text printing.
        /// </summary>
        public int PrintTextMarginTop { get; set; } = Constant.DefaultPrintTextMarginTop;

        /// <summary>
        /// Gets or sets the right margin for text printing.
        /// </summary>
        public int PrintTextMarginRight { get; set; } = Constant.DefaultPrintTextMarginRight;

        /// <summary>
        /// Gets or sets the bottom margin for text printing.
        /// </summary>
        public int PrintTextMarginBottom { get; set; } = Constant.DefaultPrintTextMarginBottom;

        #region 偶數頁的明眼字列印邊界

        /// <summary>
        /// Gets or sets the left margin for even page text printing.
        /// </summary>
        public int PrintTextMarginLeft2 { get; set; } = Constant.DefaultPrintTextMarginLeft2;

        /// <summary>
        /// Gets or sets the top margin for even page text printing.
        /// </summary>
        public int PrintTextMarginTop2 { get; set; } = Constant.DefaultPrintTextMarginTop2;

        /// <summary>
        /// Gets or sets the right margin for even page text printing.
        /// </summary>
        public int PrintTextMarginRight2 { get; set; } = Constant.DefaultPrintTextMarginRight2;

        /// <summary>
        /// Gets or sets the bottom margin for even page text printing.
        /// </summary>
        public int PrintTextMarginBottom2 { get; set; } = Constant.DefaultPrintTextMarginBottom2;

        #endregion

    }

}
