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
using ImageConverter = AnimateTheConsole.Core.ImageConverter;

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
        private static string splitString = "|";
        private static void Main(string[] args)
        {
            Console.CursorVisible = false;
            ImageConverter converter = new ImageConverter(Console.WindowWidth, Console.WindowHeight, splitString);
            AsciiFileIO fileIO = new AsciiFileIO("SixOfCrows");

            ColorMask adamMask = MakeColorMask(48.0F, 360.0F, 0.0F, 0.12F);

            BrightnessSettings sixCrowSettings = MakeBrightnessSetting(.10F, .16F, .28F, .52F);
            BrightnessSettings hadesSettings = MakeBrightnessSetting(.10F, .25F, .55F, .75F);
            BrightnessSettings adamSettings = MakeBrightnessSetting(.42F, .55F, .70F, .85F);
            BrightnessSettings coinSettings = MakeBrightnessSetting(.10F, .20F, .45F, .65F);
            BrightnessSettings defaultSettings = MakeBrightnessSetting(.20F, .40F, .60F, .80F);

            //converter.ConvertImagesToAscii(imagesFolderPath, outputFolderPath, name, sixCrowSettings, default, true);

            AsciiDisplay.DisplayAscii(fileIO, true);

            //ShowExample();
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
    public struct BrightnessSettings
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
}