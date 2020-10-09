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

		void ITile.SetTile(byte tile)
		{
			_coord.SetTile(tile);
		}

		public IEnumerable<ITile> NeighborAndAdjacentCoordinates()
        {
			return _coord.NeighborAndAdjacentCoordinates();
        }

		public IEnumerable<ITile> NeighborCoordinates()
		{
			return _coord.NeighborCoordinates();
		}

		#endregion
	}
}
