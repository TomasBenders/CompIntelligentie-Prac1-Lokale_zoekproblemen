using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    /// <summary>
    /// A helper class for Console functions
    /// https://stackoverflow.com/questions/5237666/adding-text-decorations-to-console-output
    /// </summary>
    static internal class Utils
    {
        // config
        static internal bool shouldPrintIntermediaries;  // print swaps and steps before completion
        static internal bool shouldMeasureStats;         // print stats for experiment measuring

        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        /// <summary>
        /// Writes to the console with or without underline and/or textColor
        /// </summary>
        /// <param name="str"> The string to write to the Console </param>
        /// <param name="shouldUnderline"> Whether to underline the text or not </param>
        /// <param name="textColor"> The text color to print to the console. 
        /// If textColor is null, the existing text colo is used</param>
        static internal void WriteUnderline(string str, bool shouldUnderline = true, ConsoleColor? textColor = null)
        {
            ConsoleColor fg = Console.ForegroundColor;
            Console.ForegroundColor = textColor ?? fg;

            if (shouldUnderline)
            {
                var handle = GetStdHandle(STD_OUTPUT_HANDLE);
                GetConsoleMode(handle, out uint mode);
                mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                SetConsoleMode(handle, mode);
                Console.Write($"\x1B[4m{str}\x1B[24m");
            }
            else
                Console.Write(str);

            Console.ForegroundColor = fg;
        }
    }
}
