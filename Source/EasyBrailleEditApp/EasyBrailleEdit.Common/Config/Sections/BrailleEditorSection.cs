using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Common.Config.Sections
{
    /// <summary>
    /// 區段 [BrailleEditor]
    /// </summary>
    public sealed record BrailleEditorSection
    {
        public const string Name = "BrailleEditor";

        public bool ShowUndoWindow { get; set; } = true;
        public int MaxUndoLevel { get; set; } = Constant.DefaultMaxUndoLevel;
    }
}
