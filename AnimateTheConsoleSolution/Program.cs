﻿using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;

namespace AnimateTheConsole
{
    public class Program
    {
        private static int windowWidth;
        private static int windowHeight;
        private static string splitString = "|";

        private static Dictionary<string, string> extraChars = new Dictionary<string, string>()
        {

        };
        private static void Main(string[] args)
        {
            GetFunChars();
            Console.CursorSize = 1;
            //SetTerminalFullScreen();
            Thread.Sleep(10);
            windowWidth = Console.WindowWidth;
            windowHeight = Console.WindowHeight;

            string name = "SixOfCrows";
            string solutionFolderPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString()).ToString();
            string imagesFolderPath = solutionFolderPath + @"\images\" + name;
            string outputFolderPath = solutionFolderPath + @"\image-to-ascii-output\" + name + "Frames";
            string asciiFolderPath = solutionFolderPath + @"\image-to-ascii-output\" + name + "Frames";

            BrightnessSettings sixCrowSettings = MakeBrightnessSetting(.10F, .21F, .32F, .43F);
            BrightnessSettings hadesSettings = MakeBrightnessSetting(.10F, .25F, .50F, .85F);

            ConvertImagesToAscii(imagesFolderPath, outputFolderPath, name, sixCrowSettings);

            Console.Write("Shading Example : Hades Animation");
            Console.ReadKey(true);
            Console.Clear();
            name = "Hades";
            asciiFolderPath = solutionFolderPath + @"\image-to-ascii-output\" + name + "Frames";
            ReadAsciiFromFolder(asciiFolderPath);

            Console.Write("Anti Aliasing Example : Six Of Crows Animation");
            Console.ReadKey(true);
            Console.Clear();
            name = "SixOfCrows";
            asciiFolderPath = solutionFolderPath + @"\image-to-ascii-output\" + name + "Frames";
            ReadAsciiFromFolder(asciiFolderPath);
        }
        private static void ReadAsciiFromFolder(string asciiFolderPath)
        {
            List<string> asciiFileList = new List<string>();
            List<string> frames = new List<string>();
            asciiFileList.AddRange(Directory.GetFiles(asciiFolderPath, "*.txt"));

            foreach (string filePath in asciiFileList)
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    frames.Add(sr.ReadToEnd().Replace(splitString, "\n"));
                }
            }
            foreach (string frame in frames)
            {
                Console.Write(frame);
                //WriteScreenBarriers();
                Thread.Sleep(50);
                Console.Clear();
                Console.SetCursorPosition(0, 0);
            }
        }

        struct BrightnessSettings
        {
            public float BlankThreshold;
            public float LightThreshold;
            public float MediumThreshold;
            public float DarkThreshold;
        }
        private static BrightnessSettings MakeBrightnessSetting(float blank, float light, float medium, float dark)
        {
            BrightnessSettings bs = new BrightnessSettings();
            bs.BlankThreshold = blank;
            bs.LightThreshold = light;
            bs.MediumThreshold = medium;
            bs.DarkThreshold = dark;
            return bs;
        }
        private static void ConvertImagesToAscii(string imagesFolderPath, string outputFolderPath, string name, BrightnessSettings bs)
        {
            int count = 0;
            string displayZeroes = "";
            List<string> imageFileArray = new List<string>();
            string[] foldersInImagePath = Directory.GetDirectories(imagesFolderPath);
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            for (int i = 0; i < foldersInImagePath.Length; i++)
            {
                imageFileArray.AddRange(Directory.GetFiles($"{foldersInImagePath[i]}", "*.jpg"));
            }
            //find the decimal place of frames we're working with and modify displayZeroes accordingly
            string fileCount = imageFileArray.Count.ToString();
            for (int i = 0; i < fileCount.Length - 1; i++)
            {
                displayZeroes += "0";
            }
            foreach (string imageFile in imageFileArray)
            {
                string imageText = ConvertImageToAsciiAntiAliasing(imageFile, bs);
                //string imageText = ConvertImageToAsciiShading(imageFile, bs);

                string outputFilePath = Path.Combine(outputFolderPath, $"{name}{displayZeroes}{count}.txt");
                using (StreamWriter sw = new StreamWriter(outputFilePath))
                {
                    sw.Write(imageText);
                }

                count++;
                if (count < 10)
                {
                    displayZeroes = "00";
                }
                else if (count < 100)
                {
                    displayZeroes = "0";
                }
                else
                {
                    displayZeroes = "";
                }

                Console.WriteLine($"{displayZeroes}{count} / {fileCount}");
            }
        }
        private static string ConvertImageToAsciiShading(string imageFile, BrightnessSettings bs)
        {
            string imageText = "";
            Image image = Image.FromFile(imageFile);
            Bitmap bm = new Bitmap(image, windowWidth, windowHeight);
            for (int y = 0; y < windowHeight; y++)
            {
                if (y > 0) { imageText += splitString; }
                for (int x = 0; x < windowWidth; x++)
                {
                    Color thisPixel = bm.GetPixel(x, y);
                    float brightness = thisPixel.GetBrightness();
                    if (brightness <= bs.BlankThreshold)
                    {
                        imageText += " ";
                    }
                    else if (brightness <= bs.LightThreshold)
                    {
                        //imageText += extraChars["LIGHTSHADE"];
                        imageText += ".";
                    }
                    else if (brightness <= bs.MediumThreshold)
                    {
                        //imageText += extraChars["MEDIUMSHADE"];
                        imageText += ":";
                    }
                    else if (brightness <= bs.DarkThreshold)
                    {
                        //imageText += extraChars["DARKSHADE"];
                        imageText += "+";
                    }
                    else
                    {
                        //imageText += extraChars["FULLBLOCK"];
                        imageText += "8";
                    }
                }
            }
            return imageText;
        }
        private static string ConvertImageToAsciiAntiAliasing(string imageFile, BrightnessSettings bs)
        {
            string imageText = "";
            Image image = Image.FromFile(imageFile);
            Bitmap bm = new Bitmap(image);

            //base height and width in pixels of a given pixel grid
            int pixelGridWidth = bm.Width / windowWidth;
            int pixelGridHeight = bm.Height / windowHeight;

            //Account for remaining pixels that would otherwise be cut off, used to determine current width and height.
            float pixelWidthRemainder = (bm.Width % windowWidth) / (float)windowWidth;
            float pixelHeightRemainder = (bm.Height % windowHeight) / (float)windowHeight;

            float remainderWidthCheck = 0.0F;
            float remainderHeightCheck = 0.0F;

            float topLeftQuadrant = 0F;
            float topRightQuadrant = 0F;
            float bottomLeftQuadrant = 0F;
            float bottomRightQuadrant = 0F;

            float threshold = bs.LightThreshold;

            int currentGridWidth = pixelGridWidth;
            int currentGridHeight = pixelGridHeight;

            //Inside the entire image
            for (int y = 0; y < bm.Height; y += currentGridHeight)
            {
                remainderHeightCheck += pixelHeightRemainder;
                currentGridHeight = pixelGridHeight + (int)remainderHeightCheck;

                if (y > 0) { imageText += splitString; }

                for (int x = 0; x < bm.Width; x += currentGridWidth)
                {
                    remainderWidthCheck += pixelWidthRemainder;
                    currentGridWidth = pixelGridWidth + (int)remainderWidthCheck;

                    //number of pixels in this quadrant
                    int pixelQuadrantCount = currentGridWidth * currentGridHeight / 4;

                    //Inside a single pixelGrid
                    for (int i = 0; i < currentGridWidth; i++)
                    {
                        for (int j = 0; j < currentGridHeight; j++)
                        {
                            Color thisPixel = bm.GetPixel(x + i, y + j);
                            if (i <= currentGridWidth / 2 && j <= currentGridHeight / 2)
                            {
                                //top left
                                topLeftQuadrant += thisPixel.GetBrightness();
                            }
                            if (i > currentGridWidth / 2 && j <= currentGridHeight / 2)
                            {
                                //top right
                                topRightQuadrant += thisPixel.GetBrightness();
                            }
                            if (i > currentGridWidth / 2 && j > currentGridHeight / 2)
                            {
                                //bottom right
                                bottomRightQuadrant += thisPixel.GetBrightness();
                            }
                            if (i <= currentGridWidth / 2 && j > currentGridHeight / 2)
                            {
                                //bottom left
                                bottomLeftQuadrant += thisPixel.GetBrightness();
                            }
                        }
                    }
                    if(remainderWidthCheck > 1.0F)
                    {
                        remainderWidthCheck -= 1.0f;
                    }
                    if (remainderHeightCheck > 1.0F)
                    {
                        remainderHeightCheck -= 1.0f;
                    }

                    //average brightness of each quadrant
                    topLeftQuadrant /= pixelQuadrantCount;
                    topRightQuadrant /= pixelQuadrantCount;
                    bottomLeftQuadrant /= pixelQuadrantCount;
                    bottomRightQuadrant /= pixelQuadrantCount;

                    //average brightness of entire grid
                    float gridAverageBrightness = (topLeftQuadrant + topRightQuadrant + bottomLeftQuadrant + bottomRightQuadrant) / 4.0F;


                    if (topLeftQuadrant < threshold && topRightQuadrant < threshold && bottomLeftQuadrant < threshold && bottomRightQuadrant < threshold)
                    {
                        imageText += " ";
                    }
                    else if (topLeftQuadrant >= threshold && topRightQuadrant < threshold && bottomLeftQuadrant >= threshold && bottomRightQuadrant >= threshold)
                    {
                        imageText += 'b';
                    }
                    else if (topLeftQuadrant >= threshold && topRightQuadrant < threshold && bottomLeftQuadrant < threshold && bottomRightQuadrant < threshold)
                    {
                        imageText += "`";
                    }
                    else if (topLeftQuadrant < threshold && topRightQuadrant >= threshold && bottomLeftQuadrant < threshold && bottomRightQuadrant < threshold)
                    {
                        imageText += "'";
                    }
                    else if (topLeftQuadrant >= threshold && topRightQuadrant >= threshold && bottomLeftQuadrant < threshold && bottomRightQuadrant < threshold)
                    {
                        imageText += '"';
                    }
                    else if (topLeftQuadrant < threshold && topRightQuadrant < threshold && bottomLeftQuadrant >= threshold && bottomRightQuadrant < threshold)
                    {
                        imageText += ",";
                    }
                    else if (topLeftQuadrant < threshold && topRightQuadrant < threshold && bottomLeftQuadrant < threshold && bottomRightQuadrant >= threshold)
                    {
                        imageText += ".";
                    }
                    else if (topLeftQuadrant < threshold && topRightQuadrant >= threshold && bottomLeftQuadrant >= threshold && bottomRightQuadrant >= threshold)
                    {
                        imageText += "d";
                    }
                    else if (topLeftQuadrant >= threshold && topRightQuadrant >= threshold && bottomLeftQuadrant < threshold && bottomRightQuadrant >= threshold)
                    {
                        imageText += "Y";
                    }
                    else if (topLeftQuadrant >= threshold && topRightQuadrant >= threshold && bottomLeftQuadrant >= threshold && bottomRightQuadrant < threshold)
                    {
                        imageText += "P";
                    }
                    else
                    {
                        imageText += "8";
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
                    else if (y == 0 && x == windowWidth - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["BOXTOPRIGHT"]);
                    }
                    else if (y == windowHeight - 1 && x == 0)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.Write(extraChars["BOXBOTTOMLEFT"]);
                    }
                    else if (y == windowHeight - 1 && x == windowWidth - 1)
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
            foreach (KeyValuePair<string, string> character in extraChars)
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
        private static void SetTerminalFullScreen()
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