﻿using System;
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
        int[] hVRow = new int[9];
        int[] hVColumn = new int[9];

        internal byte[] getRow(int row)
        {
            throw new NotImplementedException();
        }
        internal byte[] getColumn(int column)
        {
            throw new NotImplementedException();
        }
        internal byte[] getBox(int x, int y)
        {
            throw new NotImplementedException();
        }

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

        internal void PrintBestSuccessor(int left, int top, int x1, int y1, int x2, int y2)
        {
            for (int x = 0; x < gridValues.GetLength(0); x++)
                for (int y = 0; y < gridValues.GetLength(1); y++)
                {
                    Console.SetCursorPosition(left + x * 2, top + y);
                    bool shouldUnderLine = y % 3 == 2 && y != 8;
                    Utils.WriteUnderline(gridValues[x, y].ToString(), shouldUnderLine, 
                        (x == x1 && y == y1) || (x == x2 && y == y2) ? ConsoleColor.Red : null);
                    Utils.WriteUnderline(x < gridValues.GetLength(0) - 1 ? (x % 3 == 2 ? "|" : " ") : "", shouldUnderLine, null);
                }
        }
        internal void PrintGrid(int left, int top)
        {
            PrintBestSuccessor(left, top, -1, -1, -1, -1);
        }

        internal static List<SudokuGrid> ParsePuzzels(string puzzels)
        {
            List<SudokuGrid> result = new();

            string[] lines = puzzels.Split("\n");
            for (int i = 1; i < lines.Length; i+=2)
                result.Add(new SudokuGrid(lines[i].Trim()));
            return result;
        }
        internal void FillInZeroes()
        {
            throw new NotImplementedException();
        }

        internal int CalcHeuristicRow(int row)
        {
            int r = 0;
            HashSet<byte> seen = new();
            for (int x = 0; x < gridValues.GetLength(0); x++)
                r += seen.Add(gridValues[x, row]) ? 0 : 1;
            hVRow[row] = r;
            return r;
        }
        internal int CalcHeuristicColumn(int column)
        {
            int r = 0;
            HashSet<byte> seen = new();
            for (int y = 0; y < gridValues.GetLength(1); y++)
                r += seen.Add(gridValues[column, y]) ? 0 : 1;
            hVColumn[column] = r;
            return r;
        }
        internal int GetHeuristic()
        {
            return hVColumn.Sum() + hVRow.Sum();
        }
        internal bool IsCorrect()
        {
            throw new NotImplementedException();
        }
        internal SudokuGrid Swap(int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
        }
    }
}
