using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BrailleToolkit.Data;


namespace BrailleToolkit.Tests
{
    [Collection("Singleton-Sensitive Tests")]
    public class XmlBrailleTableTest
    {

        [Fact]
        public void Should_AutoGenerateCodeFromDots()
        {
            var text = "↔";
            var dots = "246 25 25 135";
            var expectedCode = "2A121215";

            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"utf - 8\"?>");
            xml.AppendLine("<Symbols>");
            xml.AppendLine($"<symbol text=\"{text}\" dots=\"{dots}\"  type=\"Misc\" rule=\"SpaceAtBothEnds\" description=\"單槓雙向箭頭\" />");
            xml.AppendLine("</Symbols>");

            var xmlTbl = new XmlBrailleTable();
            xmlTbl.LoadFromXmlString(xml.ToString());

            var actualCode = xmlTbl.Find(text);
            Assert.Equal(expectedCode, actualCode);
        }

        [Fact]
        public void Should_ExposeImmutableEntriesAndTypedLookups()
        {
            var xml = new StringBuilder();
            xml.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            xml.AppendLine("<Symbols>");
            xml.AppendLine("<symbol text=\"A\" dots=\"1\" type=\"Letter\" description=\"letter a\" />");
            xml.AppendLine("<symbol text=\"1\" dots=\"1\" dots2=\"2\" type=\"Digit\" />");
            xml.AppendLine("<symbol text=\"ㄨㄛ\" dots=\"25\" type=\"Phonetic\" joined=\"true\" />");
            xml.AppendLine("</Symbols>");

            var xmlTbl = new XmlBrailleTable();
            xmlTbl.LoadFromXmlString(xml.ToString());

            Assert.Equal(3, xmlTbl.Entries.Count);
            Assert.Equal("01", xmlTbl.Find("A", "Letter"));
            Assert.Equal(new[] { "1" }, xmlTbl.GetDots("A", "Letter"));

            BrailleTableEntry digitEntry = xmlTbl.Entries[1];
            Assert.Equal("02", digitEntry.Code2);
            Assert.Equal("Digit", digitEntry.Type);

            BrailleTableEntry phoneticEntry = xmlTbl.Entries[2];
            Assert.True(phoneticEntry.Joined);
        }

        [Fact]
        public void Should_SupportConcreteTableLookups_WithoutDataTable()
        {
            var englishTable = EnglishBrailleTable.GetInstance();
            Assert.Equal("01", englishTable.FindLetter("a"));
            Assert.Equal("16", englishTable.FindDigit("6", false));

            var uebTable = EnglishUebBrailleTable.GetInstance();
            Assert.Equal("01", uebTable.FindLetter("A"));

            var chineseTable = TwChineseBrailleTable.GetInstance();
            Assert.Equal("12", chineseTable.GetPhoneticJoinedCode("ㄨㄛ"));
            Assert.Equal("01", chineseTable.GetPhoneticMonoCode("ㄓ"));
            Assert.Equal("08", chineseTable.GetPhoneticToneCode("ˇ"));
            Assert.Equal("1212", chineseTable.GetPunctuationCode("："));
            Assert.Contains("。", chineseTable.GetAllPunctuations());

            var urlTable = UrlBrailleTable.GetInstance();
            Assert.Equal("01", urlTable.FindLetter("a"));
            Assert.Equal("16", urlTable.FindDigit("6", false));
        }
    }
}
