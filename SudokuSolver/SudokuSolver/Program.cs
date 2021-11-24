// See https://aka.ms/new-console-template for more information
using SudokuSolver;
using SudokuSolver.Properties;

Console.WriteLine("Sudoku is solved, bicchh");

List<SudokuGrid> sudokuGrids = SudokuGrid.ParsePuzzels(Resources.Sudoku_puzzels_5);
foreach (var item in sudokuGrids)
{
    item.PrintGrid(0, 0);
}