using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    internal class TableBottomLineContextTag : GenericContextTag
    {
        const string BottomLine = "▁";

        public TableBottomLineContextTag() : base(ContextTagNames.TableBottomLine2)
        {
            BrailleWord brWord = null;

            var cell0 = BrailleCell.GetInstanceFromPositionNumberString("1245");
            var cell1 = BrailleCell.GetInstanceFromPositionNumberString("2356");


            for (int i = 0; i < 40; i++)
            {
                brWord = new BrailleWord(BottomLine)
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
                brWord.ContextNames = XmlTagHelper.RemoveBracket(ContextTagNames.TableBottomLine2);

                PrefixBrailleWords.Add(brWord);
            }

            IsSingleLine = true;
        }
    }
}
