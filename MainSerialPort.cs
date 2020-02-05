using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;

namespace SerialPortNew
{

    public enum StopBits
    {
        One = 1,
        Two = 2,
        OnePointFive = 3
    }

    public enum Format
    {
        BIN,
        HEX,
        ASCII
    }

    static public class MainSerialPort
    {
        internal static string[] BaudRate { get; } = {
            "4800",
            "9600",
            "115200",
            "Other"

        };

        internal static string[] DataBits { get; } = {
            "5",
            "6",
            "7",
            "8"
        };

        internal static Format FormatRx { get; set; } = Format.ASCII;
        internal static bool Status { get; set; } = false;
        internal static int BytesPerLine { get; set; } = 500;

        internal static void WriteCurrentSettings(SerialPort serialPort)
        {
            MainConsole.WriteLineGreen($"Current Settings");
            Console.WriteLine($"Port:           {serialPort.PortName}");
            Console.WriteLine($"BaudRate:       {serialPort.BaudRate}");
            Console.WriteLine($"Parity:         {serialPort.Parity}");
            Console.WriteLine($"DataBits:       {serialPort.DataBits}");
            Console.WriteLine($"StopBits:       {serialPort.StopBits}");
            Console.WriteLine($"Handshake:      {serialPort.Handshake}");
            Console.WriteLine($"Format Receive: {FormatRx}");
            Console.WriteLine($"Bytes per line: {BytesPerLine}");

        }

        internal static void Write(SerialPort serialPort, string command)
        {
            Format formatTx = Format.ASCII;
            if (command.Length >= 2 && command[0] == '0' && command[1] == 'x')
            {
                formatTx = Format.HEX;
                command = command.Substring(2);
            }
            switch (formatTx)
            {
                case Format.HEX:
                    string[] commandSplit = command.Split(' ');
                    List<byte> list = new List<byte>();
                    foreach (string hex in commandSplit)
                    {
                        if ((hex.Length <= 2)
                            && int.TryParse(hex, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out int result))
                        {
                            list.Add((byte)result);
                        }
                        else
                        {
                            formatTx = Format.ASCII;
                            break;
                        }
                    }
                    serialPort.Write(list.ToArray(), 0, list.Count);
                    break;
                case Format.ASCII:
                    serialPort.Write(command);
                    break;
                default:
                    break;
            }

        }

        internal static void Read(SerialPort serialPort)
        {
            Status = true;
            serialPort.DiscardInBuffer();
            MainConsole.WriteLineGreen("Start Monitor");
            int cnt = 0;
            while (Status)
            {
                try
                {
                    int value = serialPort.ReadByte();
                    if (++cnt > BytesPerLine)
                    {
                        Console.WriteLine();
                        cnt = 0;
                    }
                    switch (FormatRx)
                    {
                        case Format.BIN:
                            Console.Write(Convert.ToString(value, 2) + " ");
                            break;
                        case Format.HEX:
                            Console.Write($"{value:X} ");
                            break;
                        case Format.ASCII:
                            Console.Write((char)value);
                            break;
                        default:
                            break;
                    }
                }
                catch (TimeoutException)
                {

                }
            }
            MainConsole.WriteLineRed("Stop Monitor");
            serialPort.Close();
        }
    }
}
