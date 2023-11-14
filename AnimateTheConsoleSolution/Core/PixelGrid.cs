using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using AnimateTheConsole;

namespace AnimateTheConsole.Core
{
    public class PixelGrid
    {
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

        public Vector2 TopLeftPixel { get; private set; }
        public Vector2 BottomRightPixel { get; private set; }

        public int GridWidth { get => (int)(BottomRightPixel.X + 1 - TopLeftPixel.X); }
        public int GridHeight { get => (int)(BottomRightPixel.Y + 1 - TopLeftPixel.Y); }

        public float PixelCount { get => GridWidth * GridHeight; }
        public float PixelPerQuad { get => PixelCount / 4; }

        public float AverageBrightness { get => (TopLeftQuad + TopRightQuad + BottomLeftQuad + BottomRightQuad) / 4; }

        public float TopLeftQuad { get; set; }
        public float TopRightQuad { get; set; }
        public float BottomLeftQuad { get; set; }
        public float BottomRightQuad { get; set; }

        public PixelGrid(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
        {
            TopLeftPixel = new Vector2(topLeftX, topLeftY);
            BottomRightPixel = new Vector2(bottomRightX, bottomRightY);
        }

        public void AverageQuadrants()
        {
            if (GridWidth % 2 == 0 && GridHeight % 2 == 0)
            {
                TopLeftQuad /= PixelPerQuad;
                TopRightQuad /= PixelPerQuad;
                BottomLeftQuad /= PixelPerQuad;
                BottomRightQuad /= PixelPerQuad;
            }
            else if (GridWidth % 2 != 0)
            {
                TopLeftQuad /= PixelPerQuad;
                TopRightQuad /= PixelPerQuad + GridHeight / 2;
                BottomLeftQuad /= PixelPerQuad;
                BottomRightQuad /= PixelPerQuad + GridHeight / 2;
            }
            else if (GridHeight % 2 != 0)
            {
                TopLeftQuad /= PixelPerQuad;
                TopRightQuad /= PixelPerQuad;
                BottomLeftQuad /= PixelPerQuad + GridWidth / 2;
                BottomRightQuad /= PixelPerQuad + GridWidth / 2;
            }
            else
            {
                TopLeftQuad /= PixelPerQuad;
                TopRightQuad /= (PixelPerQuad + GridHeight / 2);
                BottomLeftQuad /= (PixelPerQuad + GridWidth / 2);
                BottomRightQuad /= (PixelPerQuad + GridHeight / 2 + GridWidth / 2 + 1);
            }
        }

        public void AddPixel(Bitmap bm, ColorMask cm, int xPos, int yPos)
        {
            Color thisPixel = bm.GetPixel(xPos, yPos);
            float brightness = thisPixel.GetBrightness();

            if (thisPixel.GetHue() > cm.HueMin && thisPixel.GetHue() < cm.HueMax || thisPixel.GetSaturation() > cm.SaturationMin && thisPixel.GetSaturation() < cm.SaturationsMax)
            {
                brightness = 0F;
            }

            if (xPos <= TopLeftPixel.X + GridWidth / 2 && yPos <= BottomRightPixel.Y - GridHeight / 2)
            {
                //top left
                TopLeftQuad += brightness;
            }
            if (xPos > TopLeftPixel.X + GridWidth / 2 && yPos <= BottomRightPixel.Y - GridHeight / 2)
            {
                //top right
                TopRightQuad += brightness;
            }
            if (xPos > TopLeftPixel.X + GridWidth / 2 && yPos > BottomRightPixel.Y - GridHeight / 2)
            {
                //bottom right
                BottomRightQuad += brightness;
            }
            if (xPos <= TopLeftPixel.X + GridWidth / 2 && yPos > BottomRightPixel.Y - GridHeight / 2)
            {
                //bottom left
                BottomLeftQuad += brightness;
            }
        }

        public float AssignValues(BrightnessSettings bs, out Dictionary<string,string> chars)
        {
            float threshold;
            if (AverageBrightness <= bs.BlankThreshold)
            {
                threshold = 1;
                chars = null;
            }
            else if (AverageBrightness <= bs.LightThreshold)
            {
                threshold = bs.BlankThreshold;
                chars = LightValues;
            }
            else if (AverageBrightness <= bs.MediumThreshold)
            {
                threshold = bs.LightThreshold;
                chars = MediumValues;
            }
            else if (AverageBrightness <= bs.DarkThreshold)
            {
                threshold = bs.MediumThreshold;
                chars = DarkValues;
            }
            else
            {
                threshold = bs.DarkThreshold;
                chars = FullValues;
            }
            return threshold;
        }

        public string AssignCharacter(BrightnessSettings bs)
        {
            Dictionary<string, string> chars;
            AverageQuadrants();
            float threshold = AssignValues(bs, out chars);
            bool topLeft = TopLeftQuad > threshold;
            bool bottomLeft = BottomLeftQuad > threshold;
            bool topRight = TopRightQuad > threshold;
            bool bottomRight = BottomRightQuad > threshold;

            if(chars == null)
            {
                return " ";
            }
            else if (!topLeft && !topRight && bottomLeft && bottomRight)
            {
                return chars["BOTTOM"];
            }
            else if (topLeft && !topRight && !bottomLeft && !bottomRight)
            {
                return chars["TOPLEFT"];
            }
            else if (!topLeft && topRight && !bottomLeft && !bottomRight)
            {
                return chars["TOPRIGHT"];
            }
            else if (topLeft && topRight && !bottomLeft && !bottomRight)
            {
                return chars["TOP"];
            }
            else if (!topLeft && !topRight && bottomLeft && !bottomRight)
            {
                return chars["BOTTOMLEFT"];
            }
            else if (!topLeft && !topRight && !bottomLeft && bottomRight)
            {
                return chars["BOTTOMRIGHT"];
            }
            else if (topLeft && !topRight && bottomLeft && bottomRight)
            {
                return chars["UPLEFT"]; ;
            }
            else if (!topLeft && topRight && bottomLeft && bottomRight)
            {
                return chars["UPRIGHT"];
            }
            else if (topLeft && topRight && !bottomLeft && bottomRight)
            {
                return chars["DOWNRIGHT"];
            }
            else if (topLeft && topRight && bottomLeft && !bottomRight)
            {
                return chars["DOWNLEFT"];
            }
            else
            {
                return chars["FILL"].Substring(new Random().Next(0, chars["FILL"].Length - 1), 1);
            }
        }
    }
}
