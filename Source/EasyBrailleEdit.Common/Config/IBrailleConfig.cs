using Config.Net;

namespace EasyBrailleEdit.Common.Config
{
    public interface IBrailleConfig
    {
        [Option(DefaultValue = Constant.DefaultCellsPerLine)]
        int CellsPerLine { get; set; }

        [Option(DefaultValue = Constant.DefaultLinesPerPage)]
        int LinesPerPage { get; set; }

        /// <summary>
        /// 以 '#' 開頭的編號項目是否要在折行時自動內縮一方。
        /// </summary>
        [Option(DefaultValue = false)]
        bool AutoIndentNumberedLine { get; set; }

        /// <summary>
        /// 容易判斷錯誤的破音字，這些中文字在雙視編輯視窗中會以紅色顯示，以提醒使用者注意。
        /// </summary>
        [Option(DefaultValue = "為")]
        string ErrorProneWords { get; set; }

        /// <summary>
        /// ; 原書頁碼的數字都使用上位點，且不加數符。
        /// </summary>
        [Option(DefaultValue = true)]
        bool UseUpperPositionForOrgPageNumber { get; set; }

        /// <summary>
        /// 這些字元的右邊一律不加空方。
        /// </summary>
        [Option(DefaultValue = Constant.DefaultNoSpaceAfterTheseCharacters)]
        string NoSpaceAfterTheseCharacters { get; set; }

        /// <summary>
        /// 這些字元的左邊一律不加空方。
        /// </summary>
        [Option(DefaultValue = Constant.DefaultNoSpaceBeforeTheseCharacters)]
        string NoSpaceBeforeTheseCharacters { get; set; }
    }
}
