using AnimateTheConsole.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnimateTheConsole.Core
{

    public static class AsciiDisplay
    {
        private static Vector2 screenWidthHeight;
        public const int FontSizeDefault = 21;
        private static int DisplayCount { get; set; }
        private static int DisplayPlaceLimit { get; set; }
        private static string DisplayZeroes { get; set; }

        public static void DisplayAscii(AsciiFileIO fileIO, bool isFullScreen = false)
        {
            Console.Clear();
            List<string> frames = fileIO.ReadAsciiFromFolder();
            int asciiWidth = frames[0].Split("\n")[0].Length;
            int asciiHeight = frames[0].Split("\n").Length;
            int widthBuffer = Console.WindowWidth - asciiWidth;
            int heightBuffer = Console.WindowHeight - asciiHeight;
            if (isFullScreen)
            {
                screenWidthHeight = SetTerminalFullScreen();

                DetermineFontSize(asciiWidth, asciiHeight);
                widthBuffer = Console.LargestWindowWidth - asciiWidth;
                heightBuffer = Console.LargestWindowHeight - asciiHeight;
            }
            if (frames.Count > 2)
            {
                foreach (string frame in frames)
                {
                    Console.SetCursorPosition(0, heightBuffer / 2);
                    foreach (string line in frame.Split("\n"))
                    {
                        Console.WriteLine(new string(' ', widthBuffer / 2) + line);
                    }
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        Console.ReadKey(true);
                    }
                    Thread.Sleep(50);
                }
            }
            else
            {
                Console.SetCursorPosition(0, heightBuffer / 2);
                foreach (string line in frames[0].Split("\n"))
                {
                    Console.WriteLine(new string(' ', widthBuffer / 2) + line);
                }
                Console.ReadKey(true);
            }
            Console.Clear();
            if (isFullScreen)
            {
                SetFontSize(FontSizeDefault);
                Console.SetBufferSize(Console.WindowLeft + Console.LargestWindowWidth, Console.WindowTop + Console.LargestWindowHeight);
            }
        }

        private static void DetermineFontSize(int asciiWidth, int asciiHeight)
        {
            //We need to set the font size first, depending on the height and width of the ascii image, then alter the buffer size to fit.
            //Font size needs to be determined via a relationship between height and width of ascii image and screen resolution
            //(reverse engineer how LargestWindowHeight and Width are calculated)
            int fontHeight = (int)((screenWidthHeight.Y / asciiHeight) % 1 == 0 ? screenWidthHeight.Y / asciiHeight : screenWidthHeight.Y / asciiHeight - 1);
            int fontWidth = (int)((screenWidthHeight.X / asciiWidth) % 1 == 0 ? screenWidthHeight.X / asciiWidth : screenWidthHeight.X / asciiWidth - 1);
            SetFontSize((short)fontHeight);

            if (asciiWidth > Console.LargestWindowWidth) { SetFontSize((short)(fontWidth * 2)); }

            Console.SetBufferSize(Console.WindowLeft + Console.LargestWindowWidth, Console.WindowTop + Console.LargestWindowHeight);
        }

        public static void ResetDisplayCount(int length)
        {
            DisplayCount = 0;
            DisplayPlaceLimit = 10;
            DisplayZeroes = new string('0', length - 1);
            Console.Clear();
        }

        public static void IncrementDisplayCount(string message, int fileCount)
        {
            DisplayCount++;
            if (!(DisplayCount < DisplayPlaceLimit))
            {
                if (DisplayZeroes.Length > 0)
                {
                    DisplayZeroes = DisplayZeroes.Substring(0, DisplayZeroes.Length - 1);
                }
                DisplayPlaceLimit *= 10;
            }
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(message);
            Console.Write($"{DisplayZeroes}{DisplayCount} / {fileCount}");
        }

        //Method to set the terminal window to full screen
        struct WindowDimensions
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }  // Structure used by GetWindowRect
        public static Vector2 SetTerminalFullScreen()
        {
            // Import the necessary functions from user32.dll
            [DllImport("user32.dll")]
            static extern IntPtr GetForegroundWindow();
            [DllImport("user32.dll")]
            static extern bool ShowWindow(IntPtr windowHandle, int isItShowing);
            [DllImport("user32.dll")]
            static extern bool GetWindowRect(IntPtr windowHandle, out WindowDimensions dimensions);
            [DllImport("user32.dll")]
            static extern bool MoveWindow(IntPtr windowHandle, int left, int top, int newWidth, int newHeight, bool Repaint);

            // Constants for the ShowWindow function
            const int SW_MAXIMIZE = 3;
            // Get the handle of the console window
            IntPtr consoleWindowHandle = GetForegroundWindow();
            // Maximize the console window
            ShowWindow(consoleWindowHandle, SW_MAXIMIZE);
            // Get the screen size
            WindowDimensions screenRect;
            GetWindowRect(consoleWindowHandle, out screenRect);
            // Resize and reposition the console window to fill the screen
            int width = screenRect.Right - screenRect.Left;
            int height = screenRect.Bottom - screenRect.Top;
            //Sets the window to the left and top of the screen, then sets the new width and height to what was just calculated, and then says yes, recreate the window to match this information

            MoveWindow(consoleWindowHandle, screenRect.Left, screenRect.Top, width, height, true);

            return new Vector2(width, height);
        }

        //All this stuff is for font manipulation
        private enum StdHandle
        {
            OutputHandle = -11
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;

            public COORD(short X, short Y)
            {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct CONSOLE_FONT_INFO_EX
        {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] // Edit sizeconst if the font name is too big
            public string FaceName;
        }
        public static void SetFontSize(short fontHeight)
        {
            [DllImport("kernel32")]
            static extern IntPtr GetStdHandle(StdHandle index);
            IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern Int32 SetCurrentConsoleFontEx(IntPtr ConsoleOutput, bool MaximumWindow, ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);
            [DllImport("kernel32.dll", SetLastError = true)]
            static extern Int32 GetCurrentConsoleFontEx(IntPtr ConsoleOutput, bool MaximumWindow, ref CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx);

            // Instantiating CONSOLE_FONT_INFO_EX and setting its size (the function will fail otherwise)
            CONSOLE_FONT_INFO_EX ConsoleFontInfo = new CONSOLE_FONT_INFO_EX();
            ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);

            // Optional, implementing this will keep the fontweight and fontsize from changing
            GetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref ConsoleFontInfo);

            ConsoleFontInfo.dwFontSize.Y = fontHeight;

            SetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref ConsoleFontInfo);

            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        }

        private static Dictionary<string, string> extraChars = new Dictionary<string, string>();
        private static void GetFunChars()
        {
            Encoding encode = Encoding.UTF8;
            extraChars["FULLBLOCK"] = "E29688";
            extraChars["BOXHORIZONTAL"] = "E29590";
            extraChars["BOXVERTICAL"] = "E29591";
            extraChars["BOXTOPRIGHT"] = "E29597";
            extraChars["BOXBOTTOMRIGHT"] = "E2959D";
            extraChars["BOXTOPLEFT"] = "E29594";
            extraChars["BOXBOTTOMLEFT"] = "E2959a";
            extraChars["LIGHTSHADE"] = "E29691";
            extraChars["MEDIUMSHADE"] = "E29692";
            extraChars["DARKSHADE"] = "E29693";
            foreach (KeyValuePair<string, string> character in extraChars)
            {
                byte[] byteArray = ConvertHexStringToByteArray(character.Value);
                extraChars[character.Key] = encode.GetString(byteArray);
            }
        }

        //Method to convert the UTF-8 hex codes into byte arrays that can be converted into our fun characters
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            // Make sure hexString is an even length
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException($"The binary key cannot have an odd number of digits: {hexString}");
            }
            // Every two characters in hexString must be converted into a byte and added to the byte array
            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber);
            }
            return data;
        }
    }
}
