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
        /// <summary>
        /// The width and height of a single subgrid/box
        /// </summary>
        internal int boxSize { get; private set; }
        /// <summary>
        /// The width and height of the entire grid
        /// </summary>
        internal int GridSize 
        { 
            get => boxSize * 3;
            set
            {
                if (value % 3 != 0)
                    throw new ArgumentException($"GridSize must be divisable by 3. value was: {value}");
                boxSize = value / 3;
            }
        }
        /// <summary>
        /// The x position this grid will be drawn at
        /// </summary>
        internal int posX = 0;
        /// <summary>
        /// The x position this grid will be drawn at
        /// </summary>
        internal int posY = 0;

        /// <summary>
        /// The values within the grid. 
        /// A negative value indicates a value that cant be swapped.
        /// </summary>
        byte[,] GridValues { get; init; }
        /// <summary>
        /// The Heuristic Value of each Row
        /// </summary>
        int[] HVRow { get; init; }
        /// <summary>
        /// The Heuristic Value of each Column
        /// </summary>
        int[] HVColumn { get; init; }
        /// <summary>
        /// The Heuristic Value of the entire grid. 
        /// This is used to determine how incorrect the grid is.
        /// 0 being optimal/correct, anything higher being more and more incorrect
        /// </summary>
        internal int HeuristicValue { get => HVColumn.Sum() + HVRow.Sum(); }

        internal byte[] GetRow(int row)
        {
            throw new NotImplementedException();
        }
        internal byte[] GetColumn(int column)
        {
            throw new NotImplementedException();
        }
        internal byte[,] GetBox(int x, int y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a 1D byte array to a 2D byteArray as large as the sudoku grid
        /// </summary>
        private byte[,] FromByteArray(byte[] values)
        {
            byte[,] result = GridValues;
            if (values.Length != result.Length)
                throw new ArgumentException($"Array of sudoku values is not as big as the sudoku grid. " +
                    $"Values length: {values.Length}. Grid length: {result.Length}");

            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    GridValues[x, y] = values[x + y * GridSize];

            return result;
        }

        /// <summary>
        /// Creates a sudoku grid from byte values
        /// </summary>
        /// <param name="subGridSize"> 
        /// The width and height of a box/subgrid within the suduko 
        /// </param>
        /// <param name="values"> 
        /// The values with which to create the grid from top to button, left to right 
        /// </param>
        /// <exception cref="ArgumentException"> 
        /// If the amount of given values does not form a valid sudoku grid 
        /// </exception>
        internal SudokuGrid(int subGridSize, byte[] values)
        {
            if (subGridSize % 3 != 0)
                throw new ArgumentException($"Size of grid must be divisable by 3. value was: {values.Length}");
            boxSize = subGridSize;
            GridValues = new byte[GridSize, GridSize];
            HVRow = HVColumn = new int[GridSize];
            GridValues = FromByteArray(values);
        }
        /// <summary>
        /// Creates a sudoku grid from a string of byte values
        /// </summary>
        /// <param name="subGridSize"> 
        /// The width and height of a box/subgrid within the suduko 
        /// </param>
        /// <param name="values"> 
        /// A string of values with which to create the grid from top to button, left to right 
        /// </param>
        /// <exception cref="ArgumentException"> 
        /// If the amount of given values does not form a valid sudoku grid 
        /// </exception>
        internal SudokuGrid(int subGridSize, string values)
        {
            string[] split = values.Split();
            boxSize = subGridSize;
            GridValues = new byte[GridSize, GridSize];
            HVRow = HVColumn = new int[GridSize];
            GridValues = FromByteArray(split.Select(byte.Parse).ToArray());
        }

        /// <summary>
        /// Prints the grid to the console and colors two values red, indicating a swap.
        /// </summary>
        internal void PrintBestSuccessor(int x1, int y1, int x2, int y2)
        {
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                {
                    Console.SetCursorPosition(posX + x * 2, posY + y);
                    bool shouldUnderLine = y % 3 == 2 && y != 8;
                    bool isCoordOfSwap = (x == x1 && y == y1) || (x == x2 && y == y2);
                    Utils.WriteUnderline(GridValues[x, y].ToString(), shouldUnderLine, isCoordOfSwap ? ConsoleColor.Red : null);
                    Utils.WriteUnderline(x < GridSize - 1 ? (x % 3 == 2 ? "|" : " ") : "", shouldUnderLine, null);
                }
        }
        internal void PrintGrid()
        {
            PrintBestSuccessor(-1, -1, -1, -1);
        }

        /// <summary>
        /// Parses a text file containing multiple unsolved sudokus
        /// </summary>
        /// <param name="puzzels"> The contents of the text file to parse</param>
        /// <returns> A list of sudokus parsed from the text file </returns>
        internal static List<SudokuGrid> ParsePuzzels(string puzzels)
        {
            List<SudokuGrid> result = new();

            string[] lines = puzzels.Split("\n");
            for (int i = 1; i < lines.Length; i+=2)
                result.Add(new SudokuGrid(3, lines[i].Trim()));
            return result;
        }
        internal void FillInZeroes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Heuristic function to update a row in the grid
        /// </summary>
        internal void CalcHeuristicRow(int row)
        {
            int r = 0;
            HashSet<byte> seen = new();
            foreach (byte value in GetRow(row))
                r += seen.Add(value) ? 0 : 1;
            HVRow[row] = r;
        }
        /// <summary>
        /// The Heuristic function to update a column in the grid
        /// </summary>
        internal void CalcHeuristicColumn(int column)
        {
            int r = 0;
            HashSet<byte> seen = new();
            foreach (byte value in GetColumn(column))
                r += seen.Add(value) ? 0 : 1;
            HVRow[column] = r;
        }
        internal bool IsCorrect()
        {
            var allHVRowAreZero = HVRow.All(o => o == 0);
            var allHVColumnAreZero = HVColumn.All(o => o == 0);
            if(allHVColumnAreZero&&allHVRowAreZero) return true;
            else return false;
        }
        internal SudokuGrid Swap(int x1, int y1, int x2, int y2)
        {
            throw new NotImplementedException();
        }
    }
}
