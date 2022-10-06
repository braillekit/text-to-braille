using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace BrailleToolkit.Data
{
    internal class MathBrailleTable : XmlBrailleTable
    {
        private static MathBrailleTable m_Instance = null;

        private MathBrailleTable()
            : base()
        {
        }

        /// <summary>
        /// 傳回 singleton 物件，並載入資源。
        /// </summary>
        /// <returns></returns>
        public static MathBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new MathBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }
    }
}
