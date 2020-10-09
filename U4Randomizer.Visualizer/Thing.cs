using System;
using System.Collections.Generic;
using System.Text;

namespace U4Randomizer.Visualizer
{
    public class Thing
    {
        public byte X { get; set; }
        public byte Y { get; set; }

        public double ForceX { get; set; }
        public double ForceY { get; set; }


        public static byte Wrap(int input)
        {
            return Wrap(input, 256);
        }

        public static byte Wrap(double input)
        {
            return Wrap((int)Math.Round(input), 256);
        }

        public static byte Wrap(int input, int divisor)
        {
            return Convert.ToByte((input % divisor + divisor) % divisor);
        }
    }
}
