using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using U4DosRandomizer.Algorithms;

namespace U4DosRandomizer.UnitTests
{

    [TestClass]
    public class HitomezashiStitchMazeGeneratorTests
    {
        [TestMethod]
        public void GenerateMaze_ValidDungeon()
        {
            // Arrange
            byte[,,] map = MakeEmptyMap();
            List<byte[]> rooms = new List<byte[]>();

            var dungeon = new Dungeon(map, rooms);

            var stitch = new HitomezashiStitchMazeGenerator();

            // Act
            stitch.GenerateMaze("HitomezashiStitchMazeGenerator", dungeon, 8, 8, 8, new System.Random());

            // Assert
            foreach(var tile in dungeon.GetTiles())
            {
                var right = dungeon.GetTile(tile.L, tile.X+1, tile.Y);
                var below = dungeon.GetTile(tile.L, tile.X, tile.Y+1);
                var kitty = dungeon.GetTile(tile.L, tile.X + 1, tile.Y + 1);
                if (tile.GetTile() == right.GetTile() && 
                    tile.GetTile() == below.GetTile() &&
                    tile.GetTile() == kitty.GetTile())
                {
                    Assert.Fail($"All wall or all corridors at {tile.L}, {tile.X}, {tile.Y}");
                }

                if (tile.GetTile() != right.GetTile() &&
                    tile.GetTile() != below.GetTile() &&
                    tile.GetTile() == kitty.GetTile())
                {
                    Assert.Fail($"Kitty-corner walls or corridors at {tile.L}, {tile.X}, {tile.Y}");
                }
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
