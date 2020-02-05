using SerialPortNew.Properties;
using System;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

namespace SerialPortNew
{
    class Program
    {
        static SerialPort serialPort;

        static void Main()
        {
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

            string portName = Settings.Default.PortName;
            string[] portNames = SerialPort.GetPortNames();

            if (portNames.Length == 0)
            {
                MainConsole.WriteLineRed($"Serial device not detected!");
                Console.WriteLine($"Press Enter to exit");
                Console.ReadLine();
                return;
            }

            if (!portNames.Contains(portName))
            {
                portName = portNames[0];
            }

            serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = Settings.Default.BaudRate,
                Parity = Settings.Default.Parity,
                DataBits = Settings.Default.DataBits,
                StopBits = (System.IO.Ports.StopBits)Settings.Default.StopBits,
                Handshake = Settings.Default.Handshake,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            MainSerialPort.WriteCurrentSettings(serialPort);

            while (true)
            {
                string command = MainConsole.ReadLine();

                if (stringComparer.Equals("start", command))
                {
                    if (serialPort.IsOpen)
                    {

                    }
                    else
                    {
                        try
                        {
                            serialPort.Open();
                            Task.Run(() => MainSerialPort.Read(serialPort));
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MainConsole.WriteLineRed("Serial port busy!");
                        }
                    }
                }
                else if (stringComparer.Equals("stop", command))
                {
                    MainSerialPort.Status = false;
                }
                else if (stringComparer.Equals("quit", command))
                {
                    return;
                }
                else if (stringComparer.Equals("clear", command))
                {
                    Console.Clear();
                }
                else if (stringComparer.Equals("settings", command))
                {
                    if (serialPort.IsOpen)
                    {
                        MainConsole.WriteLineRed("Stop transmission first");
                    }
                    else
                    {
                        SetSettings();
                        MainSerialPort.WriteCurrentSettings(serialPort);
                    }
                }
                else
                {
                    if (serialPort.IsOpen)
                    {
                        MainConsole.WriteLineYellow(command);
                        MainSerialPort.Write(serialPort, command);
                    }
                }
            }
        }

        private static void SetSettings()
        {
            string[] strs;
            int ind;

            strs = SerialPort.GetPortNames();
            if (strs.Length == 0)
            {
                MainConsole.WriteLineRed($"Serial device not detected!");
                Console.ReadLine();
                return;
            }
            ind = Array.IndexOf(strs, serialPort.PortName);
            serialPort.PortName = SetSetting(strs, "Ports", ind);

            strs = MainSerialPort.BaudRate;
            ind = Array.IndexOf(strs, Convert.ToString(serialPort.BaudRate));
            if (ind < 0)
            {
                ind = strs.Length - 1;
            }
            string str = SetSetting(strs, "BaudRates", ind);
            if (int.TryParse(str, out int result))
            {
                serialPort.BaudRate = result;
            }
            else
            {
                MainConsole.WriteLineGreen("Enter BaudRate value:");
                ind = 3;
                while (ind > 0)
                {
                    str = Console.ReadLine();
                    if (int.TryParse(str, out result))
                    {
                        serialPort.BaudRate = result;
                        break;
                    }
                    else if (str == "")
                    {
                        break;
                    }
                    else
                    {
                        MainConsole.WriteLineRed("Enter CORRECT BaudRate value:");
                        ind--;
                    }
                }
            }

            strs = Enum.GetNames(typeof(Parity));
            ind = Array.IndexOf(strs, Convert.ToString(serialPort.Parity));
            str = SetSetting(strs, "Party", ind);
            if (Enum.TryParse(str, true, out Parity parity))
            {
                if (Enum.IsDefined(typeof(Parity), parity))
                {
                    serialPort.Parity = parity;
                }
            }

            strs = MainSerialPort.DataBits;
            ind = Array.IndexOf(strs, Convert.ToString(serialPort.DataBits));
            str = SetSetting(strs, "DataBits", ind);
            if (int.TryParse(str, out result))
            {
                serialPort.DataBits = result;
            }


            strs = Enum.GetNames(typeof(StopBits));
            ind = Array.IndexOf(strs, Convert.ToString(serialPort.StopBits));
            str = SetSetting(strs, "StopBits", ind);
            if (Enum.TryParse(str, true, out System.IO.Ports.StopBits stopBits))
            {
                if (Enum.IsDefined(typeof(System.IO.Ports.StopBits), stopBits))
                {
                    serialPort.StopBits = stopBits;
                }
            }


            strs = Enum.GetNames(typeof(Handshake));
            ind = Array.IndexOf(strs, Convert.ToString(serialPort.Handshake));
            str = SetSetting(strs, "Handshake", ind);
            if (Enum.TryParse(str, true, out Handshake handshake))
            {
                if (Enum.IsDefined(typeof(Handshake), handshake))
                {
                    serialPort.Handshake = handshake;
                }
            }

            strs = Enum.GetNames(typeof(Format));
            ind = Array.IndexOf(strs, Convert.ToString(MainSerialPort.FormatRx));
            str = SetSetting(strs, "Format Receive", ind);
            if (Enum.TryParse(str, true, out Format formatRx))
            {
                if (Enum.IsDefined(typeof(Format), formatRx))
                {
                    MainSerialPort.FormatRx = formatRx;
                }
            }

            MainConsole.WriteLineGreen("Enter number of bytes per line:");
            ind = 3;
            while (ind > 0)
            {
                str = Console.ReadLine();
                if (int.TryParse(str, out result))
                {
                    MainSerialPort.BytesPerLine = (result > 500) ? 500 : result;
                    break;
                }
                else if (str == "")
                {
                    break;
                }
                else
                {
                    MainConsole.WriteLineRed("Enter CORRECT number of bytes per line:");
                    ind--;
                }
            }
            Console.Clear();

            Settings.Default.PortName = serialPort.PortName;
            Settings.Default.BaudRate = serialPort.BaudRate;
            Settings.Default.Parity = serialPort.Parity;
            Settings.Default.DataBits = serialPort.DataBits;
            Settings.Default.StopBits = (StopBits)serialPort.StopBits;
            Settings.Default.Handshake = serialPort.Handshake;
            Settings.Default.Format = MainSerialPort.FormatRx;
            Settings.Default.BytesPerLine = MainSerialPort.BytesPerLine;

            Settings.Default.Save();
        }

        static string SetSetting(
                string[] settings,
                string nameSetting = "Parametrs",
                int currentSettingNum = 0,
                string comment = "")
        {
            Console.CursorVisible = false;
            Console.Clear();

            MainConsole.WriteLineGreen($"Available {nameSetting}:");
            Console.WriteLine(comment);
            int strPos = 0;
            int cursorTopInit = Console.CursorTop;
            foreach (string str in settings)
            {
                Console.WriteLine($"[{strPos++}] {str}");
            }

            strPos = currentSettingNum;
            Console.CursorTop = cursorTopInit + strPos;

            HighlightStr(settings, strPos, strPos);
            bool selectRun = true;


            while (selectRun)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        int strPosNew = (strPos == 0) ? (settings.Length - 1) : (strPos - 1);
                        HighlightStr(settings, strPos, strPosNew);
                        strPos = strPosNew;
                        break;
                    case ConsoleKey.DownArrow:
                        strPosNew = (strPos == settings.Length - 1) ? (0) : (strPos + 1);
                        HighlightStr(settings, strPos, strPosNew);
                        strPos = strPosNew;
                        break;
                    case ConsoleKey.Enter:
                        selectRun = false;
                        break;
                    default:
                        break;
                }
            }

            Console.CursorVisible = true;
            Console.Clear();
            return settings[strPos];
        }

        static void HighlightStr(string[] vs, int strPosOld, int strPosNew)
        {
            Console.Write($"[{strPosOld}] {vs[strPosOld]}\r");
            Console.CursorTop += (strPosNew - strPosOld);
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write($"[{strPosNew}] {vs[strPosNew]}\r");
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
