using System;
using System.Collections.Generic;
using BrailleToolkit;

namespace EasyBrailleEdit.Services
{
    /// <summary>
    /// 點字轉換結果
    /// </summary>
    public class BrailleConversionResult
    {
        /// <summary>
        /// 是否成功（無錯誤）
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 輸出的點字檔案路徑
        /// </summary>
        public string? OutputFilePath { get; set; }
        
        /// <summary>
        /// 是否有錯誤
        /// </summary>
        public bool HasError => !Success || InvalidChars.Count > 0;
        
        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
        
        /// <summary>
        /// 無法轉換的字元清單
        /// </summary>
        public List<CharPosition> InvalidChars { get; set; } = new();
    }
    
    /// <summary>
    /// 轉換進度資訊
    /// </summary>
    public class ConversionProgress
    {
        public int CurrentLine { get; set; }
        public string CurrentText { get; set; } = string.Empty;
        public int PercentComplete { get; set; }
    }
}
