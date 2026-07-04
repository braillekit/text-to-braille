using System;
using System.Text;

namespace BrailleToolkit.Data
{
    /// <summary>
    /// 台灣中文點字表。
    /// </summary>
    public sealed class TwChineseBrailleTable : XmlBrailleTable
    {
        private static TwChineseBrailleTable? m_Instance = null;

        private TwChineseBrailleTable()
            : base()
        {
        }

        // 不開放這個 method
        private TwChineseBrailleTable(string filename) : base(filename)
        {
        }

        /// <summary>
        /// 傳回 singleton 物件，並載入資源。
        /// </summary>
        /// <returns></returns>
        public static TwChineseBrailleTable GetInstance()
        {
            if (m_Instance == null)
            {
                m_Instance = new TwChineseBrailleTable();
                m_Instance.LoadFromResource();
            }
            return m_Instance;
        }



        /// <summary>
        /// 搜尋某個注音符號，並傳回對應的點字碼。
        /// </summary>
        /// <param name="text">欲搜尋的注音符號。例如："ㄅ"。</param>
        /// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
        public string? GetPhoneticCode(string text)
        {
            CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text, "Phonetic");
            return entry?.Code;
        }

        /// <summary>
        /// 尋找結合韻，並傳回對應的點字碼。
        /// </summary>
        /// <param name="text">結合韻的注音符號，不含聲調。例如 "ㄨㄛ"。</param>
        /// <returns>若是結合韻，則傳回對應的點字碼，否則傳回 null。</returns>
        public string? GetPhoneticJoinedCode(string text)
        {
            CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text, "Phonetic", static x => x.Joined);
            return entry?.Code;
        }

        /// <summary>
        /// 尋找注音符號的七個特殊單音（ㄓ、ㄔ、ㄕ、ㄖ、ㄗ、ㄘ、ㄙ）。
        /// </summary>
        /// <param name="text">某個單音注音符號，例如 "ㄓ"。</param>
        /// <returns>若是特殊單音字，則傳回對應的點字碼，否則傳回空字串。</returns>
        public string? GetPhoneticMonoCode(string text)
        {
            CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text, "Phonetic", static x => x.Mono);
            return entry?.Code;
        }

        /// <summary>
        /// 尋找注音的聲調符號。
        /// </summary>
        /// <param name="text">欲尋找的聲調符號，全形空白代表一聲。</param>
        /// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
        public string? GetPhoneticToneCode(string text)
        {
            CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text, "Tone");
            return entry?.Code;
        }

        /// <summary>
        /// 尋找標點符號。
        /// </summary>
        /// <param name="text">欲尋找的標點符號。</param>
        /// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
        public string? GetPunctuationCode(string text)
        {
            CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text, "Punctuation");
            return entry?.Code;
        }

        /// <summary>
        /// 取得所有標點符號。
        /// </summary>
        /// <returns>包含所有標點符號的字串。</returns>
        public string GetAllPunctuations()
        {
            CheckLoaded();

            var sb = new StringBuilder();
            IReadOnlyList<BrailleTableEntry> entries = FindEntriesByType("Punctuation");
            for (int i = 0; i < entries.Count; i++)
            {
                sb.Append(entries[i].Text);
            }
            return sb.ToString();
        }
    }
}
