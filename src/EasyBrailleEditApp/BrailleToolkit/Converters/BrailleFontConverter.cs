using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Globalization;
using System.Reflection;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// 點字字型轉換器。用來將點字碼轉換成點字字型對應的 ASCII 字元。
    /// </summary>
    public sealed class BrailleFontConverter
    {
        private static FrozenDictionary<string, string> m_FontTable;

        private BrailleFontConverter()
        {
        }

        static BrailleFontConverter()
        {
            m_FontTable = LoadFromResource();
        }

        /// <summary>
        /// 載入字型對應表。
        /// 檔案內容的每一列格式為 xx=yy，其中 xx 為點字碼，yy 為對應之字型碼，兩者皆為 16 進制，例如：1C=3E。
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
                m_FontTable = LoadFromStreamReader(sr);
            }
        }

        /// <summary>
        /// 從組件的資源中載入。
        /// </summary>
        public static FrozenDictionary<string, string> LoadFromResource()
        {
            const string resourceName = "BrailleToolkit.Data.BrailleFontTbl.txt";
            Assembly asmb = Assembly.GetExecutingAssembly();
            Stream? stream = asmb.GetManifestResourceStream(resourceName);
            if (stream == null)
                throw new Exception("找不到資源: " + resourceName);

            using (stream)
            {
                using (StreamReader sr = new StreamReader(stream!))
                {
                    return LoadFromStreamReader(sr);
                }
            }
        }

        private static FrozenDictionary<string, string> LoadFromStreamReader(StreamReader sr)
        {
            var dict = new Dictionary<string, string>();
            string? s;
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
                dict.Add(values[0], values[1]);
            }
            sr.Close();
            return dict.ToFrozenDictionary();
        }

        /// <summary>
        /// 將一列點字轉成對應的點字字型 ASCII 字串，以便顯示於螢幕上。
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
                sb.Append(BrailleFontConverter.ToString(brWord));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 將一個 BraillWord 物件轉換可對應的點字字型 ASCII 字串，以便顯示於螢幕上。
        /// Note: 如果你需要轉換破音字的其他注音字根的點字，請呼叫另一個 ToString 版本：
        /// public string ToString(BrailleCellList cellList)
        /// </summary>
        /// <param name="brWord"></param>
        /// <returns></returns>
        public static string ToString(BrailleWord brWord)
        {
            return BrailleFontConverter.ToString(brWord.CellList);
        }

        /// <summary>
        /// 將 BrailleCellList 串列轉成對應的點字字型 ASCII 字串，以便顯示於螢幕上。
        /// 範例：
        /// string s = brFontConvert.ToString(brWord.CandidatePhoneticCellLists);
        /// </summary>
        /// <param name="cellList"></param>
        /// <returns></returns>
        public static string ToString(BrailleCellList cellList)
        {
            if (m_FontTable.Count < 1)
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
        /// 將點字碼（兩位數16進位字串）轉換成對應的點字字型 ASCII 字元，以便顯示於 UI。
        /// </summary>
        /// <param name="brCode">點字碼，兩位數16進位字串，例如：4E。</param>
        /// <returns>對應的點字字型 ASCII 字元。</returns>
        public static char ToChar(string brCode)
        {
            string? fontCode = ToFontCode(brCode);
            if (String.IsNullOrEmpty(fontCode))
                throw new Exception("找不到對應的點字字型碼: " + brCode);
           
            byte fontValue = Byte.Parse(fontCode, NumberStyles.HexNumber);
            char ch = Convert.ToChar(fontValue);
            return ch;
        }

        /// <summary>
        /// 傳回指定點字碼所對應之點字字型碼。
        /// </summary>
        /// <param name="brCode">點字碼，兩位數16進位字串，例如：4E。</param>
        /// <returns>點字字型碼，兩位數16進位字串。</returns>
        public static string? ToFontCode(string brCode)
        {
            if (m_FontTable.TryGetValue(brCode, out var value))
            {
                return value;
            }
            return null;
        }

        /// <summary>
        /// 傳入點字字型碼，傳回對應的點字碼。
        /// </summary>
        /// <param name="fontCode">點字碼，兩位數16進位字串，例如：3F。</param>
        /// <returns>點字碼，兩位數16進位字串。</returns>
        public static string? ToBrailleCode(string fontCode)
        {
            foreach (var item in m_FontTable)
            {
                if (item.Value.Equals(fontCode))
                    return item.Key;
            }
            return null;
        }

        /// <summary>
        /// 傳入數字，傳回對應的點字字型碼。
        /// </summary>
        /// <param name="number">數字。</param>
        /// <param name="upperPosition">是否採用上位點。</param>
        /// <returns>點字字型碼。</returns>
        public static string GetDigitFontCode(int number, bool upperPosition)
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
					chars[i] = 'J';
				}
				else
				{
					chars[i] = (char)(((int)chars[i]) + 16);
				}
            }
            return new String(chars);
        }
    }
}
