using System;
using System.Collections.Generic;
using System.Text;

namespace BrailleToolkit.Data
{
	public sealed class PhoneticBrailleTable : XmlBrailleTable
	{
		private static PhoneticBrailleTable m_Instance = null;

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

        /// <summary>
        /// Creates a new instance of PhoneticBrailleTable.
        /// </summary>
        /// <returns></returns>
        public static PhoneticBrailleTable CreateInstance()
        {
            var instance = new PhoneticBrailleTable();
            instance.LoadFromResource();
            return instance;
        }
	}
}
