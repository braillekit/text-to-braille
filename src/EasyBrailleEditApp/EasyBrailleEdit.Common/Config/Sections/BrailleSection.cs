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
        /// <summary>
        /// The section name.
        /// </summary>
        public const string Name = "Braille";

        /// <summary>
        /// Gets or sets the number of cells per line.
        /// </summary>
        public int CellsPerLine { get; set; } = Constant.DefaultCellsPerLine;

        /// <summary>
        /// Gets or sets the number of lines per page.
        /// </summary>
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
        /// 原書頁碼的數字都使用上位點，且不加數符。
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

        /// <summary>
        /// 數字一律使用上位點。v4.2.1 (2023-9-1) 之後開始支援此選項。
        /// </summary>
        public bool UseUpperPositionForNumbers {  get; set; } = false;

        /// <summary>
        /// 是否使用內建轉換（預設 true）
        /// </summary>
        public bool UseInProcessConversion { get; set; } = true;

        /// <summary>
        /// 即時預覽的自動更新延遲時間（毫秒），預設 1500ms。
        /// </summary>
        public int AutoPreviewDelay { get; set; } = 1500;

        /// <summary>
        /// 是否預設啟用即時預覽（預設 true）。
        /// </summary>
        public bool EnableInstantPreview { get; set; } = true;

        /// <summary>
        /// 即時預覽時，要顯示游標所在位置前後幾行的內容（預設 3 行）。
        /// </summary>
        public int PreviewContextLines { get; set; } = 3;
    }

}
