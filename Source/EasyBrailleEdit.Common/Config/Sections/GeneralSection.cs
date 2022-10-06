using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Common.Config.Sections
{
    public sealed record GeneralSection
    {
        public const string Name = "General";

        public bool AutoUpdate { get; set; } = true;

        public string AutoUpdateFilesUrl { get; set; } = Constant.DefaultAutoUpdateFilesUrl;

        /// <summary>
        /// 詞庫檔。
        /// </summary>
        public string PhraseFiles { get; set; } = String.Empty;

        /// <summary>
        /// 轉點字之前，先依此屬性的內容來自動替換文字。
        /// </summary>
        public string AutoReplacedText { get; set; } = String.Empty;

    }
}
