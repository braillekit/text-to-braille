using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrailleToolkit.Helpers;
using BrailleToolkit.Tags;
using EasyBrailleEdit.Common;

namespace BrailleToolkit
{
    public static class BrailleDocumentFormatter
    {
        /// <summary>
        /// 編排指定的列。此函式會將指定的列斷行，並移除沒有用的空標籤。
        /// </summary>
        /// <param name="brDoc">點字文件。</param>
        /// <param name="lineIndex">欲重新編排的列索引。</param>
        /// <returns>傳回編排後的列數。</returns>
        public static int FormatLine(BrailleDocument brDoc, int lineIndex, ContextTagManager context)
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
        /// 對指定的 BrailleLine 格式化，包括斷行、移除沒有用的空標籤。
        /// </summary>
        /// <param name="brLine"></param>
        /// <param name="context"></param>
        /// <returns>可能是空的串列、經過格式化的單行串列，或者因斷行而產生的多行串列。</returns>
        public static List<BrailleLine> FormatLine(BrailleLine brLine, int cellsPerLine, ContextTagManager context)
        {
            var outputLines = new List<BrailleLine>();

            BrailleDocumentHelper.RemoveUselessWords(brLine, true, out _);

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
        /// 將一行點字串列斷成多行。
        /// </summary>
        /// <param name="brLine">來源點字串列。</param>
        /// <param name="cellsPerLine">每行最大方數。</param>
        /// <param name="context">情境物件。</param>
        /// <returns>斷行之後的多行串列。若為 null 表示無需斷行（指定的點字串列未超過每行最大方數）。</returns>
        public static List<BrailleLine> BreakLine(BrailleLine brLine, int cellsPerLine, ContextTagManager context)
        {
            int maxCellsInLine = cellsPerLine;
            if (context != null && context.IndentCount > 0) // 若目前位於縮排區塊中
            {
                // 每列最大方數要扣掉縮排數量，並於事後補縮排的空方。
                // NOTE: 必須在斷行之後才補縮排的空方!
                maxCellsInLine -= context.IndentCount;
            }

            // 若指定的點字串列未超過最大方數，則無須斷行，傳回 null。
            if (brLine.CellCount <= maxCellsInLine)
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

            // 計算折行之後的縮排格數。
            indents = CalcNewLineIndents(brLine);

            while (wordIndex < brLine.WordCount)
            {
                breakIndex = FindBreakPoint(brLine, maxCellsInLine, out needHyphen);

                newLine = brLine.ShallowCopy(wordIndex, breakIndex);   // 複製到新行。
                if (needHyphen) // 是否要附加連字號?
                {
                    newLine.Words.Add(new BrailleWord("-", BrailleCellCode.Hyphen));
                }

                // 如果是折下來的新行，就自動補上需要縮排的格數。
                if (isBroken)
                {
                    for (int i = 0; i < indents; i++)
                    {
                        newLine.Insert(0, BrailleWord.NewBlank());
                    }
                }

                brLine.RemoveRange(0, breakIndex);              // 從原始串列中刪除掉已經複製到新行的點字。
                wordIndex = 0;
                lines.Add(newLine);

                // 防錯：檢驗每個斷行後的 line 的方數是否超過每列最大方數。
                // 若超過，即表示之前的斷行處理有問題，須立即停止執行，否則錯誤會
                // 直到在雙視編輯的 Grid 顯示時才出現 index out of range，不易抓錯!
                System.Diagnostics.Debug.Assert(newLine.CellCount <= maxCellsInLine, "斷行錯誤! 超過每列最大方數!");

                // 被折行之後的第一個字需要再根據規則調整。
                EnglishBrailleRule.ApplyCapitalRule(brLine);    // 套用大寫規則。
                EnglishBrailleRule.ApplyDigitRule(brLine);		// 套用數字規則。

                isBroken = true;    // 已經至少折了一行
                maxCellsInLine = cellsPerLine - indents;  // 下一行開始就要自動縮排，共縮 indents 格。
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
        /// 尋找合適的斷行位置。
        /// </summary>
        /// <param name="brLine">點字串列。</param>
        /// <param name="cellsPerLine">每行最大允許幾方。</param>
        /// <param name="needHyphen">是否在斷行處附加一個連字號 '-'。</param>
        /// <returns>傳回可斷行的點字索引。</returns>
        private static int FindBreakPoint(BrailleLine brLine, int cellsPerLine,
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
            char[] joinLeftChars = { ',', '.', '。', '、', '，', '；', '？', '！', '」', '』', '‧' };
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
                    continue;   // 繼續判斷前一個字元可否斷行。
                }

                if (breakWord.IsWhiteSpace) // 找到空白處，可斷開
                {
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

            // 行首和行尾規則。
            if (breakIndex > 1)
            {
                // 私名號和書名號不可位於行尾。
                int newBreakIndex = breakIndex - 1;
                var lastWord = brLine[newBreakIndex];
                if (lastWord.IsConvertedFromTag
                    && (lastWord.Text == BrailleConst.DisplayText.SpecificName
                        || lastWord.Text == BrailleConst.DisplayText.BookName))
                {
                    while (newBreakIndex > 0)
                    {
                        var brWord = brLine[newBreakIndex];
                        if (brWord.IsContextTag
                            && (brWord.Text == ContextTagNames.SpecificName
                                || brWord.Text == ContextTagNames.BookName))
                        {
                            breakIndex = newBreakIndex;
                            break;
                        }
                        newBreakIndex--;
                    }
                }
            }

            // 注意!! 若 breakIndex 傳回 0 會導致呼叫的函式進入無窮迴圈!!

            return breakIndex;
        }

        private static void Indent(BrailleLine brLine, int indents)
        {
            for (int i = 0; i < indents; i++)
            {
                brLine.Insert(0, BrailleWord.NewBlank());
            }
        }

    }
}
