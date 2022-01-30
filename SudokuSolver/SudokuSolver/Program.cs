// See https://aka.ms/new-console-template for more information
using SudokuSolver;
using SudokuSolver.Properties;
using System.Collections;

Utils.shouldPrintIntermediaries = false;        // print swaps and steps before completion
Utils.howManyTimesRun = 100;                      // how many times each puzzel should be solved. Results in more consistent avarage times
Utils.shouldWaitAfterIntermediateStep = false;  // wait after an intermediate step

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
#if true // CBT
    sudokuGrids[i].posX += 20;

    CBT.resetTimes();
    SudokuGrid solvedSudoku = CBT.CBTSolver(sudokuGrids[i], out bool isSolved, out int statesGenerated);

    for (int j = 1; j < Utils.howManyTimesRun; j++) //run x times for better avarage time taken
        solvedSudoku = CBT.CBTSolver(sudokuGrids[i], out isSolved, out statesGenerated);

    solvedSudoku.PrintGrid(); // Print result
    var avgTime = CBT.getAvarageTime();

    //Print measurements
    solvedSudoku.posX += 19;
    Console.SetCursorPosition(solvedSudoku.posX, solvedSudoku.posY);
    Console.Write("Solved".PadRight(35) + ": {0}", isSolved);
    Console.SetCursorPosition(solvedSudoku.posX, solvedSudoku.posY + 1);
    Console.Write("Times run".PadRight(35) + ": {0}", Utils.howManyTimesRun);
    Console.SetCursorPosition(solvedSudoku.posX, solvedSudoku.posY + 2);
    Console.Write("States generated per run".PadRight(35) + ": {0}", statesGenerated);
    Console.SetCursorPosition(solvedSudoku.posX, solvedSudoku.posY + 3);
    Console.Write("Avarage time taken in milliseconds".PadRight(35) + ": {0}", avgTime.TotalMilliseconds);
    Console.SetCursorPosition(solvedSudoku.posX, solvedSudoku.posY + 4);
    Console.Write("Avarage time taken in ticks".PadRight(35) + ": {0}", avgTime.Ticks);

#endif
    #endregion
}

Console.SetCursorPosition(0, sudokuGrids.Count * 10);
Console.WriteLine("Press any key to quit");
Console.Read();