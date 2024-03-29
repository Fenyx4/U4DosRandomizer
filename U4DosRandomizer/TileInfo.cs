﻿using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class TileInfo
    {
        public const byte Deep_Water                          = 0;
        public const byte Medium_Water                        = 1;
        public const byte Shallow_Water                       = 2;
        public const byte Swamp                               = 3;
        public const byte Grasslands                          = 4;
        public const byte Scrubland                           = 5;
        public const byte Forest                              = 6;
        public const byte Hills                               = 7;
        public const byte Mountains                           = 8;
        public const byte Dungeon_Entrance                    = 9;
        public const byte Town                                = 10;	
        public const byte Castle                              = 11;	
        public const byte Village                             = 12;	
        public const byte Lord_British_s_Caste_West           = 13;	
        public const byte Lord_British_s_Castle_Entrance      = 14;	
        public const byte Lord_British_s_Castle_East          = 15;	
        public const byte Ship_West                           = 16;	
        public const byte Ship_North                          = 17;	
        public const byte Ship_East                           = 18;	
        public const byte Ship_South                          = 19;	
        public const byte Horse_West                          = 20;	
        public const byte Horse_East                          = 21;	
        public const byte Tile_Floor                          = 22;	
        public const byte Bridge                              = 23;	
        public const byte Balloon                             = 24;	
        public const byte Bridge_North                        = 25;	
        public const byte Bridge_South                        = 26;	
        public const byte Ladder_Up                           = 27;	
        public const byte Ladder_Down                         = 28;	
        public const byte Ruins                               = 29;	
        public const byte Shrine                              = 30;	
        public const byte Avatar                              = 31;	
        public const byte Mage_1                              = 32;	
        public const byte Mage_2                              = 33;	
        public const byte Bard_1                              = 34;	
        public const byte Bard_2                              = 35;	
        public const byte Fighter_1                           = 36;	
        public const byte Fighter_2                           = 37;	
        public const byte Druid_1                             = 38;	
        public const byte Druid_2                             = 39;	
        public const byte Tinker_1                            = 40;	
        public const byte Tinker_2                            = 41;	
        public const byte Paladin_1                           = 42;	
        public const byte Paladin_2                           = 43;	
        public const byte Ranger_1                            = 44;	
        public const byte Ranger_2                            = 45;	
        public const byte Shepherd_1                          = 46;	
        public const byte Shepherd_2                          = 47;	
        public const byte Column                              = 48;	
        public const byte White_SW                            = 49;	
        public const byte White_SE                            = 50;	
        public const byte White_NW                            = 51;	
        public const byte White_NE                            = 52;	
        public const byte Mast                                = 53;	
        public const byte Ship_s_Wheel                        = 54;	
        public const byte Rocks                               = 55;	
        public const byte Lyin_Down                           = 56;	
        public const byte Stone_Wall                          = 57;	
        public const byte Locked_Door                         = 58;	
        public const byte Unlocked_Door                       = 59;	
        public const byte Chest                               = 60;	
        public const byte Ankh                                = 61;	
        public const byte Brick_Floor                         = 62;	
        public const byte Wooden_Planks                       = 63;	
        public const byte Moongate_1                          = 64;	
        public const byte Moongate_2                          = 65;	
        public const byte Moongate_3                          = 66;	
        public const byte Moongate_4                          = 67;	
        public const byte Poison_Field                        = 68;	
        public const byte Energy_Field                        = 69;	
        public const byte Fire_Field                          = 70;	
        public const byte Sleep_Field                         = 71;	
        public const byte Solid_Barrier                       = 72;	
        public const byte Hidden_Passage                      = 73;	
        public const byte Altar                               = 74;	
        public const byte Spit                                = 75;	
        public const byte Lava_Flow                           = 76;	
        public const byte Missile                             = 77;	
        public const byte Magic_Sphere                        = 78;	
        public const byte Attack_Flash                        = 79;	
        public const byte Guard_1                             = 80;	
        public const byte Guard_2                             = 81;	
        public const byte Citizen_1                           = 82;	
        public const byte Citizen_2                           = 83;	
        public const byte Singing_Bard_1                      = 84;	
        public const byte Singing_Bard_2                      = 85;	
        public const byte Jester_1                            = 86;	
        public const byte Jester_2                            = 87;	
        public const byte Beggar_1                            = 88;	
        public const byte Beggar_2                            = 89;	
        public const byte Child_1                             = 90;	
        public const byte Child_2                             = 91;	
        public const byte Bull_1                              = 92;	
        public const byte Bull_2                              = 93;	
        public const byte Lord_British_1                      = 94;	
        public const byte Lord_British_2                      = 95;	
        public const byte A                                   = 96;	
        public const byte B                                   = 97;	
        public const byte C                                   = 98;	
        public const byte D                                   = 99;	
        public const byte E                                   = 100;	
        public const byte F                                   = 101;	
        public const byte G                                   = 102;	
        public const byte H                                   = 103;	
        public const byte I                                   = 104;	
        public const byte J                                   = 105;	
        public const byte K                                   = 106;	
        public const byte L                                   = 107;	
        public const byte M                                   = 108;	
        public const byte N                                   = 109;	
        public const byte O                                   = 110;	
        public const byte P                                   = 111;	
        public const byte Q                                   = 112;	
        public const byte R                                   = 113;	
        public const byte S                                   = 114;	
        public const byte T                                   = 115;	
        public const byte U                                   = 116;	
        public const byte V                                   = 117;	
        public const byte W                                   = 118;	
        public const byte X                                   = 119;	
        public const byte Y                                   = 120;	
        public const byte Z                                   = 121;	
        public const byte Space                               = 122;	
        public const byte Right                               = 123;	
        public const byte Left                                = 124;	
        public const byte Window                              = 125;	
        public const byte Blank                               = 126;	
        public const byte Brick_Wall                          = 127;	
        public const byte Pirate_Ship_West                    = 128;	
        public const byte Pirate_Ship_North                   = 129;	
        public const byte Pirate_Ship_East                    = 130;	
        public const byte Pirate_Ship_South                   = 131;	
        public const byte Nixie_1                             = 132;	
        public const byte Nixie_2                             = 133;	
        public const byte Giant_Squid_1                       = 134;	
        public const byte Giant_Squid_2                       = 135;	
        public const byte Sea_Serpent_1                       = 136;	
        public const byte Sea_Serpent_2                       = 137;	
        public const byte Seahorse_1                          = 138;	
        public const byte Seahorse_2                          = 139;	
        public const byte Whirlpool_1                         = 140;	
        public const byte Whirlpool_2                         = 141;	
        public const byte Storm_1                             = 142;	
        public const byte Storm_2                             = 143;	
        public const byte Rat_1                               = 144;	
        public const byte Rat_2                               = 145;	
        public const byte Rat_3                               = 146;	
        public const byte Rat_4                               = 147;	
        public const byte Bat_1                               = 148;	
        public const byte Bat_2                               = 149;	
        public const byte Bat_3                               = 150;	
        public const byte Bat_4                               = 151;	
        public const byte Giant_Spider_1                      = 152;	
        public const byte Giant_Spider_2                      = 153;	
        public const byte Giant_Spider_3                      = 154;	
        public const byte Giant_Spider_4                      = 155;	
        public const byte Ghost_1                             = 156;	
        public const byte Ghost_2                             = 157;	
        public const byte Ghost_3                             = 158;	
        public const byte Ghost_4                             = 159;	
        public const byte Slime_1                             = 160;	
        public const byte Slime_2                             = 161;	
        public const byte Slime_3                             = 162;	
        public const byte Slime_4                             = 163;	
        public const byte Troll_1                             = 164;	
        public const byte Troll_2                             = 165;	
        public const byte Troll_3                             = 166;	
        public const byte Troll_4                             = 167;	
        public const byte Gremlin_1                           = 168;	
        public const byte Gremlin_2                           = 169;	
        public const byte Gremlin_3                           = 170;	
        public const byte Gremlin_4                           = 171;	
        public const byte Mimic_1                             = 172;	
        public const byte Mimic_2                             = 173;	
        public const byte Mimic_3                             = 174;	
        public const byte Mimic_4                             = 175;	
        public const byte Reaper_1                            = 176;	
        public const byte Reaper_2                            = 177;	
        public const byte Reaper_3                            = 178;	
        public const byte Reaper_4                            = 179;	
        public const byte Insect_Swarm_1                      = 180;	
        public const byte Insect_Swarm_2                      = 181;	
        public const byte Insect_Swarm_3                      = 182;	
        public const byte Insect_Swarm_4                      = 183;	
        public const byte Gazer_1                             = 184;	
        public const byte Gazer_2                             = 185;	
        public const byte Gazer_3                             = 186;	
        public const byte Gazer_4                             = 187;	
        public const byte Phantom_1                           = 188;	
        public const byte Phantom_2                           = 189;	
        public const byte Phantom_3                           = 190;	
        public const byte Phantom_4                           = 191;	
        public const byte Orc_1                               = 192;	
        public const byte Orc_2                               = 193;	
        public const byte Orc_3                               = 194;	
        public const byte Orc_4                               = 195;	
        public const byte Skeleton_1                          = 196;	
        public const byte Skeleton_2                          = 197;	
        public const byte Skeleton_3                          = 198;	
        public const byte Skeleton_4                          = 199;	
        public const byte Rogue_1                             = 200;	
        public const byte Rogue_2                             = 201;	
        public const byte Rogue_3                             = 202;	
        public const byte Rogue_4                             = 203;	
        public const byte Python_1                            = 204;	
        public const byte Python_2                            = 205;	
        public const byte Python_3                            = 206;	
        public const byte Python_4                            = 207;	
        public const byte Ettin_1                             = 208;	
        public const byte Ettin_2                             = 209;	
        public const byte Ettin_3                             = 210;	
        public const byte Ettin_4                             = 211;	
        public const byte Headless_1                          = 212;	
        public const byte Headless_2                          = 213;	
        public const byte Headless_3                          = 214;	
        public const byte Headless_4                          = 215;	
        public const byte Cyclops_1                           = 216;	
        public const byte Cyclops_2                           = 217;	
        public const byte Cyclops_3                           = 218;	
        public const byte Cyclops_4                           = 219;	
        public const byte Wisp_1                              = 220;	
        public const byte Wisp_2                              = 221;	
        public const byte Wisp_3                              = 222;	
        public const byte Wisp_4                              = 223;	
        public const byte Evil_Mage_1                         = 224;	
        public const byte Evil_Mage_2                         = 225;	
        public const byte Evil_Mage_3                         = 226;	
        public const byte Evil_Mage_4                         = 227;	
        public const byte Lich_1                              = 228;	
        public const byte Lich_2                              = 229;	
        public const byte Lich_3                              = 230;	
        public const byte Lich_4                              = 231;	
        public const byte Lava_Lizard_1                       = 232;	
        public const byte Lava_Lizard_2                       = 233;	
        public const byte Lava_Lizard_3                       = 234;	
        public const byte Lava_Lizard_4                       = 235;	
        public const byte Zorn_1                              = 236;	
        public const byte Zorn_2                              = 237;	
        public const byte Zorn_3                              = 238;	
        public const byte Zorn_4                              = 239;	
        public const byte Daemon_1                            = 240;	
        public const byte Daemon_2                            = 241;	
        public const byte Daemon_3                            = 242;	
        public const byte Daemon_4                            = 243;	
        public const byte Hydra_1                             = 244;	
        public const byte Hydra_2                             = 245;	
        public const byte Hydra_3                             = 246;	
        public const byte Hydra_4                             = 247;	
        public const byte Dragon_1                            = 248;	
        public const byte Dragon_2                            = 249;	
        public const byte Dragon_3                            = 250;	
        public const byte Dragon_4                            = 251;	
        public const byte Balron_1                            = 252;	
        public const byte Balron_2                            = 253;	
        public const byte Balron_3                            = 254;	
        public const byte Balron_4                            = 255;

        public static string GetLabel(byte tile)
        {
            return tileLabels[tile];
        }

        private static Dictionary<byte, string> tileLabels = new Dictionary<byte, string>()
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
