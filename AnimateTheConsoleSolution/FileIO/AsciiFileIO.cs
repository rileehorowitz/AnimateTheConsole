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
        const string imagePath = @"data\images\";
        const string asciiPath = @"data\image-to-ascii-output\";
        public string SplitString { get; set; } = "|";
        public string FileName { get; set; }
        public string ImagesFolderPath { get => imagePath + FileName; }
        public string AsciiFolderPath { get => asciiPath + FileName; }

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
            imageFiles.AddRange(Directory.GetFiles(ImagesFolderPath));
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
        public List<string> GetAsciiFileNames()
        {
            List<string> output = new List<string>();
            output.AddRange(Directory.GetDirectories(asciiPath));
            for(int i = 0; i < output.Count; i++)
            {
                output[i] = output[i].Remove(0, asciiPath.Length);
            }
            return output;
        }
        public List<string> GetImageFileNames()
        {
            List<string> output = new List<string>();
            output.AddRange(Directory.GetDirectories(imagePath));
            for (int i = 0; i < output.Count; i++)
            {
                output[i] = output[i].Remove(0, imagePath.Length);
            }
            return output;
        }
    }
}
