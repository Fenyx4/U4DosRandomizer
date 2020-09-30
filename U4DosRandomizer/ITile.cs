using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public interface ITile : ICoordinate
    {
        byte GetTile();
        IEnumerable<Tile> NeighborAndAdjacentCoordinates();
    }
}
