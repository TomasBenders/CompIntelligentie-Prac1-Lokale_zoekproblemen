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

        internal class Cell {
            internal int value = 0;
            internal List<int> domain = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        static internal SudokuGrid CBTSolver(SudokuGrid sudokuGrid, int x, int y) //Tomas
        {
            // Set position for printing
            posX = sudokuGrid.posX;
            posY = sudokuGrid.posY;

            // Convert sudokuGrid to variables
            Cell[,] variables = new Cell[sudokuGrid.GridSize, sudokuGrid.GridSize];
            for (int x2 = 0; x2 < sudokuGrid.GridSize; x2++)
                for (int y2 = 0; y2 < sudokuGrid.GridSize; y2++)
                    variables[x2, y2].value = -sudokuGrid.GridValues[x, y];

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

        static internal bool ChronologicalBackTracking(ref Cell[,] variables, int x = 0, int y = 0)
        {
            if(Math.Sign(variables[x,y].value) == -1) // fixated cell
                return ChronologicalBackTracking(ref variables, (x + 1) % variables.GetLength(1), (x + 1) / variables.GetLength(1)))

            for (int i = 0; i < variables[x,y].domain.Count; i++)
            {
                if (!DynamicForwardChecking(ref variables, x, y) ||
                    !ChronologicalBackTracking(ref variables, (x + 1) % variables.GetLength(1), (x + 1) / variables.GetLength(1)))
                {
                    //TO DO: revert changes

                }
                else
                    return true;
            }
            return false;
        }

        static internal bool DynamicForwardChecking(ref Cell[,] variables, int x, int y) //Erben
        {
            List<int> actualdomain = CalcDomain(variables, x, y);
            List<int> domein = variables[x, y].domain;
            if (domein.Sum() < 0) return false;
            int number = variables[x, y].value;

            for (int right = x; right < 9; right++)
            {
                //rechts domain updaten
                variables[right, y].domain.Remove(number);
            }
            for (int left = y; left < 9; left++)
            {
                variables[x, left].domain.Remove(number);
                //links domain updaten


                //get first number in domain
                //check if row,colum and box domains still have that number
                //if not impossible
                //if they do delete number and update domain


            }
            int x1 = 3 * (x / 3);
            int y1 = 3 * (y / 3);
            int x2 = x1 + 2;
            int y2 = y1 + 2;
            for (int x3 = x1; x <= x2; x++)
            {
                for (int y3 = y1; y <= y2; y++)
                {
                    variables[x3, y3].domain.Remove(number);
                }
            }
            return true;
            //throw new NotImplementedException();
        }

        static internal void NodeConsistent(ref Cell[,] variables) //Erben
        {
            for(int x = 0; x<9;x++)
            {
                for (int y = 0; y < 9; y++)
                {
                    List<int> domein = variables[x,y].domain;

                    if (domein.Count == 1) break;
                    List<int> actualdomain = CalcDomain(variables, x, y);
                    variables[x, y].domain = (List<int>)domein.Except(actualdomain);
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

        static internal List<int> GetRowOccurrences(Cell[,] variables, int x, int y) //Tjerk
        {
            List<int> occurences = new List<int>();
            for (int i = 0; i < variables.GetLength(0); i++)
                if(variables[i, y].value != 0)
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
                    if (variables[x2 + offsetx , y2 + offsety ].value != 0)
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

        static internal void PrintVariables(Cell[,] variables, int newX = -1, int newY = -1, bool absAll = true) //Tomas
        {
            for (int x = 0; x < variables.GetLength(0); x++)
                for (int y = 0; y < variables.GetLength(1); y++)
                {
                    Console.SetCursorPosition(posX + x * 2, posY + y);
                    bool shouldUnderLine = y % 3 == 2 && y != 8;
                    bool isNewCoord = x == newX && y == newY;
                    int value = absAll ? Math.Abs(variables[x, y].value) : variables[x, y].value;
                    Utils.WriteUnderline(value.ToString(), shouldUnderLine, isNewCoord ? ConsoleColor.Red : null);
                    Utils.WriteUnderline(x < variables.GetLength(1) - 1 ? (x % 3 == 2 ? "|" : " ") : "", shouldUnderLine, null);
                }
        }
    }
}
