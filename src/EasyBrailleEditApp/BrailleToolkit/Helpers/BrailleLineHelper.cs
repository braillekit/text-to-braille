using System;
using System.Collections.Generic;

namespace BrailleToolkit.Helpers
{
    /// <summary>
    /// 提供操作 IBrailleLineView 的唯讀查詢方法，讓 BrailleLine 與 BrailleLineMaterialized 共用同一套邏輯。
    /// </summary>
    internal static class BrailleLineHelper
    {
        public static bool IsEmpty(IBrailleLineView line)
        {
            return line.WordCount < 1;
        }

        public static bool IsEmptyOrWhiteSpace(IBrailleLineView line)
        {
            for (int i = 0; i < line.WordCount; i++)
            {
                var word = line[i];
                if (!BrailleWord.IsBlank(word) && !BrailleWord.IsEmpty(word))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsBeginOfParagraph(IBrailleLineView line)
        {
            if (line.WordCount >= 2)
            {
                if (line[0].IsWhiteSpace && line[1].IsWhiteSpace)
                {
                    return true;
                }
            }
            return false;
        }

        public static int CalcBreakPoint(IBrailleLineView line, int cellsPerLine)
        {
            if (cellsPerLine < 4)
            {
                throw new ArgumentException("cellsPerLine 參數值不可小於 4。");
            }

            int cellCnt = 0;
            int index = 0;
            while (index < line.WordCount)
            {
                cellCnt += line[index].Cells.Count;
                if (cellCnt > cellsPerLine)
                {
                    break;
                }
                index++;
            }
            return index;
        }

        public static int GetFirstVisibleWordIndex(IBrailleLineView line)
        {
            for (int i = 0; i < line.WordCount; i++)
            {
                if (line[i].CellCount > 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static BrailleWord? GetFirstVisibleWord(IBrailleLineView line)
        {
            for (int i = 0; i < line.WordCount; i++)
            {
                if (line[i].CellCount > 0)
                {
                    return line[i];
                }
            }
            return null;
        }

        public static List<BrailleCell> GetBrailleCells(IBrailleLineView line)
        {
            var list = new List<BrailleCell>();
            for (int i = 0; i < line.WordCount; i++)
            {
                list.AddRange(line[i].Cells);
            }
            return list;
        }
    }
}
