using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrailleToolkit.Helpers
{
    public static class TextHelper
    {
        public static string GenerateTable(int rowCount, int columnCount, int cellsPerColumn)
        {
            var sb = new StringBuilder();
            var lineBuf = new StringBuilder();
            for (int row = 0; row < rowCount; row++)
            {
                for (int line = 0; line < 2; line++)
                {
                    lineBuf.Clear();
                    for (int col = 0; col < columnCount; col++)
                    {
                        lineBuf.Append(GetBeginSymbolOfColumn(row, line, col));
                        lineBuf.Append(AddMiddleSymbolOfRow(line));
                    }
                    lineBuf.Append(GetEndSymbolOfLine(row, line));
                    sb.AppendLine(lineBuf.ToString());
                }
            }

            // 最後一列
            lineBuf.Clear();
            for (int col = 0; col < columnCount; col++)
            {
                lineBuf.Append(GetBeginSymbolOfColumn(rowCount, 0, col));
                lineBuf.Append(AddMiddleSymbolOfRow(0));
            }
            lineBuf.Append(GetEndSymbolOfLine(rowCount, 0));
            sb.AppendLine(lineBuf.ToString());

            return sb.ToString();


            string GetBeginSymbolOfColumn(int row, int line, int col)
            {
                if (line % 2 == 0)
                {
                    if (col == 0)
                    {
                        if (row == 0) return "┌"; // 左上角
                        if (row == rowCount) return "└";   // 左下角
                        return "├";
                    }
                    else
                    {
                        if (row == 0) return "┬";
                        if (row == rowCount) return "┴";
                        return "┼";
                    }
                }
                return "│";
            }

            string AddMiddleSymbolOfRow(int line)
            {
                var middle = new StringBuilder();
                for (int cell = 0; cell < cellsPerColumn; cell++)
                {
                    if (line % 2 == 0)
                    {
                        middle.Append('─');
                    }
                    else
                    {
                        middle.Append('　');
                    }
                }
                return middle.ToString();
            }

            string GetEndSymbolOfLine(int row, int line)
            {
                if (line % 2 == 0)
                {
                    if (row == 0)
                        return "┐"; // 右上角
                    if (row == rowCount)
                        return "┘";   // 右下角
                    return "┤";
                }
                return "│";
            }
        }

    }
}
