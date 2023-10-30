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
            //if (GridWidth % 2 == 0 && GridHeight % 2 == 0)
            //{
                TopLeftQuad /= PixelPerQuad;
                TopRightQuad /= PixelPerQuad;
                BottomLeftQuad /= PixelPerQuad;
                BottomRightQuad /= PixelPerQuad;
            //}
            //else if(GridWidth % 2 != 0)
            //{
            //    TopLeftQuad /= PixelPerQuad;
            //    TopRightQuad /= PixelPerQuad + GridHeight/2;
            //    BottomLeftQuad /= PixelPerQuad;
            //    BottomRightQuad /= PixelPerQuad + GridHeight/2;
            //}
            //else if(GridHeight % 2 != 0)
            //{
            //    TopLeftQuad /= PixelPerQuad;
            //    TopRightQuad /= PixelPerQuad;
            //    BottomLeftQuad /= PixelPerQuad + GridWidth/2;
            //    BottomRightQuad /= PixelPerQuad + GridWidth/2;
            //}
            //else
            //{
            //    TopLeftQuad /= PixelPerQuad;
            //    TopRightQuad /= (PixelPerQuad + GridHeight / 2);
            //    BottomLeftQuad /= (PixelPerQuad + GridWidth / 2);
            //    BottomRightQuad /= (PixelPerQuad + GridHeight / 2 + GridWidth / 2 + 1);
            //}
        }

        public void AddPixel(Bitmap bm, Program.ColorMask cm, int xPos, int yPos)
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
    }
}
