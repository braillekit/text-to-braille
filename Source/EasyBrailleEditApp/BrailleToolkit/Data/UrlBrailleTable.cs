using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

namespace BrailleToolkit.Data
{
	public sealed class UrlBrailleTable : XmlBrailleTable
	{
        private static UrlBrailleTable m_Instance = null;

        private UrlBrailleTable() : base()
        {
        }

        // 不開放這個 method
		private UrlBrailleTable(string filename) : base(filename)
		{
		}

        /// <summary>
        /// 傳回 singleton 物件，並載入資源。
        /// </summary>
        /// <returns></returns>
        public static UrlBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new UrlBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }

		/// <summary>
		/// 搜尋某個字母，並傳回對應的點字碼。
		/// </summary>
		/// <param name="text">欲搜尋的字母。例如：'A'。</param>
		/// <returns>若有找到，則傳回對應的點字碼，否則傳回空字串。</returns>
		public string FindLetter(string text)
		{
			CheckLoaded();

			string filter = "type='Letter' and text='" + text.ToUpper() + "'";
			DataRow[] rows = m_Table.Select(filter);
			if (rows.Length > 0)
				return rows[0]["code"].ToString();
			return null;
		}

		/// <summary>
		/// 搜尋某個數字，並傳回對應的點字碼。
		/// </summary>
		/// <param name="text">欲搜尋的數字。例如：'9'。</param>
		/// <param name="upper">True/False = 傳回上位點/下位點。</param>
		/// <returns>若有找到，則傳回對應的點字碼，否則傳回空字串。</returns>
		public string FindDigit(string text, bool upper)
		{
			CheckLoaded();

			string filter = "type='Digit' and text='" + text + "'";
			DataRow[] rows = m_Table.Select(filter);
			if (rows.Length > 0)
			{
				if (upper)	// 上位點?
					return rows[0]["code"].ToString();
				return rows[0]["code2"].ToString();
			}
			return null;
		}
	}
}
