using Config.Net;

namespace EasyBrailleEdit.Common.Config
{

    public interface IAppConfig
    {
        // General section

        [Option(Alias = "General.AutoUpdate", DefaultValue = true)]
        bool AutoUpdate { get; set; }

        [Option(Alias = "General.AutoUpdateFilesUrl", DefaultValue = Constant.DefaultAutoUpdateFilesUrl)]
        string AutoUpdateFilesUrl { get; set; }

        /// <summary>
        /// 詞庫檔。
        /// </summary>
        [Option(Alias = "General.PhraseFiles", DefaultValue = "")]
        string PhraseFiles { get; set; }

        /// <summary>
        /// 轉點字之前，先依此屬性的內容來自動替換文字。
        /// </summary>
        [Option(Alias = "General.AutoReplacedText", DefaultValue = "")]
        string AutoReplacedText { get; set; }

        IBrailleConfig Braille { get; set; }  // 會自動視為區段 [Braille] 的設定

        IBrailleEditorConfig BrailleEditor { get; set; }  // 會自動視為區段 [BrailleEditor] 的設定

        IPrintingConfig Printing { get; set; } // 會自動視為區段 [Printing] 的設定
    }
}
