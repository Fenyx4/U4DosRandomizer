using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class Tile : ITile
    {
		public delegate byte ByteWrapper(int v);
		public byte X { get; internal set; }
        public byte Y { get; internal set; }
        private byte[,] Tiles { get; set; }
		private ByteWrapper _wrapper;

		public Tile(byte x, byte y, byte[,] tiles, ByteWrapper wrapper)
		{
			_wrapper = wrapper;
			this.X = x;
			this.Y = y;
			this.Tiles = tiles;
		}

		public Tile(int x, int y, byte[,] tiles, ByteWrapper wrapper)
		{
			_wrapper = wrapper;
			this.X = _wrapper(x);
			this.Y = _wrapper(y);
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
			neighbors[0] = new Tile(X - 1, Y, Tiles, _wrapper);
			neighbors[1] = new Tile(X + 1, Y, Tiles, _wrapper);
			neighbors[2] = new Tile(X, Y - 1, Tiles, _wrapper);
			neighbors[3] = new Tile(X, Y + 1, Tiles, _wrapper);

			return neighbors;
		}

		public IEnumerable<Tile> NeighborAndAdjacentCoordinates()
		{
			Tile[] neighbors = new Tile[8];
			neighbors[0] = new Tile(X - 1, Y, Tiles, _wrapper);
			neighbors[1] = new Tile(X + 1, Y, Tiles, _wrapper);
			neighbors[2] = new Tile(X, Y - 1, Tiles, _wrapper);
			neighbors[3] = new Tile(X, Y + 1, Tiles, _wrapper);
			neighbors[4] = new Tile(X - 1, Y - 1, Tiles, _wrapper);
			neighbors[5] = new Tile(X + 1, Y - 1, Tiles, _wrapper);
			neighbors[6] = new Tile(X - 1, Y + 1, Tiles, _wrapper);
			neighbors[7] = new Tile(X + 1, Y + 1, Tiles, _wrapper);

			return neighbors;
		}
	}
}