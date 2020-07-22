using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace U4DosRandomizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var worldMapDS = new DiamondSquare(256, 184643518.256878, 82759876).getData(9726547);

            var image = ToBitmap(worldMapDS);
            //FileStream stream = new FileStream("worldMap.bmp", FileMode.Create);
            image.Save("worldMap.bmp");
        }

        private static Bitmap ToBitmap(double[,] worldMapDS)
        {
            var worldMapFlattened = new double[255*255];

            for(int x = 0; x < 255; x++)
            {
                for(int y = 0; y < 255; y++)
                {
                    worldMapFlattened[x + y * 255] = worldMapDS[x, y];
                    //int res = Convert.ToInt32((worldMapDS[x, y] / double.MaxValue)*255);
                    //Color newColor = Color.FromArgb(res, res, res);
                    //image.SetPixel(x, y, newColor);
                }
            }

            var min = worldMapFlattened.Min();
            var max = worldMapFlattened.Max();

            var image = new Bitmap(256, 256);
            for (int x = 0; x < 255; x++)
            {
                for (int y = 0; y < 255; y++)
                {
                    int res = Convert.ToInt32(Linear(worldMapDS[x, y], min, max, 0, 7));
                    res = Convert.ToInt32(Linear(res, 0, 7, 0, 255));
                    Color newColor = Color.FromArgb(res, res, res);
                    image.SetPixel(x, y, newColor);
                }
            }


            return image;
        }

        static public double Linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }
    }
}
