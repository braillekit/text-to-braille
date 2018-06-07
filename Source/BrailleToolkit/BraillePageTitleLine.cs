using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit
{
    public class BraillePageTitleLine : BrailleLine
    {
        public int BeginLineIndex { get; set; }
        public BrailleLine BeginLineRef { get; set; }

        public BraillePageTitleLine()
        {
            BeginLineIndex = -1;
            BeginLineRef = null;
        }

        public BraillePageTitleLine(BrailleLine brLineForTitle, int beginLineIndex)
        {
            Words = brLineForTitle.Words;
            BeginLineIndex = beginLineIndex;
        }

        public BraillePageTitleLine(BrailleDocument brDoc, int beginLineIndex)
            : this()
        {
            if (brDoc == null)
            {
                throw new ArgumentNullException(nameof(brDoc));
            }

            if (beginLineIndex >= brDoc.LineCount)
            {
                return;
            }

            BeginLineIndex = beginLineIndex; // TODO: 要不要加 1？（下一列開始就是使用此標題。）
            BeginLineRef = brDoc.Lines[beginLineIndex];
        }
    }
}
