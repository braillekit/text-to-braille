using System;
using System.IO;
using Xunit;
using BrailleToolkit;
using BrailleToolkit.Helpers;
using EasyBrailleEdit.Common.Utilities;
using System.Linq;
using Xunit.Abstractions;

namespace BrailleToolkit.Tests
{
    public class ComparisonTaskTest
    {
        private readonly ITestOutputHelper _output;

        public ComparisonTaskTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CompareBymlAndJson()
        {
            // Paths relative to the solution root (assuming test runs from bin/... or we can find the source root)
            // We will try to locate the files.
            string baseDir = FindProjectRoot();
            string bymlPath = Path.Combine(baseDir, @"Source\EasyBrailleEditApp\BrailleToolkit.Tests\TestData\poem.byml");
            string jsonPath = Path.Combine(baseDir, @"Source\EasyBrailleEditApp\BrailleToolkit.Tests\TestData\poem.brx");

            Assert.True(File.Exists(bymlPath), $"File not found: {bymlPath}");
            Assert.True(File.Exists(jsonPath), $"File not found: {jsonPath}");

            string bymlContent = File.ReadAllText(bymlPath);
            string jsonContent = File.ReadAllText(jsonPath);

            // Deserialize YAML
            BrailleDocument docFromYaml = BrailleDocumentYamlSerializer.Deserialize(bymlContent);
            Assert.NotNull(docFromYaml);

            // Deserialize JSON
            BrailleDocument docFromJson = JsonHelper.Deserialize<BrailleDocument>(jsonContent);
            Assert.NotNull(docFromJson);

            // Compare serialized versions to ensure deep equality
            // We use JsonHelper to serialize both back to JSON strings.
            // This standardizes the property order and formatting.
            string jsonFromYamlDoc = JsonHelper.Serialize(docFromYaml);
            string jsonFromJsonDoc = JsonHelper.Serialize(docFromJson);

            // Compare length first for quick check
            if (jsonFromYamlDoc.Length != jsonFromJsonDoc.Length)
            {
                 _output.WriteLine($"Lengths differ: YAML->JSON={jsonFromYamlDoc.Length}, JSON->JSON={jsonFromJsonDoc.Length}");
            }

            // Compare strings
            // We rely on deterministic serialization order of DataContractJsonSerializer.
            Assert.Equal(jsonFromJsonDoc, jsonFromYamlDoc);
        }

        private string FindProjectRoot()
        {
            string current = Directory.GetCurrentDirectory();
            // Walk up until we find .git or Source folder
            while (!Directory.Exists(Path.Combine(current, "Source")) && Directory.GetParent(current) != null)
            {
                current = Directory.GetParent(current)?.FullName ?? throw new Exception("Project root not found");
            }
            return current;
        }
    }
}
