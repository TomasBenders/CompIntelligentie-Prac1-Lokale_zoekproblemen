using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    internal class SudokuGrid
    {
        byte[,] gridValues = new byte[9,9];

        private byte[,] FromByteArray(byte[] values)
        {
            byte[,] result = gridValues;
            if (values.Length != result.Length)
                throw new ArgumentException("array of sudoku values is not as big as the sudoku grid");

            for (int x = 0; x < gridValues.GetLength(0); x++)
                for (int y = 0; y < gridValues.GetLength(1); y++)
                    gridValues[x, y] = values[x + y * gridValues.GetLength(0)];

            return result;
        }

        internal SudokuGrid(string values)
        {
            var sd = values.Split();
            var ssd = sd.Select(int.Parse).ToArray();
            gridValues = FromByteArray(values.Split().Select(byte.Parse).ToArray());
        }

        internal void PrintGrid(int left, int top)
        {
            for (int x = 0; x < gridValues.GetLength(0); x++)
                for (int y = 0; y < gridValues.GetLength(1); y++)
                {
                    Console.SetCursorPosition(left + x * 2, top + y);
                    string str = $"{gridValues[x, y]}{(x < gridValues.GetLength(0) - 1 ? (x % 3 == 2 ? "|" : " ") : "")}";
                    if (y % 3 == 2 && y != 8)
                        Utils.WriteUnderline(str);
                    else
                        Console.Write(str);
                }
        }

        internal static List<SudokuGrid> ParsePuzzels(string puzzels)
        {
            List<SudokuGrid> result = new();

            string[] lines = puzzels.Split("\n");
            for (int i = 1; i < lines.Length; i+=2)
                result.Add(new SudokuGrid(lines[i].Trim()));
            return result;
        }
    }
}
