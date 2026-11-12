using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrailleToolkit;
using EasyBrailleEdit.Common;
using EasyBrailleEdit.Common.Utilities.Windows;

namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 外部工具點字轉換服務（呼叫 Txt2Brl.exe）
    /// </summary>
    public class ExternalBrailleConverter : IBrailleConverter
    {
        private readonly FileRunner _fileRunner = new();
        
        public async Task<BrailleConversionResult> ConvertAsync(
            string content, 
            int cellsPerLine,
            string[] phraseFiles,
            IProgress<ConversionProgress>? progress = null)
        {
            // 產生唯一識別碼以避免並行執行時的檔案衝突
            string uniqueId = Guid.NewGuid().ToString("N");
            
            // 準備輸入檔案
            string inFileName = Path.Combine(AppGlobals.TempPath, $"{Constant.Files.CvtInputTempFileName}_{uniqueId}.txt");
            string outFileName = Path.Combine(AppGlobals.TempPath, $"{Constant.Files.CvtOutputTempFileName}_{uniqueId}.brl");
            string phraseListFile = Path.Combine(AppGlobals.TempPath, $"{Constant.Files.CvtInputPhraseListFileName}_{uniqueId}.txt");
            string resultFile = Path.Combine(AppGlobals.TempPath, $"{Constant.Files.CvtResultFileName}_{uniqueId}.txt");
            string errorCharFile = Path.Combine(AppGlobals.TempPath, $"{Constant.Files.CvtErrorCharFileName}_{uniqueId}.txt");
            
            try 
            {
                await File.WriteAllTextAsync(inFileName, content, Encoding.UTF8);
                await File.WriteAllLinesAsync(phraseListFile, phraseFiles, Encoding.UTF8);
                
                // 呼叫 Txt2Brl.exe
                await InvokeTxt2BrlAsync(inFileName, outFileName, cellsPerLine, resultFile, errorCharFile);
                
                // 讀取結果
                return ReadConversionResult(outFileName, resultFile, errorCharFile, _fileRunner.StdOutputMsg);
            }
            finally
            {
                // 清理暫存檔案
                DeleteFileIfExists(inFileName);
                // DeleteFileIfExists(outFileName); // Output file should be kept for caller
                DeleteFileIfExists(phraseListFile);
                DeleteFileIfExists(resultFile);
                DeleteFileIfExists(errorCharFile);
            }
        }
        
        private void DeleteFileIfExists(string fileName)
        {
            try
            {
                if (File.Exists(fileName)) File.Delete(fileName);
            }
            catch
            {
                // 忽略刪除失敗
            }
        }
        
        private async Task InvokeTxt2BrlAsync(string inFileName, string outFileName, int cellsPerLine, string resultFile, string errorCharFile)
        {
            StringBuilder arg = new StringBuilder();
            arg.Append($" -i \"{inFileName}\" -o \"{outFileName}\" ");
            arg.Append($"-c{cellsPerLine} ");
            arg.Append($"--result \"{resultFile}\" ");
            arg.Append($"--error \"{errorCharFile}\" ");
            
            _fileRunner.NeedWait = true;
            _fileRunner.ShowWindow = false;
            _fileRunner.UseShellExecute = false;
            _fileRunner.RedirectStandardOutput = true;
            
            string cmd = Path.Combine(Application.StartupPath, "txt2brl.exe");
            int exitCode = await _fileRunner.RunAsync(cmd, arg.ToString());
            
            if (exitCode != 0)
            {
                throw new Exception($"轉點字過程發生錯誤 (Exit Code: {exitCode})! Output: {_fileRunner.StdOutputMsg}");
            }
        }
        
        private BrailleConversionResult ReadConversionResult(string outFileName, string resultFile, string errorCharFile, string stdOutput)
        {
            var result = new BrailleConversionResult
            {
                OutputFilePath = outFileName
            };
            
            // 讀取錯誤資訊（從臨時檔案）
            if (File.Exists(resultFile))
            {
                var lines = File.ReadAllLines(resultFile);
                if (lines.Length > 0 && lines[0] == "1")
                {
                    result.Success = false;
                    result.ErrorMessage = lines.Length > 1 ? lines[1] : "";
                }
                else
                {
                    result.Success = true;
                }
            }
            else
            {
                // Result file missing, likely Txt2Brl failed to run properly
                result.Success = false;
                result.ErrorMessage = $"Result file not found. Txt2Brl output: {stdOutput}";
            }
            
            if (File.Exists(errorCharFile))
            {
                result.InvalidChars = ReadInvalidChars(errorCharFile);
            }
            
            return result;
        }
        
        private List<CharPosition> ReadInvalidChars(string fileName)
        {
            var invalidChars = new List<CharPosition>();
            var lines = File.ReadAllLines(fileName);
            
            foreach (var line in lines)
            {
                var parts = line.Split(' ');
                if (parts.Length == 3)
                {
                    invalidChars.Add(new CharPosition(parts[2][0], int.Parse(parts[0]), int.Parse(parts[1])));
                }
            }
            
            return invalidChars;
        }
        public void Dispose()
        {
            // No resources to dispose
        }
    }
}
