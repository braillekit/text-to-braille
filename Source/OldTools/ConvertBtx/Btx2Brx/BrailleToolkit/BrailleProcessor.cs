using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BrailleToolkit.Tags;
using BrailleToolkit.Data;
using Huanlin.Common.Helpers;
using Huanlin.Common.Extensions;

namespace BrailleToolkit
{
    /// <summary>
    /// 此類別可用來將明眼字轉換成點字。可處理一個字、一行、或者多行。
    /// 錯誤處理機制：
    /// 1. 所有 exception 訊息會存入 ErrorMessage 屬性。
    /// 2. 所有無法轉換的字元會丟到 InvalidChars 屬性。
    /// </summary>
    public class BrailleProcessor
    {

    }
}
