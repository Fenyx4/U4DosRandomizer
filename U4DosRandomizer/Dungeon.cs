using System;
using System.Collections.Generic;
using System.IO;

namespace U4DosRandomizer
{
    internal class Dungeon
    {

        private byte[,,] map = new byte[8, 8, 8];
        private List<byte[]> rooms = new List<byte[]>();

        public Dungeon(FileStream dngStream)
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
    }
}