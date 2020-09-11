using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace U4DosRandomizer
{
    public class AvatarOffsetsNew : IAvatarOffset
    {
        public AvatarOffsetsNew(byte[] avatarBytes, string originalFile)
        {
            // Check offsets
            var avatarStream = new System.IO.FileStream(originalFile, System.IO.FileMode.Open);
            var originalAvatarBytes = avatarStream.ReadAllBytes();

            var originalOffsets = new AvatarOffsetsOriginal();

            PropertyInfo[] properties = this.GetType().GetInterface("IAvatarOffset").GetProperties();
            foreach (PropertyInfo pi in properties)
            {
                if(pi.Name.ToLower().Contains("offset"))
                {
                    var newValue = avatarBytes[(int)pi.GetValue(this, null)];
                    var oldValue = originalAvatarBytes[(int)pi.GetValue(originalOffsets, null)];
                    if (newValue != oldValue)
                    {
                        throw new Exception($"Offset {pi.Name} appears to be wrong.");
                    }
                }
            }
        }

        public int MOONGATE_X_OFFSET { get; } = 0x0f924;
        public int MOONGATE_Y_OFFSET { get; } = 0x0f92c;
        public int AREA_X_OFFSET { get; } = 0x0f954; // towns, cities, castles, dungeons, shrines
        public int AREA_Y_OFFSET { get; } = 0x0f974;
        public int LOC_BUILDINGS { get; } = 0x01;

        public int LOC_CASTLES { get; } = 0x01;
        public int LOC_LCB { get; } = 0x01;
        public int LOC_LYCAEUM { get; } = 0x02;
        public int LOC_EMPATH { get; } = 0x03;
        public int LOC_SERPENT { get; } = 0x04;

        public int LOC_TOWNS { get; } = 0x05;
        public int LOC_MOONGLOW { get; } = 0x05;
        public int LOC_BRITAIN { get; } = 0x06;
        public int LOC_JHELOM { get; } = 0x07;
        public int LOC_YEW { get; } = 0x08;
        public int LOC_MINOC { get; } = 0x09;
        public int LOC_TRINSIC { get; } = 0x0a;
        public int LOC_SKARA { get; } = 0x0b;
        public int LOC_MAGINCIA { get; } = 0x0c;
        public int LOC_PAWS { get; } = 0x0d;
        public int LOC_DEN { get; } = 0x0e;
        public int LOC_VESPER { get; } = 0x0f;
        public int LOC_COVE { get; } = 0x10;

        public int LOC_DUNGEONS { get; } = 0x11;
        public int LOC_DECEIT { get; } = 0x11;
        public int LOC_DESPISE { get; } = 0x12;
        public int LOC_DESTARD { get; } = 0x13;
        public int LOC_WRONG { get; } = 0x14;
        public int LOC_COVETOUS { get; } = 0x15;
        public int LOC_SHAME { get; } = 0x16;
        public int LOC_HYTHLOTH { get; } = 0x17;
        public int LOC_ABYSS { get; } = 0x18;

        public int LOC_SHRINES { get; } = 0x19;
        public int LOC_HONESTY { get; } = 0x19;
        public int LOC_COMPASSION { get; } = 0x1a;
        public int LOC_VALOR { get; } = 0x1b;
        public int LOC_JUSTICE { get; } = 0x1c;
        public int LOC_SACRIFICE { get; } = 0x1d;
        public int LOC_HONOR { get; } = 0x1e;
        public int LOC_SPIRITUALITY { get; } = 0x1f;
        public int LOC_HUMILITY { get; } = 0x20;
        /*
         * https://github.com/ergonomy-joe/u4-decompiled/blob/master/SRC/U4_LOC.H
         * 0 - Britannia
         * 1 - Lycaeum
         * 2 - Empath Abbey
         * 3 - Serpents Hold
         * 4 - Moonglow
         * 5 - Britain
         * 6 - Jhelom
         * 7 - Yew
         * 8 - Minoc
         * 9 - Trinsic
         * 10 - Skara Brae
         * 11 - Magincia
         * 12 - Paws
         * 13 - Buccaneer's Den
         * 14 - Vesper
         * 15 - Cove
         * 16 - Deciet
         * 17 - Despise
         * 18 - Destard
         * 19 - Wrong
         * 20 - Covetous
         * 21 - Shame
         * 22 - Hythloth
         * 23 - The Great Stygian Abyss
         * 24 - Honesty
         * 25 - Compassion
         * 26 - Valor
         * 27 - Justice
         * 28 - Sacrifice
         * 29 - Honor
         * 30 - Spirituality
         * 31 - Humility
         */
        public int PIRATE_COVE_X_OFFSET { get; } = 0x0f9fc; // length 8
        public int PIRATE_COVE_Y_OFFSET { get; } = 0x0fa04; // length 8
        public int PIRATE_COVE_SHIP_TILES { get; } = 0x0fa0c; // length 8 (Direction pirates are facing)
        public int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1 { get; } = 0x02ec2;
        public int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1 { get; } = 0x02ec9;
        public int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET2 { get; } = 0x02f61;
        public int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET2 { get; } = 0x02f68;
        public int MONSTER_HP_OFFSET { get; } = 0x114e2; // length 52
        public int MONSTER_LEADER_TYPES_OFFSET { get; } = 0x11516; // length 36
        public int MONSTER_ENCOUNTER_SIZE_OFFSET { get; } = 0x1153a; // length 36
        public int ALTAR_EXIT_DESTINATION { get; } = 0x1172a; // length 12 : altar room exit destinations 
        /*
         *     0-3 { get; } = truth (north, east, south, west)
         *     4-7 { get; } = love
         *     8-11 { get; } = courage
         */
        public int AMBUSH_MONSTER_TYPES { get; } = 0x117C8; //length 8 : ambush monster types
        public int CITY_RUNE_MASK_PAIRS_OFFSET { get; } = 0x11a14; // length 16 : city/runemask pairs (city id, corresponding rune bitmask)
        public int ITEM_LOCATIONS_OFFSET { get; } = 0x11a30; // length 120 : 24 five-byte item location records (see below)
        /*
         * Each item location record has the following structure:

            Offset	Length (in bytes)	Purpose
            0x0	1	Item Location (same encoding as party location in PARTY.SAV, e.g. 0 for surface)
            0x1	1	X Coordinate of Item
            0x2	1	Y Coordinate of Item
            0x3	2	 ??? (a pointer?)
         */
        public int ITEM_MANDRAKE { get; } = 0;
        public int ITEM_MANDRAKE2 { get; } = 1;
        public int ITEM_NIGHTSHADE { get; } = 2;
        public int ITEM_NIGHTSHADE2 { get; } = 3;
        public int ITEM_BELL { get; } = 4; // Bell of Courage
        public int ITEM_HORN { get; } = 5; // Silver Horn
        public int ITEM_WHEEL { get; } = 6; // Wheel of H.M.S. Cape
        public int ITEM_SKULL { get; } = 7; // Skull of Mondain
        public int ITEM_BLACK_STONE { get; } = 8;
        public int ITEM_WHITE_STONE { get; } = 9;
        public int ITEM_BOOK { get; } = 10; // Book of Truth
        public int ITEM_CANDLE { get; } = 11; //
        public int ITEM_TELESCOPE { get; } = 12; // telescope (Crashes if moved (probably fine in any other town))
        public int ITEM_ARMOR { get; } = 13; // Mystic Armor
        public int ITEM_WEAPON { get; } = 14; // Mystic Weapon
        public int ITEM_RUNE_HONESTY { get; } = 15;
        public int ITEM_RUNE_COMPASSION { get; } = 16;
        public int ITEM_RUNE_VALOR { get; } = 17;
        public int ITEM_RUNE_JUSTICE { get; } = 18;
        public int ITEM_RUNE_SACRIFICE { get; } = 19;
        public int ITEM_RUNE_HONOR { get; } = 20;
        public int ITEM_RUNE_SPIRITUALITY { get; } = 21;
        public int ITEM_RUNE_HUMILITY { get; } = 22;
        /*
         * https://github.com/ergonomy-joe/u4-decompiled/blob/master/SRC/U4_SRCH.C#L246
         * 0 - Mandrake
         * 1 - Mandrake
         * 2 - Nightshade
         * 3 - Nightshade
         * 4 - Bell of Courage
         * 5 - Silver Horn
         * 6 - Wheel of H.M.S. Cape
         * 7 - Skull of Mondain
         * 8 - Black Stone
         * 9 - White Stone
         * 10 - Book of Truth
         * 11 - Candle of Love
         * 12 - telescope (Crashes if moved)
         * 13 - Mystic Armor
         * 14 - Mystic Weapon
         * 15 - Rune of the Great Stygian Abyss
         * 16 - Rune of the Great Stygian Abyss
         * 17 - Rune of the Great Stygian Abyss
         * 18 - Rune of the Great Stygian Abyss
         * 19 - Rune of the Great Stygian Abyss
         * 20 - Rune of the Great Stygian Abyss
         * 21 - Rune of the Great Stygian Abyss
         * 22 - Rune of the Great Stygian Abyss
         * 23 - ??
         * All runes on the surface are bugged to be Great Stygian Abyss. I'll figure out which are which later although it doesn't really matter. They just have to be located in the right town.
         */

        public int LB_TEXT_OFFSET { get; } = 0x15567;
        public int SHRINE_TEXT_OFFSET { get; } = 0x16c90;

        public int WHITE_STONE_LOCATION_TEXT { get; } = 0x172D2;
        public int BLACK_STONE_LOCATION_TEXT { get; } = 0x17397 ;

        public int DEMON_SPAWN_TRIGGER_X1_OFFSET { get; } = 0x2D55;
        public int DEMON_SPAWN_TRIGGER_X2_OFFSET { get; } = 0x2D5C;
        public int DEMON_SPAWN_TRIGGER_Y1_OFFSET { get; } = 0x2D63;
        public int DEMON_SPAWN_TRIGGER_Y2_OFFSET { get; } = 0x2D6A;
        public int DEMON_SPAWN_LOCATION_X_OFFSET { get; } = 0x2828;

        public int BALLOON_SPAWN_TRIGGER_X_OFFSET { get; } = 0x27E6;
        public int BALLOON_SPAWN_TRIGGER_Y_OFFSET { get; } = 0x27ED;

        public int BALLOON_SPAWN_LOCATION_X_OFFSET { get; } = 0x27FC;
        public int BALLOON_SPAWN_LOCATION_Y_OFFSET { get; } = 0x2801;

        public int LBC_DUNGEON_EXIT_X_OFFSET { get; } = 0x45A4;
        public int LBC_DUNGEON_EXIT_Y_OFFSET { get; } = 0x45A9;

        public int ITEM_USE_TRIGGER_BELL_X_OFFSET { get; } = 0x04D1;
        public int ITEM_USE_TRIGGER_BELL_Y_OFFSET { get; } = 0x04D8;
        public int ITEM_USE_TRIGGER_BOOK_X_OFFSET { get; } = 0x050A;
        public int ITEM_USE_TRIGGER_BOOK_Y_OFFSET { get; } = 0x0511;
        public int ITEM_USE_TRIGGER_CANDLE_X_OFFSET { get; } = 0x054F;
        public int ITEM_USE_TRIGGER_CANDLE_Y_OFFSET { get; } = 0x0556;
        public int ITEM_USE_TRIGGER_SKULL_X_OFFSET { get; } = 0x0621;
        public int ITEM_USE_TRIGGER_SKULL_Y_OFFSET { get; } = 0x0628;

        public int WHIRLPOOL_EXIT_X_OFFSET { get; } = 0x78DA;
        public int WHIRLPOOL_EXIT_Y_OFFSET { get; } = 0x78DF;

        public int UNKNOWN_EXIT_LOCATIONS_X { get; } = 0xFD00; // Length 13 - Not sure what these are for yet. Appear to be exit coords for when you fail tests in the Abyss https://github.com/ergonomy-joe/u4-decompiled/blob/c2c2108fa3bb346bcd1d8c207c526f33a4c8f5ef/SRC/U4_END.C#L37
        public int UNKNOWN_EXIT_LOCATIONS_Y { get; } = 0xFD0D;

        public int SPELL_RECIPE_OFFSET { get; } = 0x1188E;
        public const byte Reagent_ash = (0x80 >> 0);
        public const byte Reagent_ginseng = (0x80 >> 1);
        public const byte Reagent_garlic = (0x80 >> 2);
        public const byte Reagent_spiderSilk = (0x80 >> 3);
        public const byte Reagent_bloodMoss = (0x80 >> 4);
        public const byte Reagent_blackPearl = (0x80 >> 5);
        public const byte Reagent_nightshade = (0x80 >> 6);
        public const byte Reagent_mandrake = (0x80 >> 7);
    }
}
