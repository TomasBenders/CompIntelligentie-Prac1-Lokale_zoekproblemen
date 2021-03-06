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
        internal int[] GetRow(int row)
        {
            int[] result = new int[GridSize];
            for (int x = 0; x < GridSize; x++)
                result[x] = GridValues[x, row];

            return result;
        }
        internal int[] GetColumn(int column)
        {
            int[] result = new int[GridSize];
            for (int y = 0; y < GridSize; y++)
                result[y] = GridValues[column, y];

            return result;
        }
        /// <summary>
        /// Gets a single subGrid of the sudoku
        /// </summary>
        /// <returns>A two-dimensional array containing the values within a box</returns>
        internal int[,] GetBox(int x, int y)
        {
            int[,] result = new int[boxSize, boxSize];

            for (int x1 = 0; x1 < boxSize; x1++)
                for (int y1 = 0; y1 < boxSize; y1++)
                    result[x1, y1] = GridValues[x * boxSize + x1, y * boxSize + y1];

            return result;

        }
        /// <summary>
        /// Gets a single subGrid of the sudoku
        /// </summary>
        /// <returns>A one-dimensional array containing the values within a box</returns>
        internal int[] GetBox1D(int x, int y)
        {
            int[] result = new int[boxSize * boxSize];

            for (int y1 = 0; y1 < boxSize; y1++)
                for (int x1 = 0; x1 < boxSize; x1++)
                    result[x1 + y1 * boxSize] = GridValues[x * boxSize + x1, y * boxSize + y1];

            return result;
        }
        /// <summary>
        /// The values within the grid. 
        /// A negative value indicates a value that cant be swapped.
        /// </summary>
        internal int[,] GridValues { get; init; }

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


        /// <summary>
        /// Converts a 1D int array to a 2D intArray as large as the sudoku grid
        /// </summary>
        private int[,] FromintArray(int[] values)
        {
            int[,] result = GridValues;
            if (values.Length != result.Length)
                throw new ArgumentException($"Array of sudoku values is not as big as the sudoku grid. " +
                    $"Values length: {values.Length}. Grid length: {result.Length}");

            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    GridValues[x, y] = values[x + y * GridSize];

            return result;
        }

        /// <summary>
        /// Creates a sudoku grid from int values
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
        internal SudokuGrid(int subGridSize, int[] values)
        {
            if (subGridSize % 3 != 0)
                throw new ArgumentException($"Size of grid must be divisable by 3. value was: {values.Length}");
            boxSize = subGridSize;
            GridValues = new int[GridSize, GridSize];
            HVRow = new int[GridSize];
            HVColumn = new int[GridSize];
            GridValues = FromintArray(values);
        }
        /// <summary>
        /// Creates a sudoku grid from a string of int values
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
            if (subGridSize % 3 != 0)
                throw new ArgumentException($"Size of grid must be divisable by 3. value was: {values.Length}");
            string[] split = values.Split();
            boxSize = subGridSize;
            GridValues = new int[GridSize, GridSize];
            HVRow = new int[GridSize];
            HVColumn = new int[GridSize];
            GridValues = FromintArray(split.Select(int.Parse).ToArray());
        }

        /// <summary>
        /// Prints the grid to the console and colors two values red, indicating a swap.
        /// </summary>
        internal void PrintSwap(int x1, int y1, int x2, int y2, bool absAll = true)
        {
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                {
                    Console.SetCursorPosition(posX + x * 2, posY + y);
                    bool shouldUnderLine = y % 3 == 2 && y != 8;
                    bool isCoordOfSwap = (x == x1 && y == y1) || (x == x2 && y == y2);
                    int value = absAll ? Math.Abs(GridValues[x, y]) : GridValues[x, y];
                    Utils.WriteUnderline(value.ToString(), shouldUnderLine, isCoordOfSwap ? ConsoleColor.Red : null);
                    Utils.WriteUnderline(x < GridSize - 1 ? (x % 3 == 2 ? "|" : " ") : "", shouldUnderLine, null);
                }
        }
        internal void PrintGrid(bool absAll = true)
        {
            PrintSwap(-1, -1, -1, -1, absAll);
        }
        internal void PrintScore()
        {
            Console.SetCursorPosition(posX, posY);
            Console.Write($"Score: {HeuristicValue}");
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

        /// <summary>
        /// a Method to fill the almost empty grid with all other values
        /// </summary>
        internal void GenFilledInGrid()
        {
            for (int x = 0; x < 3; x++)      //Every box is filled individualy
                for (int y = 0; y < 3; y++)
                {
                    List<int> notInBox = new List<int>();
                    for (int i = 1; i < (boxSize * boxSize + 1); i++) //Instatiate a list of values to add 
                        notInBox.Add(i);
                    for (int bx = 0; bx < boxSize; bx++)
                        for (int by = 0; by < boxSize; by++)          //For every value already in the grid that value is removed from the 'to add' list
                        {
                            if (GridValues[bx + x * 3, by + y * 3] != 0)
                            {
                                notInBox.Remove(GridValues[bx + x * 3, by + y * 3]);
                            }
                        }
                    for (int bx = 0; bx < boxSize; bx++)
                        for (int by = 0; by < boxSize; by++)          //Every empty cell in the box is assigned one of the values in the 'to add' list
                        {
                            if (GridValues[bx + x * 3, by + y * 3] == 0)
                            {
                                GridValues[bx + x * 3, by + y * 3] = notInBox[0];
                                notInBox.RemoveAt(0);
                            }
                            else
                                GridValues[bx + x * 3, by + y * 3] *= -1;
                        }
                }

        }

        /// <summary>
        /// The Heuristic function to update a row in the grid
        /// </summary>
        internal void CalcHeuristicRow(int row)
        {
            int r = 0;
            HashSet<int> seen = new();
            foreach (int value in GetRow(row))
                r += seen.Add(Math.Abs(value)) ? 0 : 1;
            HVRow[row] = r;
        }
        /// <summary>
        /// The Heuristic function to update a column in the grid
        /// </summary>
        internal void CalcHeuristicColumn(int column)
        {
            int r = 0;
            HashSet<int> seen = new();
            foreach (int value in GetColumn(column))
                r += seen.Add(Math.Abs(value)) ? 0 : 1;
            HVColumn[column] = r;
        }
        internal void CalcAllHeuristicCosts()
        {
            for (int i = 0; i < GridSize; i++)
            {
                CalcHeuristicRow(i);
                CalcHeuristicColumn(i);
            }
        }

        /// <summary>
        /// A function that swaps the indicated values
        /// </summary>
        /// <param name="x1"> The x coördinate for the first value to be swaped </param>
        /// <param name="y1"> The y coördinate for the first value to be swaped </param>
        /// <param name="x2"> The x coördinate for the second value to be swaped </param>
        /// <param name="y2"> The y coördinate for the second value to be swaped </param>
        /// <returns> A new sudokugrid with the indicated values swaped </returns>
        internal SudokuGrid Swap(int x1, int y1, int x2, int y2)
        {
            if (Math.Sign(GridValues[x1, y1]) == -1 || Math.Sign(GridValues[x2, y2]) == -1) //Fixed values should not be swaped
                throw new ArgumentException("bro das een gefixeerde");

            int[] values = new int[GridSize * GridSize];  //A new array of values (needed for the construction of a sudokugrid) is made
            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)        //All values of the old grid are copied into the new array
                {
                    if(x == x1 && y == y1)
                        values[x + y * GridSize] = GridValues[x2, y2];
                    else if (x == x2 && y == y2)                        //If a swaped value is detected , the other value is written instead
                        values[x + y * GridSize] = GridValues[x1, y1];
                    else
                        values[x + y * GridSize] = GridValues[x, y];
                }

            SudokuGrid swapped = new SudokuGrid(boxSize, values) { posX=posX, posY=posY}; //a new sudokugrid is made
            swapped.CalcAllHeuristicCosts();
            return swapped;
        }
        /// <summary>
        /// A function to check whether this SudokuGrid equals another
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        internal bool AreEqual(SudokuGrid b)
        {
            if(HeuristicValue != b.HeuristicValue)
                return false;

            if(GridSize != b.GridSize)
                return false;

            for (int x = 0; x < GridSize; x++)
                for (int y = 0; y < GridSize; y++)
                    if(GridValues[x,y] != b.GridValues[x,y])
                        return false;

            return true;
        }
    }
}
