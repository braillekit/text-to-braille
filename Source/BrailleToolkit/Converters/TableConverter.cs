using System;
using System.Collections.Generic;
using System.Text;
using Huanlin.Common.Helpers;
using BrailleToolkit.Data;

namespace BrailleToolkit.Converters
{
	public sealed class TableConverter : WordConverter
	{
		private TableBrailleTable m_Table;

		public TableConverter()
            : base()
        {
            m_Table = TableBrailleTable.GetInstance();
        }

        internal override BrailleTableBase BrailleTable
        {
            get { return m_Table; }
        }

		/// <summary>
		/// 轉換表格字元。
		/// </summary>
		/// <param name="charStack"></param>
		/// <param name="context"></param>
		/// <remarks>由於上、中、下的橫線在明眼字都是以相同的符號表示，可是點字卻不同，因此程式要特別處理：
		/// 每當碰到表格左邊的特定符號時，就設定橫線類型的旗號。</remarks>
		/// <returns></returns>
		public override List<BrailleWord> Convert(Stack<char> charStack, ContextTagManager context)
		{
			if (charStack.Count < 1)
				throw new ArgumentException("傳入空的字元堆疊!");

			bool done = false;
			char ch;
			string text;
			bool isExtracted;	// 目前處理的字元是否已從堆疊中移出。
			BrailleWord brWord;
			List<BrailleWord> brWordList = null;

			BarType barType = BarType.Middle;	// 橫線的種類			

			while (!done && charStack.Count > 0)
			{
				ch = charStack.Peek();   // 只讀取但不從堆疊移走。
				isExtracted = false;

				switch (ch) 
				{
					case '┌':
						barType = BarType.Top;
						break;
					case '└':
						barType = BarType.Bottom;
						break;
					case '├':
					case '│':	// 左直線
						barType = BarType.Middle;
						break;
					default:
						break;
				}

				text = ch.ToString();

				brWord = ConvertToBrailleWord(text);

				if (brWord == null)
					break;

                brWord.ContextNames = context.ContextNames;

                // 調整橫線的點字
                if ("─".Equals(brWord.Text))
				{
					string cellCode;

					if (barType == BarType.Top)
					{
						cellCode = m_Table["‾"];
						brWord.Cells[0] = BrailleCell.GetInstance(cellCode);
					}
					else if (barType == BarType.Bottom)
					{
						cellCode = m_Table["ˍ"];
						brWord.Cells[0] = BrailleCell.GetInstance(cellCode);
					}				
				}

				if (!isExtracted)
				{
					charStack.Pop();
				}

				if (brWordList == null)
				{
					brWordList = new List<BrailleWord>();
				}
				brWordList.Add(brWord);
			}
			return brWordList;
		}

		private enum BarType
		{
			Top,
			Middle,
			Bottom
		}
	}
}
