using System;
using System.Text;

namespace SerialPortNew
{
    public static class MainConsole
    {
        public static string ReadLine()
        {
            StringBuilder sb = new StringBuilder();

            bool flagCycle = true;
            while (flagCycle)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        flagCycle = false;
                        break;
                    case ConsoleKey.Backspace:
                        if (sb.Length != 0)
                        {
                            sb.Length--;
                        }
                        break;
                    default:
                        sb.Append(keyInfo.KeyChar);
                        break;
                }
            }
            return sb.ToString();
        }

        public static void WriteLineGreen(string str) => WriteLine(str, ConsoleColor.Green);
        public static void WriteLineRed(string str) => WriteLine(str, ConsoleColor.Red);
        public static void WriteLineYellow(string str) => WriteLine(str, ConsoleColor.DarkYellow);

        private static void WriteLine(string str, ConsoleColor consoleColor)
        {
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(str);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
