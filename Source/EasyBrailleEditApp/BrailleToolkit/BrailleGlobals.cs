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
        public static string ChinesePunctuations = ChineseBrailleTable.GetInstance().GetAllPunctuations();
    }
}
