using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BrailleToolkit.Tags;
using BrailleToolkit.Helpers;

namespace BrailleToolkit
{
    /// <summary>
    /// 用來儲存一列點字。
    /// </summary>
    [Serializable]
    [DataContract]
    public class BrailleLine : ICloneable
    {
        public BrailleLine()
        {
            Words = new List<BrailleWord>();
        }

        public void Clear()
        {
            Words.Clear();
        }

        public bool IsEmpty()
        {
            return WordCount < 1;
        }

        [DataMember]
        public List<BrailleWord> Words { get; private set; }

        public int WordCount
        {
            get { return Words.Count; }
        }

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
        /// 從指定的起始位置複製指定個數的點字 (BrailleWord) 到新建立的點字串列。
        /// </summary>
        /// <param name="index">起始位置</param>
        /// <param name="count">要複製幾個點字。</param>
        /// <returns>新的點字串列。</returns>
        public BrailleLine Copy(int index, int count)
        {
            BrailleLine brLine = new BrailleLine();
            BrailleWord newWord = null;
            while (index < Words.Count && count > 0)
            {
                //newWord = Words[index].Copy();
                newWord = Words[index]; 
                brLine.Words.Add(newWord);

                index++;
                count--;

            }
            return brLine;
        }

        public void RemoveAt(int index)
        {
            Words.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            if ((index + count) > Words.Count)    // 防止要取的數量超出邊界。
            {
                count = Words.Count - index;
            }
            Words.RemoveRange(index, count);
        }

        /// <summary>
        /// 將指定的點字列附加至此點字列。
        /// </summary>
        /// <param name="brLine"></param>
        public void Append(BrailleLine brLine)
        {
            if (brLine == null || brLine.WordCount < 1)
                return;

            Words.AddRange(brLine.Words);
        }

        public void Insert(int index, BrailleWord brWord)
        {
            Words.Insert(index, brWord);
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
                    Words.RemoveAt(i);
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
                    Words.RemoveAt(i);
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
        /// 將本串列中的所有點字轉成 16 進位的字串。
        /// </summary>
        /// <returns></returns>
        public string ToBrailleCellHexString()
        {
            var sb = new StringBuilder();
            foreach (var brWord in Words)
            {
                foreach (var cell in brWord.Cells)
                {
                    sb.Append(cell.ToHexString());
                }                
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將本串列中的所有點字轉成以點位組成的字串。各點字以一個空白字元隔開。
        /// </summary>
        /// <returns></returns>
        public string ToPositionNumberString()
        {
            var sb = new StringBuilder();
            foreach (var brWord in Words)
            {
                sb.Append(brWord.ToPositionNumberString(useParenthesis: true));
            }
            return sb.ToString();
        }

        public string ToOriginalTextString(ContextTagManager context)
        {
            var sb = new StringBuilder();
            int index = 0;
            while (index < Words.Count)
            {
                var brWord = Words[index];
                if (brWord.IsContextTag)
                {
                    sb.Append(brWord.Text); // 輸出標籤名稱（可能為起始標籤或結束標籤）。
                    index++;
                    continue;
                }
                if (brWord.IsConvertedFromTag) // 只要是由 context tag 所衍生的文字都不儲存。
                {
                    index++;
                    continue;
                }

                // 一般文字，或曾被替換過的文字。
                if (!String.IsNullOrEmpty(brWord.OriginalText))
                    sb.Append(brWord.OriginalText);
                else
                    sb.Append(brWord.Text);
                index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 是否包含標題情境標籤。
        /// </summary>
        /// <returns></returns>
        public bool ContainsTitleTag()
        {
            if (Words.Count > 0 && Words[0].Text.Equals(ContextTagNames.Title))
            {
                return true;
            }
            return false;
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
                    Words.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 在串列中尋找指定的字串，從串列中的第 startIndex 個字開始找起。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startIndex"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int IndexOf(string value, int startIndex, StringComparison comparisonType)
        {
            if (startIndex + value.Length > this.WordCount)
            {
                return -1;
            }

            int i;
            StringBuilder sb = new StringBuilder();
            for (i = startIndex; i < this.WordCount; i++)
            {
                sb.Append(Words[i].Text);
            }

            int idx = sb.ToString().IndexOf(value, comparisonType);
            if (idx < 0)
            {
                return -1;
            }

            // 有找到，但這是字元索引，還必須修正為點字索引。
            for (i = startIndex; i < this.WordCount; i++)
            {
                idx = idx - Words[i].Text.Length + 1;
            }

            return startIndex + idx;
        }

        #region ICloneable Members

        /// <summary>
        /// 深層複製。
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            BrailleLine brLine = new BrailleLine();
            BrailleWord newWord = null;

            foreach (BrailleWord brWord in Words)
            {
                newWord = brWord.Copy();
                brLine.Words.Add(newWord);
            }
            return brLine;
        }

        #endregion
    }
}
