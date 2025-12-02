namespace BrailleToolkit.Tags
{
    /// <summary>
    /// 語境標籤。用途主要是供 ContextTagManager 記錄某語境標籤在特定時間點的出現次數。
    /// </summary>
    public interface IContextTag
    {
        /// <summary>
        /// Gets or sets the text that can be converted to braille as a prefix.
        /// </summary>
        string ConvertablePrefix { get; set; }

        /// <summary>
        /// Gets or sets the text that can be converted to braille as a postfix.
        /// </summary>
        string ConvertablePostfix { get; set; }

        /// <summary>
        /// Gets the list of braille words to be inserted as a prefix.
        /// </summary>
        List<BrailleWord> PrefixBrailleWords { get; }

        /// <summary>
        /// Gets the list of braille words to be inserted as a postfix.
        /// </summary>
        List<BrailleWord> PostfixBrailleWords { get; }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        string TagName { get; }

        /// <summary>
        /// 結束標籤名稱。
        /// </summary>
        string EndTagName { get; }

        /// <summary>
        /// 是否在轉換點字的時候一併把標籤移除（作用等同於把標籤替換成特定文字與點字）。
        /// </summary>
        bool RemoveTagOnConversion { get; }

        /// <summary>
        /// Gets the lifetime of the context tag.
        /// </summary>
        ContextLifetime Lifetime { get; }

        /// <summary>
        /// 是否為單列標籤（整列只能有此標籤，不能包含其他標籤）
        /// </summary>
        bool IsSingleLine { get; }

        /// <summary>
        /// // 出現的次數
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 傳回此語境標籤目前是否在作用中。
        /// </summary>
        bool IsActive { get; }


        /// <summary>
        /// 重設此語境標籤的狀態。
        /// </summary>
        void Reset();


        /// <summary>
        /// 進入此語境標籤。
        /// </summary>
        void Enter();

        /// <summary>
        /// 離開此語境標籤。
        /// </summary>
        void Leave();
    }
}
