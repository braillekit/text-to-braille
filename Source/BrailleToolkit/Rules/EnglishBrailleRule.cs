using System;
using BrailleToolkit.Data;
using BrailleToolkit.Tags;
using Huanlin.Common.Helpers;
using NChinese;

namespace BrailleToolkit.Rules
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
                    // 明確指定數符的時候自然也要加上數符。
                    if (brWord.Text == "#" && (i+1 < brLine.WordCount))
                    {
                        AddDigitSymbol(brLine, i+1);
                    }
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

                if (digitCount > 0)   // 之前的字元是數字？
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
                var firstDigitWord = brLine.Words[index];
                if (firstDigitWord.CellCount < 1)
                {
                    return; // 防錯.
                }
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

    }
}
