using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BrailleToolkit.Data;

namespace BrailleToolkit.Converters
{
    public abstract class WordConverter
    {
        internal abstract BrailleTableBase BrailleTable
        {
            get;
        }

        public abstract List<BrailleWord> Convert(Stack<char> charStack, ContextTagManager context);

        public virtual string Convert(string text)
        {
            return BrailleTable.Find(text);
        }

        /// <summary>
        /// 將傳入的字串（字元）轉換成點字。
        /// </summary>
        /// <param name="text">字串（一個字元）。</param>
        /// <returns>若指定的字串轉換成功，則傳回轉換之後的點字物件，否則傳回 null。</returns>
        protected virtual BrailleWord ConvertToBrailleWord(string text)
        {
            string brCode = BrailleTable.Find(text);
            if (!String.IsNullOrEmpty(brCode))
            {
                var brWord = new BrailleWord(text);
                brWord.AddCells(brCode);
                return brWord;
            }
            return null;
        }

        protected void EnsureOneSpaceFollowed_UnlessNextWordIsPunctuation(List<BrailleWord> wordList, string nextWord)
        {
            EnsureOneSpaceFollowed_UnlessNextWordIsExcepted(
                wordList, 
                nextWord, 
                BrailleGlobals.ChinesePunctuations + "<");

            // 註：'<' 是 context tag 的起始符號，碰到此符號時也不加空方。
        }

        protected void EnsureOneSpaceFollowed_UnlessNextWordIsExcepted(List<BrailleWord> wordList, string nextWord, string exceptedWords)
        {
            if (nextWord == " ") return; // 如果下一個字是空白，就不用多加了

            if (exceptedWords.IndexOf(nextWord) < 0)
            {
                wordList.Add(BrailleWord.NewBlank());
            }
        }

        protected void EnsureOneSpaceFollowed(List<BrailleWord> wordList, string nextWord)
        {
            if (nextWord == " ") return; // 如果下一個字是空白，就不用多加了

            wordList.Add(BrailleWord.NewBlank());
        }

        protected bool EnsureOneSpaceBetweenParentheses(List<BrailleWord> wordList, string currentWord, string nextWord)
        {
            if (currentWord == "（" && nextWord == "）")
            {
                wordList.Add(BrailleWord.NewBlank());
                return true;
            }
            if (currentWord == "(" && nextWord == ")")
            {
                wordList.Add(BrailleWord.NewBlank());
                return true;
            }
            return false;
        }
    }
}
