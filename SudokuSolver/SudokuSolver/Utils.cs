using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    static internal class Utils
    {
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

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
