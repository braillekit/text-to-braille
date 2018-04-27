using System;
using System.Collections.Generic;
using System.Text;
using BrailleToolkit.Data;
using BrailleToolkit.Helpers;

namespace BrailleToolkit
{
    // 列舉常數 for 語境標籤的生命週期（何時要移除該語境標籤）
    public enum ContextLifetime
    {
        BeforeConvertion,   // 只存在於點字轉換動作之前。
        BeforeFormatDoc,    // 在點字轉換過程中，直到整份文件進行斷行之前。
        EndOfFormatDoc      // 在整份文件斷行完畢之後即消失。
    }

    public static class ContextTagNames
    {
        public const string Title = "<標題>";
        public const string Indent = "<縮排>";
        public const string Math = "<數學>";
        public const string Coordinate = "<座標>";
		public const string Fraction = "<分數>";
		public const string Table = "<表格>";
		public const string Time = "<時間>";
		public const string Delete = "<刪>";
		public const string Phonetic = "<音標>";
        public const string SpecificName = "<私名號>";
        public const string BookName = "<書名號>";
        public const string BrailleTranslatorNote = "<點譯者註>";

        // NOTE: 每當有變動時，必須同步修改 AllTagNames 的內容。
    }

    /// <summary>
    /// 語境標籤。用途主要是供 ContextTagManager 記錄某語境標籤在特定時間點的出現次數。
    /// 注意：此標籤物件皆為起始標籤。
    /// </summary>
    public abstract class ContextTag
    {
        public static HashSet<string> AllTagNames;
      

        public string ConvertablePrefix { get; set; }   // 可轉換成點字的前導文字
        public string ConvertablePostfix { get; set; }  // 可轉換成點字的結尾文字

        static ContextTag()
        {
            AllTagNames = new HashSet<string>()
            {
                ContextTagNames.Title,
                ContextTagNames.Indent,
                ContextTagNames.Indent,
                ContextTagNames.Math,
                ContextTagNames.Coordinate,
                ContextTagNames.Fraction,
                ContextTagNames.Table,
                ContextTagNames.Time,
                ContextTagNames.Delete,
                ContextTagNames.Phonetic,
                ContextTagNames.SpecificName,
                ContextTagNames.BookName,
                ContextTagNames.BrailleTranslatorNote

            };
        }

        /// <summary>
        /// 根據標籤名稱建立對應的 ContextTag 物件。
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static ContextTag CreateInstance(string tagName)
        {
            switch (tagName)
            {
                case ContextTagNames.Math:
                    return new MathContextTag();
                case ContextTagNames.Indent:
                    return new IndentContextTag();
                case ContextTagNames.Title:
                    return new TitleContextTag();
                case ContextTagNames.SpecificName:
                    return new GenericContextTag(tagName, ContextLifetime.BeforeFormatDoc)
                    {
                        ConvertablePrefix = BrailleConst.DisplayText.SpecificName
                    };
                case ContextTagNames.BookName:
                    return new GenericContextTag(tagName, ContextLifetime.BeforeFormatDoc)
                    {
                        ConvertablePrefix = BrailleConst.DisplayText.BookName
                    };
                case ContextTagNames.BrailleTranslatorNote:
                    return new GenericContextTag(tagName, ContextLifetime.BeforeFormatDoc)
                    {
                        ConvertablePrefix = BrailleConst.DisplayText.BrailleTranslatorNotePrefix,
                        ConvertablePostfix = BrailleConst.DisplayText.BrailleTranslatorNotePostfix
                    };
                default:
                    return new GenericContextTag(tagName, ContextLifetime.BeforeFormatDoc);
            }
            //throw new ArgumentException("呼叫 ContextTag.CreateInstance() 時傳入的參數 tagName 無效: " + tagName);
        }

        public static bool IsTitleTag(string s)
        {
            return XmlTagHelper.IsTag(s, ContextTagNames.Title);
        }

        /// <summary>
        /// 檢查傳入的字串是否是以語境標籤（含起始及結束標籤）開頭。
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool StartsWithContextTag(string s)
        {
            string beginTag;
            string endTag;
            bool found = false;

            foreach (string tagName in AllTagNames)
            {
                beginTag = tagName;
                endTag = beginTag.Insert(1, "/");
                if (s.StartsWith(beginTag) || s.StartsWith(endTag))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public ContextTag() 
        {
            Lifetime = ContextLifetime.BeforeFormatDoc;
            SingleLine = false;
            Count = 0;

            ConvertablePrefix = String.Empty;
            ConvertablePostfix = String.Empty;
        }

        public virtual void Enter()
        {
            Count++;
        }

        public virtual void Leave()
        {
            if (Count > 0)
            {
                Count--;
            }
        }

        public virtual void Reset()
        {
            Count = 0;
        }

        public virtual BrailleWord ToBrailleWord(bool isBeginTag)
        {
            var brWord = new BrailleWord();
            brWord.Text = TagName;
            brWord.IsContextTag = true;
            brWord.ContextTag = this;

            if (isBeginTag)
            {
                if (String.IsNullOrEmpty(ConvertablePrefix))
                {
                    brWord.Text = TagName;
                }
                else
                {
                    brWord.Text = ConvertablePrefix;
                    brWord.IsContextTag = false;    // 替代成文字之後就不是語境標籤了。
                    brWord.AddCell(
                        ChineseBrailleTable.GetInstance().GetPunctuationCode(ConvertablePrefix));
                }
            }
            else
            {
                if (String.IsNullOrEmpty(ConvertablePostfix))
                {
                    brWord.Text = EndTagName;
                }
                else
                {
                    brWord.Text = ConvertablePostfix;
                    brWord.IsContextTag = false;    // 替代成文字之後就不是語境標籤了。
                    brWord.AddCell(
                        ChineseBrailleTable.GetInstance().GetPunctuationCode(ConvertablePostfix));
                }
            }

            return brWord;
        }

        #region 屬性

        public string TagName { get; protected set; }

        /// <summary>
        /// 傳回結束標籤名稱。
        /// </summary>
        public string EndTagName
        {
            get { return TagName.Insert(1, "/"); }
        }

        public ContextLifetime Lifetime { get; protected set; }

        /// <summary>
        /// 是否為單列標籤（整列只能有此標籤，不能包含其他標籤）
        /// </summary>
        public bool SingleLine { get; protected set; }

        /// <summary>
        /// // 出現的次數
        /// </summary>
        public int Count { get; protected set; }

        /// <summary>
        /// 傳回此語境標籤目前是否在作用中。
        /// </summary>
        public bool IsActive
        {
            get
            {
                if (Count > 0)
                    return true;
                return false;
            }
        }

        #endregion
    }

    internal class GenericContextTag : ContextTag
    {
        public GenericContextTag(string tagName, ContextLifetime lifeTime, bool singleLine=false) : base()
        {
            TagName = tagName;
            Lifetime = lifeTime;
            SingleLine = singleLine;
        }
    }

    /// <summary>
    /// 縮排語境標籤。
    /// </summary>
    internal class IndentContextTag : ContextTag
    {
        public IndentContextTag() : base()
        {
            TagName = ContextTagNames.Indent;
            Lifetime = ContextLifetime.BeforeFormatDoc;
            SingleLine = false;
        }
    }

    /// <summary>
    /// 數學語境標籤。
    /// </summary>
    internal class MathContextTag : ContextTag
    {
        public MathContextTag() : base()
        {
            TagName = ContextTagNames.Math;
            Lifetime = ContextLifetime.BeforeFormatDoc;
            SingleLine = false;
        }
    }

    /// <summary>
    /// 標題語境標籤。用於指定頁標題。
    /// </summary>
    internal class TitleContextTag : ContextTag
    {
        public TitleContextTag() : base()
        {
            TagName = ContextTagNames.Title;
            Lifetime = ContextLifetime.EndOfFormatDoc;
            SingleLine = true;
        }
    }

	/// <summary>
	/// 分數語境標籤。
	/// </summary>
	internal class FractionContextTag : ContextTag
	{
		public FractionContextTag()
			: base()
		{
			TagName = ContextTagNames.Title;
			Lifetime = ContextLifetime.EndOfFormatDoc;
			SingleLine = true;
		}
	}
/*
    internal class SpecificNameContextTag : ContextTag
    {
        public SpecificNameContextTag() : base()
        {
            TagName = ContextTagNames.SpecificName;
            Lifetime = ContextLifetime.BeforeFormatDoc;
            SingleLine = false;
            ConvertablePrefix = "╴╴";
            ConvertablePostfix = String.Empty;
        }
    }

    internal class BookNameContextTag : ContextTag
    {
        public BookNameContextTag() : base()
        {
            TagName = ContextTagNames.BookName;
            Lifetime = ContextLifetime.BeforeFormatDoc;
            SingleLine = false;
            ConvertablePrefix = "﹏﹏";
            ConvertablePostfix = String.Empty;
        }
    }
    */
}
