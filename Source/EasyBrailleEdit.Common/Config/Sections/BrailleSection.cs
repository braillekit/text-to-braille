using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Common.Config.Sections
{
    /// <summary>
    /// 區段 [Braille]
    /// </summary>
    public sealed record BrailleSection
    {
        public const string Name = "Braille";

        public int CellsPerLine { get; set; } = Constant.DefaultCellsPerLine;

        public int LinesPerPage { get; set; } = Constant.DefaultLinesPerPage;

        /// <summary>
        /// 以 '#' 開頭的編號項目是否要在折行時自動內縮一方。
        /// </summary>
        public bool AutoIndentNumberedLine { get; set; } = false;

        /// <summary>
        /// 容易判斷錯誤的破音字，這些中文字在雙視編輯視窗中會以紅色顯示，以提醒使用者注意。
        /// </summary>
        public string ErrorProneWords { get; set; } = "為";

        /// <summary>
        /// ; 原書頁碼的數字都使用上位點，且不加數符。
        /// </summary>
        public bool UseUpperPositionForOrgPageNumber { get; set; } = true;

        /// <summary>
        /// 這些字元的右邊一律不加空方。
        /// </summary>
        public string NoSpaceAfterTheseCharacters { get; set; } = Constant.DefaultNoSpaceAfterTheseCharacters;

        /// <summary>
        /// 這些字元的左邊一律不加空方。
        /// </summary>
        public string NoSpaceBeforeTheseCharacters { get; set; } = Constant.DefaultNoSpaceBeforeTheseCharacters;
    }

}
