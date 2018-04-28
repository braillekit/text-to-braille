using System;
using System.Collections.Generic;
using System.Text;
using BrailleToolkit.Tags;
using BrailleToolkit.Helpers;

namespace BrailleToolkit
{
    /// <summary>
    /// 情境標籤管理員。
    /// </summary>
    public class ContextTagManager
    {
        private Dictionary<string, IContextTag> m_Tags;

        public ContextTagManager()
        {
            m_Tags = new Dictionary<string, IContextTag>();

            foreach (string tagName in ContextTagNames.Collection) 
            {
                m_Tags.Add(tagName, ContextTagFactory.CreateInstance(tagName));
            }

            ContextNames = String.Empty;
        }

        public string ContextNames { get; private set; }

        public void UpdateContextNames()
        {
            var sb = new StringBuilder();
            foreach (var tag in m_Tags.Values)
            {
                if (tag.IsActive)
                {
                    sb.Append($"{XmlTagHelper.RemoveBracket(tag.TagName)} ");
                }
}
            ContextNames = sb.ToString().TrimEnd();
        }

        /// <summary>
        /// 重設所有情境標籤的狀態。
        /// </summary>
        public void Reset()
        {
            foreach (var tag in m_Tags.Values)
            {
                tag.Reset();
            }
            ContextNames = String.Empty;
        }

        public void EnterContext(IContextTag tag)
        {
            tag.Enter();
            UpdateContextNames();
        }

        public void LeaveContext(IContextTag tag)
        {
            tag.Leave();
            UpdateContextNames();
        }

        /// <summary>
        /// 剖析傳入的字串，如果是以情境標籤開頭，就遞增或遞減
        /// 該情境標籤的計數值（根據它是起始還是結束標籤而定）。
        /// </summary>
        /// <param name="s">傳入的字串。</param>
        /// <param name="isBeginTag">傳回的旗號，若為 true，表示找到起始標籤，若為 false，則為結束標籤。</param>
        /// <returns>若有找到情境標籤，則傳回 ContextTag 物件，並設定 isBeginTag 輸出參數。</returns>
        public IContextTag Parse(string s, out bool isBeginTag)
        {
            IContextTag result = null;

            string beginTag;
            string endTag;

            isBeginTag = false;

            foreach (var tag in m_Tags.Values)
            {
                beginTag = tag.TagName;
                endTag = beginTag.Insert(1, "/");
                if (s.StartsWith(beginTag))
                {
                    // 進入此情境
                    EnterContext(tag);

                    result = tag;
                    isBeginTag = true;
                    break;
                }
                else if (s.StartsWith(endTag)) // 結束標籤
                {
                    // 離開此情境
                    LeaveContext(tag);

                    result = tag;
                    isBeginTag = false;
                    break;
                }
            }
            
            return result;
        }

        /// <summary>
        /// 剖析傳入的字串，如果是以指定的標籤開頭，就遞增或遞減
        /// 該情境標籤的計數值（根據它是起始還是結束標籤而定）。
        /// </summary>
        /// <param name="s">輸入字串。</param>
        /// <param name="tagName">指定的標籤。</param>
        /// <returns></returns>
        public IContextTag Parse(string s, string tagName)
        {
            IContextTag result = null;

            if (!m_Tags.ContainsKey(tagName))
                return null;

            var tag = m_Tags[tagName];

            string beginTag = tag.TagName;
            string endTag = tag.EndTagName;
            if (s.StartsWith(beginTag))
            {
                // 進入此情境
                EnterContext(tag);
                result = tag;
            }
            else if (s.StartsWith(endTag)) // 結束標籤
            {
                // 離開此情境
                LeaveContext(tag);
                result = tag;
            }
            return result;
        }

        /// <summary>
        /// 傳回指定的情境標籤目前是否在作用中。
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool IsActive(string tagName)
        {
            if (!m_Tags.ContainsKey(tagName))
            {
                return false;
            }
            return m_Tags[tagName].IsActive;
        }

        public bool IsOrgPageNumberActive()
        {
            return IsActive(ContextTagNames.OrgPageNumber);
        }

        /// <summary>
        /// 傳回縮排的數量。
        /// </summary>
        public int IndentCount
        {
            get
            {
                if (!m_Tags.ContainsKey(ContextTagNames.Indent))
                    return 0;
                return m_Tags[ContextTagNames.Indent].Count;
            }
        }
    }
}
