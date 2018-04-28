using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Tags
{
    /// <summary>
    /// 語境標籤。用途主要是供 ContextTagManager 記錄某語境標籤在特定時間點的出現次數。
    /// </summary>
    public interface IContextTag
    {
        string ConvertablePrefix { get; set; }   // 可轉換成點字的前導文字
        string ConvertablePostfix { get; set; }  // 可轉換成點字的結尾文字

        List<BrailleWord> PrefixBrailleWords { get; }
        List<BrailleWord> PostfixBrailleWords { get; }

        string TagName { get; }

        /// <summary>
        /// 結束標籤名稱。
        /// </summary>
        string EndTagName { get; }

        ContextLifetime Lifetime { get; }

        /// <summary>
        /// 是否為單列標籤（整列只能有此標籤，不能包含其他標籤）
        /// </summary>
        bool SingleLine { get; }

        /// <summary>
        /// // 出現的次數
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 傳回此語境標籤目前是否在作用中。
        /// </summary>
        bool IsActive { get; }


        void Reset();


        void Enter();

        void Leave();
    }
}
