using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Tags
{
    /// <summary>
    /// 簡單標籤，會在轉點字過程中的預先處理階段就把標籤文字替換成對應的、可轉換成點字的文字或符號。
    /// </summary>
    internal static class SimpleTag
    {
        internal static class Names
        {
            public const string Unit1End = "<大單元結束>";
            public const string Unit2End = "<小單元結束>";
            public const string Unit3End = "<小題結束>";
        }

        public static Dictionary<string, string> Tags = new Dictionary<string, string>
        {
            { Names.Unit1End, new string ('ˍ', 20) },   // 大單元結束
            { Names.Unit2End, new string ('﹍', 20) },  // 小單元結束
            { Names.Unit3End, new string ('﹋', 20) }   // 小題結束
        };

        internal static bool IsSimpleTag(string tagName)
        {
            return Tags.ContainsKey(tagName);
        }

        internal static string GetTextValue(string tagName)
        {
            return Tags[tagName];
        }
    }

}
