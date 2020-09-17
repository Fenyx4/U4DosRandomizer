﻿using SixLabors.ImageSharp.PixelFormats;
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

        public void Load(string path, int overworld, int mapSeed, Random mapGeneratorSeed, Random randomMap)
        {
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            if (overworld == 5)
            {
                _worldMapGenerated = new DiamondSquare(SIZE, 184643518.256878, mapSeed).getData(mapGeneratorSeed);
                MapGeneratedMapToUltimaTiles();

                var worldMapFlattened = new double[SIZE * SIZE];

                for (int x = 0; x < SIZE; x++)
                {
                    for (int y = 0; y < SIZE; y++)
                    {
                        worldMapFlattened[x + y * SIZE] = _worldMapGenerated[x, y];
                    }
                }

                _generatedMin = worldMapFlattened.Min();
                _generatedMax = worldMapFlattened.Max();

                CleanupAndAddFeatures(randomMap);
            }
            else if(overworld == 0)
            {
                LoadOriginalMap(path);
            }
        }

        public void Randomize(UltimaData ultimaData, Random randomLocations, Random randomItems)
        {
            //Completely random location placements of buildings still. Just trying to make sure I'm editing the files correctly right now. Not looking for a cohesive map that makes sense.
            var exclude = RandomizeLocations(ultimaData, randomLocations);

            RandomizeItems(ultimaData, randomItems, exclude);
        }

        private List<Tile> RandomizeLocations(UltimaData ultimaData, Random random)
        {
            var excludeLocations = new List<Tile>();
            // Lay down Stygian Abyss first so it doesn't stomp on other things
            // TODO: Make the entrance to the Abyss more random instead of laying down what is in the base game
            // Find a reasonable mountainous area
            var possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Mountains, 3, c));
            var stygian = possibleLocations[random.Next(0, possibleLocations.Count)];
            // Get a path from the entrance to water
            var entranceToStygian = GetCoordinate(stygian.X - 14, stygian.Y - 9);
            //var entrancePathToWater = worldMap.GetRiverPath(entranceToStygian, c => { return c.GetTile() == TileInfo.Deep_Water; } );

            var shapeLoc = new Coordinate(stygian.X - 2, stygian.Y - 7);
            ApplyShape(shapeLoc, "abyss");

            var entrancePathToWater = Search.GetPath(SIZE, SIZE, entranceToStygian,
                c => { return c.GetTile() == TileInfo.Deep_Water; }, // Find deep water to help make sure a boat can reach here. TODO: Make sure it reaches the ocean.
                c => { return !(Between(c.X, Wrap(shapeLoc.X - 12), Wrap(shapeLoc.X + 12)) && Between(c.Y, Wrap(shapeLoc.Y - 12), Wrap(shapeLoc.Y + 12))); },
                GoDownhillHueristic);

            for (int i = 0; i < entrancePathToWater.Count; i++)
            {
                GetCoordinate(entrancePathToWater[i].X, entrancePathToWater[i].Y).SetTile(TileInfo.Medium_Water);
            }

            for (int x = -12; x <= 12; x++)
            {
                for (int y = -12; y <= 12; y++)
                {
                    excludeLocations.Add(GetCoordinate(shapeLoc.X + x, shapeLoc.Y + y));
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
            ultimaData.BlinkExclusionX1 = Convert.ToByte(Wrap(stygian.X - 5));
            ultimaData.BlinkExclusionY1 = Convert.ToByte(Wrap(stygian.Y - 18));
            ultimaData.BlinkExclusionX2 = Convert.ToByte(Wrap(stygian.X + 9));
            ultimaData.BlinkExclusionY2 = Convert.ToByte(Wrap(stygian.Y + 5));

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
            possibleLocations = GetAllMatchingTiles(WorldMap.IsWalkableGround);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));

            // Towns
            Tile loc = null;
            for (int i = 0; i < 7; i++)
            {
                loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Town, ultimaData, false);
                ultimaData.Towns.Add(loc);

            }
            loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Ruins, ultimaData, false); // Magincia
            ultimaData.Towns.Add(loc);

            // Castles
            for (int i = 0; i < 3; i++)
            {
                loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Castle, ultimaData);
                ultimaData.Castles.Add(loc);
            }

            // Villages
            for (int i = 0; i < 4; i++)
            {
                loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Village, ultimaData);
                ultimaData.Towns.Add(loc);
            }

            // Shrines
            for (int i = 0; i < 7; i++)
            {
                if (i == 5)
                {
                    // Empty spot for spirit
                    ultimaData.Shrines.Add(null);
                }
                else
                {
                    loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Shrine, ultimaData);
                    ultimaData.Shrines.Add(loc);
                }
            }
            // Humility
            // TODO: Shrine prettier
            possibleLocations = GetAllMatchingTiles(c => GoodForHumility(c));
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            loc.SetTile(TileInfo.Shrine);
            ultimaData.Shrines.Add(loc);
            for (int y = -4; y < 0; y++)
            {
                GetCoordinate(loc.X, loc.Y + y).SetTile(TileInfo.Hills);
            }
            ultimaData.DaemonSpawnLocationX = loc.X;
            ultimaData.DaemonSpawnX1 = Wrap(loc.X - 1);
            ultimaData.DaemonSpawnX2 = Wrap(loc.X + 1);
            ultimaData.DaemonSpawnY1 = Wrap(loc.Y - 4);
            ultimaData.DaemonSpawnY2 = Wrap(loc.Y + 1);
            ultimaData.BlinkExclusion2X1 = ultimaData.DaemonSpawnX1;
            ultimaData.BlinkExclusion2X2 = ultimaData.DaemonSpawnX2;
            ultimaData.BlinkExclusion2Y1 = ultimaData.DaemonSpawnY1;
            ultimaData.BlinkExclusion2Y2 = ultimaData.DaemonSpawnY2;

            // Moongates
            List<Tile> path = new List<Tile>();
            List<byte> validTiles = new List<byte>() { TileInfo.Grasslands, TileInfo.Scrubland, TileInfo.Swamp, TileInfo.Forest, TileInfo.Hills };
            for (int i = 0; i < 8; i++)
            {
                path = new List<Tile>();
                var distance = random.Next(5, 10);
                while (path.Count == 0 && distance > 0)
                {
                    path = Search.GetPath(SIZE, SIZE, ultimaData.Towns[i],
                        // Move at least 9 spaces away from from the entrance
                        c => { return distance * distance <= DistanceSquared(c, ultimaData.Towns[i]) && IsWalkable(c); },
                        // Only valid if all neighbors all also mountains
                        c => { return IsMatchingTile(c, validTiles); },
                        (c, b) => { return (float)random.NextDouble(); });
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
                        ultimaData.Moongates.Add(loc);
                    }
                }
                if (distance == 0)
                {
                    Console.WriteLine($"Utterly failed at Moongate placement of {i} placement. Trying random.");
                    possibleLocations = GetAllMatchingTiles(IsGrass);
                    possibleLocations.RemoveAll(c => excludeLocations.Contains(c));

                    loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Grasslands, ultimaData, false);
                    ultimaData.Moongates.Add(loc);
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

                path = new List<Tile>();
                if (IsWalkableGround(lcb) && IsWalkableGround(lcbEntrance) && !excludeLocations.Contains(lcb))
                {
                    path = Search.GetPath(SIZE, SIZE, lcbEntrance,
                        // Gotta be able to walk to a Moongate from LCB
                        c => { return ultimaData.Moongates.Contains(c); },
                        // Only valid if all neighbors all also mountains
                        c => { return IsWalkable(c) && c != lcbEastSide && c != lcbWestSide && c != lcbEastSide; },
                        (c, b) => { return (float)random.NextDouble(); });
                    if (path.Count > 0)
                    {
                        lcb.SetTile(TileInfo.Lord_British_s_Castle_Entrance);
                        ultimaData.LCB.Add(lcb);
                        lcbWestSide.SetTile(TileInfo.Lord_British_s_Caste_West);
                        ultimaData.LCB.Add(lcbWestSide);
                        lcbEastSide.SetTile(TileInfo.Lord_British_s_Castle_East);
                        ultimaData.LCB.Add(lcbEastSide);

                        placed = true;
                    }
                }
            }

            // Dungeons
            possibleLocations = GetAllMatchingTiles(c => c.GetTile() == TileInfo.Mountains && IsWalkableGround(GetCoordinate(c.X, c.Y + 1)) && Search.GetPath(SIZE, SIZE, c,
            coord => { return IsGrass(coord) || coord.GetTile() == TileInfo.Deep_Water; },
            IsWalkableOrSailable).Count > 0);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            for (int i = 0; i < 6; i++)
            {
                loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Dungeon_Entrance, ultimaData);
                ultimaData.Dungeons.Add(loc);
            }

            // special for Hythloth
            // TODO: Hythloth prettier
            possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Mountains, 4, c));

            path = new List<Tile>();
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
            ultimaData.Dungeons.Add(loc);
            ultimaData.BalloonSpawn = path.Last();

            // Stygian Abyss
            ultimaData.Dungeons.Add(stygian);

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

            return excludeLocations;
        }

        private void RandomizeItems(UltimaData ultimaData, Random random, List<Tile> exclude)
        {
            Tile loc = null;// RandomizeLocation(random, TileInfo.Swamp, worldMap, WorldMap.IsWalkableGround, exclude);
            var possibleLocations = GetAllMatchingTiles(IsWalkableGround);
            possibleLocations.RemoveAll(c => exclude.Contains(c));
            loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Swamp, ultimaData);
            ultimaData.Items[ultimaData.ITEM_MANDRAKE].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_MANDRAKE].Y = loc.Y;
            exclude.Add(loc);

            loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, null, ultimaData);
            ultimaData.Items[ultimaData.ITEM_HORN].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_HORN].Y = loc.Y;
            exclude.Add(loc);

            possibleLocations = GetAllMatchingTiles(c => c.GetTile() == TileInfo.Swamp);
            possibleLocations.RemoveAll(c => exclude.Contains(c));
            loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Swamp, ultimaData);
            ultimaData.Items[ultimaData.ITEM_MANDRAKE2].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_MANDRAKE2].Y = loc.Y;
            exclude.Add(loc);

            possibleLocations = GetAllMatchingTiles(c => c.GetTile() == TileInfo.Forest);
            possibleLocations.RemoveAll(c => exclude.Contains(c));
            loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Forest, ultimaData);
            ultimaData.Items[ultimaData.ITEM_NIGHTSHADE].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_NIGHTSHADE].Y = loc.Y;
            exclude.Add(loc);

            loc = RandomSelectFromListCheckPathChangeAndRemove(random, possibleLocations, TileInfo.Forest, ultimaData);
            ultimaData.Items[ultimaData.ITEM_NIGHTSHADE2].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_NIGHTSHADE2].Y = loc.Y;
            exclude.Add(loc);

            possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Deep_Water, 14, c) && !exclude.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[ultimaData.ITEM_SKULL].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_SKULL].Y = loc.Y;
            ApplyShape(loc, "skull");
            exclude.Add(loc);

            possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Deep_Water, 7, c) && !exclude.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[ultimaData.ITEM_BELL].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_BELL].Y = loc.Y;
            ApplyShape(loc, "bell");
            exclude.Add(loc);

            // TODO Put in ocean
            possibleLocations = GetAllMatchingTiles(c => c.GetTile() == TileInfo.Deep_Water && !exclude.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[ultimaData.ITEM_WHEEL].X = loc.X;
            ultimaData.Items[ultimaData.ITEM_WHEEL].Y = loc.Y;

            // TODO: Do I move the black stone?
            ultimaData.Items[ultimaData.ITEM_BLACK_STONE].X = ultimaData.Moongates[0].X;
            ultimaData.Items[ultimaData.ITEM_BLACK_STONE].Y = ultimaData.Moongates[0].Y;

            // White stone
            possibleLocations = GetAllMatchingTiles(c => AreaIsAll(TileInfo.Mountains, 4, c) && !exclude.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[ultimaData.ITEM_WHITE_STONE].X = Convert.ToByte(loc.X - 1);
            ultimaData.Items[ultimaData.ITEM_WHITE_STONE].Y = loc.Y;
            ApplyShape(loc, "white");
        }

        private void LoadOriginalMap(string path)
        {
            var file = Path.Combine(path, $"{filename}.orig");

            _worldMapTiles = new byte[SIZE, SIZE];

            int chunkwidth = 32;
            int chunkSize = chunkwidth * chunkwidth;
            byte[] chunk; // = new byte[chunkSize];
            System.IO.BinaryReader worldMap = new System.IO.BinaryReader(new System.IO.FileStream(file, System.IO.FileMode.Open));

            for (int chunkCount = 0; chunkCount < 64; chunkCount++)
            {
                chunk = worldMap.ReadBytes(chunkSize);

                // Copy the chunk over
                for (int i = 0; i < chunkSize; i++)
                {
                    _worldMapTiles[i % chunkwidth + chunkCount % 8 * chunkwidth,i / chunkwidth + chunkCount / 8 * chunkwidth] = chunk[i];
                }
            }
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


        private bool AreaIsAll(int tile, int length, ICoordinate coordinate)
        {
            int radius = length / 2;

            var result = true;
            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    result = result && IsTile(coordinate.X - radius + x, coordinate.Y - radius + y, tile);
                }
            }

            return result;
        }


        private void ApplyShape(ICoordinate loc, string file)
        {
            //var shape = new System.IO.FileStream($"{file}", System.IO.FileMode.Open);
            object obj = U4DosRandomizer.Resources.Shapes.ResourceManager.GetObject(file, U4DosRandomizer.Resources.Shapes.Culture);
            var shape = ((byte[])(obj));

            var length = shape[0];
            byte[] shapeBytes = new byte[shape.Count() - 1];
            Array.Copy(shape, 1, shapeBytes, 0, shape.Count() - 1);

            int radius = length / 2;
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    var idx = x + y * length;
                    var tile = shapeBytes[idx];
                    if (tile != 0xFF)
                    {
                        GetCoordinate(loc.X - radius + x, loc.Y - radius + y).SetTile(tile);
                    }
                }
            }
        }

        private static Tile RandomSelectFromListCheckPathChangeAndRemove(Random random, List<Tile> possibleLocations, byte? tile, UltimaData ultimaData, bool requirePath = true)
        {
            Tile loc = null;
            while (loc == null && possibleLocations.Count > 0)
            {
                var randomIdx = random.Next(0, possibleLocations.Count);
                var selection = possibleLocations[randomIdx];

                var path = Search.GetPath(WorldMap.SIZE, WorldMap.SIZE, selection,
                c => { return IsWalkableOrSailable(c) || selection.Equals(c); },
                c => { return ultimaData.Towns.Contains(c); });

                if (path.Count > 0)
                {
                    loc = selection;
                    if (tile != null)
                    {
                        loc.SetTile(tile.Value);
                    }
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
            var loc = GetCoordinate(random.Next(0, WorldMap.SIZE), random.Next(0, WorldMap.SIZE));
            return loc;
        }

        private Tile GetRandomCoordinate(Random random, Func<Tile, bool> criteria, List<Tile> excludes)
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

        private static bool IsMatchingTile(Tile coord, List<byte> validTiles)
        {
            return validTiles.Contains(coord.GetTile());
        }

        private static bool IsWalkable(Tile coord)
        {
            return (coord.GetTile() >= TileInfo.Swamp && coord.GetTile() <= TileInfo.Hills) || (coord.GetTile() >= TileInfo.Dungeon_Entrance && coord.GetTile() <= TileInfo.Village);
        }

        private static bool IsWalkableOrSailable(Tile coord)
        {
            return (coord.GetTile() >= TileInfo.Swamp && coord.GetTile() <= TileInfo.Hills) || (coord.GetTile() >= TileInfo.Dungeon_Entrance && coord.GetTile() <= TileInfo.Village) || coord.GetTile() == TileInfo.Deep_Water || coord.GetTile() == TileInfo.Medium_Water;
        }

        private static bool IsGrassOrSailable(Tile coord)
        {
            return coord.GetTile() == TileInfo.Grasslands || coord.GetTile() == TileInfo.Deep_Water || coord.GetTile() == TileInfo.Medium_Water;
        }

        private static bool IsGrass(Tile coord)
        {
            return coord.GetTile() == TileInfo.Grasslands;
        }

        public static byte Wrap(int input)
        {
            return Wrap(input, SIZE);
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
