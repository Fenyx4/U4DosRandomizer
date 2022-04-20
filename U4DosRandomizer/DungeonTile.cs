using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class DungeonTile : ITile
    {
        private int l_value = 0;
        public int L { get
            {
                return l_value;
            }
            private set
            {
                l_value = value;
            }
        }

        private byte x_value = 0;
        public byte X { get
            {
                return x_value;
            } 
            internal set
            {
                x_value = value;
            } 
        }
        private byte y_value = 0;
        public byte Y { get
            {
                return y_value;
            }
            internal set
            {
                y_value = value;
            }
        }

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
            return map[l_value, x_value, y_value];
        }

        public void SetTile(byte tile)
        {
            map[l_value, x_value, y_value] = tile;
        }

        public IEnumerable<DungeonTile> NeighborsSameLevel()
        {
            DungeonTile[] neighbors = new DungeonTile[4];
            neighbors[0] = new DungeonTile(l_value, x_value - 1, y_value, map);
            neighbors[1] = new DungeonTile(l_value, x_value + 1, y_value, map);
            neighbors[2] = new DungeonTile(l_value, x_value, y_value - 1, map);
            neighbors[3] = new DungeonTile(l_value, x_value, y_value + 1, map);

            return neighbors;
        }

        public IEnumerable<DungeonTile> NeighborsSameLevelAndAdjacent()
        {
            DungeonTile[] neighbors = new DungeonTile[8];
            neighbors[0] = new DungeonTile(l_value, x_value - 1, y_value, map);
            neighbors[1] = new DungeonTile(l_value, x_value + 1, y_value, map);
            neighbors[2] = new DungeonTile(l_value, x_value, y_value - 1, map);
            neighbors[3] = new DungeonTile(l_value, x_value, y_value + 1, map);
            neighbors[4] = new DungeonTile(l_value, x_value - 1, y_value - 1, map);
            neighbors[5] = new DungeonTile(l_value, x_value + 1, y_value - 1, map);
            neighbors[6] = new DungeonTile(l_value, x_value - 1, y_value + 1, map);
            neighbors[7] = new DungeonTile(l_value, x_value + 1, y_value + 1, map);

            return neighbors;
        }

        public IEnumerable<DungeonTile> KittyCorners()
        {
            DungeonTile[] neighbors = new DungeonTile[4];
            neighbors[0] = new DungeonTile(l_value, x_value - 1, y_value - 1, map);
            neighbors[1] = new DungeonTile(l_value, x_value + 1, y_value - 1, map);
            neighbors[2] = new DungeonTile(l_value, x_value - 1, y_value + 1, map);
            neighbors[3] = new DungeonTile(l_value, x_value + 1, y_value + 1, map);

            return neighbors;
        }

        private static int Wrap(int input)
        {
            return (input % 8 + 8) % 8;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(l_value, x_value, y_value);
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