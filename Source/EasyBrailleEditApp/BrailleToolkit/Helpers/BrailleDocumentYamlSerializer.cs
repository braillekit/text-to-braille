using System;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BrailleToolkit.Helpers
{
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

        public static string Serialize(BrailleDocument doc)
        {
            var serializer = CreateSerializer();
            return serializer.Serialize(doc);
        }

        public static void Serialize(BrailleDocument doc, TextWriter writer)
        {
            var serializer = CreateSerializer();
            serializer.Serialize(writer, doc);
        }

        public static BrailleDocument Deserialize(string yaml)
        {
            var deserializer = CreateDeserializer();
            return deserializer.Deserialize<BrailleDocument>(yaml);
        }

        public static BrailleDocument Deserialize(TextReader reader)
        {
            var deserializer = CreateDeserializer();
            return deserializer.Deserialize<BrailleDocument>(reader);
        }
    }
}
