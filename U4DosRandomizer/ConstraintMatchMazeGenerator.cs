using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class ConstraintMatchMazeGenerator : IMazeGenerator
    {
        private static int north = 0;
        private static int east = 1;
        private static int west = 2;
        private static int south = 3;

        public void GenerateMaze(string dungeonName, Dungeon dungeon, int numLevels, int width, int height, Random rand)
        {
            var quadTileList = new List<int> { 0, 1, 2, 3 };
            var valid = true;

            // Loop through every tile
            for (int l = 0; l < numLevels && valid; l++)
            {
                valid = false;
                while (!valid)
                {
                    dungeon.ClearImmuneTiles();
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            
                            dungeon.GetTile(l,x,y).SetTile(DungeonTileInfo.Nothing);
                        }
                    }
                    if (l == 7)
                    {
                        PlaceAltar(dungeonName, dungeon, numLevels, width, height, rand);
                    }
                    valid = true;
                    //for(int x = 0; x < width; x++)
                    //{
                    //    for(int y = 0; y < height; y++)
                    //    {
                    //        if(dungeon.GetTile(l,x,y).GetTile() == DungeonTileInfo.Nothing)
                    //        {
                    //            dungeon.GetTile(l, x, y).SetTile(0xF1);
                    //        }
                    //    }
                    //}
                    for (int x = 0; x < width && valid; x++)
                    {
                        for (int y = 0; y < height && valid; y++)
                        {
                            quadTileList.Shuffle(rand);

                            valid = false;
                            for (int qIdx = 0; qIdx < quadTileList.Count && !valid; qIdx++)
                            {
                                var quadTileOffset = quadTileList[qIdx];

                                // The four tiles will be invalid because they will all be nothing (Not allowed in an Ultima map)
                                // So choose a random tile an make it a wall
                                var quadTile = dungeon.GetTile(l, x + quadTileOffset % 2, y + quadTileOffset / 2);
                                if (!dungeon.GetImmuneTiles().Contains(quadTile))
                                {
                                    quadTile.SetTile(DungeonTileInfo.Wall);

                                    // Check if that tile is valid in the grander scale of things
                                    valid = true;
                                    for (int i = 0; i < 2; i++)
                                    {
                                        for (int j = 0; j < 2; j++)
                                        {
                                            var surroundingTile = dungeon.GetTile(quadTile.L, quadTile.X - 1 + i, quadTile.Y - 1 + j);
                                            valid = valid && dungeon.ValidateTile(surroundingTile);
                                        }
                                    }

                                    if (!valid)
                                    {
                                        quadTile.SetTile(DungeonTileInfo.Nothing);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Need space for a ladder at 0,1,1
            if (dungeon.GetTile(0, 1, 1).GetTile() != DungeonTileInfo.Nothing)
            {
                DungeonTile tile = null;
                for (int x = 0; x < width && tile == null; x++)
                {
                    for (int y = 0; y < height && tile == null; y++)
                    {
                        if (dungeon.GetTile(0, x, y).GetTile() == DungeonTileInfo.Nothing)
                        {
                            tile = dungeon.GetTile(0, x, y);
                        }
                    }
                }

                var x_offset = 1 - tile.X;
                var y_offset = 1 - tile.Y;

                var dungeonCopy = dungeon.Copy();

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        dungeon.SetTile(0, x + x_offset, y + y_offset, dungeonCopy.GetTile(0, x, y).GetTile());
                    }
                }
            }

            return;
        }

        private void PlaceAltar(string dungeonName, Dungeon dungeon, int numLevels, int width, int height, Random rand)
        {
            var truth = false;
            var truthEntrance = north;
            var love = false;
            var loveEntrance = north;
            var courage = false;
            var courageEntrance = north;
  
            if(dungeonName.ToLower() == "Despise".ToLower())
            {
                love = true;
                loveEntrance = north;
            }

            if (dungeonName.ToLower() == "Deceit".ToLower())
            {
                truth = true;
                truthEntrance = north;
            }

            if (dungeonName.ToLower() == "Destard".ToLower())
            {
                courage = true;
                courageEntrance = north;
            }

            if (dungeonName.ToLower() == "Covetous".ToLower())
            {
                love = true;
                loveEntrance = west;

                courage = true;
                courageEntrance = east;

            }

            if (dungeonName.ToLower() == "Wrong".ToLower())
            {
                love = true;
                loveEntrance = east;

                truth = true;
                truthEntrance = west;
            }

            if (dungeonName.ToLower() == "Shame".ToLower())
            {
                love = true;
                loveEntrance = east;

                courage = true;
                courageEntrance = west;
            }

            if (dungeonName.ToLower() == "Hythloth".ToLower())
            {
                truth = true;
                truthEntrance = south;

                love = true;
                loveEntrance = south;

                courage = true;
                courageEntrance = south;
            }

            var altars = new List<DungeonTile>();
            if (truth)
            {
                var x = rand.Next(0, 3);
                var y = rand.Next(height);
                var altarTile = dungeon.GetTile(7, x, y);
                altars.Add(altarTile);
                AltarAndWalls(dungeon, numLevels, truthEntrance, x, y);
            }

            if (love)
            {
                var x = 3;
                var y = 0;
                var altarTile = dungeon.GetTile(7, x, y);
                var valid = false;
                while(!valid)
                {
                    y = rand.Next(height);
                    altarTile = dungeon.GetTile(7, x, y);
                    valid = !altars.Any(a => a.NeighborAndAdjacentCoordinates().Contains(altarTile));
                }

                altars.Add(altarTile);
                AltarAndWalls(dungeon, numLevels, loveEntrance, x, y);
            }

            if (courage)
            {
                var x = rand.Next(4, 8);
                var y = 0;
                var altarTile = dungeon.GetTile(7, x, y);
                var valid = false;
                while (!valid)
                {
                    y = rand.Next(height);
                    altarTile = dungeon.GetTile(7, x, y);
                    valid = !altars.Any(a => a.NeighborAndAdjacentCoordinates().Contains(altarTile));
                }

                altars.Add(altarTile);
                AltarAndWalls(dungeon, numLevels, courageEntrance, x, y);
            }
        }

        private static void AltarAndWalls(Dungeon dungeon, int numLevels, int entranceDirection, int x, int y)
        {
            dungeon.SetTile(numLevels - 1, x, y, DungeonTileInfo.DungeonRoomStart + 15);
            dungeon.AddImmuneTile(dungeon.GetTile(numLevels - 1, x, y));

            if (entranceDirection != south)
            {
                dungeon.SetTile(numLevels - 1, x + 1, y, DungeonTileInfo.Wall);
                dungeon.AddImmuneTile(dungeon.GetTile(numLevels - 1, x + 1, y));
            }

            if (entranceDirection != east)
            {
                dungeon.SetTile(numLevels - 1, x, y + 1, DungeonTileInfo.Wall);
                dungeon.AddImmuneTile(dungeon.GetTile(numLevels - 1, x, y + 1));
            }

            if (entranceDirection != west)
            {
                dungeon.SetTile(numLevels - 1, x, y - 1, DungeonTileInfo.Wall);
                dungeon.AddImmuneTile(dungeon.GetTile(numLevels - 1, x, y - 1));
            }

            if (entranceDirection != north)
            {
                dungeon.SetTile(numLevels - 1, x - 1, y, DungeonTileInfo.Wall);
                dungeon.AddImmuneTile(dungeon.GetTile(numLevels - 1, x - 1, y));
            }
        }
    }



}
