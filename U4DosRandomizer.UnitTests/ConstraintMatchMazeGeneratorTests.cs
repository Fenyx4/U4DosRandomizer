using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var dungeon = new Dungeon(map, rooms, null);

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

            var dungeon = new Dungeon(map, rooms, null);

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

        [TestMethod]
        public void GenerateMaze_Altar()
        {
            // Arrange
            var dungeonNames = new List<string>() { "Wrong", "Covetous", "Despise", "Deceit", "Hythloth", "Shame", "Destard" };
            var dungeons = new List<Tuple<string,Dungeon>>();

            foreach (var dungeonName in dungeonNames)
            {
                byte[,,] map = MakeEmptyMap();
                List<byte[]> rooms = new List<byte[]>();

                var dungeon = new Dungeon(map, rooms, null);
                dungeons.Add(new Tuple<string,Dungeon>(dungeonName, dungeon));
            }

            var mg = new ConstraintMatchMazeGenerator();

            var rand = new System.Random(0);
            for (int i = 0; i < 100; i++)
            {
                // Act
                var dungeon = dungeons[i%7];
                mg.GenerateMaze(dungeon.Item1, dungeon.Item2, 8, 8, 8, rand);

                // Assert
                var altars = dungeon.Item2.GetTiles().Where(t => t.GetTile() == DungeonTileInfo.DungeonRoomStart + 15).ToList();

                if (dungeon.Item1 == "Despise")
                {
                    Assert.AreEqual(1, altars.Count());
                    var altar = altars[0];
                    Assert.AreEqual(3, altar.X);
                    TestNorthEntrance(dungeon, altar);
                    //Assert.AreEqual(DungeonTileInfo.Nothing, dungeon.Item2.GetTile(0, 1, 1).GetTile(), "0,1,1 needs to be empty so there can be a ladder up.");
                }

                if (dungeon.Item1 == "Deceit")
                {
                    Assert.AreEqual(1, altars.Count());
                    var altar = altars[0];
                    Assert.IsTrue(altar.X < 3);
                    TestNorthEntrance(dungeon, altar);
                    //Assert.AreEqual(DungeonTileInfo.Nothing, dungeon.Item2.GetTile(0, 1, 1).GetTile(), "0,1,1 needs to be empty so there can be a ladder up.");
                }

                if (dungeon.Item1 == "Destard")
                {
                    Assert.AreEqual(1, altars.Count());
                    var altar = altars[0];
                    Assert.IsTrue(altar.X > 3);
                    TestNorthEntrance(dungeon, altar);
                    //Assert.AreEqual(DungeonTileInfo.Nothing, dungeon.Item2.GetTile(0, 1, 1).GetTile(), "0,1,1 needs to be empty so there can be a ladder up.");
                }

                if (dungeon.Item1 == "Covetous")
                {
                    Assert.AreEqual(2, altars.Count());
                    var altar = altars[0];
                    if (altar.X < 3)
                    {
                        Assert.IsTrue(altar.X < 3);
                        TestWestEntrance(dungeon, altar);
                    }
                    altar = altars[1];
                    if (altar.X > 3)
                    {
                        Assert.IsTrue(altar.X > 3);
                        TestEastEntrance(dungeon, altar);
                    }
                    //Assert.AreEqual(DungeonTileInfo.Nothing, dungeon.Item2.GetTile(0, 1, 1).GetTile(), "0,1,1 needs to be empty so there can be a ladder up.");
                }

                if (dungeon.Item1 == "Wrong")
                {
                    Assert.AreEqual(2, altars.Count());
                    var altar = altars[0];
                    if (altar.X < 3)
                    {
                        Assert.IsTrue(altar.X < 3);
                        TestWestEntrance(dungeon, altar);
                    }
                    altar = altars[1];
                    if (altar.X == 3)
                    {
                        Assert.IsTrue(altar.X == 3);
                        TestEastEntrance(dungeon, altar);
                    }
                    //Assert.AreEqual(DungeonTileInfo.Nothing, dungeon.Item2.GetTile(0, 1, 1).GetTile(), "0,1,1 needs to be empty so there can be a ladder up.");
                }

                if (dungeon.Item1 == "Shame")
                {
                    Assert.AreEqual(2, altars.Count());
                    var altar = altars[0];
                    if (altar.X < 3)
                    {
                        Assert.IsTrue(altar.X < 3);
                        TestEastEntrance(dungeon, altar);
                    }
                    altar = altars[1];
                    if (altar.X > 3)
                    {
                        Assert.IsTrue(altar.X > 3);
                        TestWestEntrance(dungeon, altar);
                    }
                    //Assert.AreEqual(DungeonTileInfo.Nothing, dungeon.Item2.GetTile(0, 1, 1).GetTile(), "0,1,1 needs to be empty so there can be a ladder up.");
                }

                if (dungeon.Item1 == "Hythloth")
                {
                    Assert.AreEqual(3, altars.Count());
                    var altar = altars[0];
                    if (altar.X < 3)
                    {
                        Assert.IsTrue(altar.X < 3);
                        TestSouthEntrance(dungeon, altar);
                    }
                    if (altar.X == 3)
                    {
                        Assert.IsTrue(altar.X == 3);
                        TestSouthEntrance(dungeon, altar);
                    }
                    altar = altars[1];
                    if (altar.X > 3)
                    {
                        Assert.IsTrue(altar.X > 3);
                        TestSouthEntrance(dungeon, altar);
                    }
                    //Assert.AreEqual(DungeonTileInfo.Nothing, dungeon.Item2.GetTile(0, 1, 1).GetTile(), "0,1,1 needs to be empty so there can be a ladder up.");
                }
            }

        }

        private static void TestEastEntrance(Tuple<string, Dungeon> dungeon, DungeonTile altar)
        {
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X - 1, altar.Y).GetTile());
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X, altar.Y - 1).GetTile());
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X + 1, altar.Y).GetTile());
        }

        private static void TestWestEntrance(Tuple<string, Dungeon> dungeon, DungeonTile altar)
        {
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X - 1, altar.Y).GetTile());
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X, altar.Y + 1).GetTile());
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X + 1, altar.Y).GetTile());
        }

        private static void TestNorthEntrance(Tuple<string, Dungeon> dungeon, DungeonTile altar)
        {
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X, altar.Y - 1).GetTile());
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X, altar.Y + 1).GetTile());
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X + 1, altar.Y).GetTile());
        }

        private static void TestSouthEntrance(Tuple<string, Dungeon> dungeon, DungeonTile altar)
        {
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X, altar.Y - 1).GetTile());
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X, altar.Y + 1).GetTile());
            Assert.AreEqual(DungeonTileInfo.Wall, dungeon.Item2.GetTile(altar.L, altar.X - 1, altar.Y).GetTile());
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
