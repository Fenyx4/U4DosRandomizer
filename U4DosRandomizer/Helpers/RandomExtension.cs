using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer.Helpers
{
    public static class RandomExtension
    {
        public static int FavorLower(this Random random, int min, int max)
        {
            return Convert.ToInt32(Math.Floor(Math.Abs(random.NextDouble() - random.NextDouble()) * (1 + max - min) + min));
        }

        public static double NextDouble(this Random random, double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
