using EasyBrailleEdit.Common;
using EasyBrailleEdit.DualEdit;
using Xunit;

namespace EasyBrailleEdit.Tests.DualEdit
{
    public class BrailleGridControllerFileTests
    {
        [Theory]
        [InlineData("cvt_out.tmp", true)]
        [InlineData("cvt_out.tmp_0123456789abcdef0123456789abcdef.brl", true)]
        [InlineData("CVT_OUT.TMP_0123456789ABCDEF0123456789ABCDEF.BRL", true)]
        [InlineData("cvt_out.tmp_zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz.brl", false)]
        [InlineData("cvt_out.tmp_notes.brl", false)]
        [InlineData("lesson.brl", false)]
        public void IsConversionOutputFile_RecognizesOnlyConversionNamesInTempDirectory(
            string name, bool expected)
        {
            string path = Path.Combine(AppGlobals.TempPath, name);

            Assert.Equal(expected, BrailleGridController.IsConversionOutputFile(path));
            Assert.False(BrailleGridController.IsConversionOutputFile(
                Path.Combine(AppGlobals.TempPath, "Documents", name)));
        }
    }
}
