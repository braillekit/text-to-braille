using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    internal class OrgPageNumberContextTag : GenericContextTag
    {
        const string LeadingUnderline = "_";
        const int NumberOfUnderline = 36;   // 用於表示原書頁次的底線字元數量

        public static string LeadingUnderlines = new StringBuilder().Insert(0, LeadingUnderline, NumberOfUnderline).ToString();

        public OrgPageNumberContextTag() : base(ContextTagNames.OrgPageNumber)
        {
            BrailleWord brWord = null;

            for (int i = 0; i < NumberOfUnderline; i++)
            {
                brWord = new BrailleWord(LeadingUnderline)
                {
                    IsConvertedFromTag = true
                };
                brWord.CellList.Add(BrailleCell.GetInstance(new int[] { 3, 6}));
                brWord.ContextNames = XmlTagHelper.RemoveBracket(ContextTagNames.OrgPageNumber);

                PrefixBrailleWords.Add(brWord);
            }
        }
    }
}
