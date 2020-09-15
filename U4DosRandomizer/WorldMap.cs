using SixLabors.ImageSharp.PixelFormats;
using SimplexNoise;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class WorldMap
    {
        private const string filename = "WORLD.MAP";
        private double[,] _worldMapGenerated;
        private byte[,] _worldMapTiles;

        private double _generatedMin;
        private double _generatedMax;

        public const int SIZE = 256; 

        public WorldMap()
        { 
        }

        private List<Tile> _potentialSwamps = new List<Tile>();
        private void MapGeneratedMapToUltimaTiles()
        {
            var mapGenerated = _worldMapGenerated;
            var percentInMap = new Dictionary<byte, double>(_percentInMap);

            // Kill shallow water for now... May want to special place that later
            percentInMap[TileInfo.Grasslands] += percentInMap[TileInfo.Shallow_Water];
            percentInMap.Remove(TileInfo.Shallow_Water);
            // Kill Scrubs and forest we are special placing that later
            percentInMap[TileInfo.Grasslands] += percentInMap[TileInfo.Scrubland];
            percentInMap.Remove(TileInfo.Scrubland);
            percentInMap[TileInfo.Grasslands] += percentInMap[TileInfo.Forest];
            percentInMap.Remove(TileInfo.Forest);

            _worldMapTiles = ClampToValuesInSetRatios(mapGenerated, percentInMap, SIZE);

            // Kill swamps after generation so we can use their placements for later
            for(int x = 0; x < SIZE; x++)
            {
                for(int y = 0; y < SIZE; y++)
                {
                    if (_worldMapTiles[x, y] == TileInfo.Swamp)
                    {
                        var tile = GetCoordinate(x, y);
                        _potentialSwamps.Add(tile);
                        tile.SetTile(TileInfo.Grasslands);
                    }
                }
            }

            return;
        }

        private static byte[,] ClampToValuesInSetRatios(double[,] mapGenerated, Dictionary<byte, double> percentInMap, int size)
        {
            var worldMapFlattened = new double[size * size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    worldMapFlattened[x + y * size] = mapGenerated[x, y];
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
                var index = Convert.ToInt32(Math.Floor((worldMapFlattenedList.Count-1) * percentSum));
                upperBound = worldMapFlattenedList[index];
                rangeList.Add(new Tuple<byte, double, double>(key.Key, lowerBound, upperBound));
                lowerBound = upperBound;
            }
            var last = rangeList.Last();
            rangeList[rangeList.Count - 1] = new Tuple<byte, double, double>(last.Item1, last.Item2, worldMapFlattened.Max());

            var worldMapUlt = new byte[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    // Smush it down to the number of tile types we want
                    //int res = Convert.ToInt32(Linear(worldMapDS[x, y], min, max, 0, percentInMap.Count));
                    byte res = 99;
                    foreach (var range in rangeList)
                    {
                        var value = mapGenerated[x, y];
                        if (mapGenerated[x, y] > range.Item2 && mapGenerated[x, y] <= range.Item3)
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

        internal static void Restore(string path)
        {
            var file = Path.Combine(path, filename);
            FileHelper.Restore(file);
        }

        public Tile GetCoordinate(int x, int y)
        {
            return new Tile(Convert.ToByte(Wrap(x)), Convert.ToByte(Wrap(y)), _worldMapTiles);
        }

        public List<Tile> GetAllMatchingTiles(Func<Tile, bool> criteria, int minX = 0, int maxX = SIZE, int minY = 0, int maxY = SIZE)
        {
            var tiles = new List<Tile>();
            for (int x = minX; x < maxX; x++)
            {
                for(int y = minX; y < maxX; y++)
                {
                    var tile = GetCoordinate(x, y);
                    if(criteria(tile))
                    {
                        tiles.Add(tile);
                    }
                }
            }

            return tiles;
        }

        private byte[,] ScrubMap(Random random)
        {
            //var scrubNoise = new DiamondSquare(WorldMap.SIZE, 184643518.256878*128, 82759876).getData(random);
            SimplexNoise.Noise.Seed = random.Next();
            var scrubNoiseFloatLayerOne = SimplexNoise.Noise.Calc2D(SIZE, SIZE, 0.05f);
            var scrubNoiseFloatLayerTwo = SimplexNoise.Noise.Calc2D(SIZE, SIZE, 0.01f);


            var scrubNoiseLayerOne = Float2dToDouble2d(scrubNoiseFloatLayerOne, SIZE);
            var scrubNoiseLayerTwo = Float2dToDouble2d(scrubNoiseFloatLayerTwo, SIZE);

            var scrubNoise = new double[SIZE, SIZE];
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    scrubNoise[x, y] = scrubNoiseLayerOne[x, y] + (scrubNoiseLayerTwo[x,y] * 0.5);
                }
            }

            var totalGrass = _percentInMap[TileInfo.Scrubland] + _percentInMap[TileInfo.Forest] + _percentInMap[TileInfo.Swamp] + _percentInMap[TileInfo.Shallow_Water] + _percentInMap[TileInfo.Grasslands];
            // As we will be overlaying it on the grass so bump up the percentage so the ratio stays correct
            var scrubPercent = _percentInMap[TileInfo.Scrubland] / totalGrass;
            var forestPercent = _percentInMap[TileInfo.Forest] / totalGrass;
            var percentInMap = new Dictionary<byte, double>()
            {
                {TileInfo.Grasslands,(1.0-scrubPercent)-forestPercent},
                {TileInfo.Scrubland,scrubPercent},
                {TileInfo.Forest,forestPercent }
            };

            return ClampToValuesInSetRatios(scrubNoise, percentInMap, SIZE);
        }

        public void TestAbyssEjection()
        {
            byte[] D_0BF0 = { 0xE7, 0x53, 0x23, 0x3B, 0x9E, 0x69, 0x17, 0xBA, 0xD8, 0x1D, 0x91, 0x59, 0xE9 };
            byte[] D_0BFE = { 0x88, 0x69, 0xDD, 0x2C, 0x15, 0xB7, 0x81, 0xAC, 0x6A, 0x30, 0xF3, 0x6A, 0xE9 };

            for(int i = 0; i < 13; i++)
            {
                _worldMapTiles[D_0BF0[i], D_0BFE[i]] = (byte)(TileInfo.A + i);
            }
        }

        private static double[,] Float2dToDouble2d(float[,] floatArray, int size)
        {
            var result = new double[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    result[x, y] = floatArray[x, y];
                }
            }

            return result;
        }

        public void ScrubTest(Random random)
        {
            //_worldMapTiles = ScrubMap(random);
        }

        public void SwampTest(Random random)
        {
            _worldMapTiles = new byte[SIZE, SIZE];
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    _worldMapTiles[x, y] = TileInfo.Grasslands;
                }
            }

            for (int i = 0; i < 23; i++)
            {
                _potentialSwamps.Add(GetCoordinate(random.Next(0, SIZE), random.Next(0, SIZE)));
            }

            AddSwamp(random);

            //var swampSize = 16;
            //var chosenSwampTile = GetCoordinate(SIZE/2, SIZE/2);
            //var swamp = SwampMap(random, swampSize);

            //for (int x = 0; x < swampSize; x++)
            //{
            //    for (int y = 0; y < swampSize; y++)
            //    {
            //        var tile = GetCoordinate(chosenSwampTile.X - swampSize / 2 + x, chosenSwampTile.Y - swampSize / 2 + y);
            //        if (tile.GetTile() == TileInfo.Grasslands)
            //        {
            //            tile.SetTile(swamp[x, y]);
            //        }
            //    }
            //}
        }

        internal bool IsTile(int x, int y, int tile)
        {
            return _worldMapTiles[Wrap(x), Wrap(y)] == tile;
        }

        //https://stackoverflow.com/questions/3041366/shortest-distance-between-points-on-a-toroidally-wrapped-x-and-y-wrapping-ma
        public static int DistanceSquared(ICoordinate destination, ICoordinate origin)
        {
            var deltaX = Math.Abs(destination.X - origin.X);
            if(deltaX > SIZE/2)
            {
                deltaX = SIZE - deltaX;
            }
            var deltaY = Math.Abs(destination.Y - origin.Y);
            if (deltaY > SIZE / 2)
            {
                deltaY = SIZE - deltaY;
            }
            var distanceSquared = (deltaX * deltaX + deltaY * deltaY);

            return distanceSquared;
        }

        public void Load(string path, double[,] worldMapGenerated, Random random)
        {
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            _worldMapGenerated = worldMapGenerated;
            MapGeneratedMapToUltimaTiles();

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

            CleanupAndAddFeatures(random);
        }


        public void CleanupAndAddFeatures(Random random)
        {
            // Original game only had single tiles in very special circumstances
            RemoveSingleTiles();
            var rivers = AddRivers(random);
            Dictionary<Tile, List<River>> collectionOfRiversWithSameMouth = new Dictionary<Tile, List<River>>();

            foreach (var river in rivers)
            {
                var mouth = river.Path[river.Path.Count() - 1];
                if (collectionOfRiversWithSameMouth.ContainsKey(mouth))
                {
                    collectionOfRiversWithSameMouth[mouth].Add(river);
                }
                else
                {
                    collectionOfRiversWithSameMouth.Add(mouth, new List<River>());
                    collectionOfRiversWithSameMouth[mouth].Add(river);
                }
            }
            var riverCollections = collectionOfRiversWithSameMouth.Values.ToList();
            AddBridges(random, riverCollections);
            AddScrubAndForest(random, riverCollections);
            AddSwamp(random);
        }

        public void Save(string path)
        {
            var file = Path.Combine(path, filename);
            var worldFile = new System.IO.BinaryWriter(new System.IO.FileStream(file, System.IO.FileMode.OpenOrCreate));
            WriteMapToOriginalFormat(worldFile);
            worldFile.Close();
        }

        //public void WriteHashes(string path)
        //{
        //    var file = Path.Combine(path, filename);

        //    var worldHash = new Dictionary<string, string>();

        //    var hash = HashHelper.GetHashSha256(file);
        //    Console.WriteLine($"{file}: {HashHelper.BytesToString(hash)}");
        //    worldHash.Add(Path.GetFileName(file), HashHelper.BytesToString(hash));

        //    string json = JsonConvert.SerializeObject(worldHash); // the dictionary is inside client object
        //                                                             //write string to file
        //    System.IO.File.WriteAllText(@"world_hash.json", json);
        //}

        private void RemoveSingleTiles()
        {
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
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

        private void AddSwamp(Random random)
        {
            // 23
            int totalNumOfSwamps = 19 + random.Next(1, 3) + random.Next(1, 3);

            for(int i = 0; i < totalNumOfSwamps; i++)
            {
                var swampSize = 16;
                var chosenSwampTile = _potentialSwamps[random.Next(0, _potentialSwamps.Count() - 1)];
                var swamp = SwampMap(random, swampSize);

                for (int x = 0; x < swampSize; x++)
                {
                    for (int y = 0; y < swampSize; y++)
                    {
                        var tile = GetCoordinate(chosenSwampTile.X - swampSize / 2 + x, chosenSwampTile.Y - swampSize / 2 + y);
                        if (tile.GetTile() == TileInfo.Grasslands || tile.GetTile() == TileInfo.Scrubland)
                        {
                            if (swamp[x, y] == TileInfo.Swamp)
                            {
                                tile.SetTile(swamp[x, y]);
                            }
                        }
                    }
                }
            }

            return;
        }

        private static byte[,] SwampMap(Random random, int swampSize)
        {
            SimplexNoise.Noise.Seed = random.Next();
            var swampNoiseFloat = SimplexNoise.Noise.Calc2D(swampSize, swampSize, 0.1f);
            var swampNoise = Float2dToDouble2d(swampNoiseFloat, swampSize);

            var percentInMap = new Dictionary<byte, double>()
                {
                    {TileInfo.Grasslands,0.7},
                    {TileInfo.Swamp,0.3}
                };

            double halfSwampSize = Convert.ToDouble(swampSize)/2;
            for (int x = 0; x < swampSize; x++)
            {
                for (int y = 0; y < swampSize; y++)
                {
                    swampNoise[x, y] = swampNoise[x, y]
                        * (-Math.Pow(((x - halfSwampSize) / halfSwampSize), 2) + 1)
                        * (-Math.Pow(((y - halfSwampSize) / halfSwampSize), 2) + 1);
                }
            }

            var swamp = ClampToValuesInSetRatios(swampNoise, percentInMap, swampSize);

            return swamp;
        }

        private void FindOcean()
        {
            // Find Ocean
            var bodiesOfWater = new List<List<Tile>>();
            var closedSet = new HashSet<Tile>();
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    var tile = GetCoordinate(x, y);
                    if (!closedSet.Contains(tile) && (tile.GetTile() == TileInfo.Deep_Water || tile.GetTile() == TileInfo.Medium_Water) )
                    {
                        var bodyOfWater = new List<Tile>();
                        var queue = new Queue<Tile>();
                        queue.Enqueue(tile);
                        while (queue.Count() > 0)
                        {
                            tile = queue.Dequeue();
                            if (!closedSet.Contains(tile) && (tile.GetTile() == TileInfo.Deep_Water || tile.GetTile() == TileInfo.Medium_Water))
                            {
                                bodyOfWater.Add(tile);

                                foreach(var n in tile.NeighborAndAdjacentCoordinates())
                                {
                                    queue.Enqueue(n);
                                }
                            }

                            closedSet.Add(tile);
                        }
                        bodiesOfWater.Add(bodyOfWater);
                    }
                }
            }

            //for (int i = 0; i < bodiesOfWater.Count(); i++)
            //{
            //    foreach (var tile in bodiesOfWater[i])
            //    {
            //        tile.SetTile(Convert.ToByte(TileInfo.A + (i % 26)));
            //    }
            //}

            var ocean = bodiesOfWater.OrderByDescending(b => b.Count()).FirstOrDefault();

            return;
        }

        private void AddScrubAndForest(Random random, List<List<River>> rivers)
        {
            // Add to Rivers
            foreach(var riverCollection in rivers)
            {
                var openSet = new HashSet<Tile>();
                var queue = new Queue<Tuple<Tile, int>>();
                foreach (var river in riverCollection)
                {
                    //foreach(var tile in river.Path)
                    for (int i = river.Head; i < river.Path.Count; i++)
                    {
                        if (!openSet.Contains(river.Path[i]))
                        {
                            openSet.Add(river.Path[i]);
                            queue.Enqueue(new Tuple<Tile,int>(river.Path[i], 0));
                        }
                    }
                }

                var finalSet = new HashSet<Tile>();
                var closedSet = new HashSet<Tile>();

                while(openSet.Count > 0)
                {
                    var current = queue.Dequeue();
                    openSet.Remove(current.Item1);
                    closedSet.Add(current.Item1);

                    if (current.Item2 < 4)
                    {
                        foreach (var neighbor in current.Item1.NeighborCoordinates())
                        {
                            if (!closedSet.Contains(neighbor))
                            {
                                if (!openSet.Contains(neighbor))
                                {
                                    if (neighbor.GetTile() == TileInfo.Grasslands)
                                    {
                                        finalSet.Add(neighbor);
                                        openSet.Add(neighbor);
                                        queue.Enqueue(new Tuple<Tile, int>(neighbor, current.Item2 + 1));
                                    }
                                }
                            }
                        }
                    }
                }

                foreach(var tile in finalSet)
                {
                    tile.SetTile(TileInfo.Scrubland);
                }
            }

            // Add other blobs
            var scrub = ScrubMap(random);

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    if (_worldMapTiles[x, y] == TileInfo.Grasslands)
                    {
                        _worldMapTiles[x, y] = scrub[x, y];
                    }
                }
            }
        }

        private void AddBridges(Random random, List<List<River>> rivers)
        {
            int totalNumOBridgedRivers = 9 + random.Next(1, 3) + random.Next(1, 3);

            var riverCollections = rivers.ToList();

            var bridgeCount = 0;            
            foreach(var riverCollection in riverCollections.ToList())
            {
                var headInMountains = false;
                foreach (var river in riverCollection)
                {
                    foreach (var neighbor in river.Path[river.Head].NeighborAndAdjacentCoordinates())
                    {
                        if (neighbor.GetTile() == TileInfo.Mountains)
                        {
                            headInMountains = true;
                        }
                    }
                }
                if(headInMountains)
                {
                    AddBridge(random, riverCollection, true);
                    riverCollections.Remove(riverCollection);
                    bridgeCount++;
                }
            }

            while (bridgeCount < totalNumOBridgedRivers && riverCollections.Count > 0)
            {
                var index = random.Next(0, riverCollections.Count());
                AddBridge(random, riverCollections[index], false);
                riverCollections.RemoveAt(index);
                bridgeCount++;
            }

            return;
        }

        private void AddBridge(Random random, List<River> riverCollection, bool tryHarder)
        {
            int minBridgeDepth = random.Next(1, 6);

            foreach (var river in riverCollection)
            {
                // Bridge can't be further out than the river

                var correctedMinBridgeDepth = (minBridgeDepth > river.Path.Count() ? 1 : minBridgeDepth);
                var maxBridgeDepth = Math.Max(river.Path.Count() - 9, 0);


                var bridgeAdded = false;
                for (int i = river.Path.Count() - correctedMinBridgeDepth; i > maxBridgeDepth; i--)
                {
                    var possibleBridge = river.Path[i];

                    if (_worldMapTiles[possibleBridge.X, possibleBridge.Y] != TileInfo.Shallow_Water)
                    {
                        break;
                    }

                    if (IsWalkableGround(GetCoordinate(possibleBridge.X - 1, possibleBridge.Y)) && IsWalkableGround(GetCoordinate(possibleBridge.X + 1, possibleBridge.Y)))
                    {
                        _worldMapTiles[possibleBridge.X, possibleBridge.Y] = TileInfo.Bridge;
                        bridgeAdded = true;
                        break;
                    }
                }

                if(!bridgeAdded && tryHarder)
                {
                    for (int i = river.Path.Count() - 1; i > correctedMinBridgeDepth; i--)
                    {
                        var possibleBridge = river.Path[i];

                        if (_worldMapTiles[possibleBridge.X, possibleBridge.Y] != TileInfo.Shallow_Water)
                        {
                            break;
                        }

                        if (IsWalkableGround(GetCoordinate(possibleBridge.X - 1, possibleBridge.Y)) && IsWalkableGround(GetCoordinate(possibleBridge.X + 1, possibleBridge.Y)))
                        {
                            _worldMapTiles[possibleBridge.X, possibleBridge.Y] = TileInfo.Bridge;
                            bridgeAdded = true;
                            break;
                        }
                    }
                }
            }
        }

        private List<River> AddRivers(Random random)
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

            var highPoints = new HashSet<Tile>();
            for (int riverNum = 0; riverNum < totalNumOfRivers; riverNum++)
            {
                var randomPoint = FindRandomPointHigherThan(TileInfo.Grasslands, random);

                // Track previous point so I can step back one when I find the highest point
                var prevPoint = randomPoint;
                var currPoint = randomPoint;
                Tile foundHighPoint = null;
                while (foundHighPoint == null)
                {
                    //_worldMapTiles[prevPoint.X, prevPoint.Y] = 0xA1;
                    var highestDirection = surroundingPoints.OrderByDescending(p => _worldMapGenerated[Wrap(currPoint.X + (p.X)), Wrap(currPoint.Y + (p.Y))]).First();

                    if (_worldMapGenerated[currPoint.X, currPoint.Y] < _worldMapGenerated[Wrap(currPoint.X + (highestDirection.X)), Wrap(currPoint.Y + (highestDirection.Y))])
                    {
                        prevPoint = currPoint;
                        //int distance = Convert.ToInt32(Math.Sqrt(highestDirection.Item1 * highestDirection.Item1 + highestDirection.Item2 * highestDirection.Item2));
                        int distance = Math.Abs(highestDirection.X != 0 ? highestDirection.X : highestDirection.Y);
                        currPoint = new Tile(Wrap(currPoint.X + highestDirection.X / distance), Wrap(currPoint.Y + highestDirection.Y / distance), _worldMapTiles);
                    }
                    else
                    {
                        foundHighPoint = currPoint;
                    }
                }

                //_worldMapTiles[prevPoint.X, prevPoint.Y] = 76;
                highPoints.Add(prevPoint);
            }

            var rivers = new List<River>();
            foreach (var highPoint in highPoints)
            {
                // find shortest path
                List<Tile> path = GetRiverPath(highPoint, IsCoordinateWater);
                path.RemoveAt(path.Count() - 1);
                var river = new River();
                river.Path = path;
                rivers.Add(river);
            }

            foreach (var river in rivers)
            {
                //Choose random spot along the path for the headwater
                //var start = random.Next(0, path.Count);
                // That is weighted towards the start of the path
                var max = river.Path.Count - 10;
                var min = 10;
                var start = Convert.ToInt32(Math.Floor(Math.Abs(random.NextDouble() - random.NextDouble()) * (1 + max - min) + min));
                //for( int i = 1; i < start; i++)
                //{
                //    _worldMapTiles[path[i].X, path[i].Y] = 0x70;
                //}
                river.Head = start;
                for (int i = start; i < river.Path.Count; i++)
                {
                    _worldMapTiles[river.Path[i].X, river.Path[i].Y] = TileInfo.Shallow_Water;
                }
            }

            // TODO: Make rivers fork

            return rivers;
        }

        public static bool Between(byte x, int v1, int v2)
        {
            if (v1 <= v2)
            {
                return x >= v1 && x <= v2;
            }
            else
            {
                return x >= v1 || x <= v2;
            }

            //return ((v1 <= v2) && (x >= v1 && x <= v2)) || ((v1 > v2) && (x >= v1 || x <= v2));
        }

        public List<Tile> GetRiverPath(Tile startTile, IsNodeValid matchesGoal)
        {
            return Search.GetPath(WorldMap.SIZE, WorldMap.SIZE, startTile, matchesGoal, delegate { return true; }, GoDownhillHueristic);
        }

        public List<Tile> FindAllByPattern(int[,] pattern)
        {
            var validCoordinates = new List<Tile>();
            for(int map_x = 0; map_x < SIZE; map_x++)
            {
                for (int map_y = 0; map_y < SIZE; map_y++)
                {
                    var matchesAll = true;
                    for (int pat_x = 0; pat_x < pattern.GetLength(0); pat_x++)
                    {
                        for (int pat_y = 0; pat_y < pattern.GetLength(1); pat_y++)
                        {
                            var tile = _worldMapTiles[Wrap(map_x + pat_x), Wrap(map_y + pat_y)];
                            if (_worldMapTiles[Wrap(map_x+pat_x), Wrap(map_y+pat_y)] != pattern[pat_x, pat_y] && pattern[pat_x, pat_y] != -1)
                            {
                                matchesAll = false;
                            }
                        }
                    }

                    if(matchesAll)
                    {
                        validCoordinates.Add(new Tile(map_x, map_y, _worldMapTiles));
                    }
                }
            }

            return validCoordinates;
        }

        //public delegate float NodeHuersticValue(Coordinate coord, IsNodeValid matchesGoal);
        public float GoDownhillHueristic(Tile coord, IsNodeValid matchesGoal)
        {
            if( matchesGoal(coord))
            { 
                return 1.0f; 
            }

            var range = _generatedMax - _generatedMin;
            var value = _worldMapGenerated[coord.X, coord.Y] - _generatedMin;

            return Convert.ToSingle(value / range);
        }

        public static bool IsWalkableGround(ITile coord)
        {
            return coord.GetTile() >= TileInfo.Swamp && coord.GetTile() <= TileInfo.Hills;
        }

        private static bool IsCoordinateWater(ITile coordinate)
        {
            return coordinate.GetTile() < TileInfo.Shallow_Water;
        }

        public HashSet<Tile> GetTilesNear(Tile tile, int distance)
        {
            var results = new HashSet<Tile>();
            for(int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                {
                    int x_res = tile.X + x;
                    int y_res = tile.Y + y;
                    results.Add(new Tile(x_res, y_res, _worldMapTiles));
                }
            }

            return results;
        }

        public List<Tile> GetPathableTilesNear(Tile goal, int distance, Func<Tile, bool> isWalkableGround)
        {
            var possibleTiles = GetTilesNear(goal, distance);
            var results = new HashSet<Tile>();
            var pathableSet = new HashSet<Tile>();

            results = Search.GetSuccessfulPaths(SIZE, SIZE, goal, possibleTiles, c => { return isWalkableGround(c); });


            return results.ToList();
        }

        private Tile FindRandomPointHigherThan(int tile, Random random)
        {
            Tile result = null;
            while (result == null)
            {
                var x = random.Next(0, 256);
                var y = random.Next(0, 256);

                if (_worldMapTiles[x, y] > tile)
                {
                    result = new Tile(x, y, _worldMapTiles);
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

        static private Dictionary<byte, double> _percentInMap = new Dictionary<byte, double>()
        {
            {TileInfo.Deep_Water,0.519012451171875},
            {TileInfo.Medium_Water,0.15771484375},
            {TileInfo.Shallow_Water,0.0294952392578125},
            {TileInfo.Swamp,0.010162353515625},
            {TileInfo.Grasslands,0.1092376708984375},
            {TileInfo.Scrubland,0.07513427734375},
            {TileInfo.Forest,0.03515625},
            {TileInfo.Hills,0.0355224609375},
            {TileInfo.Mountains,0.0266265869140625},
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

        public SixLabors.ImageSharp.Image ToImage()
        {
            var image = new SixLabors.ImageSharp.Image<Rgba32>(WorldMap.SIZE, WorldMap.SIZE);
            for (int y = 0; y < WorldMap.SIZE; y++)
            {
                Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(y);
                for (int x = 0; x < WorldMap.SIZE; x++)
                {
                    if (colorMap.ContainsKey(_worldMapTiles[x, y]))
                    {
                        pixelRowSpan[x] = colorMap[_worldMapTiles[x, y]];
                    }
                    else
                    {
                        pixelRowSpan[x] = SixLabors.ImageSharp.Color.White;
                    }

                }
            }

            return image;
        }

        static private Dictionary<byte, SixLabors.ImageSharp.Color> colorMap = new Dictionary<byte, SixLabors.ImageSharp.Color>()
        {
            {TileInfo.Deep_Water, SixLabors.ImageSharp.Color.FromRgb(0, 0, 112) },
            {TileInfo.Medium_Water, SixLabors.ImageSharp.Color.FromRgb(20,20,112) },
            {TileInfo.Shallow_Water, SixLabors.ImageSharp.Color.FromRgb(60,60,112) },
            {TileInfo.Swamp, SixLabors.ImageSharp.Color.FromRgb(112, 0, 112) },
            {TileInfo.Grasslands, SixLabors.ImageSharp.Color.FromRgb(18, 112+18, 18) },
            {TileInfo.Scrubland, SixLabors.ImageSharp.Color.FromRgb(68, 112+68, 68) },
            {TileInfo.Forest, SixLabors.ImageSharp.Color.FromRgb(108,112+108,108) },
            {TileInfo.Hills, SixLabors.ImageSharp.Color.FromRgb(112+45,112+45,112+45) },
            {TileInfo.Mountains, SixLabors.ImageSharp.Color.FromRgb(112+15,112+15,112+15) },
            {TileInfo.Fire_Field, SixLabors.ImageSharp.Color.Orange },
            {TileInfo.Lava_Flow, SixLabors.ImageSharp.Color.Red },
            //{TileInfo.Slime_2, SixLabors.ImageSharp.Color.Purple },
        };
    }
}
