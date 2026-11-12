using System;
using System.Collections.Generic;
using BrailleToolkit.Helpers;

namespace BrailleToolkit
{
    /// <summary>
    /// 已 materialize 的點字列唯讀介面。
    /// </summary>
    internal interface IBrailleLineResult : IBrailleLineView
    {
        BrailleLine ToBrailleLine();
    }

    /// <summary>
    /// 不可變的點字列快照，用來保存 builder 建構完成後的結果。
    /// </summary>
    internal sealed class BrailleLineMaterialized : IBrailleLineResult
    {
        private readonly BrailleWord[] _words;

        public BrailleLineMaterialized(IReadOnlyList<BrailleWord> words, object? tag)
        {
            _words = new BrailleWord[words.Count];
            for (int i = 0; i < words.Count; i++)
            {
                _words[i] = words[i];
            }
            Tag = tag;
        }

        public long Identity => 0; // materialized result 不帶 identity，由 ToBrailleLine 產生新的

        public IReadOnlyList<BrailleWord> Words => _words;

        public int WordCount => _words.Length;

        public int CellCount
        {
            get
            {
                int cnt = 0;
                for (int i = 0; i < _words.Length; i++)
                {
                    cnt += _words[i].Cells.Count;
                }
                return cnt;
            }
        }

        public object? Tag { get; }

        public BrailleWord this[int index] => _words[index];

        public BrailleLine ToBrailleLine()
        {
            return BrailleLine.FromResult(this);
        }
    }

    /// <summary>
    /// 點字列的可變建構器，提供與 BrailleLine 相同的 mutation API。
    /// 建構完成後可透過 Build() 產生不可變快照，或透過 ToBrailleLine() 直接轉為 BrailleLine。
    /// </summary>
    internal sealed class BrailleLineBuilder
    {
        private readonly List<BrailleWord> _words = new();

        public BrailleLineBuilder()
        {
        }

        public static BrailleLineBuilder FromBrailleLine(BrailleLine source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var builder = new BrailleLineBuilder();
            builder._words.AddRange(source.Words);
            builder.Tag = source.Tag;
            return builder;
        }

        public IReadOnlyList<BrailleWord> Words => _words;

        public int WordCount => _words.Count;

        public BrailleWord this[int index] => _words[index];

        public object? Tag { get; set; }

        public void Clear()
        {
            _words.Clear();
        }

        public void AddWord(BrailleWord word)
        {
            _words.Add(word);
        }

        public void AddWords(IEnumerable<BrailleWord> words)
        {
            _words.AddRange(words);
        }

        public void Insert(int index, BrailleWord word)
        {
            _words.Insert(index, word);
        }

        public void InsertWords(int index, IEnumerable<BrailleWord> words)
        {
            _words.InsertRange(index, words);
        }

        public void RemoveAt(int index)
        {
            _words.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            if ((index + count) > _words.Count)
            {
                count = _words.Count - index;
            }
            _words.RemoveRange(index, count);
        }

        public void Append(BrailleLineBuilder other)
        {
            if (other == null || other.WordCount < 1)
                return;
            _words.AddRange(other._words);
        }

        public void TrimStart()
        {
            int i = 0;
            while (i < _words.Count)
            {
                if (BrailleWord.IsBlank(_words[i]) || BrailleWord.IsEmpty(_words[i]))
                {
                    _words.RemoveAt(i);
                    continue;
                }
                break;
            }
        }

        public void TrimEnd()
        {
            int i = _words.Count - 1;
            while (i >= 0)
            {
                if (BrailleWord.IsBlank(_words[i]) || BrailleWord.IsEmpty(_words[i]))
                {
                    _words.RemoveAt(i);
                    i--;
                    continue;
                }
                break;
            }
        }

        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        public void RemoveContextTags()
        {
            for (int i = _words.Count - 1; i >= 0; i--)
            {
                if (_words[i].IsContextTag)
                {
                    _words.RemoveAt(i);
                }
            }
        }

        public IBrailleLineResult Build()
        {
            return new BrailleLineMaterialized(_words, Tag);
        }

        public BrailleLine ToBrailleLine()
        {
            return Build().ToBrailleLine();
        }

        public void ApplyTo(BrailleLine target)
        {
            target.ApplyResult(Build());
        }
    }
}
