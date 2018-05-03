using System;
using BrailleToolkit.Data;
using Huanlin.Common.Helpers;
using NChinese;

namespace BrailleToolkit
{
    internal class EnglishBrailleRule
    {
        private EnglishBrailleRule() { }

        /// <summary>
        /// 根據英文點字的大寫規則調整一行點字。
        /// </summary>
        /// <param name="brLine">點字串列。</param>
        public static void ApplyCapitalRule(BrailleLine brLine)
        {
            char ch;
            int capitalCount = 0;   // 連續出現的大寫字母數量
            int firstCapitalIndex = -1;
            int wordIdx = 0;
            BrailleWord brWord;

            while (wordIdx < brLine.WordCount)
            {
                brWord = brLine[wordIdx];


                if (brWord.IsContextTag)
                {
                    wordIdx++;
                    continue;
                }
                if (brWord.Language == BrailleLanguage.English)
                {
                    ch = brWord.Text[0];
                    if (Char.IsLetter(ch) && !Char.IsLower(ch)) // 大寫字母？
                    {
                        capitalCount++; // 累計大寫字母數量

                        // 記住第一個大寫字母在串列中的位置，以便稍後插入點字大寫記號。
                        if (firstCapitalIndex < 0) 
                        {
                            firstCapitalIndex = wordIdx;
                        }
                        wordIdx++;
                        continue;
                    }
                }

                // 不是大寫字母。
                if (firstCapitalIndex >= 0)
                {
                    if (capitalCount >= 2)  // 全大寫
                    {
                        // 第一個大寫字母前面加兩個大寫點。
                        AddCapitalCell(brLine.Words[firstCapitalIndex], 2);
                    }
                    if (capitalCount == 1)   // 一個大寫字母 
                    {
                        AddCapitalCell(brLine.Words[firstCapitalIndex], 1);
                    }
                }
                firstCapitalIndex = -1;
                capitalCount = 0;

                wordIdx++;
            }

            // 處理最後一次連續大寫字母。
            if (firstCapitalIndex >= 0)
            {
                if (capitalCount >= 2)  // 全大寫
                {
                    // 第一個大寫字母前面加兩個大寫點。
                    AddCapitalCell(brLine.Words[firstCapitalIndex], 2);
                }
                if (capitalCount == 1)   // 一個大寫字母 
                {
                    AddCapitalCell(brLine.Words[firstCapitalIndex], 1);
                }
            }
        }

        /// <summary>
        /// 加入大寫點。
        /// </summary>
        /// <param name="brWord">點字物件</param>
        /// <param name="count">要加幾個大寫點。</param>
        private static void AddCapitalCell(BrailleWord brWord, int count)
        {
            int capCnt = 0;

            foreach (BrailleCell brCell in brWord.Cells)
            {
                if (brCell.Equals(BrailleCell.GetInstance(BrailleCellCode.Capital)))
                    capCnt++;
            }

            int neededCapCnt = count - capCnt;
            while (neededCapCnt > 0)
            {
                brWord.Cells.Insert(0, BrailleCell.Capital);
                neededCapCnt--;
            }
        }

		/// <summary>
		/// 套用數字規則：數字前面要加數字符號。
		/// 由於數字符號和數字不可分割，因此加在第一個數字的 Cells 裡面。
		/// </summary>
		/// <param name="brLine"></param>
		public static void ApplyDigitRule(BrailleLine brLine)
		{
			char ch;
			int digitCount = 0;   // 連續出現的數字數量
			int firstDigitIndex = -1;
			BrailleWord brWord;

            for (int i = 0; i < brLine.WordCount; i++)
            {
                brWord = brLine[i];

                if (brWord.IsContextTag) // 如果是情境標籤
                {
                    //context.Parse(brWord.Text); // 剖析情境標籤
                    continue;
                }
                if (brWord.NoDigitCell) // 如果預先指定不加數符（例如: 表示座標、次方時）
                {
                    continue;
                }

                ch = brWord.Text[0];
                if (Char.IsDigit(ch))
                {
                    digitCount++;

                    // 記住第一個數字字元在串列中的位置，以便稍後插入點字的數字記號。
                    if (firstDigitIndex < 0)
                    {
                        firstDigitIndex = i;
                    }
                    continue;
                }

                // 目前的字元不是數字
                if (digitCount > 0)     // 但之前的字元是數字?
                {
                    // 如果數字之後接著連字號 '-'、逗號 ',' 或 '+'
                    if (ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == ',' 
						|| ch == '.'	// 數字後面接著小數點，則小數點之後的數字不加數符。
						|| ch == '×' || ch == '÷') // NOTE: 這裡的 '×' 和 '÷' 分別是全型的乘號、除號，VS2005 編輯器無法正確顯示這兩個字元。
                    {
                        continue;   // 則繼續處理下一個字元。
                    }
                }

                // 目前的字元不是數字，且之前有出現過數字，則需在第一個數字前面加數符。
                if (firstDigitIndex >= 0)
                {
                    AddDigitSymbol(brLine, firstDigitIndex);
                }
                firstDigitIndex = -1;
                digitCount = 0;
            }

			// 處理最後一次連續數字。
			if (firstDigitIndex >= 0)
			{
                AddDigitSymbol(brLine, firstDigitIndex);
            }			
		}

        /// <summary>
        /// 在指定的索引處的 BrailleWord 物件中加入數符。
        /// 此函式會自動判斷是否有不需加入數符的例外狀況，例如：次方。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="index"></param>
        private static void AddDigitSymbol(BrailleLine brLine, int index)
        {
            BrailleCell digitCell = BrailleCell.GetInstance(BrailleCellCode.Digit);

            bool needDigitSymbol = true;

            if (index > 0)    // 先檢查前一個字元，是否為不需加數符的特例。
            {
                if (brLine.Words[index - 1].Text.Equals("^"))   // 次方。
                {
                    needDigitSymbol = false;
                }
            }
            if (needDigitSymbol)
            {
                BrailleWord firstDigitWord = brLine.Words[index];
                // 如果已經有加上數字記號就不再重複加。
                if (!firstDigitWord.Cells[0].Equals(digitCell))
                {
                    firstDigitWord.Cells.Insert(0, digitCell);
                }
            }
        }

		/// <summary>
		/// 把編號的數字修正成上位點。
		/// 注意：此函式會把點字串列中的 # 點字物件刪除。
		/// </summary>
		/// <param name="brLine"></param>
		public static void FixNumbers(BrailleLine brLine, EnglishBrailleTable brTable)
		{
			BrailleWord brWord;
			bool isNumberMode = false;
			string brCode;

			int index = 0;
			while (index < brLine.WordCount)
			{
				brWord = brLine[index];
				if (brWord.Text == "#")
				{
					isNumberMode = true;

                    brWord.IsContextTag = true; // 讓這個 word 不在雙視編輯視窗中顯示出來。
                    index++;
					continue;
				}
				if (Char.IsDigit(brWord.Text[0]))
				{
					if (isNumberMode)
					{
						// 把編號的數字改成上位點。
						brCode = brTable.FindDigit(brWord.Text, true);
						if (brWord.Cells.Count > 1)	// 第 0 個 cell 可能是數字記號。
						{
							brWord.Cells[1] = BrailleCell.GetInstance(brCode);
						}
						else
						{
                            brWord.Cells[0] = BrailleCell.GetInstance(brCode);
						}
					}
				}
				else
				{
					if (isNumberMode && brWord.Text != "." && brWord.Text != "-" && brWord.Text != ",")
					{
						isNumberMode = false;
					}
				}
				index++;
			}
		}

        /// <summary>
        /// 把多個連續空白刪到只剩一個。
        /// </summary>
        /// <param name="brLine"></param>
        public static void ShrinkSpaces(BrailleLine brLine)
        {
            int i = 0;
            int firstSpaceIndex = -1;
            int spaceCount = 0;
            BrailleWord brWord;
            while (i < brLine.WordCount)
            {
                brWord = brLine[i];
                if (brWord.Text == " ")
                {
                    spaceCount++;
                    if (firstSpaceIndex < 0)
                    {
                        firstSpaceIndex = i;
                    }
                }
                else
                {
                    // 不是全形空白，把之前取得的全形空白數量刪到剩一個。
                    if (firstSpaceIndex >= 0 && spaceCount > 1)
                    {
                        int cnt = spaceCount - 1;
                        brLine.Words.RemoveRange(firstSpaceIndex, cnt);
                        i = i - cnt;
                    }
                    firstSpaceIndex = -1;
                    spaceCount = 0;
                }
                i++;
            }

            // 去掉最後的連續空白
            if (firstSpaceIndex >= 0 && spaceCount > 1)
            {
                int cnt = spaceCount - 1;
                brLine.Words.RemoveRange(firstSpaceIndex, cnt);
            }
        }

        /// <summary>
        /// 補加必要的空白：在英數字母和中文字之間補上空白。
        /// </summary>
        /// <param name="brLine"></param>
        public static void AddSpaces(BrailleLine brLine)
        {
            int wordIdx = 0;
            BrailleWord brWord;
            BrailleWord lastBrWord;
            int wordOffset;

            while (wordIdx < brLine.WordCount)
            {
                brWord = brLine[wordIdx];

                if (brWord.IsContextTag)
                {
                    wordIdx++;
                    continue;
                }

                if (brWord.Text.Length < 1) 
                {
                    wordIdx++;
                    continue;
                }
                if (Char.IsWhiteSpace(brWord.Text[0]))
                {
                    wordIdx++;
                    continue;
                }
                if (wordIdx == 0)
                {
                    wordIdx++;
                    continue;                    
                }

                wordOffset = AddBlankForSpecialCharacters(brLine, wordIdx);
                if (wordOffset > 0)
                {
                    wordIdx += wordOffset;
                    continue;
                }

                // 取前一個字元。
                lastBrWord = brLine[wordIdx - 1];

                if (NeedSpace(lastBrWord, brWord))
                {
                    brLine.Words.Insert(wordIdx, BrailleWord.NewBlank());
                    wordIdx++;
                }
                wordIdx++;
            }
        }


        /// <summary>
        /// 根據前後鄰近的字元判斷中間是否需要加一個空方。
        /// </summary>
        /// <param name="lastWord">前一個字。</param>
        /// <param name="currWord">目前的字。</param>
        /// <returns></returns>
        private static bool NeedSpace(BrailleWord lastWord, BrailleWord currWord)
        {
            if (lastWord == null || currWord == null) 
            {
                throw new ArgumentException("傳入 NeedSpace() 的參數為 null!");
            }

			if (lastWord.IsEngPhonetic && currWord.IsEngPhonetic)
			{
				return false;
			}

            if (String.IsNullOrEmpty(lastWord.Text) || String.IsNullOrEmpty(currWord.Text))
                return false;

            if (lastWord.IsContextTag || lastWord.IsConvertedFromTag) // 如果前一個字是情境標籤，就不加空方
            {
                return false;
            }
            if (currWord.IsContextTag || currWord.IsConvertedFromTag) // 如果目前的字是情境標籤，就不加空方
            {
                return false;
            }

            if (lastWord.Text == BrailleConst.DisplayText.BrailleTranslatorNotePrefix) // 文字跟<點譯者註>的起始符號之間不加空方
            {
                return false;
            }

            char lastChar = lastWord.Text[lastWord.Text.Length-1];
            char currChar = currWord.Text[0];

            // 若前一個字元已經是空白，就不用處理。
            if (Char.IsWhiteSpace(lastChar))
            {
                return false;
            }

            if (lastChar == '□' || currChar == '□')  // 「滿點」符號的左右均不加空方。
            {
                return false;
            }

            if (CharHelper.IsAscii(currChar) && !CharHelper.IsAscii(lastChar))
            {
                // 目前的字元是 ASCII，但前一個字元不是（視為中文或其他雙位元組字元），需插入
                // 一個空白，除了一些例外，如：全型＋號前後若接數字，即視為數學式子，＋號前後都不空方。

                if (Char.IsDigit(currChar))
                {
                    switch (lastChar)
                    {
                        case '＋':
                        case '－':
                        case '×':   // 全型乘號 (有些編輯器無法正確顯示)
                        case '÷':   // 全型除號 (有些編輯器無法正確顯示)
                        case '（':
						case '【':	// // 用粗中刮弧把數字包起來時，代表題號，不用加空方.
                            return false;
                    }
                }
                // 注音符號後面接小數點時不加空方
                if (currChar == '.' && lastChar.ToString().IsZhuyinSymbol())
                {
                    return false;
                }

                return true;
            }

            if (!CharHelper.IsAscii(currChar) && CharHelper.IsAscii(lastChar))
            {
                // 目前的字元不是 ASCII，但前一個字元是，需插入一個空白，
                // 除了一些例外，例如：12℃ 的溫度符號前面不加空方。

                if (Char.IsDigit(lastChar))
                {
                    switch (currChar) 
                    {
                        case '。':   // 句號
                        case '，':   // 逗號
                        case '；':   // 分號
                        case '∘':    // 溫度符號
                        case '℃':
                        case '＋':
                        case '－':
                        case '×':   // 全型乘號 (有些編輯器無法正確顯示)
                        case '÷':   // 全型除號 (有些編輯器無法正確顯示)
                        case '）':
                        case '」':
                        case '』':
                        case '】':	// 用粗中刮弧把數字包起來時，代表題號，不用加空方.
                            return false;
                    }                    
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 處理那些需要在左右兩邊加空方的字元。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static int AddBlankForSpecialCharacters(BrailleLine brLine, int wordIdx)
        {
            int wordOffset = 0;
            BrailleWord currWord = brLine[wordIdx]; // 目前的字
            BrailleWord prevWord = null;            // 上一個字
            BrailleWord nextWord = null;            // 下一個字

            if ((wordIdx - 1) >= 0)
            {
                prevWord = brLine[wordIdx - 1];
            }
            if ((wordIdx + 1) < brLine.WordCount)
            {
                nextWord = brLine[wordIdx + 1];
            }

            switch (brLine[wordIdx].Text)
            {
                case "=":
					wordOffset += EncloseBlankCells(brLine, wordIdx);
					break;
// 注意!! '/' 符號不可以自動加空方，因為在表示分數時，'/' 不加空方。其他情況請編輯時自行加空方。 
//				case "/":
//                    wordOffset += EncloseBlankCells(brLine, wordIdx);
//                    break;
                case "%":
                    wordOffset += PostfixBlankCell(brLine, wordIdx);
                    if (prevWord != null && !Char.IsDigit(prevWord.Text[0]))
                    {
                        // % 符號的左邊如果不是數字，則加一空方。
                        wordOffset += PrefixBlankCell(brLine, wordIdx);
                    }
                    break;
                default:
                    break;
            }
            return wordOffset;
        }

        /// <summary>
        /// 在指定位置的左邊及右邊各加一個空方。若該位置已經有空方，則不做任何處理。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="index"></param>
        /// <returns>這次調整一共增加或刪除了幾個 word。</returns>
        private static int EncloseBlankCells(BrailleLine brLine, int index)
        {
            int wordOffset = 0;

            // NOTE: 一定要先加後面的空方，再插入前面的空方，否則 index 參數必須調整。
            wordOffset += PostfixBlankCell(brLine, index);
            wordOffset += PrefixBlankCell(brLine, index);
            return wordOffset;
        }

        /// <summary>
        /// 在指定的位置左邊附加一個空方，若該位置已經有空方，則不做任何處理。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="index"></param>
        /// <returns>這次調整一共增加或刪除了幾個 word。</returns>
        private static int PrefixBlankCell(BrailleLine brLine, int index)
        {
            int wordOffset = 0;
            if (index > 0 && !BrailleWord.IsBlank(brLine[index - 1]))
            {
                brLine.Words.Insert(index, BrailleWord.NewBlank());
                wordOffset = 1;
            }
            return wordOffset;
        }

        /// <summary>
        /// 在指定的位置右邊附加一個空方，若該位置已經有空方，則不做任何處理。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="index"></param>
        /// <returns>這次調整一共增加或刪除了幾個 word。</returns>
        private static int PostfixBlankCell(BrailleLine brLine, int index)
        {
            int wordOffset = 0;
            index++;
            if (index < brLine.WordCount) // 如果已經到結尾，就不加空方。
            {
                if (!BrailleWord.IsBlank(brLine[index]))
                {
                    brLine.Words.Insert(index, BrailleWord.NewBlank());
                    wordOffset = 1;
                }
            }
            return wordOffset;
        }
    }
}
