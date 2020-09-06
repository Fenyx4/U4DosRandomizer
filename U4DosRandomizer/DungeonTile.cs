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
            this.X = x;
            this.Y = y;
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
    }
}