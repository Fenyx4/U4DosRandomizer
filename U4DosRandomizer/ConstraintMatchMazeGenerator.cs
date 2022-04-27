using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class ContraintMatchMazeGenerator : IMazeGenerator
    {
        
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
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            
                            dungeon.GetTile(l,x,y).SetTile(DungeonTileInfo.Nothing);
                        }
                    }
                    valid = true;
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

            // Need space for a ladder at 0,1,1
            if(dungeon.GetTile(0,1,1).GetTile() != DungeonTileInfo.Nothing)
            {
                DungeonTile tile = null;
                for (int x = 0; x < width && tile == null; x++)
                {
                    for (int y = 0; y < height && tile == null; y++)
                    {
                        if(dungeon.GetTile(0, x, y).GetTile() == DungeonTileInfo.Nothing)
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

    }



}
