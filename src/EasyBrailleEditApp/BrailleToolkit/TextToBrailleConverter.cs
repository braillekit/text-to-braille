using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyBrailleEdit.Common;
using NChinese;
using NChinese.Phonetic;
using Serilog;

namespace BrailleToolkit
{
    /// <summary>
    /// 此類別可用來將一串明眼字轉換成點字，並保存於 BrailleLine 物件中。
    /// 此類別不做格式化（斷行、段落編排），僅適用於單行字串的轉換，而不適合用來轉換包含多行文字的文件。
    /// </summary>
    public sealed class TextToBrailleConverter
    {
        /// <summary>
        /// 點字處理器。
        /// </summary>
        public BrailleProcessor Processor { get; }
        
        /// <summary>
        /// 點字文件物件。
        /// </summary>
        public BrailleDocument Doc { get; }

        /// <summary>
        /// 建構函式。
        /// </summary>
        public TextToBrailleConverter()
        {
            Processor = BrailleProcessor.CreateInstance();
            Doc = new BrailleDocument(Processor);

            Processor.ConvertionFailed += BrailleProcessor_ConvertionFailed;
            Processor.TextConverted += BrailleProcessor_TextConverted;

            LoadPhraseFiles();

            // TODO: 還需要這個嗎？
            ZhuyinQueryHelper.Initialize(); // 初始化注音字根查詢器（載入注音字根表）。
        }

        /// <summary>
        /// 載入使用者自訂詞庫。
        /// </summary>
        private void LoadPhraseFiles()
        {
            string phraseListFileName = Path.Combine(AppGlobals.TempPath, Constant.Files.CvtInputPhraseListFileName);
            if (!File.Exists(phraseListFileName))
            {
                return;
            }

            string[] phraseFiles = File.ReadAllLines(phraseListFileName, Encoding.UTF8);

            string fname;
            ZhuyinPhraseTable phtbl = ZhuyinPhraseTable.GetInstance();

            foreach (string s in phraseFiles)
            {
                fname = s.Trim().ToLower();
                if (String.IsNullOrEmpty(fname))
                    continue;
                if (!File.Exists(fname))    // 檔案如果不存在，就不處理
                    continue;
                phtbl.Load(fname);
            }
        }

        /// <summary>
        /// 轉換指定的明眼字串。
        /// </summary>
        /// <param name="inputText">明眼字串。</param>
        /// <returns>轉換後的點字列。</returns>
        public BrailleLine? Convert(string inputText)
        {
            Processor.InitializeForConversion();
            return Processor.ConvertLine(inputText);
        }


        /// <summary>
        /// 是否發生錯誤。
        /// </summary>
        public bool HasError
        {
            get
            {
                return Processor.HasError;
            }
        }

        /// <summary>
        /// 取得所有轉換失敗的字元。
        /// </summary>
        public string GetInvalidChars()
        {
            var sb = new StringBuilder();
            foreach (CharPosition ch in Processor.InvalidChars)
            {
                sb.Append(ch.CharValue);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 碰到無法轉換的字元時觸發此事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BrailleProcessor_ConvertionFailed(object? sender, ConversionFailedEventArgs args)
        {
            Log.Information("無法轉換: " + args.InvalidChar.CharValue);
        }

        private void BrailleProcessor_TextConverted(object? sender, TextConvertedEventArgs e)
        {
        }
    }

}
