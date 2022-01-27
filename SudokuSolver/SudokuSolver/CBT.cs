using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    static internal class CBT
    {
        static int posX;
        static int posY;

        internal class Cell
        {
            internal int value;
            internal List<int> domain = new();

            internal Cell(int value = 0)
            {
                this.value = value;
            }
        }

        static internal SudokuGrid CBTSolver(SudokuGrid sudokuGrid) //Tomas
        {
            // Set position for printing
            posX = sudokuGrid.posX;
            posY = sudokuGrid.posY;

            // Convert sudokuGrid to variables
            Cell[,] variables = new Cell[sudokuGrid.GridSize, sudokuGrid.GridSize];
            for (int x2 = 0; x2 < sudokuGrid.GridSize; x2++)
                for (int y2 = 0; y2 < sudokuGrid.GridSize; y2++)
                    variables[x2, y2] = new(-sudokuGrid.GridValues[x2, y2]);

            PrintVariables(variables);
            //Apply CBT Algorithm
            ChronologicalBackTracking(ref variables);

            // Convert variables to value array
            int[] values = new int[sudokuGrid.GridSize * sudokuGrid.GridSize];
            for (int x2 = 0; x2 < sudokuGrid.GridSize; x2++)
                for (int y2 = 0; y2 < sudokuGrid.GridSize; y2++)
                    values[x2 + y2 * sudokuGrid.GridSize] = variables[x2, y2].value;

            // Return new sudoGrid with value array
            return new SudokuGrid(sudokuGrid.boxSize, values) { posX = posX, posY = posY };
        }

        static internal void ChronologicalBackTracking(ref Cell[,] variables)
        {
            NodeConsistent(ref variables);
            ChronologicalBackTracking(ref variables, 0, 0);
        }
        static internal bool ChronologicalBackTracking(ref Cell[,] variables, int x, int y)
        {
            if (y >= variables.GetLength(1)) // all cells done
                return true;

            if (Math.Sign(variables[x, y].value) == -1) // fixated cell
                return ChronologicalBackTracking(ref variables, (x + 1) % variables.GetLength(1), y + (x + 1) / variables.GetLength(1));

            for (int i = 0; i < variables[x, y].domain.Count; i++)
            {
                variables[x, y].value = variables[x, y].domain[i];
                PrintVariables(variables, x, y);

                if (!DynamicForwardChecking(ref variables, x, y) ||
                    !ChronologicalBackTracking(ref variables, (x + 1) % variables.GetLength(1), y + (x + 1) / variables.GetLength(1)))
                {
                    variables[x, y].value = 0;
                    PrintVariables(variables, x, y, color: ConsoleColor.Blue);

                    for (int x1 = x + 1; x1 < variables.GetLength(0); x1++)
                        NodeConsistentSingle(ref variables, x1, y);
                    for (int y1 = y + 1; y1 < variables.GetLength(1); y1++)
                        NodeConsistentSingle(ref variables, x, y1);
                    for (int j = (y % 3) * 3 + x % 3 + 1; j < 9; j++)
                        NodeConsistentSingle(ref variables, j % 3 + (x / 3) * 3, j / 3 + (y / 3) * 3);
                }
                else
                    return true;
            }
            return false;
        }

        static internal bool DynamicForwardChecking(ref Cell[,] variables, int x, int y) //Erben
        {
            for (int x1 = x + 1; x1 < variables.GetLength(0); x1++)
                if (!DynamicForwardCheckingSingle(ref variables, variables[x, y].value, x1, y))
                    return false;

            for (int y1 = y + 1; y1 < variables.GetLength(1); y1++)
                if (!DynamicForwardCheckingSingle(ref variables, variables[x, y].value, x, y1))
                    return false;

            for (int i = (y % 3) * 3 + x % 3 + 1; i < 9; i++)
                if (!DynamicForwardCheckingSingle(ref variables, variables[x, y].value, i % 3 + (x / 3) * 3, i / 3 + (y / 3) * 3))
                    return false;

            return true;
        }
        static internal bool DynamicForwardCheckingSingle(ref Cell[,] variables, int value, int x, int y)
        {
            if (Math.Sign(variables[x, y].value) == -1)
                return true;

            variables[x, y].domain.Remove(value);
            return variables[x, y].domain.Count > 0;
        }

        static internal void NodeConsistent(ref Cell[,] variables) //Erben
        {
            for (int x = 0; x < variables.GetLength(0); x++)
                for (int y = 0; y < variables.GetLength(1); y++)
                    NodeConsistentSingle(ref variables, x, y);
        }
        static internal void NodeConsistentSingle(ref Cell[,] variables, int x, int y)
        {
            if (Math.Sign(variables[x, y].value) != -1)
                variables[x, y].domain = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.Except(CalcDomain(variables, x, y)).ToList();
        }

        static internal List<int> GetRowOccurrences(Cell[,] variables, int x, int y) //Tjerk
        {
            List<int> occurences = new List<int>();
            for (int i = 0; i < variables.GetLength(0); i++)
                if (variables[i, y].value != 0)
                    occurences.Add(Math.Abs(variables[i, y].value));
            return occurences;
        }
        static internal List<int> GetColumnOccurrences(Cell[,] variables, int x, int y) //Tjerk
        {
            List<int> occurences = new List<int>();
            for (int i = 0; i < variables.GetLength(1); i++)
                if (variables[x, i].value != 0)
                    occurences.Add(Math.Abs(variables[x, i].value));
            return occurences;
        }
        static internal List<int> GetBoxOccurrences(Cell[,] variables, int x, int y) //Tjerk
        {
            List<int> occurences = new List<int>();
            int offsetx = (x / 3) * 3;
            int offsety = (y / 3) * 3;
            for (int x2 = 0; x2 < 3; x2++)
            {
                for (int y2 = 0; y2 < 3; y2++)
                {
                    if (variables[x2 + offsetx, y2 + offsety].value != 0)
                        occurences.Add(Math.Abs(variables[x2 + offsetx, y2 + offsety].value));
                }
            }
            return occurences;
        }
        static internal List<int> CalcDomain(Cell[,] variables, int x, int y) //Tjerk
        {
            HashSet<int> occurences = new HashSet<int>();
            List<int> oRow = GetRowOccurrences(variables, x, y);
            List<int> oCol = GetColumnOccurrences(variables, x, y);
            List<int> oDom = GetBoxOccurrences(variables, x, y);

            for (int i = 0; i < oRow.Count; i++)
                occurences.Add(oRow[i]);
            for (int i = 0; i < oCol.Count; i++)
                occurences.Add(oCol[i]);
            for (int i = 0; i < oDom.Count; i++)
                occurences.Add(oDom[i]);

            return occurences.ToList();
        }

        static internal void PrintVariables(Cell[,] variables, bool absAll = true) //Tomas
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
