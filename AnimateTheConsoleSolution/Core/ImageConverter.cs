using AnimateTheConsole.FileIO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimateTheConsole.Core
{
    public class ImageConverter
    {
        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }
        public string SplitString { get; set; }

        public ImageConverter(int windowWidth, int windowHeight, string splitString)
        {
            WindowWidth = windowWidth;
            WindowHeight = windowHeight;
            SplitString = splitString;
        }

        public void ConvertImagesToAscii(AsciiFileIO fileIO, BrightnessSettings bs, ColorMask cm = new ColorMask(), bool keepBaseDimensions = false)
        {
            List<Bitmap> images = fileIO.LoadImages();
            string imageText = "";
            AsciiDisplay.ResetDisplayCount(images.Count.ToString().Length);
            foreach (Bitmap image in images)
            {
                if (keepBaseDimensions)
                {
                    imageText += ConvertImageToAsciiFitImage(image, bs, cm) + SplitString;
                }
                else
                {
                    imageText += ConvertImageToAsciiFitWindow(image, bs, cm) + SplitString;
                }
                AsciiDisplay.IncrementDisplayCount("Converting Images", images.Count);
            }

            fileIO.WriteAsciiToFile(imageText);
        }
        private string ConvertImageToAsciiFitWindow(Bitmap bm, BrightnessSettings bs, ColorMask cm)
        {
            bm = new Bitmap(bm, bm.Width / 2, bm.Height / 4);
            string imageText = "";

            //base height and width in pixels of a given pixel grid
            int pixelGridWidth = (bm.Width / WindowWidth);
            int pixelGridHeight = (bm.Height / WindowHeight);

            //Account for remaining pixels that would otherwise be cut off, used to determine current width and height.
            int pixelWidthRemainder = bm.Width % WindowWidth;
            int pixelHeightRemainder = bm.Height % WindowHeight;

            int widthDenominator = FindCommonDenominator(pixelWidthRemainder, WindowWidth - pixelWidthRemainder);
            int gridsToAddWidthPixel = pixelWidthRemainder / widthDenominator;
            int gridsInWidthSet = gridsToAddWidthPixel + ((WindowWidth - pixelWidthRemainder) / widthDenominator);
            int addedWidthPixel = 1;
            int widthCount = 0;

            int heightDenominator = FindCommonDenominator(pixelHeightRemainder, WindowHeight - pixelHeightRemainder);
            int gridsToAddHeightPixel = pixelHeightRemainder / heightDenominator;
            int gridsInHeightSet = ((WindowHeight - pixelHeightRemainder) / heightDenominator) / pixelGridHeight;
            int addedHeightPixel = 1;
            int heightCount = 0;

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

                    if (widthCount < gridsToAddWidthPixel)
                    {
                        addedWidthPixel = 1;
                        widthCount++;
                    }
                    else if (widthCount < gridsInWidthSet)
                    {
                        addedWidthPixel = 0;
                        widthCount++;
                    }
                    if (widthCount >= gridsInWidthSet)
                    {
                        widthCount = 0;
                    }

                    currentGridWidth = pixelGridWidth + addedWidthPixel;

                    PixelGrid grid = new PixelGrid(x, y, x + currentGridWidth - 1, y + currentGridHeight - 1);

                    //Inside a single pixelGrid
                    for (int i = 0; i < grid.GridWidth; i++)
                    {
                        for (int j = 0; j < grid.GridHeight; j++)
                        {
                            grid.AddPixel(bm, cm, x + i, y + j);
                        }
                    }

                    imageText += grid.AssignCharacter(bs);
                }
            }
            return imageText;
        }
        private string ConvertImageToAsciiFitImage(Bitmap bm, BrightnessSettings bs, ColorMask cm)
        {
            bm = new Bitmap(bm, bm.Width / 2, bm.Height / 4);
            WindowWidth = bm.Width / 2;
            WindowHeight = bm.Height / 2;

            string imageText = "";

            //base height and width in pixels of a given pixel grid
            int pixelGridWidth = (bm.Width / WindowWidth);
            int pixelGridHeight = (bm.Height / WindowHeight);

            List<PixelGrid> gridList = new List<PixelGrid>();

            for (int i = 0; i < bm.Width * bm.Height; i++) //run through every pixel
            {
                int xPos = i % bm.Width;
                int yPos = i / bm.Width;
                int key = (yPos / pixelGridHeight) * (WindowWidth) + (xPos / pixelGridWidth);
                if (gridList.Count <= key)
                {
                    gridList.Add(new PixelGrid(xPos, yPos, xPos + pixelGridWidth - 1, yPos + pixelGridHeight - 1));
                }

                gridList[key].AddPixel(bm, cm, xPos, yPos);

                //Added final pixel to grid, so we convert entire grid to a single character and add it to our imageText
                if (xPos == gridList[key].BottomRightPixel.X && yPos == gridList[key].BottomRightPixel.Y)
                {
                    //If this grid is the first in a new row, add a line break first before adding next character
                    if (key % WindowWidth == 0 && key != 0)
                    {
                        imageText += "\n";
                    }

                    imageText += gridList[key].AssignCharacter(bs);
                }
            }
            return imageText;
        }
        private int FindCommonDenominator(int a, int b)
        {
            while (b > 0)
            {
                int remainder = a % b;
                a = b;
                b = remainder;
            }
            return a;
        }

    }
}
