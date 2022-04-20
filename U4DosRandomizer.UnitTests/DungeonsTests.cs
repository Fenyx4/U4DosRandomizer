using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using U4DosRandomizer.Algorithms;

namespace U4DosRandomizer.UnitTests
{
    
    [TestClass]
    public class DungeonsTests
    {
        [TestMethod]
        public void Fill_NoConnectionBetweenLevels_FillLevelOne()
        {
            // Arrange
            byte[,,] map = new byte[8, 8, 8];
            List<byte[]> rooms = new List<byte[]>();

            for (int l = 0; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for(int y = 0; y < 8; y++)
                    {
                        map[l, x, y] = DungeonTileInfo.Nothing;
                    }
                }
            }

            map[0, 1, 1] = DungeonTileInfo.LadderUp;

            var dungeon = new Dungeon(map, rooms);

            // Act
            Dungeons.Fill(dungeon.GetTile(0, 1, 1), dungeon);

            // Assert
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        Assert.AreEqual(DungeonTileInfo.LadderUp, map[0, x, y]);
                    }
                    else
                    {
                        Assert.AreEqual(0xF1, map[0, x, y]);
                    }
                }
            }
            for (int l = 1; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        Assert.AreEqual(DungeonTileInfo.Nothing, map[l, x, y]);
                    }
                }
            }
        }

        [TestMethod]
        public void Fill_ConnectionBetweenLevels_FillLevelTwo()
        {
            // Arrange
            byte[,,] map = new byte[8, 8, 8];
            List<byte[]> rooms = new List<byte[]>();

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
            map[0, 6, 6] = DungeonTileInfo.LadderDown;
            map[1, 6, 6] = DungeonTileInfo.LadderUp;

            var dungeon = new Dungeon(map, rooms);

            // Act
            Dungeons.Fill(dungeon.GetTile(0, 1, 1), dungeon);

            // Assert
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        Assert.AreEqual(DungeonTileInfo.LadderUp, map[0, x, y]);
                    }
                    else if (x == 6 && y == 6)
                    {
                        Assert.AreEqual(DungeonTileInfo.LadderDown, map[0, x, y]);
                    }
                    else
                    {
                        Assert.AreEqual(0xF1, map[0, x, y]);
                    }
                }
            }
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (x == 6 && y == 6)
                    {
                        Assert.AreEqual(DungeonTileInfo.LadderUp, map[1, x, y]);
                    }
                    else
                    {
                        Assert.AreEqual(0xF1, map[1, x, y]);
                    }
                }
            }
            for (int l = 2; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        Assert.AreEqual(DungeonTileInfo.Nothing, map[l, x, y]);
                    }
                }
            }
        }

        [TestMethod]
        public void Fill_ConnectionBetweenLevels_ClosedArea_FillLevelOne()
        {
            // Arrange
            byte[,,] map = new byte[8, 8, 8];
            List<byte[]> rooms = new List<byte[]>();

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
            map[0, 3, 3] = DungeonTileInfo.Wall;
            map[0, 3, 4] = DungeonTileInfo.Wall;
            map[0, 3, 5] = DungeonTileInfo.Wall;
            map[0, 4, 3] = DungeonTileInfo.Wall;
            map[0, 4, 5] = DungeonTileInfo.Wall;
            map[0, 5, 3] = DungeonTileInfo.Wall;
            map[0, 5, 4] = DungeonTileInfo.Wall;
            map[0, 5, 5] = DungeonTileInfo.Wall;

            var dungeon = new Dungeon(map, rooms);

            // Act
            Dungeons.Fill(dungeon.GetTile(0, 1, 1), dungeon);

            // Assert
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (x == 1 && y == 1)
                    {
                        Assert.AreEqual(DungeonTileInfo.LadderUp, map[0, x, y]);
                    }
                    else if ((x == 3 && y == 3) ||
                        (x == 3 && y == 4) ||
                        (x == 3 && y == 5) ||
                        (x == 4 && y == 3) ||
                        (x == 4 && y == 5) ||
                        (x == 5 && y == 3) ||
                        (x == 5 && y == 4) ||
                        (x == 5 && y == 5))
                    {
                        Assert.AreEqual(DungeonTileInfo.Wall, map[0, x, y]);
                    }
                    else if (x == 4 && y == 4)
                    {
                        Assert.AreEqual(DungeonTileInfo.Nothing, map[0, x, y]);
                    }
                    else
                    {
                        Assert.AreEqual(0xF1, map[0, x, y]);
                    }
                }
            }
            for (int l = 1; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        Assert.AreEqual(DungeonTileInfo.Nothing, map[l, x, y]);
                    }
                }
            }
        }

        [TestMethod]
        public void IsolationRemoval_NoConnectionBetweenLevels_ConnectAllLevels()
        {
            // Arrange
            byte[,,] map = new byte[8, 8, 8];
            List<byte[]> rooms = new List<byte[]>();

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

            var dungeon = new Dungeon(map, rooms);

            // Act
            Dungeons.IsolationRemover("IsolationRemoval_NoConnectionBetweenLevels_ConnectAllLevels", dungeon, 8, 8, 8, new System.Random(0));

            // Assert
            Dungeons.Fill(dungeon.GetTile(0, 1, 1), dungeon);
            var numberLadderUp = 0;
            var numberLadderDown = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if(map[0, x, y] == 0xF1)
                    {
                        Assert.AreEqual(0xF1, map[0, x, y]);
                    }
                    else if(map[0, x, y] == DungeonTileInfo.LadderUp)
                    {
                        numberLadderUp++;
                    }
                    else if(map[0, x, y] == DungeonTileInfo.LadderDown)
                    {
                        numberLadderDown++;
                    }

                    if (x == 1 && y == 1)
                    {
                        Assert.AreEqual(DungeonTileInfo.LadderUp, map[0, x, y]);
                    }
                }
            }
            Assert.AreEqual(1, numberLadderDown, "No ladders down on level 1");
            Assert.AreEqual(1, numberLadderUp, "No ladders up on level 1");

            for (int l = 1; l < 8; l++)
            {
                numberLadderUp = 0;
                numberLadderDown = 0;
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        if (map[l, x, y] == 0xF1)
                        {
                            Assert.AreEqual(0xF1, map[l, x, y]);
                        }
                        else if (map[l, x, y] == DungeonTileInfo.LadderUp)
                        {
                            numberLadderUp++;
                        }
                        else if (map[l, x, y] == DungeonTileInfo.LadderDown)
                        {
                            numberLadderDown++;
                        }
                    }
                }
                Assert.AreEqual(1, numberLadderUp, "No ladders up on level " + l.ToString());
                Assert.AreEqual(l == 7 ? 0 : 1, numberLadderDown, "Incorrect number of ladders on " + l.ToString());
            }
        }
    }
}
