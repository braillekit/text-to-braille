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
        /// 是否優先使用 IFELanguage API 來反查注音字根。預設為 False。
        /// </summary>
        [Option(Alias = "General.PreferIFELanguage", DefaultValue = false)]
        bool PreferIFELanguage { get; set; }

        /// <summary>
        /// 詞庫檔。
        /// </summary>
        [Option(Alias = "General.PhraseFiles", DefaultValue = "")]
        string PhraseFiles { get; set; }

        IBrailleConfig Braille { get; set; }  // 會自動視為區段 [Braille] 的設定

        IPrintingConfig Printing { get; set; } // 會自動視為區段 [Printing] 的設定
    }
}
