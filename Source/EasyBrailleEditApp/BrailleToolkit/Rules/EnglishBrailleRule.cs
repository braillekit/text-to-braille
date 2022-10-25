using System;
using BrailleToolkit.Data;
using BrailleToolkit.Tags;
using Huanlin.Common.Helpers;
using NChinese;

namespace BrailleToolkit.Rules
{
    internal sealed class EnglishBrailleRule
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

                if (brWord.IsContextTag || brWord.NoCapitalRule)
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
