using System;
using System.Threading.Tasks;

namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 點字轉換介面，支援不同的轉換實作
    /// </summary>
    public interface IBrailleConverter : IDisposable
    {
        /// <summary>
        /// 執行點字轉換
        /// </summary>
        /// <param name="content">要轉換的文字內容</param>
        /// <param name="cellsPerLine">每列最大方數</param>
        /// <param name="phraseFiles">使用者自訂詞庫檔案</param>
        /// <param name="progress">進度回報</param>
        /// <returns>轉換結果</returns>
        Task<BrailleConversionResult> ConvertAsync(
            string content, 
            int cellsPerLine,
            string[] phraseFiles,
            IProgress<ConversionProgress>? progress = null);
    }
}
