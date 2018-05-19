using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using Huanlin.Common.Helpers;

namespace BrailleToolkit
{
    [Serializable]
    [DataContract]
    public class BrailleCellList
    {
        private List<BrailleCell> m_Cells;

        public BrailleCellList()
        {
            m_Cells = new List<BrailleCell>();
        }

        /// <summary>
        /// 將十六進位的點字碼字串轉成對應的點字物件，並加入點字串列。
        /// </summary>
        /// <param name="brCodes">十六進位的點字碼字串。此字串的長度應為 2 的倍數。</param>
        /// <returns></returns>
        public void Add(string brCodes)
        {
            if (String.IsNullOrEmpty(brCodes))
            {
                return; // 忽略空的點字碼（因為呼叫端可能常常會傳入空的點字碼）
            }

            for (int i = 0; i < brCodes.Length; i += 2)
            {
                string s = brCodes.Substring(i, 2);
                byte aByte = StrHelper.HexStrToByte(s);
                BrailleCell cell = BrailleCell.GetInstance(aByte);
                m_Cells.Add(cell);
            }
        }

        public void Assign(BrailleCellList aCellList)
        {
            m_Cells.Clear();
            m_Cells.AddRange(aCellList.m_Cells);
        }

        public void Add(BrailleCell cell)
        {
            m_Cells.Add(cell);
        }

        public void Insert(int index, BrailleCell cell)
        {
            m_Cells.Insert(index, cell);
        }

        public void Clear()
        {
            m_Cells.Clear();
        }

        public BrailleCell this[int index]
        {
            get
            {
                return m_Cells[index];
            }
            set
            {
                m_Cells[index] = value;
            }
        }

        [DataMember]
        public List<BrailleCell> Items
        {
            get
            {
                return m_Cells;
            }

            set
            {
                m_Cells = value;
            }
        }

        public int Count
        {
            get 
            {
                return m_Cells.Count;
            }
        }

        /// <summary>
        /// 將所有點字轉成對應的十六進位碼字串，每個 16 進位字串之間沒有分隔字元。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(null);
        }

        /// <summary>
        /// 將所有點字轉成對應的十六進位碼字串，可指定每個 16 進位字串之間的分隔字串。
        /// </summary>
        /// <param name="separator">每個 16 進位字串之間的分隔字串。</param>
        /// <returns></returns>
        public string ToString(string separator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (BrailleCell cell in m_Cells)
            {
                sb.Append(cell.ToString());
                if (!String.IsNullOrEmpty(separator))
                    sb.Append(separator);
            }
            // 去掉多餘的分隔字元
            if (!String.IsNullOrEmpty(separator))
            {
                if (sb.Length >= 2)
                {
                    sb.Length -= 2;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 跟傳入的點字串列比較，兩者的內容是否相同。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            BrailleCellList cells2 = (BrailleCellList)obj;

            if (this.Count != cells2.Count)
                return false;

            for (int i = 0; i < m_Cells.Count; i++)
            {
                if (!m_Cells[i].Equals(cells2[i]))
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;
            for (int i = 0; i < m_Cells.Count; i++)
            {
                hash += (int)m_Cells[i].Value;
            }
            return hash;
        }
    }
}
