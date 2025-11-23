using System;
using System.Collections.Generic;
using System.Text;

namespace BrailleToolkit.Data
{
    /// <summary>
    /// 音標符號點字表。
    /// </summary>
	public sealed class PhoneticBrailleTable : XmlBrailleTable
	{
		private static PhoneticBrailleTable? m_Instance = null;

		/// <summary>
		/// 建構函式。
		/// </summary>
		private PhoneticBrailleTable() : base()
        {
        }

        /// <summary>
        /// 傳回 singleton 物件，並載入資源。
        /// </summary>
        /// <returns></returns>
		public static PhoneticBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
				m_Instance = new PhoneticBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }


	}
}
