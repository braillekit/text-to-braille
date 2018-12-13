namespace EasyBrailleEdit.Common
{
    public static class Constant
    {
        public const string AppName = "EasyBrailleEdit";
        public const string FacebookPageUrl = "https://www.facebook.com/easybraille/";

        public const string UsersLicenseFileUrl = "https://raw.githubusercontent.com/huanlin/easy-braille-edit/forblind/users.lic";
        public const string UsersLicenseFileName = "users.lic";
        public const string SerialNumberRegKey = "SerialNumber";
        public const string CustomerNameRegKey = "Customer";
        public const string ExpiredDateRegKey = "ExpiredDate";

        public static class ProductBranches
        {
            public const string Community = "master";         // 社群版  
            public const string TaipeiForBlind = "forblind";  // 台北市視障者家長協會
        }

        // 預設一行最大方數
        public const int DefaultCellsPerLine = 40;
        public const int DefaultLinesPerPage = 25;

        // 雙視編輯器的預設最大可復原操作數量
        public const int DefaultMaxUndoLevel = 10;

        public static class Files
        {
            public const string DefaultMainBrailleFileExt = ".brx";    // 預設的點字檔副檔名 (v1為 .btx；v2 為 .brlj)
            public const string DefaultPrintableBrailleFileExt = ".brp"; // p 代表 print

            // 暫存檔案
            public const string CvtInputTempFileName = "cvt_in.tmp";            // 輸入的明眼字檔
            public const string CvtInputPhraseListFileName = "cvt_in_phrase.tmp";   // 輸入的詞庫設定檔
            public const string CvtOutputTempFileName = "cvt_out.tmp";          // 輸出的點字檔
            public const string CvtErrorCharFileName = "cvt_errchar.tmp";       // 儲存轉換失敗的字元資訊
            public const string CvtResultFileName = "cvt_result.tmp";   // 儲存成功或失敗的旗號以及錯誤訊息

            public static string MainFileNameFilter = $"雙視檔案 3.x 版 (*{DefaultMainBrailleFileExt})|*{DefaultMainBrailleFileExt}|雙視檔案 2.x 版 (*.brlj)|*.brlj|所有檔案|*.*";
            public static string SaveAsFileNameFilter = $"雙視檔案 (*{DefaultMainBrailleFileExt})|*{DefaultMainBrailleFileExt}";

            public static string SavePrintableBrailleFileNameFilter = $"用於列印的點字檔案(*{DefaultPrintableBrailleFileExt})|*{DefaultPrintableBrailleFileExt}";
        }

        // 預設明眼字列印邊界
        public const int DefaultPrintTextMarginLeft = 105;
        public const int DefaultPrintTextMarginTop = 12;
        public const int DefaultPrintTextMarginRight = 150;
        public const int DefaultPrintTextMarginBottom = 100;

        // 預設的偶數頁明眼字列印邊界
        public const int DefaultPrintTextMarginLeft2 = 105;
        public const int DefaultPrintTextMarginTop2 = 15; 
        public const int DefaultPrintTextMarginRight2 = 150;
        public const int DefaultPrintTextMarginBottom2 = 100;

        public const double DefaultPrintTextFontSize = 12;      // 預設明眼字字型大小
        public const double DefaultPrintTextLineHeight = 40.0975;  // 明眼字列高（文字高度+列距）
        public const double DefaultPrintTextLineSpace = 20.9;   // 預設明眼字列距

        public const string DefaultAutoUpdateFilesUrl = "https://raw.githubusercontent.com/huanlin/easy-braille-edit/forblind/UpdateFiles/";

        // 與轉點字規則有關的常數
        public const string DefaultNoSpaceAfterTheseCharacters = "「『“‘…";
        public const string DefaultNoSpaceBeforeTheseCharacters = "，。？！：；、…」』”’）";
    }
}
