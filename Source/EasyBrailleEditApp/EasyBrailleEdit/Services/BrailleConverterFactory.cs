using EasyBrailleEdit.Common;

namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 點字轉換器工廠，根據組態建立適當的轉換器
    /// </summary>
    public static class BrailleConverterFactory
    {
        public static IBrailleConverter CreateConverter()
        {
            if (AppGlobals.Config.Braille.UseInProcessConversion)
            {
                return new InProcessBrailleConverter();
            }
            else
            {
                return new ExternalBrailleConverter();
            }
        }
    }
}
