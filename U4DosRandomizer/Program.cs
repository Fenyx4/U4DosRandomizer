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

            //var seed = 9726547;
            var seed = 1033542421;
            //var seed = Environment.TickCount;
            System.IO.File.WriteAllText(@"seed.txt", seed.ToString());
            Console.WriteLine("Seed: " + seed);
            var random = new Random(seed);
            var worldMapDS = new DiamondSquare(WorldMap.SIZE, 184643518.256878, 82759876).getData(random);
            var worldMap = new WorldMap(worldMapDS);
            worldMap.CleanupAndAddFeatures(random);
            
            var avatar = new Avatar();
            avatar.Load(ultimaData);

            var talk = new Talk();
            talk.Load();

            //TODO Randomize mantras o_0           
            

            //Completely random location placements of buildings still. Just trying to make sure I'm editing the files correctly right now. Not looking for a cohesive map that makes sense.
            RandomizeLocations(ultimaData, worldMap, random);
            // TODO: Change starting locations for new characters to match towns

            Console.WriteLine(Talk.GetSextantText(ultimaData.LCB[0]));

            ultimaData.Items[Avatar.ITEM_BELL].X = ultimaData.LCB[0].X;
            ultimaData.Items[Avatar.ITEM_BELL].Y = Convert.ToByte(ultimaData.LCB[0].Y+1);

            //WorldMap.MoveBuildings(worldMapUlt, ultimaData);

            avatar.Update(ultimaData);
            avatar.Save();
            
            var worldFile = new System.IO.BinaryWriter(new System.IO.FileStream("WORLD.MAP", System.IO.FileMode.OpenOrCreate));
            worldMap.WriteMapToOriginalFormat(worldFile);
            worldFile.Close();

            talk.Update(ultimaData);
            talk.Save();

            var image = worldMap.ToBitmap();
            //FileStream stream = new FileStream("worldMap.bmp", FileMode.Create);
            image.Save("worldMap.bmp");

            //PrintWorldMapInfo();
        }


        private static void RandomizeLocations(UltimaData ultimaData, WorldMap worldMap, Random random)
        {
            // LCB
            var placed = false;
            while (!placed)
            {
                var lcb = GetRandomCoordinate(random, worldMap);
                var lcbEntrance = worldMap.GetCoordinate(lcb.X, lcb.Y + 1);

                if (IsWalkableGround(lcb) && IsWalkableGround(lcbEntrance))
                {
                    lcb.SetTile(14);
                    ultimaData.LCB.Add(lcb);
                    Tile lcbSide = worldMap.GetCoordinate(lcb.X - 1, lcb.Y);
                    lcbSide.SetTile(13);
                    ultimaData.LCB.Add(lcbSide);
                    lcbSide = worldMap.GetCoordinate(lcb.X + 1, lcb.Y);
                    lcbSide.SetTile(15);
                    ultimaData.LCB.Add(lcbSide);
                    
                    placed = true;
                }
            }

            // Castles
            var loc = RandomizeLocation(random, 11, worldMap, IsWalkableGround );
            ultimaData.Castles.Add(loc);
            loc = RandomizeLocation(random, 11, worldMap, IsWalkableGround);
            ultimaData.Castles.Add(loc);
            loc = RandomizeLocation(random, 11, worldMap, IsWalkableGround);
            ultimaData.Castles.Add(loc);

            // Towns
            loc = RandomizeLocation(random, 10, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 10, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 29, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);

            // Villages
            loc = RandomizeLocation(random, 12, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 12, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 12, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);
            loc = RandomizeLocation(random, 12, worldMap, IsWalkableGround);
            ultimaData.Towns.Add(loc);

            // Shrines
            loc = RandomizeLocation(random, 30, worldMap, IsWalkableGround);
            ultimaData.Shrines.Add(loc);
            loc = RandomizeLocation(random, 30, worldMap, IsWalkableGround);
            ultimaData.Shrines.Add(loc);
            loc = RandomizeLocation(random, 30, worldMap, IsWalkableGround);
            ultimaData.Shrines.Add(loc);
            loc = RandomizeLocation(random, 30, worldMap, IsWalkableGround);
            ultimaData.Shrines.Add(loc);
            loc = RandomizeLocation(random, 30, worldMap, IsWalkableGround);
            ultimaData.Shrines.Add(loc);
            loc = RandomizeLocation(random, 30, worldMap, IsWalkableGround);
            ultimaData.Shrines.Add(loc);
            loc = RandomizeLocation(random, 30, worldMap, IsWalkableGround);
            ultimaData.Shrines.Add(loc);
            loc = RandomizeLocation(random, 30, worldMap, IsWalkableGround);
            ultimaData.Shrines.Add(loc);
            // TODO: Shrine of humility hordes of daemons

            // Moongates
            loc = RandomizeLocation(random, 4, worldMap, IsGrass);
            ultimaData.Moongates.Add(loc);
            loc = RandomizeLocation(random, 4, worldMap, IsGrass);
            ultimaData.Moongates.Add(loc);
            loc = RandomizeLocation(random, 4, worldMap, IsGrass);
            ultimaData.Moongates.Add(loc);
            loc = RandomizeLocation(random, 4, worldMap, IsGrass);
            ultimaData.Moongates.Add(loc);
            loc = RandomizeLocation(random, 4, worldMap, IsGrass);
            ultimaData.Moongates.Add(loc);
            loc = RandomizeLocation(random, 4, worldMap, IsGrass);
            ultimaData.Moongates.Add(loc);
            loc = RandomizeLocation(random, 4, worldMap, IsGrass);
            ultimaData.Moongates.Add(loc);
            loc = RandomizeLocation(random, 4, worldMap, IsGrass);
            ultimaData.Moongates.Add(loc);

            // Dungeons
            // TODO: Change this to be grab all mountains, then check if you can path to something landable by balloon or ship
            var pattern = new int[1, 2];
            pattern[0, 0] = 8;
            pattern[0, 1] = 7; // TODO add a wildcard. Something for "Walkable" or "Not mountain"
            var validLocations = worldMap.FindAllByPattern(pattern);
            var randomIdx = random.Next(0, validLocations.Count);
            loc = validLocations[randomIdx];
            loc.SetTile(9);
            ultimaData.Dungeons.Add(loc);
            validLocations.RemoveAt(randomIdx);

            randomIdx = random.Next(0, validLocations.Count);
            loc = validLocations[randomIdx];
            loc.SetTile(9);
            ultimaData.Dungeons.Add(loc);
            validLocations.RemoveAt(randomIdx);

            randomIdx = random.Next(0, validLocations.Count);
            loc = validLocations[randomIdx];
            loc.SetTile(9);
            ultimaData.Dungeons.Add(loc);
            validLocations.RemoveAt(randomIdx);

            randomIdx = random.Next(0, validLocations.Count);
            loc = validLocations[randomIdx];
            loc.SetTile(9);
            ultimaData.Dungeons.Add(loc);
            validLocations.RemoveAt(randomIdx);

            randomIdx = random.Next(0, validLocations.Count);
            loc = validLocations[randomIdx];
            loc.SetTile(9);
            ultimaData.Dungeons.Add(loc);
            validLocations.RemoveAt(randomIdx);

            randomIdx = random.Next(0, validLocations.Count);
            loc = validLocations[randomIdx];
            loc.SetTile(9);
            ultimaData.Dungeons.Add(loc);
            validLocations.RemoveAt(randomIdx);

            randomIdx = random.Next(0, validLocations.Count);
            loc = validLocations[randomIdx];
            loc.SetTile(9);
            ultimaData.Dungeons.Add(loc);
            validLocations.RemoveAt(randomIdx);

            // TODO: Something special for Stygian Abyss
            randomIdx = random.Next(0, validLocations.Count);
            loc = validLocations[randomIdx];
            loc.SetTile(9);
            ultimaData.Dungeons.Add(loc);
            validLocations.RemoveAt(randomIdx);
        }

        private static bool IsWalkableGround(Tile coord)
        {
            return coord.GetTile() >= 3 && coord.GetTile() <= 7;
        }

        private static bool IsGrass(Tile coord)
        {
            return coord.GetTile() == 4;
        }

        private static Tile GetRandomCoordinate(Random random, WorldMap worldMap)
        {
            var loc = worldMap.GetCoordinate(random.Next(0, WorldMap.SIZE), random.Next(0, WorldMap.SIZE));
            return loc;
        }

        private static Tile RandomizeLocation(Random random, byte tile, WorldMap worldMap)
        {
            var loc = GetRandomCoordinate(random, worldMap);
            loc.SetTile(tile);
            return loc;
        }

        private static Tile RandomizeLocation(Random random, byte tile, WorldMap worldMap, Func<Tile, bool> criteria)
        {
            while (true)
            {
                var loc = GetRandomCoordinate(random, worldMap);
                if (criteria(loc))
                {
                    loc.SetTile(tile);
                    return loc;
                }
            }
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
