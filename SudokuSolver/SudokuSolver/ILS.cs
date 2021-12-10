using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    static internal class ILS
    {
        // Used to select a random box to find successors from
        static readonly Random rnd = new();

        static internal SudokuGrid IterativeLocalSearch(SudokuGrid sudokuGrid, int flatTolerance, int randomSteps)
        {
            int bestScore = int.MaxValue;
            SudokuGrid localMax = sudokuGrid;
            for (int i = 0; i < 10; i++)
            {
                localMax = HillClimb(localMax, flatTolerance);
                bestScore = Math.Min(bestScore,localMax.HeuristicValue);
                if (localMax.HeuristicValue == 0) break;
                localMax = RandomWalk(localMax, randomSteps);
            }
            return localMax;
        }
        /// <summary>
        /// Returns whether a better or equal successor exists and if there is returns it through the out parameter
        /// </summary>
        /// <param name="sudokuGrid"> The grid from which to find successors </param>
        /// <param name="successor"> The better or equal successor if it was found </param>
        /// <param name="boxX"> The x coordinate of the box within the grid for which to search for successors </param>
        /// <param name="boxY"> The y coordinate of the box within the grid for which to search for successors </param>
        /// <returns> <c>bool</c> for if there exists a better or equal successor </returns>
        static internal bool GetBetterOrEqualSuccessor(SudokuGrid sudokuGrid, int boxX, int boxY, out SudokuGrid successor)
        {
            bool foundBetterOrEqualSuccessor = false;
            (int x1, int y1, int x2, int y2, SudokuGrid grid) bestSuccessor = (-1, -1, -1, -1, sudokuGrid);

            int[,] box = sudokuGrid.GetBox(boxX, boxY);
            boxX *= sudokuGrid.boxSize;
            boxY *= sudokuGrid.boxSize;

            for (int x1 = 0; x1 < sudokuGrid.boxSize; x1++)
                for (int y1 = 0; y1 < sudokuGrid.boxSize; y1++)
                {
                    if (Math.Sign(box[x1, y1]) == -1)
                        continue;

                    for (int x2 = 0; x2 < sudokuGrid.boxSize; x2++)
                        for (int y2 = 0; y2 < sudokuGrid.boxSize; y2++)
                        {
                            if (Math.Sign(box[x2, y2]) == -1 ||
                                (x1 == x2 && y1 == y2))
                                continue;

                            successor = sudokuGrid.Swap(boxX + x1, boxY + y1, boxX + x2, boxY + y2);
                            if (successor.HeuristicValue <= bestSuccessor.grid.HeuristicValue)
                            {
                                foundBetterOrEqualSuccessor = true;
                                bestSuccessor = (x1, y1, x2, y2, successor);
                            }
                        }
                }

            successor = bestSuccessor.grid;
            if (foundBetterOrEqualSuccessor)
                successor.PrintSwap(
                    boxX + bestSuccessor.x1,
                    boxY + bestSuccessor.y1,
                    boxX + bestSuccessor.x2,
                    boxY + bestSuccessor.y2);
            return foundBetterOrEqualSuccessor;
        }

        static internal SudokuGrid HillClimb(SudokuGrid sudokuGrid, int flatTolerance)
        {
            SudokuGrid newgrid = sudokuGrid;
            bool uphill = true;
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
            return newgrid;
        }

        static internal SudokuGrid RandomWalk(SudokuGrid sudokuGrid, int s)
        {
            if (s < 0)
                return sudokuGrid;

            int whichsquarex = rnd.Next(0, sudokuGrid.boxSize);
            int whichsquarey = rnd.Next(0, sudokuGrid.boxSize);

            int x1;
            int y1;
            do 
            {
                x1 = rnd.Next(whichsquarex * sudokuGrid.boxSize,
                    whichsquarex * sudokuGrid.boxSize + sudokuGrid.boxSize);
                y1 = rnd.Next(whichsquarey * sudokuGrid.boxSize,
                    whichsquarey * sudokuGrid.boxSize + sudokuGrid.boxSize);
            }
            while (Math.Sign(sudokuGrid.GridValues[x1, y1]) == -1);
            
            int x2;
            int y2;
            do
            {
                x2 = rnd.Next(whichsquarex * sudokuGrid.boxSize,
                    whichsquarex * sudokuGrid.boxSize + sudokuGrid.boxSize);
                y2 = rnd.Next(whichsquarey * sudokuGrid.boxSize,
                    whichsquarey * sudokuGrid.boxSize + sudokuGrid.boxSize);
            }
            while ((x2 == x1 && y2 == y1) || Math.Sign(sudokuGrid.GridValues[x2, y2]) == -1);

            SudokuGrid walked = sudokuGrid.Swap(x1, y1, x2, y2);
            walked.PrintSwap(x1, y1, x2, y2);
            return RandomWalk(walked, --s);
        }
    }
}
