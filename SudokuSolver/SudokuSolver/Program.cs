// See https://aka.ms/new-console-template for more information
using SudokuSolver;
using SudokuSolver.Properties;
using System.Collections;

List<SudokuGrid> sudokuGrids = SudokuGrid.ParsePuzzels(Resources.Sudoku_puzzels_5);
//for (int i = 0; i < sudokuGrids.Count; i++)
//{
//    sudokuGrids[i].posY = i * 10;
//    sudokuGrids[i].PrintGrid();

//    sudokuGrids[i].posX += 20;
//    sudokuGrids[i].GenFilledInGrid();
//    sudokuGrids[i].CalcAllHeuristicCosts();
//    sudokuGrids[i].PrintGrid();

//    sudokuGrids[i].posX += 20;
//    //ILS.GetBetterOrEqualSuccessor(sudokuGrids[i], 1, 1, out _);
//    //ILS.RandomWalk(sudokuGrids[i], 2);
//    //var localMaxima = ILS.HillClimb(sudokuGrids[i])
//    sudokuGrids[i] = ILS.ILSRandomWalkHillClimbing(sudokuGrids[i], 10, 30);

//    sudokuGrids[i].posX += 20;
//    sudokuGrids[i].PrintScore();
//}
