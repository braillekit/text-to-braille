using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BrailleToolkit;
using BrailleToolkit.Data;
using EasyBrailleEdit.Common;
using NChinese.Phonetic;

namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 內建點字轉換服務（在主程式內執行）
    /// </summary>
    public class InProcessBrailleConverter : IBrailleConverter, IDisposable
    {
        private BrailleDocument? _doc;
        private BrailleProcessor? _processor;
        private ZhuyinReverseConverter? _zhuyinConverter;
        
        public async Task<BrailleConversionResult> ConvertAsync(
            string content, 
            int cellsPerLine,
            string[] phraseFiles,
            IProgress<ConversionProgress>? progress = null)
        {
            return await Task.Run(() => ConvertInternal(content, cellsPerLine, phraseFiles, progress));
        }
        
        private BrailleConversionResult ConvertInternal(
            string content, 
            int cellsPerLine,
            string[] phraseFiles,
            IProgress<ConversionProgress>? progress)
        {
            try
            {
                // 初始化轉換器
                var provider = new ZhuyinReverseConversionProvider();
                _zhuyinConverter = new ZhuyinReverseConverter(provider);
                _processor = BrailleProcessor.CreateInstance(_zhuyinConverter);
                _doc = new BrailleDocument(_processor);
                
                // 載入自訂詞庫
                LoadPhraseFiles(phraseFiles);
                
                // 設定進度回報
                if (progress != null)
                {
                    _processor.TextConverted += (s, e) => 
                    {
                        progress.Report(new ConversionProgress 
                        { 
                            CurrentLine = e.LineNumber,
                            CurrentText = e.Text 
                        });
                    };
                }
                
                // 執行轉換
                _doc.CellsPerLine = cellsPerLine;
                _doc.Convert(content);
                
                // 儲存到臨時檔案
                string outFileName = Path.Combine(
                    AppGlobals.TempPath, 
                    Constant.Files.CvtOutputTempFileName);
                    
                if (!_processor.HasError)
                {
                    _doc.SaveBrailleFile(outFileName);
                }
                
                // 建立結果
                var result = new BrailleConversionResult
                {
                    Success = !_processor.HasError,
                    OutputFilePath = outFileName,
                    ErrorMessage = _processor.ErrorMessage,
                    InvalidChars = new List<CharPosition>(_processor.InvalidChars)
                };
                
                return result;
            }
            catch (Exception ex)
            {
                return new BrailleConversionResult
                {
                    Success = false,
                    ErrorMessage = $"轉換過程發生未預期的錯誤: {ex.Message}",
                    OutputFilePath = null
                };
            }
            finally
            {
                // 確保資源釋放
                Cleanup();
            }
        }
        
        private void LoadPhraseFiles(string[] phraseFiles)
        {
            var phtbl = ZhuyinPhraseTable.GetInstance();
            foreach (string fname in phraseFiles)
            {
                if (!string.IsNullOrEmpty(fname) && File.Exists(fname))
                {
                    phtbl.Load(fname);
                }
            }
        }
        
        private void Cleanup()
        {
            _doc?.Clear();
            _doc = null;
            _processor = null;
            _zhuyinConverter = null;
        }
        
        public void Dispose()
        {
            Cleanup();
        }
    }
}
