using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    internal class TableSingleLineContextTag : GenericContextTag
    {
        const string TopLine = "▔";
        const string BottomLine = "▁";

        public TableSingleLineContextTag(string tagName) : base(tagName)
        {


            BrailleWord brWord = null;

            string text = TopLine;
            var cell = BrailleCell.GetInstanceFromPositionNumberString("1245");
            if (tagName.IndexOf("下") >= 0)
            {
                cell = BrailleCell.GetInstanceFromPositionNumberString("2356");
                text = BottomLine;
            }


            for (int i = 0; i < 40; i++)
            {
                brWord = new BrailleWord(text)
                {
                    IsConvertedFromTag = true
                };
                brWord.CellList.Add(cell);
                brWord.ContextNames = XmlTagHelper.RemoveBracket(tagName);

                PrefixBrailleWords.Add(brWord);
            }

            IsSingleLine = true;
        }
    }
}
