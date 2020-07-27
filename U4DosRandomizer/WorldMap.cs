using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class WorldMap
    {
        public static void MoveBuildings(byte[,] worldMapUlt, UltimaData data)
        {
            foreach (var loc in data.LCB)
            {
                worldMapUlt[loc.X, loc.Y] = loc.Tile;
            }

            foreach (var loc in data.Castles)
            {
                worldMapUlt[loc.X, loc.Y] = loc.Tile;
            }

            foreach (var loc in data.Towns)
            {
                worldMapUlt[loc.X, loc.Y] = loc.Tile;
            }
        }
    }
}
