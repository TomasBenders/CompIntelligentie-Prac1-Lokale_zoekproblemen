// See https://aka.ms/new-console-template for more information
using SudokuSolver;
using SudokuSolver.Properties;

List<SudokuGrid> sudokuGrids = SudokuGrid.ParsePuzzels(Resources.Sudoku_puzzels_5);
for (int i = 0; i < sudokuGrids.Count; i++)
{
    sudokuGrids[i].posX = i * 20 % 40;
    sudokuGrids[i].posY = i / 2 * 10;
    sudokuGrids[i].PrintGrid();
}

sudokuGrids[0].PrintBestSuccessor(0, 0, 5, 5);