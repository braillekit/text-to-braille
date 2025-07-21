using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

namespace BrailleToolkit.Data
{
    public sealed class ChineseBrailleTable : XmlBrailleTable
    {
        private static ChineseBrailleTable m_Instance = null;

        private ChineseBrailleTable()
            : base()
        {
        }

        // 不開放這個 method
        private ChineseBrailleTable(string filename) : base(filename)
        {
        }

        /// <summary>
        /// 傳回 singleton 物件，並載入資源。
        /// </summary>
        /// <returns></returns>
        public static ChineseBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new ChineseBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }

        /// <summary>
        /// Creates a new instance of ChineseBrailleTable.
        /// </summary>
        /// <returns></returns>
        public static ChineseBrailleTable CreateInstance()
        {
            var instance = new ChineseBrailleTable();
            instance.LoadFromResource();
            return instance;
        }

        /// <summary>
        /// 搜尋某個注音符號，並傳回對應的點字碼。
        /// </summary>
        /// <param name="text">欲搜尋的注音符號。例如："ㄅ"。</param>
        /// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
        public string GetPhoneticCode(string text)
        {
            CheckLoaded();

            string filter = "type='Phonetic' and text='" + text + "'";
            DataRow[] rows = m_Table.Select(filter);
            if (rows.Length > 0)
                return rows[0]["code"].ToString();
            return null;
        }

        /// <summary>
        /// 尋找結合韻，並傳回對應的點字碼。
        /// </summary>
        /// <param name="text">結合韻的注音符號，不含聲調。例如 "ㄨㄛ"。</param>
        /// <returns>若是結合韻，則傳回對應的點字碼，否則傳回 null。</returns>
        public string GetPhoneticJoinedCode(string text)
        {
            CheckLoaded();

            string filter = "type='Phonetic' and joined=true and text='" + text + "'";
            DataRow[] rows = m_Table.Select(filter);
            if (rows.Length > 0)
                return rows[0]["code"].ToString();
            return null;
        }

        /// <summary>
        /// 尋找注音符號的七個特殊單音（ㄓ、ㄔ、ㄕ、ㄖ、ㄗ、ㄘ、ㄙ）。
        /// </summary>
        /// <param name="text">某個單音注音符號，例如 "ㄓ"。
        /// <returns>若是特殊單音字，則傳回對應的點字碼，否則傳回空字串。</returns>
        public string GetPhoneticMonoCode(string text)
        {
            CheckLoaded();

            string filter = "type='Phonetic' and mono=true and text='" + text + "'";
            DataRow[] rows = m_Table.Select(filter);
            if (rows.Length > 0)
                return rows[0]["code"].ToString();
            return null;
        }

        /// <summary>
        /// 尋找注音的聲調符號。
        /// </summary>
        /// <param name="text">欲尋找的聲調符號，全形空白代表一聲。</param>
        /// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
        public string GetPhoneticToneCode(string text)
        {
            CheckLoaded();

            string filter = "type='Tone' and text='" + text + "'";
            DataRow[] rows = m_Table.Select(filter);
            if (rows.Length > 0)
                return rows[0]["code"].ToString();
            return null;
        }

        /// <summary>
        /// 尋找標點符號。
        /// </summary>
        /// <param name="text">欲尋找的標點符號。</param>
        /// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
        public string GetPunctuationCode(string text)
        {
            CheckLoaded();

            // 修正單引號：在 SQL 查詢條件中的單引號必須連續兩個
            if ("'".Equals(text))
            {
                text = "''";
            }
            string filter = "type='Punctuation' and text='" + text + "'";
            DataRow[] rows = m_Table.Select(filter);
            if (rows.Length > 0)
                return rows[0]["code"].ToString();
            return null;
        }

        public string GetAllPunctuations()
        {
            CheckLoaded();

            string filter = "type='Punctuation'";
            DataRow[] rows = m_Table.Select(filter);
            var sb = new StringBuilder();
            foreach (var row in rows)
            {
                sb.Append(row["text"]).ToString();
            }
            return sb.ToString();
        }
    }
}
