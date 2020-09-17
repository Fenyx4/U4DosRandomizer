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
                if(pi.Name.ToLower().Contains("offset") && !pi.Name.ToLower().Contains("blink") && pi.Name != "ENABLE_MIX_QUANTITY_OFFSET")
                {
                    var newValue = avatarBytes[(int)pi.GetValue(this, null)];
                    var oldValue = originalAvatarBytes[(int)pi.GetValue(originalOffsets, null)];
                    if (newValue != oldValue)
                    {
                        throw new Exception($"Offset {pi.Name} appears to be wrong.");
                    }
                }
                else if (pi.Name.ToLower().Contains("blink_cast"))
                {
                    var newValue = avatarBytes[(int)pi.GetValue(this, null)];
                    if(!(newValue == 0xc0 || newValue == 0xff))
                    {
                        throw new Exception($"Offset {pi.Name} appears to be wrong.");
                    }
                }
                else if (pi.Name.ToLower().Contains("blink_destination"))
                {
                    var newValue = avatarBytes[(int)pi.GetValue(this, null)];
                    if (!(newValue == 0x01 || newValue == 0x02))
                    {
                        throw new Exception($"Offset {pi.Name} appears to be wrong.");
                    }
                }
                else if (pi.Name == "ENABLE_MIX_QUANTITY_OFFSET")
                {
                    var newValue = avatarBytes[(int)pi.GetValue(this, null)];
                    if (newValue != 0x08)
                    {
                        throw new Exception($"Offset {pi.Name} appears to be wrong.");
                    }
                }
            }
        }

        public int MOONGATE_X_OFFSET { get; } = 0x0fa44; //0fad1
        public int MOONGATE_Y_OFFSET { get; } = 0x0fa4c; //fad9
        public int AREA_X_OFFSET { get; } = 0x0fa74; //fb01 // towns, cities, castles, dungeons, shrines
        public int AREA_Y_OFFSET { get; } = 0x0fa94; //fb21
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
        public int PIRATE_COVE_X_OFFSET { get; } = 0x0fb1c; //fba9 // length 8
        public int PIRATE_COVE_Y_OFFSET { get; } = 0x0fb24; //fbb1 // length 8
        public int PIRATE_COVE_SHIP_TILES { get; } = 0x0fb2c; //fbb9 // length 8 (Direction pirates are facing)
        public int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1 { get; } = 0x02f04; //3084
        public int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1 { get; } = 0x02f0b; //308B
        public int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET2 { get; } = 0x02fa3; //3123
        public int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET2 { get; } = 0x02faa; //312A
        public int MONSTER_HP_OFFSET { get; } = 0x11602; //11685 // length 52
        public int MONSTER_LEADER_TYPES_OFFSET { get; } = 0x11636; //116b9 // length 36
        public int MONSTER_ENCOUNTER_SIZE_OFFSET { get; } = 0x1165a; //116dd // length 36
        public int ALTAR_EXIT_DESTINATION { get; } = 0x1184a; //118c5 // length 12 : altar room exit destinations 
        /*
         *     0-3 { get; } = truth (north, east, south, west)
         *     4-7 { get; } = love
         *     8-11 { get; } = courage
         */
        public int AMBUSH_MONSTER_TYPES { get; } = 0x1188E8; //11963 //length 8 : ambush monster types
        public int CITY_RUNE_MASK_PAIRS_OFFSET { get; } = 0x11b42; //11baf // length 16 : city/runemask pairs (city id, corresponding rune bitmask)
        public int ITEM_LOCATIONS_OFFSET { get; } = 0x11b5e; //11bcb // length 120 : 24 five-byte item location records (see below)
        /*
         * Each item location record has the following structure:

            Offset	Length (in bytes)	Purpose
            0x0	1	Item Location (same encoding as party location in PARTY.SAV, e.g. 0 for surface)
            0x1	1	X Coordinate of Item
            0x2	1	Y Coordinate of Item
            0x3	2	 ??? (a pointer?)
         */
        
        public int LB_TEXT_OFFSET { get; } = 0x15695; //156ca
        public int SHRINE_TEXT_OFFSET { get; } = 0x16dbe; //16df2

        public int WHITE_STONE_LOCATION_TEXT { get; } = 0x17400; //17434
        public int BLACK_STONE_LOCATION_TEXT { get; } = 0x174C5; //174F9

        public int DEMON_SPAWN_TRIGGER_X1_OFFSET { get; } = 0x2D8C; //2F17 !!! e5
        public int DEMON_SPAWN_TRIGGER_X2_OFFSET { get; } = 0x2D90; //2F1E !!! ea
        public int DEMON_SPAWN_TRIGGER_Y1_OFFSET { get; } = 0x2DA1; //2F25 !!! d4
        public int DEMON_SPAWN_TRIGGER_Y2_OFFSET { get; } = 0x2DA5; //2F2C !!! d9
        public int DEMON_SPAWN_LOCATION_X_OFFSET { get; } = 0x285C; //29EA

        public int BALLOON_SPAWN_TRIGGER_X_OFFSET { get; } = 0x281A; //29A8
        public int BALLOON_SPAWN_TRIGGER_Y_OFFSET { get; } = 0x2821; //29AF

        public int BALLOON_SPAWN_LOCATION_X_OFFSET { get; } = 0x2830; //29BE
        public int BALLOON_SPAWN_LOCATION_Y_OFFSET { get; } = 0x2835; //29C3

        public int LBC_DUNGEON_EXIT_X_OFFSET { get; } = 0x45E6; //4766
        public int LBC_DUNGEON_EXIT_Y_OFFSET { get; } = 0x45EB; //476B

        public int ITEM_USE_TRIGGER_BELL_X_OFFSET { get; } = 0x04D1; //693
        public int ITEM_USE_TRIGGER_BELL_Y_OFFSET { get; } = 0x04D8; //69A
        public int ITEM_USE_TRIGGER_BOOK_X_OFFSET { get; } = 0x050A; //6CC
        public int ITEM_USE_TRIGGER_BOOK_Y_OFFSET { get; } = 0x0511; //6D3
        public int ITEM_USE_TRIGGER_CANDLE_X_OFFSET { get; } = 0x054F; //711
        public int ITEM_USE_TRIGGER_CANDLE_Y_OFFSET { get; } = 0x0556; //718
        public int ITEM_USE_TRIGGER_SKULL_X_OFFSET { get; } = 0x0621; //7E3
        public int ITEM_USE_TRIGGER_SKULL_Y_OFFSET { get; } = 0x0628; //7EA

        public int WHIRLPOOL_EXIT_X_OFFSET { get; } = 0x79B6; //7A92
        public int WHIRLPOOL_EXIT_Y_OFFSET { get; } = 0x79BB; //7A97

        public int ABYSS_EJECTION_LOCATIONS_X { get; } = 0xFE20; //FEAD  // Length 13 - Exit coords for when you fail tests in the Abyss https://github.com/ergonomy-joe/u4-decompiled/blob/c2c2108fa3bb346bcd1d8c207c526f33a4c8f5ef/SRC/U4_END.C#L37
        public int ABYSS_EJECTION_LOCATIONS_Y { get; } = 0xFE2E; //FEBB

        public int SPELL_RECIPE_OFFSET { get; } = 0x119BC; //11A29

        public int BLINK_CAST_EXCLUSION_X1_OFFSET { get; } = 0x666D;

        public int BLINK_CAST_EXCLUSION_X2_OFFSET { get; } = 0x6671;

        public int BLINK_CAST_EXCLUSION_Y1_OFFSET { get; } = 0x6682;

        public int BLINK_CAST_EXCLUSION_Y2_OFFSET { get; } = 0x6686;


        public int BLINK_DESTINATION_EXCLUSION_X1_OFFSET { get; } = 0x6707;

        public int BLINK_DESTINATION_EXCLUSION_X2_OFFSET { get; } = 0x670B;

        public int BLINK_DESTINATION_EXCLUSION_Y1_OFFSET { get; } = 0x6726;

        public int BLINK_DESTINATION_EXCLUSION_Y2_OFFSET { get; } = 0x672A;

        public int BLINK_DESTINATION2_EXCLUSION_X1_OFFSET { get; } = 0x6749;

        public int BLINK_DESTINATION2_EXCLUSION_X2_OFFSET { get; } = 0x674D;

        public int BLINK_DESTINATION2_EXCLUSION_Y1_OFFSET { get; } = 0x676C;

        public int BLINK_DESTINATION2_EXCLUSION_Y2_OFFSET { get; } = 0x6770;

        public int ENABLE_MIX_QUANTITY_OFFSET { get; } = 0x8D79;

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
