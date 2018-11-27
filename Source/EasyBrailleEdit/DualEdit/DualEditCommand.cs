using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyBrailleEdit.DualEdit
{
    internal static class DualEditCommand
    {
        public static class Names
        {
            public const string AddLine = "AddLine";     // 在下方插入一列
            public const string AppendWord = "AppendWord";  // 在列尾插入空方
            public const string BackDeleteWord = "BackDeleteWord";
            public const string BreakLine = "BreakLine";
            public const string SelectAll = "SelectAll";
            public const string CopyToClipboard = "CopyToClipboard";
            public const string CutToClipboard = "CutToClipboard";
            public const string DeleteLine = "DeleteLine";
            public const string DeleteWord = "DeleteWord";
            public const string EditWord = "EditWord";
            public const string FormatParagraph = "FormatParagraph";
            public const string InsertBlank = "InsertBlank";  // 插入空方                        
            public const string InsertLine = "InsertLine";  // 插入一列
            public const string InsertWord = "InsertWord";
            public const string InsertText = "InsertText";
            public const string InsertTable = "InsertTable";
            public const string PasteFromClipboard = "PasteFromClipboard";
            public const string RemoveDigitSymbol = "RemoveDigitSymbol";
        }
    }
}
