using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BrailleToolkit.Tags;
using BrailleToolkit.Helpers;
using BrailleToolkit.Converters;

namespace BrailleToolkit
{
    /// <summary>
    /// 用來儲存一列點字。
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrailleLine : ICloneable
    {
        private readonly long m_Identity = BrailleObjectIdentityGenerator.NextLineIdentity();

        [DataMember(Name = "Words")]
        private List<BrailleWord> m_Words;

        /// <summary>
        /// 取得或設定組成此行的點字詞串列。
        /// </summary>
        [IgnoreDataMember]
        public IReadOnlyList<BrailleWord> Words
        {
            get { return m_Words; }
            private set { m_Words = CopyWords(value); }
        }

        /// <summary>
        /// 加入 Tag 屬性的最初目的用來記住標題列在雙視文件中的 begin line index，但也可以作為其他用途。
        /// 此屬性不會序列化，不會保存。
        /// </summary>
        public object? Tag { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="BrailleLine"/> class.
        /// </summary>
        public BrailleLine()
        {
            m_Words = new List<BrailleWord>();
        }

        /// <summary>
        /// 此物件的執行期識別碼，用來在文件編輯流程中追蹤同一列。
        /// </summary>
        public long Identity
        {
            get { return m_Identity; }
        }

        private static List<BrailleWord> CopyWords(IEnumerable<BrailleWord>? words)
        {
            var result = new List<BrailleWord>();
            if (words == null)
            {
                return result;
            }

            foreach (var word in words)
            {
                result.Add(word);
            }
            return result;
        }

        /// <summary>
        /// Clears all words from this line.
        /// </summary>
        public void Clear()
        {
            m_Words.Clear();
        }

        /// <summary>
        /// 以指定的點字詞集合取代目前這一列的內容，但保留此列本身的執行期識別碼。
        /// </summary>
        /// <param name="words">新的點字詞集合。</param>
        internal void AssignWords(IEnumerable<BrailleWord>? words)
        {
            m_Words.Clear();
            if (words == null)
            {
                return;
            }

            foreach (var word in words)
            {
                m_Words.Add(word);
            }
        }

        /// <summary>
        /// Checks if this line is empty.
        /// </summary>
        /// <returns>True if empty; otherwise, false.</returns>
        public bool IsEmpty()
        {
            return WordCount < 1;
        }

        /// <summary>
        /// Checks if this line is empty or contains only whitespace.
        /// </summary>
        /// <returns>True if empty or whitespace; otherwise, false.</returns>
        public bool IsEmptyOrWhiteSpace()
        {
            foreach (var word in Words)
            {
                if (!BrailleWord.IsBlank(word) && !BrailleWord.IsEmpty(word))
                {
                    return false;
                }

            }
            return true;
        }

        /// <summary>
        /// Checks if this line is the beginning of a paragraph.
        /// </summary>
        /// <returns>True if it is a paragraph beginning; otherwise, false.</returns>
        public bool IsBeginOfParagraph()
        {
            if (WordCount >= 2)
            {
                if (Words[0].IsWhiteSpace && Words[1].IsWhiteSpace)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the number of words in this line.
        /// </summary>
        public int WordCount
        {
            get { return Words.Count; }
        }

        /// <summary>
        /// Gets the word at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The BrailleWord at the index.</returns>
        public BrailleWord this[int index]
        {
            get
            {
                return Words[index];
            }
        }

        /// <summary>
        /// 傳回所有點字的總方數。
        /// </summary>
        public int CellCount
        {
            get
            {
                int cnt = 0;
                foreach (BrailleWord brWord in Words)
                {
                    cnt += brWord.Cells.Count;
                }
                return cnt;
            }
        }

        /// <summary>
        /// 取得本串列中的所有的 BraillCell 物件。
        /// </summary>
        /// <returns></returns>
        public List<BrailleCell> GetBrailleCells()
        {
            var list = new List<BrailleCell>();
            foreach (var brWord in Words)
            {
                list.AddRange(brWord.Cells);
            }
            return list;
        }

        /// <summary>
        /// 計算斷行的點字索引位置。
        /// 此處僅根據傳入的最大方數來計算可斷行的點字索引，並未加入其他斷行規則的判斷。
        /// </summary>
        /// <param name="cellsPerLine">一行可允許多少方數。</param>
        /// <returns>可斷行的點字索引。例如，若索引編號第 29 個字（0-based）必須折到下一行，
        /// 傳回值就是 29。若不需要斷行，則傳回整行的字數。</returns>
        public int CalcBreakPoint(int cellsPerLine)
        {
            if (cellsPerLine < 4)
            {
                throw new ArgumentException("cellsPerLine 參數值不可小於 4。");
            }

            int cellCnt = 0;
            int index = 0;
            while (index < Words.Count)
            {
                cellCnt += Words[index].Cells.Count;
                if (cellCnt > cellsPerLine)
                {
                    break;
                }
                index++;
            }
            return index;
        }

        /// <summary>
        /// 取得第一個可見點字詞的索引。
        /// </summary>
        /// <returns>如果找到，則為第一個可見點字詞的索引；否則為 -1。</returns>
        public int GetFirstVisibleWordIndex()
        {
            for (int i = 0; i < Words.Count; i++)
            {
                if (Words[i].CellCount > 0)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 取得第一個可見的點字詞。
        /// </summary>
        /// <returns>如果找到，則為第一個可見的 BrailleWord 物件；否則為 null。</returns>
        public BrailleWord? GetFirstVisibleWord()
        {
            for (int i = 0; i < Words.Count; i++)
            {
                if (Words[i].CellCount > 0)
                {
                    return Words[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 從點字行中移除指定索引處的點字詞。
        /// </summary>
        /// <param name="index">要移除之項目的以零為起始的索引。</param>
        public void RemoveAt(int index)
        {
            m_Words.RemoveAt(index);
        }

        /// <summary>
        /// 從點字行移除一定範圍的點字詞。
        /// </summary>
        /// <param name="index">要移除之項目範圍的以零為起始的起始索引。</param>
        /// <param name="count">要移除的項目數。</param>
        public void RemoveRange(int index, int count)
        {
            if ((index + count) > Words.Count)    // 防止要取的數量超出邊界。
            {
                count = Words.Count - index;
            }
            m_Words.RemoveRange(index, count);
        }

        /// <summary>
        /// 將指定的點字列附加至此點字列。
        /// </summary>
        /// <param name="brLine"></param>
        public void Append(BrailleLine brLine)
        {
            if (brLine == null || brLine.WordCount < 1)
                return;

            m_Words.AddRange(brLine.Words);
        }

        /// <summary>
        /// 將點字詞附加至此點字行。
        /// </summary>
        public void AddWord(BrailleWord brWord)
        {
            m_Words.Add(brWord);
        }

        /// <summary>
        /// 將多個點字詞附加至此點字行。
        /// </summary>
        public void AddWords(IEnumerable<BrailleWord> words)
        {
            m_Words.AddRange(words);
        }

        /// <summary>
        /// 將點字詞插入點字行的指定索引處。
        /// </summary>
        /// <param name="index">應插入點字詞的以零為起始的索引。</param>
        /// <param name="brWord">要插入的點字詞。</param>
        public void Insert(int index, BrailleWord brWord)
        {
            m_Words.Insert(index, brWord);
        }

        /// <summary>
        /// 將多個點字詞插入點字行的指定索引處。
        /// </summary>
        public void InsertWords(int index, IEnumerable<BrailleWord> words)
        {
            m_Words.InsertRange(index, words);
        }

        /// <summary>
        /// 去掉開頭的空白字元。
        /// </summary>
        public void TrimStart()
        {
            int i = 0;
            while (i < Words.Count)
            {
                if (BrailleWord.IsBlank(Words[i]) || BrailleWord.IsEmpty(Words[i]))
                {
                    m_Words.RemoveAt(i);
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// 去掉結尾的空白字元。
        /// </summary>
        public void TrimEnd()
        {
            int i = Words.Count - 1;
            while (i >= 0)
            {
                if (BrailleWord.IsBlank(Words[i]) || BrailleWord.IsEmpty(Words[i]))
                {
                    m_Words.RemoveAt(i);
                    i--;
                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// 把頭尾的空白去掉。
        /// </summary>
        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        /// <summary>
        /// 將點字行轉換為其點字碼的字串表示形式。
        /// </summary>
        /// <returns>包含所有點字詞的點字碼的字串。</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (BrailleWord brWord in Words)
            {
                sb.Append(brWord.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將點字行轉換為其文字表示形式，會包含情境標籤。
        /// </summary>
        /// <returns>代表此點字行的文字字串。</returns>
        public string ToTextString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (BrailleWord brWord in Words)
            {
                if (brWord.IsContextTag)
                {
                    sb.Append(brWord.Text); // 輸出標籤名稱（可能為起始標籤或結束標籤）。
                    continue;
                }
                if (brWord.IsConvertedFromTag) // 只要是由 context tag 所衍生的文字都忽略。
                {
                    continue;
                }

                sb.Append(brWord.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將點字行轉換回其原始的明眼文字串。
        /// </summary>
        /// <returns>原始的明眼文字串。</returns>
        public string ToOriginalTextString()
        {
            return BrailleWordHelper.ToOriginalTextString(Words);
        }

        /// <summary>
        /// 將本串列中的所有點字轉成 16 進位的字串。
        /// </summary>
        /// <returns></returns>
        public string ToBrailleCellHexString()
        {
            return BrailleWordSequenceFormatter.ToBrailleCellHexString(Words);
        }

        /// <summary>
        /// 將本串列中的所有點字轉成以點位組成的字串。各點字以一個空白字元隔開。
        /// </summary>
        /// <returns></returns>
        public string ToPositionNumberString()
        {
            return BrailleWordSequenceFormatter.ToPositionNumberString(Words);
        }

        /// <summary>
        /// 將點字行轉換為 HTML 表格列的字串表示形式。
        /// </summary>
        /// <param name="leadingSpaces">行首的空白字元。</param>
        /// <param name="cssClassTd">用於儲存格 (td) 的 CSS 類別。</param>
        /// <param name="cssClassBraille">用於點字區塊 (div) 的 CSS 類別。</param>
        /// <param name="cssClassText">用於明眼字區塊 (div) 的 CSS 類別。</param>
        /// <returns>表示此點字行的 HTML 字串。</returns>
        public string ToHtmlString(string leadingSpaces, string cssClassTd, string cssClassBraille, string cssClassText)
        {
            return BrailleWordSequenceFormatter.ToHtmlString(Words, leadingSpaces, cssClassTd, cssClassBraille, cssClassText);
        }

        /// <summary>
        /// 檢查此行是否包含標題標籤。
        /// </summary>
        /// <returns>如果包含標題標籤，則為 true；否則為 false。</returns>
        public bool ContainsTitleTag()
        {
            return BrailleWordHelper.ContainsTitleTag(Words);
        }

        /// <summary>
        /// 移除所有情境標籤。
        /// </summary>
        public void RemoveContextTags()
        {
            BrailleWord brWord;

            for (int i = WordCount - 1; i >= 0; i--)
            {
                brWord = Words[i];
                if (brWord.IsContextTag)
                {
                    m_Words.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 尋找指定 BrailleWord 物件在此行中的索引。
        /// </summary>
        /// <param name="brWord">要尋找的 BrailleWord 物件。</param>
        /// <returns>如果找到，則為其索引；否則為 -1。</returns>
        public int IndexOf(BrailleWord brWord)
        {
            // 不能用 Words.IndexOf(brWord) 來尋找!
            for (int i = 0; i < Words.Count; i++)
            {
                if (Words[i].Identity == brWord.Identity)
                {
                    return i;
                }
            }
            return -1;            
        }

        /// <summary>
        /// 在串列中尋找指定的字串，從串列中的第 startIndex 個字開始找起。
        /// 尋找過程中，會略過 cell count 為 0 的 BrailleWord 物件。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int IndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            int index = startIndex;
            while (index < WordCount)
            {
                if (index + value.Length > WordCount)
                {
                    return -1;
                }

                int matchedCount = 0;
                int wordPointer = index;
                while (wordPointer < WordCount)
                {
                    var brWord = Words[wordPointer];
                    if (brWord.CellCount < 1 || String.IsNullOrEmpty(brWord.Text))
                    {
                        wordPointer++;  // 跳過 cell count 為 0 的物件
                        if (matchedCount == 0)
                        {
                            // 匹配的字串裡面可以包含 context tag，可是 context tag 不能是匹配字串的第一個字。
                            index = wordPointer;
                        }
                        continue;
                    }

                    string s = value[matchedCount].ToString();
                    if (!brWord.Text.Equals(s, comparisonType))
                    {
                        break;
                    }
                    matchedCount++;
                    wordPointer++;

                    if (matchedCount >= value.Length)
                    {
                        return index;
                    }
                }
                index++;
            }
            return -1;
        }


        /// <summary>
        /// 從指定的起始位置複製指定個數的點字 (BrailleWord) 到新建立的點字串列。
        /// 注意：這是 shallow copy，新的串列中包含既有的元素參考，而非建立新元素。
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">要複製幾個點字。</param>
        /// <returns>新的點字串列。</returns>
        public BrailleLine ShallowCopy(int index, int count)
        {
            BrailleLine newLine = new BrailleLine();
            BrailleWord? newWord = null;
            while (index < Words.Count && count > 0)
            {
                newWord = Words[index];
                newLine.AddWord(newWord);

                index++;
                count--;

            }
            newLine.Tag = Tag;
            return newLine;
        }

        /// <summary>
        /// 建立目前 BrailleLine 物件的深層複本。
        /// </summary>
        /// <returns>目前執行個體的深層複本。</returns>
        public BrailleLine DeepCopy()
        {
            return DeepCopy(0, WordCount);
        }

        /// <summary>
        /// 建立目前 BrailleLine 物件一部分的深層複本。
        /// </summary>
        /// <param name="index">要複製之範圍的起始索引。</param>
        /// <param name="count">要複製的項目數。</param>
        /// <returns>指定範圍的深層複本。</returns>
        public BrailleLine DeepCopy(int index, int count)
        {
            BrailleLine newLine = new BrailleLine();
            BrailleWord? newWord = null;
            while (index < Words.Count && count > 0)
            {
                newWord = Words[index].Copy();
                newLine.AddWord(newWord);

                index++;
                count--;
            }
            newLine.Tag = Tag;
            return newLine;
        }


        #region ICloneable Members

        /// <summary>
        /// 深層複製。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            BrailleLine newLine = new BrailleLine();
            BrailleWord? newWord = null;

            foreach (BrailleWord brWord in Words)
            {
                newWord = brWord.Copy();
                newLine.AddWord(newWord);
            }
            newLine.Tag = Tag;
            return newLine;
        }

        #endregion
    }
}
