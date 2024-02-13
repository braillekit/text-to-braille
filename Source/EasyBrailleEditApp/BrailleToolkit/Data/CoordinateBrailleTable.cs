using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace BrailleToolkit.Data
{
    public sealed class CoordinateBrailleTable : XmlBrailleTable
    {
        private static CoordinateBrailleTable m_Instance = null;

        private CoordinateBrailleTable()
            : base()
        {
        }

        /// <summary>
        /// 傳回 singleton 物件，並載入資源。
        /// </summary>
        /// <returns></returns>
        public static CoordinateBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new CoordinateBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }
    }
}
