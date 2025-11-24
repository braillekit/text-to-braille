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
            // 準備輸入檔案
            string inFileName = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtInputTempFileName);
            string outFileName = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtOutputTempFileName);
            string phraseListFile = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtInputPhraseListFileName);
            
            await File.WriteAllTextAsync(inFileName, content, Encoding.UTF8);
            await File.WriteAllLinesAsync(phraseListFile, phraseFiles, Encoding.UTF8);
            
            // 呼叫 Txt2Brl.exe
            await InvokeTxt2BrlAsync(inFileName, outFileName, cellsPerLine);
            
            // 讀取結果
            return ReadConversionResult(outFileName);
        }
        
        private async Task InvokeTxt2BrlAsync(string inFileName, string outFileName, int cellsPerLine)
        {
            StringBuilder arg = new StringBuilder();
            arg.Append($" -i \"{inFileName}\" -o \"{outFileName}\" ");
            arg.Append($"-c{cellsPerLine} ");
            
            _fileRunner.NeedWait = true;
            _fileRunner.ShowWindow = false;
            _fileRunner.UseShellExecute = false;
            _fileRunner.RedirectStandardOutput = true;
            
            string cmd = Path.Combine(Application.StartupPath, "txt2brl.exe");
            int exitCode = await _fileRunner.RunAsync(cmd, arg.ToString());
            
            if (exitCode != 0)
            {
                throw new Exception($"轉點字過程發生錯誤 (Exit Code: {exitCode})!");
            }
        }
        
        private BrailleConversionResult ReadConversionResult(string outFileName)
        {
            var result = new BrailleConversionResult
            {
                OutputFilePath = outFileName
            };
            
            // 讀取錯誤資訊（從臨時檔案）
            string resultFile = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtResultFileName);
            string errorCharFile = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtErrorCharFileName);
            
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
                    invalidChars.Add(new CharPosition
                    {
                        LineNumber = int.Parse(parts[0]),
                        CharIndex = int.Parse(parts[1]),
                        CharValue = parts[2][0]
                    });
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
