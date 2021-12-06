// See https://aka.ms/new-console-template for more information
using SudokuSolver;
using SudokuSolver.Properties;

List<SudokuGrid> sudokuGrids = SudokuGrid.ParsePuzzels(Resources.Sudoku_puzzels_5);
for (int i = 0; i < sudokuGrids.Count; i++)
    sudokuGrids[i].PrintGrid(i*20%40, i/2*10);

sudokuGrids[0].PrintBestSuccessor(0, 0, 0, 0, 5, 5);