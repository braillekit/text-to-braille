using Config.Net;

namespace EasyBrailleEdit.Common.Config
{
    public interface IBrailleEditorConfig
    {
        [Option(DefaultValue = Constant.DefaultMaxUndoLevel)]
        int MaxUndoLevel { get; set; }
    }
}
