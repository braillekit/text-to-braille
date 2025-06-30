using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using BrailleToolkit.Tags;
using BrailleToolkit.Data;
using BrailleToolkit.Helpers;
using Huanlin.Common.Helpers;
using NChinese.Phonetic;

namespace BrailleToolkit
{
    public enum BrailleLanguage
    {
        Neutral = 0,
        Chinese,
        English
    };


    /// <summary>
    /// 代表一個中文字，內含注音碼與點字值。
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class BrailleWord
    {
        public static BrailleWord BlankWord { get; } = NewBlank();

        private List<string> m_PhoneticCodes;   // 所有注音組字字根（以支援破音字）。
        private int m_ActivePhoneticIndex;      // 目前使用的注音組字字根索引。

        [NonSerialized]
        private string m_PhoneticCode;          // 注音字根（ㄅㄆㄇㄈ）。

        [NonSerialized]
        private bool m_IsPolyphonic;            // 是否為多音字。

        [NonSerialized]
        private bool m_IsContextTag;

        //private bool m_QuotationResolved;	// 是否已經識別出左右引號（英文的單引號和雙引號都是同一個符號，但點字不同）

        public BrailleWord(string text)
        {
            Text = text;
            OriginalText = text;

            Language = BrailleLanguage.Neutral;
            CellList = new BrailleCellList();

            m_PhoneticCodes = new List<string>();
            m_ActivePhoneticIndex = -1;

            DontBreakLineHere = false;
            ContextNames = String.Empty;

            m_IsContextTag = false;
            NoDigitCell = false;
            IsEngPhonetic = false;
        }

        public BrailleWord(string text, BrailleCellCode brCode) : this(text)
        {
            CellList.Add(BrailleCell.GetInstance(brCode));
        }

        public BrailleWord(string text, string brCode) : this(text)
        {
            AddCells(brCode);
        }

        public BrailleWord(string text, byte brCode) : this(text)
        {
            CellList.Add(BrailleCell.GetInstance(brCode));
        }

        public BrailleWord(string text, string phCode, string brCode) : this(text, brCode)
        {
            Language = BrailleLanguage.Chinese;
            m_PhoneticCodes.Add(phCode);
            m_ActivePhoneticIndex = 0;
        }

        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            BrailleWord brWord = (BrailleWord)obj;

            if (CellList.Count != brWord.Cells.Count)
                return false;
            for (int i = 0; i < CellList.Count; i++)
            {				
                if (!CellList[i].Equals(brWord.Cells[i]))
                    return false;
            }

            // 不比對注音，因為一聲常常和全形空白搞混。
            //if (m_PhoneticCode != brWord.PhoneticCode)
            //    return false;

            // 不比對文字，因為全形空白和半形空白其實應視為相等，比較 cells 就夠了。
            //if (m_Text != brWord.Text)
            //
            //{
            //    return false;
            //}

            return true;
        }

        public override string ToString()
        {
            return Text;
        }

        public string ToPositionNumberString(bool useParenthesis)
        {
            if (IsContextTag)
            {
                return String.Empty;
            }

            var sb = new StringBuilder();
            if (useParenthesis)
                sb.Append("(");
            foreach (var cell in Cells)
            {
                sb.Append(cell.ToPositionNumberString());
                sb.Append(" ");
            }
            var result = sb.ToString().TrimEnd();
            if (useParenthesis)
                result += ")";
            return result;
        }

        public string ToHexSting()
        {
            var sb = new StringBuilder();
            foreach (var cell in Cells)
            {
                sb.Append(cell.ToHexString());
            }
            return sb.ToString();
        }

        public void Clear()
        {
            Text = String.Empty;
            CellList.Clear();
            ContextTag = null;
            ContextNames = String.Empty;
        }

        /// <summary>
        /// 顯示文字。可能是一個英數字、中文字、全形標點符號、或雙字元標點符號，例如：破折號。
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// 保留最初的文字。
        /// 此屬性可用來判斷當前的 BrailleWord 是不是從 context tag 的起始或結束標籤轉換而成。
        /// 甚至將來可能利用此屬性將已經轉換好的點字文件還原成純文字。
        /// </summary>
        [DataMember]
        public string OriginalText { get; private set; }

        public int CellCount
        {
            get { return CellList.Count; }
        }
        
        public List<BrailleCell> Cells
        {
            get
            {
                return CellList.Items;
            }
        }

        /// <summary>
        /// 點字串列。
        /// </summary>
        [DataMember]
        public BrailleCellList CellList { get; set; }

        [DataMember]
        public string PhoneticCode
        {
            get
            {
                if (String.IsNullOrEmpty(m_PhoneticCode))   // 這是為了向下相容，舊版沒有 PhoneticCode 屬性。
                {
                    // 若沒有注音字根，則傳回空字串。
                    if (m_PhoneticCodes == null || m_PhoneticCodes.Count < 1 || m_ActivePhoneticIndex < 0)
                    {
                        return "";
                    }
                    return m_PhoneticCodes[m_ActivePhoneticIndex];
                }
                return m_PhoneticCode;
            }
            set
            {
                if (m_PhoneticCode == value)
                    return;
                m_PhoneticCode = value;
/*
                // 若沒有注音字根，則增加一個。
                if (m_PhoneticCodes.Count < 1)
                {
                    m_PhoneticCodes.Add(value);
                    m_ActivePhoneticIndex = 0;
                }
                else
                {   // 否則設定作用中的注音字根索引
                    int i = m_PhoneticCodes.IndexOf(value);
                    if (i < 0)
                    {
                        m_PhoneticCodes.Add(value);
                        i = m_PhoneticCodes.Count - 1;
                        System.Diagnostics.Trace.WriteLine("指定給 BrailleWord.PhoneticCode 的注音字根不存在! 已自動加入。");
                    }
                    m_ActivePhoneticIndex = i;
                }
*/
            }
        }

        [DataMember]
        public bool IsPolyphonic
        {
            get 
            {
                if (m_PhoneticCodes != null && m_PhoneticCodes.Count > 1)   // for 向下相容.
                {
                    return true;
                }
                return m_IsPolyphonic; 
            }
            set { m_IsPolyphonic = value; }
        }

        /// <summary>
        /// 設定/判別在斷行時是否能斷在這個字。
        /// </summary>
        [DataMember]
        public bool DontBreakLineHere { get; set; }

        public List<string> PhoneticCodes
        {
            get
            {
                if (m_PhoneticCodes == null)
                {
                    m_PhoneticCodes = new List<string>();
                }
                return m_PhoneticCodes;
            }
        }

        public int ActivePhoneticIndex
        {
            get
            {
                return m_ActivePhoneticIndex;
            }
            set
            {
                if (value >= m_PhoneticCodes.Count)
                    throw new ArgumentOutOfRangeException();
                m_ActivePhoneticIndex = value;
            }
        }

        /// <summary>
        /// 語言國別。用來識別是中文還是英文。
        /// </summary>
        public BrailleLanguage Language { get; set; }

        /// <summary>
        /// 以空白區隔的 context 名稱，不含 tag 的角括號。例如 "數學 私名號"。
        /// </summary>
        [DataMember]
        public string ContextNames { get; set; } 

        /// <summary>
        /// ContextTag 屬性會用在轉換點字的過程中暫時保留的語境標籤。
        /// 這些語境標籤在整個轉點字程序完成時都會被移除（或轉換成對應的點字）。
        /// 是否可序列化：否。
        /// </summary>
        public IContextTag ContextTag { get; set; }


        /// <summary>
        /// 在轉點字過程中可能需要得知當前的 BrailleWord 物件是不是某 context tag 的起始標籤。
        /// </summary>
        public bool IsContextBeginTag()
        {
            return IsContextTag && XmlTagHelper.IsBeginTag(Text);
        }

        /// <summary>
        /// 在轉點字過程中可能需要得知當前的 BrailleWord 物件是不是某 context tag 的結束標籤。
        /// </summary>
        public bool IsContextEndTag()
        {
            return IsContextTag && XmlTagHelper.IsEndTag(Text);
        }

        /// <summary>
        /// 此 word 是否在指定的語境中？
        /// </summary>
        /// <param name="contextName">語境標籤名稱。若有帶左右角括號，會自動去除括號。</param>
        /// <returns></returns>
        public bool IsInContext(string contextName)
        {
            contextName = XmlTagHelper.RemoveBracket(contextName);
            return (ContextNames.IndexOf(contextName) >= 0);
        }

        public void EnterContext(string contextName)
        {
            contextName = XmlTagHelper.RemoveBracket(contextName);
            if (ContextNames.IndexOf(contextName) >= 0)
                return;
            ContextNames = $"{ContextNames} {contextName}";
        }

        public void LeaveContext(string contextName)
        {
            contextName = XmlTagHelper.RemoveBracket(contextName);
            if (ContextNames.IndexOf(contextName) < 0)
                return;
            var names = ContextNames.Replace(contextName, String.Empty).Split(' ');
            var sb = new StringBuilder();
            foreach (var s in names)
            {
                sb.Append($"{s} ");
            }
            ContextNames = sb.ToString().TrimEnd();
        }

        public void SetPhoneticCodes(string[] phCodes)
        {
            m_PhoneticCodes.Clear();
            m_PhoneticCodes.AddRange(phCodes);
        }

        /// <summary>
        /// 建立一個新的 BrailleWord 物件，並將自己的內容完整複製到新的物件。
        /// </summary>
        /// <returns></returns>
        public BrailleWord Copy()
        {
            BrailleWord newBrWord = new BrailleWord(Text);
            newBrWord.Language = Language;
            newBrWord.DontBreakLineHere = DontBreakLineHere;
            newBrWord.NoDigitCell = NoDigitCell;
            newBrWord.NoSpace = NoSpace;
            newBrWord.PhoneticCode = PhoneticCode;
            newBrWord.IsPolyphonic = IsPolyphonic;
            newBrWord.IsContextTag = IsContextTag;
            newBrWord.IsConvertedFromTag = IsConvertedFromTag;
            newBrWord.ContextTag = ContextTag;
            newBrWord.ContextNames = ContextNames;

            foreach (BrailleCell brCell in CellList.Items)
            {
                newBrWord.Cells.Add(brCell);
            }

            return newBrWord;
        }

        /// <summary>
        /// 將指定的 BrailleWord 內容完整複製給自己。
        /// </summary>
        /// <param name="brWord"></param>
        public void Copy(BrailleWord brWord)
        {
            if (brWord == null)
            {
                throw new ArgumentNullException("參數 brWord 不可為 null!");
            }

            Text = brWord.Text;
            Language = brWord.Language;

            CellList.Clear();
            foreach (BrailleCell brCell in brWord.CellList.Items)
            {
                CellList.Add(brCell);
            }

            PhoneticCode = brWord.PhoneticCode;
            IsPolyphonic = brWord.IsPolyphonic;
            IsContextTag = brWord.IsContextTag;
            IsConvertedFromTag = brWord.IsConvertedFromTag;
            ContextTag = brWord.ContextTag;
            ContextNames = brWord.ContextNames;
/*
            // 複製所有注音字根與點字串列, for 向下相容.
            if (brWord.PhoneticCodes != null)
            {
                m_PhoneticCodes.Clear();
                m_PhoneticCodes.AddRange(brWord.PhoneticCodes);
                m_ActivePhoneticIndex = brWord.ActivePhoneticIndex;
            }
 */
        }

        /// <summary>
        /// 把指定的點字字串（16進位）轉成 BrailleCell 物件，並加入點字串列中。
        /// </summary>
        /// <param name="brCodes">欲加入串列的點字碼 16 進位字串。</param>
        public void AddCells(string brCodes)
        {
            if (String.IsNullOrEmpty(brCodes))
            {
                return;
            }

            for (int i = 0; i < brCodes.Length; i += 2)
            {
                string s = brCodes.Substring(i, 2);
                byte aByte = StrHelper.HexStrToByte(s);
                BrailleCell cell = BrailleCell.GetInstance(aByte);
                CellList.Add(cell);
            }
        }

        public void AddCellsFromPositionNumbers(string positionNumberString)
        {
            if (String.IsNullOrEmpty(positionNumberString))
                return;
            var numbers = positionNumberString.Split(' ');
            foreach (string num in numbers)
            {
                var cell = BrailleCell.GetInstanceFromPositionNumberString(num);
                CellList.Add(cell);
            }
        }

        public bool IsWhiteSpace
        {
            get
            {
                if (Text.Length == 1)
                {
                    if (Char.IsWhiteSpace(Text[0]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsLetter
        {
            get
            {
                if (Text.Length == 1)
                {
                    if (CharHelper.IsAsciiLetter(Text[0]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsDigit
        {
            get
            {
                if (Text.Length == 1)
                {
                    if (CharHelper.IsAsciiDigit(Text[0]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsLetterOrDigit
        {
            get
            {
                if (Text.Length == 1)
                {
                    if (CharHelper.IsAsciiLetterOrDigit(Text[0]))
                    {
                        return true;
                    }
                }
                return false;
            }
        }


        /// <summary>
        /// 傳回此點字是否為中文字（以是否有注音字根為判斷依據）。
        /// 注意：跟 Language 屬性無關，Language 屬性包含中文標點符號
        /// </summary>
        /// <returns></returns>
        public bool IsChinese
        {
            get
            {
                if (!String.IsNullOrEmpty(m_PhoneticCode))
                    return true;
                if (!String.IsNullOrEmpty(Text) && Zhuyin.IsTone(Text[0]))
                    return true;
                if (m_PhoneticCodes != null)
                {
                    return (m_PhoneticCodes.Count > 0);
                }
                return false;
            }
        }

        /// <summary>
        /// 建立一個新的空方點字物件。
        /// </summary>
        /// <returns></returns>
        public static BrailleWord NewBlank()
        {
            return new BrailleWord(" ", BrailleCellCode.Blank);
        }

        /// <summary>
        /// 檢查指定的 BrailleWord 是否為空方。
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static bool IsBlank(BrailleWord brWord)
        {
            if (brWord.Equals(BlankWord))
                return true;
            return false;
        }

        /// <summary>
        /// 檢查指定的 BrailleWord 物件是否沒有包含任何有意義的資料（空方算有意義的資料）。
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static bool IsEmpty(BrailleWord brWord)
        {
            if (StrHelper.IsEmpty(brWord.Text) && brWord.CellCount < 1) 
            {
                // 文字為空字串，且沒有任何點字物件，即視為空的 BrailleWord 物件.
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否為語境標籤。此屬性與 ContextNames 沒有絕對關係。
        /// 若為 true，代表此 BraillWord 在原始文件中是一個語境標籤，而且目前還沒有轉換成對應的點字。
        /// 若為 false，代表此 BraillWord 不是語境標籤，或者已經被轉換成對應的點字。
        /// </summary>
        [DataMember]
        public bool IsContextTag
        {
            get { return m_IsContextTag; }
            set
            {
                m_IsContextTag = value;
                if (m_IsContextTag)    // 如果是語境標籤，就要清除點字串列
                {
                    CellList.Clear();
                }
                else
                {
                    ContextTag = null; // 若不是語境標籤，應清除 ContextTag 參考
                }
            }
        }

        /// <summary>
        ///  此物件是否由 context tag 所衍生（不是 context tag，但是因為 context tag 而額外增加的文字）。
        /// </summary>
        [DataMember]
        public bool IsConvertedFromTag { get; set; }

        /// <summary>
        /// 是否不加數符。
        /// </summary>
        public bool NoDigitCell { get; set; }

        /// <summary>
        /// 是否前後都不加空方。
        /// </summary>
        public bool NoSpace { get; set; }

        /// <summary>
        /// 是否不要套用英文字母大寫規則。最初設計目的是給 UrlConverter 使用。
        /// </summary>
        public bool NoCapitalRule { get; set; }

        /// <summary>
        /// 是否為英語音標（用來判斷不要加空方）.
        /// </summary>
        public bool IsEngPhonetic { get; set; }

        /// <summary>
        /// 檢查指定的 BrailleWord 是否為數字編號的起始點字。亦即以 # 開頭的數字。
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static bool IsOrderedListItem(BrailleWord brWord)
        {
            if (brWord.Cells.Count < 2)
                return false;
            if (brWord.Cells[0].Value == (byte)BrailleCellCode.Digit) // 以數字點開頭？
            {
                // 接著比較第二方是否為上位點，注意這裡的上位點數值並未使用查表，
                // 而是寫死在程式裡。參考: BraillTableEng.xml。
                // TODO: 改成查表。
                byte value = brWord.Cells[1].Value;
                switch (value) 
                {
                    case 0x01:  case 0x03:
                    case 0x09:  case 0x19:
                    case 0x11:  case 0x0B:
                    case 0x1B:  case 0x13:
                    case 0x4A:  case 0x1A:
                        return true;
                    default:
                        break;
                }
            }
            return false;
        }

        /// <summary>
        /// 檢查指定的 BrailleWord 是否為國語標點符號。
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static bool IsChinesePunctuation(BrailleWord brWord)
        {
            ChineseBrailleTable chtBrlTbl = ChineseBrailleTable.GetInstance();
            string brCode = chtBrlTbl.GetPunctuationCode(brWord.Text);
            if (String.IsNullOrEmpty(brCode))
                return false;
            return true;
        }
    }
}
