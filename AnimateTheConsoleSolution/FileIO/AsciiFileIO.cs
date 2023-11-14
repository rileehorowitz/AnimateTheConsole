using AnimateTheConsole.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimateTheConsole.FileIO
{
    public class AsciiFileIO
    {
        public string SplitString { get; set; } = "|";
        public string FileName { get; set; }
        public string ImagesFolderPath { get => @"data\images\" + FileName; }
        public string AsciiFolderPath { get => @"data\image-to-ascii-output\" + FileName + "Frames"; }

        public AsciiFileIO(string fileName)
        {
            FileName = fileName;
        }

        public List<string> ReadAsciiFromFolder()
        {

            List<string> frames = new List<string>();
            string asciiFile = Directory.GetFiles(AsciiFolderPath)[0];
            using (StreamReader sr = new StreamReader(asciiFile))
            {
                frames.AddRange(sr.ReadToEnd().Split(SplitString));
            }

            return frames;
        }
        public void WriteAsciiToFile(string imageText)
        {
            if (!Directory.Exists(AsciiFolderPath))
            {
                Directory.CreateDirectory(AsciiFolderPath);
            }
            //store ascii frames in a text file
            string outputFilePath = Path.Combine(AsciiFolderPath, $"{FileName}.txt");
            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                sw.WriteLine(imageText);
            }
        }
        public List<Bitmap> LoadImages()
        {
            List<Bitmap> images = new List<Bitmap>();
            string[] foldersInImagePath = Directory.GetDirectories(ImagesFolderPath);
            List<string> imageFiles = new List<string>();
            images = new List<Bitmap>();
            for (int i = 0; i < foldersInImagePath.Length; i++)
            {
                imageFiles.AddRange(Directory.GetFiles($"{foldersInImagePath[i]}", "*.jpg"));
            }
            //Load all images and display status to user
            AsciiDisplay.ResetDisplayCount(imageFiles.Count.ToString().Length);
            foreach (string imageFile in imageFiles)
            {
                images.Add(new Bitmap(imageFile));
                AsciiDisplay.IncrementDisplayCount(FileName, imageFiles.Count);
            }
            return images;
        }
    }
}
