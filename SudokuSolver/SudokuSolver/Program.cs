// See https://aka.ms/new-console-template for more information
using SudokuSolver;
using SudokuSolver.Properties;
using System.Collections;

//zet de 5 gegeven puzzels om in sudokuGrids
List<SudokuGrid> sudokuGrids = SudokuGrid.ParsePuzzels(Resources.Sudoku_puzzels_5);
for (int i = 0; i < sudokuGrids.Count; i++)
{
    // Print de input puzzel
    sudokuGrids[i].posY = i * 10;
    sudokuGrids[i].PrintGrid();

    #region Assignment 1
#if false
    // Print de ingevulde puzzel met score
    sudokuGrids[i].posX += 20;
    sudokuGrids[i].GenFilledInGrid();
    sudokuGrids[i].CalcAllHeuristicCosts();
    sudokuGrids[i].PrintGrid();  
    sudokuGrids[i].posX += 18;
    sudokuGrids[i].PrintScore();

    // Onthou de begintoestand
    int[,] startingValues = sudokuGrids[i].GridValues;

    // Print de ILSRandomWalkHillClimbing opgeloste puzzel met score
    sudokuGrids[i].posX += 12;
    sudokuGrids[i] = ILS.ILSRandomWalkHillClimbing(sudokuGrids[i], 10, 3, 10);
    sudokuGrids[i].PrintGrid();
    sudokuGrids[i].posX += 18;
    sudokuGrids[i].PrintScore();

    //reset de sudoku naar de begintoestand
    for (int x = 0; x < sudokuGrids[i].GridSize; x++)
        for (int y = 0; y < sudokuGrids[i].GridSize; y++)
            sudokuGrids[i].GridValues[x, y] = startingValues[x, y];

    // Print de TabuSearch opgeloste puzzel met score
    sudokuGrids[i].posX += 12;
    sudokuGrids[i] = ILS.TabuSearch(sudokuGrids[i], 100, 100);
    sudokuGrids[i].PrintGrid();
    sudokuGrids[i].posX += 18;
    sudokuGrids[i].PrintScore();
#endif
    #endregion

    #region Assignment 2
#if true
    // Print resultaat van CBT
    sudokuGrids[i].posX += 20;
    sudokuGrids[i] = CBT.CBTSolver(sudokuGrids[i], 0, 0);
    sudokuGrids[i].PrintGrid();
#endif
    #endregion
}

Console.SetCursorPosition(0, sudokuGrids.Count * 10);
Console.WriteLine("Press any key to quit");
Console.Read();