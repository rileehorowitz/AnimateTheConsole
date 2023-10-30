using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Numerics;
using AnimateTheConsole.Core;
using AnimateTheConsole.FileIO;

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
        private static Dictionary<string, string> FullValues = new Dictionary<string, string>()
        {
            {"FILL", "8H@$0" },
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
            {"FILL", "OI/\\" },
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
            {"FILL", "!\\/" },
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
            {"FILL", ":.^" },
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
            Console.CursorVisible = false;
            windowWidth = Console.LargestWindowWidth;
            windowHeight = Console.LargestWindowHeight;
            

            string name = "Coins";
            string imagesFolderPath = @"data\images\" + name;
            string outputFolderPath = @"data\image-to-ascii-output\" + name + "Frames";
            string asciiFolderPath = @"data\image-to-ascii-output\" + name + "Frames";

            ColorMask adamMask = MakeColorMask(48.0F, 360.0F, 0.0F, 0.12F);

            BrightnessSettings sixCrowSettings = MakeBrightnessSetting(.10F, .16F, .28F, .52F);
            BrightnessSettings hadesSettings = MakeBrightnessSetting(.10F, .25F, .55F, .75F);
            BrightnessSettings adamSettings = MakeBrightnessSetting(.42F, .55F, .70F, .85F);
            BrightnessSettings coinSettings = MakeBrightnessSetting(.10F, .20F, .45F, .65F);
            BrightnessSettings defaultSettings = MakeBrightnessSetting(.20F, .40F, .60F, .80F);

            //ConvertImagesToAscii(imagesFolderPath, outputFolderPath, name, coinSettings, default, true);

            AsciiDisplay.DisplayAscii(asciiFolderPath, splitString);

            //ShowExample();
        }

        public static void ShowExample()
        {
            List<string> frames = new List<string>();
            string name = "";
            string path = "";
            Console.Write("Shading and Anti Aliasing at Higher Brightness Example : Hades Animation");
            Console.ReadKey(true);
            Console.Clear();
            name = "Hades";
            path = @"data\image-to-ascii-output\" + name + "Frames";
            AsciiDisplay.DisplayAscii(path, splitString);

            Console.Write("Shading and Anti Aliasing at Lower Brightness Example : Six Of Crows Animation");
            Console.ReadKey(true);
            Console.Clear();
            name = "SixOfCrows";
            path = @"data\image-to-ascii-output\" + name + "Frames";
            AsciiDisplay.DisplayAscii(path, splitString);
        }

        struct BrightnessSettings
        {
            public float BlankThreshold;
            public float LightThreshold;
            public float MediumThreshold;
            public float DarkThreshold;
        }
        public struct ColorMask
        {
            public float HueMin;
            public float HueMax;
            public float SaturationMin;
            public float SaturationsMax;
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
            List<Bitmap> images = AsciiFileIO.LoadImages(imagesFolderPath, name);
            string imageText = "";
            AsciiDisplay.ResetDisplayCount(images.Count.ToString().Length);
            foreach (Bitmap image in images)
            {
                imageText += TestConvertImageToAscii(image, bs, cm, keepBaseDimensions) + splitString;
                AsciiDisplay.IncrementDisplayCount("Converting Images", images.Count);
            }

            AsciiFileIO.WriteAsciiToFile(outputFolderPath, name, imageText);
        }
        private static string ConvertImageToAscii(Bitmap bm, BrightnessSettings bs, ColorMask cm, bool keepBaseDimensions = false)
        {
            string imageText = "";

            if (keepBaseDimensions)
            {
                windowWidth = bm.Width / 4 ;
                windowHeight = bm.Height / 8 ;
            }

            //base height and width in pixels of a given pixel grid
            int pixelGridWidth = (bm.Width / windowWidth);
            int pixelGridHeight = (bm.Height / windowHeight);

            //Account for remaining pixels that would otherwise be cut off, used to determine current width and height.
            int pixelWidthRemainder = bm.Width % windowWidth;
            int pixelHeightRemainder = bm.Height % windowHeight;

            int widthDenominator = FindCommonDenominator(pixelWidthRemainder, windowWidth - pixelWidthRemainder);
            int gridsToAddWidthPixel = pixelWidthRemainder / widthDenominator;
            int gridsInWidthSet = ((windowWidth - pixelWidthRemainder) / widthDenominator) / pixelGridWidth;
            int addedWidthPixel = 1;
            int widthCount = 0;
            
            int heightDenominator = FindCommonDenominator(pixelHeightRemainder, windowHeight - pixelHeightRemainder);
            int gridsToAddHeightPixel = pixelHeightRemainder / heightDenominator;
            int gridsInHeightSet = ((windowHeight - pixelHeightRemainder) / heightDenominator) / pixelGridHeight;
            int addedHeightPixel = 1;
            int heightCount = 0;

            //set quadrant brightness averages starting at 0
            float topLeftQuadrant = 0F;
            float topRightQuadrant = 0F;
            float bottomLeftQuadrant = 0F;
            float bottomRightQuadrant = 0F;

            //Set the grid width and height to start
            int currentGridWidth = pixelGridWidth;
            int currentGridHeight = pixelGridHeight;

            //Inside the entire image
            for (int y = 0; y < bm.Height - pixelGridHeight; y += pixelGridHeight + addedHeightPixel)
            {

                if (heightCount < gridsToAddHeightPixel)
                {
                    addedHeightPixel = 1;
                    heightCount++;
                }
                else if (heightCount < gridsInHeightSet)
                {
                    addedHeightPixel = 0;
                    heightCount++;
                }
                if (heightCount >= gridsInHeightSet)
                {
                    heightCount = 0;
                }
                currentGridHeight = pixelGridHeight + addedHeightPixel;

                if (y > 0) { imageText += '\n'; }

                for (int x = 0; x < bm.Width - pixelGridWidth; x += pixelGridWidth + addedWidthPixel)
                {
                    
                    if(widthCount < gridsToAddWidthPixel)
                    {
                        addedWidthPixel = 1;
                        widthCount++;
                    }
                    else if(widthCount < gridsInWidthSet)
                    {
                        addedWidthPixel = 0;
                        widthCount++;
                    }
                    if (widthCount >= gridsInWidthSet)
                    {
                        widthCount = 0;
                    }

                    currentGridWidth = pixelGridWidth + addedWidthPixel;

                    PixelGrid grid = new PixelGrid(x, y, x + currentGridWidth, y + currentGridHeight);

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
                                grid.TopLeftQuad += brightness;
                            }
                            if (i > currentGridWidth / 2 && j <= currentGridHeight / 2)
                            {
                                //top right
                                grid.TopRightQuad += brightness;
                            }
                            if (i > currentGridWidth / 2 && j > currentGridHeight / 2)
                            {
                                //bottom right
                                grid.BottomRightQuad += brightness;
                            }
                            if (i <= currentGridWidth / 2 && j > currentGridHeight / 2)
                            {
                                //bottom left
                                grid.BottomLeftQuad += brightness;
                            }
                        }
                    }

                    //average brightness of each quadrant
                    topLeftQuadrant /= pixelQuadrantCount;
                    topRightQuadrant /= pixelQuadrantCount;
                    bottomLeftQuadrant /= pixelQuadrantCount;
                    bottomRightQuadrant /= pixelQuadrantCount;

                    grid.AverageQuadrants();

                    //average brightness of entire grid
                    float gridAverageBrightness = grid.AverageBrightness;


                    Dictionary<string, string> chars;

                    float threshold;


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


                    bool topLeft = grid.TopLeftQuad > threshold;
                    bool bottomLeft = grid.BottomLeftQuad > threshold;
                    bool topRight = grid.TopRightQuad > threshold;
                    bool bottomRight = grid.BottomRightQuad > threshold;

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
                        imageText += chars["FILL"].Substring(new Random().Next(0, chars["FILL"].Length - 1),1);
                    }
                }
            }
            return imageText;
        }
        private static string TestConvertImageToAscii(Bitmap bm, BrightnessSettings bs, ColorMask cm, bool keepBaseDimensions = false)
        {
            string imageText = "";
            List<PixelGrid> gridList = new List<PixelGrid>();
            if (keepBaseDimensions)
            {
                windowWidth = bm.Width / 4 ;
                windowHeight = bm.Height / 8 ;
            }
            //base height and width in pixels of a given pixel grid
            int pixelGridWidth = (bm.Width / windowWidth);
            int pixelGridHeight = (bm.Height / windowHeight);
            for(int i = 0; i < bm.Width * bm.Height; i++) //run through every pixel
            {
                int xPos = i % bm.Width;
                int yPos = i / bm.Width;
                int key = (yPos / pixelGridHeight) * (windowWidth) + (xPos / pixelGridWidth);
                if (gridList.Count <= key)
                {
                    gridList.Add(new PixelGrid(xPos, yPos, xPos + pixelGridWidth-1, yPos + pixelGridHeight-1));
                }
                gridList[key].AddPixel(bm, cm, xPos, yPos);


                if (xPos == gridList[key].BottomRightPixel.X && yPos == gridList[key].BottomRightPixel.Y)
                {
                    gridList[key].AverageQuadrants();
                    float gridAverageBrightness = gridList[key].AverageBrightness;
                    Dictionary<string, string> chars;
                    float threshold;

                    if (key % windowWidth == 0 && key != 0)
                    {
                        imageText += "\n";
                    }

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
                    bool topLeft = gridList[key].TopLeftQuad > threshold;
                    bool bottomLeft = gridList[key].BottomLeftQuad > threshold;
                    bool topRight = gridList[key].TopRightQuad > threshold;
                    bool bottomRight = gridList[key].BottomRightQuad > threshold;

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
                        imageText += chars["FILL"].Substring(new Random().Next(0, chars["FILL"].Length - 1), 1);
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