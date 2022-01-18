using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class DungeonTile
    {
        public int L { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        private byte[,,] map;

        public DungeonTile(int l, int x, int y, byte[,,] map)
        {
            this.L = l;
            this.X = Wrap(x);
            this.Y = Wrap(y);
            this.map = map;
        }

        public byte GetTile()
        {
            return map[L, X, Y];
        }

        public void SetTile(byte tile)
        {
            map[L, X, Y] = tile;
        }

        public IEnumerable<DungeonTile> NeighborsSameLevel()
        {
            DungeonTile[] neighbors = new DungeonTile[4];
            neighbors[0] = new DungeonTile(L, X - 1, Y, map);
            neighbors[1] = new DungeonTile(L, X + 1, Y, map);
            neighbors[2] = new DungeonTile(L, X, Y - 1, map);
            neighbors[3] = new DungeonTile(L, X, Y + 1, map);

            return neighbors;
        }

        public IEnumerable<DungeonTile> NeighborsSameLevelAndAdjacent()
        {
            DungeonTile[] neighbors = new DungeonTile[8];
            neighbors[0] = new DungeonTile(L, X - 1, Y, map);
            neighbors[1] = new DungeonTile(L, X + 1, Y, map);
            neighbors[2] = new DungeonTile(L, X, Y - 1, map);
            neighbors[3] = new DungeonTile(L, X, Y + 1, map);
            neighbors[4] = new DungeonTile(L, X - 1, Y - 1, map);
            neighbors[5] = new DungeonTile(L, X + 1, Y - 1, map);
            neighbors[6] = new DungeonTile(L, X - 1, Y + 1, map);
            neighbors[7] = new DungeonTile(L, X + 1, Y + 1, map);

            return neighbors;
        }

        public IEnumerable<DungeonTile> KittyCorners()
        {
            DungeonTile[] neighbors = new DungeonTile[4];
            neighbors[0] = new DungeonTile(L, X - 1, Y - 1, map);
            neighbors[1] = new DungeonTile(L, X + 1, Y - 1, map);
            neighbors[2] = new DungeonTile(L, X - 1, Y + 1, map);
            neighbors[3] = new DungeonTile(L, X + 1, Y + 1, map);

            return neighbors;
        }

        private static int Wrap(int input)
        {
            return (input % 8 + 8) % 8;
        }

        public override int GetHashCode()
        {
            int result = (int)(X ^ (X >> 32));
            result = 31 * result + (int)(Y ^ (Y >> 32));
            result = 31 * result + (int)(L ^ (L >> 32));
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !(obj is DungeonTile))
                return false;
            else
                return X == ((DungeonTile)obj).X && Y == ((DungeonTile)obj).Y && L == ((DungeonTile)obj).L;
        }
    }
}