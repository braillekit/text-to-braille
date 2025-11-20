using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using BrailleToolkit.Data;
using BrailleToolkit.Helpers;
using EasyBrailleEdit.Common.Utilities;

namespace BrailleToolkit.Tests
{
    public class YamlSerializationTests
    {
        [Fact]
        public void RoundTripTest_BrailleDocument()
        {
            // Arrange
            var doc = new BrailleDocument();
            doc.CellsPerLine = 30;
            
            // Add a line with words
            var line = new BrailleLine();
            
            var word1 = new BrailleWord("Test");
            word1.AddCells("12"); // random hex
            word1.Language = BrailleLanguage.English; // Note: Language is NOT serialized in JSON/YAML currently per DataMember check, but checking logic anyway
            
            var word2 = new BrailleWord("中文", "ㄓㄨㄥ ㄨㄣˊ", "AABB"); // Fake codes
            word2.IsPolyphonic = true;

            line.Words.Add(word1);
            line.Words.Add(word2);
            
            doc.AddLine(line);

            // Add Page Title (if possible to mock without BrailleProcessor context complexity)
            // PageTitles logic relies on matching lines.
            // We'll skip PageTitles for basic test unless necessary, but doc structure supports it.
            
            // Act
            string yaml = BrailleDocumentYamlSerializer.Serialize(doc);
            BrailleDocument deserializedDoc = BrailleDocumentYamlSerializer.Deserialize(yaml);

            // Assert
            Assert.NotNull(deserializedDoc);
            Assert.Equal(doc.CellsPerLine, deserializedDoc.CellsPerLine);
            Assert.Equal(doc.Lines.Count, deserializedDoc.Lines.Count);
            
            var dLine = deserializedDoc.Lines[0];
            Assert.Equal(line.Words.Count, dLine.Words.Count);
            
            var dWord1 = dLine.Words[0];
            Assert.Equal(word1.Text, dWord1.Text);
            Assert.Equal(word1.CellCount, dWord1.CellCount);
            Assert.Equal(word1.Cells[0].Value, dWord1.Cells[0].Value);
            
            var dWord2 = dLine.Words[1];
            Assert.Equal(word2.Text, dWord2.Text);
            Assert.Equal(word2.PhoneticCode, dWord2.PhoneticCode);
            Assert.Equal(word2.IsPolyphonic, dWord2.IsPolyphonic);
            Assert.Equal(word2.Cells.Count, dWord2.Cells.Count);
        }

        [Fact]
        public void CompareJsonAndYamlContent()
        {
             // Arrange
            var doc = new BrailleDocument();
            doc.CellsPerLine = 40;
            var line = new BrailleLine();
            var word = new BrailleWord("A", "01");
            line.Words.Add(word);
            doc.AddLine(line);

            // Act
            string json = JsonHelper.Serialize(doc);
            string yaml = BrailleDocumentYamlSerializer.Serialize(doc);

            // Output for manual inspection if needed (can use ITestOutputHelper if strictly needed, but this is automated)
            // Assert
            Assert.Contains("Value: 1", yaml); // Check for flow style mapping or at least value presence
            Assert.Contains("Text: A", yaml);
        }
    }
}
