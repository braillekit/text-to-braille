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
        /// �N�ǤJ���r��]�r���^�ഫ���I�r�C
        /// </summary>
        /// <param name="text">�r��]�@�Ӧr���^�C</param>
        /// <returns>�Y���w���r���ഫ���\�A�h�Ǧ^�ഫ���᪺�I�r����A�_�h�Ǧ^ null�C</returns>
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

            // ���G'<' �O context tag ���_�l�Ÿ��A�I�즹�Ÿ��ɤ]���[�Ť�C
        }

        protected void EnsureOneSpaceFollowed_UnlessNextWordIsExcepted(List<BrailleWord> wordList, string nextWord, string exceptedWords)
        {
            if (nextWord == " ") return; // �p�G�U�@�Ӧr�O�ťաA�N���Φh�[�F

            if (exceptedWords.IndexOf(nextWord) < 0)
            {
                wordList.Add(BrailleWord.NewBlank());
            }
        }

        protected void EnsureOneSpaceFollowed(List<BrailleWord> wordList, string nextWord)
        {
            if (nextWord == " ") return; // �p�G�U�@�Ӧr�O�ťաA�N���Φh�[�F

            wordList.Add(BrailleWord.NewBlank());
        }

        protected bool EnsureOneSpaceBetweenParentheses(List<BrailleWord> wordList, string currentWord, string nextWord)
        {
            if (currentWord == "�]" && nextWord == "�^")
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
