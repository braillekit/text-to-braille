using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BrailleToolkit.Tags;
using BrailleToolkit.Converters;
using BrailleToolkit.Data;
using BrailleToolkit.Helpers;
using EasyBrailleEdit.Common;
using Huanlin.Common.Helpers;
using NChinese.Phonetic;
using Huanlin.Common.Extensions;

namespace BrailleToolkit
{
    public struct CharPosition
    {
        public char CharValue { get; set; }     // 字元
        public int LineNumber { get; set; }     // 第幾列
        public int CharIndex { get; set; }      // 第幾個字元
    }

    public class ConversionFailedEventArgs : EventArgs
    {
        public string OriginalText { get; private set; }

        public CharPosition InvalidChar { get; private set; }

        public bool Stop { get; set; }

        internal void SetArgs(int lineNumber, int charIndex, string line, char ch)
        {
            InvalidChar = new CharPosition
            {
                LineNumber = lineNumber,
                CharIndex = charIndex,
                CharValue = ch
            };
            OriginalText = line;
        }
    }

    public class TextConvertedEventArgs : EventArgs
    {
        internal void SetArgValues(int lineNum, string text)
        {
            Text = text;
            LineNumber = lineNum;
        }

        public string Text { get; private set; }
        public int LineNumber { get; private set; }
    }

    /// <summary>
    /// 此類別可用來將明眼字轉換成點字。可處理一個字、一行、或者多行。
    /// 錯誤處理機制：
    /// 1. 所有 exception 訊息會存入 ErrorMessage 屬性。
    /// 2. 所有無法轉換的字元會丟到 InvalidChars 屬性。
    /// </summary>
    public class BrailleProcessor
    {
        private static BrailleProcessor s_Processor;

        private CoordinateConverter _coordConverter;
        private TableConverter _tableConverter;
        private PhoneticConverter _phoneticConverter;

        // Extended converters
        private List<WordConverter> _converters;

        private StringBuilder _errorMsg;           // 轉換過程中發生的錯誤訊息。

        private event EventHandler<ConversionFailedEventArgs> _conversionFailedEvent;
        private event EventHandler<TextConvertedEventArgs> _textConvertedEvent;

        private Dictionary<string, string> _autoReplacedText;

        #region 建構函式

        private BrailleProcessor(ZhuyinReverseConverter zhuyinConverter)
        {
            _converters = new List<WordConverter>();

            ControlTagConverter = new ContextTagConverter();
            ZhuyinConverter = zhuyinConverter;
            ChineseConverter = new ChineseWordConverter(this);
            EnglishConverter = new EnglishWordConverter();
            MathConverter = new MathConverter();
            _coordConverter = new CoordinateConverter();
            _tableConverter = new TableConverter();
            _phoneticConverter = new PhoneticConverter();

            ContextManager = new ContextTagManager();

            InvalidChars = new List<CharPosition>();
            _errorMsg = new StringBuilder();
            SuppressEvents = false;

            // 轉點字之前，預先替換的文字
            var replacedText = AppGlobals.Config.AutoReplacedText.EnsureNotEncloseWith("{", "}");
            _autoReplacedText = StrHelper.SplitToDictionary(replacedText, ' ', '=');
        }

        /// <summary>
        /// Get singleton instance.
        /// </summary>
        /// <returns></returns>
        public static BrailleProcessor GetInstance(ZhuyinReverseConverter zhuyinConverter = null)
        {
            if (s_Processor != null)
            {
                return s_Processor;
            }

            if (zhuyinConverter == null)
            {
                // create default zhuyin reverse converter if not specified.
                zhuyinConverter = new ZhuyinReverseConverter(new ZhuyinReverseConversionProvider());
            }

            s_Processor = new BrailleProcessor(zhuyinConverter);
            return s_Processor;
        }

        #endregion

        #region 屬性

        /// <summary>
        /// 取得或設定中文點字轉換器。
        /// </summary>
        public ChineseWordConverter ChineseConverter { get; set; }

        /// <summary>
        /// 取得或設定英文點字轉換器。
        /// </summary>
        public EnglishWordConverter EnglishConverter { get; set; }

        public ContextTagConverter ControlTagConverter { get; set; }

        public MathConverter MathConverter { get; set; }

        public ZhuyinReverseConverter ZhuyinConverter { get; set; }

        /// <summary>
        /// 是否抑制點字轉換的回饋事件。
        /// 範例：
        ///     brProcessor.SuppressEvents = true;  // 關閉點字處理器事件
        ///     BrailleLine brLine = brProcessor.ConvertLine("測試");
        ///     brProcessor.SuppressEvents = false; // 恢復點字處理器事件
        /// </summary>
        public bool SuppressEvents { get; set; }

        public List<CharPosition> InvalidChars { get; }

        public bool HasError
        {
            get
            {
                if (_errorMsg.Length > 0 || InvalidChars.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public string ErrorMessage
        {
            get { return _errorMsg.ToString(); }
        }

        public ContextTagManager ContextManager { get; private set; }

        #endregion

        #region 事件

        public event EventHandler<ConversionFailedEventArgs> ConvertionFailed
        {
            add
            {
                _conversionFailedEvent += value;
            }
            remove
            {
                _conversionFailedEvent -= value;
            }
        }

        public event EventHandler<TextConvertedEventArgs> TextConverted
        {
            add
            {
                _textConvertedEvent += value;
            }
            remove
            {
                _textConvertedEvent -= value;
            }
        }

        #endregion

        #region 事件方法

        protected virtual void OnConvertionFailed(ConversionFailedEventArgs args)
        {
            // 將無效字元記錄於內部變數。

            InvalidChars.Add(args.InvalidChar);

            if (SuppressEvents)
                return;

            if (_conversionFailedEvent != null)
            {
                _conversionFailedEvent(this, args);
            }
        }

        protected virtual void OnTextConverted(TextConvertedEventArgs args)
        {
            if (SuppressEvents)
                return;

            if (_textConvertedEvent != null)
            {
                _textConvertedEvent(this, args);
            }
        }

        #endregion

        public void AddConverter(WordConverter cvt)
        {
            // 設定已知的轉換器。

            if (cvt is ContextTagConverter)
            {
                ControlTagConverter = (ContextTagConverter)cvt;
                return;
            }
            if (cvt is ChineseWordConverter)
            {
                ChineseConverter = (ChineseWordConverter)cvt;
                return;
            }
            if (cvt is EnglishWordConverter)
            {
                EnglishConverter = (EnglishWordConverter)cvt;
                return;
            }
            if (cvt is MathConverter)   // 數學符號轉換器.
            {
                MathConverter = (MathConverter)cvt;
                return;
            }
            if (cvt is PhoneticConverter)   // 音標轉換器.
            {
                _phoneticConverter = (PhoneticConverter)cvt;
                return;
            }

            // 加入其他未知的轉換器。
            if (_converters.IndexOf(cvt) < 0)
                _converters.Add(cvt);
        }

        public void RemoveConverter(WordConverter cvt)
        {
            // 移除已知的轉換器。

            if (cvt is ContextTagConverter)
            {
                ControlTagConverter = null;
                return;
            }
            if (cvt is ChineseWordConverter)
            {
                ChineseConverter = null;
                return;
            }
            if (cvt is EnglishWordConverter)
            {
                EnglishConverter = null;
                return;
            }
            if (cvt is MathConverter)
            {
                MathConverter = null;
                return;
            }
            if (cvt is PhoneticConverter)   // 音標轉換器.
            {
                _phoneticConverter = null;
                return;
            }

            _converters.Remove(cvt);
        }

        /// <summary>
        /// 根據指定的類別名稱傳回對應之 word converter 物件。
        /// 主要是用在需要單獨轉換一個中文字的時候。
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public WordConverter GetConverter(string className)
        {
            foreach (WordConverter cvt in _converters)
            {
                if (cvt.GetType().Name.Equals(className, StringComparison.CurrentCultureIgnoreCase))
                    return cvt;
            }
            return null;
        }

        #region 轉換函式

        /// <summary>
        /// 在開始轉換文件之前，先呼叫此函式以進行初始化。
        /// </summary>
        public void InitializeForConversion()
        {
            _errorMsg.Length = 0;
            InvalidChars.Clear();
            ContextManager.Reset();
        }

        /// <summary>
        /// 簡單版本的轉換方法：不處理任何標籤額外格式，單純轉換文字符號。
        /// 此方法可用來轉換 context tag 裡面的文字。
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public BrailleLine SimpleConvertText(string Text)
        {
            var outputBrLine = new BrailleLine();

            string orgLine = Text;	// 保存原始的字串。

            Text = StrHelper.Reverse(Text);
            Stack<char> charStack = new Stack<char>(Text);

            char ch;
            List<BrailleWord> brWordList;
            StringBuilder convertedText = new StringBuilder();

            ConversionFailedEventArgs cvtFailedArgs = new ConversionFailedEventArgs();
            TextConvertedEventArgs textCvtArgs = new TextConvertedEventArgs();

            while (charStack.Count > 0)
            {
                brWordList = ConvertWord(charStack);

                if (brWordList != null && brWordList.Count > 0)
                {
                    // 成功轉換成點字，有 n 個字元會從串流中取出
                    outputBrLine.Words.AddRange(brWordList);

                    convertedText.Length = 0;
                    foreach (BrailleWord brWord in brWordList)
                    {
                        convertedText.Append(brWord.Text);
                    }
                    textCvtArgs.SetArgValues(0, convertedText.ToString());
                    OnTextConverted(textCvtArgs);
                }
                else
                {
                    // 無法判斷和處理的字元應該會留存在串流中，將之取出。
                    ch = charStack.Pop();

                    int charIndex = Text.Length - charStack.Count;

                    // 引發事件。
                    cvtFailedArgs.SetArgs(0, charIndex, orgLine, ch);
                    OnConvertionFailed(cvtFailedArgs);
                    if (cvtFailedArgs.Stop)
                    {
                        break;
                    }
                }
            }
            return outputBrLine;
        }

        /// <summary>
        /// 把一行明眼字串轉換成點字串列。
        /// 此時不考慮一行幾方和斷行的問題，只進行單純的轉換。
        /// 斷行由其他函式負責處理，因為有些點字規則必須在斷行時才能處理。
        /// </summary>
        /// <param name="line">輸入的明眼字串。</param>
        /// <param name="lineNumber">字串的行號。此參數只是用來當轉換失敗時，傳給轉換失敗事件處理常式的資訊。</param>
        /// <param name="isTitle">輸出參數，是否為標題。</param>
        /// <returns>點字串列。若則傳回 null，表示該列不需要轉成點字。</returns>
        public BrailleLine ConvertLine(string line, int lineNumber)
        {
            if (line == null)
                return null;

            BrailleLine brLine = new BrailleLine();

            string orgLine = line;	// 保存原始的字串。

            // 把換行符號之後的字串去掉
            int i = line.IndexOfAny(new char[] { '\r', '\n' });
            if (i >= 0)
            {
                line = line.Substring(0, i);
            }

            // 若去掉換行字元之後變成空字串，則傳回只包含一個空方的列。
            if (String.IsNullOrEmpty(line))
            {
                brLine.Words.Add(BrailleWord.NewBlank());
                return brLine;
            }

            // 替換組態檔中指定的字串
            line = ReplaceTextDefinedInAppConfig(line);

            // 預先處理特殊標籤的字元替換。
            line = ReplaceSimpleTagsWithConvertableText(line);
            if (line == null)
                return null;

            // 原書頁碼可能會輸入羅馬數字，所以把底下檢查數字格式的程式碼註解掉。
            // 如果是原書頁碼，先檢查格式是否正確。
            //try
            //{
            //	GetOrgPageNumber(line);
            //}
            //catch (Exception ex)
            //{
            //	m_ErrorMsg.Append(String.Format("第 {0} 列 : ", lineNumber));
            //	m_ErrorMsg.Append(ex.Message);
            //	m_ErrorMsg.Append("\r\n");
            //	return null;
            //}

            line = StrHelper.Reverse(line);
            Stack<char> charStack = new Stack<char>(line);

            char ch;
            List<BrailleWord> brWordList;
            StringBuilder text = new StringBuilder();

            ConversionFailedEventArgs cvtFailedArgs = new ConversionFailedEventArgs();
            TextConvertedEventArgs textCvtArgs = new TextConvertedEventArgs();

            while (charStack.Count > 0)
            {
                brWordList = ConvertWord(charStack);

                if (brWordList != null && brWordList.Count > 0)
                {
                    // 成功轉換成點字，有 n 個字元會從串流中取出
                    brLine.Words.AddRange(brWordList);

                    // 通知事件
                    textCvtArgs.SetArgValues(lineNumber, BrailleWordHelper.ToTextString(brWordList));
                    OnTextConverted(textCvtArgs);
                }
                else
                {
                    // 無法判斷和處理的字元應該會留存在串流中，將之取出。
                    ch = charStack.Pop();

                    int charIndex = line.Length - charStack.Count;

                    // 引發事件。
                    cvtFailedArgs.SetArgs(lineNumber, charIndex, orgLine, ch);
                    OnConvertionFailed(cvtFailedArgs);
                    if (cvtFailedArgs.Stop)
                    {
                        break;
                    }
                }

                // 如果進入分數情境，就把整個分數處理完。
                if (ContextManager.IsActive(ContextTagNames.Fraction))
                {
                    try
                    {
                        brWordList = ConvertFraction(lineNumber, charStack);
                        if (brWordList != null && brWordList.Count > 0)
                        {
                            // 成功轉換成點字，有 n 個字元會從串流中取出
                            brLine.Words.AddRange(brWordList);
                        }
                    }
                    catch (Exception ex)
                    {
                        _errorMsg.Append(String.Format("第 {0} 列 : ", lineNumber));
                        _errorMsg.Append(ex.Message);
                        _errorMsg.Append("\r\n");
                    }
                }
            }

            /* 到此階段，一列文字已經被初步轉換成一個包含點字物件串列的 BrailleLine。
             * 有些可轉換的 context tags 會保留到此階段才處理（可能是刪除或者轉換成特定文字與點字）
             */


            ConvertContextTags(brLine);

            // 注意: 不要隨意調動底下各項檢查規則的順序!

            ChineseBrailleRule.ApplySpecificNameAndBookNameRules(brLine);  // 處理私名號和書名號的規則。

            ChineseBrailleRule.ApplyPunctuationRules(brLine);   // 套用中文標點符號規則。

            // 不刪除多餘空白，因為原本輸入時可能就希望縮排。
            //ChineseBrailleRule.ShrinkSpaces(brLine);	// 把連續全形空白刪到只剩一個。

            // 將編號的數字修正成上位點。
            if (EnglishConverter != null)
            {
                EnglishBrailleRule.FixNumbers(brLine, EnglishConverter.BrailleTable as EnglishBrailleTable);
            }

            EnglishBrailleRule.ApplyCapitalRule(brLine);    // 套用大寫規則。
            EnglishBrailleRule.ApplyDigitRule(brLine);		// 套用數字規則（加數字符號）。
            EnglishBrailleRule.AddSpaces(brLine);           // 補加必要的空白。

            ChineseBrailleRule.EnsureNoDigitSymbolInBrackets(brLine);    // 套用括弧規則。

            // 不刪除多於空白，因為原本輸入時可能就希望縮排。
            //EnglishBrailleRule.ShrinkSpaces(brLine);        // 把連續空白刪到只剩一個。

            return brLine;
        }

        /// <summary>
        /// 把一行明眼字串轉換成點字串列。
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        /// <see cref="BrailleProcessor.ConvertLine(int,string)"/>
        public BrailleLine ConvertLine(string line)
        {
            return ConvertLine(line, 1);
        }

        /// <summary>
        /// 把一個明眼字元轉換成點字（BrailleWord）。
        /// 原則上，能夠在這裡處理掉點字特殊規則的，就盡量在這裡處理掉，
        /// 特別是不可斷行分開的點字，例如：一個中文字的所有點字碼、特殊單音字附加的「ㄦ」等等。
        /// </summary>
        /// <param name="reader">字串流。</param>
        /// <returns>若成功轉換成點字，則傳回已轉換的點字 BrailleWord 物件串列，否則傳回 null。</returns>
        /// <remarks>若轉換成功，則已轉換的字元會從串流中讀出，否則該字元仍會保留在串流中。</remarks>
        public List<BrailleWord> ConvertWord(Stack<char> chars)
        {
            List<BrailleWord> brWordList = null;

            // Two-pass 處理（因為有些點字必須再交給其它點字轉換器，故需兩次）。
            for (int pass = 0; pass < 2; pass++)
            {
                if (chars.Count < 1)
                    break;


                // TODO: 檢查 ContextnTag 物件的 IsSingleLine 屬性，若為 true，則不允許標籤以外的其餘文字。

                // 1. 轉換語境標籤。NOTE: 語境標籤一定要先處理!
                if (chars.Count > 0 && ControlTagConverter != null)
                {
                    brWordList = ControlTagConverter.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }

                // 2. 轉換座標符號
                if (chars.Count > 0 && _coordConverter != null && ContextManager.IsActive(ContextTagNames.Coordinate))
                {
                    brWordList = _coordConverter.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }

                // 3. 轉換數學符號。
                if (chars.Count > 0 && ContextManager.IsActive(ContextTagNames.Math) && MathConverter != null)
                {
                    brWordList = MathConverter.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }

                // 4. 轉換表格符號。
                if (chars.Count > 0 && ContextManager.IsActive(ContextTagNames.Table) && _tableConverter != null)
                {
                    brWordList = _tableConverter.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }

                // 5. 轉換音標符號.
                if (chars.Count > 0 && ContextManager.IsActive(ContextTagNames.Phonetic) && _phoneticConverter != null)
                {
                    brWordList = _phoneticConverter.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }

                // 6. 轉換中文。
                if (chars.Count > 0 && ChineseConverter != null)
                {
                    // 若成功轉換成點字，就不再 pass 給其它轉換器。
                    brWordList = ChineseConverter.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }

                // 7. 轉換英文。
                if (chars.Count > 0 && EnglishConverter != null)
                {
                    // 若成功轉換成點字，就不再 pass 給其它轉換器。
                    brWordList = EnglishConverter.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }
            }

            if (chars.Count > 0)
            {
                // 其它註冊的轉換器。
                foreach (WordConverter cvt in _converters)
                {
                    // 若其中一個轉換器成功轉換成點字，就不再 pass 給其它轉換器。
                    brWordList = cvt.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }
            }

            // TODO: 碰到無法轉換成點字的情況時，觸發事件通知呼叫端處理，例如：顯示在在訊息視窗裡。

            return null;
        }

        /// <summary>
        /// 剖析分數格式的字串，分別取出整數部份、分子、以及分母。
        /// 分數格式範例： 三又五分之一的分數字串表示為： 3&1/5。
        /// </summary>
        /// <param name="s"></param>
        /// <param name="intPart"></param>
        /// <param name="numerator"></param>
        /// <param name="denumerator"></param>
        private void ParseFraction(string s, out string intPart, out string numerator, out string denumerator)
        {
            // 取出整數部份
            intPart = "";
            int idxAmpersand = s.IndexOf('&');
            if (idxAmpersand > 0)
            {
                intPart = s.Substring(0, idxAmpersand);
            }

            // 取出分子
            numerator = "";
            int idxBar = s.IndexOf("/");
            if (idxBar <= 0 || idxBar < idxAmpersand)
            {
                throw new Exception("分數的格式不正確: 沒有分數的分隔符號 ('/') 或者分隔符號的位置不對。");
            }
            numerator = s.Substring(idxAmpersand + 1, idxBar - idxAmpersand - 1);

            // 取出分母
            denumerator = s.Substring(idxBar + 1);

            if (String.IsNullOrEmpty(numerator) || String.IsNullOrEmpty(denumerator))
            {
                throw new Exception("分數的格式不正確!! 無法剖析分子或分母。");
            }
        }


        /// <summary>
        /// 對上一個階段所轉換出來的 BrailleLine 物件逐一找出尚未處理的語境標籤，並轉換成對應的點字。
        /// </summary>
        /// <param name="brLine"></param>
        private void ConvertContextTags(BrailleLine brLine)
        {
            int index = 0;
            while (index < brLine.WordCount)
            {
                var brWord = brLine[index];

                if (!brWord.IsContextTag || brWord.ContextTag == null)
                {
                    index++;
                    continue;
                }

                // BrailleWord 物件如果仍是 context tag，則其 Text 屬性就是 tag name。
                // 所以可利用 Text 屬性來判斷這是哪一個 context tag，以及是起始標籤還是結束標籤。

                var ctag = brWord.ContextTag;

                if (XmlTagHelper.IsBeginTag(brWord.Text))
                {
                    // 處理起始標籤。

                    // 優先使用預先建立好的點字串列。
                    if (ctag.PrefixBrailleWords.Count > 0)
                    {
                        index++;
                        brLine.Words.InsertRange(index, ctag.PrefixBrailleWords);
                        index += ctag.PrefixBrailleWords.Count;
                        continue;
                    }
                    // 沒有點字串列的時候才去轉換文字。
                    if (!String.IsNullOrEmpty(ctag.ConvertablePrefix))
                    {
                        var newBrLine = SimpleConvertText(ctag.ConvertablePrefix);
                        foreach (var aWord in newBrLine.Words)
                        {
                            aWord.IsConvertedFromTag = true;
                        }
                        index++;
                        brLine.Words.InsertRange(index, newBrLine.Words);
                        index += newBrLine.WordCount;
                        continue;
                    }
                    // 是起始標籤，但不需要轉換成文字：保留此 BraillWord，以便識別這是個 context tag。
                    brWord.Cells.Clear();
                    index++;
                    continue;

                    // 註：也許可以不用急著刪除，而讓它活到轉換程序的最後階段，也就是由清除全部 context tag 的步驟來刪除。
                }
                else if (XmlTagHelper.IsEndTag(brWord.Text))
                {
                    // 處理結束標籤。

                    // 優先使用預先建立好的點字串列。
                    if (ctag.PostfixBrailleWords.Count > 0)
                    {
                        index++;
                        brLine.Words.InsertRange(index, ctag.PostfixBrailleWords);
                        index += ctag.PostfixBrailleWords.Count;
                        continue;
                    }
                    // 沒有點字串列的時候才去轉換文字。
                    if (!String.IsNullOrEmpty(ctag.ConvertablePostfix))
                    {
                        var newBrLine = SimpleConvertText(ctag.ConvertablePostfix);
                        foreach (var aWord in newBrLine.Words)
                        {
                            aWord.IsConvertedFromTag = true;
                        }
                        brLine.Words.InsertRange(index, newBrLine.Words);
                        index += newBrLine.WordCount + 1;
                        continue;
                    }
                    // 是結束標籤，但不需要轉換成文字：保留此 BraillWord，以便識別這是個 context tag。
                    brWord.Cells.Clear();
                    index++;
                    continue;
                }
                // 不是起始標籤，也不是結束標籤。這裡應該不可能執行到!
                index++;
            }
        }

        /// <summary>
        /// 轉換分數。
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <param name="chars"></param>
        /// <returns></returns>
        private List<BrailleWord> ConvertFraction(int lineNumber, Stack<char> chars)
        {
            char[] charAry = chars.ToArray();
            string s = new string(charAry);
            int idxEof = s.IndexOf(XmlTagHelper.GetEndTagName(ContextTagNames.Fraction));   // end of fraction
            if (idxEof < 0)
            {
                throw new Exception("<分數> 標籤有起始但沒有結束標籤!");
            }

            s = s.Substring(0, idxEof); // 從字串頭取到 </分數> 標籤之前。

            string intPart;
            string numerator;
            string denumerator;

            ParseFraction(s, out intPart, out numerator, out denumerator);

            // Note: 整數部份、分子、分母都有可能是英文字母（代數）。

            string temp;
            Stack<char> charStack;
            List<BrailleWord> brWordList = null;

            List<BrailleWord> brWordListIntPart = new List<BrailleWord>();

            if (!String.IsNullOrEmpty(intPart))
            {
                // 將整數部份轉換成點字串列。
                temp = StrHelper.Reverse(intPart);
                charStack = new Stack<char>(temp);

                while (charStack.Count > 0)
                {
                    brWordList = ConvertWord(charStack);
                    if (brWordList == null)
                    {
                        throw new Exception("無法轉換分數的整數部份!");
                    }
                    else
                    {
                        brWordListIntPart.AddRange(brWordList);
                    }
                }
            }

            // 將分子部份轉換成點字串列。
            temp = StrHelper.Reverse(numerator + "/");
            charStack = new Stack<char>(temp);
            List<BrailleWord> brWordListNumerator = new List<BrailleWord>();
            while (charStack.Count > 0)
            {
                brWordList = ConvertWord(charStack);
                if (brWordList == null)
                {
                    throw new Exception("無法轉換分數的分子部份!");
                }
                else
                {
                    brWordListNumerator.AddRange(brWordList);
                }
            }
            // 分子的數字不要加數符
            foreach (BrailleWord brWord in brWordListNumerator)
            {
                brWord.NoDigitCell = true;
            }
            // 補上分子的點字符號
            brWordListNumerator[0].Cells.Insert(0, BrailleCell.GetInstance(new int[] { 1, 4, 5, 6 }));
            if (brWordListIntPart.Count > 0)
            {
                brWordListNumerator[0].Cells.Insert(0, BrailleCell.GetInstance(new int[] { 4, 5, 6 }));
            }

            // 將分母部份轉換成點字串列。
            temp = StrHelper.Reverse(denumerator);
            charStack = new Stack<char>(temp);
            List<BrailleWord> brWordListDenumerator = new List<BrailleWord>();
            while (charStack.Count > 0)
            {
                brWordList = ConvertWord(charStack);
                if (brWordList == null)
                {
                    throw new Exception("無法轉換分數的分母部份!");
                }
                else
                {
                    brWordListDenumerator.AddRange(brWordList);
                }
            }
            // 分母的數字不要加數符
            foreach (BrailleWord brWord in brWordListDenumerator)
            {
                brWord.NoDigitCell = true;
            }

            // 補上分母後面的點字符號
            BrailleWord lastBrWord = brWordListDenumerator[brWordListDenumerator.Count - 1];
            if (brWordListIntPart.Count > 0)
            {
                lastBrWord.Cells.Add(BrailleCell.GetInstance(new int[] { 4, 5, 6 }));
            }
            lastBrWord.Cells.Add(BrailleCell.GetInstance(new int[] { 3, 4, 5, 6 }));

            // 結合整數部份、分子、分母至同一個串列。
            List<BrailleWord> brWordListFraction = new List<BrailleWord>();
            brWordListFraction.AddRange(brWordListIntPart);
            brWordListFraction.AddRange(brWordListNumerator);
            brWordListFraction.AddRange(brWordListDenumerator);

            // 完成! 從傳入的字元堆疊中取出已經處理的字元。
            while (idxEof > 0)
            {
                chars.Pop();
                idxEof--;
            }

            return brWordListFraction;
        }

        public string ReplaceTextDefinedInAppConfig(string line)
        {
            var sb = new StringBuilder(line);
            foreach (var item in _autoReplacedText)
            {
                sb.Replace(item.Key, item.Value);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 在進行轉換之前預先處理一列中的所有特殊標籤，將這些標籤替換成特定字元。
        /// </summary>
        /// <param name="line"></param>
        /// <returns>傳回置換過的字串。若傳回 null，表示這行不要轉換成點字。</returns>
        public string ReplaceSimpleTagsWithConvertableText(string line)
        {
            // 處理標題標籤 (這是舊的程式碼，保留供參考，若未來有需要類似的前置標籤處理，可依樣畫葫蘆)
            //isTitle = false;
            //string tagName = TextTag.DocTitle;
            //int startIdx = line.IndexOf(tagName);
            //int endIdx = line.IndexOf(tagName.Insert(1, "/"));
            //if (startIdx >= 0 && endIdx >= 0 && startIdx < endIdx)
            //{
            //    startIdx = startIdx + tagName.Length;
            //    string title = line.Substring(startIdx, endIdx - startIdx);
            //    isTitle = true; // 傳入的列是標題文字。
            //    return title;
            //}

            return Regex.Replace(line, RegExpPatterns.Tags, new MatchEvaluator(OnMatchedTagFound));
        }

        /// <summary>
        /// 每當有找到匹配字串時觸發此事件。
        /// </summary>
        /// <param name="token"></param>
        /// <returns>傳回用來把這次找到的 token 置換掉的字串。</returns>
        private string OnMatchedTagFound(Match token)
        {
            if (SimpleTag.IsSimpleTag(token.Value))
            {
                return SimpleTag.GetTextValue(token.Value);
            }

            // 結束標籤
            if (token.Value.StartsWith("</"))
            {
                string key = token.Value.Remove(1, 1);  // 把 '/' 字元去掉，即得到起始標籤名稱。
                if (SimpleTag.IsSimpleTag(key))
                {
                    return String.Empty;
                }
            }

            return token.Value;
        }

        #endregion

        #region 格式化、編排、與修正函式

        /// <summary>
        /// 編排點字文件，包括斷行、移除所有語境標籤。
        /// </summary>
        public void FormatDocument(BrailleDocument doc)
        {
            ContextTagManager context = new ContextTagManager();

            int index = 0;
            while (index < doc.Lines.Count)
            {
                ProcessIndentTags(doc, index, context);
                index += BrailleDocumentFormatter.FormatLine(doc, index, context);
            }
        }

        /// <summary>
        /// 處理縮排語境標籤：碰到縮排標籤時，將縮排次數更新至 ContextTagManager 物件，並移除此縮排標籤。
        /// 
        /// NOTE: 縮排標籤必須位於列首，一列可有多個連續縮排標籤，例如：＜縮排＞＜縮排＞。
        /// </summary>
        /// <param name="brDoc"></param>
        /// <param name="lineIndex"></param>
        /// <param name="context">情境物件。</param>
        /// <returns></returns>
        public void ProcessIndentTags(BrailleDocument brDoc, int lineIndex, ContextTagManager context)
        {
            BrailleLine brLine = brDoc.Lines[lineIndex];
            int wordIdx = 0;
            IContextTag ctag;

            while (brLine.WordCount > 0)
            {
                ctag = context.Parse(brLine[0].Text, ContextTagNames.Indent);
                if (ctag != null)
                {
                    brLine.RemoveAt(wordIdx);
                }
                else
                {
                    break;
                }
            }
        }

        #endregion

    }
}
