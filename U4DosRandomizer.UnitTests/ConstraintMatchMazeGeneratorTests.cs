using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using U4DosRandomizer.Algorithms;

namespace U4DosRandomizer.UnitTests
{

    [TestClass]
    public class ConstraintMatchMazeGeneratorTests
    {
        [TestMethod]
        public void GenerateMaze_ValidDungeon()
        {
            // Arrange
            byte[,,] map = MakeEmptyMap();
            List<byte[]> rooms = new List<byte[]>();

            var dungeon = new Dungeon(map, rooms);

            var mg = new ConstraintMatchMazeGenerator();

            var rand = new System.Random(0);
            for (int i = 0; i < 100; i++)
            {
                // Act
                mg.GenerateMaze("ConstraintMatchMazeGenerator", dungeon, 8, 8, 8, rand);


                // Assert
                for (int l = 0; l < 8; l++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        for (int x = 0; x < 8; x++)
                        {
                            Console.Write(dungeon.GetTile(l, x, y).GetTile() == DungeonTileInfo.Nothing ? "_" : x.ToString());
                        }
                        Console.WriteLine();
                    }

                    Console.WriteLine();
                    for (int x = 0; x < 8; x++)
                    {
                        for (int y = 0; y < 8; y++)
                        {
                            var tile = dungeon.GetTile(l, x, y);
                            var right = dungeon.GetTile(l, x + 1, y);
                            var below = dungeon.GetTile(l, x, y + 1);
                            var kitty = dungeon.GetTile(l, x + 1, y + 1);
                            if (tile.GetTile() == right.GetTile() &&
                                tile.GetTile() == below.GetTile() &&
                                tile.GetTile() == kitty.GetTile())
                            {
                                Assert.Fail($"All walls or all corridors at {tile.L}, {tile.X}, {tile.Y}");
                            }

                            if (tile.GetTile() != right.GetTile() &&
                                tile.GetTile() != below.GetTile() &&
                                tile.GetTile() == kitty.GetTile())
                            {
                                Assert.Fail($"Kitty-corner walls or corridors at {tile.L}, {tile.X}, {tile.Y}");
                            }
                        }
                    }
                }
            }

        }

        [TestMethod]
        public void GenerateMaze_011IsEmpty()
        {
            // Arrange
            byte[,,] map = MakeEmptyMap();
            List<byte[]> rooms = new List<byte[]>();

            var dungeon = new Dungeon(map, rooms);

            var mg = new ConstraintMatchMazeGenerator();

            var rand = new System.Random(0);
            for (int i = 0; i < 100; i++)
            {
                // Act
                mg.GenerateMaze("ConstraintMatchMazeGenerator", dungeon, 8, 8, 8, rand);

                // Assert
                Assert.AreEqual(DungeonTileInfo.Nothing, dungeon.GetTile(0, 1, 1).GetTile(), "0,1,1 needs to be empty so there can be a ladder up.");
            }

        }


        private static byte[,,] MakeEmptyMap()
        {
            byte[,,] map = new byte[8, 8, 8];

            for (int l = 0; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        map[l, x, y] = DungeonTileInfo.Nothing;
                    }
                }
            }

            map[0, 1, 1] = DungeonTileInfo.LadderUp;
            return map;
        }
    }
}
