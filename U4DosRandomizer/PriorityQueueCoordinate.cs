using System;
using System.Collections.Generic;
using System.Text;
using U4DosRandomizer.BlueRajaPriorityQueue;

namespace U4DosRandomizer
{
	public class PriorityQueueCoordinate<T> : PriorityQueueNode, ITile where T : ITile
	{
		private T _coord;
		public PriorityQueueCoordinate(T coord)
		{
			_coord = coord;
		}

        public T GetCoord()
		{
			return _coord;
		}

        #region ICoordinate implementation


        public byte X => _coord.X;

		public byte Y => _coord.Y;

		byte ITile.GetTile()
		{
			return _coord.GetTile();

		}

        public IEnumerable<Tile> NeighborAndAdjacentCoordinates()
        {
			return _coord.NeighborAndAdjacentCoordinates();
        }

        #endregion
    }
}
