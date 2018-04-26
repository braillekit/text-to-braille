﻿using Huanlin.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Extensions
{
    public static class BrailleWordHelper
    {

        public static bool IsChinesePunctuation(BrailleWord brWord)
        {
            if (brWord == null)
            {
                throw new ArgumentNullException("呼叫 IsChinesePunctuation() 時傳入了 null 參數", nameof(brWord));
            }
            return (BrailleGlobals.ChinesePunctuations.IndexOf(brWord.Text) >= 0);
        }

        public static string ToString(this List<BrailleWord> brWordList)
        {
            var sb = new StringBuilder();
            foreach (var brWord in brWordList)
            {            
                sb.Append("{");
                sb.Append($"\"{brWord.Text}\",");
                sb.Append($"\"{brWord.PhoneticCode}\",");
                sb.Append($"\"{brWord.CellList.ToString()}\"");
                sb.Append("},");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public static string ToDotNumberString(this List<BrailleWord> brWordList)
        {
            var sb = new StringBuilder();
            foreach (var brWord in brWordList)
            {
                sb.Append(brWord.ToPositionNumberString(useParenthesis: true));
            }
            return sb.ToString();
        }
    }
}
