using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using BrailleToolkit.Data;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// 字詞轉換器基底類別。
    /// </summary>
    public abstract class WordConverter
    {
        internal abstract BrailleTableBase? BrailleTable
        {
            get;
        }

        /// <summary>
        /// 將字元堆疊轉換為點字詞串列。
        /// </summary>
        /// <param name="charStack">字元堆疊。</param>
        /// <param name="context">情境標籤管理員。</param>
        /// <returns>點字詞串列。</returns>
        public abstract List<BrailleWord>? Convert(Stack<char> charStack, ContextTagManager context);

        /// <summary>
        /// 轉換指定的字串。
        /// </summary>
        /// <param name="text">欲轉換的字串。</param>
        /// <returns>轉換後的點字碼。</returns>
        public virtual string? Convert(string text)
        {
            return BrailleTable?.Find(text);
        }

        /// <summary>
        /// 將傳入的字串（字元）轉換成點字。
        /// </summary>
        /// <param name="text">字串（一個字元）。</param>
        /// <returns>若指定的字串轉換成功，則傳回轉換之後的點字物件，否則傳回 null。</returns>
        protected virtual BrailleWord? ConvertToBrailleWord(string text)
        {
            if (BrailleTable == null)
                return null;
            string? brCode = BrailleTable.Find(text);
            if (!String.IsNullOrEmpty(brCode))
            {
                var brWord = new BrailleWord(text);
                brWord.AddCells(brCode);
                return brWord;
            }
            return null;
        }

        /// <summary>
        /// 確保目前轉換的字詞後面有一個空方，除非下一個字是標點符號。
        /// </summary>
        /// <param name="wordList">目前的點字詞串列。</param>
        /// <param name="nextWord">下一個字。</param>
        protected void EnsureOneSpaceFollowed_UnlessNextWordIsPunctuation(List<BrailleWord> wordList, string nextWord)
        {
            EnsureOneSpaceFollowed_UnlessNextWordIsExcepted(
                wordList, 
                nextWord, 
                BrailleGlobals.ChinesePunctuations + "<");

            // 註：'<' 是 context tag 的起始符號，碰到此符號時也不加空方。
        }

        /// <summary>
        /// 確保目前轉換的字詞後面有一個空方，除非下一個字是排除的字元。
        /// </summary>
        /// <param name="wordList">目前的點字詞串列。</param>
        /// <param name="nextWord">下一個字。</param>
        /// <param name="exceptedWords">排除的字元集合。</param>
        protected void EnsureOneSpaceFollowed_UnlessNextWordIsExcepted(List<BrailleWord> wordList, string nextWord, string exceptedWords)
        {
            if (nextWord == " ") return; // 如果下一個字是空白，就不用多加了

            if (exceptedWords.IndexOf(nextWord) < 0)
            {
                wordList.Add(BrailleWord.NewBlank());
            }
        }

        /// <summary>
        /// 確保目前轉換的字詞後面有一個空方。
        /// </summary>
        /// <param name="wordList">目前的點字詞串列。</param>
        /// <param name="nextWord">下一個字。</param>
        protected void EnsureOneSpaceFollowed(List<BrailleWord> wordList, string nextWord)
        {
            if (nextWord == " ") return; // 如果下一個字是空白，就不用多加了

            wordList.Add(BrailleWord.NewBlank());
        }

        /// <summary>
        /// 確保括號之間有一個空方（若括號內無內容）。
        /// </summary>
        /// <param name="wordList">目前的點字詞串列。</param>
        /// <param name="currentWord">目前的字。</param>
        /// <param name="nextWord">下一個字。</param>
        /// <returns>若有加入空方則傳回 true。</returns>
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
