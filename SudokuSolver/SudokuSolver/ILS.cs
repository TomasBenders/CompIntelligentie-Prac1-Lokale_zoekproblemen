using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    internal enum WEB { worse,equal,better}; 
    static internal class ILS
    {
        // Used to select a random box to find successors from
        static readonly Random rnd = new();

        static internal SudokuGrid ILSRandomWalkHillClimbing(SudokuGrid sudokuGrid, int flatTolerance, int randomSteps)
        {
            SudokuGrid best = sudokuGrid;
            SudokuGrid localMax = sudokuGrid;
            for (int i = 0; i < 10; i++)
            {
                localMax = HillClimb(localMax, flatTolerance);
                if(localMax.HeuristicValue < best.HeuristicValue)
                    best = localMax;
                if (localMax.HeuristicValue == 0) break;
                localMax = RandomWalk(localMax, randomSteps);
            }
            return best;
        }
        /// <summary>
        /// Returns whether a better or equal successor exists and if there is returns it through the out parameter
        /// </summary>
        /// <param name="sudokuGrid"> The grid from which to find successors </param>
        /// <param name="successor"> The better or equal successor if it was found </param>
        /// <param name="boxX"> The x coordinate of the box within the grid for which to search for successors </param>
        /// <param name="boxY"> The y coordinate of the box within the grid for which to search for successors </param>
        /// <returns> <c>bool</c> for if there exists a better or equal successor </returns>
        static internal WEB GetBetterOrEqualSuccessor(SudokuGrid sudokuGrid, int boxX, int boxY, out SudokuGrid successor)
        {
            WEB web = WEB.worse;
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
                            if (successor.HeuristicValue < bestSuccessor.grid.HeuristicValue)
                            {
                                web = WEB.better;
                                bestSuccessor = (x1, y1, x2, y2, successor);
                            }
                            else if (web != WEB.better && successor.HeuristicValue == bestSuccessor.grid.HeuristicValue)
                            {
                                web = WEB.equal;
                                bestSuccessor = (x1, y1, x2, y2, successor);
                            }
                        }
                }

            successor = bestSuccessor.grid;
            if (web != WEB.worse)
                successor.PrintSwap(
                    boxX + bestSuccessor.x1,
                    boxY + bestSuccessor.y1,
                    boxX + bestSuccessor.x2,
                    boxY + bestSuccessor.y2);
            return web;
        }

        /// <summary>
        /// The primary function that executes the hillclimb algoritm
        /// </summary> 
        /// <param name="sudokuGrid"> The orgininal sudokugrid on which the algoritm should be aplied </param>
        /// <param name="flatTolerance"> A value to describe how long the algoritm can 'walk' across a platform </param>
        /// <returns> A new sudokugrid with the hillclimb algoritm aplied to it </returns>
        static internal SudokuGrid HillClimb(SudokuGrid sudokuGrid, int flatTolerance)
        {
            SudokuGrid newgrid = sudokuGrid;
            bool uphill = true;
            int timesConsecutiveWorse = 0;         //An integer to keep track of how many times there where only worse grid succesors
            int timesConsecutiveWorseOrEqual = 0;  //An integer to keep track of how many times there was no better grid succesors
            int box = 0;                           //An integer to keep track of the box that is currently inspected
            while (uphill)
            {
                WEB web = GetBetterOrEqualSuccessor(newgrid, (box % 3), (box / 3), out SudokuGrid potentialgrid);
                if (web == WEB.worse)
                {
                    timesConsecutiveWorse++;
                    timesConsecutiveWorseOrEqual++;
                }
                else if (web == WEB.equal)
                {
                    timesConsecutiveWorse = 0;
                    timesConsecutiveWorseOrEqual++;
                }
                else
                {
                    newgrid = potentialgrid;
                    timesConsecutiveWorse = 0;
                    timesConsecutiveWorseOrEqual = 0;
                }
                if (timesConsecutiveWorse >= 9 || timesConsecutiveWorseOrEqual >= flatTolerance) //If a flat or top is detected the loop is stoped
                    uphill = false;

                box++;      //The algoritm moves on to the next box
                box %= 9;
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

        static internal SudokuGrid TabuSearch(SudokuGrid sudokuGrid, int k)
        {
            SudokuGrid current = sudokuGrid;
            SudokuGrid best = sudokuGrid;
            Queue<SudokuGrid> tabuList = new();

            while (true)
            {
                if (current.HeuristicValue == 0)
                    return current;

                tabuList.Enqueue(current);
                if (tabuList.Count > k)
                    tabuList.Dequeue();

                if (!GetBestNotTabued(current, tabuList, out current))
                    break;

                if (best.AreEqual(current))
                    return best;

                if (current.HeuristicValue < best.HeuristicValue)
                    best = current;
            }

            return best;
        }

        static internal bool GetBestNotTabued(SudokuGrid sudokuGrid, Queue<SudokuGrid> tabu, out SudokuGrid best)
        {
            best = sudokuGrid;
            List<(int x1, int y1, int x2, int y2, SudokuGrid grid)> bests = new(sudokuGrid.boxSize * sudokuGrid.boxSize);

            for (int boxX = 0; boxX < 3; boxX++)
                for (int boxY = 0; boxY < 3; boxY++)
                {
                    for (int i = 0; i < sudokuGrid.boxSize * sudokuGrid.boxSize - 1; i++)
                        for (int j = i + 1; j < sudokuGrid.boxSize * sudokuGrid.boxSize; j++)
                        {
                            int x1 = boxX * sudokuGrid.boxSize + i % sudokuGrid.boxSize;
                            int y1 = boxY * sudokuGrid.boxSize + i / sudokuGrid.boxSize;
                            int x2 = boxX * sudokuGrid.boxSize + j % sudokuGrid.boxSize;
                            int y2 = boxY * sudokuGrid.boxSize + j / sudokuGrid.boxSize;

                            if (Math.Sign(sudokuGrid.GridValues[x1, y1]) == -1 ||
                                Math.Sign(sudokuGrid.GridValues[x2, y2]) == -1)
                                continue;

                            SudokuGrid successor = sudokuGrid.Swap(x1, y1, x2, y2);
                            bests.Add((x1, y1, x2, y2, successor));
                        }
                }

            bests.Sort((a, b) => a.grid.HeuristicValue.CompareTo(b.grid.HeuristicValue));

            bool foundOne = false;
            for (int i = 0; i < sudokuGrid.boxSize * sudokuGrid.boxSize; i++)
                if (!tabu.Any(a => a.AreEqual(bests[i].grid)))
                {
                    foundOne = true;
                    best = bests[i].grid;
                    best.PrintSwap(bests[i].x1, bests[i].y1, bests[i].x2, bests[i].y2);
                    break;
                }

            return foundOne;
        }
    }
}
