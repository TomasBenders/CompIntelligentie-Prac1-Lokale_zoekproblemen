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

        static internal SudokuGrid CBTSolver(SudokuGrid sudokuGrid, int x, int y) //Tomas
        {
            // Set position for printing
            posX = sudokuGrid.posX;
            posY = sudokuGrid.posY;

            // Convert sudokuGrid to variables
            List<int>[,] variables = new List<int>[sudokuGrid.GridSize, sudokuGrid.GridSize];
            for (int x2 = 0; x2 < sudokuGrid.GridSize; x2++)
                for (int y2 = 0; y2 < sudokuGrid.GridSize; y2++)
                    variables[x2, y2] = sudokuGrid.GridValues[x2, y2] == 0 ?
                        new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 } :
                        new List<int> { -sudokuGrid.GridValues[x2, y2] };

            //TO DO: CBT Algorithm


            // Convert variables to value array
            int[] values = new int[sudokuGrid.GridSize * sudokuGrid.GridSize];
            for (int x2 = 0; x2 < sudokuGrid.GridSize; x2++)
                for (int y2 = 0; y2 < sudokuGrid.GridSize; y2++)
                    values[x2 + y2 * sudokuGrid.GridSize] = variables[x2, y2].Count == 1 ? variables[x2, y2][0] : 0;

            // Return new sudoGrid with value array
            return new SudokuGrid(sudokuGrid.boxSize, values) { posX = posX, posY = posY };
        }

        static internal bool DynamicForwardChecking(ref List<int>[,] variables, int x, int y) //Erben
        {
            throw new NotImplementedException();
        }

        static internal void NodeConsistent(ref List<int>[,] variables) //Erben
        {
            for(int x = 0; x<9;x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    List<int> domein = variables[x,y];

                    if (domein.Count == 1) break;
                    List<int> actualdomain = CalcDomain(variables, x, y);
                    variables[x, y] = (List<int>)domein.Except(actualdomain);
                }
            }

            //procedure node-consistency(CSP) returns CSP:
            //for each Vi ∈ V do
            //        Di ← Di ∩ { d | (d, d) ∈ Ci,i}
            //od;
            //for each Ci, j ∈ C do
            //        Ci,j ← Ci,j ∩ (Di × Dj)
            //od
            //end
           // throw new NotImplementedException();
        }

        static internal List<int> GetRowOccurrences(List<int>[,] variables, int x, int y) //Tjerk
        {
            throw new NotImplementedException();
        }
        static internal List<int> GetColumnOccurrences(List<int>[,] variables, int x, int y) //Tjerk
        {
            throw new NotImplementedException();
        }
        static internal List<int> GetBoxOccurrences(List<int>[,] variables, int x, int y) //Tjerk
        {
            throw new NotImplementedException();
        }
        static internal List<int> CalcDomain(List<int>[,] variables, int x, int y) //Tjerk
        {
            throw new NotImplementedException();
        }

        static internal void PrintVariables(List<int>[,] variables, int newX = -1, int newY = -1, bool absAll = true) //Tomas
        {
            for (int x = 0; x < variables.GetLength(0); x++)
                for (int y = 0; y < variables.GetLength(1); y++)
                {
                    Console.SetCursorPosition(posX + x * 2, posY + y);
                    bool shouldUnderLine = y % 3 == 2 && y != 8;
                    bool isNewCoord = x == newX && y == newY;
                    int value = 0;
                    if (variables[x, y].Count == 1)
                        value = absAll ? Math.Abs(variables[x, y][0]) : variables[x, y][0];
                    Utils.WriteUnderline(value.ToString(), shouldUnderLine, isNewCoord ? ConsoleColor.Red : null);
                    Utils.WriteUnderline(x < variables.GetLength(1) - 1 ? (x % 3 == 2 ? "|" : " ") : "", shouldUnderLine, null);
                }
        }
    }
}
