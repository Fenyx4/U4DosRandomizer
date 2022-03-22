using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class DungeonTile : ITile
    {
        public int L { get; private set; }

        public byte X { get; internal set; }
        public byte Y { get; internal set; }

        private byte[,,] map;

        public DungeonTile(int l, int x, int y, byte[,,] map)
        {
            this.L = l;
            this.X = (byte)Wrap(x);
            this.Y = (byte)Wrap(y);
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

        public IEnumerable<ITile> NeighborCoordinates()
        {
            DungeonTile[] neighbors = null;
            int idx = 0;
            if(L == 0)
            {
                neighbors = new DungeonTile[5];
                neighbors[idx++] = new DungeonTile(L + 1, X, Y, map);
            }
            else if(L == 7)
            {
                neighbors = new DungeonTile[5];
                neighbors[idx++] = new DungeonTile(L - 1, X, Y, map);
            }
            else 
            {
                neighbors = new DungeonTile[6];
                neighbors[idx++] = new DungeonTile(L + 1, X, Y, map);
                neighbors[idx++] = new DungeonTile(L - 1, X, Y, map);
            }
            neighbors[idx++] = new DungeonTile(L, X - 1, Y, map);
            neighbors[idx++] = new DungeonTile(L, X + 1, Y, map);
            neighbors[idx++] = new DungeonTile(L, X, Y - 1, map);
            neighbors[idx++] = new DungeonTile(L, X, Y + 1, map);

            return neighbors;
        }

        public IEnumerable<ITile> NeighborAndAdjacentCoordinates()
        {
            DungeonTile[] neighbors = null;
            int idx = 0;
            if (L == 0)
            {
                neighbors = new DungeonTile[9];
                neighbors[idx++] = new DungeonTile(L + 1, X, Y, map);
            }
            else if (L == 7)
            {
                neighbors = new DungeonTile[9];
                neighbors[idx++] = new DungeonTile(L - 1, X, Y, map);
            }
            else
            {
                neighbors = new DungeonTile[10];
                neighbors[idx++] = new DungeonTile(L + 1, X, Y, map);
                neighbors[idx++] = new DungeonTile(L - 1, X, Y, map);
            }
            neighbors[idx++] = new DungeonTile(L, X - 1, Y, map);
            neighbors[idx++] = new DungeonTile(L, X + 1, Y, map);
            neighbors[idx++] = new DungeonTile(L, X, Y - 1, map);
            neighbors[idx++] = new DungeonTile(L, X, Y + 1, map);
            neighbors[idx++] = new DungeonTile(L, X - 1, Y - 1, map);
            neighbors[idx++] = new DungeonTile(L, X + 1, Y - 1, map);
            neighbors[idx++] = new DungeonTile(L, X - 1, Y + 1, map);
            neighbors[idx++] = new DungeonTile(L, X + 1, Y + 1, map);

            return neighbors;
        }
    }
}