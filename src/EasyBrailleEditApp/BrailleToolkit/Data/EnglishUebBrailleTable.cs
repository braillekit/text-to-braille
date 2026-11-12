using System;
namespace BrailleToolkit.Data
{
    /// <summary>
    /// UEB (Unified English Braille) 點字表。
    /// </summary>
	public sealed class EnglishUebBrailleTable : XmlBrailleTable
	{
        private static EnglishUebBrailleTable? m_Instance = null;

        /// <summary>
        /// 建構函式。
        /// </summary>
        private EnglishUebBrailleTable() : base()
        {
        }

        // 不開放這個 method
		private EnglishUebBrailleTable(string filename) : base(filename)
		{
		}

        /// <summary>
        /// 傳回 singleton 物件，並載入資源。
        /// </summary>
        /// <returns></returns>
        public static EnglishUebBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new EnglishUebBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }



		/// <summary>
		/// 搜尋某個字母，並傳回對應的點字碼。
		/// </summary>
		/// <param name="text">欲搜尋的字母。例如：'A'。</param>
		/// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
		public string? FindLetter(string text)
		{
			CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text.ToLower(), "Letter");
            return entry?.Code;
		}
	}
}
