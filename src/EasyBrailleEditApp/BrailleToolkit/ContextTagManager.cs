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
    public sealed class ContextTagManager
    {
        private Dictionary<string, IContextTag> _tags;

        /// <summary>
        /// 初始化 ContextTagManager 類別的新執行個體。
        /// </summary>
        public ContextTagManager()
        {
            _tags = new Dictionary<string, IContextTag>();

            foreach (string tagName in ContextTagNames.Collection) 
            {
                _tags.Add(tagName, ContextTagFactory.CreateInstance(tagName));
            }

            ContextNames = String.Empty;
        }

        /// <summary>
        /// 取得目前作用中的情境名稱字串（以空白分隔）。
        /// </summary>
        public string ContextNames { get; private set; }

        /// <summary>
        /// 更新 ContextNames 屬性。
        /// </summary>
        public void UpdateContextNames()
        {
            var sb = new StringBuilder();
            foreach (var tag in _tags.Values)
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
            foreach (var tag in _tags.Values)
            {
                tag.Reset();
            }
            ContextNames = String.Empty;
        }

        /// <summary>
        /// 進入指定的情境。
        /// </summary>
        /// <param name="tag"></param>
        public void EnterContext(IContextTag tag)
        {
            tag.Enter();
            UpdateContextNames();
        }

        /// <summary>
        /// 離開指定的情境。
        /// </summary>
        /// <param name="tag"></param>
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
        public IContextTag? Parse(string s, out bool isBeginTag)
        {
            IContextTag? result = null;

            string beginTag;
            string endTag;

            isBeginTag = false;

            foreach (var tag in _tags.Values)
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
        public IContextTag? Parse(string s, string tagName)
        {
            IContextTag? result = null;

            if (!_tags.ContainsKey(tagName))
                return null;

            var tag = _tags[tagName];

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
        /// <param name="tagName">情境標籤名稱</param>
        /// <returns></returns>
        public bool IsActive(string tagName)
        {
            tagName = XmlTagHelper.GetBeginTagName(tagName);
            if (!_tags.ContainsKey(tagName))
            {
                return false;
            }
            return _tags[tagName].IsActive;
        }

        /// <summary>
        /// 數學情境是否在作用中。
        /// </summary>
        /// <returns></returns>
        public bool IsMathActive()
        {
            return IsActive(ContextTagNames.Math);
        }

        /// <summary>
        /// 原書頁碼情境是否在作用中。
        /// </summary>
        /// <returns></returns>
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
                if (!_tags.ContainsKey(ContextTagNames.Indent))
                    return 0;
                return _tags[ContextTagNames.Indent].Count;
            }
        }

        /// <summary>
        /// 取得所有情境標籤。
        /// </summary>
        public IReadOnlyDictionary<string, IContextTag> Tags => _tags;
    }
}
