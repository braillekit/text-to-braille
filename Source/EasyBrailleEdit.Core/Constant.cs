﻿namespace EasyBrailleEdit.Core
{
    public static class Constant
    {
        public const string AppName = "EasyBrailleEdit";

        // 預設一行最大方數
        public const int DefaultCellsPerLine = 40;
        public const int DefaultLinesPerPage = 25;

        public static class Files
        {
            public const string DefaultBrailleFileExt = ".brlj";    // 預設的點字檔副檔名 (舊版為 .btx)

            // 暫存檔案
            public const string CvtInputTempFileName = "cvt_in.tmp";            // 輸入的明眼字檔
            public const string CvtInputPhraseListFileName = "cvt_in_phrase.tmp";   // 輸入的詞庫設定檔
            public const string CvtOutputTempFileName = "cvt_out.tmp";          // 輸出的點字檔
            public const string CvtErrorCharFileName = "cvt_errchar.tmp";       // 儲存轉換失敗的字元資訊
            public const string CvtResultFileName = "cvt_result.tmp";   // 儲存成功或失敗的旗號以及錯誤訊息

            public const string FileNameFilter = "雙視檔案 1.x 版 (*.btx)|*.btx|雙視檔案 2.x 版 (*.brlj)|*.brlj";
            public const int FileNameFilterIndex = 2;
            public const string SaveAsFileNameFilter = "雙視檔案 2.x 版 (*.brlj)|*.brlj";
            public const int SaveAsFileNameFilterIndex = 1;
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

        public const string DefaultAutoUpdateFilesUrl = "https://raw.githubusercontent.com/huanlin/EasyBrailleEdit/master/UpdateFiles/";
    }
}
