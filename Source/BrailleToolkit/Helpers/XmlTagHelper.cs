using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huanlin.Common;

namespace BrailleToolkit.Helpers
{
    public static class XmlTagHelper
    {
        public static bool IsBeginTag(string s)
        {
            if (String.IsNullOrEmpty(s))
                return false;
            return (s.StartsWith("<") && !s.StartsWith("</") && s.EndsWith(">"));
        }

        public static bool IsEndTag(string s)
        {
            if (String.IsNullOrEmpty(s))
                return false;
            return (s.StartsWith("</") && s.EndsWith(">"));
        }

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

        public static string RemoveBracket(string tagName)
        {
            if (String.IsNullOrEmpty(tagName))
            {
                return String.Empty;
            }
            return tagName.Replace("</", String.Empty).Replace("<", String.Empty).Replace(">", String.Empty);
        }
        
    }
}
