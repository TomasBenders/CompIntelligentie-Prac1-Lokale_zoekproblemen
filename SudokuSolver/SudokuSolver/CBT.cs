using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    static internal class CBT
    {
        static internal SudokuGrid CBTSolver(SudokuGrid sudokuGrid, int x, int y) //Tomas
        {
            throw new NotImplementedException();
        }

        static internal bool DynamicForwardChecking(ref List<int>[,] variables, int x, int y) //Erben
        {
            throw new NotImplementedException();
        }

        static internal void NodeConsistent(ref List<int>[,] variables) //Erben
        {
            throw new NotImplementedException();
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

        static internal void PrintVariables(List<int>[,] variables, int newX, int newY) //Tomas
        {
            throw new NotImplementedException();
        }
    }
}
