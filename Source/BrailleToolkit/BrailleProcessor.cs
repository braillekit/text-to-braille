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
using Huanlin.Extensions;

namespace BrailleToolkit
{
    public struct CharPosition
	{
		public char CharValue { get; set; }		// 字元
		public int LineNumber { get; set; }		// 第幾列
		public int CharIndex { get; set; }      // 第幾個字元
    }

	public class ConvertionFailedEventArgs : EventArgs
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

        private CoordinateConverter m_CoordConverter;
		private TableConverter m_TableConverter;
		private PhoneticConverter m_PhoneticConverter;

        // Extended converters
        private List<WordConverter> m_Converters;

        private StringBuilder m_ErrorMsg;           // 轉換過程中發生的錯誤訊息。

        private event EventHandler<ConvertionFailedEventArgs> m_ConvertionFailedEvent;
		private event EventHandler<TextConvertedEventArgs> m_TextConvertedEvent;

        private Dictionary<string, string> _autoReplacedText;

        #region 建構函式

        private BrailleProcessor(ZhuyinReverseConverter zhuyinConverter)
		{
            m_Converters = new List<WordConverter>();

            ControlTagConverter = new ContextTagConverter();
            ChineseConverter = new ChineseWordConverter(zhuyinConverter);
            EnglishConverter = new EnglishWordConverter();
            MathConverter = new MathConverter();
            m_CoordConverter = new CoordinateConverter();
			m_TableConverter = new TableConverter();
			m_PhoneticConverter = new PhoneticConverter();

            ContextManager = new ContextTagManager();

            InvalidChars = new List<CharPosition>();
			m_ErrorMsg = new StringBuilder();
            SuppressEvents = false;

            // 轉點字之前，預先替換的文字
            var replacedText = AppGlobals.Config.AutoReplacedText.EnsureNotEnclosedWith("{", "}");
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
				if (m_ErrorMsg.Length > 0 || InvalidChars.Count > 0)
				{
					return true;
				}
				return false;
			}
		}

		public string ErrorMessage
		{
			get { return m_ErrorMsg.ToString(); }
		}

        public ContextTagManager ContextManager { get; private set; }

        #endregion

        #region 事件

        public event EventHandler<ConvertionFailedEventArgs> ConvertionFailed
		{
			add
			{
				m_ConvertionFailedEvent += value;
			}
			remove
			{
				m_ConvertionFailedEvent -= value;
			}
		}

		public event EventHandler<TextConvertedEventArgs> TextConverted
		{
			add
			{
				m_TextConvertedEvent += value;
			}
			remove
			{
				m_TextConvertedEvent -= value;
			}
		}

		#endregion

		#region 事件方法

		protected virtual void OnConvertionFailed(ConvertionFailedEventArgs args)
		{
            // 將無效字元記錄於內部變數。

			InvalidChars.Add(args.InvalidChar);

            if (SuppressEvents)
                return;

			if (m_ConvertionFailedEvent != null)
			{
				m_ConvertionFailedEvent(this, args);
			}
		}

		protected virtual void OnTextConverted(TextConvertedEventArgs args)
		{
            if (SuppressEvents)
                return;

			if (m_TextConvertedEvent != null)
			{
				m_TextConvertedEvent(this, args);
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
                ChineseConverter = (ChineseWordConverter) cvt;
                return;
            }
            if (cvt is EnglishWordConverter)
            {
                EnglishConverter = (EnglishWordConverter) cvt;
                return;
            }
            if (cvt is MathConverter)   // 數學符號轉換器.
            {
                MathConverter = (MathConverter)cvt;
                return;
            }
			if (cvt is PhoneticConverter)	// 音標轉換器.
			{
				m_PhoneticConverter = (PhoneticConverter)cvt;
				return;
			}

            // 加入其他未知的轉換器。
            if (m_Converters.IndexOf(cvt) < 0)
                m_Converters.Add(cvt);
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
			if (cvt is PhoneticConverter)	// 音標轉換器.
			{
				m_PhoneticConverter = null;
				return;
			}
            
            m_Converters.Remove(cvt);
        }

        /// <summary>
        /// 根據指定的類別名稱傳回對應之 word converter 物件。
        /// 主要是用在需要單獨轉換一個中文字的時候。
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public WordConverter GetConverter(string className)
        {            
            foreach (WordConverter cvt in m_Converters)
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
			m_ErrorMsg.Length = 0;
            InvalidChars.Clear();
            ContextManager.Reset();
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

			ConvertionFailedEventArgs cvtFailedArgs = new ConvertionFailedEventArgs();
			TextConvertedEventArgs textCvtArgs = new TextConvertedEventArgs();

			while (charStack.Count > 0)
			{
				brWordList = ConvertWord(charStack);

				if (brWordList != null && brWordList.Count > 0)	
				{
					// 成功轉換成點字，有 n 個字元會從串流中取出
					brLine.Words.AddRange(brWordList);

					text.Length = 0;
					foreach (BrailleWord brWord in brWordList) 
					{
						text.Append(brWord.Text);
					}
					textCvtArgs.SetArgValues(lineNumber, text.ToString());
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
						m_ErrorMsg.Append(String.Format("第 {0} 列 : ", lineNumber));
						m_ErrorMsg.Append(ex.Message);
						m_ErrorMsg.Append("\r\n");
					}
				}
			}

            /* 到此階段，一列文字已經被初步轉換成一個包含點字物件串列的 BrailleLine。
             * 有些可轉換的 context tags 會保留到此階段才處理（可能是刪除或者轉換成特定文字與點字）
             */


            ConvertContextTags(brLine);

            // 注意: 不要隨意調動底下各項檢查規則的順序!

            ChineseBrailleRule.ApplySpecificNameAndBookNameRules(brLine);  // 處理私名號和書名號的規則。

            ChineseBrailleRule.ApplyPunctuationRules(brLine);	// 套用中文標點符號規則。

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

			ChineseBrailleRule.ApplyBracketRule(brLine);	// 套用括弧規則。

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

                // 1. 轉換語境標籤。NOTE: 語境標籤一定要先處理!
				if (chars.Count > 0 && ControlTagConverter != null)
                {
                    brWordList = ControlTagConverter.Convert(chars, ContextManager);
                    if (brWordList != null && brWordList.Count > 0)
                        return brWordList;
                }

                // 2. 轉換座標符號
				if (chars.Count > 0 && m_CoordConverter != null && ContextManager.IsActive(ContextTagNames.Coordinate))
                {
                    brWordList = m_CoordConverter.Convert(chars, ContextManager);
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
				if (chars.Count > 0 && ContextManager.IsActive(ContextTagNames.Table) && m_TableConverter != null)
				{
					brWordList = m_TableConverter.Convert(chars, ContextManager);
					if (brWordList != null && brWordList.Count > 0)
						return brWordList;
				}

				// 5. 轉換音標符號.
				if (chars.Count > 0 && ContextManager.IsActive(ContextTagNames.Phonetic) && m_PhoneticConverter != null)
				{
					brWordList = m_PhoneticConverter.Convert(chars, ContextManager);
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
				if (chars.Count > 0 &&  EnglishConverter != null)
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
				foreach (WordConverter cvt in m_Converters)
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
                // 所以可利用 Text 屬性來判斷這是哪一個 context tag
              
                var ctag = brWord.ContextTag;

                if (XmlTagHelper.IsBeginTag(brWord.Text))
                {
                    // 處理起始標籤。
                    brWord.IsContextBeginTag = true;

                    // 優先使用預先建立好的點字串列。
                    if (ctag.PrefixBrailleWords.Count > 0)
                    {
                        brWord.Clear();
                        brLine.RemoveAt(index);
                        brLine.Words.InsertRange(index, ctag.PrefixBrailleWords);
                        index += ctag.PrefixBrailleWords.Count;
                        continue;
                    }
                    // 沒有點字串列的時候才去轉換文字。
                    if (!String.IsNullOrEmpty(ctag.ConvertablePrefix))
                    {
                        brWord.Text = ctag.ConvertablePrefix;
                        brWord.IsContextTag = false;    // 轉換成文字之後就不是語境標籤了。
                        brWord.ContextTag = null;
                        string brCode = ChineseBrailleTable.GetInstance().Find(ctag.ConvertablePrefix);
                        if (brCode == null)
                        {
                            throw new Exception($"無法轉換語境標籤 '{ctag.TagName}' 的對應文字: {ctag.ConvertablePrefix}");
                        }
                        brWord.AddCell(brCode);
                    }
                    else
                    {
                        // 是起始標籤，但不需要轉換成文字，便可刪除此 BrailleWord。
                        brWord.Clear();
                        brLine.RemoveAt(index);
                        continue;

                        // 註：也許可以不用急著刪除，而讓它活到轉換程序的最後階段，也就是由清除全部 context tag 的步驟來刪除。
                    }
                }
                else if (XmlTagHelper.IsEndTag(brWord.Text))
                {
                    // 處理結束標籤。
                    brWord.IsContextEndTag = true;

                    // 優先使用預先建立好的點字串列。
                    if (ctag.PostfixBrailleWords.Count > 0)
                    {
                        brWord.Clear();
                        brLine.RemoveAt(index);
                        brLine.Words.InsertRange(index, ctag.PostfixBrailleWords);
                        index += ctag.PostfixBrailleWords.Count;
                        continue;
                    }
                    // 沒有點字串列的時候才去轉換文字。
                    if (!String.IsNullOrEmpty(ctag.ConvertablePostfix))
                    {
                        brWord.Text = ctag.ConvertablePostfix;
                        brWord.IsContextTag = false;    // 轉換成文字之後就不是語境標籤了。
                        brWord.ContextTag = null;
                        string brCode = ChineseBrailleTable.GetInstance().Find(ctag.ConvertablePostfix);
                        if (brCode == null)
                        {
                            throw new Exception($"無法轉換語境標籤 '{ctag.TagName}' 的對應文字: {ctag.ConvertablePostfix}");
                        }
                        brWord.AddCell(brCode);
                    }
                    else
                    {
                        // 是結束標籤，但不需要轉換成文字，便可刪除此 BrailleWord。
                        brWord.Clear();
                        brLine.RemoveAt(index);
                        continue;
                    }
                }
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
			int idxEof = s.IndexOf(XmlTagHelper.GetEndTagName(ContextTagNames.Fraction));	// end of fraction
			if (idxEof < 0) 
			{
				throw new Exception("<分數> 標籤有起始但沒有結束標籤!");
			}

			s = s.Substring(0, idxEof);	// 從字串頭取到 </分數> 標籤之前。

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
			brWordListNumerator[0].Cells.Insert(0, BrailleCell.GetInstance(new int[] {1, 4, 5, 6}));
			if (brWordListIntPart.Count > 0) 
			{
				brWordListNumerator[0].Cells.Insert(0, BrailleCell.GetInstance(new int[] {4, 5, 6}));
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
			BrailleWord lastBrWord = brWordListDenumerator[brWordListDenumerator.Count-1];
			if (brWordListIntPart.Count > 0) 
			{
				lastBrWord.Cells.Add(BrailleCell.GetInstance(new int[] {4, 5, 6}));
			}
			lastBrWord.Cells.Add(BrailleCell.GetInstance(new int[] {3, 4, 5, 6}));
		
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
                    return " ";
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
                index += FormatLine(doc, index, context);
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

        /// <summary>
        /// 編排指定的列。此函式會將指定的列斷行，並移除所有語境標籤。
        /// </summary>
        /// <param name="brDoc">點字文件。</param>
        /// <param name="lineIndex">欲重新編排的列索引。</param>
        /// <returns>傳回編排後的列數。</returns>
        public int FormatLine(BrailleDocument brDoc, int lineIndex, ContextTagManager context)
        {
            BrailleLine brLine = brDoc.Lines[lineIndex];

            var formattedLines = FormatLine(brLine, brDoc.CellsPerLine, context);

            if (formattedLines.Count < 1)
            {
                brDoc.RemoveLine(lineIndex);
                return 0;
            }

            if (formattedLines.Count == 1)   // 沒有斷行？
            {
                return 1;
            }

            // 有斷行，先移除原始的 line，再加入新的斷行結果。
            brLine.Clear();
            brDoc.RemoveLine(lineIndex);

            // 加入斷行後的 lines
            brDoc.Lines.InsertRange(lineIndex, formattedLines);

            return formattedLines.Count;
        }

        /// <summary>
        /// 對指定的 BrailleLine 格式化，包括斷行、移除語境標籤。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="context"></param>
        /// <returns>可能是空的串列、經過格式化的單行串列，或者因斷行而產生的多行串列。</returns>
        public List<BrailleLine> FormatLine(BrailleLine brLine, int cellsPerLine, ContextTagManager context)
        {
            var outputLines = new List<BrailleLine>();

            RemoveContextTagsButTitle(brLine);   // 清除語境標籤，除了標題標籤。

            if (brLine.IsEmpty())
            {
                return outputLines;
            }

            outputLines.Add(brLine);

            var newLines = BreakLine(brLine, cellsPerLine, context);

            if (newLines == null || newLines.Count < 1)   // 沒有斷行？
            {
                return outputLines;
            }

            return newLines;
        }

        /// <summary>
        /// 移除所有語境標籤，除了標題標籤。
        /// </summary>
        public void RemoveContextTagsButTitle(BrailleLine brLine)
        {
            BrailleWord brWord;

            for (int i = brLine.WordCount - 1; i >= 0; i--)
            {
                brWord = brLine.Words[i];
                if (brWord.IsContextTag && !ContextTagNames.IsTitleTag(brWord.Text))
                {
                    brLine.Words.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 將一行點字串列斷成多行。
        /// </summary>
        /// <param name="brLine">來源點字串列。</param>
        /// <param name="cellsPerLine">每行最大方數。</param>
        /// <param name="context">情境物件。</param>
        /// <returns>斷行之後的多行串列。若為 null 表示無需斷行（指定的點字串列未超過每行最大方數）。</returns>
        public List<BrailleLine> BreakLine(BrailleLine brLine, int cellsPerLine, ContextTagManager context)
		{
            if (context != null && context.IndentCount > 0) // 若目前位於縮排區塊中
            {
                // 每列最大方數要扣掉縮排數量，並於事後補縮排的空方。
                // NOTE: 必須在斷行之後才補縮排的空方!
                cellsPerLine -= context.IndentCount;    
            }

            // 若指定的點字串列未超過每行最大方數，則無須斷行，傳回 null。
            if (brLine.CellCount <= cellsPerLine)
            {
                // 補縮排的空方。
                if (context != null && context.IndentCount > 0) // 若目前位於縮排區塊中
                {
                    Indent(brLine, context.IndentCount);
                }
                return null;
            }

			List<BrailleLine> lines = new List<BrailleLine>();
			BrailleLine newLine = null;
			int wordIndex = 0;
			int breakIndex = 0;
			bool needHyphen = false;
            bool isBroken = false;      // 是否已經斷行了？
            int indents = 0;    // 第一次斷行時，不會有系統自斷加上的縮排，因此初始為 0。
            int maxCells = cellsPerLine;;

            // 計算折行之後的縮排格數。
            indents = CalcNewLineIndents(brLine);            

            while (wordIndex < brLine.WordCount)
			{
				breakIndex = CalcBreakPoint(brLine, maxCells, out needHyphen);

				newLine = brLine.Copy(wordIndex, breakIndex);	// 複製到新行。
				if (needHyphen)	// 是否要附加連字號?
				{
					newLine.Words.Add(new BrailleWord("-", BrailleCellCode.Hyphen));
				}
				newLine.TrimEnd();	// 去尾空白。 

                // 如果是折下來的新行，就自動補上需要縮排的格數。
                if (isBroken)
                {
                    for (int i = 0; i < indents; i++)
                    {
                        newLine.Insert(0, BrailleWord.NewBlank());
                    }
                }

				brLine.RemoveRange(0, breakIndex);				// 從原始串列中刪除掉已經複製到新行的點字。
				wordIndex = 0;
				lines.Add(newLine);

                // 防錯：檢驗每個斷行後的 line 的方數是否超過每列最大方數。
                // 若超過，即表示之前的斷行處理有問題，須立即停止執行，否則錯誤會
                // 直到在雙視編輯的 Grid 顯示時才出現 index out of range，不易抓錯!
                System.Diagnostics.Debug.Assert(newLine.CellCount <= cellsPerLine, "斷行錯誤! 超過每列最大方數!");

				// 被折行之後的第一個字需要再根據規則調整。
				EnglishBrailleRule.ApplyCapitalRule(brLine);    // 套用大寫規則。
				EnglishBrailleRule.ApplyDigitRule(brLine);		// 套用數字規則。

                isBroken = true;    // 已經至少折了一行
                maxCells = cellsPerLine - indents;  // 下一行開始就要自動縮排，共縮 indents 格。
			}

            // 補縮排的空方。
            if (context != null && context.IndentCount > 0) // 若目前位於縮排區塊中
            {
                indents = context.IndentCount;
                foreach (BrailleLine aLine in lines)
                {
                    Indent(aLine, indents);
                }
            }

			return lines;
        }

        private void Indent(BrailleLine brLine, int indents)
        {
            for (int i = 0; i < indents; i++)
            {
                brLine.Insert(0, BrailleWord.NewBlank());
            }
        }

        /// <summary>
        /// 計算折行之後的縮排格數。
        /// </summary>
        /// <param name="brLine"></param>
        /// <returns>縮排格數。</returns>
        private static int CalcNewLineIndents(BrailleLine brLine)
        {
            if (AppGlobals.Config.Braille.AutoIndentNumberedLine)
			{
				int count = 0;
				bool foundOrderedItem = false;

				// 如果是以數字編號開頭（空白略過），自動計算折行的列要縮排幾格。
				foreach (BrailleWord brWord in brLine.Words)
				{
					if (BrailleWord.IsBlank(brWord))
					{
						count++;
						continue;
					}

					if (BrailleWord.IsOrderedListItem(brWord))
					{
						count++;
						foundOrderedItem = true;
						break;
					}
				}

				if (foundOrderedItem)
					return count;
			}

            return 0;
        }

		/// <summary>
		/// 計算斷行位置。
		/// </summary>
		/// <param name="brLine">點字串列。</param>
		/// <param name="cellsPerLine">每行最大允許幾方。</param>
		/// <param name="needHyphen">是否在斷行處附加一個連字號 '-'。</param>
		/// <returns>傳回可斷行的點字索引。</returns>
		private static int CalcBreakPoint(BrailleLine brLine, int cellsPerLine, 
			out bool needHyphen)
		{
			needHyphen = false;

            // 先根據每列最大方數取得要斷行的字元索引。
			int fixedBreakIndex = brLine.CalcBreakPoint(cellsPerLine);

			if (fixedBreakIndex >= brLine.WordCount)   // 無需斷行？
			{
				return fixedBreakIndex;
			}

			// 需斷行，根據點字規則調整斷行位置。

            int breakIndex = fixedBreakIndex;

			BrailleWord breakWord;

			// 必須和前一個字元一起斷至下一行的字元。亦即，只要剛好斷在這些字元，就要改成斷前一個字元。
			char[] joinLeftChars = {',', '.', '。', '、', '，', '；', '？', '！', '」', '』', '‧'};
			int loopCount = 0;

			while (breakIndex >= 0)
			{
				loopCount++;
				if (loopCount > 10000)
				{
					throw new Exception("偵測到無窮回圈於 BrailleProcessor.CalcBreakPoint()，請通知程式設計師!");
				}

				breakWord = brLine[breakIndex];

                if (breakWord.DontBreakLineHere)    // 如果之前已經設定這個字不能在此處斷行
                {
                    breakIndex--;
                    continue;
                }				

				if (breakWord.Text.IndexOfAny(joinLeftChars) >= 0)
				{
					// 前一個字要和此字元一起移到下一行。
					breakIndex--;
					continue;	// 繼續判斷前一個字元可否斷行。
				}

                if (breakWord.IsWhiteSpace) // 找到空白處，可斷開
                {
                    breakIndex++;   // 斷在空白右邊的字元。
                    break;
                }

                // 處理數字的斷字：連續數字不可斷開。
                if (breakWord.IsDigit)
                {
                    breakIndex--;
                    while (breakIndex >= 0)
                    {
                        if (!brLine[breakIndex].IsDigit)
                        {
                            break; 
                        }
                        breakIndex--;
                    }                    
                }
                else if (breakWord.IsLetter)    // 英文單字不斷字。
				{
					breakIndex--;
                    while (breakIndex >= 0)
                    {
                        if (!brLine[breakIndex].IsLetter)
                        {
                            break;
                        }
                        breakIndex--;
                    }
				}
                else if (breakWord.Text.Equals("_"))    // 連續底線不斷字。
                {
                    breakIndex--;
                    while (breakIndex >= 0)
                    {
                        if (!brLine[breakIndex].Text.Equals("_"))
                        {
                            break;
                        }
                        breakIndex--;
                    }
                }
                else
                {
                    break;
                }
            } // of while (breakIndex >= 0)

			if (breakIndex <= 0)
			{
				// 若此處 breakIndex < 0，表示找不到任何可斷行的位置；
				// 若此處 breakIndex == 0，表示可斷在第一個字元，那也沒有意義，因此也視為找不到斷行位置。

                //Trace.WriteLine("無法找到適當的斷行位置，使用每列最大方數斷行!");
                breakIndex = fixedBreakIndex;
			}

			// 注意!! 若 breakIndex 傳回 0 會導致呼叫的函式進入無窮迴圈!!

			return breakIndex;
		}

        /// <summary>
        /// 將指定的列與下一列相結合（下一列附加至本列）。
        /// </summary>
        /// <param name="brDoc">點字文件。</param>
        /// <param name="lineIndex">本列的列索引。</param>        
        public void JoinNextLine(BrailleDocument brDoc, int lineIndex)
        {
            BrailleLine brLine = brDoc.Lines[lineIndex];

            // 將下一列附加至本列，以結合成一列。
            int nextIndex = lineIndex + 1;
            if (nextIndex < brDoc.Lines.Count)
            {
                brLine.Append(brDoc.Lines[nextIndex]);
                brDoc.Lines.RemoveAt(nextIndex);
            }
        }

        #endregion


        #region Misc. methods.

        /// <summary>
        /// 從傳入的字串中取出原書頁碼。
        /// </summary>
        /// <param name="line"></param>
        /// <returns>若傳入的字串不是原書頁碼，則傳回空字串。否則傳回原書頁碼的文字（必須是字串，因為頁碼可能是羅馬數字）。</returns>
        public static string GetOrgPageNumber(string line)
        {
            if (String.IsNullOrEmpty(line)) return String.Empty;

            var pageNumberText = String.Empty;

            var endTagName = XmlTagHelper.GetEndTagName(ContextTagNames.OrgPageNumber);
            if (line.IndexOf(ContextTagNames.OrgPageNumber) >= 0 && line.IndexOf(endTagName) > 0)
            {
                pageNumberText = 
                    line.Replace(ContextTagNames.OrgPageNumber, String.Empty)
                        .Replace(endTagName, String.Empty)
                        .Trim();

            }
            else if (line.StartsWith(OrgPageNumberContextTag.LeadingUnderlines))
            {
                pageNumberText = line.Remove(0, OrgPageNumberContextTag.LeadingUnderlines.Length);
            }

            return pageNumberText;
        }

        #endregion
    }
}
