using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    internal sealed class TableBottomLineContextTag : GenericContextTag
    {
        const string BottomLine = "▁";

        public TableBottomLineContextTag() : base(ContextTagNames.TableBottomLine2)
        {
            RemoveTagOnConversion = true;
            Lifetime = ContextLifetime.Transient;
            IsSingleLine = true;

            var cell0 = BrailleCell.GetInstanceFromPositionNumberString("1245");
            var cell1 = BrailleCell.GetInstanceFromPositionNumberString("2356");


            for (int i = 0; i < 40; i++)
            {
                var brWord = new BrailleWord(BottomLine)
                {
                    // 當標籤的 RemoveTagOnConvertion 為 true 時，
                    // 不要設定 IsConvertedFromTag 為 true，否則在匯出文字檔時，會變成空字串！
                    IsConvertedFromTag = false
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
        }
    }
}
