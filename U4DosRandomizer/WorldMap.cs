using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace U4DosRandomizer
{
    public class WorldMap
    {
        private double[,] _worldMapGenerated;
        private byte[,] _worldMapTiles;

        private double _generatedMin;
        private double _generatedMax;

        public const int SIZE = 256; 

        public WorldMap(double[,] worldMapGenerated)
        {
            _worldMapGenerated = worldMapGenerated;
            _worldMapTiles = MapGeneratedMapToUltimaTiles();

            var worldMapFlattened = new double[WorldMap.SIZE * WorldMap.SIZE];

            for (int x = 0; x < WorldMap.SIZE; x++)
            {
                for (int y = 0; y < WorldMap.SIZE; y++)
                {
                    worldMapFlattened[x + y * WorldMap.SIZE] = _worldMapGenerated[x, y];
                }
            }

            _generatedMin = worldMapFlattened.Min();
            _generatedMax = worldMapFlattened.Max();
        }

        private byte[,] MapGeneratedMapToUltimaTiles()
        {
            var worldMapFlattened = new double[WorldMap.SIZE * WorldMap.SIZE];

            for (int x = 0; x < WorldMap.SIZE; x++)
            {
                for (int y = 0; y < WorldMap.SIZE; y++)
                {
                    worldMapFlattened[x + y * WorldMap.SIZE] = _worldMapGenerated[x, y];
                }
            }

            //var min = worldMapFlattened.Min();
            //var max = worldMapFlattened.Max();
            var worldMapFlattenedList = worldMapFlattened.ToList();
            worldMapFlattenedList.Sort();

            var rangeList = new List<Tuple<byte, double, double>>();
            var lowerBound = worldMapFlattened.Min() - 1;
            var upperBound = 0.0;
            var percentSum = 0.0;
            foreach (var key in percentInMap)
            {
                percentSum += key.Value;
                var index = Convert.ToInt32(Math.Floor(worldMapFlattenedList.Count * percentSum));
                upperBound = worldMapFlattenedList[index];
                rangeList.Add(new Tuple<byte, double, double>(key.Key, lowerBound, upperBound));
                lowerBound = upperBound;
            }
            var last = rangeList.Last();
            rangeList[rangeList.Count - 1] = new Tuple<byte, double, double>(last.Item1, last.Item2, worldMapFlattened.Max());

            var worldMapUlt = new byte[WorldMap.SIZE, WorldMap.SIZE];
            for (int x = 0; x < WorldMap.SIZE; x++)
            {
                for (int y = 0; y < WorldMap.SIZE; y++)
                {
                    // Smush it down to the number of tile types we want
                    //int res = Convert.ToInt32(Linear(worldMapDS[x, y], min, max, 0, percentInMap.Count));
                    byte res = 99;
                    foreach (var range in rangeList)
                    {
                        var value = _worldMapGenerated[x, y];
                        if (_worldMapGenerated[x, y] > range.Item2 && _worldMapGenerated[x, y] <= range.Item3)
                        {
                            res = range.Item1;
                            break;
                        }
                    }

                    // Spread it back out so we can see some color differences
                    //res = Convert.ToInt32(Linear(res, 0, percentInMap.Count, 0, 255));
                    //Color newColor = Color.FromArgb(res, res, res);
                    worldMapUlt[x, y] = res;
                }
            }

            return worldMapUlt;
        }

        public Coordinate GetCoordinate(int x, int y)
        {
            return new Coordinate(Convert.ToByte(Wrap(x + 1)), Convert.ToByte(Wrap(y)), _worldMapTiles);
        }

        public void CleanupAndAddFeatures(Random random)
        {
            RemoveSingleTiles();
            AddRivers(random);
            AddBridges();
            AddLava();
        }

        private void RemoveSingleTiles()
        {
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {   
                    var adjacentTiles = new byte[] { _worldMapTiles[Wrap(x + 1), y], _worldMapTiles[Wrap(x - 1), y], _worldMapTiles[x, Wrap(y + 1)], _worldMapTiles[x, Wrap(y - 1)] };
                    if (!adjacentTiles.Contains(_worldMapTiles[x, y]))
                    {
                        var mostUsedAdjacentTile = (from item in adjacentTiles
                                                    group item by item into g
                                                    orderby g.Count() descending
                                                    select g.Key).First();
                        _worldMapTiles[x, y] = mostUsedAdjacentTile;
                    }
                }
            }

            return;
        }


        private void AddLava()
        {
            return;
        }

        private void AddBridges()
        {
            return;
        }

        private void AddRivers(Random random)
        {
            // There are ~32 in the original Ultima map
            int totalNumOfRivers = 28 + random.Next(1, 3) + random.Next(1, 3);
            var surroundingPoints = new List<Point>();

            for (int dist = 1; dist <= 3; dist++)
            {
                surroundingPoints.Add(new Point(1 * dist, 0));
                surroundingPoints.Add(new Point(-1 * dist, 0));
                surroundingPoints.Add(new Point(0, 1 * dist));
                surroundingPoints.Add(new Point(0, -1 * dist));
            }


            // Pick a random spot that isn't water
            // Head uphill from there until you reach a high point
            // Go down hill from all the highpoints marking the path for a river

            var highPoints = new HashSet<Coordinate>();
            for (int riverNum = 0; riverNum < totalNumOfRivers; riverNum++)
            {
                var randomPoint = FindRandomPointHigherThan(4, _worldMapTiles, random);

                // Track previous point so I can step back one when I find the highest point
                var prevPoint = randomPoint;
                var currPoint = randomPoint;
                Coordinate foundHighPoint = null;
                while (foundHighPoint == null)
                {
                    //_worldMapTiles[prevPoint.X, prevPoint.Y] = 0xA1;
                    var highestDirection = surroundingPoints.OrderByDescending(p => _worldMapGenerated[Wrap(currPoint.X + (p.X)), Wrap(currPoint.Y + (p.Y))]).First();

                    if (_worldMapGenerated[currPoint.X, currPoint.Y] < _worldMapGenerated[Wrap(currPoint.X + (highestDirection.X)), Wrap(currPoint.Y + (highestDirection.Y))])
                    {
                        prevPoint = currPoint;
                        //int distance = Convert.ToInt32(Math.Sqrt(highestDirection.Item1 * highestDirection.Item1 + highestDirection.Item2 * highestDirection.Item2));
                        int distance = Math.Abs(highestDirection.X != 0 ? highestDirection.X : highestDirection.Y);
                        currPoint = new Coordinate(Wrap(currPoint.X + highestDirection.X / distance), Wrap(currPoint.Y + highestDirection.Y / distance), _worldMapTiles);
                    }
                    else
                    {
                        foundHighPoint = currPoint;
                    }
                }

                //_worldMapTiles[prevPoint.X, prevPoint.Y] = 76;
                highPoints.Add(prevPoint);
            }

            var paths = new List<List<Coordinate>>();
            foreach (var highPoint in highPoints)
            {
                // find shortest path
                var path = Search.GetPath(WorldMap.SIZE, WorldMap.SIZE, new List<Coordinate> { highPoint }, IsCoordinateWater, delegate { return true; }, FindWaterHueristic);
                paths.Add(path);
                //List<Point> pathToWater = new List<Point>();
                //pathToWater.Add(highPoint);
                //var currPoint = highPoint;
                //var foundWater = false;
                //Point lastDirection = new Point(0, 1);
                //while(!foundWater)
                //{
                //    var lowestDirection = surroundingPoints.OrderBy(p => worldMapDS[Wrap(currPoint.X + (p.X)), Wrap(currPoint.Y + (p.Y))]).First();
                //    var lowestPoint = new Point(Wrap(currPoint.X + (lowestDirection.X)), Wrap(currPoint.Y + (lowestDirection.Y)));

                //    if (//worldMapDS[currPoint.X, currPoint.Y] >= worldMapDS[lowestPoint.X, lowestPoint.Y] &&
                //        !pathToWater.Contains(lowestPoint))
                //    {
                //        lastDirection = lowestDirection;
                //    }
                //    // Else keep moving in the same direction as last time

                //    int distance = Math.Abs(lastDirection.X != 0 ? lastDirection.X : lastDirection.Y);
                //    currPoint = new Point(Wrap(currPoint.X + lastDirection.X / distance), Wrap(currPoint.Y + lastDirection.Y / distance));
                //    pathToWater.Add(currPoint);

                //    if (worldMapUlt[currPoint.X, currPoint.Y] < 3)
                //    {
                //        foundWater = true;
                //    }

                //    worldMapUlt[currPoint.X, currPoint.Y] = 0x70;
                //}

                //paths.Add(pathToWater);
            }

            foreach (var path in paths)
            {
                //Choose ransom spot along the path for the headwater
                //var start = random.Next(0, path.Count);
                // That is weighted towards the start of the path
                var max = path.Count - 10;
                var min = 10;
                var start = Convert.ToInt32(Math.Floor(Math.Abs(random.NextDouble() - random.NextDouble()) * (1 + max - min) + min));
                //for( int i = 1; i < start; i++)
                //{
                //    _worldMapTiles[path[i].X, path[i].Y] = 0x70;
                //}
                for (int i = start; i < path.Count; i++)
                {
                    _worldMapTiles[path[i].X, path[i].Y] = 0x02;
                }
            }

            return;
        }

        //public delegate float NodeHuersticValue(Coordinate coord, IsNodeValid matchesGoal);
        private float FindWaterHueristic(Coordinate coord, IsNodeValid matchesGoal)
        {
            if( matchesGoal(coord))
            { 
                return 1.0f; 
            }

            var range = _generatedMax - _generatedMin;
            var value = _worldMapGenerated[coord.X, coord.Y] - _generatedMin;

            return Convert.ToSingle(value / range);
        }

        private static bool IsCoordinateWater(ICoordinate coordinate)
        {
            return coordinate.GetTile() < 2;
        }

        private static Coordinate FindRandomPointHigherThan(int tile, byte[,] worldMapUlt, Random random)
        {
            Coordinate result = null;
            while (result == null)
            {
                var x = random.Next(0, 256);
                var y = random.Next(0, 256);

                if (worldMapUlt[x, y] > tile)
                {
                    result = new Coordinate(x, y, worldMapUlt);
                }
            }

            return result;
        }

        public static byte Wrap(int input)
        {
            return Wrap(input, WorldMap.SIZE);
        }

        public static byte Wrap(int input, int divisor)
        {
            return Convert.ToByte((input % divisor + divisor) % divisor);
        }

        static private Dictionary<byte, double> percentInMap = new Dictionary<byte, double>()
        {
            {0,0.519012451171875},
            {1,0.15771484375},
            {2,0.0294952392578125},
            //{3,0.010162353515625},
            {4,0.1092376708984375+0.010162353515625}, // Adding on the swamps cuz I think I'll add those in later
            {5,0.07513427734375},
            {6,0.03515625},
            {7,0.0355224609375},
            {8,0.0266265869140625},
            //{9,0.0001068115234375},
            //{10,0.0001068115234375},
            //{11,4.57763671875E-05},
            //{12,6.103515625E-05},
            //{13,1.52587890625E-05},
            //{14,1.52587890625E-05},
            //{15,1.52587890625E-05},
            //{23,0.0002593994140625},
            //{29,1.52587890625E-05},
            //{30,0.0001068115234375},
            //{61,1.52587890625E-05},
            //{70,0.0001068115234375},
            //Lava{76,0.001068115234375}
        };

        public static void MoveBuildings(byte[,] worldMapUlt, UltimaData data)
        {
            foreach (var loc in data.LCB)
            {
                worldMapUlt[loc.X, loc.Y] = loc.GetTile();
            }

            foreach (var loc in data.Castles)
            {
                worldMapUlt[loc.X, loc.Y] = loc.GetTile();
            }

            foreach (var loc in data.Towns)
            {
                worldMapUlt[loc.X, loc.Y] = loc.GetTile();
            }
        }

        public void WriteMapToOriginalFormat(System.IO.BinaryWriter worldFile)
        {
            int chunkwidth = 32;
            int chunkSize = chunkwidth * chunkwidth;
            byte[] chunk = new byte[chunkSize];

            for (int chunkCount = 0; chunkCount < 64; chunkCount++)
            {
                // Copy the chunk over
                for (int i = 0; i < chunkSize; i++)
                {
                    chunk[i] = _worldMapTiles[i % chunkwidth + chunkCount % 8 * chunkwidth, i / chunkwidth + chunkCount / 8 * chunkwidth];
                }

                worldFile.Write(chunk);
            }
        }

        public Bitmap ToBitmap()
        {
            var image = new Bitmap(WorldMap.SIZE, WorldMap.SIZE);
            for (int x = 0; x < WorldMap.SIZE; x++)
            {
                for (int y = 0; y < WorldMap.SIZE; y++)
                {
                    if (colorMap.ContainsKey(_worldMapTiles[x, y]))
                    {
                        image.SetPixel(x, y, colorMap[_worldMapTiles[x, y]]);
                    }
                    else
                    {
                        image.SetPixel(x, y, Color.White);
                    }

                }
            }

            return image;
        }

        static private Dictionary<byte, Color> colorMap = new Dictionary<byte, Color>()
        {
            {0, Color.FromArgb(0,0,112) },
            {1, Color.FromArgb(20,20,112) },
            {2, Color.FromArgb(60,60,112) },
            {3, Color.FromArgb(112, 0, 112) },
            {4, Color.FromArgb(18, 112+18, 18) },
            {5, Color.FromArgb(68, 112+68, 68) },
            {6, Color.FromArgb(108,112+108,108) },
            {7, Color.FromArgb(112+45,112+45,112+45) },
            {8, Color.FromArgb(112+15,112+15,112+15) },
            {70, Color.Orange },
            {76, Color.Red },
            {0xA1, Color.Purple },
        };
    }
}
