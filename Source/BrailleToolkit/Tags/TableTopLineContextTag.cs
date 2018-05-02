using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    internal class TableTopLineContextTag : GenericContextTag
    {
        const string TopLine = "▔";

        public TableTopLineContextTag() : base(ContextTagNames.TableTopLine)
        {
            BrailleWord brWord = null;

            var cell0 = BrailleCell.GetInstanceFromPositionNumberString("2356");
            var cell1 = BrailleCell.GetInstanceFromPositionNumberString("1245");

            for (int i = 0; i < 40; i++)
            {
                brWord = new BrailleWord(TopLine)
                {
                    IsConvertedFromTag = true
                };
                if (i % 2 == 0)
                {
                    brWord.CellList.Add(cell0);
                }
                else
                {
                    brWord.CellList.Add(cell1);
                }
                brWord.ContextNames = XmlTagHelper.RemoveBracket(ContextTagNames.TableTopLine);

                PrefixBrailleWords.Add(brWord);
            }

            IsSingleLine = true;
        }
    }
}
