using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class HitomezashiStitchMazeGenerator : IMazeGenerator
    {
        private static int north = 0;
        private static int east = 1;
        private static int west = 2;
        private static int south = 3;
        private class Cell
        {
            public int Color { get; set; }
            public bool[] Edges { get; set; }
            public int L { get; internal set; }
            public int X { get; internal set; }
            public int Y { get; internal set; }
        }
        // https://www.youtube.com/watch?v=JbfhzlMk2eY
        public void GenerateMaze(string dungeonName, Dungeon dungeon, int numLevels, int width, int height, Random rand)
        {
            Cell[,,] map = new Cell[numLevels, width, height];

            // Init 
            for (int l = 0; l < numLevels; l++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        dungeon.SetTile(l, x, y, DungeonTileInfo.Wall);
                        map[l, x, y] = new Cell
                        {
                            Color = 0,
                            Edges = new bool[4],
                            L = l,
                            X = x,
                            Y = y
                        };
                    }
                }
            }

            var stitchStarters = new int[16];
            for (int x = 0; x < 16; x++)
            {
                stitchStarters[x] = rand.Next();
            }
            for (int l = 0; l < numLevels; l++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        map[l, x, y].Edges[east] = (y % 2 == 0 ? TST_MSK(stitchStarters[l * 2], x) : !TST_MSK(stitchStarters[l * 2], x));
                        //map[l, x, y].Edges[south] = (y % 2 == 0 ? !TST_MSK(stitchStarters[l * 2], (y + 1) % height) : TST_MSK(stitchStarters[l * 2], (y + 1) % height));
                        map[l, x, y].Edges[north] = (x % 2 == 0 ? TST_MSK(stitchStarters[(l * 2) + 1], y) : !TST_MSK(stitchStarters[(l * 2) + 1], y));
                        //map[l, x, y].Edges[west] = (x % 2 == 0 ? !TST_MSK(stitchStarters[(l * 2) + 1], (x + 1) % width) : TST_MSK(stitchStarters[(l * 2) + 1], (x + 1) % width));
                    }
                }
            }

            for (int l = 0; l < numLevels; l++)
            {
                Console.WriteLine(l);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        //if (map[l, x, y].Edges[north] && map[l, x, y].Edges[east])
                        //{
                        //    Console.Write("7");
                        //} 
                        //if (map[l, x, y].Edges[north])
                        //{
                        //    Console.Write("-");
                        //}
                        if (!map[l, y, x].Edges[east])
                        { 
                            Console.Write("|");
                        }
                        else
                        {
                            Console.Write("0");
                        }
                    }
                    Console.Write("    ");
                    for (int y = 0; y < height; y++)
                    {
                        //if (map[l, x, y].Edges[north] && map[l, x, y].Edges[east])
                        //{
                        //    Console.Write("7");
                        //} 
                        if (map[l, y, x].Edges[north])
                        {
                            Console.Write("-");
                        }
                        //if (map[l, y, x].Edges[east])
                        //{
                        //    Console.Write("|");
                        //}
                        else
                        {
                            Console.Write("0");
                        }
                    }
                    Console.WriteLine();
                }
            }
            
            int currentColor = 1;
            for (int l = 0; l < numLevels; l++)
            {
                // Go through the western side setting some initial color values
                int x = 0;
                for (int y = 0; y < height; y++)
                {
                    if(map[l, x, y].Color == 0)
                    {
                        currentColor = (currentColor + 1) % 2;
                        BreadthFirstColoring(map, l, x, y, currentColor + 1, width, height);
                    }
                    else
                    {
                        currentColor = map[l, x, y].Color - 1;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        // Do to above loop when x =0 then color will not be 0 so this will seed the color to use
                        if (map[l, x, y].Color == 0)
                        {
                            currentColor = (currentColor + 1) % 2;
                            BreadthFirstColoring(map, l, x, y, currentColor + 1, width, height);
                        }
                        else
                        {
                            currentColor = map[l, x, y].Color - 1;
                        }
                    }
                }
            }

            for (int l = 0; l < numLevels; l++)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var tile = DungeonTileInfo.Wall;
                        switch (map[l, x, y].Color)
                        {
                            case 0:
                                tile = DungeonTileInfo.AltarOrStone;
                                break;
                            case 1:
                                tile = DungeonTileInfo.Nothing;
                                break;
                            case 2:
                                tile = DungeonTileInfo.Wall;
                                break;
                            default:
                                break;
                        }
                        dungeon.SetTile(l, y, x, tile);
                    }
                }
            }

            return;
        }

        private void BreadthFirstColoring(Cell[,,] map, int root_l, int root_x, int root_y, int color, int width, int height)
        {
            var queue = new Queue<Cell>();
            var currentCell = map[root_l, root_x, root_y];
            queue.Enqueue(currentCell);
            while(queue.Count > 0)
            {
                var cell = queue.Dequeue();
                if (cell.Color == 0)
                {
                    cell.Color = color;
                    var connectedNeighbors = GetConnectedNeighbors(map, root_l, cell, width, height);
                    foreach (var neighbor in connectedNeighbors)
                    {
                        if (neighbor.Color == 0)
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        private List<Cell> GetConnectedNeighbors(Cell[,,] map, int root_l, Cell cell, int width, int height)
        {
            var result = new List<Cell>();
            if((cell.Y - 1) >= 0 && !cell.Edges[north])
            {
                result.Add(map[root_l, cell.X, (cell.Y + (height - 1)) % height]);
            }
            if (cell.Y + 1 < height && !map[root_l, cell.X, (cell.Y + 1) % height].Edges[north])
            {
                result.Add(map[root_l, cell.X, (cell.Y + 1) % height]);
            }
            if (cell.X + 1 < width && !cell.Edges[east])
            {
                result.Add(map[root_l, (cell.X+1)%width, cell.Y]);
            }
            if (cell.X - 1  >= 0 && !map[root_l, (cell.X + (width - 1)) % width, cell.Y].Edges[east])
            {
                result.Add(map[root_l, (cell.X + (width -1)) % width, cell.Y]);
            }

            return result;
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
