using BrailleToolkit.Tags;

namespace BrailleToolkit
{
    /// <summary>
    /// 點字詞的唯讀檢視介面。
    /// </summary>
    internal interface IBrailleWordView
    {
        string Text { get; }
        string OriginalText { get; }
        BrailleLanguage Language { get; }
        int CellCount { get; }
        BrailleCell GetCell(int index);
        string? PhoneticCode { get; }
        bool IsPolyphonic { get; }
        bool DontBreakLineHere { get; }
        string ContextNames { get; }
        IContextTag? ContextTag { get; }
        bool IsContextTag { get; }
        bool IsConvertedFromTag { get; }
        bool NoDigitCell { get; }
        bool NoSpace { get; }
        bool NoCapitalRule { get; }
        bool IsEngPhonetic { get; }
    }
}
