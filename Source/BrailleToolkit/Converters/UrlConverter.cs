using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Huanlin.Common.Helpers;
using BrailleToolkit.Data;
using BrailleToolkit.Tags;
using EasyBrailleEdit.Common;

namespace BrailleToolkit.Converters
{
    /// <summary>
    /// �B�z�^�Ʀr���I�r�ഫ�C
    /// </summary>
    public sealed class UrlConverter : WordConverter
    {
        private UrlBrailleTable m_Table;
        private BrailleProcessor _processor;

        public UrlConverter(BrailleProcessor processor)
        {
            m_Table = UrlBrailleTable.GetInstance();
            _processor = processor;
        }

        internal override BrailleTableBase BrailleTable
        {
            get { return m_Table; }
        }

        /// <summary>
        /// �q���|��Ū���r���A�� ASCII �r���]�b�Ϊ��^�Ʀr�^�ഫ���I�r�C
        /// </summary>
        /// <param name="charStack">��J�� ASCII �r�����|�C</param>
        /// <param name="context">���Ҫ���C</param>
        /// <returns>�Ǧ^�ഫ�᪺�I�r�����C�A�Y��C���Ŧ�C�A��ܨS�����\�ഫ���r���C</returns>
        public override List<BrailleWord> Convert(Stack<char> charStack, ContextTagManager context)
        {
			if (charStack.Count < 1)
				throw new ArgumentException("�ǤJ�Ū��r�����|!");

            bool done = false;
            char ch;
			string currentWord;
			bool isExtracted;	// �ثe�B�z���r���O�_�w�q���|�����X�C
            BrailleWord brWord;
            List<BrailleWord> brWordList = null;

            while (!done && charStack.Count > 0)
            {
                ch = charStack.Peek();   // �uŪ�������q���|�����C
                isExtracted = false;
               
                // �p�G�O�b�Τp��Ÿ��A���ˬd�O�_�����Ҽ��ҡC
                if (ch == '<')
                {
                    char[] charBuf = charStack.ToArray();
                    string s = new string(charBuf);
                    if (ContextTagNames.StartsWithContextTag(s))
                    {
                        break;  // ���Ҽ��ҥ����浹 ContextTagConverter �B�z�C
                    }
                }

                currentWord = ch.ToString();

                // �B�z�S��r���C
                isExtracted = ProcessSpecialEntity(charStack, ref currentWord);

                brWord = InternalConvert(currentWord, context);
                if (brWord == null)
                     break;

                brWord.ContextNames = context.ContextNames;

                if (!isExtracted)
                {
                    charStack.Pop();
                }
                 
                brWord.Language = BrailleLanguage.English;                
                brWord.NoDigitCell = true;   // ���[�Ʀr�I��C
                brWord.NoSpace = true;       // ���[�Ť�C
                brWord.NoCapitalRule = true; // ���M�έ^��r���j�g�W�h�C
            
                if (brWordList == null)
                {
                    brWordList = new List<BrailleWord>();
                }
                brWordList.Add(brWord);
            }
			return brWordList;
        }
        
        /// <summary>
        /// �B�z�H '&' �Ÿ��}�Y���S��r���A�Ҧp�G"&gt;" �M "&lt;" �i�H���O���
        /// �b�Ϊ��j��B�p��Ÿ��C
        /// </summary>
        /// <param name="charStack">�r�����|�C</param>
        /// <param name="text">�Y�Ǧ^ true�A�h�|�]�w���ѼơC</param>
        /// <returns>�Ǧ^ true ��ܦ��I��S��r���A�äw�q charStack �����X�C</returns>
        private bool ProcessSpecialEntity(Stack<char> charStack, ref string text)
        {
            if (charStack.Count < 4)
                return false;

            char ch = charStack.Peek();
            bool isExtracted = false;

            if (ch == '&')  // �S��r��: &gt; �M &lt;
            {
                // Ū�U�@�Ӧr���A�Y�O�ۦP�Ÿ��A�h�i���L�F�Y���P�A�h�U���j�餴�ݳB�z�C
                if (charStack.Count >= 4)
                {
                    charStack.Pop();
                    char ch2 = charStack.Pop();
                    char ch3 = charStack.Pop();
                    char ch4 = charStack.Pop();
                    StringBuilder sb = new StringBuilder();
                    sb.Append(ch);
                    sb.Append(ch2);
                    sb.Append(ch3);
                    sb.Append(ch4);
                    if (sb.ToString().Equals("&gt;"))
                    {
                        text = ">";
                        isExtracted = true;
                    }
                    else if (sb.ToString().Equals("&lt;"))
                    {
                        text = "<";
                        isExtracted = true;
                    }
                    else
                    {
                        // �⤧�e���X���r����^���|�C
                        charStack.Push(ch4);
                        charStack.Push(ch3);
                        charStack.Push(ch2);
                        charStack.Push(ch);
                        isExtracted = false;
                    }
                }
            }
            return isExtracted;
        }

		/// <summary>
		/// ��^�Ʀr�ഫ���I�r�C
		/// </summary>
		/// <param name="text">�@�ӭ^�Ʀr�έ^����I�Ÿ��C</param>
		/// <returns>�Y���w���r��O����r�B�ഫ���\�A�h�Ǧ^�ഫ���᪺�I�r����A�_�h�Ǧ^ null�C</returns>
		private BrailleWord InternalConvert(string text, ContextTagManager context)
		{
			if (String.IsNullOrEmpty(text))
				return null;

			var brWord = new BrailleWord(text);

			string brCode = null;

			// �B�z�^��r���M�Ʀr�C
			if (text.Length == 1)
			{
				char ch = text[0];
				if (CharHelper.IsAsciiLetter(ch))
				{
					brCode = m_Table.FindLetter(text);
					if (!String.IsNullOrEmpty(brCode))
					{
						brWord.AddCells(brCode);
						return brWord;
						// ���G�j�g�O���M�s��j�g�O���b�����@�椧��~�B�z�C
					}
					throw new Exception("�䤣��������I�r: " + text);
				}
				if (CharHelper.IsAsciiDigit(ch))
				{
                    bool useUpperPositionDots = false;  // �@��Ʀr���U���I�C
                    if (context.IsActive(ContextTagNames.UpperPosition))
                    {
                        useUpperPositionDots = true;
                    }

					brCode = m_Table.FindDigit(text, useUpperPositionDots);	
					if (!String.IsNullOrEmpty(brCode))
					{
						brWord.AddCells(brCode);
						return brWord;
					}
					throw new Exception("�䤣��������I�r: " + text);
				}
			}

			brCode = m_Table.Find(text);
			if (!String.IsNullOrEmpty(brCode))
			{
				brWord.AddCells(brCode);
				return brWord;
			}

			brWord = null;
			return null;
		}
    }
}
