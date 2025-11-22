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
        /// <summary>
        /// 區段名稱。
        /// </summary>
        public const string Name = "BrailleEditor";

        /// <summary>
        /// 取得或設定是否顯示復原視窗。
        /// </summary>
        public bool ShowUndoWindow { get; set; } = true;
        /// <summary>
        /// 取得或設定最大復原層數。
        /// </summary>
        public int MaxUndoLevel { get; set; } = Constant.DefaultMaxUndoLevel;
    }
}
