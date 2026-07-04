using System.Collections.Generic;

namespace BrailleToolkit
{
    /// <summary>
    /// 點字列的唯讀檢視介面。
    /// </summary>
    internal interface IBrailleLineView
    {
        long Identity { get; }
        IReadOnlyList<BrailleWord> Words { get; }
        int WordCount { get; }
        int CellCount { get; }
        object? Tag { get; }
        BrailleWord this[int index] { get; }
    }
}
