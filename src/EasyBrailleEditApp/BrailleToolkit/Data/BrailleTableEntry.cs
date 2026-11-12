namespace BrailleToolkit.Data
{
    /// <summary>
    /// 點字表中的單一項目。
    /// </summary>
    /// <param name="Text">對應的原始文字。</param>
    /// <param name="Dots">第一方點位字串。</param>
    /// <param name="Code">由 <paramref name="Dots"/> 轉換而得的十六進位點字碼。</param>
    /// <param name="Type">項目類型。</param>
    /// <param name="Dots2">第二方點位字串。</param>
    /// <param name="Code2">由 <paramref name="Dots2"/> 轉換而得的十六進位點字碼。</param>
    /// <param name="Joined">是否為結合韻。</param>
    /// <param name="Mono">是否為特殊單音。</param>
    /// <param name="Rule">額外規則名稱。</param>
    /// <param name="Description">描述文字。</param>
    public readonly record struct BrailleTableEntry(
        string Text,
        string Dots,
        string Code,
        string? Type = null,
        string? Dots2 = null,
        string? Code2 = null,
        bool Joined = false,
        bool Mono = false,
        string? Rule = null,
        string? Description = null);
}
