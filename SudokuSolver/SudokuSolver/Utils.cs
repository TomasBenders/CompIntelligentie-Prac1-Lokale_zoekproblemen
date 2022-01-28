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
        static internal bool shouldPrintIntermediaries;         // print swaps and steps before completion
        static internal int howManyTimesRun = 1;                // how many times each puzzel should be solved. Results in more consistent avarage times
        static internal bool shouldWaitAfterIntermediateStep;   // wait after an intermediate step

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

        #region https://stackoverflow.com/a/52339246
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadConsoleOutputCharacter(
            IntPtr hConsoleOutput,
            [Out] StringBuilder lpCharacter,
            uint length,
            COORD bufferCoord,
            out uint lpNumberOfCharactersRead);

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        public static char ReadCharacterAt(int x, int y)
        {
            IntPtr consoleHandle = GetStdHandle(-11);
            if (consoleHandle == IntPtr.Zero)
            {
                return '\0';
            }
            COORD position = new COORD
            {
                X = (short)x,
                Y = (short)y
            };
            StringBuilder result = new StringBuilder(1);
            uint read = 0;
            if (ReadConsoleOutputCharacter(consoleHandle, result, 1, position, out read))
            {
                return result[0];
            }
            else
            {
                return '\0';
            }
        }
        public static char ReadCursorChar()
        {
            return ReadCharacterAt(Console.CursorLeft, Console.CursorTop);
        }
        #endregion
    }
}
