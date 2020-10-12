using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class WilsonMazeGenerator
    {
        //private byte[,,] map = new byte[8, 8, 8];

        // Heavily modified from https://www.pixata.co.uk/2020/01/27/mazes-for-programmers-part-8-a-random-walk-with-wilsons-algorithm/
        // since Ultima walls take up a tile
        public void GenerateMaze(Dungeon dungeon, int numLevels, int width, int height, Random rand)
        {
            byte[,,] map = new byte[numLevels, width, height];

            // Init with walls
            for(int l = 0; l < numLevels; l++)
            {
                for(int x = 0; x < width; x++)
                {
                    for(int y = 0; y < height; y++)
                    {
                        dungeon.GetTile(l, x, y).SetTile(DungeonTileInfo.Wall);
                    }
                }
            }

            for (int l = 0; l < numLevels; l++)
            {
                List<DungeonTile> walk = new List<DungeonTile>();
                var openSet = new HashSet<DungeonTile>();
                // Add all tiles of a level to the the set of stuff we can grab from for random walks
                openSet.UnionWith(dungeon.Tiles(l));
                // Pick a random tile to mark as visited. This will be the end point of our first walk.
                var first = openSet.Rand(rand);
                first.SetTile(DungeonTileInfo.Nothing);
                openSet.Remove(first);
                // Must remove all neighbors too so the walls are maintained
                foreach (var neighbor in first.NeighborsSameLevelAndAdjacent())
                {
                    openSet.Remove(neighbor);
                }
                // Pick a starting cell for our first walk
                var current = openSet.Rand(rand);
                walk.Add(current);
                while (openSet.Count > 0)
                {
                    DungeonTile next = walk.Last().NeighborsSameLevel().Rand(rand);

                    if (next.NeighborsSameLevel().Any(t => t.GetTile() != DungeonTileInfo.Wall)) // Found a connection
                    {
                        walk.Add(next);
                        //Turn the entire walk into a corridor
                        foreach (var tile in walk)
                        {
                            tile.SetTile(DungeonTileInfo.Nothing);
                            openSet.Remove(tile);
                            // Must remove all neighbors too so the walls are maintained
                            foreach (var neighbor in tile.NeighborsSameLevelAndAdjacent())
                            {
                                openSet.Remove(neighbor);
                            }
                        }

                        walk.First().SetTile(DungeonTileInfo.AltarOrStone);
                        walk.Last().SetTile(DungeonTileInfo.Chest);

                        // If there are still tiles not in the maze let's setup another walk
                        if (openSet.Count > 0)
                        {
                            walk = new List<DungeonTile>();
                            current = openSet.Rand(rand);
                            walk.Add(current);
                        }
                    }
                    else if (next.NeighborsSameLevelAndAdjacent().Any(t => t.GetTile() != DungeonTileInfo.Wall))
                    {
                        // If there is one kitty corner from us. Then go towards it.
                        var kittyCorner = next.NeighborsSameLevelAndAdjacent().Where(t => t.GetTile() != DungeonTileInfo.Wall).First();
                        next = next.NeighborsSameLevel().Union(kittyCorner.NeighborsSameLevel()).Rand(rand);
                    }
                    else if (walk.Contains(next) || KittyCornerInWalk(walk, next))
                    {
                        // Snip off all cells after the last time we visited this cell
                        if (walk.Contains(next))
                        {
                            walk = walk.Take(walk.IndexOf(next) + 1).ToList();
                        }
                        else
                        {
                            walk = walk.Take(walk.IndexOf(GetUnconnectedKittyCornerInWalk(walk, next)) + 1).ToList();
                        }
                    }
                    else
                    {
                        walk.Add(next);
                    }
                }
            }

            return;
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
                if(!connected.Any(x => walk.Contains(x)))
                {
                    return kitty;
                }
            }

            return null;
        }
    }



}
