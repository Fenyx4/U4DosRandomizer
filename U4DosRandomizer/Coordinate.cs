using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class Coordinate : ICoordinate
    {
        public byte X { get; internal set; }
        public byte Y { get; internal set; }
        private byte[,] Tiles { get; set; }

		public Coordinate(byte x, byte y, byte[,] tiles)
		{
			this.X = x;
			this.Y = y;
			this.Tiles = tiles;
		}

		public Coordinate(int x, int y, byte[,] tiles)
		{
			this.X = Convert.ToByte(x);
			this.Y = Convert.ToByte(y);
			this.Tiles = tiles;
		}

		public byte GetTile()
        {
			return Tiles[X, Y];
        }

		public void SetTile(byte tile)
        {
			Tiles[X, Y] = tile;
        }

		public override int GetHashCode()
		{
			return X * 31 + Y;
		}

		public override bool Equals(Object obj)
		{
			if (obj == null || !(obj is Coordinate))
				return false;
			else
				return X == ((Coordinate)obj).X && Y == ((Coordinate)obj).Y && Tiles == ((Coordinate)obj).Tiles;
		}

		public IEnumerable<Coordinate> NeighborCoordinates()
		{
			Coordinate[] neighbors = new Coordinate[4];
			neighbors[0] = new Coordinate(Wrap(X - 1), Y, Tiles);
			neighbors[1] = new Coordinate(Wrap(X + 1), Y, Tiles);
			neighbors[2] = new Coordinate(X, Wrap(Y - 1), Tiles);
			neighbors[3] = new Coordinate(X, Wrap(Y + 1), Tiles);

			return neighbors;
		}

		public static byte Wrap(int input)
        {
			return Wrap(input, WorldMap.SIZE);
        }

		public static byte Wrap(int input, int divisor)
		{
			return Convert.ToByte((input % divisor + divisor) % divisor);
		}
	}
}