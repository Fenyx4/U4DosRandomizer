using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public interface ICoordinate
    {
        byte X { get; }
        byte Y { get; }

        byte GetTile();
    }
}
