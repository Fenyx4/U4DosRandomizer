using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public interface ITile : ICoordinate
    {
        byte GetTile();
        void SetTile(byte tile);
        IEnumerable<ITile> NeighborAndAdjacentCoordinates();
        IEnumerable<ITile> NeighborCoordinates();
    }
}
