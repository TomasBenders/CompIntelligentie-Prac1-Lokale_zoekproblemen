using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    static internal class CBT
    {
        static int posX; // where to print
        static int posY; // where to print
        static int statesCount = 0; // amount of generated states
        static List<TimeSpan> times = new(); // list of recorded times

        static internal void resetTimes()
        {
            times = new();
        }
        static internal TimeSpan getAvarageTime()
        {
            TimeSpan timeTaken = new(times.Sum(x => x.Ticks));
            timeTaken /= Utils.howManyTimesRun;
            return timeTaken;
        }

        internal class Cell // Data structure for a variable in the Constraint Satisfaction Problem
        {
            internal int value;
            internal List<int> domain = new();

            internal Cell(int value = 0)
            {
                this.value = value;
            }
        }

        static internal SudokuGrid CBTSolver(SudokuGrid sudokuGrid, out bool solved, out int statesGenerated)
        {
            // Set position for printing
            posX = sudokuGrid.posX;
            posY = sudokuGrid.posY;
            statesCount = 0;

            // Convert sudokuGrid to variables
            Cell[,] variables = new Cell[sudokuGrid.GridSize, sudokuGrid.GridSize];
            for (int x2 = 0; x2 < sudokuGrid.GridSize; x2++)
                for (int y2 = 0; y2 < sudokuGrid.GridSize; y2++)
                    variables[x2, y2] = new(-sudokuGrid.GridValues[x2, y2]);

            PrintVariables(variables);

            // Start measurements
            statesCount++;
            Stopwatch timer = new();
            timer.Start();

            //Apply CBT Algorithm
            solved = ChronologicalBackTracking(ref variables);

            // Stop measurements
            timer.Stop();
            statesGenerated = statesCount;
            times.Add(timer.Elapsed);

            // Convert variables to value array
            int[] values = new int[sudokuGrid.GridSize * sudokuGrid.GridSize];
            for (int x2 = 0; x2 < sudokuGrid.GridSize; x2++)
                for (int y2 = 0; y2 < sudokuGrid.GridSize; y2++)
                    values[x2 + y2 * sudokuGrid.GridSize] = variables[x2, y2].value;

            // Return new sudoGrid with value array
            return new SudokuGrid(sudokuGrid.boxSize, values) { posX = posX, posY = posY };
        }

        static internal bool ChronologicalBackTracking(ref Cell[,] variables)
        {
            NodeConsistent(ref variables); // first apply node consistency
            return ChronologicalBackTracking(ref variables, 0, 0); // then start traversing and generating the tree
        }
        static internal bool ChronologicalBackTracking(ref Cell[,] variables, int x, int y)
        {
            if (y >= variables.GetLength(1)) // all cells done
                return true;

            if (Math.Sign(variables[x, y].value) == -1) // fixated cell
                return ChronologicalBackTracking(ref variables, (x + 1) % variables.GetLength(1), y + (x + 1) / variables.GetLength(1));

            for (int i = 0; i < variables[x, y].domain.Count; i++) // loop over domain options
            {
                variables[x, y].value = variables[x, y].domain[i]; // set cell, counts as a new state or node in the tree
                statesCount++;
                PrintVariables(variables, x, y);

                if (!DynamicForwardChecking(ref variables, x, y) ||
                    !ChronologicalBackTracking(ref variables, (x + 1) % variables.GetLength(1), y + (x + 1) / variables.GetLength(1)))
                { // if this child is not the solution, revert changes
                    variables[x, y].value = 0; // revert current cell
                    PrintVariables(variables, x, y, color: ConsoleColor.Blue);

                    // recalculate domains
                    for (int x1 = x + 1; x1 < variables.GetLength(0); x1++)
                        NodeConsistentSingle(ref variables, x1, y);
                    for (int y1 = y + 1; y1 < variables.GetLength(1); y1++)
                        NodeConsistentSingle(ref variables, x, y1);
                    for (int j = (y % 3) * 3 + x % 3 + 1; j < 9; j++)
                        NodeConsistentSingle(ref variables, j % 3 + (x / 3) * 3, j / 3 + (y / 3) * 3);
                }
                else // this is the solution
                    return true;
            }
            return false; // no option in domain was the solution meaning the parent is wrong
        }

        static internal bool DynamicForwardChecking(ref Cell[,] variables, int x, int y)
        {
            // loop over cells to the right
            for (int x1 = x + 1; x1 < variables.GetLength(0); x1++)
                if (!DynamicForwardCheckingSingle(ref variables, variables[x, y].value, x1, y))
                    return false;

            // loop over cells below
            for (int y1 = y + 1; y1 < variables.GetLength(1); y1++)
                if (!DynamicForwardCheckingSingle(ref variables, variables[x, y].value, x, y1))
                    return false;

            // loop over cell in box, right and below
            for (int i = (y % 3) * 3 + x % 3 + 1; i < 9; i++)
                if (!DynamicForwardCheckingSingle(ref variables, variables[x, y].value, i % 3 + (x / 3) * 3, i / 3 + (y / 3) * 3))
                    return false;

            return true; // change is possible, no domain became empty
        }
        static internal bool DynamicForwardCheckingSingle(ref Cell[,] variables, int value, int x, int y)
        {
            if (Math.Sign(variables[x, y].value) == -1) // fixated cells
                return true;

            variables[x, y].domain.Remove(value); // reduce domain
            return variables[x, y].domain.Count > 0; // check if domain became empty
        }

        static internal void NodeConsistent(ref Cell[,] variables)
        {
            // loop over all variables
            for (int x = 0; x < variables.GetLength(0); x++)
                for (int y = 0; y < variables.GetLength(1); y++)
                    NodeConsistentSingle(ref variables, x, y);
        }
        static internal void NodeConsistentSingle(ref Cell[,] variables, int x, int y)
        {
            if (Math.Sign(variables[x, y].value) != -1) // if not fixated, callculate reduced domain
                variables[x, y].domain = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Except(CalcInverseDomain(variables, x, y)).ToList();
        }

        /// <summary>
        /// Gets a list of assigned values in the given row of the given sudokugrid
        /// </summary>
        /// <returns>A list of values found</returns>
        static internal List<int> GetRowOccurrences(Cell[,] variables, int x, int y)
        {
            List<int> occurences = new List<int>();
            for (int i = 0; i < variables.GetLength(0); i++) // Loop over every variables in that row and add all found values
                if (variables[i, y].value != 0)
                    occurences.Add(Math.Abs(variables[i, y].value));
            return occurences;
        }

        /// <summary>
        /// Gets a list of assigned values in the given column of the given sudokugrid
        /// </summary>
        /// <returns>A list of values found</returns>
        static internal List<int> GetColumnOccurrences(Cell[,] variables, int x, int y)
        {
            List<int> occurences = new List<int>();
            for (int i = 0; i < variables.GetLength(1); i++) // Loop over every variables in that column and add all found values
                if (variables[x, i].value != 0)
                    occurences.Add(Math.Abs(variables[x, i].value));
            return occurences;
        }

        /// <summary>
        /// Gets a list of assigned values in the given box of the given sudokugrid
        /// </summary>
        /// <returns>A list of values found</returns>
        static internal List<int> GetBoxOccurrences(Cell[,] variables, int x, int y)
        {
            List<int> occurences = new List<int>();
            int offsetx = (x / 3) * 3;
            int offsety = (y / 3) * 3;
            for (int x2 = 0; x2 < 3; x2++) // Loop over every variables in the box and add all found values
            {
                for (int y2 = 0; y2 < 3; y2++)
                {
                    if (variables[x2 + offsetx, y2 + offsety].value != 0)
                        occurences.Add(Math.Abs(variables[x2 + offsetx, y2 + offsety].value));
                }
            }
            return occurences;
        }

        /// <summary>
        /// Gets a list of assigned values for the domain (Vertical, Horizontal and Box) of the given place in the given sudokugrid
        /// </summary>
        /// <returns>A list of values found</returns>
        static internal List<int> CalcInverseDomain(Cell[,] variables, int x, int y)
        {
            HashSet<int> occurences = new HashSet<int>(); // Create a hashset to automaticly get rid off duplicates
            List<int> oRow = GetRowOccurrences(variables, x, y);
            List<int> oCol = GetColumnOccurrences(variables, x, y);
            List<int> oDom = GetBoxOccurrences(variables, x, y);

            for (int i = 0; i < oRow.Count; i++)          // Add all list in the hashset 
                occurences.Add(oRow[i]);
            for (int i = 0; i < oCol.Count; i++)
                occurences.Add(oCol[i]);
            for (int i = 0; i < oDom.Count; i++)
                occurences.Add(oDom[i]);

            return occurences.ToList();
        }

        static internal void PrintVariables(Cell[,] variables, bool absAll = true)
        {
            if (!Utils.shouldPrintIntermediaries)
                return;

            for (int x = 0; x < variables.GetLength(0); x++)
                for (int y = 0; y < variables.GetLength(1); y++)
                {
                    Console.SetCursorPosition(posX + x * 2, posY + y);
                    bool shouldUnderLine = y % 3 == 2 && y != 8;
                    int value = absAll ? Math.Abs(variables[x, y].value) : variables[x, y].value;
                    Utils.WriteUnderline(value.ToString(), shouldUnderLine);
                    Utils.WriteUnderline(x < variables.GetLength(1) - 1 ? (x % 3 == 2 ? "|" : " ") : "", shouldUnderLine, null);
                }

            if (Utils.shouldWaitAfterIntermediateStep)
            {
                char c = Utils.ReadCursorChar();
                Console.ReadKey();
                int x = Console.CursorLeft - 1;
                int y = Console.CursorTop;
                Console.SetCursorPosition(x, y);
                bool shouldUnderLine = (y - posY) % 3 == 2 && (y - posY) != 8;
                Utils.WriteUnderline(c.ToString(), shouldUnderLine);
            }
        }
        static internal void PrintVariables(Cell[,] variables, int x, int y, bool absAll = true, ConsoleColor color = ConsoleColor.Red) //Tomas
        {
            if (!Utils.shouldPrintIntermediaries)
                return;

            bool shouldUnderLine = y % 3 == 2 && y != 8;
            int value = absAll ? Math.Abs(variables[x, y].value) : variables[x, y].value;
            Console.SetCursorPosition(posX + x * 2, posY + y);
            Utils.WriteUnderline(value.ToString(), shouldUnderLine, color);

            if (Utils.shouldWaitAfterIntermediateStep)
            {
                char c = Utils.ReadCursorChar();
                Console.ReadKey();
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                shouldUnderLine = y % 3 == 2 && y != 8;
                Utils.WriteUnderline(c.ToString(), shouldUnderLine);
            }
        }
    }
}
