using System;
namespace BrailleToolkit.Data
{
    /// <summary>
    /// URL 點字表。
    /// </summary>
	public sealed class UrlBrailleTable : XmlBrailleTable
	{
        private static UrlBrailleTable? m_Instance = null;

        /// <summary>
        /// 建構函式。
        /// </summary>
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
		public string? FindLetter(string text)
		{
			CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text.ToUpper(), "Letter");
            return entry?.Code;
		}

		/// <summary>
		/// 搜尋某個數字，並傳回對應的點字碼。
		/// </summary>
		/// <param name="text">欲搜尋的數字。例如：'9'。</param>
		/// <param name="upper">True/False = 傳回上位點/下位點。</param>
		/// <returns>若有找到，則傳回對應的點字碼，否則傳回空字串。</returns>
		public string? FindDigit(string text, bool upper)
		{
			CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text, "Digit");
            if (entry == null)
            {
                return null;
            }

			if (upper)	// 上位點?
				return entry.Value.Code;
			return entry.Value.Code2;
		}
	}
}
