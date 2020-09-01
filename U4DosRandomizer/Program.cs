using Microsoft.Extensions.CommandLineUtils;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace U4DosRandomizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
            CommandOption seedArg = commandLineApplication.Option(
                "-s |--s <seed>",
                "The seed for the randomizer. "
                + " Same seed will produce the same map.",
                CommandOptionType.SingleValue);
            CommandOption pathArg = commandLineApplication.Option(
                "-p |--p <path>",
                "Path to Ultima 4 installation. "
                + " Leaving blank will assume it is the working directory.",
                CommandOptionType.SingleValue);
            CommandOption restoreArg = commandLineApplication.Option(
                "-r |--r",
                "Restore original Ultima 4 files. ",
                CommandOptionType.NoValue);
            CommandOption minimapArg = commandLineApplication.Option(
                "-m |--m",
                "Output a minimap of the overworld. ",
                CommandOptionType.NoValue);
            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                var seed = Environment.TickCount;
                if (seedArg.HasValue())
                {
                    if (!int.TryParse(seedArg.Value(), out seed))
                    {
                        throw new InvalidCastException("Seed must be a number");
                    }
                }

                var path = Directory.GetCurrentDirectory();
                if (pathArg.HasValue())
                {
                    if(!Directory.Exists(pathArg.Value()))
                    {
                        throw new ArgumentException("Path provided does not exist");
                    }
                    else 
                    {
                        path = pathArg.Value();
                    }
                }
                if(!File.Exists(Path.Combine(path, "WORLD.MAP")))
                {
                    Console.Write("Could not find WORLD.MAP please provide path:  ");
                    path = Console.ReadLine().Trim();

                    if (!Directory.Exists(path))
                    {
                        throw new ArgumentException("Path provided does not exist");
                    }
                }

                if (restoreArg.HasValue())
                {
                    Restore(path);
                }
                else
                {
                    Randomize(seed, path, minimapArg.HasValue());
                    //Console.WriteLine("Seed: " + seed);
                    //var random = new Random(seed);
                    //var worldMap = new WorldMap();
                    //worldMap.SwampTest(random);
                    //worldMap.Save(path);

                    //var image = worldMap.ToImage();
                    //image.SaveAsPng("worldMap.png");
                }

                return 0;
            });
            commandLineApplication.Execute(args);           
        }

        private static void Restore(string path)
        {
            WorldMap.Restore(path);
            Avatar.Restore(path);
            Title.Restore(path);
            Talk.Restore(path);
        }

        private static void Randomize(int seed, string path, bool minimap)
        {
            System.IO.File.AppendAllText(@"seed.txt", seed.ToString() + Environment.NewLine);
            Console.WriteLine("Seed: " + seed);
            var random = new Random(seed);

            var randomValues = new List<int>();
            for(int i = 0; i < 50; i++)
            {
                randomValues.Add(random.Next());
            }


            var ultimaData = new UltimaData();

            var worldMapDS = new DiamondSquare(WorldMap.SIZE, 184643518.256878, randomValues[0]).getData(new Random(randomValues[1]));
            var worldMap = new WorldMap();
            worldMap.Load(path, worldMapDS, new Random(randomValues[2]));

            var avatar = new Avatar();
            avatar.Load(path, ultimaData);

            var title = new Title();
            title.Load(path, ultimaData);

            var talk = new Talk();
            talk.Load(path);

            //TODO Randomize mantras o_0           


            //Completely random location placements of buildings still. Just trying to make sure I'm editing the files correctly right now. Not looking for a cohesive map that makes sense.
            var exclude = RandomizeLocations(ultimaData, worldMap, new Random(randomValues[3]));

            //Console.WriteLine(Talk.GetSextantText(ultimaData.LCB[0]));

            RandomizeItems(ultimaData, worldMap, new Random(randomValues[4]), exclude);

            title.Update(ultimaData);
            talk.Update(ultimaData);
            avatar.Update(ultimaData);

            title.Save(path);
            talk.Save(path);
            avatar.Save(path);
            worldMap.Save(path);

            if (minimap)
            {
                var image = worldMap.ToImage();
                image.SaveAsPng($"worldMap-{seed}.png");
            }

            //PrintWorldMapInfo();
        }

        private static void RandomizeItems(UltimaData ultimaData, WorldMap worldMap, Random random, List<Tile> exclude)
        {
            var loc = RandomizeLocation(random, TileInfo.Swamp, worldMap, WorldMap.IsWalkableGround, exclude);
            ultimaData.Items[Avatar.ITEM_MANDRAKE].X = loc.X;
            ultimaData.Items[Avatar.ITEM_MANDRAKE].Y = loc.Y;

            loc = RandomizeLocation(random, TileInfo.Swamp, worldMap, (x) => x.GetTile() == TileInfo.Forest, exclude);
            ultimaData.Items[Avatar.ITEM_NIGHTSHADE].X = loc.X;
            ultimaData.Items[Avatar.ITEM_NIGHTSHADE].Y = loc.Y;

            var possibleLocations = worldMap.GetAllMatchingTiles(c => AreaIsAll(worldMap, TileInfo.Deep_Water, 14, c) && !exclude.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[Avatar.ITEM_SKULL].X = loc.X;
            ultimaData.Items[Avatar.ITEM_SKULL].Y = loc.Y;
            ApplyShape(worldMap, loc, "skull");

            possibleLocations = worldMap.GetAllMatchingTiles(c => AreaIsAll(worldMap, TileInfo.Deep_Water, 7, c) && !exclude.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[Avatar.ITEM_BELL].X = loc.X;
            ultimaData.Items[Avatar.ITEM_BELL].Y = loc.Y;
            ApplyShape(worldMap, loc, "bell");

            loc = GetRandomCoordinate(random, worldMap, WorldMap.IsWalkableGround, exclude);
            ultimaData.Items[Avatar.ITEM_HORN].X = loc.X;
            ultimaData.Items[Avatar.ITEM_HORN].Y = loc.Y;

            possibleLocations = worldMap.GetAllMatchingTiles(c => c.GetTile() == TileInfo.Deep_Water && !exclude.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[Avatar.ITEM_WHEEL].X = loc.X;
            ultimaData.Items[Avatar.ITEM_WHEEL].Y = loc.Y;

            // TODO: Do I move the black stone?
            ultimaData.Items[Avatar.ITEM_WHEEL].X = ultimaData.Moongates[0].X;
            ultimaData.Items[Avatar.ITEM_BLACK_STONE].Y = ultimaData.Moongates[0].Y;

            // White stone
            possibleLocations = worldMap.GetAllMatchingTiles(c => AreaIsAll(worldMap, TileInfo.Mountains, 4, c) && !exclude.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            ultimaData.Items[Avatar.ITEM_WHITE_STONE].X = Convert.ToByte(loc.X-1);
            ultimaData.Items[Avatar.ITEM_WHITE_STONE].Y = loc.Y;
            ApplyShape(worldMap, loc, "white");
        }

        private static void ApplyShape(WorldMap worldMap, ICoordinate loc, string file)
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
                        worldMap.GetCoordinate(loc.X - radius + x, loc.Y - radius + y).SetTile(tile);
                    }
                }
            }
        }

        private static bool AreaIsAll(WorldMap worldMap, int tile, int length, ICoordinate coordinate)
        {
            int radius = length / 2;

            var result = true;
            for(int x = 0; x < length; x++)
            {
                for(int y = 0; y < length; y++)
                {
                    result = result && worldMap.IsTile(coordinate.X - radius + x, coordinate.Y - radius + y, tile);
                }
            }

            return result;
        }

        private static bool GoodForHumility(WorldMap worldMap, ICoordinate coordinate)
        {
            var result = true;
            for (int y = -4; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    result = result && worldMap.IsTile(coordinate.X + x, coordinate.Y + y, TileInfo.Mountains);
                }
            }

            var entrance = worldMap.GetCoordinate(coordinate.X, coordinate.Y - 5);

            result = result && IsWalkable(entrance);

            if (result)
            {
                // Make sure we can reach it by boat or balloon
                var path = Search.GetPath(256, 256, entrance, IsGrassOrSailable, IsWalkableOrSailable, null);

                result = result && path.Count > 0;
            }

            return result;
        }

        private static List<Tile> RandomizeLocations(UltimaData ultimaData, WorldMap worldMap, Random random)
        {
            var excludeLocations = new List<Tile>();
            // Lay down Stygian Abyss first so it doesn't stomp on other things
            // TODO: Make the entrance to the Abyss more random instead of laying down what is in the base game
            // Find a reasonable mountainous area
            var possibleLocations = worldMap.GetAllMatchingTiles(c => AreaIsAll(worldMap, TileInfo.Mountains, 3, c));
            var stygian = possibleLocations[random.Next(0, possibleLocations.Count)];
            // Get a path from the entrance to water
            var entranceToStygian = worldMap.GetCoordinate(stygian.X - 14, stygian.Y - 9);
            //var entrancePathToWater = worldMap.GetRiverPath(entranceToStygian, c => { return c.GetTile() == TileInfo.Deep_Water; } );

            var shapeLoc = new Coordinate(stygian.X - 2, stygian.Y - 7);
            ApplyShape(worldMap, shapeLoc, "abyss");

            var entrancePathToWater = Search.GetPath(WorldMap.SIZE, WorldMap.SIZE, entranceToStygian,
                c => { return c.GetTile() == TileInfo.Deep_Water; }, // Find deep water to help make sure a boat can reach here. TODO: Make sure it reaches the ocean.
                c => { return !(WorldMap.Between(c.X, WorldMap.Wrap(shapeLoc.X - 12), WorldMap.Wrap(shapeLoc.X + 12)) && WorldMap.Between(c.Y, WorldMap.Wrap(shapeLoc.Y - 12), WorldMap.Wrap(shapeLoc.Y + 12))); },
                worldMap.GoDownhillHueristic);

            for (int i = 0; i < entrancePathToWater.Count; i++)
            {
                worldMap.GetCoordinate(entrancePathToWater[i].X, entrancePathToWater[i].Y).SetTile(TileInfo.Medium_Water);
            }

            for (int x = -12; x <= 12; x++)
            {
                for (int y = -12; y <= 12; y++)
                {
                    excludeLocations.Add(worldMap.GetCoordinate(shapeLoc.X + x, shapeLoc.Y + y));
                }
            }

            //Pirate Cove - Set locations based off Stygian location
            var originalX = 0xe9; // Original Stygian location
            var originalY = 0xe9;
            for (var i = 0; i < ultimaData.PirateCove.Count; i++)
            {
                ultimaData.PirateCove[i].X = Convert.ToByte(WorldMap.Wrap(ultimaData.PirateCove[i].X - originalX + stygian.X));
                ultimaData.PirateCove[i].Y = Convert.ToByte(WorldMap.Wrap(ultimaData.PirateCove[i].Y - originalY + stygian.Y));
            }
            ultimaData.PirateCoveSpawnTrigger = new Coordinate(ultimaData.PirateCoveSpawnTrigger.X - originalX + stygian.X, ultimaData.PirateCoveSpawnTrigger.Y - originalY + stygian.Y);
            //worldMap.GetCoordinate(ultimaData.PirateCoveSpawnTrigger.X, ultimaData.PirateCoveSpawnTrigger.Y).SetTile(TileInfo.A);

            // Buildings
            possibleLocations = worldMap.GetAllMatchingTiles(WorldMap.IsWalkableGround);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            // Castles
            Tile loc = null;
            for (int i = 0; i < 3; i++)
            {
                loc = RandomSelectFromListChangeAndRemove(random, possibleLocations, TileInfo.Castle);
                ultimaData.Castles.Add(loc);
            }

            // Towns
            for(int i = 0; i < 7; i++)
            {
                loc = RandomSelectFromListChangeAndRemove(random, possibleLocations, TileInfo.Town);
                ultimaData.Towns.Add(loc);

            }
            loc = RandomSelectFromListChangeAndRemove(random, possibleLocations, TileInfo.Ruins); // Magincia
            ultimaData.Towns.Add(loc);

            // Villages
            for (int i = 0; i < 4; i++)
            {
                loc = RandomSelectFromListChangeAndRemove(random, possibleLocations, TileInfo.Village);
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
                    loc = RandomSelectFromListChangeAndRemove(random, possibleLocations, TileInfo.Shrine);
                    ultimaData.Shrines.Add(loc);
                }
            }
            // Humility
            // TODO: Shrine prettier
            possibleLocations = worldMap.GetAllMatchingTiles(c => GoodForHumility(worldMap, c));
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            loc = possibleLocations[random.Next(0, possibleLocations.Count)];
            loc.SetTile(TileInfo.Shrine);
            ultimaData.Shrines.Add(loc);
            for (int y = -4; y < 0; y++)
            {
                worldMap.GetCoordinate(loc.X, loc.Y + y).SetTile(TileInfo.Hills);
            }
            ultimaData.DaemonSpawnLocationX = loc.X;
            ultimaData.DaemonSpawnX1 = WorldMap.Wrap(loc.X - 1);
            ultimaData.DaemonSpawnX2 = WorldMap.Wrap(loc.X + 1);
            ultimaData.DaemonSpawnY1 = WorldMap.Wrap(loc.Y - 4);
            ultimaData.DaemonSpawnY2 = WorldMap.Wrap(loc.Y + 1);

            // Moongates
            List<Tile> path = new List<Tile>();
            List<byte> validTiles = new List<byte>() { TileInfo.Grasslands, TileInfo.Scrubland, TileInfo.Swamp, TileInfo.Forest, TileInfo.Hills };
            for (int i = 0; i < 8; i++)
            {
                path = new List<Tile>();
                var distance = random.Next(5, 10);
                while (path.Count == 0 && distance > 0)
                {
                    path = Search.GetPath(WorldMap.SIZE, WorldMap.SIZE, ultimaData.Towns[i],
                        // Move at least 9 spaces away from from the entrance
                        c => { return distance * distance <= WorldMap.DistanceSquared(c, ultimaData.Towns[i]) && IsWalkable(c); },
                        // Only valid if all neighbors all also mountains
                        c => { return IsMatchingTile(c,validTiles); },
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
                        foreach(var n in loc.NeighborCoordinates())
                        {
                            if(validTiles.Contains(n.GetTile()))
                            {
                                n.SetTile(TileInfo.Grasslands);
                            }
                        }
                        possibleLocations.Remove(loc);
                        ultimaData.Moongates.Add(loc);
                    }
                }
                if(distance == 0)
                {
                    Console.WriteLine($"Utterly failed at Moongate placement of {i} placement. Trying random.");
                    possibleLocations = worldMap.GetAllMatchingTiles(IsGrass);
                    possibleLocations.RemoveAll(c => excludeLocations.Contains(c));

                    loc = RandomSelectFromListChangeAndRemove(random, possibleLocations, TileInfo.Grasslands);
                    ultimaData.Moongates.Add(loc);
                }
            }

            // LCB
            var placed = false;
            while (!placed)
            {
                var lcb = GetRandomCoordinate(random, worldMap);
                var lcbEntrance = worldMap.GetCoordinate(lcb.X, lcb.Y + 1);
                Tile lcbWestSide = worldMap.GetCoordinate(lcb.X - 1, lcb.Y);
                Tile lcbEastSide = worldMap.GetCoordinate(lcb.X + 1, lcb.Y);

                path = new List<Tile>();
                if (WorldMap.IsWalkableGround(lcb) && WorldMap.IsWalkableGround(lcbEntrance) && !excludeLocations.Contains(lcb))
                {
                    path = Search.GetPath(WorldMap.SIZE, WorldMap.SIZE, lcbEntrance,
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
            possibleLocations = worldMap.GetAllMatchingTiles(c => c.GetTile() == TileInfo.Mountains && WorldMap.IsWalkableGround(worldMap.GetCoordinate(c.X, c.Y+1)) && Search.GetPath(WorldMap.SIZE, WorldMap.SIZE, c,
            coord => { return IsGrass(coord) || coord.GetTile() == TileInfo.Deep_Water; },
            IsWalkableOrSailable).Count > 0);
            possibleLocations.RemoveAll(c => excludeLocations.Contains(c));
            for (int i = 0; i < 6; i++)
            {
                loc = RandomSelectFromListChangeAndRemove(random, possibleLocations, 9);
                ultimaData.Dungeons.Add(loc);
            }

            // special for Hythloth
            // TODO: Hythloth prettier
            possibleLocations = worldMap.GetAllMatchingTiles(c => AreaIsAll(worldMap, TileInfo.Mountains, 4, c));

            path = new List<Tile>();
            while (path.Count == 0)
            {
                loc = possibleLocations[random.Next(0, possibleLocations.Count)];
                possibleLocations.Remove(loc);
                path = Search.GetPath(WorldMap.SIZE, WorldMap.SIZE, loc,
                    // Move at least 9 spaces away from from the entrance
                    c => { return 9 * 9 <= WorldMap.DistanceSquared(c, loc); },
                    // Only valid if all neighbors all also mountains
                    c => { return c.GetTile() == TileInfo.Mountains && c.NeighborAndAdjacentCoordinates().All(n => n.GetTile() == TileInfo.Mountains); },
                    worldMap.GoDownhillHueristic);
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

            // Move starting positions to Towns
            for (int i = 0; i < 8; i++)
            {
                var validPositions = worldMap.GetPathableTilesNear(ultimaData.Towns[i], 3, IsWalkable);
                loc = validPositions[random.Next(0, validPositions.Count)];
                ultimaData.StartingPositions[i].X = loc.X;
                ultimaData.StartingPositions[i].Y = loc.Y;
            }

            // Whirlpool normally exits in Lock Lake
            // TODO: Put it somewhere more thematic
            // For now stick it in the middle of some deep water somewhere
            loc = GetRandomCoordinate(random, worldMap, c => c.GetTile() == TileInfo.Deep_Water, excludeLocations);
            ultimaData.WhirlpoolExit = new Coordinate(loc.X, loc.Y);

            return excludeLocations;
        }

        private static Tile RandomSelectFromListChangeAndRemove(Random random, List<Tile> possibleLocations, byte tile)
        {
            var randomIdx = random.Next(0, possibleLocations.Count);
            var loc = possibleLocations[randomIdx];
            loc.SetTile(tile);
            possibleLocations.RemoveAt(randomIdx);

            return loc;
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

        private static Tile GetRandomCoordinate(Random random, WorldMap worldMap)
        {
            var loc = worldMap.GetCoordinate(random.Next(0, WorldMap.SIZE), random.Next(0, WorldMap.SIZE));
            return loc;
        }

        private static Tile GetRandomCoordinate(Random random, WorldMap worldMap, Func<Tile, bool> criteria, List<Tile> excludes)
        {
            while (true)
            {
                var loc = GetRandomCoordinate(random, worldMap);
                if (criteria(loc) && !excludes.Contains(loc))
                {
                    return loc;
                }
            }
        }

        private static Tile RandomizeLocation(Random random, byte tile, WorldMap worldMap)
        {
            var loc = GetRandomCoordinate(random, worldMap);
            loc.SetTile(tile);
            return loc;
        }

        private static Tile RandomizeLocation(Random random, byte tile, WorldMap worldMap, Func<Tile, bool> criteria, List<Tile> excludes)
        {
            var loc = GetRandomCoordinate(random, worldMap, criteria, excludes);

            loc.SetTile(tile);
            return loc;
        }
        
        private static void PrintWorldMapInfo()
        {
            FileStream stream = new FileStream("ULT\\WORLD.MAP", FileMode.Open);
            var world = new byte[256 * 256];
            stream.Read(world, 0, 256 * 256);
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

        static public double Linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        static private Dictionary<byte, string> shapeLabels = new Dictionary<byte, string>()
        {
            {0  ,"Deep Water"},
            {1  ,"Medium Water"},
            {2  ,"Shallow Water"},
            {3  ,"Swamp"},
            {4  ,"Grasslands"},
            {5  ,"Scrubland"},
            {6  ,"Forest"},
            {7  ,"Hills"},
            {8  ,"Mountains"},
            {9  ,"Dungeon Entrance"},
            {10 ,"Town"},
            {11 ,"Castle"},
            {12 ,"Village"},
            {13 ,"Lord British's Caste West"},
            {14 ,"Lord British's Castle Entrance"},
            {15 ,"Lord British's Castle East"},
            {16 ,"Ship West"},
            {17 ,"Ship North"},
            {18 ,"Ship East"},
            {19 ,"Ship South"},
            {20 ,"Horse West"},
            {21 ,"Horse East"},
            {22 ,"Tile Floor"},
            {23 ,"Bridge"},
            {24 ,"Balloon"},
            {25 ,"Bridge North"},
            {26 ,"Bridge South"},
            {27 ,"Ladder Up"},
            {28 ,"Ladder Down"},
            {29 ,"Ruins"},
            {30 ,"Shrine"},
            {31 ,"Avatar"},
            {32 ,"Mage 1"},
            {33 ,"Mage 2"},
            {34 ,"Bard 1"},
            {35 ,"Bard 2"},
            {36 ,"Fighter 1"},
            {37 ,"Fighter 2"},
            {38 ,"Druid 1"},
            {39 ,"Druid 2"},
            {40 ,"Tinker 1"},
            {41 ,"Tinker 2"},
            {42 ,"Paladin 1"},
            {43 ,"Paladin 2"},
            {44 ,"Ranger 1"},
            {45 ,"Ranger 2"},
            {46 ,"Shepherd 1"},
            {47 ,"Shepherd 2"},
            {48 ,"Column"},
            {49 ,"White SW"},
            {50 ,"White SE"},
            {51 ,"White NW"},
            {52 ,"White NE"},
            {53 ,"Mast"},
            {54 ,"Ship's Wheel"},
            {55 ,"Rocks"},
            {56 ,"Lyin Down"},
            {57 ,"Stone Wall"},
            {58 ,"Locked Door"},
            {59 ,"Unlocked Door"},
            {60 ,"Chest"},
            {61 ,"Ankh"},
            {62 ,"Brick Floor"},
            {63 ,"Wooden Planks"},
            {64 ,"Moongate 1"},
            {65 ,"Moongate 2"},
            {66 ,"Moongate 3"},
            {67 ,"Moongate 4"},
            {68 ,"Poison Field"},
            {69 ,"Energy Field"},
            {70 ,"Fire Field"},
            {71 ,"Sleep Field"},
            {72 ,"Solid Barrier"},
            {73 ,"Hidden Passage"},
            {74 ,"Altar"},
            {75 ,"Spit"},
            {76 ,"Lava Flow"},
            {77 ,"Missile"},
            {78 ,"Magic Sphere"},
            {79 ,"Attack Flash"},
            {80 ,"Guard 1"},
            {81 ,"Guard 2"},
            {82 ,"Citizen 1"},
            {83 ,"Citizen 2"},
            {84 ,"Singing Bard 1"},
            {85 ,"Singing Bard 2"},
            {86 ,"Jester 1"},
            {87 ,"Jester 2"},
            {88 ,"Beggar 1"},
            {89 ,"Beggar 2"},
            {90 ,"Child 1"},
            {91 ,"Child 2"},
            {92 ,"Bull 1"},
            {93 ,"Bull 2"},
            {94 ,"Lord British 1"},
            {95 ,"Lord British 2"},
            {96 ,"A"},
            {97 ,"B"},
            {98 ,"C"},
            {99 ,"D"},
            {100,   "E"},
            {101,   "F"},
            {102,   "G"},
            {103,   "H"},
            {104,   "I"},
            {105,   "J"},
            {106,   "K"},
            {107,   "L"},
            {108,   "M"},
            {109,   "N"},
            {110,   "O"},
            {111,   "P"},
            {112,   "Q"},
            {113,   "R"},
            {114,   "S"},
            {115,   "T"},
            {116,   "U"},
            {117,   "V"},
            {118,   "W"},
            {119,   "X"},
            {120,   "Y"},
            {121,   "Z"},
            {122,   "Space"},
            {123,   "Right"},
            {124,   "Left"},
            {125,   "Window"},
            {126,   "Blank"},
            {127,   "Brick Wall"},
            {128,   "Pirate Ship West"},
            {129,   "Pirate Ship North"},
            {130,   "Pirate Ship East"},
            {131,   "Pirate Ship South"},
            {132,   "Nixie 1"},
            {133,   "Nixie 2"},
            {134,   "Giant Squid 2"},
            {135,   "Giant Squid 2"},
            {136,   "Sea Serpent 1"},
            {137,   "Sea Serpent 2"},
            {138,   "Seahorse 1"},
            {139,   "Seahorse 2"},
            {140,   "Whirlpool 1"},
            {141,   "Whirlpool 2"},
            {142,   "Storm 1"},
            {143,   "Storm 2"},
            {144,   "Rat 1"},
            {145,   "Rat 2"},
            {146,   "Rat 3"},
            {147,   "Rat 4"},
            {148,   "Bat 1"},
            {149,   "Bat 2"},
            {150,   "Bat 3"},
            {151,   "Bat 4"},
            {152,   "Giant Spider 1"},
            {153,   "Giant Spider 2"},
            {154,   "Giant Spider 3"},
            {155,   "Giant Spider 4"},
            {156,   "Ghost 1"},
            {157,   "Ghost 2"},
            {158,   "Ghost 3"},
            {159,   "Ghost 4"},
            {160,   "Slime 1"},
            {161,   "Slime 2"},
            {162,   "Slime 3"},
            {163,   "Slime 4"},
            {164,   "Troll 1"},
            {165,   "Troll 2"},
            {166,   "Troll 3"},
            {167,   "Troll 4"},
            {168,   "Gremlin 1"},
            {169,   "Gremlin 2"},
            {170,   "Gremlin 3"},
            {171,   "Gremlin 4"},
            {172,   "Mimic 1"},
            {173,   "Mimic 2"},
            {174,   "Mimic 3"},
            {175,   "Mimic 4"},
            {176,   "Reaper 1"},
            {177,   "Reaper 2"},
            {178,   "Reaper 3"},
            {179,   "Reaper 4"},
            {180,   "Insect Swarm 1"},
            {181,   "Insect Swarm 2"},
            {182,   "Insect Swarm 3"},
            {183,   "Insect Swarm 4"},
            {184,   "Gazer 1"},
            {185,   "Gazer 2"},
            {186,   "Gazer 3"},
            {187,   "Gazer 4"},
            {188,   "Phantom 1"},
            {189,   "Phantom 2"},
            {190,   "Phantom 3"},
            {191,   "Phantom 4"},
            {192,   "Orc 1"},
            {193,   "Orc 2"},
            {194,   "Orc 3"},
            {195,   "Orc 4"},
            {196,   "Skeleton 1"},
            {197,   "Skeleton 2"},
            {198,   "Skeleton 3"},
            {199,   "Skeleton 4"},
            {200,   "Rogue 1"},
            {201,   "Rogue 2"},
            {202,   "Rogue 3"},
            {203,   "Rogue 4"},
            {204,   "Python 1"},
            {205,   "Python 2"},
            {206,   "Python 3"},
            {207,   "Python 4"},
            {208,   "Ettin 1"},
            {209,   "Ettin 2"},
            {210,   "Ettin 3"},
            {211,   "Ettin 4"},
            {212,   "Headless 1"},
            {213,   "Headless 2"},
            {214,   "Headless 3"},
            {215,   "Headless 4"},
            {216,   "Cyclops 1"},
            {217,   "Cyclops 2"},
            {218,   "Cyclops 3"},
            {219,   "Cyclops 4"},
            {220,   "Wisp 1"},
            {221,   "Wisp 2"},
            {222,   "Wisp 3"},
            {223,   "Wisp 4"},
            {224,   "Mage 1"},
            {225,   "Mage 2"},
            {226,   "Mage 3"},
            {227,   "Mage 4"},
            {228,   "Lich 1"},
            {229,   "Lich 2"},
            {230,   "Lich 3"},
            {231,   "Lich 4"},
            {232,   "Lava Lizard 1"},
            {233,   "Lava Lizard 2"},
            {234,   "Lava Lizard 3"},
            {235,   "Lava Lizard 4"},
            {236,   "Zorn 1"},
            {237,   "Zorn 2"},
            {238,   "Zorn 3"},
            {239,   "Zorn 4"},
            {240,   "Daemon 1"},
            {241,   "Daemon 2"},
            {242,   "Daemon 3"},
            {243,   "Daemon 4"},
            {244,   "Hydra 1"},
            {245,   "Hydra 2"},
            {246,   "Hydra 3"},
            {247,   "Hydra 4"},
            {248,   "Dragon 1"},
            {249,   "Dragon 2"},
            {250,   "Dragon 3"},
            {251,   "Dragon 2"},
            {252,   "Balron 1"},
            {253,   "Balron 2"},
            {254,   "Balron 3"},
            {255,   "Balron 4"}
        };
    }
}
