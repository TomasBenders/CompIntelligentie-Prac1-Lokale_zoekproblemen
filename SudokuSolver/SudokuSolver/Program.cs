// See https://aka.ms/new-console-template for more information
using SudokuSolver;
using SudokuSolver.Properties;

List<SudokuGrid> sudokuGrids = SudokuGrid.ParsePuzzels(Resources.Sudoku_puzzels_5);
for (int i = 0; i < sudokuGrids.Count; i++)
    sudokuGrids[i].PrintGrid(0, i*10);