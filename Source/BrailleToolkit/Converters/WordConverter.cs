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
            BrailleWord brWord = new BrailleWord(text);

            string brCode;

            brCode = BrailleTable.Find(text);
            if (!String.IsNullOrEmpty(brCode))
            {
                brWord.AddCell(brCode);
                return brWord;
            }

            brWord.Clear();
            brWord = null;
            return null;
        }

    }
}
