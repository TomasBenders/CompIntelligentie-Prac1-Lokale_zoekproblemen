using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    internal class ILS
    {
        // Used to select a random box to find successors from
        readonly Random rnd = new();

        /// <summary>
        /// Returns whether a better or equal successor exists and if there is returns it through the out parameter
        /// </summary>
        /// <param name="sudokuGrid"> The grid from which to find successors </param>
        /// <param name="successor"> The better or equal successor if it was found </param>
        /// <param name="boxX"> The x coordinate of the box within the grid for which to search for successors </param>
        /// <param name="boxY"> The y coordinate of the box within the grid for which to search for successors </param>
        /// <returns> <c>bool</c> for if there exists a better or equal successor </returns>
        private bool GetBetterOrEqualSuccessor(SudokuGrid sudokuGrid, int boxX, int boxY, out SudokuGrid successor)
        {
            bool foundBetterOrEqualSuccessor = false;
            (int x1, int y1, int x2, int y2, SudokuGrid) bestSuccessor = (-1, -1, -1, -1, sudokuGrid);

            byte[,] box = sudokuGrid.GetBox(boxX, boxY);

            for (int x1 = 0; x1 < sudokuGrid.boxSize; x1++)
                for (int y1 = 0; y1 < sudokuGrid.boxSize; y1++)
                {
                    if (Math.Sign(box[x1, y1]) == -1)
                        continue;

                    for (int x2 = 0; x2 < sudokuGrid.boxSize; x2++)
                        for (int y2 = 0; y2 < sudokuGrid.boxSize; y2++)
                        {
                            if (Math.Sign(box[x2, y2]) == -1)
                                continue;

                            successor = sudokuGrid.Swap(x1, y1, x2, y2);
                            if (successor.HeuristicValue >= bestSuccessor.Item5.HeuristicValue)
                            {
                                foundBetterOrEqualSuccessor = true;
                                bestSuccessor = (x1, y1, x2, y2, successor);
                            }
                        }
                }

            successor = bestSuccessor.Item5;
            if (foundBetterOrEqualSuccessor)
                bestSuccessor.Item5.PrintBestSuccessor(bestSuccessor.Item1, bestSuccessor.Item2, bestSuccessor.Item3, bestSuccessor.Item4);
            return foundBetterOrEqualSuccessor;
        }

        private SudokuGrid HillClimb(SudokuGrid sudokuGrid)
        {
            SudokuGrid newgrid = sudokuGrid;
            bool uphill = true;
            int flatTolerance = 5;
            int timesOnFlat = 0;
            while (uphill)
            {
                int box = rnd.Next(0, 8);
                SudokuGrid potentialgrid;
                bool different = GetBetterOrEqualSuccessor(newgrid, (box % 3), (box / 3), out potentialgrid);
                if (different)
                {
                    newgrid = potentialgrid;
                    timesOnFlat = 0;
                }
                else
                    timesOnFlat++;
                if (timesOnFlat >= flatTolerance)
                    uphill = false;
            }
            return sudokuGrid;
        }

        private SudokuGrid RandomWalk(SudokuGrid sudokuGrid, int s)
        {
            for (int i = 0; i < s; i++)
            {
                int whichsquarex = rnd.Next(0, 2);
                int whichsquarey = rnd.Next(0, 2);
                int x1 = rnd.Next(0, 2);
                int y1 = rnd.Next(0, 2);
                int x2;
                int y2;

                do
                {
                    x2 = rnd.Next(0, 2);
                    y2 = rnd.Next(0, 2);
                }
                while (x2 == x1 && y2 == y1);
                
                return sudokuGrid.Swap(x1+whichsquarex*3, y1 + whichsquarey * 3, x2 + whichsquarex * 3, y2 + whichsquarey * 3);

            }
            return sudokuGrid;
            //throw new NotImplementedException();
        }
    }
}
