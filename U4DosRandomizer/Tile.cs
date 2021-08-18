using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class Tile : ITile
    {
		public delegate byte ByteWrapper(int v);
		public byte X { get; internal set; }
        public byte Y { get; internal set; }
        private byte[,] WorldMapTiles { get; set; }
		private byte[,] ClothMapTiles { get; set; }
		private ByteWrapper _wrapper;

		public Tile(byte x, byte y, byte[,] worldMapTiles, ByteWrapper wrapper)
		{
			_wrapper = wrapper;
			this.X = x;
			this.Y = y;
			this.WorldMapTiles = worldMapTiles;
		}

		public Tile(int x, int y, byte[,] worldMapTiles, ByteWrapper wrapper)
		{
			_wrapper = wrapper;
			this.X = _wrapper(x);
			this.Y = _wrapper(y);
			this.WorldMapTiles = worldMapTiles;
		}

		public byte GetTile()
        {
			return WorldMapTiles[X, Y];
        }

		public void SetTile(byte tile)
        {
			WorldMapTiles[X, Y] = tile;
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
				return X == ((Tile)obj).X && Y == ((Tile)obj).Y && WorldMapTiles == ((Tile)obj).WorldMapTiles;
		}

		public IEnumerable<ITile> NeighborCoordinates()
		{
			Tile[] neighbors = new Tile[4];
			neighbors[0] = new Tile(X - 1, Y, WorldMapTiles, _wrapper);
			neighbors[1] = new Tile(X + 1, Y, WorldMapTiles, _wrapper);
			neighbors[2] = new Tile(X, Y - 1, WorldMapTiles, _wrapper);
			neighbors[3] = new Tile(X, Y + 1, WorldMapTiles, _wrapper);

			return neighbors;
		}

		public IEnumerable<ITile> NeighborAndAdjacentCoordinates()
		{
			Tile[] neighbors = new Tile[8];
			neighbors[0] = new Tile(X - 1, Y, WorldMapTiles, _wrapper);
			neighbors[1] = new Tile(X + 1, Y, WorldMapTiles, _wrapper);
			neighbors[2] = new Tile(X, Y - 1, WorldMapTiles, _wrapper);
			neighbors[3] = new Tile(X, Y + 1, WorldMapTiles, _wrapper);
			neighbors[4] = new Tile(X - 1, Y - 1, WorldMapTiles, _wrapper);
			neighbors[5] = new Tile(X + 1, Y - 1, WorldMapTiles, _wrapper);
			neighbors[6] = new Tile(X - 1, Y + 1, WorldMapTiles, _wrapper);
			neighbors[7] = new Tile(X + 1, Y + 1, WorldMapTiles, _wrapper);

			return neighbors;
		}
    }
}