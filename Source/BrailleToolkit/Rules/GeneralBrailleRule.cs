using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huanlin.Common.Helpers;
using NChinese;

namespace BrailleToolkit.Rules
{
    /// <summary>
    /// 此類別包含了無論中英文點字還是數學點字都適用的規則。
    /// </summary>
    public static class GeneralBrailleRule
    {
        // 底下這些符號的右邊都不用加空方。
        const string NoExtraSpaceAfterTheseCharacters = "「『“‘";
        const string NoExtraSpaceBeforeTheseCharacters = "」』”’";

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

            if (NoExtraSpaceAfterTheseCharacters.IndexOf(lastWord.Text) >= 0)
            {
                return false;
            }

            if (NoExtraSpaceBeforeTheseCharacters.IndexOf(currWord.Text) >= 0)
            {
                return false;
            }

            char lastChar = lastWord.Text[lastWord.Text.Length - 1];
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

            // 底下是更細的規則

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
                        case '【':   // // 用粗中刮弧把數字包起來時，代表題號，不用加空方.
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
