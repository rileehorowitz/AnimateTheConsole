using AnimateTheConsole.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimateTheConsole.FileIO
{
    public static class AsciiFileIO
    {
        public static List<string> ReadAsciiFromFolder(string asciiFolderPath, string splitString)
        {

            List<string> frames = new List<string>();
            string asciiFile = Directory.GetFiles(asciiFolderPath)[0];
            using (StreamReader sr = new StreamReader(asciiFile))
            {
                frames.AddRange(sr.ReadToEnd().Split(splitString));
            }

            return frames;
        }
        public static void WriteAsciiToFile(string outputFolderPath, string name, string imageText)
        {
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }
            //store ascii frames in a text file
            string outputFilePath = Path.Combine(outputFolderPath, $"{name}.txt");
            using (StreamWriter sw = new StreamWriter(outputFilePath))
            {
                sw.WriteLine(imageText);
            }
        }

        public static List<Bitmap> LoadImages(string imagesFolderPath, string name)
        {
            List<Bitmap> images = new List<Bitmap>();
            string[] foldersInImagePath = Directory.GetDirectories(imagesFolderPath);
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
                AsciiDisplay.IncrementDisplayCount(name, imageFiles.Count);
            }
            return images;
        }
    }
}
