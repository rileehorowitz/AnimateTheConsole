using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Numerics;

namespace AnimateTheConsole
{
    public class Program
    {
        /*
        ,s888s.
      ,d888H8888b.
     d888$8888@88b
    (HH88HH@HSH$HH)
    88@8888H8888888
    `Y88$88000H88P'
      `*Y888@8P*'
         `***'
        ,oOOOo.
      ,dOOOO/OOb.
     dOO\IOOO\OOOb
    {O\OIOIOIOOIIO}
    OOOO/OOOI/OOOOO
    `+OOI/OOOO\OO+'
      `"+OOOOO+"'
         `"""'
        ,+!!!+.
      ,<!!!!!!!>.
     <!!!!!!!!!!!>
    [!!!!!!!!!!!!!]
    !!!!!!!!!!!!!!!
    `<!!!!!!!!!!!>'
      `"<!!!!!>"'
         `"""'
        ,:::::.
      ,;::::::::.
     ;::::::::::::
    !:::::::::::::!
    :::::::::::::::
    `::::::::::::;'
      `'::::::;''
         `"""'
   */
        private static int windowWidth;
        private static int windowHeight;
        private static string splitString = "|";
        private static Vector2 screenWidthHeight;
        private const int FontSizeDefault = 21;
        private static Dictionary<string, string> FullValues = new Dictionary<string, string>()
        {
            {"FILL", "8" },
            {"TOP", "*" },
            {"BOTTOM", "s" },
            {"LEFT", ")" },
            {"RIGHT", "(" },
            {"UPRIGHT", "P" },
            {"UPLEFT", "Y" },
            {"DOWNRIGHT", "b" },
            {"DOWNLEFT", "d" },
            {"TOPLEFT", "'" },
            {"BOTTOMLEFT", "." },
            {"TOPRIGHT", "`" },
            {"BOTTOMRIGHT", "," }
        };
        private static Dictionary<string, string> DarkValues = new Dictionary<string, string>() 
        {
            {"FILL", "O" },
            {"TOP", "\"" },
            {"BOTTOM", "o" },
            {"LEFT", "}" },
            {"RIGHT", "{" },
            {"UPRIGHT", "+" },
            {"UPLEFT", "+" },
            {"DOWNRIGHT", "b" },
            {"DOWNLEFT", "d" },
            {"TOPLEFT", "'" },
            {"BOTTOMLEFT", "." },
            {"TOPRIGHT", "`" },
            {"BOTTOMRIGHT", "," }
        };
        private static Dictionary<string, string> MediumValues = new Dictionary<string, string>()
        {
            {"FILL", "!" },
            {"TOP", "\"" },
            {"BOTTOM", "+" },
            {"LEFT", "]" },
            {"RIGHT", "[" },
            {"UPRIGHT", ">" },
            {"UPLEFT", "<" },
            {"DOWNRIGHT", ">" },
            {"DOWNLEFT", "<" },
            {"TOPLEFT", "'" },
            {"BOTTOMLEFT", "." },
            {"TOPRIGHT", "`" },
            {"BOTTOMRIGHT", "," }
        };
        private static Dictionary<string, string> LightValues = new Dictionary<string, string>() 
        {
            {"FILL", ":" },
            {"TOP", "\"" },
            {"BOTTOM", ":" },
            {"LEFT", "!" },
            {"RIGHT", "!" },
            {"UPRIGHT", ";" },
            {"UPLEFT", ":" },
            {"DOWNRIGHT", ":" },
            {"DOWNLEFT", ";" },
            {"TOPLEFT", "'" },
            {"BOTTOMLEFT", "." },
            {"TOPRIGHT", "`" },
            {"BOTTOMRIGHT", "," }
        };

        
        private static void Main(string[] args)
        {
            //TestCode.ConsoleBufferExample();

            screenWidthHeight = SetTerminalFullScreen();
            Console.CursorVisible = false;
            SetFontSize(21);
            //Console.SetWindowPosition(0, 0);

            int test1 = Console.BufferWidth + Console.BufferHeight;
            windowWidth = Console.LargestWindowWidth;
            windowHeight = Console.LargestWindowHeight;


            string name = "CreationOfAdam";
            string imagesFolderPath = @"..\..\..\data\images\" + name;
            string outputFolderPath = @"..\..\..\data\image-to-ascii-output\" + name + "Frames";
            string asciiFolderPath = @"..\..\..\data\image-to-ascii-output\" + name + "Frames";

            ColorMask adamMask = MakeColorMask(48.0F, 360.0F, 0.0F, 0.12F);

            BrightnessSettings sixCrowSettings = MakeBrightnessSetting(.10F, .16F, .28F, .52F);
            BrightnessSettings hadesSettings = MakeBrightnessSetting(.10F, .25F, .55F, .75F);
            BrightnessSettings adamSettings = MakeBrightnessSetting(.42F, .55F, .70F, .85F);
            BrightnessSettings coinSettings = MakeBrightnessSetting(.10F, .20F, .45F, .65F);
            BrightnessSettings defaultSettings = MakeBrightnessSetting(.20F, .40F, .60F, .80F);
            BrightnessSettings negativeSettings = MakeBrightnessSetting(.90F, .15F, .10F, .05F, true);

            //ConvertImagesToAscii(imagesFolderPath, outputFolderPath, name, adamSettings, adamMask, true);

            //ReadAsciiFromFolder(asciiFolderPath);

            ShowExample();
        }

        private static void ShowExample()
        {
            string name = "";
            string path = "";
            Console.Write("Shading and Anti Aliasing at Higher Brightness Example : Hades Animation");
            Console.ReadKey(true);
            Console.Clear();
            name = "Hades";
            path = @"..\..\..\data\image-to-ascii-output\" + name + "Frames";
            ReadAsciiFromFolder(path);

            Console.Write("Shading and Anti Aliasing at Lower Brightness Example : Six Of Crows Animation");
            Console.ReadKey(true);
            Console.Clear();
            name = "SixOfCrows";
            path = @"..\..\..\data\image-to-ascii-output\" + name + "Frames";
            ReadAsciiFromFolder(path);
        }

        private static void ReadAsciiFromFolder(string asciiFolderPath, bool isDrawingWhite = true)
        {
            float asciiWidth = 0;
            float asciiHeight = 0;
            List<string> asciiFileList = new List<string>();
            List<string> frames = new List<string>();
            asciiFileList.AddRange(Directory.GetFiles(asciiFolderPath, "*.txt"));

            foreach (string filePath in asciiFileList)
            {
                string toAdd = "";
                using (StreamReader sr = new StreamReader(filePath))
                {
                    toAdd += sr.ReadToEnd().Replace(splitString, "\n"); 
                }
                frames.Add(toAdd);
            }
            asciiWidth = frames[0].Split("\n")[0].Length;
            asciiHeight = frames[0].Split("\n").Length;

            //We need to set the font size first, depending on the height and width of the ascii image, then alter the buffer size to fit.
            //Font size needs to be determined via a relationship between height and width of ascii image and screen resolution
            //(reverse engineer how LargestWindowHeight and Width are calculated)
            int fontHeight = (int)((screenWidthHeight.Y / asciiHeight) % 1 == 0? screenWidthHeight.Y / asciiHeight : screenWidthHeight.Y / asciiHeight - 1);
            int fontWidth = (int)((screenWidthHeight.X / asciiWidth) % 1 == 0? screenWidthHeight.X / asciiWidth : screenWidthHeight.X / asciiWidth - 1);
            SetFontSize((short)fontHeight);

            if (asciiWidth > Console.LargestWindowWidth) { SetFontSize((short)(fontWidth * 2)); }
           
            Console.SetBufferSize(Console.WindowLeft + Console.LargestWindowWidth, Console.WindowTop + Console.LargestWindowHeight);

            foreach (string frame in frames)
            {
                Console.Write(frame);
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    Console.ReadKey(true);
                }
                Thread.Sleep(50);
                if(asciiFileList.Count <= 1)
                {
                    Console.ReadKey(true);
                }
                Console.SetCursorPosition(0, 0);
            }
            SetFontSize(FontSizeDefault);
            Console.SetBufferSize(Console.WindowLeft + Console.LargestWindowWidth, Console.WindowTop + Console.LargestWindowHeight);
        }

        struct BrightnessSettings
        {
            public float BlankThreshold;
            public float LightThreshold;
            public float MediumThreshold;
            public float DarkThreshold;
            public bool isPhotoNegative;
        }
        struct ColorMask
        {
            public float HueMin;
            public float HueMax;
            public float SaturationMin;
            public float SaturationsMax;
        }
        private static BrightnessSettings MakeBrightnessSetting(float blank, float light, float medium, float dark, bool isPhotoNegative = false)
        {
            BrightnessSettings bs = new BrightnessSettings();
            bs.BlankThreshold = blank;
            bs.LightThreshold = light;
            bs.MediumThreshold = medium;
            bs.DarkThreshold = dark;
            bs.isPhotoNegative = isPhotoNegative;
            return bs;
        }
        private static ColorMask MakeColorMask(float hueMin, float hueMax, float saturationMin, float saturationMax)
        {
            ColorMask cm = new ColorMask();
            cm.HueMin = hueMin;
            cm.HueMax = hueMax;
            cm.SaturationMin = saturationMin;
            cm.SaturationsMax = saturationMax;
            return cm;
        }
        private static void ConvertImagesToAscii(string imagesFolderPath, string outputFolderPath, string name, BrightnessSettings bs, ColorMask cm = new ColorMask(), bool keepBaseDimensions = false)
        {
            int count = 0;
            int placeLimit = 10;
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

                string imageText = ConvertImageToAscii(imageFile, bs, cm, keepBaseDimensions);

                string outputFilePath = Path.Combine(outputFolderPath, $"{name}{displayZeroes}{count}.txt");

                using (StreamWriter sw = new StreamWriter(outputFilePath))
                {
                    sw.WriteLine(imageText);
                }

                count++;
                if (!(count < placeLimit))
                {
                    if (displayZeroes.Length > 0) {
                        displayZeroes = displayZeroes.Substring(0,displayZeroes.Length-1);
                    }
                    placeLimit *= 10;
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
        private static string ConvertImageToAscii(string imageFile, BrightnessSettings bs, ColorMask cm, bool keepBaseDimensions = false)
        {
            string imageText = "";
            //Image image = Image.FromFile(imageFile);
            Bitmap bm = new Bitmap(imageFile);

            if (keepBaseDimensions)
            {
                windowHeight = bm.Height / 8 ;
                windowWidth = bm.Width / 4 ;
            }

            //base height and width in pixels of a given pixel grid
            int pixelGridWidth = (bm.Width / windowWidth);
            int pixelGridHeight = (bm.Height / windowHeight);

            //Account for remaining pixels that would otherwise be cut off, used to determine current width and height.
            int pixelWidthRemainder = bm.Width % windowWidth;
            int pixelHeightRemainder = bm.Height % windowHeight;

            int widthDenominator = FindCommonDenominator(pixelWidthRemainder, windowWidth - pixelWidthRemainder);
            int gridsToAddWidthPixel = pixelWidthRemainder / widthDenominator;
            int gridsNotToAddWidthPixel = (windowWidth - pixelWidthRemainder) / widthDenominator;
            int addedWidthPixel = 1;
            int widthCount = 0;
            
            int heightDenominator = FindCommonDenominator(pixelHeightRemainder, windowHeight - pixelHeightRemainder);
            int gridsToAddHeightPixel = pixelHeightRemainder / heightDenominator;
            int gridsNotToAddHeightPixel = (windowHeight - pixelHeightRemainder) / heightDenominator;
            int addedHeightPixel = 1;
            int heightCount = 0;


            float remainderWidthCheck = 0.0F;
            float remainderHeightCheck = 0.0F;

            //set quadrant brightness averages starting at 0
            float topLeftQuadrant = 0F;
            float topRightQuadrant = 0F;
            float bottomLeftQuadrant = 0F;
            float bottomRightQuadrant = 0F;

            //Set the grid width and height to start
            int currentGridWidth = pixelGridWidth;
            int currentGridHeight = pixelGridHeight;

            //Inside the entire image
            for (int y = 0; y < bm.Height - pixelGridHeight + (int)remainderHeightCheck; y += pixelGridHeight + addedHeightPixel)
            {

                if (heightCount < gridsToAddHeightPixel)
                {
                    addedHeightPixel = 1;
                    heightCount++;
                }
                else if (heightCount < gridsToAddHeightPixel + gridsNotToAddHeightPixel)
                {
                    addedHeightPixel = 0;
                    heightCount++;
                }
                if (heightCount >= gridsToAddHeightPixel + gridsNotToAddHeightPixel)
                {
                    heightCount = 0;
                }
                currentGridHeight = pixelGridHeight + addedHeightPixel;

                if (y > 0) { imageText += '\n'; }

                for (int x = 0; x < bm.Width - pixelGridWidth + (int)remainderWidthCheck; x += pixelGridWidth + addedWidthPixel)
                {
                    
                    if(widthCount < gridsToAddWidthPixel)
                    {
                        addedWidthPixel = 1;
                        widthCount++;
                    }
                    else if(widthCount < gridsToAddWidthPixel + gridsNotToAddWidthPixel)
                    {
                        addedWidthPixel = 0;
                        widthCount++;
                    }
                    if (widthCount >= gridsToAddWidthPixel + gridsNotToAddWidthPixel)
                    {
                        widthCount = 0;
                    }

                    currentGridWidth = pixelGridWidth + addedWidthPixel;

                    //number of pixels in this quadrant
                    int pixelQuadrantCount = currentGridWidth * currentGridHeight / 4;

                    //Inside a single pixelGrid
                    for (int i = 0; i < currentGridWidth; i++)
                    {
                        
                        for (int j = 0; j < currentGridHeight; j++)
                        {
                            Color thisPixel = bm.GetPixel(x + i, y + j);
                            float brightness = thisPixel.GetBrightness();

                            if (thisPixel.GetHue() > cm.HueMin && thisPixel.GetHue() < cm.HueMax || thisPixel.GetSaturation() > cm.SaturationMin && thisPixel.GetSaturation() < cm.SaturationsMax)
                            {
                                brightness = 0F;
                            }

                            if (i <= currentGridWidth / 2 && j <= currentGridHeight / 2)
                            {
                                //top left
                                topLeftQuadrant += brightness;
                            }
                            if (i > currentGridWidth / 2 && j <= currentGridHeight / 2)
                            {
                                //top right
                                topRightQuadrant += brightness;
                            }
                            if (i > currentGridWidth / 2 && j > currentGridHeight / 2)
                            {
                                //bottom right
                                bottomRightQuadrant += brightness;
                            }
                            if (i <= currentGridWidth / 2 && j > currentGridHeight / 2)
                            {
                                //bottom left
                                bottomLeftQuadrant += brightness;
                            }
                        }
                    }
                    if (remainderWidthCheck > 1.0F)
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


                    Dictionary<string, string> chars;

                    float threshold;
                    if (bs.isPhotoNegative)
                    {
                        if (gridAverageBrightness >= bs.DarkThreshold)
                        {
                            imageText += " ";
                            continue;
                        }
                        else if (gridAverageBrightness >= bs.MediumThreshold)
                        {
                            threshold = bs.BlankThreshold;
                            chars = LightValues;
                        }
                        else if (gridAverageBrightness >= bs.LightThreshold)
                        {
                            threshold = bs.LightThreshold;
                            chars = MediumValues;
                        }
                        else if (gridAverageBrightness >= bs.BlankThreshold)
                        {
                            threshold = bs.MediumThreshold;
                            chars = DarkValues;
                        }
                        else
                        {
                            threshold = bs.DarkThreshold;
                            chars = FullValues;
                        }
                    }
                    else
                    {
                        if (gridAverageBrightness <= bs.BlankThreshold)
                        {
                            imageText += " ";
                            continue;
                        }
                        else if (gridAverageBrightness <= bs.LightThreshold)
                        {
                            threshold = bs.BlankThreshold;
                            chars = LightValues;
                        }
                        else if (gridAverageBrightness <= bs.MediumThreshold)
                        {
                            threshold = bs.LightThreshold;
                            chars = MediumValues;
                        }
                        else if (gridAverageBrightness <= bs.DarkThreshold)
                        {
                            threshold = bs.MediumThreshold;
                            chars = DarkValues;
                        }
                        else
                        {
                            threshold = bs.DarkThreshold;
                            chars = FullValues;
                        }
                    }
                   
                    bool topLeft = topLeftQuadrant > threshold;
                    bool bottomLeft = bottomLeftQuadrant > threshold;
                    bool topRight = topRightQuadrant > threshold;
                    bool bottomRight = bottomRightQuadrant > threshold;

                    if (!topLeft && !topRight && bottomLeft && bottomRight)
                    {
                        imageText += chars["BOTTOM"];
                    }
                    else if (topLeft && !topRight && !bottomLeft && !bottomRight)
                    {
                        imageText += chars["TOPLEFT"];
                    }
                    else if (!topLeft && topRight && !bottomLeft && !bottomRight)
                    {
                        imageText += chars["TOPRIGHT"];
                    }
                    else if (topLeft && topRight && !bottomLeft && !bottomRight)
                    {
                        imageText += chars["TOP"];
                    }
                    else if (!topLeft && !topRight && bottomLeft && !bottomRight)
                    {
                        imageText += chars["BOTTOMLEFT"];
                    }
                    else if (!topLeft && !topRight && !bottomLeft && bottomRight)
                    {
                        imageText += chars["BOTTOMRIGHT"];
                    }
                    else if (topLeft && !topRight && bottomLeft && bottomRight)
                    {
                        imageText += chars["UPLEFT"]; ;
                    }
                    else if (!topLeft && topRight && bottomLeft && bottomRight)
                    {
                        imageText += chars["UPRIGHT"]; 
                    }
                    else if (topLeft && topRight && !bottomLeft && bottomRight)
                    {
                        imageText += chars["DOWNRIGHT"];
                    }
                    else if (topLeft && topRight && bottomLeft && !bottomRight)
                    {
                        imageText += chars["DOWNLEFT"];
                    }
                    else
                    {
                        imageText += chars["FILL"];
                    }
                }
            }
            return imageText;
        }
        private static int FindCommonDenominator(int a, int b)
        {
            while (b > 0)
            {
                int remainder = a % b;
                a = b;
                b = remainder;
            }
            return a;
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
        private static void SetFontSize(short fontHeight)
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

        //Method to set the terminal window to full screen
        struct WindowDimensions
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }  // Structure used by GetWindowRect
        private static Vector2 SetTerminalFullScreen()
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