using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Common.Config.Sections
{
    /// <summary>
    /// General configuration section.
    /// </summary>
    public sealed record GeneralSection
    {
        /// <summary>
        /// The section name.
        /// </summary>
        public const string Name = "General";

        /// <summary>
        /// Gets or sets a value indicating whether to enable auto-update.
        /// </summary>
        public bool AutoUpdate { get; set; } = true;

        /// <summary>
        /// Gets or sets the URL for auto-update files.
        /// </summary>
        public string AutoUpdateFilesUrl { get; set; } = Constant.DefaultAutoUpdateFilesUrl;

        /// <summary>
        /// 詞庫檔。
        /// </summary>
        public string PhraseFiles { get; set; } = String.Empty;

        /// <summary>
        /// 轉點字之前，先依此屬性的內容來自動替換文字。
        /// </summary>
        public string AutoReplacedText { get; set; } = String.Empty;

        /// <summary>
        /// 是否使用全形輸入法（預設 false）。
        /// </summary>
        public bool UseFullWidthIme { get; set; } = false;
    }
}
