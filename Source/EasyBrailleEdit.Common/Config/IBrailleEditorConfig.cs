using Config.Net;

namespace EasyBrailleEdit.Common.Config
{
    public interface IBrailleEditorConfig
    {
        [Option(DefaultValue = true)]
        bool ShowUndoWindow { get; set; }


        [Option(DefaultValue = Constant.DefaultMaxUndoLevel)]
        int MaxUndoLevel { get; set; }
    }
}
