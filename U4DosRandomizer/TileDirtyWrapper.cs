using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class TileDirtyWrapper : ITile
    {
        private Tile _original;
		private Tile _current;

        public byte X 
		{
			get
			{
				return _current.X;
			}
			internal set
            {
				_current.X = value;
            }

		}
		public byte Y 
		{
			get
			{
				return _current.Y;
			}
			internal set
			{
				_current.Y = value;
			}
		}

		public TileDirtyWrapper(byte x, byte y, byte[,] tiles, Tile.ByteWrapper wrapper)
		{
			_original = new Tile(x, y, tiles, wrapper);
			_current = new Tile(x, y, tiles, wrapper);
		}

		public TileDirtyWrapper(int x, int y, byte[,] tiles, Tile.ByteWrapper wrapper)
			: this(wrapper(x), wrapper(y), tiles, wrapper)
		{
		}

        public TileDirtyWrapper(Tile tile, IWorldMap worldMap)
        {
			_original = worldMap.GetCoordinate(tile.X, tile.Y);
			_current = worldMap.GetCoordinate(tile.X, tile.Y);
		}

        public byte GetTile()
        {
			return _current.GetTile();
        }

		public void SetTile(byte tile)
        {
			_current.SetTile(tile);
        }

		public override int GetHashCode()
		{
			return _current.GetHashCode();
		}

		public override bool Equals(Object obj)
		{
			if (obj == null || !(obj is ITile))
				return false;
			else
				return X == ((ITile)obj).X && Y == ((ITile)obj).Y;
		}

		public IEnumerable<ITile> NeighborCoordinates()
		{
			return _current.NeighborCoordinates();
		}

		public IEnumerable<ITile> NeighborAndAdjacentCoordinates()
		{
			return _current.NeighborAndAdjacentCoordinates();
		}

        public bool IsDirty()
		{
			return !_current.Equals(_original);
		}

        internal TileDirtyWrapper Copy(IWorldMap worldMap)
        {
			return new TileDirtyWrapper(worldMap.GetCoordinate(X, Y), worldMap);
        }
    }
}