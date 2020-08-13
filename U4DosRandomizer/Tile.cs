using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class Tile : ITile
    {
        public byte X { get; internal set; }
        public byte Y { get; internal set; }
        private byte[,] Tiles { get; set; }

		public Tile(byte x, byte y, byte[,] tiles)
		{
			this.X = x;
			this.Y = y;
			this.Tiles = tiles;
		}

		public Tile(int x, int y, byte[,] tiles)
		{
			this.X = Wrap(x);
			this.Y = Wrap(y);
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
			if (obj == null || !(obj is Tile))
				return false;
			else
				return X == ((Tile)obj).X && Y == ((Tile)obj).Y && Tiles == ((Tile)obj).Tiles;
		}

		public IEnumerable<Tile> NeighborCoordinates()
		{
			Tile[] neighbors = new Tile[4];
			neighbors[0] = new Tile(Wrap(X - 1), Y, Tiles);
			neighbors[1] = new Tile(Wrap(X + 1), Y, Tiles);
			neighbors[2] = new Tile(X, Wrap(Y - 1), Tiles);
			neighbors[3] = new Tile(X, Wrap(Y + 1), Tiles);

			return neighbors;
		}

		public IEnumerable<Tile> NeighborAndAdjacentCoordinates()
		{
			Tile[] neighbors = new Tile[8];
			neighbors[0] = new Tile(Wrap(X - 1), Y, Tiles);
			neighbors[1] = new Tile(Wrap(X + 1), Y, Tiles);
			neighbors[2] = new Tile(X, Wrap(Y - 1), Tiles);
			neighbors[3] = new Tile(X, Wrap(Y + 1), Tiles);
			neighbors[4] = new Tile(Wrap(X - 1), Wrap(Y - 1), Tiles);
			neighbors[5] = new Tile(Wrap(X + 1), Wrap(Y - 1), Tiles);
			neighbors[6] = new Tile(Wrap(X - 1), Wrap(Y - 1), Tiles);
			neighbors[7] = new Tile(Wrap(X + 1), Wrap(Y + 1), Tiles);

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