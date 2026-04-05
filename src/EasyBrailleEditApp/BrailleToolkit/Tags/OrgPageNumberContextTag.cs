using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    /// <summary>
    /// Context tag for original page number.
    /// </summary>
    public class OrgPageNumberContextTag : GenericContextTag
    {
        /// <summary>
        /// The leading underline character.
        /// </summary>
        public const string LeadingUnderline = "_";

        /// <summary>
        /// The number of underline characters.
        /// </summary>
        public const int NumberOfUnderline = 36;   // 用於表示原書頁次的底線字元數量

        /// <summary>
        /// The string containing leading underlines.
        /// </summary>
        public static string LeadingUnderlines = new StringBuilder().Insert(0, LeadingUnderline, NumberOfUnderline).ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="OrgPageNumberContextTag"/> class.
        /// </summary>
        public OrgPageNumberContextTag() : base(ContextTagNames.OrgPageNumber, singleLine: true)
        {
            BrailleWord? brWord = null; // Fixed CS8600

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
