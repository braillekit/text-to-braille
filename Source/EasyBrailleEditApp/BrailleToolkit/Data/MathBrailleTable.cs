using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace BrailleToolkit.Data
{
    public sealed class MathBrailleTable : XmlBrailleTable
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

        /// <summary>
        /// Creates a new instance of MathBrailleTable.
        /// </summary>
        /// <returns></returns>
        public static MathBrailleTable CreateInstance()
        {
            var instance = new MathBrailleTable();
            instance.LoadFromResource();
            return instance;
        }
    }
}
