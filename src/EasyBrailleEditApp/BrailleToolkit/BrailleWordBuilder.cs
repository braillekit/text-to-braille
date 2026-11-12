using System;
using System.Runtime.InteropServices;
using BrailleToolkit.Tags;
using Huanlin.Common.Helpers;

namespace BrailleToolkit
{
    /// <summary>
    /// 已 materialize 的點字詞唯讀介面。
    /// </summary>
    internal interface IBrailleWordResult : IBrailleWordView
    {
        ReadOnlyMemory<BrailleCell> Cells { get; }

        BrailleWord ToBrailleWord();
    }

    /// <summary>
    /// Builder 階段使用的點字詞物件。
    /// </summary>
    internal sealed class BrailleWordBuilder
    {
        private BrailleCellBuffer _cells;
        private bool _isContextTag;

        public BrailleWordBuilder(string text)
        {
            Text = text ?? String.Empty;
            OriginalText = Text;
            ContextNames = String.Empty;
            Language = BrailleLanguage.Neutral;
            _cells = new BrailleCellBuffer();
        }

        public static BrailleWordBuilder FromBrailleWord(BrailleWord source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var builder = new BrailleWordBuilder(source.Text)
            {
                OriginalText = source.OriginalText,
                Language = source.Language,
                PhoneticCode = source.PhoneticCode,
                IsPolyphonic = source.IsPolyphonic,
                DontBreakLineHere = source.DontBreakLineHere,
                ContextNames = source.ContextNames,
                ContextTag = source.ContextTag,
                IsContextTag = source.IsContextTag,
                IsConvertedFromTag = source.IsConvertedFromTag,
                NoDigitCell = source.NoDigitCell,
                NoSpace = source.NoSpace,
                NoCapitalRule = source.NoCapitalRule,
                IsEngPhonetic = source.IsEngPhonetic
            };

            if (!builder.IsContextTag && source.CellCount > 0)
            {
                builder.AppendCells(CollectionsMarshal.AsSpan(source.Cells));
            }
            return builder;
        }

        public string Text { get; set; }

        public string OriginalText { get; set; }

        public BrailleLanguage Language { get; set; }

        public string? PhoneticCode { get; set; }

        public bool IsPolyphonic { get; set; }

        public bool DontBreakLineHere { get; set; }

        public string ContextNames { get; set; }

        public IContextTag? ContextTag { get; set; }

        public bool IsContextTag
        {
            get { return _isContextTag; }
            set
            {
                _isContextTag = value;
                if (_isContextTag)
                {
                    ClearCells();
                }
                else
                {
                    ContextTag = null;
                }
            }
        }

        public bool IsConvertedFromTag { get; set; }

        public bool NoDigitCell { get; set; }

        public bool NoSpace { get; set; }

        public bool NoCapitalRule { get; set; }

        public bool IsEngPhonetic { get; set; }

        public int CellCount
        {
            get { return _cells.Count; }
        }

        public ReadOnlySpan<BrailleCell> Cells
        {
            get { return _cells.AsSpan(); }
        }

        public void ClearCells()
        {
            _cells.Clear();
        }

        public void AppendCell(BrailleCell cell)
        {
            _cells.Append(cell);
        }

        public void AppendCells(ReadOnlySpan<BrailleCell> cells)
        {
            _cells.AppendRange(cells);
        }

        public void AppendHex(string? hex)
        {
            if (String.IsNullOrEmpty(hex))
                return;

            for (int i = 0; i < hex.Length; i += 2)
            {
                string s = hex.Substring(i, 2);
                byte aByte = StrHelper.HexStrToByte(s);
                AppendCell(BrailleCell.GetInstance(aByte));
            }
        }

        public void AppendPositionNumbers(string positionNumbers)
        {
            if (String.IsNullOrEmpty(positionNumbers))
                return;

            var numbers = positionNumbers.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string num in numbers)
            {
                AppendCell(BrailleCell.GetInstanceFromPositionNumberString(num));
            }
        }

        public void PrependCell(BrailleCell cell)
        {
            _cells.Prepend(cell);
        }

        public void ReplaceCell(int index, BrailleCell cell)
        {
            _cells[index] = cell;
        }

        public IBrailleWordResult Build()
        {
            return new BrailleWordMaterialized
            {
                Text = Text,
                OriginalText = OriginalText,
                Language = Language,
                Cells = _cells.ToArray(),
                PhoneticCode = PhoneticCode,
                IsPolyphonic = IsPolyphonic,
                DontBreakLineHere = DontBreakLineHere,
                ContextNames = ContextNames,
                ContextTag = ContextTag,
                IsContextTag = IsContextTag,
                IsConvertedFromTag = IsConvertedFromTag,
                NoDigitCell = NoDigitCell,
                NoSpace = NoSpace,
                NoCapitalRule = NoCapitalRule,
                IsEngPhonetic = IsEngPhonetic
            };
        }

        public BrailleWord ToBrailleWord()
        {
            return BrailleWord.CreateFromConstruction(
                Text,
                OriginalText,
                Language,
                _cells.AsSpan(),
                PhoneticCode,
                IsPolyphonic,
                DontBreakLineHere,
                ContextNames,
                ContextTag,
                IsContextTag,
                IsConvertedFromTag,
                NoDigitCell,
                NoSpace,
                NoCapitalRule,
                IsEngPhonetic);
        }

        public void ApplyTo(BrailleWord target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.ApplyConstruction(
                Text,
                OriginalText,
                Language,
                _cells.AsSpan(),
                PhoneticCode,
                IsPolyphonic,
                DontBreakLineHere,
                ContextNames,
                ContextTag,
                IsContextTag,
                IsConvertedFromTag,
                NoDigitCell,
                NoSpace,
                NoCapitalRule,
                IsEngPhonetic);
        }
    }

    /// <summary>
    /// 由 builder materialize 後的點字詞結果。
    /// </summary>
    internal sealed class BrailleWordMaterialized : IBrailleWordResult
    {
        public string Text { get; init; } = String.Empty;

        public string OriginalText { get; init; } = String.Empty;

        public BrailleLanguage Language { get; init; }

        public ReadOnlyMemory<BrailleCell> Cells { get; init; }

        public string? PhoneticCode { get; init; }

        public bool IsPolyphonic { get; init; }

        public bool DontBreakLineHere { get; init; }

        public string ContextNames { get; init; } = String.Empty;

        public IContextTag? ContextTag { get; init; }

        public bool IsContextTag { get; init; }

        public bool IsConvertedFromTag { get; init; }

        public bool NoDigitCell { get; init; }

        public bool NoSpace { get; init; }

        public bool NoCapitalRule { get; init; }

        public bool IsEngPhonetic { get; init; }

        public int CellCount
        {
            get { return Cells.Length; }
        }

        public BrailleCell GetCell(int index)
        {
            return Cells.Span[index];
        }

        public BrailleWord ToBrailleWord()
        {
            return BrailleWord.FromResult(this);
        }
    }
}
