﻿using System;
using System.Collections.Generic;
using System.IO;

namespace U4DosRandomizer
{
    public class Dungeon
    {

        private byte[,,] map = new byte[8, 8, 8];
        private List<byte[]> rooms = new List<byte[]>();

        private List<DungeonTile> immuneTiles = new List<DungeonTile>();
        private Dictionary<int, DungeonTile> dungeonTileHash = new Dictionary<int, DungeonTile>();

        public Dungeon(byte[,,] map, List<byte[]> rooms, List<DungeonTile> immuneTiles)
        {
            this.map = map;
            this.rooms = rooms;

            for (int l = 0; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        dungeonTileHash.Add((l * 8 * 8) + x + y * 8, new DungeonTile(l, x, y, this, map));
                    }
                }
            }

            if (immuneTiles != null)
            {
                foreach (var tile in immuneTiles)
                {
                    this.immuneTiles.Add(GetTile(tile.L, tile.X, tile.Y));
                }
            }
        }

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

            for (int l = 0; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        dungeonTileHash.Add((l * 8 * 8) + x + y * 8, new DungeonTile(l, x, y, this, map));
                    }
                }
            }
        }

        public bool ValidateTile(DungeonTile tile)
        {
            var right = GetTile(tile.L, tile.X + 1, tile.Y).GetTile();
            right = right == DungeonTileInfo.Wall ? DungeonTileInfo.Wall : DungeonTileInfo.Nothing;
            var below = GetTile(tile.L, tile.X, tile.Y + 1).GetTile(); ;
            below = below == DungeonTileInfo.Wall ? DungeonTileInfo.Wall : DungeonTileInfo.Nothing;
            var kitty = GetTile(tile.L, tile.X + 1, tile.Y + 1).GetTile();
            kitty = kitty == DungeonTileInfo.Wall ? DungeonTileInfo.Wall : DungeonTileInfo.Nothing;
            if (tile.GetTile() == right &&
                tile.GetTile() == below &&
                tile.GetTile() == kitty)
            {
                return false;
            }

            if (tile.GetTile() != right &&
                tile.GetTile() != below &&
                tile.GetTile() == kitty)
            {
                return false;
            }

            return true;
        }

        internal byte GetTileValue(int l, byte x, byte y)
        {
            return map[l, x, y];
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
                            results.Add(GetTile(l, x, y));
                        }
                    }
                }
            }

            return results;
        }

        public List<DungeonTile> GetTiles()
        {
            var results = new List<DungeonTile>();

            foreach (var tile in dungeonTileHash.Values)
            {
                results.Add(tile);
            }

            return results;
        }

        internal void AddImmuneTile(DungeonTile dungeonTile)
        {
            immuneTiles.Add(dungeonTile);
        }

        public DungeonTile GetTile(int level, int x, int y)
        {
            return dungeonTileHash[(level * 8 * 8) + Wrap(x) + Wrap(y) * 8];
        }

        public void SetTile(int l, int x, int y, byte tile)
        {
            if(immuneTiles.Contains(GetTile(l, x, y)))
            {
                Console.WriteLine("Trying to change an immune tile");
            }
            map[l, Wrap(x) , Wrap(y)] = tile;
        }

        public int GetWidth()
        {
            return 8;
        }

        public int GetHeight()
        {
            return 8;
        }

        public Dungeon Copy()
        {
            var mapCopy = new byte[8, 8, 8];
            for(int l = 0; l < 8; l++)
            {
                for(int x = 0; x < 8; x++)
                {
                    for(int y = 0; y < 8; y++)
                    {
                        mapCopy[l, x, y] = map[l, x, y];
                    }
                }
            }

            var roomsCopy = new List<byte[]>();
            for(int i = 0; i < roomsCopy.Count; i++)
            {
                roomsCopy.Add(new byte[rooms[i].Length]);
                for(int j = 0; j < 8; j++)
                {
                    roomsCopy[i][j] = roomsCopy[i][j];
                }
            }

            return new Dungeon(mapCopy, roomsCopy, immuneTiles);
        }

        private static int Wrap(int input)
        {
            return (input % 8 + 8) % 8;
        }

        internal List<DungeonTile> GetImmuneTiles()
        {
            var result = new List<DungeonTile>();
            result.AddRange(immuneTiles);
            return result; 
        }

        public void ClearImmuneTiles()
        {
            immuneTiles.Clear();
        }
    }
}