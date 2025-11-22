namespace EasyBrailleEdit.Common;

/// <summary>
/// Provides application-wide constants.
/// </summary>
public static class Constant
{
    /// <summary>
    /// Gets the application name.
    /// </summary>
    public const string AppName = "EasyBrailleEdit";

    /// <summary>
    /// Gets the product version name.
    /// </summary>
    public const string ProductVersionName = "開源社群版";

    /// <summary>
    /// Gets the project URL.
    /// </summary>
    public const string ProjectUrl = "https://github.com/braillekit/text-to-braille";

    /// <summary>
    /// Gets the Facebook page URL.
    /// </summary>
    public const string FacebookPageUrl = "https://www.facebook.com/easybraille/";
    
    /// <summary>
    /// Gets the default auto-update root URL.
    /// </summary>
    public const string DefaultAutoUpdateRootUrl = "https://raw.githubusercontent.com/braillekit/text-to-braille-updates/refs/heads/main/";

    /// <summary>
    /// Gets the default auto-update files URL.
    /// </summary>
    public const string DefaultAutoUpdateFilesUrl = DefaultAutoUpdateRootUrl + "Files/";

    /// <summary>
    /// Gets the default auto-update file list name.
    /// </summary>
    public const string DefaultAutoUpdateFileListName = "_updates.txt";

    /// <summary>
    /// Gets the change log file name.
    /// </summary>
    public const string ChangeLogFileName = "ChangeLog.md";

    /// <summary>
    /// 預設一行最大方數
    /// </summary>
    public const int DefaultCellsPerLine = 40;

    /// <summary>
    /// Gets the default number of lines per page.
    /// </summary>
    public const int DefaultLinesPerPage = 25;

    /// <summary>
    /// 預設一方點字的寬度。此參數會用來計算每個明眼字的字寬（會影響字距）
    /// </summary>
    public const double DefaultBrailleWidth = 24;

    /// <summary>
    /// 雙視編輯器的預設最大可復原操作數量
    /// </summary>
    public const int DefaultMaxUndoLevel = 10;

    /// <summary>
    /// 提供與檔案相關的常數。
    /// </summary>
    public static class Files
    {
        /// <summary>
        /// 預設的點字檔副檔名 (v1為 .btx；v2 為 .brlj)
        /// </summary>
        public const string JsonBrailleFileExt = ".brx";
        
        /// <summary>
        /// 雙視檔案 YAML 格式
        /// </summary>
        public const string YamlBrailleFileExt = ".byml";
        
        /// <summary>
        /// p 代表 print
        /// </summary>
        public const string PrintableBrailleFileExt = ".brp";

        // 暫存檔案
        /// <summary>
        /// 輸入的明眼字檔
        /// </summary>
        public const string CvtInputTempFileName = "cvt_in.tmp";
        
        /// <summary>
        /// 輸入的詞庫設定檔
        /// </summary>
        public const string CvtInputPhraseListFileName = "cvt_in_phrase.tmp";
        
        /// <summary>
        /// 輸出的點字檔
        /// </summary>
        public const string CvtOutputTempFileName = "cvt_out.tmp";
        
        /// <summary>
        /// 儲存轉換失敗的字元資訊
        /// </summary>
        public const string CvtErrorCharFileName = "cvt_errchar.tmp";
        
        /// <summary>
        /// 儲存成功或失敗的旗號以及錯誤訊息
        /// </summary>
        public const string CvtResultFileName = "cvt_result.tmp";

        /// <summary>
        /// 主要的「開啟檔案」對話方塊中使用的檔案過濾器字串。
        /// </summary>
        public static string MainFileNameFilter = $"雙視檔案 YAML 格式 (*{YamlBrailleFileExt})|*{YamlBrailleFileExt}|雙視檔案 3.x 版 (*{JsonBrailleFileExt})|*{JsonBrailleFileExt}|雙視檔案 2.x 版 (*.brlj)|*.brlj|所有檔案|*.*";
        
        /// <summary>
        /// 「另存新檔」對話方塊中使用的檔案過濾器字串。
        /// </summary>
        public static string SaveAsFileNameFilter = $"雙視檔案 YAML 格式 (*{YamlBrailleFileExt})|*{YamlBrailleFileExt}|雙視檔案 (*{JsonBrailleFileExt})|*{JsonBrailleFileExt}";

        /// <summary>
        /// 「儲存可列印點字檔」對話方塊中使用的檔案過濾器字串。
        /// </summary>
        public static string SavePrintableBrailleFileNameFilter = $"用於列印的點字檔案(*{PrintableBrailleFileExt})|*{PrintableBrailleFileExt}";
    }

    /// <summary>
    /// 預設奇數頁明眼字列印左邊界。
    /// </summary>
    public const int DefaultPrintTextMarginLeft = 105;

    /// <summary>
    /// 預設奇數頁明眼字列印上邊界。
    /// </summary>
    public const int DefaultPrintTextMarginTop = 12;

    /// <summary>
    /// 預設奇數頁明眼字列印右邊界。
    /// </summary>
    public const int DefaultPrintTextMarginRight = 150;

    /// <summary>
    /// 預設奇數頁明眼字列印下邊界。
    /// </summary>
    public const int DefaultPrintTextMarginBottom = 100;

    /// <summary>
    /// 預設偶數頁明眼字列印左邊界。
    /// </summary>
    public const int DefaultPrintTextMarginLeft2 = 105;

    /// <summary>
    /// 預設偶數頁明眼字列印上邊界。
    /// </summary>
    public const int DefaultPrintTextMarginTop2 = 15; 

    /// <summary>
    /// 預設偶數頁明眼字列印右邊界。
    /// </summary>
    public const int DefaultPrintTextMarginRight2 = 150;

    /// <summary>
    /// 預設偶數頁明眼字列印下邊界。
    /// </summary>
    public const int DefaultPrintTextMarginBottom2 = 100;

    /// <summary>
    /// 預設明眼字字型名稱。
    /// </summary>
    public const string DefaultPrintTextFontName = "新細明體";
    
    /// <summary>
    /// 預設明眼字字型大小
    /// </summary>
    public const double DefaultPrintTextFontSize = 12;
    
    /// <summary>
    /// 明眼字列高（文字高度+列距）
    /// </summary>
    public const double DefaultPrintTextLineHeight = 40.0975;
    
    /// <summary>
    /// 預設明眼字列距
    /// </summary>
    public const double DefaultPrintTextLineSpace = 20.9;

    // 與轉點字規則有關的常數

    /// <summary>
    /// Gets the default characters that should not have a space after them.
    /// </summary>
    public const string DefaultNoSpaceAfterTheseCharacters = "「『\"'…";

    /// <summary>
    /// Gets the default characters that should not have a space before them.
    /// </summary>
    public const string DefaultNoSpaceBeforeTheseCharacters = "，。？！：；、…」』\"'）";
}
