namespace BrailleToolkit
{
    public static class BrailleConst
    {
        /// <summary>
        /// 預設每列方數。
        /// </summary>
        public const int DefaultCellsPerLine = 40;

        /// <summary>
        /// 顯示文字常數。
        /// </summary>
        public static class DisplayText
        {
            // 注意：必須與 Data/TwChineseBrailleTable.xml 的內容一致。

            /// <summary>
            /// 私名號
            /// </summary>
            public const string SpecificName = "╴╴";
            
            /// <summary>
            /// 書名號
            /// </summary>
            public const string BookName = "﹏﹏";
            
            /// <summary>
            /// 點字翻譯者註解前置詞
            /// </summary>
            public const string BrailleTranslatorNotePrefix = "（★";
            
            /// <summary>
            /// 點字翻譯者註解後置詞
            /// </summary>
            public const string BrailleTranslatorNotePostfix = "）";
            
            /// <summary>
            /// 刪除開始
            /// </summary>
            public const string DeleteBegin = "▲";
            
            /// <summary>
            /// 刪除結束
            /// </summary>
            public const string DeleteEnd = "▼";
        }
    }
}
