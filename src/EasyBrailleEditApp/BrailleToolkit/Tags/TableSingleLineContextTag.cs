using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    internal sealed class TableSingleLineContextTag : GenericContextTag
    {
        const string TopLine = "▔";
        const string BottomLine = "▁";

        public TableSingleLineContextTag(string tagName) : base(tagName)
        {
            RemoveTagOnConversion = true;
            Lifetime = ContextLifetime.Transient;
            IsSingleLine = true;

            string text = TopLine;
            var cell = BrailleCell.GetInstanceFromPositionNumberString("2356");
            if (tagName.IndexOf("下") >= 0)
            {
                cell = BrailleCell.GetInstanceFromPositionNumberString("1245");
                text = BottomLine;
            }

            for (int i = 0; i < 40; i++)
            {
                var brWord = new BrailleWord(text)
                {
                    // 當標籤的 RemoveTagOnConvertion 為 true 時，
                    // 不要設定 IsConvertedFromTag 為 true，否則在匯出文字檔時，會變成空字串！
                    IsConvertedFromTag = false
                };
                brWord.CellList.Add(cell);
                brWord.ContextNames = XmlTagHelper.RemoveBracket(tagName);

                PrefixBrailleWords.Add(brWord);
            }            
        }
    }
}
