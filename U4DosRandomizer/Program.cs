using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace U4DosRandomizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var ultimaData = new UltimaData();

            var seed = 9726547;
            var random = new Random(seed);
            var worldMapDS = new DiamondSquare(256, 184643518.256878, 82759876).getData(9726547);
            var worldMapUlt = MapDiamondSquareToUltimaTiles(worldMapDS);
            // Original game only had single tiles in very special circumstances
            worldMapUlt = RemoveSingleTiles(worldMapUlt);
            worldMapUlt = AddRivers(worldMapUlt, worldMapDS, ultimaData, random);
            worldMapUlt = AddBridges(worldMapUlt);
            worldMapUlt = AddLava(worldMapUlt);
            var avatar = LoadAvatar();

            //Completely random location placements of buildings still. Just trying to make sure I'm editing the files correctly right now. Not looking for a cohesive map that makes sense.
            RandomizeLocations(ultimaData, random);

            WorldMap.MoveBuildings(worldMapUlt, ultimaData);

            Avatar.MoveMoongates(worldMapUlt, avatar);
            Avatar.PlaceAllItems(avatar);
            Avatar.MoveBuildings(avatar, ultimaData);
            WriteMapToOriginalFormat(worldMapUlt);
            WriteToAvatar(avatar);
            var image = ToBitmap(worldMapUlt);
            //FileStream stream = new FileStream("worldMap.bmp", FileMode.Create);
            image.Save("worldMap.bmp");

            //PrintWorldMapInfo();
        }

        private static byte[,] AddLava(byte[,] worldMapUlt)
        {
            return worldMapUlt;
        }

        private static byte[,] AddBridges(byte[,] worldMapUlt)
        {
            return worldMapUlt;
        }

        private static byte[,] AddRivers(byte[,] worldMapUlt, double[,] worldMapDS, UltimaData data, Random random)
        {
            // There are ~32 in the original Ultima map
            int totalNumOfRivers = 7 + random.Next(1, 3) + random.Next(1, 3);
            var surroundingPoints = new List<Point>();

            for(int dist = 1; dist <= 3; dist++)
            {
                surroundingPoints.Add(new Point(1 * dist, 0));
                surroundingPoints.Add(new Point(-1 * dist, 0));
                surroundingPoints.Add(new Point(0, 1 * dist));
                surroundingPoints.Add(new Point(0, -1 * dist));
            }


            // Pick a random spot that isn't water
            // Head uphill from there until you reach a high point
            // Go down hill from all the highpoints marking the path for a river

            var highPoints = new HashSet<Point>();
            for (int riverNum = 0; riverNum < totalNumOfRivers; riverNum++)
            {
                var randomPoint = FindRandomPointHigherThan(4, worldMapUlt, random);

                // Track previous point so I can step back one when I find the highest point
                var prevPoint = randomPoint;
                var currPoint = randomPoint;
                Point? foundHighPoint = null;
                while (foundHighPoint == null)
                {
                    //worldMapUlt[prevPoint.X, prevPoint.Y] = 0xA1;
                    var highestDirection = surroundingPoints.OrderByDescending(p => worldMapDS[Wrap(currPoint.X + (p.X)), Wrap(currPoint.Y + (p.Y))]).First();

                    if (worldMapDS[currPoint.X, currPoint.Y] < worldMapDS[Wrap(currPoint.X + (highestDirection.X)), Wrap(currPoint.Y + (highestDirection.Y))])
                    {
                        prevPoint = currPoint;
                        //int distance = Convert.ToInt32(Math.Sqrt(highestDirection.Item1 * highestDirection.Item1 + highestDirection.Item2 * highestDirection.Item2));
                        int distance = Math.Abs(highestDirection.X != 0 ? highestDirection.X : highestDirection.Y);
                        currPoint = new Point(Wrap(currPoint.X + highestDirection.X / distance), Wrap(currPoint.Y + highestDirection.Y / distance));
                    }
                    else
                    {
                        foundHighPoint = currPoint;
                    }
                }

                //worldMapUlt[prevPoint.X, prevPoint.Y] = 76;
                highPoints.Add(prevPoint);
            }

            var paths = new List<List<Point>>();
            foreach(var highPoint in highPoints)
            { 
                List<Point> pathToWater = new List<Point>();
                pathToWater.Add(highPoint);
                var currPoint = highPoint;
                var foundWater = false;
                Point lastDirection = new Point(0, 1);
                while(!foundWater)
                {
                    var lowestDirection = surroundingPoints.OrderBy(p => worldMapDS[Wrap(currPoint.X + (p.X)), Wrap(currPoint.Y + (p.Y))]).First();
                    var lowestPoint = new Point(Wrap(currPoint.X + (lowestDirection.X)), Wrap(currPoint.Y + (lowestDirection.Y)));

                    if (//worldMapDS[currPoint.X, currPoint.Y] >= worldMapDS[lowestPoint.X, lowestPoint.Y] &&
                        !pathToWater.Contains(lowestPoint))
                    {
                        lastDirection = lowestDirection;
                    }
                    // Else keep moving in the same direction as last time

                    int distance = Math.Abs(lastDirection.X != 0 ? lastDirection.X : lastDirection.Y);
                    currPoint = new Point(Wrap(currPoint.X + lastDirection.X / distance), Wrap(currPoint.Y + lastDirection.Y / distance));
                    pathToWater.Add(currPoint);

                    if (worldMapUlt[currPoint.X, currPoint.Y] < 3)
                    {
                        foundWater = true;
                    }

                    worldMapUlt[currPoint.X, currPoint.Y] = 0x70;
                }

                paths.Add(pathToWater);
            }

            foreach(var path in paths)
            {
                //Choose ransom spot along the path for the headwater
                //var start = random.Next(0, path.Count);
                // That is weighted towards the start of the path
                var max = path.Count-10;
                var min = 10;
                var start = Convert.ToInt32(Math.Floor(Math.Abs(random.NextDouble() - random.NextDouble()) * (1 + max - min) + min));
                for(int i = start; i < path.Count; i++)
                {
                    worldMapUlt[path[i].X, path[i].Y] = 0x02;
                }
            }

            return worldMapUlt;
        }

        private static Point FindRandomPointHigherThan(int tile, byte[,] worldMapUlt, Random random)
        {
            Point? result = null;
            while(result == null)
            {
                var x = random.Next(0, 256);
                var y = random.Next(0, 256);

                if(worldMapUlt[x,y] > tile)
                {
                    result = new Point(x, y);
                }
            }

            return result.Value;
        }

        private static void RandomizeLocations(UltimaData ultimaData, Random random)
        {
            // LCB
            Location lcb = RandomizeLocation(random, 14);
            ultimaData.LCB.Add(lcb);
            Location loc = new Location();
            loc.X = Convert.ToByte(lcb.X + 1);
            loc.Y = Convert.ToByte(lcb.Y);
            loc.Tile = 13;
            ultimaData.LCB.Add(loc);
            loc = new Location();
            loc.X = Convert.ToByte(lcb.X + 2);
            loc.Y = Convert.ToByte(lcb.Y);
            loc.Tile = 15;
            ultimaData.LCB.Add(loc);

            // Castles
            loc = RandomizeLocation(random, 11);
            ultimaData.Castles.Add(loc);
            loc = RandomizeLocation(random, 11);
            ultimaData.Castles.Add(loc);
            loc = RandomizeLocation(random, 11);
            ultimaData.Castles.Add(loc);

            // Towns
            loc = RandomizeLocation(random, 10);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10);
            ultimaData.Towns.Add(loc);

            loc = RandomizeLocation(random, 12);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 12);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 12);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 12);
            ultimaData.Towns.Add(loc);
        }

        private static Location RandomizeLocation(Random random, byte tile)
        {
            var loc = new Location();
            loc.X = Convert.ToByte(random.Next(0, 256));
            loc.Y = Convert.ToByte(random.Next(0, 256));
            loc.Tile = tile;
            return loc;
        }

        private static void WriteToAvatar(byte[] avatar)
        {
            var avatarOut = new System.IO.BinaryWriter(new System.IO.FileStream("AVATAR.EXE", System.IO.FileMode.OpenOrCreate));

            avatarOut.Write(avatar);

            avatarOut.Close();
        }

        private static byte[] LoadAvatar()
        {
            var avatar = new System.IO.FileStream("ULT\\AVATAR.EXE", System.IO.FileMode.Open);
            var avatarBytes = avatar.ReadAllBytes();
            return avatarBytes;
        }

        private static byte[,] RemoveSingleTiles(byte[,] worldMapUlt)
        {
            for(int x = 0; x < 256; x++)
            {
                for(int y = 0; y < 256; y++)
                {
                    var adjacentTiles = new byte[] { worldMapUlt[Wrap(x + 1), y], worldMapUlt[Wrap(x - 1), y], worldMapUlt[x, Wrap(y + 1)], worldMapUlt[x, Wrap(y - 1)] };
                    if(!adjacentTiles.Contains(worldMapUlt[x,y]))
                    {
                        var mostUsedAdjacentTile = (from item in adjacentTiles
                                     group item by item into g
                                     orderby g.Count() descending
                                     select g.Key).First();
                        worldMapUlt[x, y] = mostUsedAdjacentTile;
                    }
                }
            }

            return worldMapUlt;
        }

        public static int Wrap(int input, int divisor = 256)
        {
            return (input % divisor + divisor) % divisor;
        }

        private static void WriteMapToOriginalFormat(byte[,] worldMapUlt)
        {
            int chunkwidth = 32;
            int chunkSize = chunkwidth * chunkwidth;
            byte[] chunk = new byte[chunkSize];
            var worldMap = new System.IO.BinaryWriter(new System.IO.FileStream("WORLD.MAP", System.IO.FileMode.OpenOrCreate));

            for (int chunkCount = 0; chunkCount < 64; chunkCount++)
            {
                // Copy the chunk over
                for (int i = 0; i < chunkSize; i++)
                {
                    chunk[i] = worldMapUlt[i % chunkwidth + chunkCount % 8 * chunkwidth, i / chunkwidth + chunkCount / 8 * chunkwidth];
                }

                worldMap.Write(chunk);
            }

            worldMap.Close();
        }

        private static byte[,] MapDiamondSquareToUltimaTiles(double[,] worldMapDS)
        {
            var worldMapFlattened = new double[256 * 256];

            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    worldMapFlattened[x + y * 256] = worldMapDS[x, y];
                    //int res = Convert.ToInt32((worldMapDS[x, y] / double.MaxValue)*255);
                    //Color newColor = Color.FromArgb(res, res, res);
                    //image.SetPixel(x, y, newColor);
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

            var worldMapUlt = new byte[256,256];
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    // Smush it down to the number of tile types we want
                    //int res = Convert.ToInt32(Linear(worldMapDS[x, y], min, max, 0, percentInMap.Count));
                    byte res = 99;
                    foreach (var range in rangeList)
                    {
                        var value = worldMapDS[x, y];
                        if (worldMapDS[x, y] > range.Item2 && worldMapDS[x, y] <= range.Item3)
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

        private static Bitmap ToBitmap(byte[,] worldMapUlt)
        {
            var image = new Bitmap(256, 256);
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    if( colorMap.ContainsKey(worldMapUlt[x,y]) )
                    {
                        image.SetPixel(x, y, colorMap[worldMapUlt[x, y]]);
                    }
                    else
                    {
                        image.SetPixel(x, y, Color.White);
                    }
                    
                }
            }


            return image;
        }

        static public double Linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
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
