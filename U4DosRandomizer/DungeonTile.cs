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
            private set
            {
                x_value = value;
            } 
        }
        private byte y_value = 0;
        public byte Y { get
            {
                return y_value;
            }
            private set
            {
                y_value = value;
            }
        }

        private byte[,,] map;
        private Dungeon dungeon;

        public DungeonTile(int l, int x, int y, Dungeon dungeon, byte[,,] map)
        {
            this.L = l;
            this.X = (byte)Wrap(x);
            this.Y = (byte)Wrap(y);
            this.map = map;
            this.dungeon = dungeon;
        }

        public byte GetTile() 
        {
            return map[l_value, x_value, y_value];
        }

        public void SetTile(byte tile)
        {
            dungeon.SetTile(l_value, x_value, y_value, tile);
        }

        public IEnumerable<DungeonTile> NeighborsSameLevel()
        {
            DungeonTile[] neighbors = new DungeonTile[4];
            neighbors[0] = dungeon.GetTile(l_value, x_value - 1, y_value);
            neighbors[1] = dungeon.GetTile(l_value, x_value + 1, y_value);
            neighbors[2] = dungeon.GetTile(l_value, x_value, y_value - 1);
            neighbors[3] = dungeon.GetTile(l_value, x_value, y_value + 1);

            return neighbors;
        }

        public IEnumerable<DungeonTile> NeighborsSameLevelAndAdjacent()
        {
            DungeonTile[] neighbors = new DungeonTile[8];
            neighbors[0] = dungeon.GetTile(l_value, x_value - 1, y_value);
            neighbors[1] = dungeon.GetTile(l_value, x_value + 1, y_value);
            neighbors[2] = dungeon.GetTile(l_value, x_value, y_value - 1);
            neighbors[3] = dungeon.GetTile(l_value, x_value, y_value + 1);
            neighbors[4] = dungeon.GetTile(l_value, x_value - 1, y_value - 1);
            neighbors[5] = dungeon.GetTile(l_value, x_value + 1, y_value - 1);
            neighbors[6] = dungeon.GetTile(l_value, x_value - 1, y_value + 1);
            neighbors[7] = dungeon.GetTile(l_value, x_value + 1, y_value + 1);

            return neighbors;
        }

        public IEnumerable<DungeonTile> KittyCorners()
        {
            DungeonTile[] neighbors = new DungeonTile[4];
            neighbors[0] = dungeon.GetTile(l_value, x_value - 1, y_value - 1);
            neighbors[1] = dungeon.GetTile(l_value, x_value + 1, y_value - 1);
            neighbors[2] = dungeon.GetTile(l_value, x_value - 1, y_value + 1);
            neighbors[3] = dungeon.GetTile(l_value, x_value + 1, y_value + 1);

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
                return x_value == ((DungeonTile)obj).x_value && y_value == ((DungeonTile)obj).y_value && l_value == ((DungeonTile)obj).l_value;
        }

        public List<ITile> WalkableNeighborCoordinates()
        {
            var neighbors = new List<ITile>();
            var tileValue = GetTile();
            if (l_value != 0 && (tileValue == DungeonTileInfo.LadderUp || tileValue == DungeonTileInfo.LadderBoth))
            {
                neighbors.Add(dungeon.GetTile(l_value - 1, x_value, y_value));
            }
            if (tileValue == DungeonTileInfo.LadderDown || tileValue == DungeonTileInfo.LadderBoth)
            {
                neighbors.Add(dungeon.GetTile(l_value + 1, x_value, y_value));
            }
            var tile = dungeon.GetTile(l_value, x_value - 1, y_value);
            if (tile.GetTile() != DungeonTileInfo.Wall)
            {
                neighbors.Add(tile);
            }
            tile = dungeon.GetTile(l_value, x_value + 1, y_value);
            if (tile.GetTile() != DungeonTileInfo.Wall)
            {
                neighbors.Add(tile);
            }
            tile = dungeon.GetTile(l_value, x_value, y_value - 1);
            if (tile.GetTile() != DungeonTileInfo.Wall)
            {
                neighbors.Add(tile);
            }
            tile = dungeon.GetTile(l_value, x_value, y_value + 1);
            if (tile.GetTile() != DungeonTileInfo.Wall)
            {
                neighbors.Add(tile);
            }

            return neighbors;
        }

        public IEnumerable<ITile> NeighborCoordinates()
        {
            DungeonTile[] neighbors = null;
            int idx = 0;
            if(l_value == 0)
            {
                neighbors = new DungeonTile[5];
                neighbors[idx++] = dungeon.GetTile(l_value + 1, x_value, y_value);
            }
            else if(l_value == 7)
            {
                neighbors = new DungeonTile[5];
                neighbors[idx++] = dungeon.GetTile(l_value - 1, x_value, y_value);
            }
            else 
            {
                neighbors = new DungeonTile[6];
                neighbors[idx++] = dungeon.GetTile(l_value + 1, x_value, y_value);
                neighbors[idx++] = dungeon.GetTile(l_value - 1, x_value, y_value);
            }
            neighbors[idx++] = dungeon.GetTile(l_value, x_value - 1, y_value);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value + 1, y_value);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value, y_value - 1);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value, y_value + 1);

            return neighbors;
        }

        public IEnumerable<ITile> NeighborAndAdjacentCoordinates()
        {
            DungeonTile[] neighbors = null;
            int idx = 0;
            if (l_value == 0)
            {
                neighbors = new DungeonTile[9];
                neighbors[idx++] = dungeon.GetTile(l_value + 1, x_value, y_value);
            }
            else if (l_value == 7)
            {
                neighbors = new DungeonTile[9];
                neighbors[idx++] = dungeon.GetTile(l_value - 1, x_value, y_value);
            }
            else
            {
                neighbors = new DungeonTile[10];
                neighbors[idx++] = dungeon.GetTile(l_value + 1, x_value, y_value);
                neighbors[idx++] = dungeon.GetTile(l_value - 1, x_value, y_value);
            }
            neighbors[idx++] = dungeon.GetTile(l_value, x_value - 1, y_value);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value + 1, y_value);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value, y_value - 1);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value, y_value + 1);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value - 1, y_value - 1);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value + 1, y_value - 1);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value - 1, y_value + 1);
            neighbors[idx++] = dungeon.GetTile(l_value, x_value + 1, y_value + 1);

            return neighbors;
        }
    }
}