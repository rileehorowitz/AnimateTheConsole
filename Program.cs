using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;

namespace AnimateTheConsole
{
    internal class Program
    {
       
       

        private static int windowWidth;
        private static int windowHeight;

        private static Dictionary<string, string> extraChars = new Dictionary<string, string>() 
        { 
        
        };
       
        private static void Main(string[] args)
        {
            GetFunChars();
            SetTerminalWindowSize();
            windowWidth = Console.WindowWidth;
            windowHeight = Console.WindowHeight;

            string imagesFolderPath = @"C:\Users\Student\workspace\rilee-horowitz-side-projects\AnimateTheConsole\images";
            string outputFolderPath = @"C:\Users\Student\workspace\rilee-horowitz-side-projects\AnimateTheConsole\image-to-ascii-output";

            //ConvertImagesToAscii(imagesFolderPath, outputFolderPath);

            string asciiFolderPath = @"C:\Users\Student\workspace\rilee-horowitz-side-projects\AnimateTheConsole\image-to-ascii-output\SixOfCrowsFrames";
            ReadAsciiFromFolder(asciiFolderPath);

            char[,] gridContent = new char[windowWidth, windowHeight];

            //WriteScreenBarriers();
            //RunBlockThroughScreen();

            Console.ReadKey(true);
        }

        private static void ReadAsciiFromFolder(string asciiFolderPath)
        {
            List<string> asciiFileArray = new List<string>();
            List<string> frames = new List<string>();
            asciiFileArray.AddRange(Directory.GetFiles(asciiFolderPath, "*.txt"));

            foreach(string filePath in asciiFileArray)
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string frameText = sr.ReadToEnd();
                    string[] frameLines = frameText.Split("split");
                    for(int i = 0; i < frameLines.Length; i++)
                    {
                        Console.Write($"{frameLines[i]}");
                        if(i < frameLines.Length - 1)
                        {
                            Console.WriteLine();
                        }
                    }
                    Thread.Sleep(50);
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                }
            }
        }

        private static void ConvertImagesToAscii(string imagesFolderPath, string outputFolderPath)
        {
            int count = 0;
            string displayZeroes = "00";
            List<string> imageFileArray = new List<string>();
            for (int i = 1; i <= 4; i++)
            {
                imageFileArray.AddRange(Directory.GetFiles($"{imagesFolderPath}{i}", "*.jpg"));
            }
            foreach (string imageFile in imageFileArray)
            {
                string imageText = ConvertImageToAscii(imageFile);
                string outputFilePath = Path.Combine(outputFolderPath, $"SixOfCrowsFrame{displayZeroes}{count}.txt");
                using (StreamWriter sw = new StreamWriter(outputFilePath))
                {
                    sw.Write(imageText);
                }
                count++;
                if(count < 10)
                {
                    displayZeroes = "00";
                }else if(count < 100)
                {
                    displayZeroes = "0";
                }
                else
                {
                    displayZeroes = "";
                }
                Console.Write(imageText);
            }
        }

        private static string ConvertImageToAscii(string imageFile)
        {
            string imageText = "";
            Image image = Image.FromFile(imageFile);
            Bitmap bm = new Bitmap(image, windowWidth / 2, windowHeight);
            for (int y = 0; y < windowHeight; y++)
            {
                if (y > 0) { imageText += "split"; }
                for (int x = 0; x < windowWidth / 2; x++)
                {
                    Color thisPixel = bm.GetPixel(x, y);
                    float brightness = thisPixel.GetBrightness();
                    if (brightness <= 0.20F)
                    {
                        imageText += " ";
                    }
                    else if (brightness <= 0.40F)
                    {
                        imageText += extraChars["LIGHTSHADE"];
                    }
                    else if (brightness <= 0.60F)
                    {
                        imageText += extraChars["MEDIUMSHADE"];
                    }
                    else if (brightness <= 0.80F)
                    {
                        imageText += extraChars["DARKSHADE"];
                    }
                    else
                    {
                        imageText += extraChars["FULLBLOCK"];
                    }
                } 
            }
            return imageText;
        }

        private static void RunBlockThroughScreen()
        {
            for (int y = 1; y < windowHeight - 1; y++)
            {
                for (int x = 1; x < windowWidth; x++)
                {
                    if (x > 1)
                    {
                        Console.SetCursorPosition(x - 1, y);
                        Console.Write(' ');
                    }
                    if (x < windowWidth - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["FULLBLOCK"]);
                    }
                    Thread.Sleep(10);
                }
            }
        }

        private static void WriteScreenBarriers()
        {
            for (int y = 0; y < windowHeight; y++)
            {
                for (int x = 0; x < windowWidth; x++)
                {
                    if (y == 0 && x == 0)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["BOXTOPLEFT"]);
                    }
                    else if(y == 0 && x == windowWidth - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["BOXTOPRIGHT"]);
                    }
                    else if(y == windowHeight-1  && x == 0)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["BOXBOTTOMLEFT"]);
                    }
                    else if(y == windowHeight-1  && x == windowWidth - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["BOXBOTTOMRIGHT"]);
                    }
                    else if (y == 0 || y == windowHeight - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["BOXHORIZONTAL"]);
                    }
                    else if (x == 0 || x == windowWidth - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["BOXVERTICAL"]);
                    }
                }
            }
        }

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

            foreach (KeyValuePair<string,string> character in extraChars)
            {
                byte[] byteArray = ConvertHexStringToByteArray(character.Value);
                extraChars[character.Key] = encode.GetString(byteArray);
            }
        }

        // Structure used by GetWindowRect
        struct WindowDimensions
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        //Method to set the terminal window to full screen
        private static void SetTerminalWindowSize()
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
            //... And then this part spits out the current dimensions in to our struct object via the "out" bit there
            GetWindowRect(consoleWindowHandle, out screenRect);
            // Resize and reposition the console window to fill the screen
            int width = screenRect.Right - screenRect.Left;
            int height = screenRect.Bottom - screenRect.Top;
            //And this one here picks our console window, sets the window to the left and top of the screen, then sets the new width and height to what was just calculated, and then says yes, recreate the window to match this information
            MoveWindow(consoleWindowHandle, screenRect.Left, screenRect.Top, width, height, true);
        }

        //Method to convert the UTF-8 hex codes into byte arrays that can be converted into our fun characters
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            // Make sure hexString is an even length
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException( $"The binary key cannot have an odd number of digits: {hexString}");
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