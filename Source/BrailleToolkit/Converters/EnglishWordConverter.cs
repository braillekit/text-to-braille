using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Huanlin.Common.Helpers;
using BrailleToolkit.Data;
using BrailleToolkit.Tags;
using EasyBrailleEdit.Common;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// 處理英數字的點字轉換。
    /// </summary>
    public sealed class EnglishWordConverter : WordConverter
    {
        private EnglishBrailleTable m_Table;

        public EnglishWordConverter()
            : base()
        {
            m_Table = EnglishBrailleTable.GetInstance();
        }

        internal override BrailleTableBase BrailleTable
        {
            get { return m_Table; }
        }

        /// <summary>
        /// 從堆疊中讀取字元，把 ASCII 字元（半形的英數字）轉換成點字。
        /// </summary>
        /// <param name="charStack">輸入的 ASCII 字元堆疊。</param>
        /// <param name="context">情境物件。</param>
        /// <returns>傳回轉換後的點字物件串列，若串列為空串列，表示沒有成功轉換的字元。</returns>
        public override List<BrailleWord> Convert(Stack<char> charStack, ContextTagManager context)
        {
			if (charStack.Count < 1)
				throw new ArgumentException("傳入空的字元堆疊!");

            bool done = false;
            char ch;
			string text;
			bool isExtracted;	// 目前處理的字元是否已從堆疊中移出。
            BrailleWord brWord;
            List<BrailleWord> brWordList = null;

            while (!done && charStack.Count > 0)
            {
                ch = charStack.Peek();   // 只讀取但不從堆疊移走。
                isExtracted = false;
               
                // 如果在數學區塊中
                if (context.IsActive(ContextTagNames.Math))
                {
                    if (ch == '*' || ch == '.' || ch == ':')
                    {
						break;  // 以上符號須交給 MathConverter 處理。
                    }
                }

                // 如果在座標區塊中，'('、',' 和 ')' 必須交給 CoordinateConverter 處理。
                if (context.IsActive(ContextTagNames.Coordinate))
                {
                    if (ch == '(' || ch == ',' || ch == ')')
                    {
                        break;
                    }
                }

                // 如果是半形小於符號，先檢查是否為情境標籤。
                if (ch == '<')
                {
                    char[] charBuf = charStack.ToArray();
                    string s = new string(charBuf);
                    if (ContextTagNames.StartsWithContextTag(s))
                    {
                        break;  // 情境標籤必須交給 ContextTagConverter 處理。
                    }
                }

                text = ch.ToString();

                // 處理特殊字元。
                isExtracted = ProcessSpecialEntity(charStack, ref text);

                if (!isExtracted)
                {
                    // 處理刪節號。
                    if (ch == '.' && charStack.Count >= 3)
                    {
                        charStack.Pop();
                        char ch2 = charStack.Pop();
                        char ch3 = charStack.Pop();
                        if (ch2 == '.' && ch3 == '.')	// 連續三個點: 刪節號
                        {
                            isExtracted = true;
                            text = "...";
                        }
                        else
                        {
                            charStack.Push(ch3);
                            charStack.Push(ch2);
                            charStack.Push(ch);
                            isExtracted = false;
                        }
                    }
                    // Tab 字元視為一個空白。
                    if (ch == '\t')
                    {
                        text = " ";
                    }
                }

                brWord = InternalConvert(text, context);
                if (brWord == null)
                     break;

                brWord.ContextNames = context.ContextNames;

                if (!isExtracted)
                {
                    charStack.Pop();
                }
                 
                brWord.Language = BrailleLanguage.English;

                if (context.IsActive(ContextTagNames.Coordinate))   // 若處於座標情境標籤內
                {
                    // 不加數字點位。
                    brWord.NoDigitCell = true;
                }
				else if (":".Equals(text) && context.IsActive(ContextTagNames.Time))	// 處理時間的冒號。
				{
					// 在冒號的點字前面加上 456 點。
					BrailleCell cell = BrailleCell.GetInstance(new int[] { 4, 5, 6 });
					brWord.Cells.Insert(0, cell);	
				}

                if (brWordList == null)
                {
                    brWordList = new List<BrailleWord>();
                }
                brWordList.Add(brWord);
            }
			return brWordList;
        }
        
        /// <summary>
        /// 處理以 '&' 符號開頭的特殊字元，例如："&gt;" 和 "&lt;" 可以分別表示
        /// 半形的大於、小於符號。
        /// </summary>
        /// <param name="charStack">字元堆疊。</param>
        /// <param name="text">若傳回 true，則會設定此參數。</param>
        /// <returns>傳回 true 表示有碰到特殊字元，並已從 charStack 中取出。</returns>
        private bool ProcessSpecialEntity(Stack<char> charStack, ref string text)
        {
            if (charStack.Count < 4)
                return false;

            char ch = charStack.Peek();
            bool isExtracted = false;

            if (ch == '&')  // 特殊字元: &gt; 和 &lt;
            {
                // 讀下一個字元，若是相同符號，則可略過；若不同，則下次迴圈仍需處理。
                if (charStack.Count >= 4)
                {
                    charStack.Pop();
                    char ch2 = charStack.Pop();
                    char ch3 = charStack.Pop();
                    char ch4 = charStack.Pop();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(ch);
                    sb.Append(ch2);
                    sb.Append(ch3);
                    sb.Append(ch4);
                    if (sb.ToString().Equals("&gt;"))
                    {
                        text = ">";
                        isExtracted = true;
                    }
                    else if (sb.ToString().Equals("&lt;"))
                    {
                        text = "<";
                        isExtracted = true;
                    }
                    else
                    {
                        // 把之前取出的字元放回堆疊。
                        charStack.Push(ch4);
                        charStack.Push(ch3);
                        charStack.Push(ch2);
                        charStack.Push(ch);
                        isExtracted = false;
                    }
                }
            }
            return isExtracted;
        }

		/// <summary>
		/// 把英數字轉換成點字。
		/// </summary>
		/// <param name="text">一個英數字或英文標點符號。</param>
		/// <returns>若指定的字串是中文字且轉換成功，則傳回轉換之後的點字物件，否則傳回 null。</returns>
		private BrailleWord InternalConvert(string text, ContextTagManager context)
		{
			if (String.IsNullOrEmpty(text))
				return null;

			var brWord = new BrailleWord(text);

			string brCode = null;

			// 如果是刪節號
			if (text == "...")
			{
				brCode = m_Table.Find(text);
				brWord.AddCells(brCode);
				return brWord;
			}

			// 處理英文字母和數字。
			if (text.Length == 1)
			{
				char ch = text[0];
				if (CharHelper.IsAsciiLetter(ch))
				{
					brCode = m_Table.FindLetter(text);
					if (!String.IsNullOrEmpty(brCode))
					{
						brWord.AddCells(brCode);
						return brWord;
						// 註：大寫記號和連續大寫記號在完成一行之後才處理。
					}
					throw new Exception("找不到對應的點字: " + text);
				}
				if (CharHelper.IsAsciiDigit(ch))
				{
                    bool useUpperPositionDots = false;  // 一般數字取下位點。
                    if (AppGlobals.Config.Braille.UseUpperPositionForOrgPageNumber && context.IsOrgPageNumberActive())
                    {
                        // 原書頁碼使用上位點，且不加數符。
                        useUpperPositionDots = true;
                        brWord.NoDigitCell = true;
                    }                        

					brCode = m_Table.FindDigit(text, useUpperPositionDots);	
					if (!String.IsNullOrEmpty(brCode))
					{
						brWord.AddCells(brCode);
						return brWord;
					}
					throw new Exception("找不到對應的點字: " + text);
				}
			}

			// 處理編號。
			if (text == "#")
			{
				// # 沒有對應的點字碼，只是用它來代表編號數字的開始，
				// 以便後續處理編號用（將編號數字轉成上位點）。
				return brWord;
			}

			brCode = m_Table.Find(text);
			if (!String.IsNullOrEmpty(brCode))
			{
				brWord.AddCells(brCode);
				return brWord;
			}

			brWord = null;
			return null;
		}
    }
}
