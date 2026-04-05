using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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
                .IgnoreUnmatchedProperties()
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
            var yamlModel = BrailleDocumentYamlModel.FromBrailleDocument(doc);
            return serializer.Serialize(yamlModel);
        }

        /// <summary>
        /// 將 BrailleDocument 物件序列化至指定的 TextWriter。
        /// </summary>
        /// <param name="doc">要序列化的 BrailleDocument 物件。</param>
        /// <param name="writer">要寫入 YAML 的 TextWriter。</param>
        public static void Serialize(BrailleDocument doc, TextWriter writer)
        {
            var serializer = CreateSerializer();
            var yamlModel = BrailleDocumentYamlModel.FromBrailleDocument(doc);
            serializer.Serialize(writer, yamlModel);
        }

        /// <summary>
        /// 從 YAML 字串反序列化為 BrailleDocument 物件。
        /// </summary>
        /// <param name="yaml">包含 YAML 資料的字串。</param>
        /// <returns>反序列化後的 BrailleDocument 物件。</returns>
        public static BrailleDocument Deserialize(string yaml)
        {
            var deserializer = CreateDeserializer();
            var yamlModel = deserializer.Deserialize<BrailleDocumentYamlModel>(yaml);
            return yamlModel.ToBrailleDocument();
        }

        /// <summary>
        /// 從指定的 TextReader 反序列化為 BrailleDocument 物件。
        /// </summary>
        /// <param name="reader">要從中讀取 YAML 的 TextReader。</param>
        /// <returns>反序列化後的 BrailleDocument 物件。</returns>
        public static BrailleDocument Deserialize(TextReader reader)
        {
            var deserializer = CreateDeserializer();
            var yamlModel = deserializer.Deserialize<BrailleDocumentYamlModel>(reader);
            return yamlModel.ToBrailleDocument();
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

        private sealed class BrailleDocumentYamlModel
        {
            public int StartPageNumber { get; set; }

            public int CellsPerLine { get; set; }

            public List<BrailleLineYamlModel> Lines { get; set; } = new List<BrailleLineYamlModel>();

            public List<BraillePageTitleYamlModel> PageTitles { get; set; } = new List<BraillePageTitleYamlModel>();

            public static BrailleDocumentYamlModel FromBrailleDocument(BrailleDocument doc)
            {
                var yamlModel = new BrailleDocumentYamlModel
                {
                    StartPageNumber = doc.StartPageNumber,
                    CellsPerLine = doc.CellsPerLine
                };

                foreach (var line in doc.Lines)
                {
                    yamlModel.Lines.Add(BrailleLineYamlModel.FromBrailleLine(line));
                }

                foreach (var title in doc.PageTitles)
                {
                    yamlModel.PageTitles.Add(BraillePageTitleYamlModel.FromBraillePageTitle(title));
                }

                return yamlModel;
            }

            public BrailleDocument ToBrailleDocument()
            {
                var doc = new BrailleDocument
                {
                    StartPageNumber = StartPageNumber,
                    CellsPerLine = CellsPerLine
                };

                foreach (var line in Lines)
                {
                    doc.AddLine(line.ToBrailleLine());
                }

                foreach (var title in PageTitles)
                {
                    var pageTitle = title.ToBraillePageTitle(doc);
                    if (pageTitle != null)
                    {
                        doc.AddPageTitle(pageTitle);
                    }
                }

                return doc;
            }
        }

        private sealed class BrailleLineYamlModel
        {
            public List<BrailleWordYamlModel> Words { get; set; } = new List<BrailleWordYamlModel>();

            public static BrailleLineYamlModel FromBrailleLine(BrailleLine line)
            {
                var yamlModel = new BrailleLineYamlModel();
                foreach (var word in line.Words)
                {
                    yamlModel.Words.Add(BrailleWordYamlModel.FromBrailleWordView(word));
                }
                return yamlModel;
            }

            public BrailleLine ToBrailleLine()
            {
                var line = new BrailleLine();
                foreach (var word in Words)
                {
                    line.AddWord(word.ToBrailleWord());
                }
                return line;
            }
        }

        private sealed class BrailleWordYamlModel
        {
            public string Text { get; set; } = string.Empty;

            public string OriginalText { get; set; } = string.Empty;

            public BrailleCellList CellList { get; set; } = new BrailleCellList();

            public string PhoneticCode { get; set; } = string.Empty;

            public bool IsPolyphonic { get; set; }

            public bool DontBreakLineHere { get; set; }

            public string ContextNames { get; set; } = string.Empty;

            public bool IsContextTag { get; set; }

            public bool IsConvertedFromTag { get; set; }

            public static BrailleWordYamlModel FromBrailleWordView(IBrailleWordView word)
            {
                return new BrailleWordYamlModel
                {
                    Text = word.Text,
                    OriginalText = word.OriginalText,
                    CellList = CreateCellListFromWordView(word),
                    PhoneticCode = word.PhoneticCode ?? String.Empty,
                    IsPolyphonic = word.IsPolyphonic,
                    DontBreakLineHere = word.DontBreakLineHere,
                    ContextNames = word.ContextNames,
                    IsContextTag = word.IsContextTag,
                    IsConvertedFromTag = word.IsConvertedFromTag
                };
            }

            public BrailleWord ToBrailleWord()
            {
                var copiedCellList = CopyCellList(CellList);
                return BrailleWord.CreateFromConstruction(
                    Text,
                    String.IsNullOrEmpty(OriginalText) ? Text : OriginalText,
                    BrailleLanguage.Neutral,
                    CollectionsMarshal.AsSpan(copiedCellList.Items),
                    PhoneticCode ?? String.Empty,
                    IsPolyphonic,
                    DontBreakLineHere,
                    ContextNames ?? String.Empty,
                    contextTag: null,
                    isContextTag: IsContextTag,
                    isConvertedFromTag: IsConvertedFromTag,
                    noDigitCell: false,
                    noSpace: false,
                    noCapitalRule: false,
                    isEngPhonetic: false);
            }
        }

        private sealed class BraillePageTitleYamlModel
        {
            public BrailleLineYamlModel? TitleLine { get; set; }

            public int BeginLineIndex { get; set; } = -1;

            public static BraillePageTitleYamlModel FromBraillePageTitle(BraillePageTitle title)
            {
                return new BraillePageTitleYamlModel
                {
                    TitleLine = title.TitleLine == null ? null : BrailleLineYamlModel.FromBrailleLine(title.TitleLine),
                    BeginLineIndex = title.ContentStartLineIndex
                };
            }

            public BraillePageTitle? ToBraillePageTitle(BrailleDocument doc)
            {
                if (TitleLine == null)
                {
                    return null;
                }

                if (BeginLineIndex < 0 || BeginLineIndex >= doc.LineCount)
                {
                    return null;
                }

                return new BraillePageTitle(TitleLine.ToBrailleLine(), BeginLineIndex, doc.Lines[BeginLineIndex]);
            }
        }

        private static BrailleCellList CopyCellList(BrailleCellList? source)
        {
            var cellList = new BrailleCellList();
            if (source == null || source.Items == null)
            {
                return cellList;
            }

            var items = new List<BrailleCell>();
            foreach (var cell in source.Items)
            {
                items.Add(BrailleCell.GetInstance(cell.Value));
            }
            cellList.Items = items;
            return cellList;
        }

        private static BrailleCellList CreateCellListFromWordView(IBrailleWordView word)
        {
            var cellList = new BrailleCellList();
            for (int i = 0; i < word.CellCount; i++)
            {
                cellList.Add(BrailleCell.GetInstance(word.GetCell(i).Value));
            }
            return cellList;
        }
    }
}
