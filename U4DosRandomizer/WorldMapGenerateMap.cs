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
    public class WorldMapGenerateMap : WorldMapAbstract, IWorldMap
    {
        private double[,] _worldMapGenerated;

        private double _generatedMin;
        private double _generatedMax;
        private double _mountainMin;
        private double _mountainMax;
        private double[,] _mountHeightMap;
        private List<ITile> excludeLocations = new List<ITile>();
        private List<ITile> usedLocations = new List<ITile>();

        private Random randomDownhill;

        private SpoilerLog SpoilerLog { get; }

        public WorldMapGenerateMap(SpoilerLog spoilerLog)
        {
            SpoilerLog = spoilerLog;
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
            // Kill Mountains and hills too! We are special placing that later. (Maybe change the name of this function...)
            percentInMap[TileInfo.Grasslands] += percentInMap[TileInfo.Mountains];
            percentInMap.Remove(TileInfo.Mountains);
            percentInMap[TileInfo.Grasslands] += percentInMap[TileInfo.Hills];
            percentInMap.Remove(TileInfo.Hills);


            _worldMapTiles = ClampToValuesInSetRatios(mapGenerated, percentInMap, SIZE);

            // Kill swamps after generation so we can use their placements for later
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
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
                var index = Convert.ToInt32(Math.Floor((worldMapFlattenedList.Count - 1) * percentSum));
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

        private Tuple<byte[,], double[,]>  MountainMap(Random random)
        {
            var seed = random.Next();
            var scrubNoiseFloatLayerOne = SeamlessSimplexNoise.simplexnoise(seed, SIZE, SIZE, 0.0f, 0.8f);
            seed = random.Next();
            var scrubNoiseFloatLayerTwo = SeamlessSimplexNoise.simplexnoise(seed, SIZE, SIZE, 0.2f, 1.6f);

            var avgOne = scrubNoiseFloatLayerOne.Cast<float>().Average();
            var avgTwo = scrubNoiseFloatLayerOne.Cast<float>().Average();


            var scrubNoiseLayerOne = Float2dToDouble2d(scrubNoiseFloatLayerOne, SIZE);
            var scrubNoiseLayerTwo = Float2dToDouble2d(scrubNoiseFloatLayerTwo, SIZE);
            // 1 - abs of noise
            var scrubNoise = new double[SIZE, SIZE];
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    //scrubNoise[x, y] = scrubNoiseLayerOne[x, y] + (scrubNoiseLayerTwo[x, y] * 0.5);
                    scrubNoise[x, y] = (1 - Math.Abs(scrubNoiseLayerOne[x, y] - avgOne)) + ((1 - Math.Abs(scrubNoiseLayerTwo[x, y] - avgTwo)) * 0.3);
                    //scrubNoise[x, y] = (1 - Math.Abs(scrubNoiseLayerTwo[x, y] - avgTwo));
                }
            }

            //var avg = scrubNoise.Cast<double>().Average();

            //for(int x = 0; x < SIZE; x++)
            //{
            //    for (int y = 0; y < SIZE; y++)
            //    {
            //        scrubNoise[x, y] = 1 - Math.Abs(scrubNoise[x, y] - avg);
            //    }
            //}


            var totalGrass = _percentInMap[TileInfo.Mountains] + _percentInMap[TileInfo.Hills] + _percentInMap[TileInfo.Scrubland] + _percentInMap[TileInfo.Forest] + _percentInMap[TileInfo.Swamp] + _percentInMap[TileInfo.Shallow_Water] + _percentInMap[TileInfo.Grasslands];
            // As we will be overlaying it on the grass, scrubland and forest so bump up the percentage so the ratio stays correct
            var hillsPercent = _percentInMap[TileInfo.Hills] / totalGrass;
            var mountainsPercent = _percentInMap[TileInfo.Mountains] / totalGrass;
            var percentInMap = new Dictionary<byte, double>()
            {
                {TileInfo.Grasslands,(1.0-hillsPercent)-mountainsPercent},
                {TileInfo.Hills,hillsPercent},
                {TileInfo.Mountains,mountainsPercent }
            };

            return new Tuple<byte[,], double[,]>(ClampToValuesInSetRatios(scrubNoise, percentInMap, SIZE), scrubNoise);
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
                    scrubNoise[x, y] = scrubNoiseLayerOne[x, y] + (scrubNoiseLayerTwo[x, y] * 0.5);
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

            for (int i = 0; i < 13; i++)
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

        public override void Load(string path, int mapSeed, Random mapGeneratorSeed, Random randomMap)
        {
            randomDownhill = new Random(randomMap.Next());
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            _worldMapGenerated = new DiamondSquare(SIZE, 184643518.256878, mapSeed).getData(mapGeneratorSeed);
            MapGeneratedMapToUltimaTiles();

            _generatedMin = _worldMapGenerated.Cast<double>().Min();
            _generatedMax = _worldMapGenerated.Cast<double>().Max();

            CleanupAndAddFeatures(randomMap);
        }

        public override void Randomize(UltimaData ultimaData, Random randomLocations, Random randomItems)
        {
            
            RandomizeLocations(ultimaData, randomLocations);

            RandomizeItems(ultimaData, randomItems);
            WriteSpoilerLog(ultimaData);
        }

        private void WriteSpoilerLog(UltimaData data)
        {
            SpoilerLog.Add(SpoilerCategory.MiscWorldLocation, $"Pirate Cove at {Talk.GetSextantText(data.PirateCoveSpawnTrigger, ' ')}");
            for (int i = 0; i < data.LOC_HUMILITY - 1; i++)
            {
                SpoilerLog.Add(SpoilerCategory.Location, $"{data.LocationNames[i]} at {Talk.GetSextantText(data.GetLocation(i), ' ')}");
            }
            for (int i = 0; i < 8; i++)
            {
                SpoilerLog.Add(SpoilerCategory.Location, $"Moongate at {Talk.GetSextantText(data.Moongates[i])}");
            }
            for (int i = 0; i < data.Items.Count; i++)
            {
                SpoilerLog.Add(SpoilerCategory.Location, $"{data.ItemNames[i]} at {Talk.GetSextantText(data.Items[i], ' ')}");
            }
        }


        private void RandomizeLocations(UltimaData ultimaData, Random random)
        {
            // Lay down Stygian Abyss first so it doesn't stomp on other things
            // TODO: Make the entrance to the Abyss more random instead of laying down what is in the base game
            // Find a reasonable mountainous area
            var avatarIsleSizeX = 40 + 12;
            var avatarIsleSizeY = 80 + 12;
            var possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Deep_Water, avatarIsleSizeX, avatarIsleSizeY, c, false));
            var stygianUpperLeft = possibleLocations[random.Next(0, possibleLocations.Count)];

            var stygian = GetCoordinate(stygianUpperLeft.X + 20 + 5, stygianUpperLeft.Y + 60 + 4);
            
            // Get a path from the entrance to water
            //var entranceToStygian = GetCoordinate(stygian.X - 14, stygian.Y - 9);
            //var entrancePathToWater = worldMap.GetRiverPath(entranceToStygian, c => { return c.GetTile() == TileInfo.Deep_Water; } );

            var shapeLoc = new Coordinate(stygianUpperLeft.X + 6, stygianUpperLeft.Y + 6);
            ApplyShape(shapeLoc, "abyss", false);

            var ocean = FindOcean();

            //var entrancePathToWater = Search.GetPath(SIZE, SIZE, entranceToStygian,
            //    c => { return ocean.Contains(c); }, // Find deep water to help make sure a boat can reach here. TODO: Make sure it reaches the ocean.
            //    c => { return !(Between(c.X, Wrap(shapeLoc.X - 12), Wrap(shapeLoc.X + 12)) && Between(c.Y, Wrap(shapeLoc.Y - 12), Wrap(shapeLoc.Y + 12))); },
            //    GoDownhillHueristic);

            //for (int i = 0; i < entrancePathToWater.Count; i++)
            //{
            //    GetCoordinate(entrancePathToWater[i].X, entrancePathToWater[i].Y).SetTile(TileInfo.Medium_Water);
            //}

            for (int x = 0; x <= avatarIsleSizeX; x++)
            {
                for (int y = 0; y <= avatarIsleSizeY; y++)
                {
                    excludeLocations.Add(GetCoordinate(stygianUpperLeft.X + x, stygianUpperLeft.Y + y));
                }
            }

            //Pirate Cove - Set locations based off Stygian location
            var originalX = 0xe9; // Original Stygian location
            var originalY = 0xe9;
            for (var i = 0; i < ultimaData.PirateCove.Count; i++)
            {
                ultimaData.PirateCove[i].X = Convert.ToByte(Wrap(ultimaData.PirateCove[i].X - originalX + stygian.X));
                ultimaData.PirateCove[i].Y = Convert.ToByte(Wrap(ultimaData.PirateCove[i].Y - originalY + stygian.Y));
            }
            ultimaData.PirateCoveSpawnTrigger = new Coordinate(ultimaData.PirateCoveSpawnTrigger.X - originalX + stygian.X, ultimaData.PirateCoveSpawnTrigger.Y - originalY + stygian.Y);
            //worldMap.GetCoordinate(ultimaData.PirateCoveSpawnTrigger.X, ultimaData.PirateCoveSpawnTrigger.Y).SetTile(TileInfo.A);

            // Blink Exclusion
            // Cast exclusion isn't precise enough so allow them to cast anywhere and exclude the destination
            ultimaData.BlinkCastExclusionX1 = 0x01;
            ultimaData.BlinkCastExclusionX2 = 0x01;
            ultimaData.BlinkCastExclusionY1 = 0x01;
            ultimaData.BlinkCastExclusionY2 = 0x01;
            ultimaData.BlinkDestinationExclusionX1 = Convert.ToByte(Wrap(stygianUpperLeft.X));
            ultimaData.BlinkDestinationExclusionY1 = Convert.ToByte(Wrap(stygianUpperLeft.Y));
            ultimaData.BlinkDestinationExclusionX2 = Convert.ToByte(Wrap(stygianUpperLeft.X+avatarIsleSizeX));
            ultimaData.BlinkDestinationExclusionY2 = Convert.ToByte(Wrap(stygianUpperLeft.Y+avatarIsleSizeY));

            //for (int x = 0; x < WorldMap.SIZE; x++)
            //{
            //    for (int y = 0; y < WorldMap.SIZE; y++)
            //    {
            //        if (WorldMap.Between(Convert.ToByte(x), ultimaData.BlinkExclusionX1, ultimaData.BlinkExclusionX2)
            //            && WorldMap.Between(Convert.ToByte(y), ultimaData.BlinkExclusionY1, ultimaData.BlinkExclusionY2))
            //        {
            //            worldMap.GetCoordinate(x, y).SetTile(TileInfo.Lava_Flow);
            //        }
            //    }
            //}

            // Buildings
            possibleLocations = GetAllMatchingTiles(WorldMapGenerateMap.IsWalkableGround);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            List<ITile> evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, 8, usedLocations, possibleLocations, ultimaData, false);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();

            // Towns
            ITile loc = null;
            for (int i = 0; i < 7; i++)
            {
                loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Town);
                ultimaData.Towns[i].X = loc.X;
                ultimaData.Towns[i].Y = loc.Y;
                usedLocations.Add(ultimaData.Towns[i]);
            }
            loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Ruins); // Magincia
            ultimaData.Towns[7].X = loc.X;
            ultimaData.Towns[7].Y = loc.Y;

            // Castles
            var numLocations = 4 + ultimaData.Castles.Count + ultimaData.Shrines.Count - 1;
            evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, numLocations, usedLocations, possibleLocations, ultimaData, true);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();
            for (int i = 0; i < 3; i++)
            {
                loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Castle);
                ultimaData.Castles[i].X = loc.X;
                ultimaData.Castles[i].Y = loc.Y;
            }

            // Villages
            for (int i = 0; i < 4; i++)
            {
                loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Village);
                ultimaData.Towns[i + 8].X = loc.X;
                ultimaData.Towns[i + 8].Y = loc.Y;
            }

            // Shrines
            for (int i = 0; i < 7; i++)
            {
                if (i == 5)
                {
                    // Unchanged spot for spirit
                }
                else
                {
                    loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Shrine);
                    ultimaData.Shrines[i].X = loc.X;
                    ultimaData.Shrines[i].Y = loc.Y;
                }
            }
            // Humility
            // TODO: Shrine prettier
            //possibleLocations = GetAllMatchingTiles(c => GoodForHumility(c));
            //possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            //loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            loc = GetCoordinate(stygianUpperLeft.X + 23, stygianUpperLeft.Y + 47);
            loc.SetTile(TileInfo.Shrine);
            ultimaData.Shrines[7].X = loc.X;
            ultimaData.Shrines[7].Y = loc.Y;
            //for (int y = -4; y < 0; y++)
            //{
            //    GetCoordinate(loc.X, loc.Y + y).SetTile(TileInfo.Hills);
            //}
            ultimaData.DaemonSpawnLocationX = loc.X;
            ultimaData.DaemonSpawnX1 = Wrap(loc.X - 1);
            ultimaData.DaemonSpawnX2 = Wrap(loc.X + 1);
            ultimaData.DaemonSpawnY1 = Wrap(loc.Y - 4);
            ultimaData.DaemonSpawnY2 = Wrap(loc.Y + 1);
            //ultimaData.BlinkDestinationExclusion2X1 = ultimaData.DaemonSpawnX1;
            //ultimaData.BlinkDestinationExclusion2X2 = ultimaData.DaemonSpawnX2;
            //ultimaData.BlinkDestinationExclusion2Y1 = ultimaData.DaemonSpawnY1;
            //ultimaData.BlinkDestinationExclusion2Y2 = ultimaData.DaemonSpawnY2;

            // Moongates
            List<ITile> path = new List<ITile>();
            List<byte> validTiles = new List<byte>() { TileInfo.Grasslands, TileInfo.Scrubland, TileInfo.Swamp, TileInfo.Forest, TileInfo.Hills };
            for (int i = 0; i < 8; i++)
            {
                path = new List<ITile>();
                var distance = random.Next(5, 10);
                while (path.Count == 0 && distance > 0)
                {
                    path = Search.GetPath(SIZE, SIZE, ultimaData.Towns[i],
                        // Move at least 9 spaces away from from the entrance
                        c => { return distance * distance <= DistanceSquared(c, ultimaData.Towns[i]) && IsWalkable(c); },
                        // Only valid if all neighbors all also mountains
                        c => { return IsMatchingTile(c, validTiles); },
                        (c, cf, b) => { return (float)random.NextDouble(); });
                    if (path.Count == 0)
                    {
                        Console.WriteLine($"Failed Moongate placement of {i} placement. Retrying.");
                        distance--;
                    }
                    else
                    {
                        loc = path[path.Count - 1];
                        loc.SetTile(TileInfo.Grasslands);
                        foreach (var n in loc.NeighborCoordinates())
                        {
                            if (validTiles.Contains(n.GetTile()))
                            {
                                n.SetTile(TileInfo.Grasslands);
                            }
                        }
                        possibleLocations.Remove(loc);
                        ultimaData.Moongates[i].X = loc.X;
                        ultimaData.Moongates[i].Y = loc.Y;
                        excludeLocations.Add(ultimaData.Moongates[i]);
                    }
                }
                if (distance == 0)
                {
                    Console.WriteLine($"Utterly failed at Moongate placement of {i} placement. Trying random.");
                    possibleLocations = GetAllMatchingTiles(IsGrass);
                    possibleLocations.RemoveAll(c => excludeLocations.Contains(c));

                    loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Grasslands);
                    ultimaData.Moongates[i].X = loc.X;
                    ultimaData.Moongates[i].Y = loc.Y;
                }
            }

            // LCB
            var placed = false;
            while (!placed)
            {
                var lcb = GetRandomCoordinate(random);
                var lcbEntrance = GetCoordinate(lcb.X, lcb.Y + 1);
                Tile lcbWestSide = GetCoordinate(lcb.X - 1, lcb.Y);
                Tile lcbEastSide = GetCoordinate(lcb.X + 1, lcb.Y);

                path = new List<ITile>();
                if (IsWalkableGround(lcb) && IsWalkableGround(lcbEntrance) && !excludeLocations.Contains(lcb) && !excludeLocations.Contains(lcbWestSide) && !excludeLocations.Contains(lcbEastSide))
                {
                    path = Search.GetPath(SIZE, SIZE, lcbEntrance,
                        // Gotta be able to walk to a Moongate from LCB
                        c => { return ultimaData.Moongates.Contains(c); },
                        // Only valid if all neighbors all also mountains
                        c => { return IsWalkable(c) && c != lcbEastSide && c != lcbWestSide && c != lcbEastSide; },
                        (c, cf, b) => { return (float)random.NextDouble(); });
                    if (path.Count > 0)
                    {
                        lcb.SetTile(TileInfo.Lord_British_s_Castle_Entrance);
                        ultimaData.LCB[0].X = lcb.X;
                        ultimaData.LCB[0].Y = lcb.Y;
                        lcbWestSide.SetTile(TileInfo.Lord_British_s_Caste_West);
                        ultimaData.LCB[1].X = lcbWestSide.X;
                        ultimaData.LCB[1].Y = lcbWestSide.Y;
                        lcbEastSide.SetTile(TileInfo.Lord_British_s_Castle_East);
                        ultimaData.LCB[1].X = lcbEastSide.X;
                        ultimaData.LCB[1].Y = lcbEastSide.Y;

                        placed = true;
                    }
                }
            }

            // Dungeons
            possibleLocations = GetAllMatchingTiles(c => c.GetTile() == TileInfo.Mountains && IsWalkableGround(GetCoordinate(c.X, c.Y + 1)) && Search.GetPath(SIZE, SIZE, c,
            coord => { return IsGrass(coord) || coord.GetTile() == TileInfo.Deep_Water; },
            IsWalkableOrSailable).Count > 0);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, 8, usedLocations, possibleLocations, ultimaData, false);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();
            for (int i = 0; i < 6; i++)
            {
                loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Dungeon_Entrance);
                ultimaData.Dungeons[i].X = loc.X;
                ultimaData.Dungeons[i].Y = loc.Y;
            }

            // special for Hythloth
            // TODO: Hythloth prettier
            possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Mountains, 4, c));

            path = new List<ITile>();
            while (path.Count == 0)
            {
                loc = possibleLocations[random.Next(0, possibleLocations.Count)];
                possibleLocations.Remove(loc);
                path = Search.GetPath(SIZE, SIZE, loc,
                    // Move at least 9 spaces away from from the entrance
                    c => { return 9 * 9 <= DistanceSquared(c, loc); },
                    // Only valid if all neighbors all also mountains
                    c => { return c.GetTile() == TileInfo.Mountains && c.NeighborAndAdjacentCoordinates().All(n => n.GetTile() == TileInfo.Mountains); },
                    GoDownhillHueristic);
                if (path.Count == 0)
                {
                    Console.WriteLine("Failed Hythloth placement. Retrying.");
                }
            }
            for (int i = 0; i < 3; i++)
            {
                path[i].SetTile(TileInfo.Grasslands);
            }
            for (int i = 3; i < path.Count; i++)
            {
                path[i].SetTile(TileInfo.Hills);
            }
            loc.SetTile(TileInfo.Dungeon_Entrance);
            //for(int i = 0; i < path.Count; i++)
            //{
            //    path[i].SetTile(WorldMap.Wrap( TileInfo.A + i));
            //}
            ultimaData.Dungeons[6].X = loc.X;
            ultimaData.Dungeons[6].Y = loc.Y;
            ultimaData.BalloonSpawn = path.Last();

            // Stygian Abyss
            ultimaData.Dungeons[7].X = stygian.X;
            ultimaData.Dungeons[7].Y = stygian.Y;

            // Move starting positions and abyss ejection locations to Towns
            for (int i = 0; i < 8; i++)
            {
                var validPositions = GetPathableTilesNear(ultimaData.Towns[i], 3, IsWalkable);
                loc = validPositions[random.Next(0, validPositions.Count)];
                ultimaData.StartingPositions[i].X = loc.X;
                ultimaData.StartingPositions[i].Y = loc.Y;
                ultimaData.AbyssEjectionLocations[i].X = loc.X;
                ultimaData.AbyssEjectionLocations[i].Y = loc.Y;
            }

            // More ejection locations for Castles
            for (int i = 0; i < 3; i++)
            {
                var validPositions = GetPathableTilesNear(ultimaData.Castles[i], 3, IsWalkable);
                loc = validPositions[random.Next(0, validPositions.Count)];
                ultimaData.AbyssEjectionLocations[i + 8].X = loc.X;
                ultimaData.AbyssEjectionLocations[i + 8].Y = loc.Y;
            }

            // Ejection for LCB
            ultimaData.AbyssEjectionLocations[11].X = ultimaData.LCB[0].X;
            ultimaData.AbyssEjectionLocations[11].Y = (byte)(ultimaData.LCB[0].Y + 1);

            // Ejection for Abyss
            ultimaData.AbyssEjectionLocations[12].X = stygian.X;
            ultimaData.AbyssEjectionLocations[12].Y = stygian.Y;

            // Whirlpool normally exits in Lock Lake
            // TODO: Put it somewhere more thematic
            // For now stick it in the middle of some deep water somewhere
            loc = GetRandomCoordinate(random, c => c.GetTile() == TileInfo.Deep_Water, excludeLocations);
            ultimaData.WhirlpoolExit = new Coordinate(loc.X, loc.Y);

            return;
        }

        private void RandomizeItems(UltimaData ultimaData, Random random)
        {
            ITile loc = null;// RandomizeLocation(random, TileInfo.Swamp, worldMap, WorldMap.IsWalkableGround, exclude);
            var possibleLocations = GetAllMatchingTiles(IsWalkableGround);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            List<ITile> evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, 2, usedLocations, possibleLocations, ultimaData, true);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();

            loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Swamp);
            ultimaData.Items[ultimaData.ITEM_MANDRAKE].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_MANDRAKE].Y = loc.Y;
            excludeLocations.Add(loc);

            loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, null);
            ultimaData.Items[ultimaData.ITEM_HORN].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_HORN].Y = loc.Y;
            excludeLocations.Add(loc);

            possibleLocations = GetAllMatchingTiles(c => c.GetTile() == TileInfo.Swamp);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, 1, usedLocations, possibleLocations, ultimaData, true);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();
            loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Swamp);
            ultimaData.Items[ultimaData.ITEM_MANDRAKE2].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_MANDRAKE2].Y = loc.Y;
            excludeLocations.Add(loc);

            possibleLocations = GetAllMatchingTiles(c => c.GetTile() == TileInfo.Forest);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, 2, usedLocations, possibleLocations, ultimaData, true);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();
            loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Forest);
            ultimaData.Items[ultimaData.ITEM_NIGHTSHADE].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_NIGHTSHADE].Y = loc.Y;
            excludeLocations.Add(loc);

            loc = RandomSelectFromListCheckPathChangeAndRemove(random, evenlyDistributedLocations, TileInfo.Forest);
            ultimaData.Items[ultimaData.ITEM_NIGHTSHADE2].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_NIGHTSHADE2].Y = loc.Y;
            excludeLocations.Add(loc);

            possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Deep_Water, 14, c) && !excludeLocations.Contains(c));
            evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, 1, usedLocations, possibleLocations, ultimaData, true);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();
            loc = evenlyDistributedLocations[0];
            ultimaData.Items[ultimaData.ITEM_SKULL].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_SKULL].Y = loc.Y;
            ApplyShape(loc, "skull");
            excludeLocations.Add(loc);

            possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Deep_Water, 7, c) && !excludeLocations.Contains(c));
            evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, 1, usedLocations, possibleLocations, ultimaData, true);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();
            loc = evenlyDistributedLocations[0];
            ultimaData.Items[ultimaData.ITEM_BELL].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_BELL].Y = loc.Y;
            ApplyShape(loc, "bell");
            excludeLocations.Add(loc);

            // TODO Put in ocean
            possibleLocations = GetAllMatchingTiles(c => c.GetTile() == TileInfo.Deep_Water && !excludeLocations.Contains(c));
            evenlyDistributedLocations = GetEvenlyDistributedValidLocations(random, 1, usedLocations, possibleLocations, ultimaData, true);
            possibleLocations = possibleLocations.Except(evenlyDistributedLocations).ToList();
            loc = evenlyDistributedLocations[0];
            ultimaData.Items[ultimaData.ITEM_WHEEL].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_WHEEL].Y = loc.Y;

            // TODO: Do I move the black stone?
            ultimaData.Items[ultimaData.ITEM_BLACK_STONE].X = ultimaData.Moongates[0].X;
            ultimaData.Items[ultimaData.ITEM_BLACK_STONE].Y = ultimaData.Moongates[0].Y;

            // White stone
            possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Mountains, 4, c) && !excludeLocations.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[ultimaData.ITEM_WHITE_STONE].X = Convert.ToByte(loc.X - 1);
            ultimaData.Items[ultimaData.ITEM_WHITE_STONE].Y = loc.Y;
            ApplyShape(loc, "white");
        }

        public void CleanupAndAddFeatures(Random random)
        {
            AddMountainsAndHills(random);
            // Original game only had single tiles in very special circumstances
            RemoveSingleTiles();

            var rivers = AddRivers(random);
            Dictionary<ITile, List<River>> collectionOfRiversWithSameMouth = new Dictionary<ITile, List<River>>();

            // Original game only had single tiles in very special circumstances
            RemoveSingleTiles();

            AddBridges(random, rivers);
            AddScrubAndForest(random, rivers);
            AddSwamp(random);
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

            for (int i = 0; i < totalNumOfSwamps; i++)
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

            double halfSwampSize = Convert.ToDouble(swampSize) / 2;
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

        private List<ITile> FindOcean()
        {
            // Find Ocean
            var bodiesOfWater = new List<List<ITile>>();
            var closedSet = new HashSet<ITile>();
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    ITile tile = GetCoordinate(x, y);
                    if (!closedSet.Contains(tile) && (tile.GetTile() == TileInfo.Deep_Water || tile.GetTile() == TileInfo.Medium_Water))
                    {
                        var bodyOfWater = new List<ITile>();
                        var queue = new Queue<ITile>();
                        queue.Enqueue(tile);
                        while (queue.Count() > 0)
                        {
                            tile = queue.Dequeue();
                            if (!closedSet.Contains(tile) && (tile.GetTile() == TileInfo.Deep_Water || tile.GetTile() == TileInfo.Medium_Water))
                            {
                                bodyOfWater.Add(tile);

                                foreach (var n in tile.NeighborAndAdjacentCoordinates())
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

            return ocean;
        }

        private void AddMountainsAndHills(Random random)
        {
            // Add other blobs
            var mountains = MountainMap(random);

            // Map the mountain noise to the same number range as the generated world maps so we can put them together
            var mountainMin = mountains.Item2.Cast<double>().Min();
            var mountainMax = mountains.Item2.Cast<double>().Max();
            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    //https://math.stackexchange.com/questions/914823/shift-numbers-into-a-different-range/914843
                    mountains.Item2[x, y] = _generatedMin + (((_generatedMax - _generatedMin) / (mountainMax - mountainMin)) * (mountains.Item2[x, y] - mountainMin));
                    //mountains.Item2[x, y] = mountains.Item2[x, y] * mountains.Item2[x, y];
                }
            }

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    if (_worldMapTiles[x, y] == TileInfo.Grasslands)
                    {
                        _worldMapTiles[x, y] = mountains.Item1[x, y];
                    }
                }
            }

            _generatedMin = _worldMapGenerated.Cast<double>().Min();
            _generatedMax = _worldMapGenerated.Cast<double>().Max();

            _mountHeightMap = mountains.Item2;
            _mountainMin = _mountHeightMap.Cast<double>().Min();
            _mountainMax = _mountHeightMap.Cast<double>().Max();
        }

        private void AddScrubAndForest(Random random, List<River> rivers)
        {
            // Add to Rivers
            var openSet = new HashSet<ITile>();
            var queue = new Queue<Tuple<ITile, int>>();
            foreach (var river in rivers)
            {
                //foreach(var tile in river.Path)
                for (int i = 0; i < river.Path.Count; i++)
                {
                    if (!openSet.Contains(river.Path[i]))
                    {
                        openSet.Add(river.Path[i]);
                        queue.Enqueue(new Tuple<ITile, int>(river.Path[i], 0));
                    }
                }
            }

            var finalSet = new HashSet<ITile>();
            var closedSet = new HashSet<ITile>();

            while (openSet.Count > 0)
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
                                    queue.Enqueue(new Tuple<ITile, int>(neighbor, current.Item2 + 1));
                                }
                            }
                        }
                    }
                }
            }

            foreach (var tile in finalSet)
            {
                tile.SetTile(TileInfo.Scrubland);
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

        private void AddBridges(Random random, List<River> rivers)
        {
            int totalNumOBridgedRivers = 9 + random.Next(1, 3) + random.Next(1, 3);

            var bridgeCount = 0;

            var riversCopy = rivers.ToList<River>();
            
            foreach (var river in rivers)
            {
                var headInMountains = false;
                foreach (var riverTile in river.Path)
                {
                    foreach (var neighbor in riverTile.NeighborAndAdjacentCoordinates())
                    {
                        if (neighbor.GetTile() == TileInfo.Mountains)
                        {
                            headInMountains = true;
                        }
                    }
                }
                if (headInMountains)
                {
                    AddBridge(random, river, true);
                    riversCopy.Remove(river);
                    bridgeCount++;
                }
            }


            while (bridgeCount < totalNumOBridgedRivers && riversCopy.Count > 0)
            {
                var index = random.Next(0, riversCopy.Count());
                AddBridge(random, riversCopy[index], false);
                riversCopy.RemoveAt(index);
                bridgeCount++;
            }

            return;
        }

        private void AddBridge(Random random, River river, bool tryHarder)
        {
            int minBridgeDepth = random.Next(1, 6);

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

            if (!bridgeAdded && tryHarder)
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

        private List<River> AddRivers(Random random)
        {
            // There are ~32 in the original Ultima map
            int totalNumOfRivers = 28 + random.Next(1, 3) + random.Next(1, 3);

            //find possible points
            var possible = new List<ITile>();
            for(int x = 0; x < SIZE; x++)
            {
                for(int y = 0; y < SIZE; y++)
                {
                    var coord = GetCoordinate(x, y);
                    if(IsWalkable(coord))
                    {
                        if(IsSailableWater(GetCoordinate(x + 1, y)) ||
                           IsSailableWater(GetCoordinate(x - 1, y)) ||
                           IsSailableWater(GetCoordinate(x, y + 1)) ||
                           IsSailableWater(GetCoordinate(x, y - 1)))
                        {
                            possible.Add(coord);
                        }
                    }
                }
            }

            var usedLocations = new List<ITile>();
            var chosenLocations = GetEvenlyDistributedValidLocations(random, totalNumOfRivers, usedLocations, possible, null, false, 100);

            var rivers = new List<River>();
            for (int i = 0; i < chosenLocations.Count; i++)
            {
                var mouth = chosenLocations[i];
                Tuple<int, int> direction = null;

                if (IsWater(GetCoordinate(mouth.X + 1, mouth.Y)))
                {
                    direction = new Tuple<int, int>(-1, 0); ;
                }
                else if (IsWater(GetCoordinate(mouth.X - 1, mouth.Y)))
                {
                    direction = new Tuple<int, int>(1, 0); ;
                }
                if (IsWater(GetCoordinate(mouth.X, mouth.Y + 1)))
                {
                    direction = new Tuple<int, int>(0, -1); ;
                }
                if (IsWater(GetCoordinate(mouth.X, mouth.Y - 1)))
                {
                    direction = new Tuple<int, int>(0, 1); ;
                }

                var riverLength = random.Next(6) + random.Next(6) + random.Next(6);

                var riverPath = RiverTributary(random, mouth, direction, riverLength, TileInfo.A);
                if(riverPath.Count > 0)
                {
                    rivers.Add(new River()
                    {
                        Path = riverPath
                    });

                    foreach(var node in riverPath)
                    {
                        node.SetTile(TileInfo.Shallow_Water);
                    }
                }
                else
                {
                    //mouth.SetTile(TileInfo.F);
                    usedLocations.Remove(mouth);
                    chosenLocations[i] = GetEvenlyDistributedValidLocations(random, 1, usedLocations, possible, null, false, 100)[0];
                    i--;
                }
            }

            return rivers;
        }

        private List<ITile> RiverTributary(Random random, ITile mouth, Tuple<int, int> direction, int riverLength, byte tile)
        {
            var river = new List<ITile>();
            var currentCoord = mouth;
            for (int i = 0; i < riverLength; i++)
            {
                var advanced = false;
                if (LookAhead(currentCoord, direction, IsWalkable, 2, 1, 1))
                {
                    _worldMapTiles[currentCoord.X, currentCoord.Y] = tile;
                    river.Add(currentCoord);
                    currentCoord = GetCoordinate(currentCoord.X + direction.Item1, currentCoord.Y + direction.Item2);
                    _worldMapTiles[currentCoord.X, currentCoord.Y] = tile;
                    river.Add(currentCoord);

                    advanced = true;
                }


                // Do we wiggle?
                Tuple<int, int> wiggleDirection = null;
                if (i != 0 && (!advanced && random.Next(16) < 10 || random.Next(16) < 5))
                {
                    var flipIt = (random.Next(2) == 0 ? -1 : 1);
                    wiggleDirection = new Tuple<int, int>(direction.Item2 * flipIt, direction.Item1 * flipIt);
                    var oppositeWiggleDirection = new Tuple<int, int>(direction.Item2, direction.Item1);

                    // Don't wiggle if there is water in the direction I am wiggling
                    //if (!IsWater(GetCoordinate(currentCoord.X + wiggleDirection.Item1, currentCoord.Y + wiggleDirection.Item2)) &&
                    //   !IsWater(GetCoordinate(currentCoord.X + wiggleDirection.Item1 * 2, currentCoord.Y + wiggleDirection.Item2 * 2)))
                    if (LookAhead(currentCoord, wiggleDirection, IsNotWater, 2, 1, 1))
                    {
                        currentCoord = GetCoordinate(currentCoord.X + wiggleDirection.Item1, currentCoord.Y + wiggleDirection.Item2);
                        _worldMapTiles[currentCoord.X, currentCoord.Y] = tile;
                        river.Add(currentCoord);
                    }
                    else if (LookAhead(currentCoord, oppositeWiggleDirection, IsNotWater, 2, 1, 1))
                    {
                        wiggleDirection = oppositeWiggleDirection;
                        currentCoord = GetCoordinate(currentCoord.X + wiggleDirection.Item1, currentCoord.Y + wiggleDirection.Item2);
                        _worldMapTiles[currentCoord.X, currentCoord.Y] = tile;
                        river.Add(currentCoord);
                    }
                    else
                    {
                        wiggleDirection = null;
                    }
                }

                if(!advanced && wiggleDirection == null)
                {
                    break;
                }

                // Do we split
                if (i != 0 && i != riverLength - 1 && random.Next(8) < 3)
                {

                    Tuple<int, int> splitDirection = null;
                    // If we already wiggled just go the other direction
                    if (wiggleDirection != null)
                    {
                        splitDirection = new Tuple<int, int>(wiggleDirection.Item1 * -1, wiggleDirection.Item2 * -1);
                    }
                    else
                    {
                        var flipIt = (random.Next(2) == 0 ? -1 : 1);
                        splitDirection = new Tuple<int, int>(direction.Item2 * flipIt, direction.Item1 * flipIt);
                    }

                    // Don't split if there is water in the direction I am splitting
                    //if (!IsWater(GetCoordinate(currentCoord.X + splitDirection.Item1, currentCoord.Y + splitDirection.Item2)) &&
                    //   !IsWater(GetCoordinate(currentCoord.X + splitDirection.Item1 * 2, currentCoord.Y + splitDirection.Item2 * 2)) &&
                    //   !IsWater(GetCoordinate(currentCoord.X + splitDirection.Item1 * 3, currentCoord.Y + splitDirection.Item2 * 3)))
                    var newTributaryCoord = GetCoordinate(currentCoord.X + splitDirection.Item1, currentCoord.Y + splitDirection.Item2);
                    if (LookAhead(newTributaryCoord, splitDirection, IsNotWater, 2, 1, 1))
                    {
                        
                        //_worldMapTiles[newTributaryCoord.X, newTributaryCoord.Y] = tile;
                        //river.Add(newTributaryCoord);
                        //newTributaryCoord = GetCoordinate(currentCoord.X + splitDirection.Item1, currentCoord.Y + splitDirection.Item2);
                        _worldMapTiles[newTributaryCoord.X, newTributaryCoord.Y] = tile;
                        river.Add(newTributaryCoord);
                        newTributaryCoord = GetCoordinate(newTributaryCoord.X + splitDirection.Item1, newTributaryCoord.Y + splitDirection.Item2);
                        _worldMapTiles[newTributaryCoord.X, newTributaryCoord.Y] = tile;
                        river.Add(newTributaryCoord);

                        river.AddRange(RiverTributary(random, currentCoord, direction, riverLength - i, (byte)(tile + 1)));
                        var max = riverLength + 1;
                        var min = i + 2;
                        riverLength = min;
                        if (max >= min)
                        {
                            riverLength = Convert.ToInt32(Math.Floor(Math.Abs(random.NextDouble() - random.NextDouble()) * (1 + max - min) + min));
                        }

                        //riverLength = random.Next(riverLength - i) + 1;
                        currentCoord = newTributaryCoord;
                    }
                }

            }

            return river;
        }

        private bool LookAhead(ITile currentCoord, Tuple<int, int> direction, Func<ITile, bool> tileMatcher, int forwardDist, int leftDist, int rightDist)
        {
            //if (IsWalkable(GetCoordinate(currentCoord.X + direction.Item1, currentCoord.Y + direction.Item2)) &&
            //       IsWalkable(GetCoordinate(currentCoord.X + direction.Item1 * 2, currentCoord.Y + direction.Item2 * 2)))

            var left = new Tuple<int, int>(direction.Item2, direction.Item1);
            var right = new Tuple<int, int>(direction.Item2 * -1, direction.Item1 * -1);
            bool result = true;
            for (int i = 1; i <= forwardDist && result; i++)
            {
                result = result && tileMatcher(GetCoordinate(currentCoord.X + direction.Item1 * i, currentCoord.Y + direction.Item2 * i));

                for(int l = 1; l <= leftDist; l++)
                {
                    result = result && tileMatcher(GetCoordinate(currentCoord.X + direction.Item1 * i + left.Item1 * l, currentCoord.Y + direction.Item2 * i + left.Item2 * l));
                }

                for (int r = 1; r <= rightDist; r++)
                {
                    result = result && tileMatcher(GetCoordinate(currentCoord.X + direction.Item1 * i + right.Item1 * r, currentCoord.Y + direction.Item2 * i + right.Item2 * r));
                }
            }

            return result;
        }

        public List<ITile> GetRiverPath(ITile startTile, IsNodeValid matchesGoal)
        {
            return Search.GetPath(WorldMapGenerateMap.SIZE, WorldMapGenerateMap.SIZE, startTile, matchesGoal, delegate { return true; }, GoDownhillHueristic);
        }

        public List<Tile> FindAllByPattern(int[,] pattern)
        {
            var validCoordinates = new List<Tile>();
            for (int map_x = 0; map_x < SIZE; map_x++)
            {
                for (int map_y = 0; map_y < SIZE; map_y++)
                {
                    var matchesAll = true;
                    for (int pat_x = 0; pat_x < pattern.GetLength(0); pat_x++)
                    {
                        for (int pat_y = 0; pat_y < pattern.GetLength(1); pat_y++)
                        {
                            var tile = _worldMapTiles[Wrap(map_x + pat_x), Wrap(map_y + pat_y)];
                            if (_worldMapTiles[Wrap(map_x + pat_x), Wrap(map_y + pat_y)] != pattern[pat_x, pat_y] && pattern[pat_x, pat_y] != -1)
                            {
                                matchesAll = false;
                            }
                        }
                    }

                    if (matchesAll)
                    {
                        validCoordinates.Add(new Tile(map_x, map_y, _worldMapTiles, v => Wrap(v)));
                    }
                }
            }

            return validCoordinates;
        }

        //public delegate float NodeHuersticValue(Coordinate coord, IsNodeValid matchesGoal);
        public float GoDownhillHueristic(ITile coord, ITile cameFrom, IsNodeValid matchesGoal)
        {
            if (matchesGoal(coord))
            {
                return 1.0f;
            }

            // Map to 0 - 1
            var value = ((0.0 + (((1.0 - 0.0) / (_mountainMax - _mountainMin)) * (_mountHeightMap[coord.X, coord.Y] - _mountainMin))));

            return Convert.ToSingle(value - (randomDownhill.NextDouble()/25));
        }

        public static bool IsWalkableGround(ITile coord)
        {
            return coord.GetTile() >= TileInfo.Swamp && coord.GetTile() <= TileInfo.Hills;
        }

        private static bool IsNotWater(ITile coordinate)
        {
            return !IsWater(coordinate);
        }
        private static bool IsWater(ITile coordinate)
        {
            return (coordinate.GetTile() == TileInfo.Deep_Water || coordinate.GetTile() == TileInfo.Medium_Water || coordinate.GetTile() == TileInfo.Shallow_Water || (coordinate.GetTile() >= TileInfo.A && coordinate.GetTile() <= TileInfo.Z));
        }

        private static bool IsSailableWater(ITile coordinate)
        {
            return coordinate.GetTile() < TileInfo.Shallow_Water;
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
                    result = new Tile(x, y, _worldMapTiles, v => Wrap(v));
                }
            }

            return result;
        }


        private bool AreaIsAll(int tile, int length, ICoordinate coordinate)
        {
            return AreaIsAll(tile, length, length, coordinate, true);
        }

        private bool AreaIsAll(int tile, int width, int height, ICoordinate coordinate, bool center)
        {
            int centerX = width / 2;
            int centerY = height / 2;

            if(!center)
            {
                centerX = 0;
                centerY = 0;
            }

            var result = true;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result = result && IsTile(coordinate.X - centerX + x, coordinate.Y - centerY + y, tile);
                }
            }

            return result;
        }


        private void ApplyShape(ICoordinate loc, string file, bool center = true)
        {
            //var shape = new System.IO.FileStream($"{file}", System.IO.FileMode.Open);
            object obj = U4DosRandomizer.Resources.Shapes.ResourceManager.GetObject(file, U4DosRandomizer.Resources.Shapes.Culture);
            var shape = ((byte[])(obj));

            var width = shape[0];
            var height = shape[1];
            byte[] shapeBytes = new byte[shape.Count() - 2];
            Array.Copy(shape, 2, shapeBytes, 0, shape.Count() - 2);

            int centerX = width / 2;
            int centerY = height / 2;
            if(!center)
            {
                centerX = 0;
                centerY = 0;
            }
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var idx = x + y * width;
                    var tile = shapeBytes[idx];
                    if (tile != 0xFF)
                    {
                        GetCoordinate(loc.X - centerX + x, loc.Y - centerY + y).SetTile(tile);
                    }
                }
            }
        }

        private List<ITile> GetEvenlyDistributedValidLocations(Random random, int totalResults, List<ITile> usedLocations, List<ITile> possibleLocations, UltimaData ultimaData, bool requirePath, int numCandidates = 10)
        {
            // Using Mitchell's best-candidate algorithm - https://bost.ocks.org/mike/algorithms/
            var results = new List<ITile>();

            var randomIdx = random.Next(0, possibleLocations.Count);
            var original = possibleLocations[randomIdx];

            results.Add(original);
            usedLocations.Add(original);

            for (int i = 0; i < totalResults; i++)
            {
                ITile bestCandidate = null;
                var bestDistance = 0;
                for (int sample = 0; sample < numCandidates; sample++)
                {
                    randomIdx = random.Next(0, possibleLocations.Count);
                    ITile selection = null; // possibleLocations[randomIdx];
                    if (requirePath)
                    {
                        while (selection == null)
                        {
                            selection = possibleLocations[randomIdx];
                            var path = Search.GetPath(WorldMapGenerateMap.SIZE, WorldMapGenerateMap.SIZE, selection,
                            c => { return IsWalkableOrSailable(c) || selection.Equals(c); },
                            c => { return ultimaData.Towns.Contains(c); });

                            if (path.Count == 0)
                            {
                                selection = null;
                            }
                        }
                    }
                    else
                    {
                        selection = possibleLocations[randomIdx];
                    }

                    var distance = DistanceSquared(FindClosest(selection, usedLocations), selection);

                    if (distance > bestDistance)
                    {
                        bestDistance = distance;
                        bestCandidate = selection;
                    }
                }

                var result = bestCandidate;
                results.Add(result);
                usedLocations.Add(result);
            }

            return results;
        }

        private ICoordinate FindClosest(ITile selection, List<ITile> locations)
        {
            ITile closest = null;
            var closestDistance = int.MaxValue;
            for (int i = 0; i < locations.Count(); i++)
            {
                var distance = DistanceSquared(selection, locations[i]);
                if (distance > 0 && distance < closestDistance)
                {
                    closest = locations[i];
                    closestDistance = distance;
                }
            }

            return closest;
        }

        private static ITile RandomSelectFromListCheckPathChangeAndRemove(Random random, List<ITile> possibleLocations, byte? tile)
        {
            ITile loc = null;
            while (loc == null && possibleLocations.Count > 0)
            {
                var randomIdx = random.Next(0, possibleLocations.Count);
                loc = possibleLocations[randomIdx];

                if (tile != null)
                {
                    loc.SetTile(tile.Value);
                }

                possibleLocations.RemoveAt(randomIdx);
            }

            if (loc == null)
            {
                if (tile != null)
                {
                    throw new Exception($"Failed to find location for {TileInfo.GetLabel(tile.Value)}.");
                }
                else
                {
                    throw new Exception($"Failed to find location with path to town.");
                }
            }

            return loc;
        }

        private Tile GetRandomCoordinate(Random random)
        {
            var loc = GetCoordinate(random.Next(0, WorldMapGenerateMap.SIZE), random.Next(0, WorldMapGenerateMap.SIZE));
            return loc;
        }

        private Tile GetRandomCoordinate(Random random, Func<Tile, bool> criteria, List<ITile> excludes)
        {
            while (true)
            {
                var loc = GetRandomCoordinate(random);
                if (criteria(loc) && !excludes.Contains(loc))
                {
                    return loc;
                }
            }
        }

        private bool GoodForHumility(ICoordinate coordinate)
        {
            var result = true;
            for (int y = -4; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    result = result && IsTile(coordinate.X + x, coordinate.Y + y, TileInfo.Mountains);
                }
            }

            var entrance = GetCoordinate(coordinate.X, coordinate.Y - 5);

            result = result && IsWalkable(entrance);

            if (result)
            {
                // Make sure we can reach it by boat or balloon
                var path = Search.GetPath(256, 256, entrance, IsGrassOrSailable, IsWalkableOrSailable, null);

                result = result && path.Count > 0;
            }

            return result;
        }


        public new SixLabors.ImageSharp.Image ToHeightMapImage()
        {
            var image = new SixLabors.ImageSharp.Image<Rgba32>(WorldMapGenerateMap.SIZE*2, WorldMapGenerateMap.SIZE*2);
            for (int y = 0; y < WorldMapGenerateMap.SIZE; y++)
            {
                Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(y);
                for (int x = 0; x < WorldMapGenerateMap.SIZE; x++)
                {
                    var val = (byte)((0 + (((Byte.MaxValue - 0) / (_generatedMax - _generatedMin)) * (_worldMapGenerated[x, y] - _generatedMin))));

                    //SixLabors.ImageSharp.Color.FromRgb(0, 0, 112)
                    pixelRowSpan[x] = SixLabors.ImageSharp.Color.FromRgb(val, val, val);
                }

                for (int x = 0; x < WorldMapGenerateMap.SIZE; x++)
                {
                    var val = (byte)((0 + (((Byte.MaxValue - 0) / (_mountainMax - _mountainMin)) * (_mountHeightMap[x, y] - _mountainMin))));

                    //SixLabors.ImageSharp.Color.FromRgb(0, 0, 112)
                    pixelRowSpan[SIZE+x] = SixLabors.ImageSharp.Color.FromRgb(val, val, val);
                }
            }

            for (int y = 0; y < WorldMapGenerateMap.SIZE; y++)
            {
                Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(y+SIZE);
                for (int x = 0; x < WorldMapGenerateMap.SIZE; x++)
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

        public static void PrintWorldMapInfo(string path)
        {
            var world = new byte[256 * 256];

            var file = Path.Combine(path, filename);
            using (FileStream stream = new FileStream(file, FileMode.Open))
            {
                stream.Read(world, 0, 256 * 256);
            }
            var worldList = world.ToList();

            worldList.Sort();

            var worldTileCount = new Dictionary<byte, int>();
            for (int i = 0; i < worldList.Count(); i++)
            {
                if (worldTileCount.ContainsKey(worldList[i]))
                {
                    worldTileCount[worldList[i]] = worldTileCount[worldList[i]] + 1;
                }
                else
                {
                    worldTileCount[worldList[i]] = 1;
                }
            }

            foreach (var key in worldTileCount.Keys)
            {
                //var output = $"{shapes[key]}:".PadRight(31) + $" {worldTileCount[key]/(256.0*256.0)}";
                var output = $"{{{key},{worldTileCount[key] / (256.0 * 256.0)}}}";
                Console.WriteLine(output);
            }
        }

        static private Dictionary<byte, double> _percentInMap = new Dictionary<byte, double>()
        {
            // With Avatar Isle
            //{TileInfo.Deep_Water,   0.519012451171875},
            //{TileInfo.Medium_Water, 0.15771484375},
            //{TileInfo.Shallow_Water,0.0294952392578125},
            //{TileInfo.Swamp,        0.010162353515625},
            //{TileInfo.Grasslands,   0.1092376708984375},
            //{TileInfo.Scrubland,    0.07513427734375},
            //{TileInfo.Forest,       0.03515625},
            //{TileInfo.Hills,        0.0355224609375},
            //{TileInfo.Mountains,    0.0266265869140625},
            ////{9,0.0001068115234375},
            ////{10,0.0001068115234375},
            ////{11,4.57763671875E-05},
            ////{12,6.103515625E-05},
            ////{13,1.52587890625E-05},
            ////{14,1.52587890625E-05},
            ////{15,1.52587890625E-05},
            ////{23,0.0002593994140625},
            ////{29,1.52587890625E-05},
            ////{30,0.0001068115234375},
            ////{61,1.52587890625E-05},
            ////{70,0.0001068115234375},
            ////Lava{76,0.001068115234375}

            // Without Avatar Isle
            {TileInfo.Deep_Water,   0.54730224609375},
            {TileInfo.Medium_Water, 0.146514892578125},
            {TileInfo.Shallow_Water,0.026275634765625},
            {TileInfo.Swamp,        0.0097198486328125},
            {TileInfo.Grasslands,   0.1090240478515625},
            {TileInfo.Scrubland,    0.07513427734375},
            {TileInfo.Forest,       0.03515625},
            {TileInfo.Hills,        0.03271484375},
            {TileInfo.Mountains,    0.0171966552734375},
            //{9,9.1552734375E-05},
            //{10,0.0001068115234375},
            //{11,4.57763671875E-05},
            //{12,6.103515625E-05},
            //{13,1.52587890625E-05},
            //{14,1.52587890625E-05},
            //{15,1.52587890625E-05},
            //{23,0.0002593994140625},
            //{29,1.52587890625E-05},
            //{30,9.1552734375E-05},
            //{61,1.52587890625E-05},
            //{70,4.57763671875E-05},
            //{76,0.00018310546875}
        };

    }
}
