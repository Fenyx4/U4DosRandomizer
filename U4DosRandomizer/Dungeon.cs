using System;
using System.Collections.Generic;
using System.IO;

namespace U4DosRandomizer
{
    public class Dungeon
    {

        private byte[,,] map = new byte[8, 8, 8];
        private List<byte[]> rooms = new List<byte[]>();

        public Dungeon(Stream dngStream, UltimaData data)
        {
            BinaryReader readBinary = new BinaryReader(dngStream);

            // Load each level
            for (int l = 0; l < 8; l++)
            {
                byte[] levelArr = new byte[8 * 8];
                readBinary.Read(levelArr, 0, 8 * 8);
                
                for(int i = 0; i < levelArr.Length; i++)
                {
                    map[l, i / 8, i % 8] = levelArr[i];
                }
            }

            // Load each room
            byte[] roomArr = new byte[256];
            while(readBinary.Read(roomArr,0,256) == 256)
            {
                byte[] roomClone = new byte[256];
                roomArr.CopyTo(roomClone, 0);
                rooms.Add(roomClone);
            }
        }

        public IEnumerable<DungeonTile> Tiles(int level)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    yield return GetTile(level, row, col);
                }
            }
        }

        public byte[] Save()
        {
            var dungeonBytes = new byte[8 * 8 * 8 + 256 * rooms.Count];
            int idx = 0;
            for(int l = 0; l < 8; l++)
            {
                for(int x = 0; x < 8; x++)
                {
                    for(int y = 0; y < 8; y++)
                    {
                        dungeonBytes[idx++] = map[l, x, y];
                    }
                }
            }

            for(int i = 0; i < rooms.Count; i++)
            {
                for(int j = 0; j < 256; j++)
                {
                    dungeonBytes[idx++] = rooms[i][j];
                }
            }

            return dungeonBytes;
        }

        public List<DungeonTile> GetTiles(byte tile)
        {
            var results = new List<DungeonTile>();
            for(int l = 0; l < 8; l++)
            {
                for(int x = 0; x < 8; x++)
                {
                    for(int y = 0; y < 8; y++)
                    {
                        if(map[l, x, y] == tile)
                        {
                            results.Add(new DungeonTile(l, x, y, map));
                        }
                    }
                }
            }

            return results;
        }

        public DungeonTile GetTile(int level, int x, int y)
        {
            return new DungeonTile(level, x, y, map);
        }

        public void SetTile(int l, int x, int y, byte tile)
        {
            map[l, x % 8 , y % 8] = tile;
        }
    }
}