using System;

namespace U4DosRandomizer
{
    public class MapTiles
    {
        private byte[,] _mapTiles;
        private byte[,] _clothMapTiles;
        public byte Get(int x, int y)
        {
            return _mapTiles[x, y];
        }

        public void Set(byte x, byte y, byte tile, bool setCloth = false)
        {
            _mapTiles[x, y] = tile;
            if (setCloth)
            {
                for (int xOffset = 0; xOffset < 4; xOffset++)
                {
                    for (int yOffset = 0; yOffset < 4; yOffset++)
                    {
                        _clothMapTiles[x * 4 + xOffset, y * 4 + yOffset] = tile;
                    }
                }
            }
        }

        public void SetWorldMapTiles(byte[,] worldMapTiles)
        {
            _mapTiles = worldMapTiles;
        }

        internal void SetClothMapTiles(byte[,] clothMapTiles)
        {
            _clothMapTiles = clothMapTiles;
        }

        internal byte GetCloth(int x, int y)
        {
            return _clothMapTiles[x, y];
        }

        public void SetCloth(int x, int y, byte tile)
        {
            _clothMapTiles[x, y] = tile;
        }
    }
}