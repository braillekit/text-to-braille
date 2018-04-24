﻿using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using EasyBrailleEdit.Common;
using Serilog;

namespace Txt2Brl
{
    class Program
	{
		const int DefaultCellsPerLine = 40;

		[STAThread]
		static int Main(string[] args)
		{
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .WriteTo.RollingFile(@"Logs\log-txt2brl-{Date}.txt")
                .CreateLogger();

            Console.WriteLine("Txt2Brl version 3.2 Copyright(c) 2007-2018 Michael Tsai.\n");

            CommandLine.Parser.Default.ParseArguments<Options>(args)
               .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts));
           
			return 0;
		}

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            if (opts.ShowHelp)
            {
                ShowUsage();
                return;
            }

            // 若有指定 stdin，則優先以 stdin 為輸入。若沒有輸入，才看有沒有指定輸入檔案。
            string inputText = string.Empty;
            if (opts.Stdin)
            {
                if (String.IsNullOrEmpty(opts.OutputFileName))
                {
                    Console.WriteLine("使用 --stdin 選項時必須指定輸出檔案名稱!");
                    return;
                }

                Console.Write("輸入欲轉換的文字: ");
                inputText = Console.ReadLine();

                if (String.IsNullOrEmpty(inputText))
                {
                    Console.WriteLine("沒有從 console 輸入文字。程式結束。");
                    return;
                }
                Log.Debug("輸入文字: " + inputText);
            }
            else
            {
                if (String.IsNullOrEmpty(opts.InputFileName))
                {
                    Console.WriteLine("必須指定輸入檔案名稱，或使用 -stdin 參數來輸入欲轉換的文字。");
                    return;
                }
                if (String.IsNullOrEmpty(opts.OutputFileName))
                {
                    opts.OutputFileName = Path.ChangeExtension(opts.InputFileName, Constant.Files.DefaultBrailleFileExt);
                }
                Console.WriteLine("輸入檔案: " + opts.InputFileName);
            }
            Log.Debug("輸出檔案: " + opts.OutputFileName);
            Log.Debug("每列方數: " + opts.CellsPerLine);
            Log.Debug("冗長訊息: " + opts.Verbose);

            Console.WriteLine();
            Console.WriteLine("★注意：執行點字轉換的過程中，請勿關閉此視窗!!!");
            Console.WriteLine(" ");
            Console.WriteLine("轉換工作開始於 " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));

            if (opts.Stdin)
            {
                DoConvertString(inputText, opts.OutputFileName, opts.CellsPerLine, opts.Verbose);
            }
            else
            {                
                DoConvertFile(opts.InputFileName, opts.OutputFileName, opts.CellsPerLine, opts.Verbose);
            }

            Log.Debug("轉換完畢。");

            Console.WriteLine();
            Console.WriteLine("轉換工作結束於 " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        }

        private static void ShowUsage()
		{
			Console.WriteLine("使用方法: Txt2Brl -i <輸入檔名> -o [輸出檔名] [選項]\n");
			Console.WriteLine("選項:\n");
            Console.WriteLine("    -i, --input         : 輸入檔案名稱。\n");
            Console.WriteLine("    -o, --output        : 輸出檔案名稱。若未指定，則會以輸入檔名作為輸出檔案的主檔名。\n");
            Console.WriteLine("    --stdin             : 讓你輸入欲轉換的文字。若同時使用了 -i 來指定輸入檔名，會優先採用此選項。\n");
            Console.WriteLine("    -cn, --cellsperline : 每列最大方數。\n");
			Console.WriteLine("    -v, --verbose       : 冗長訊息模式。\n");
		}


		private static void DoConvertFile(string inputFile, string outputFile, 
			int cellsPerLine, bool verboseMode)
		{
			BrailleConverter cvt = new BrailleConverter();

			cvt.ConvertFile(inputFile, outputFile, cellsPerLine, verboseMode);
		}

        private static void DoConvertString(string inputText, string outputFile,
            int cellsPerLine, bool verboseMode)
        {
            BrailleConverter cvt = new BrailleConverter();

            cvt.Convert(inputText, outputFile, cellsPerLine, verboseMode);
        }

    }
}
