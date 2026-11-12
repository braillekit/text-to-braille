using System;
using System.Collections.Generic;
using System.Collections.Frozen;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Data
{
    /// <summary>
    /// 從 XML 檔案讀取點字對照表，並提供搜尋功能。
    /// </summary>
    public class XmlBrailleTable : BrailleTableBase
    {
        private readonly record struct BrailleTableLookupKey(string Text, string Type);

        private string m_FileName;
        private bool m_Loaded;
        private BrailleTableEntry[] m_Entries;
        private FrozenDictionary<string, BrailleTableEntry> m_EntriesByText;
        private FrozenDictionary<BrailleTableLookupKey, BrailleTableEntry> m_EntriesByTypeAndText;
        private FrozenDictionary<string, BrailleTableEntry[]> m_EntriesByType;

        /// <summary>
        /// 取得內部的點字表項目。
        /// </summary>
        public IReadOnlyList<BrailleTableEntry> Entries => m_Entries;

        /// <summary>
        /// 建構函式。
        /// </summary>
        public XmlBrailleTable()
        {
            m_Entries = Array.Empty<BrailleTableEntry>();
            m_EntriesByText = FrozenDictionary<string, BrailleTableEntry>.Empty;
            m_EntriesByTypeAndText = FrozenDictionary<BrailleTableLookupKey, BrailleTableEntry>.Empty;
            m_EntriesByType = FrozenDictionary<string, BrailleTableEntry[]>.Empty;
            m_FileName = string.Empty;
        }

        /// <summary>
        /// 建構函式。
        /// </summary>
        /// <param name="filename">XML 檔案名稱。</param>
        public XmlBrailleTable(string filename)
            : this()
        {
            Load(filename);
        }

        /// <summary>
        /// 載入點字對照表。
        /// </summary>
        public override void Load()
        {
            Load(m_FileName);
        }

        /// <summary>
        /// 從 XML 檔案載入點字對照表。
        /// </summary>
        /// <param name="filename"></param>
        public override void Load(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("檔名未指定!");
            }

            if (m_Loaded && (string.Compare(m_FileName, filename, true, CultureInfo.CurrentUICulture) == 0))
            {
                return;
            }

            using (StreamReader sr = new StreamReader(filename))
            {
                LoadFromTextReader(sr);
                m_FileName = filename;
            }
        }

        /// <summary>
        /// 從指定組件資源載入點字對照表。
        /// </summary>
        /// <param name="asmb"></param>
        /// <param name="resourceName"></param>
        public override void LoadFromResource(Assembly asmb, string resourceName)
        {
            Stream? stream = asmb.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new Exception("XmlBrailleTable.LoadFromResource 找不到資源: " + resourceName);
            }

            using (stream)
            using (StreamReader sr = new StreamReader(stream))
            {
                LoadFromTextReader(sr);
            }
        }

        /// <summary>
        /// 從預設的資源名稱（即物件的類別名稱加上 .xml 副檔名）載入點字對照表。
        /// </summary>
        public virtual void LoadFromResource()
        {
            Assembly asmb = Assembly.GetExecutingAssembly();
            string resName = GetType().FullName + ".xml"; // Note: 這種寫法可以避免寫死的 namsepace，而且用於 obfuscator 時也能正常運作。
            LoadFromResource(asmb, resName);
        }

        private void LoadFromTextReader(TextReader reader)
        {
            XDocument document = XDocument.Load(reader);
            XElement? root = document.Root;
            if (root == null)
            {
                throw new Exception("點字對照表 XML 格式不正確: 缺少根節點。");
            }

            var entries = new List<BrailleTableEntry>();
            foreach (XElement element in root.Elements())
            {
                if (!string.Equals(element.Name.LocalName, "symbol", StringComparison.Ordinal))
                {
                    continue;
                }

                entries.Add(CreateEntry(element));
            }

            BuildIndexes(entries);
            m_Loaded = true;
        }

        /// <summary>
        /// 從 XML 字串載入點字對照表。
        /// </summary>
        /// <param name="xml"></param>
        public void LoadFromXmlString(string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                LoadFromTextReader(sr);
            }
        }

        /// <summary>
        /// 從 XML 元素建立單一點字表項目。
        /// </summary>
        protected virtual BrailleTableEntry CreateEntry(XElement element)
        {
            string text = GetRequiredAttributeValue(element, "text");
            string dots = GetOptionalAttributeValue(element, "dots") ?? string.Empty;
            string? dots2 = GetOptionalAttributeValue(element, "dots2");
            string code = ConvertDotsToCode(dots);
            string? code2 = ConvertDots2ToCode2(dots2);

            return new BrailleTableEntry(
                text,
                dots,
                code,
                GetOptionalAttributeValue(element, "type"),
                dots2,
                code2,
                GetBooleanAttributeValue(element, "joined"),
                GetBooleanAttributeValue(element, "mono"),
                GetOptionalAttributeValue(element, "rule"),
                GetOptionalAttributeValue(element, "description") ?? GetOptionalAttributeValue(element, "deccription"));
        }

        /// <summary>
        /// 建立各種查詢索引。
        /// </summary>
        protected virtual void BuildIndexes(IReadOnlyList<BrailleTableEntry> entries)
        {
            var entriesByText = new Dictionary<string, BrailleTableEntry>(StringComparer.Ordinal);
            var entriesByTypeAndText = new Dictionary<BrailleTableLookupKey, BrailleTableEntry>();
            var entriesByType = new Dictionary<string, List<BrailleTableEntry>>(StringComparer.Ordinal);

            for (int i = 0; i < entries.Count; i++)
            {
                BrailleTableEntry entry = entries[i];

                if (!entriesByText.TryAdd(entry.Text, entry))
                {
                    throw new Exception("點字對照表的資料不正確! 發現重複的 text: " + entry.Text);
                }

                if (!string.IsNullOrEmpty(entry.Type))
                {
                    var key = new BrailleTableLookupKey(entry.Text, entry.Type);
                    if (!entriesByTypeAndText.TryAdd(key, entry))
                    {
                        throw new Exception("點字對照表的資料不正確! 發現重複的 type/text: " + entry.Type + "/" + entry.Text);
                    }

                    if (!entriesByType.TryGetValue(entry.Type, out List<BrailleTableEntry>? list))
                    {
                        list = new List<BrailleTableEntry>();
                        entriesByType.Add(entry.Type, list);
                    }
                    list.Add(entry);
                }
            }

            var frozenEntriesByType = new Dictionary<string, BrailleTableEntry[]>(StringComparer.Ordinal);
            foreach (KeyValuePair<string, List<BrailleTableEntry>> pair in entriesByType)
            {
                frozenEntriesByType.Add(pair.Key, pair.Value.ToArray());
            }

            m_Entries = entries as BrailleTableEntry[] ?? new List<BrailleTableEntry>(entries).ToArray();
            m_EntriesByText = entriesByText.ToFrozenDictionary(StringComparer.Ordinal);
            m_EntriesByTypeAndText = entriesByTypeAndText.ToFrozenDictionary();
            m_EntriesByType = frozenEntriesByType.ToFrozenDictionary(StringComparer.Ordinal);
        }

        /// <summary>
        /// 將點位字串轉換為點字碼。
        /// </summary>
        protected virtual string ConvertDotsToCode(string dots)
        {
            if (string.IsNullOrWhiteSpace(dots))
            {
                return "00";
            }

            return BrailleCellHelper.PositionNumbersToHexString(dots.Split(' '));
        }

        /// <summary>
        /// 將第二方點位字串轉換為點字碼。
        /// </summary>
        protected virtual string? ConvertDots2ToCode2(string? dots2)
        {
            if (string.IsNullOrWhiteSpace(dots2))
            {
                return null;
            }

            return BrailleCellHelper.PositionNumbersToHexString(dots2.Split(' '));
        }

        private static string GetRequiredAttributeValue(XElement element, string attributeName)
        {
            string? value = GetOptionalAttributeValue(element, attributeName);
            if (value == null)
            {
                throw new Exception("點字對照表 XML 格式不正確: 缺少屬性 " + attributeName);
            }

            return value;
        }

        private static string? GetOptionalAttributeValue(XElement element, string attributeName)
        {
            XAttribute? attribute = element.Attribute(attributeName);
            return attribute?.Value;
        }

        private static bool GetBooleanAttributeValue(XElement element, string attributeName)
        {
            string? value = GetOptionalAttributeValue(element, attributeName);
            return bool.TryParse(value, out bool result) && result;
        }

        /// <summary>
        /// 檢查點字對照表是否已經載入，若否，則丟出 exception。
        /// </summary>
        protected void CheckLoaded()
        {
            if (!m_Loaded)
            {
                throw new Exception("點字對照表尚未載入資料!");
            }
        }

        /// <summary>
        /// 檢查是否為合法的點字碼。
        /// </summary>
        /// <param name="code">點字碼的十六進位字串。例如："A0"。</param>
        protected void CheckCode(string code)
        {
            if (!string.IsNullOrWhiteSpace(code) && code.Length % 2 != 0)
            {
                throw new Exception("點字對照表的資料不正確! code=" + code);
            }
        }

        /// <summary>
        /// 索引子。從文字符號取得對應的點字碼（16 進位）字串。
        /// </summary>
        /// <param name="text">文字符號，例如：ㄅ、：。</param>
        /// <returns>點字碼字串，若找不到對應的符號，會丟出例外。</returns>
        /// <remarks>如果你希望找不到對應的點字碼時不要丟出例外，而是傳回空字串，請使用 Find 方法。</remarks>
        public override string this[string text]
        {
            get
            {
                string? brCode = Find(text);
                if (string.IsNullOrEmpty(brCode))
                {
                    throw new Exception("找不到對應的點字碼: " + text);
                }
                return brCode;
            }
        }

        /// <summary>
        /// 搜尋對應的點字表項目。
        /// </summary>
        /// <param name="text">文字。</param>
        /// <param name="type">類型。</param>
        /// <returns>點字表項目。</returns>
        protected virtual BrailleTableEntry? FindEntry(string text, string? type = null)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                if (m_EntriesByText.TryGetValue(text, out BrailleTableEntry entry))
                {
                    return entry;
                }
                return null;
            }

            var key = new BrailleTableLookupKey(text, type);
            if (m_EntriesByTypeAndText.TryGetValue(key, out BrailleTableEntry typedEntry))
            {
                return typedEntry;
            }
            return null;
        }

        /// <summary>
        /// 搜尋指定類型的所有項目。
        /// </summary>
        /// <param name="type">類型。</param>
        /// <returns>符合類型的項目。</returns>
        protected virtual IReadOnlyList<BrailleTableEntry> FindEntriesByType(string type)
        {
            if (m_EntriesByType.TryGetValue(type, out BrailleTableEntry[]? entries))
            {
                return entries;
            }

            return Array.Empty<BrailleTableEntry>();
        }

        /// <summary>
        /// 搜尋符合指定條件的點字表項目。
        /// </summary>
        /// <param name="text">文字。</param>
        /// <param name="type">類型。</param>
        /// <param name="predicate">附加條件。</param>
        /// <returns>點字表項目。</returns>
        protected virtual BrailleTableEntry? FindEntry(string text, string type, Func<BrailleTableEntry, bool> predicate)
        {
            BrailleTableEntry? entry = FindEntry(text, type);
            if (entry == null)
            {
                return null;
            }

            if (!predicate(entry.Value))
            {
                return null;
            }

            return entry;
        }

        /// <summary>
        /// 搜尋某個文字符號，並傳回對應的點字碼。
        /// </summary>
        /// <param name="text">欲搜尋的符號。</param>
        /// <param name="type">限定欲搜尋的符號類型。若不同類型當中存在相同的文字符號，便應指定此參數，以確保找到正確的符號。</param>
        /// <returns>若有找到，則傳回對應的點字碼，否則傳回 null。</returns>
        /// <remarks>如果你希望找不到對應的點字碼時丟出例外，請使用索引子。</remarks>
        public override string? Find(string text, string? type = null)
        {
            CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text, type);
            if (entry == null)
            {
                return null;
            }

            string code = entry.Value.Code;
            CheckCode(code);
            return code;
        }

        /// <summary>
        /// 取得點位字串陣列。
        /// </summary>
        /// <param name="text">文字。</param>
        /// <param name="type">類型。</param>
        /// <returns>點位字串陣列。</returns>
        public override string[]? GetDots(string text, string? type = null)
        {
            CheckLoaded();

            BrailleTableEntry? entry = FindEntry(text, type);
            if (entry == null)
            {
                return null;
            }

            return entry.Value.Dots.Split(' ');
        }

        /// <summary>
        /// 檢查文字是否存在於對照表中。
        /// </summary>
        /// <param name="text">文字。</param>
        /// <returns>若存在則傳回 true，否則傳回 false。</returns>
        public override bool Exists(string text)
        {
            CheckLoaded();
            return m_EntriesByText.ContainsKey(text);
        }
    }
}
