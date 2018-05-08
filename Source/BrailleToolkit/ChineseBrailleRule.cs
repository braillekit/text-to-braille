using System;
using System.Collections.Generic;
using BrailleToolkit.Tags;
using BrailleToolkit.Extensions;
using BrailleToolkit.Helpers;

namespace BrailleToolkit
{
    public static class ChineseBrailleRule
    {
        const string OpeningSymbols = "「『（【｛＂‘";    // 左開放符號。
        const string ClosingSymbols = "」』）】｝＂’";  // 右封閉符號。
    

        private static void AddOneSpaceAfterContext_UnlessNextWordIsPunctuation(
            string contextTagName, BrailleLine brLine)
        {
            var endTagName = XmlTagHelper.GetEndTagName(contextTagName);
            int wordIdx = 0;
            while (wordIdx < brLine.WordCount)
            {
                var brWord = brLine[wordIdx];
                wordIdx++;
                if (!brWord.IsContextTag || brWord.Text != contextTagName)
                {
                    continue; // 不是指定的起始標籤
                }

                // 持續往下移動，直到碰到此 context name 的結束標籤，然後取出下一個字。                
                BrailleWord nonContextNeighbor = null;
                while (wordIdx < brLine.WordCount)
                {
                    brWord = brLine[wordIdx];
                    wordIdx++;
                    if (wordIdx >= brLine.WordCount)
                        return;
                    if (brWord.IsContextTag && brWord.Text == endTagName)
                    {
                        nonContextNeighbor = brLine[wordIdx];
                        wordIdx++;
                        break;
                    }
                }
                if (nonContextNeighbor != null)
                {
                    if (!BrailleWordHelper.IsChinesePunctuation(nonContextNeighbor))
                    {
                        int insertPosition = wordIdx - 1;
                        if (!BrailleWord.IsBlank(brLine[insertPosition]))
                        {
                            brLine.Insert(insertPosition, BrailleWord.NewBlank());
                        }
                    }
                }
            }
        }

        public static void ApplySpecificNameAndBookNameRules(BrailleLine brLine)
        {
            AddOneSpaceAfterContext_UnlessNextWordIsPunctuation(ContextTagNames.SpecificName, brLine);
            AddOneSpaceAfterContext_UnlessNextWordIsPunctuation(ContextTagNames.BookName, brLine);
        }

        /// <summary>
        /// 根據標點符號規則調整一整行點字。
        /// </summary>
        /// <param name="brLine"></param>
        public static void ApplyPunctuationRules(BrailleLine brLine)
        {
            int wordIdx = 0;
            BrailleWord brWord;
            string text;

            while (wordIdx < brLine.WordCount)
            {
                brWord = brLine[wordIdx];

                if (brWord.IsContextTag || brWord.IsConvertedFromTag)
                {
                    wordIdx++;
                    continue;
                }
                if (brWord.Language != BrailleLanguage.Chinese)
                {
                    wordIdx++;
                    continue;
                }

                text = brWord.Text;
                if (text.Length == 3)
                {
                    // 判斷是否為特殊的編號（選擇題的答案編號）
                    if (text[0] == '【' && Char.IsDigit(text[1]) && text[2] == '】')
                    {
                        // 前後需各加一空方。
                        //wordIdx += EncloseBlankCells(brLine, wordIdx) + 1;
                        continue;
                    }
                }
                // 刪節號與破折號不可位於行首。
                if (text == "……" || text == "…" || text == "──" || text == "－－" || text == "─" || text == "－")
                {
                    brWord.DontBreakLineHere = true;
                    wordIdx++;
                    continue;
                }
                // 處理緊鄰的"（）"，在中間加一空方，使其變成"（ ）"。
                if ("（".Equals(text) && ((wordIdx + 1) < brLine.WordCount))
                {
                    BrailleWord brWord2 = brLine[wordIdx + 1];
                    if ("）".Equals(brWord2.Text))
                    {
                        // 兩個緊鄰的點字："（）"，要在左括弧前面加一空方，且括弧中間要加一空方，
                        // 否則組成的點字會跟 '○' 的點字一樣。
                        int offset = EncloseBlankCells(brLine, wordIdx);

                        // 設定 '（' 後面跟著的空方和 '）' 為不可斷行。
                        brLine[wordIdx + 1].DontBreakLineHere = true;
                        brLine[wordIdx + 2].DontBreakLineHere = true;

                        wordIdx += offset + 1;
                        continue;
                    }
                    if (BrailleWord.IsBlank(brWord2) && ((wordIdx + 2) < brLine.WordCount))
                    {
                        BrailleWord brWord3 = brLine[wordIdx + 2];
                        if ("）".Equals(brWord3.Text))
                        {
                            // 原本輸入的文字就已經在 '（' 和 '）' 中間加了空方。
                            // 設定 '（' 後面跟著的空方和 '）' 為不可斷行。
                            brLine[wordIdx + 1].DontBreakLineHere = true;
                            brLine[wordIdx + 2].DontBreakLineHere = true;

                            int offset = PrefixBlankCell(brLine, wordIdx);

                            wordIdx += offset + 2;
                            continue;
                        }
                    }
                    // 註：左括弧後面跟著的既不是空白也不是右括弧，視為括弧中有文字，
                    // 規則為：左括弧前方不空方，括弧中間不空方，右括弧後面要空方。
                    // 由於無論什麼情況，右括弧後面一定要加空方，因此由下面的程式碼處理，
                    // 這裡無須搜尋對應的右括弧並加空方。
                }

                switch (text)
                {
                    case "。":
                    case "」":  // 下引號的規則同句號。
                        wordIdx += ApplyPeriodRule(brLine, wordIdx);
                        break;
                    case "，":
                        wordIdx += ApplyCommaRule(brLine, wordIdx);
                        break;
                    case "：":
                        wordIdx += ApplyColonRule(brLine, wordIdx);
                        break;
                    case "！":  // 驚嘆號、問號的規則相同。
                    case "？":
                        wordIdx += EnsureOneSpaceFollowed_ExceptNextWordIsClosingSymbol(brLine, wordIdx);
                        break;
                    case "）":  // 後面要加一空方。
                    case "∘":      // 溫度符號
                    case "℃":
                        wordIdx += EnsureOneSpaceFollowed_ExceptNextWordIsPunctuation(brLine, wordIdx);
                        break;
                    case "※":  // 前後要加一空方。
                    case "◎":
                    case "○":
                    case "╳":
                    case "←":
                    case "→":
                    case "↑":
                    case "↓":
                    case "∴":
                    case "∵":
                    case "＝":
                    case "≠":
                    case "＜":
                    case "＞":
                    case "≦":
                    case "≧":
                    case "∠":
                    case "⊥":
                        wordIdx += EncloseBlankCells(brLine, wordIdx);
                        break;
                    case "√":   // 打勾符號，前後須加空方，後面接逗號時需特殊處理。
                        if ((wordIdx + 1) < brLine.WordCount)
                        {
                            string text2 = brLine[wordIdx + 1].Text;
                            if (text2.Equals("，") || text2.Equals(","))
                            {
                                // 打勾符號後面若跟著逗號，則可連書，且該逗號需用第 6 點。
                                brLine[wordIdx + 1].CellList.Clear();
                                brLine[wordIdx + 1].CellList.Add(BrailleCell.GetInstance(new int[] { 6 }));
                                wordIdx += PrefixBlankCell(brLine, wordIdx);
                            }
                            else
                            {
                                // 否則打勾符號前後
                                wordIdx += EncloseBlankCells(brLine, wordIdx);
                            }
                        }
                        else
                        {
                            wordIdx += EncloseBlankCells(brLine, wordIdx);
                        }
                        break;
                    case "▲":	// 刪除符號起始
                        wordIdx += PrefixBlankCell(brLine, wordIdx);
                        break;
                    case "▼":	// 刪除符號結束
                        wordIdx += PostfixBlankCell(brLine, wordIdx);
                        break;
                }

                wordIdx++;
            }
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
            index++;
            if (index >= brLine.WordCount)  // 如果已經到結尾或超過，就不加空方。
                return 0;

            int wordOffset = 0;
            if (!BrailleWord.IsBlank(brLine[index]))
            {
                brLine.Words.Insert(index, BrailleWord.NewBlank());
                wordOffset = 1;
            }
            return wordOffset;
        }

        /// <summary>
        /// 套用句號規則：
        /// 1.在同一點字行中，句號之後，須空一方，再點寫下文；
        ///   如在行末不夠點寫句號時，須將原句最末一字與句號移至次一行連書，
        ///   然後空方接寫下文；如句號恰在一行之最末一方，下文換行點寫時，次行之首無須空方。 
        /// 2.句號可與後引號（包括後單或雙引號）、刪節號、後括弧
        ///   （包括後圓括弧、後方括弧、後大括弧）、後夾註號連書，但不得單獨書於一行之首）。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int ApplyPeriodRule(BrailleLine brLine, int index)
        {
            index++;
            if (index >= brLine.WordCount)  // 如果已經到結尾或超過，就不加空方。
                return 0;

            int wordOffset = 0;
            BrailleWord brWord = brLine[index];
            if (!BrailleWord.IsBlank(brWord))  // 若原本已有空方，就不再多加。
            {
                // 句號可與標點符號連書而無須加空方。例外：句號後面接前引號 "「" 時需加空方。
                if (OpeningSymbols.IndexOf(brWord.Text) >= 0 ||
                    !BrailleWordHelper.IsChinesePunctuation(brWord))
                {
                    brLine.Words.Insert(index, BrailleWord.NewBlank());
                    wordOffset = 1;
                }
            }
            return wordOffset;
        }

        /// <summary>
        /// 如果下一個字元不是下引號、右括弧，則加一空方。
        /// 適用於：當前字元是問號、驚嘆號。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static int EnsureOneSpaceFollowed_ExceptNextWordIsClosingSymbol(BrailleLine brLine, int index)
        {
            return EnsureOneSpaceFollowed_UnlessNextWordIsExcepted(brLine, index, ClosingSymbols);
        }

        private static int EnsureOneSpaceFollowed_ExceptNextWordIsPunctuation(BrailleLine brLine, int index)
        {
            return EnsureOneSpaceFollowed_UnlessNextWordIsExcepted(brLine, index, BrailleGlobals.ChinesePunctuations);
        }

        /// <summary>
        /// 確保下一個字是空方，除非下一個字元是指定排除的字元。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="index"></param>
        /// <param name="exceptCharacters"></param>
        /// <returns></returns>
        private static int EnsureOneSpaceFollowed_UnlessNextWordIsExcepted(BrailleLine brLine, int index, string exceptedWords)
        {
            index++;
            if (index >= brLine.WordCount)  // 如果已經到結尾或超過，就不加空方。
                return 0;

            int wordOffset = 0;
            BrailleWord brWord = brLine[index];

            if (!BrailleWord.IsBlank(brWord))  // 若原本已有空方，就不再多加。
            {
                if (exceptedWords.IndexOf(brWord.Text) < 0)
                {
                    brLine.Words.Insert(index, BrailleWord.NewBlank());
                    wordOffset = 1;
                }
            }
            return wordOffset;
        }

        private static int ApplyCommaRule(BrailleLine brLine, int index)
        {
            int wordOffset = 0;
            return wordOffset;
        }

        /// <summary>
        /// 根據中文點字的冒號規則修正傳入的點字行。
        /// 規則：冒號之後若是 "我" 字，必須加一空方。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="index"></param>
        /// <returns>這次調整一共增加或刪除了幾個 word。</returns>
        private static int ApplyColonRule(BrailleLine brLine, int index)
        {
            int wordOffset = 0;

            index++;
            if (index < brLine.WordCount)
            {
                // 若下一個點字是我，則加一空方。
                if (brLine[index].Text == "我")
                {
                    brLine.Words.Insert(index, BrailleWord.NewBlank());
                    wordOffset = 1;	// 跳過空方
                }
            }
            return wordOffset;
        }

        /// <summary>
        /// 把多個連續全形空白刪到只剩一個。
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
                if (brWord.Text == "　")
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
        /// 處理包著數字的中括號，例如：【12】。
        /// </summary>
        /// <param name="brLine"></param>
        public static void ApplyBracketRule(BrailleLine brLine)
        {
            int wordIdx = 0;
            BrailleWord brWord;
            string text;
            int beginIdx = -1;
            int endIdx = -1;

            while (wordIdx < brLine.WordCount)
            {
                brWord = brLine[wordIdx];

                if (brWord.IsContextTag)
                {
                    wordIdx++;
                    continue;
                }

                text = brWord.Text;

                // 判斷是否為'【'
                if ("【".Equals(text))
                {
                    beginIdx = wordIdx;
                }
                else if ("】".Equals(text))
                {
                    if (beginIdx >= 0)
                    {
                        endIdx = wordIdx;
                        RemoveDigitCell(brLine, beginIdx + 1, endIdx - 1);
                    }
                }
                wordIdx++;
            }
        }

        /// <summary>
        /// 移除代表數字的點位。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="beginIdx"></param>
        /// <param name="endIdx"></param>
        public static void RemoveDigitCell(BrailleLine brLine, int beginIdx, int endIdx)
        {
            if (beginIdx < 0 || endIdx < 0 || beginIdx > endIdx)
                return;

            int wordIdx = beginIdx;
            BrailleWord brWord;
            string text;

            while (wordIdx < brLine.WordCount)
            {
                brWord = brLine[wordIdx];
                text = brWord.Text;

                if (text.Length > 0 && Char.IsDigit(text[0]) &&
                    brWord.Cells[0].Value == (byte)BrailleCellCode.Digit)
                {
                    brWord.Cells.RemoveAt(0);	// 移除小數點位.
                }

                wordIdx++;
            }
        }
    }
}
