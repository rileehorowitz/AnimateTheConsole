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
        
        private static void Main(string[] args)
        {
            Application app = new Application();
            if (args.Length == 0)
            {
                app.UseConHost();
            }
            else if (args[0] == "run")
            {
                app.Run();
            }
            //app.Run();
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