using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class Coordinate : ICoordinate
    {
        public byte X { get; set; }

        public byte Y { get; set; }

		public Coordinate(byte x, byte y)
		{
			this.X = x;
			this.Y = y;
		}

		public Coordinate(int x, int y)
		{
			this.X = WorldMap.Wrap(x);
			this.Y = WorldMap.Wrap(y);
		}
	}
}
