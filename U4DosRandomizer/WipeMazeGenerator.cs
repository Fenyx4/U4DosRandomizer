using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class WipeMazeGenerator : IMazeGenerator
    {
        public void GenerateMaze(string dungeonName, Dungeon dungeon, int numLevels, int width, int height, Random rand)
        {
            for (int l = 0; l < numLevels; l++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if(dungeon.GetTile(l, x, y).GetTile() != DungeonTileInfo.Wall)
                        {
                            dungeon.SetTile(l, x, y, DungeonTileInfo.Nothing);
                        }
                    }
                }
            }

            return;
        }

        private static int MinKey(int[] key, bool[] set, int verticesCount)
        {
            int min = int.MaxValue, minIndex = 0;

            for (int v = 0; v < verticesCount; ++v)
            {
                if (set[v] == false && key[v] < min)
                {
                    min = key[v];
                    minIndex = v;
                }
            }

            return minIndex;
        }

        private bool KittyCornerInWalk(List<DungeonTile> walk, DungeonTile next)
        {
            return GetUnconnectedKittyCornerInWalk(walk, next) != null;
        }

        private DungeonTile GetUnconnectedKittyCornerInWalk(List<DungeonTile> walk, DungeonTile next)
        {
            var kittyCornersInWalk = walk.Intersect(next.KittyCorners());

            foreach (var kitty in kittyCornersInWalk)
            {
                var connected = next.NeighborsSameLevel().Intersect(kitty.NeighborsSameLevel()).ToList();
                if (!connected.Any(x => walk.Contains(x)))
                {
                    return kitty;
                }
            }

            return null;
        }

        private static int SET_MSK(int mask, bool bit, int offset)
        {
            return mask |= ((bit ? 1 : 0) << offset);
        }

        public static bool TST_MSK(int mask, int offset)
        {
            return (mask & (1 << offset)) != 0;
        }
    }



}
