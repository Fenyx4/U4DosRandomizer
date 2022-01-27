using System;

namespace U4DosRandomizer
{
    internal class NoDungeonGenerator : IMazeGenerator
    {
        public NoDungeonGenerator()
        {
        }

        public void GenerateMaze(string dungeonName, Dungeon dungeon, int numLevels, int width, int height, Random rand)
        {
            for(int l = 0; l < numLevels; l++)
            {
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        dungeon.SetTile(l, x, y, DungeonTileInfo.Wall);
                    }
                }
            }
            

            if (dungeonName.ToLower() != "abyss")
            {
                // Starting tile needs to exit dungeon
                dungeon.SetTile(0, 1, 1, DungeonTileInfo.LadderUp);
                for(int x = 2; x < 6; x++)
                {
                    dungeon.SetTile(0, x, 1, DungeonTileInfo.Nothing);
                }
                dungeon.SetTile(0, 6, 1, DungeonTileInfo.LadderDown);

                for (int l = 1; l < numLevels; l++)
                {
                    if (l != numLevels - 1)
                    {
                        dungeon.SetTile(l, 6, 1, DungeonTileInfo.LadderBoth);
                    }
                    else
                    {
                        dungeon.SetTile(l, 6, 1, DungeonTileInfo.LadderUp);
                    }
                }

                dungeon.SetTile(numLevels - 1, 6, 2, DungeonTileInfo.AltarOrStone);

                //// Altar of Truth needs to be in row 1 or 2
                //dungeon.SetTile(numLevels - 1, 1, 1, DungeonTileInfo.DungeonRoomStart + 15);
                //// Altar of Love needs to be in row 3
                //dungeon.SetTile(numLevels - 1, 3, 3, DungeonTileInfo.DungeonRoomStart + 15);


                if (dungeonName.ToLower() == "deceit")
                {
                    // Altar of Truth enter from the north needs to be in row 1 or 2
                    dungeon.SetTile(numLevels - 1, 1, 1, DungeonTileInfo.DungeonRoomStart + 15);
                    dungeon.SetTile(numLevels - 1, 7, 1, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 8, 1, DungeonTileInfo.Nothing);
                }
                if(dungeonName.ToLower() == "shame")
                {
                    // Altar of Truth enter from the east needs to be in row 1 or 2
                    dungeon.SetTile(numLevels - 1, 1, 1, DungeonTileInfo.DungeonRoomStart + 15);
                    dungeon.SetTile(numLevels - 1, 8, 2, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 1, 2, DungeonTileInfo.Nothing);

                    // Altar of Courage enter from the west needs to be in row 4-8
                    dungeon.SetTile(numLevels - 1, 5, 5, DungeonTileInfo.DungeonRoomStart + 15);
                    dungeon.SetTile(numLevels - 1, 1, 2, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 2, 2, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 3, 2, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 4, 2, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 5, 2, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 5, 3, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 5, 4, DungeonTileInfo.Nothing);
                }
                if (dungeonName.ToLower() == "wrong")
                {
                    // Altar of Truth enter from the west needs to be in row 1 or 2
                    dungeon.SetTile(numLevels - 1, 1, 1, DungeonTileInfo.DungeonRoomStart + 15);
                    dungeon.SetTile(numLevels - 1, 1, 8, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 8, 8, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 7, 8, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 6, 8, DungeonTileInfo.Nothing);

                    // Altar of Love enter from the east needs to be in row 3
                    dungeon.SetTile(numLevels - 1, 3, 3, DungeonTileInfo.DungeonRoomStart + 15);
                    dungeon.SetTile(numLevels - 1, 2, 8, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 3, 8, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 3, 7, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 3, 6, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 3, 5, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 3, 4, DungeonTileInfo.Nothing);

                }
                if (dungeonName.ToLower() == "despise")
                {
                    // Altar of Love enter from the north needs to be in row 3
                    dungeon.SetTile(numLevels - 1, 3, 3, DungeonTileInfo.DungeonRoomStart + 15);
                    dungeon.SetTile(numLevels - 1, 7, 1, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 8, 1, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 1, 1, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 2, 1, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 2, 2, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 2, 3, DungeonTileInfo.Nothing);
                }
                if (dungeonName.ToLower() == "covetous")
                {
                    // Altar of Love enter from the west needs to be in row 3
                    dungeon.SetTile(numLevels - 1, 3, 3, DungeonTileInfo.DungeonRoomStart + 15);
                    dungeon.SetTile(numLevels - 1, 3, 2, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 3, 1, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 4, 1, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 5, 1, DungeonTileInfo.Nothing);

                    // Altar of Courage enter from the east needs to be in row 4-8
                    dungeon.SetTile(numLevels - 1, 5, 5, DungeonTileInfo.DungeonRoomStart + 15);
                    dungeon.SetTile(numLevels - 1, 5, 6, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 5, 7, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 5, 8, DungeonTileInfo.Nothing);
                }
                if (dungeonName.ToLower() == "destard")
                {
                    // Altar of Courage enter from the north needs to be in row 4-8
                    dungeon.SetTile(numLevels - 1, 5, 5, DungeonTileInfo.DungeonRoomStart + 15);

                    dungeon.SetTile(numLevels - 1, 7, 1, DungeonTileInfo.Nothing);
                    dungeon.SetTile(numLevels - 1, 8, 1, DungeonTileInfo.Nothing);
                    for (int y = 1; y < 6; y++)
                    {
                        dungeon.SetTile(numLevels - 1, 8, y, DungeonTileInfo.Nothing);
                    }
                    for (int x = 1; x < 5; x++)
                    {
                        dungeon.SetTile(numLevels - 1, x, 5, DungeonTileInfo.Nothing);
                    }
                }
                if (dungeonName.ToLower() == "hythloth")
                {
                    // Altar of Truth needs to be in row 1 or 2
                    dungeon.SetTile(numLevels - 1, 1, 1, DungeonTileInfo.DungeonRoomStart + 15);
                    // Altar of Love needs to be in row 3
                    dungeon.SetTile(numLevels - 1, 3, 3, DungeonTileInfo.DungeonRoomStart + 15);
                    // Altar of Courage needs to be in row 4-8
                    dungeon.SetTile(numLevels - 1, 5, 5, DungeonTileInfo.DungeonRoomStart + 15);

                    // LB entrance connection
                    for (int y = 0; y < height; y++)
                    {
                        dungeon.SetTile(0, 5, y, DungeonTileInfo.Nothing);
                    }

                    // Passage way connecting all
                    for (int y = 2; y < 6; y++)
                    {
                        dungeon.SetTile(numLevels - 1, 6, y, DungeonTileInfo.Nothing);
                    }

                    // Altar of Truth enter from the south needs to be in row 1 or 2
                    for (int x = 2; x < 6; x++)
                    {
                        dungeon.SetTile(numLevels - 1, x, 1, DungeonTileInfo.Nothing);
                    }

                    // Altar of Love enter from the south needs to be in row 3
                    for (int x = 4; x < 6; x++)
                    {
                        dungeon.SetTile(numLevels - 1, x, 3, DungeonTileInfo.Nothing);
                    }
                }
            }
            else
            {
                for (int l = 0; l < numLevels; l++)
                {
                    dungeon.SetTile(l, 1, 1+l, DungeonTileInfo.LadderUp);
                }
                for (var l = 0;l < numLevels; l++)
                {
                    dungeon.SetTile(l, 1, 2+l, DungeonTileInfo.AltarOrStone);
                }
            }
        }
    }
}