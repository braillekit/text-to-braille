using System;
using System.Collections.Generic;
using System.Text;
using BrailleToolkit.Data;

namespace BrailleToolkit
{
    /// <summary>
    /// �@���ܼơC
    /// </summary>
    public static class BrailleGlobals
    {
        public static string ChinesePunctuations = ChineseBrailleTable.GetInstance().GetAllPunctuations();
    }
}
