using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Txt2Brl
{
    public class Options
    {
        public const int DefaultCellsPerLine = 40;

        [Option('h', "help", Required = false, HelpText = "顯示用法說明。")]
        public bool ShowHelp { get; set; }

        [Option('i', "input", Required = false, HelpText = "輸入檔案名稱，至少一個。")]
        public string InputFileName { get; set; }

        [Option('o', "output", Required = false, HelpText = "輸出檔案名稱。")]
        public string OutputFileName { get; set; }

        [Option("stdin", Required = false, Default = false, HelpText = "從 STDIN 輸入欲轉換的文字。")]
        public bool Stdin { get; set; }

        [Option('c', "cellsperline", Required = false, Default = DefaultCellsPerLine, HelpText = "每列最多幾方。")]
        public int CellsPerLine { get; set; }

        // Omitting long name, defaults to name of property, ie "--verbose"
        [Option('v', "verbose", Default = false, HelpText = "顯示詳細的處理過程。")]
        public bool Verbose { get; set; }
    }
}
