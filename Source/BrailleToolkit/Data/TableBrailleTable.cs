using System;
using System.Collections.Generic;
using System.Text;

namespace BrailleToolkit.Data
{
	internal sealed class TableBrailleTable : XmlBrailleTable
	{
		private static TableBrailleTable m_Instance = null;

		private TableBrailleTable()
            : base()
        {
        }

		/// <summary>
        /// 傳回 singleton 物件，並載入資源。
        /// </summary>
        /// <returns></returns>
		public static TableBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
				m_Instance = new TableBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }
	}
}
