using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Reflection;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// 點字碼轉換器。用來將點字碼轉換成可輸出至點字印表機和「超點」點字觸摸器的 ASCII 字元。
    /// </summary>
    public sealed class BrailleCharConverter
    {
        private static Dictionary<string, string> m_CharTable;

        private BrailleCharConverter()
        {            
        }

        static BrailleCharConverter()
        {
            m_CharTable = new Dictionary<string, string>();
            LoadFromResource();
        }

        /// <summary>
        /// 載入點字字元對應表。
        /// 檔案內容的每一列格式為 xx=yy，其中 xx 為點字碼，yy 為對應之點字碼，兩者皆為 16 進制，例如：1C=3E。
        /// </summary>
        /// <param name="filename"></param>
        public static void Load(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException("檔案不存在!", filename);
            }
            using (StreamReader sr = new StreamReader(filename))
            {
                LoadFromStreamReader(sr);
            }
        }

        /// <summary>
        /// 從組件的資源中載入。
        /// </summary>
        public static void LoadFromResource()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            string resourceName = "BrailleToolkit.Data.BrailleFontTbl.txt";
            Stream stream = asmb.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception("找不到資源: " + resourceName);
            using (stream)
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    LoadFromStreamReader(sr);
                }
            }
        }

        private static void LoadFromStreamReader(StreamReader sr)
        {
            string s;
            string[] values;
            while (true)
            {
                s = sr.ReadLine();
                if (s == null)
                    break;
                s = s.Trim();
                if (s.Length < 2)
                    continue;
                if (s[0] == ';')    // 忽略註解.
                    continue;
                values = s.Split('=');
                m_CharTable.Add(values[0], values[1]);
            }
            sr.Close();
        }

        /// <summary>
        /// 將一列點字轉成可對應的點字 ASCII 字串，以便輸出至點字印表機。
        /// </summary>
        /// <param name="brLine"></param>
        /// <returns></returns>
        public static string ToString(BrailleLine brLine)
        {
            if (brLine == null)
                return "";

            StringBuilder sb = new StringBuilder();

            foreach (BrailleWord brWord in brLine.Words)
            {
                sb.Append(BrailleCharConverter.ToString(brWord));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將一個 BraillWord 物件轉換成對應的點字 ASCII 字串，以便輸出至點字印表機。
        /// Note: 如果你需要轉換破音字的其他注音字根的點字，請呼叫另一個 ToString 版本：
        /// public string ToString(BrailleCellList cellList)
        /// </summary>
        /// <param name="brCell"></param>
        /// <returns></returns>
        public static string ToString(BrailleWord brWord)
        {
            return ToString(brWord.CellList);
        }

        /// <summary>
        /// 將 BrailleCellList 串列轉成對應的點字 ASCII 字串，以便輸出至點字印表機。
        /// 範例：
        /// string s = brFontConvert.ToString(brWord.CandidatePhoneticCellLists);
        /// </summary>
        /// <param name="cellList"></param>
        /// <returns></returns>
        public static string ToString(BrailleCellList cellList)
        {
            if (m_CharTable.Count < 1)
            {
                throw new Exception("尚未載入字型對應表!");
            }

            StringBuilder sb = new StringBuilder();

            foreach (BrailleCell brCell in cellList.Items)
            {
                sb.Append(ToChar(brCell.ToString()));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將內部點字碼（兩位數16進位字串）轉換成對應的點字字元，以便輸出至點字印表機。
        /// </summary>
        /// <param name="brCode">點字碼，兩位數16進位字串，例如：4E。</param>
        /// <returns></returns>
        public static char ToChar(string brailleHexCode)
        {
            string charCode = ToBrailleCharCode(brailleHexCode);
            if (String.IsNullOrEmpty(charCode))
                throw new Exception("找不到對應的點字字型碼: " + brailleHexCode);
           
            byte charValue = Byte.Parse(charCode, NumberStyles.HexNumber);
            char ch = Convert.ToChar(charValue);
            return ch;
        }

        /// <summary>
        /// 傳入內部點字碼，傳回對應之標準點字碼（點字印表機和點字觸摸器的點字碼）。
        /// </summary>
        /// <param name="brCode">內部點字碼，兩位數16進位字串，例如：4E。</param>
        /// <returns>點字字型碼，兩位數16進位字串。</returns>
        public static string ToBrailleCharCode(string brailleHexCode)
        {
            if (m_CharTable.ContainsKey(brailleHexCode))
            {
                return m_CharTable[brailleHexCode];
            }
            return null;
        }

        /// <summary>
        /// 傳入點字字元碼，傳回對應的內部點字碼。
        /// </summary>
        /// <param name="fontCode">點字碼，兩位數16進位字串，例如：3F。</param>
        /// <returns>點字碼，兩位數16進位字串。</returns>
        public static string ToBrailleHexCode(string brailleCharCode)
        {
            foreach (var item in m_CharTable)
            {
                if (item.Value.Equals(brailleCharCode))
                    return item.Key;
            }
            return null;
        }

        /// <summary>
        /// 傳入數字，傳回對應的點字字元碼。
        /// </summary>
        /// <param name="upperPosition">是否採用上位點。</param>
        /// <returns></returns>
        public static string GetDigitCharCode(int number, bool upperPosition)
        {
            string s = number.ToString();

            if (!upperPosition)
            {
                return s;
            }
            // 數字的 1,2,3,4,5,6,7,8,9,0 的上位點就是 a,b,c,d,e,f,g,h,i,j 的點字。
            char[] chars = s.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
				if (chars[i] == '0')
				{
					chars[i] = 'j';
				}
				else
				{
					chars[i] = (char)(((int)chars[i]) + 48);
				}
            }
            return new String(chars);
        }
    }
}
