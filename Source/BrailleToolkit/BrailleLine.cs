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
        [DataMember]
        public List<BrailleWord> Words { get; protected set; }

        /// <summary>
        /// 加入 Tag 屬性的最初目的用來記住標題列在雙視文件中的 begin line index，但也可以作為其他用途。
        /// 此屬性不會序列化，不會保存。
        /// </summary>
        public object Tag { get; set; }


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

        public BrailleWord GetFirstVisibleWord()
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

        public string ToOriginalTextString()
        {
            return BrailleWordHelper.ToOriginalTextString(Words);
        }

        public string ToHtmlString(string leadingSpaces, string cssClassTd, string cssClassBraille, string cssClassText)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{leadingSpaces}<tr>");

            foreach (var brWord in Words)
            {
                if (brWord.IsContextTag || brWord.CellCount < 1)
                    continue;

                string brFontText = BrailleFontConverter.ToString(brWord);

                if (String.IsNullOrEmpty(brFontText))
                {
                    sb.AppendLine($"無法轉換成對應的點字字型: {brWord.Text}。");
                    break;
                }

                sb.AppendLine($"{leadingSpaces}  <td colspan='{brFontText.Length}' class='{cssClassTd}'>");
                sb.AppendLine($"{leadingSpaces}    <div class='{cssClassBraille}'>{brFontText}</div>");
                sb.AppendLine($"{leadingSpaces}    <div class='{cssClassText}'>{brWord.Text}</div>");
                sb.AppendLine($"{leadingSpaces}  </td>");
            }

            sb.AppendLine($"{leadingSpaces}</tr>");
            return sb.ToString();
        }


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
                    Words.RemoveAt(i);
                }
            }
        }

        public int IndexOf(BrailleWord brWord)
        {
            // 不能用 Words.IndexOf(brWord) 來尋找! 
            for (int i = 0; i < Words.Count; i++)
            {
                if (ReferenceEquals(Words[i], brWord))
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
            BrailleWord newWord = null;
            while (index < Words.Count && count > 0)
            {
                newWord = Words[index];
                newLine.Words.Add(newWord);

                index++;
                count--;

            }
            newLine.Tag = Tag;
            return newLine;
        }

        public BrailleLine DeepCopy()
        {
            return DeepCopy(0, WordCount);
        }

        public BrailleLine DeepCopy(int index, int count)
        {
            BrailleLine newLine = new BrailleLine();
            BrailleWord newWord = null;
            while (index < Words.Count && count > 0)
            {
                newWord = Words[index].Copy();
                newLine.Words.Add(newWord);

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
            BrailleWord newWord = null;

            foreach (BrailleWord brWord in Words)
            {
                newWord = brWord.Copy();
                newLine.Words.Add(newWord);
            }
            newLine.Tag = Tag;
            return newLine;
        }

        #endregion
    }
}
