using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using BrailleToolkit.Data;


namespace Test.BrailleToolkit
{
    [TestFixture()]
    public class XmlBrailleTableTest
    {

        [Test]
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
            Assert.AreEqual(actualCode, expectedCode);
        }
    }
}
