using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using AnimateTheConsole.Core;
using AnimateTheConsole.FileIO;
using System.Runtime.InteropServices;

namespace AnimateTheConsole
{
    public class Application
    {
        ConsoleService console = new ConsoleService();
        private string splitString;
        private ImageConverter converter;
        private AsciiFileIO fileIO;

        private Dictionary<string,BrightnessSettings> bs = new Dictionary<string,BrightnessSettings>();
        public void Run()
        {
            Startup();
            bool isRunning = true;
            while (isRunning)
            {
                isRunning = UserInterface();
            }
        }
        public void Startup()
        {
            splitString = "|";
            Console.CursorVisible = false;
            AsciiDisplay.SetTerminalFullScreen();
            AsciiDisplay.SetFontSize(AsciiDisplay.FontSizeDefault);
            converter = new ImageConverter(Console.WindowWidth, Console.WindowHeight, splitString);
            fileIO = new AsciiFileIO("");

            ColorMask adamMask = MakeColorMask(48.0F, 360.0F, 0.0F, 0.12F);

            bs.Add("SixOfCrows", MakeBrightnessSetting(.10F, .16F, .28F, .52F));
            bs.Add("Hades", MakeBrightnessSetting(.10F, .25F, .55F, .75F));
            bs.Add("CreationOfAdam", MakeBrightnessSetting(.42F, .55F, .70F, .85F));
            bs.Add("Coins", MakeBrightnessSetting(.10F, .20F, .45F, .65F));
            bs.Add("Default", MakeBrightnessSetting(.20F, .40F, .60F, .80F));

        }

        public void UseConHost()
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.CreateNoWindow = false;

            process.StartInfo.FileName = @"conhost.exe";
            process.StartInfo.Arguments = $"conhost.exe -- cmd.exe /c {Environment.CurrentDirectory}\\AnimateTheConsole.exe run";

            process.Start();
            process.WaitForExit();
        }
        public bool UserInterface()
        {
            PrintMainMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2);
            if(menuSelection == 0)
            {
                //Exit the application
                return false;
            }
            else if(menuSelection == 1)
            {
                
                //Convert Images
                if (PrintFolderChoiceMenu(fileIO.GetImageFileNames()))
                {
                    string bsKey = "Default";
                    if (bs.ContainsKey(fileIO.FileName))
                    {
                        bsKey = fileIO.FileName;
                    }
                    converter.ConvertImagesToAscii(fileIO, bs[bsKey], default, true);
                    console.Pause();
                }
            }
            else if(menuSelection == 2)
            {
                //Display ASCII
                if (PrintFolderChoiceMenu(fileIO.GetAsciiFileNames()))
                {
                    AsciiDisplay.DisplayAscii(fileIO, true);
                    console.Pause();
                }
            }
            return true;
        }

        public void PrintMainMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello!");
            Console.WriteLine();
            Console.WriteLine("1: Convert Images to ASCII");
            Console.WriteLine("2: Display ASCII");
            Console.WriteLine();
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }
        public bool PrintFolderChoiceMenu(List<string> options)
        {
            Console.Clear();
            Console.WriteLine();
            for (int i = 1; i <= options.Count; i++)
            {
                Console.WriteLine($"{i}: {options[i-1]}");
            }
            Console.WriteLine();
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");

            int userChoice = console.PromptForInteger("Please choose an option", 0, options.Count);
            if(userChoice == 0)
            {
                return false;
            }
            else
            {
                fileIO.FileName = options[userChoice-1];
            }
            return true;
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
    }
}
