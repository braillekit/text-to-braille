using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huanlin.Common;

namespace BrailleToolkit.Helpers
{
    /// <summary>
    /// 提供處理 XML/HTML 標籤的靜態輔助函式。
    /// </summary>
    public static class XmlTagHelper
    {
        /// <summary>
        /// 判斷指定的字串是否為起始標籤。
        /// </summary>
        /// <param name="s">要檢查的字串。</param>
        /// <returns>如果是起始標籤，則為 true；否則為 false。</returns>
        public static bool IsBeginTag(string s)
        {
            if (String.IsNullOrEmpty(s))
                return false;
            return (s.StartsWith("<") && !s.StartsWith("</") && s.EndsWith(">"));
        }

        /// <summary>
        /// 判斷指定的字串是否為結束標籤。
        /// </summary>
        /// <param name="s">要檢查的字串。</param>
        /// <returns>如果是結束標籤，則為 true；否則為 false。</returns>
        public static bool IsEndTag(string s)
        {
            if (String.IsNullOrEmpty(s))
                return false;
            return (s.StartsWith("</") && s.EndsWith(">"));
        }

        /// <summary>
        /// 判斷指定的字串是否為空元素標籤。
        /// </summary>
        /// <param name="s">要檢查的字串。</param>
        /// <returns>如果是空元素標籤，則為 true；否則為 false。</returns>
        public static bool IsEmptyTag(string s)
        {
            if (String.IsNullOrEmpty(s))
                return false;
            return (s.StartsWith("<") && s.EndsWith("/>"));
        }

        /// <summary>
        /// 比對傳入的字串是否為特定的標籤。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static bool IsTag(string value, string tagName)
        {
            if (String.IsNullOrEmpty(value) || String.IsNullOrEmpty(tagName))
                return false;
            if (value.Equals(tagName))
                return true;
            tagName = tagName.Insert(1, "/");   // 結束標籤
            if (value.Equals(tagName))
                return true;
            return false;
        }

        /// <summary>
        /// 取得起始標籤字串。
        /// </summary>
        /// <param name="tagName">標籤名稱。</param>
        /// <returns>包含角括號的起始標籤字串。</returns>
        public static string GetBeginTagName(string tagName)
        {
            if (String.IsNullOrWhiteSpace(tagName))
                throw new ArgumentException($"{nameof(tagName)} 不可為空字串!");

            if (IsBeginTag(tagName))
            {
                return tagName;
            }
            if (IsEndTag(tagName))
            {
                return tagName.Remove(1, 1);
            }
            return $"<{tagName}>";
        }

        /// <summary>
        /// 傳回結束標籤字串。
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public static string GetEndTagName(string tagName)
        {
            if (String.IsNullOrWhiteSpace(tagName))
                throw new ArgumentException($"{nameof(tagName)} 不可為空字串!");
            if (IsEndTag(tagName))
            {
                return tagName;
            }
            if (IsBeginTag(tagName))
            {
                return tagName.Insert(1, "/");
            }
            return $"</{tagName}>";          
        }

        /// <summary>
        /// 從標籤字串中移除角括號和斜線。
        /// </summary>
        /// <param name="tagName">要處理的標籤字串。</param>
        /// <returns>不含角括號和斜線的標籤名稱。</returns>
        public static string RemoveBracket(string tagName)
        {
            if (String.IsNullOrEmpty(tagName))
            {
                return String.Empty;
            }
            return tagName.Replace("</", String.Empty).Replace("<", String.Empty).Replace(">", String.Empty);
        }
        
        /// <summary>
        /// 使用指定的標籤將文字包圍起來。
        /// </summary>
        /// <param name="text">要包圍的文字。</param>
        /// <param name="tagName">標籤名稱。</param>
        /// <returns>被標籤包圍的完整字串。</returns>
        public static string EncloseWithTag(string text, string tagName)
        {
            return GetBeginTagName(tagName) + text + GetEndTagName(tagName);
        }

    }
}
