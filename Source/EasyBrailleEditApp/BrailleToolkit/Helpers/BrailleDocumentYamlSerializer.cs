using System;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BrailleToolkit.Helpers
{
    /// <summary>
    /// 提供將 BrailleDocument 物件序列化為 YAML 格式或從中反序列化的靜態輔助函式。
    /// </summary>
    public static class BrailleDocumentYamlSerializer
    {
        private static ISerializer CreateSerializer()
        {
            return new SerializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .WithTypeConverter(new BrailleCellYamlTypeConverter())
                .DisableAliases()
                .Build();
        }

        private static IDeserializer CreateDeserializer()
        {
            return new DeserializerBuilder()
                .WithNamingConvention(PascalCaseNamingConvention.Instance)
                .WithTypeConverter(new BrailleCellYamlTypeConverter())
                .IgnoreUnmatchedProperties() // To be safe with potential version mismatches or extra fields
                .Build();
        }

        /// <summary>
        /// 將 BrailleDocument 物件序列化為 YAML 字串。
        /// </summary>
        /// <param name="doc">要序列化的 BrailleDocument 物件。</param>
        /// <returns>表示文件的 YAML 字串。</returns>
        public static string Serialize(BrailleDocument doc)
        {
            var serializer = CreateSerializer();
            return serializer.Serialize(doc);
        }

        /// <summary>
        /// 將 BrailleDocument 物件序列化至指定的 TextWriter。
        /// </summary>
        /// <param name="doc">要序列化的 BrailleDocument 物件。</param>
        /// <param name="writer">要寫入 YAML 的 TextWriter。</param>
        public static void Serialize(BrailleDocument doc, TextWriter writer)
        {
            var serializer = CreateSerializer();
            serializer.Serialize(writer, doc);
        }

        /// <summary>
        /// 從 YAML 字串反序列化為 BrailleDocument 物件。
        /// </summary>
        /// <param name="yaml">包含 YAML 資料的字串。</param>
        /// <returns>反序列化後的 BrailleDocument 物件。</returns>
        public static BrailleDocument Deserialize(string yaml)
        {
            var deserializer = CreateDeserializer();
            return deserializer.Deserialize<BrailleDocument>(yaml);
        }

        /// <summary>
        /// 從指定的 TextReader 反序列化為 BrailleDocument 物件。
        /// </summary>
        /// <param name="reader">要從中讀取 YAML 的 TextReader。</param>
        /// <returns>反序列化後的 BrailleDocument 物件。</returns>
        public static BrailleDocument Deserialize(TextReader reader)
        {
            var deserializer = CreateDeserializer();
            return deserializer.Deserialize<BrailleDocument>(reader);
        }

        /// <summary>
        /// 將 BrailleDocument 物件儲存至 YAML 檔案。
        /// </summary>
        /// <param name="doc">要儲存的 BrailleDocument 物件。</param>
        /// <param name="filename">目標檔案的完整路徑。</param>
        public static void SaveToYamlFile(BrailleDocument doc, string filename)
        {
            using (var writer = new StreamWriter(filename, false, Encoding.UTF8))
            {
                Serialize(doc, writer);
            }
        }

        /// <summary>
        /// 從 YAML 檔案載入 BrailleDocument 物件。
        /// </summary>
        /// <param name="filename">來源檔案的完整路徑。</param>
        /// <returns>從檔案載入的 BrailleDocument 物件。</returns>
        public static BrailleDocument LoadFromYamlFile(string filename)
        {
            using (var reader = new StreamReader(filename, Encoding.UTF8))
            {
                return Deserialize(reader);
            }
        }
    }
}
