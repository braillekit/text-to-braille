namespace BrailleToolkit
{
    public static class BrailleConst
    {
        public const int DefaultCellsPerLine = 40;

        public static class DisplayText
        {
            // 注意：必須與 Data/ChineseBrailleTable.xml 的內容一致。

            public const string SpecificName = "╴╴";
            public const string BookName = "﹏﹏";
            public const string BrailleTranslatorNotePrefix = "（★";
            public const string BrailleTranslatorNotePostfix = "）";
        }
    }
}
