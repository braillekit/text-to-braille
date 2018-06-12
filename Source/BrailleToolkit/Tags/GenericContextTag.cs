using System;
using System.Collections.Generic;
using System.Text;
using BrailleToolkit.Data;
using BrailleToolkit.Helpers;

namespace BrailleToolkit.Tags
{
    public class GenericContextTag : IContextTag
    {
        public string ConvertablePrefix { get; set; }   // 可轉換成點字的前導文字
        public string ConvertablePostfix { get; set; }  // 可轉換成點字的結尾文字

        public List<BrailleWord> PrefixBrailleWords { get; } = new List<BrailleWord>();
        public List<BrailleWord> PostfixBrailleWords { get; } = new List<BrailleWord>();


        public string TagName { get; protected set; }

        /// <summary>
        /// 傳回結束標籤名稱。
        /// </summary>
        public string EndTagName { get => TagName.Insert(1, "/"); }

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


        public GenericContextTag(string tagName,
            ContextLifetime lifeTime = ContextLifetime.BeforeFormatDoc,
            bool singleLine = false)
        {
            TagName = tagName;
            Lifetime = lifeTime;
            IsSingleLine = singleLine;
            Count = 0;

            ConvertablePrefix = String.Empty;
            ConvertablePostfix = String.Empty;
        }
    }
}

