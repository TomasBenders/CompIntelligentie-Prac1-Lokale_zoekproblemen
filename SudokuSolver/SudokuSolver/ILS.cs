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
        
        /// <summary>
        /// The function to combine hill climbing and random walk (almost) as described in the assignment
        /// </summary>
        /// <param name="sudokuGrid">The sudoku to solve</param>
        /// <param name="flatTolerance">The parameter to pass on to hillClimb</param>
        /// <param name="randomSteps">The parameter to pass on to RandomWalk</param>
        /// <param name="itterations">The amount of times to perform hillClimb and RandomWalk</param>
        /// <returns>A hopefully solved Sudoku</returns>
        static internal SudokuGrid ILSRandomWalkHillClimbing(SudokuGrid sudokuGrid, int flatTolerance, int randomSteps, int itterations)
        {
            SudokuGrid best = sudokuGrid;
            SudokuGrid localMax = sudokuGrid;
            for (int i = 0; i < itterations; i++)
            {
                localMax = HillClimb(localMax, flatTolerance);
                if(localMax.HeuristicValue < best.HeuristicValue)
                    best = localMax; // Remember best grid
                if (localMax.HeuristicValue == 0) break; // If solved return
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
            //remember best successor and its swap
            (int x1, int y1, int x2, int y2, SudokuGrid grid) bestSuccessor = (-1, -1, -1, -1, sudokuGrid);

            int[,] box = sudokuGrid.GetBox(boxX, boxY);
            boxX *= sudokuGrid.boxSize;
            boxY *= sudokuGrid.boxSize;

            //Loop over all possible swaps
            for (int x1 = 0; x1 < sudokuGrid.boxSize; x1++)
                for (int y1 = 0; y1 < sudokuGrid.boxSize; y1++)
                {
                    if (Math.Sign(box[x1, y1]) == -1) // If fixated, dont swap
                        continue;

                    for (int x2 = 0; x2 < sudokuGrid.boxSize; x2++)
                        for (int y2 = 0; y2 < sudokuGrid.boxSize; y2++)
                        {
                            if (Math.Sign(box[x2, y2]) == -1 ||
                                (x1 == x2 && y1 == y2))
                                continue;// If fixated or the same as the first coordinate, dont swap

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
        /// <summary>
        /// The function that swaps random values s times
        /// </summary> 
        /// <param name="sudokuGrid"> The orgininal sudokugrid on which the algoritm should be aplied </param>
        /// <param name="s"> the amount of times the function will randomly swap </param>
        /// <returns> A new sudokugrid with the randomWalk algoritm aplied to it </returns>
        static internal SudokuGrid RandomWalk(SudokuGrid sudokuGrid, int s)
        {
            if (s < 0) //if no more randomwalk just return the given sudokugrid
                return sudokuGrid;

            int whichsquarex = rnd.Next(0, sudokuGrid.boxSize);// get a random square
            int whichsquarey = rnd.Next(0, sudokuGrid.boxSize);

            int x1;
            int y1;
            do //gets a random number in the square to swap, based on which square was chosen and the boxsize
            {
                x1 = rnd.Next(whichsquarex * sudokuGrid.boxSize,
                    whichsquarex * sudokuGrid.boxSize + sudokuGrid.boxSize); 
                y1 = rnd.Next(whichsquarey * sudokuGrid.boxSize,
                    whichsquarey * sudokuGrid.boxSize + sudokuGrid.boxSize);
            }
            while (Math.Sign(sudokuGrid.GridValues[x1, y1]) == -1);
            
            int x2;
            int y2;
            do //same thing but check if the number isnt the same so to no swap the same number
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
        /// <summary>
        /// The function to apply TabuSearch to a SudokuGrid
        /// </summary>
        /// <param name="sudokuGrid">The sudoku to solve</param>
        /// <param name="k">The length of the tabuList</param>
        /// <param name="iterationsWithoutImprovement">The maximum number of steps without improvement</param>
        /// <returns>A potentially solved sudoku</returns>
        static internal SudokuGrid TabuSearch(SudokuGrid sudokuGrid, int k, int iterationsWithoutImprovement)
        {
            SudokuGrid current = sudokuGrid;
            SudokuGrid best = sudokuGrid;
            Queue<SudokuGrid> tabuList = new(); // The banned states
            int i = 0;

            while (true)
            {
                if (current.HeuristicValue == 0)
                    return current; // if solved, return

                tabuList.Enqueue(current);
                if (tabuList.Count > k)
                    tabuList.Dequeue(); // keep the amount of banned states equal to k

                if (!GetBestNotTabued(current, tabuList, out current)) // find successor
                    break; // if not successor that isnt banned, return

                if (best.AreEqual(current)) // if current == banned, we have a sycle and should return
                    return best;

                if (current.HeuristicValue < best.HeuristicValue) // remember best state
                {
                    best = current;
                    i = 0;
                }
                else
                {
                    i++;
                    if (i > iterationsWithoutImprovement)
                        return best; // if no improvement for iterationsWithoutImprovement times, return
                }
            }

            return best;
        }
        /// <summary>
        /// The function to get the successor for tabuSearch
        /// </summary>
        /// <param name="sudokuGrid">The sudoku in which to find a successor</param>
        /// <param name="tabu">The list of taboed sudokus</param>
        /// <param name="best">The successor if one was found</param>
        /// <returns>A bool for whether or not a successor was found that isnt taboed</returns>
        static internal bool GetBestNotTabued(SudokuGrid sudokuGrid, Queue<SudokuGrid> tabu, out SudokuGrid best)
        {
            best = sudokuGrid;
            // List to remember potential successors and their swaps
            List<(int x1, int y1, int x2, int y2, SudokuGrid grid)> bests = new(sudokuGrid.boxSize * sudokuGrid.boxSize);

            // loop over every potential swap in the entire grid
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
                                continue; // if fixated, dont swap

                            SudokuGrid successor = sudokuGrid.Swap(x1, y1, x2, y2);
                            bests.Add((x1, y1, x2, y2, successor));
                        }
                }

            bests.Sort((a, b) => a.grid.HeuristicValue.CompareTo(b.grid.HeuristicValue));

            bool foundOne = false;
            for (int i = 0; i < sudokuGrid.boxSize * sudokuGrid.boxSize; i++)
                if (!tabu.Any(a => a.AreEqual(bests[i].grid))) // if state is not taboed
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
