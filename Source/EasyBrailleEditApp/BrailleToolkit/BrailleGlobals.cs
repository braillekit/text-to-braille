using System;
using System.Collections.Generic;
using System.Text;
using BrailleToolkit.Data;

namespace BrailleToolkit
{
    /// <summary>
    /// 共用變數。
    /// </summary>
    public static class BrailleGlobals
    {
        /// <summary>
        /// 中文標點符號集合。
        /// </summary>
        public static string ChinesePunctuations = TwChineseBrailleTable.GetInstance().GetAllPunctuations();
    }
}
