using System;
using System.Collections.Generic;
using System.Text;
using BrailleToolkit.Data;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    public class GenericContextTag : IContextTag
    {
        /// <summary>
        /// Gets or sets the text that can be converted to braille as a prefix.
        /// </summary>
        public string ConvertablePrefix { get; set; }

        /// <summary>
        /// Gets or sets the text that can be converted to braille as a postfix.
        /// </summary>
        public string ConvertablePostfix { get; set; }

        /// <summary>
        /// Gets the list of braille words to be inserted as a prefix.
        /// </summary>
        public List<BrailleWord> PrefixBrailleWords { get; } = new List<BrailleWord>();

        /// <summary>
        /// Gets the list of braille words to be inserted as a postfix.
        /// </summary>
        public List<BrailleWord> PostfixBrailleWords { get; } = new List<BrailleWord>();

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        public string TagName { get; protected set; }

        /// <summary>
        /// 傳回結束標籤名稱。
        /// </summary>
        public string EndTagName { get => TagName.Insert(1, "/"); }

        /// <summary>
        /// Gets the lifetime of the context tag.
        /// </summary>
        public ContextLifetime Lifetime { get; protected set; }

        /// <summary>
        /// 是否為單列標籤（整列只能有此標籤，不能包含其他標籤）
        /// </summary>
        public bool IsSingleLine { get; protected set; }


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

        /// <summary>
        /// Gets a value indicating whether to remove the tag upon conversion.
        /// </summary>
        public bool RemoveTagOnConversion { get; protected set; }

        public virtual void Reset()
        {
            Count = 0;
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

        protected GenericContextTag() { }


        /// <summary>
        /// Initializes a new instance of the <see cref="GenericContextTag"/> class.
        /// </summary>
        /// <param name="tagName">The name of the tag.</param>
        /// <param name="lifeTime">The lifetime of the tag.</param>
        /// <param name="removeTagOnConversion">Whether to remove the tag on conversion.</param>
        /// <param name="singleLine">Whether the tag is a single line tag.</param>
        public GenericContextTag(string tagName,
            ContextLifetime lifeTime = ContextLifetime.Persistent,
            bool removeTagOnConversion = false,
            bool singleLine = false)
        {
            TagName = tagName;
            Lifetime = lifeTime;
            IsSingleLine = singleLine;
            Count = 0;
            RemoveTagOnConversion = removeTagOnConversion;

            ConvertablePrefix = String.Empty;
            ConvertablePostfix = String.Empty;
        }
    }
}

