using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class AvatarOffsetsOriginal : IAvatarOffset
    {
        public int BELL_REQUIREMENT_OFFSET => throw new NotImplementedException();
        public int BOOK_REQUIREMENT_OFFSET { get; } = 0x6DB;
        public int CANDLE_REQUIREMENT_OFFSET { get; } = 0x720;
        // https://wiki.ultimacodex.com/wiki/Ultima_IV_Internal_Formats#AVATAR.EXE
        // Above doesn't work anymore cuz we have modified AVATAR.EXE
        public int ABYSS_PARTY_COMPARISON { get; } = 0x34AB;
        public int LB_PARTY_COMPARISON { get; } = 0xE449;
        public int MOONGATE_X_OFFSET { get; } = 0x0fad1;
        public int MOONGATE_Y_OFFSET { get; } = 0x0fad9;
        public int AREA_X_OFFSET { get; } = 0x0fb01; // towns, cities, castles, dungeons, shrines
        public int AREA_Y_OFFSET { get; } = 0x0fb21;


        public int DEATH_EXIT_X_OFFSET { get; } = 0x011ac;
        public int DEATH_EXIT_Y_OFFSET { get; } = 0x011b1;

        
        public int PIRATE_COVE_X_OFFSET { get; } = 0x0fba9; // length 8
        public int PIRATE_COVE_Y_OFFSET { get; } = 0x0fbb1; // length 8
        public int PIRATE_COVE_SHIP_TILES { get; } = 0x0fbb9; // length 8 (Direction pirates are facing)
        public int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1 { get; } = 0x03084;
        public int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1 { get; } = 0x0308B;
        public int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET2 { get; } = 0x03123;
        public int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET2 { get; } = 0x0312A;
        public int WORD_OF_PASSAGE { get; } = 0x104F0;
        public int MONSTER_HP_OFFSET { get; } = 0x11685; // length 52
        public int MONSTER_LEADER_TYPES_OFFSET { get; } = 0x116b9; // length 36
        public int MONSTER_ENCOUNTER_SIZE_OFFSET { get; } = 0x116dd; // length 36
        public int ALTAR_EXIT_DESTINATION { get; } = 0x118c5; // length 12 : altar room exit destinations 
        /*
         *     0-3 { get; } = truth (north, east, south, west)
         *     4-7 { get; } = love
         *     8-11 { get; } = courage
         */
        public int AMBUSH_MONSTER_TYPES { get; } = 0x11963; //length 8 : ambush monster types
        public int CITY_RUNE_MASK_PAIRS_OFFSET { get; } = 0x11baf; // length 16 : city/runemask pairs (city id, corresponding rune bitmask)
        public int ITEM_LOCATIONS_OFFSET { get; } = 0x11bcb; // length 120 : 24 five-byte item location records (see below)
        /*
         * Each item location record has the following structure:

            Offset	Length (in bytes)	Purpose
            0x0	1	Item Location (same encoding as party location in PARTY.SAV, e.g. 0 for surface)
            0x1	1	X Coordinate of Item
            0x2	1	Y Coordinate of Item
            0x3	2	 ??? (a pointer?)
         */

        public int MONSTER_DAMAGE_BITSHIFT_OFFSET { get; } = 0x98E6;

        public int WEAPON_DAMAGE_OFFSET { get; } = 0x11703;
        public int MONSTER_SPAWN_TIER_ONE { get; } = 0x5B68;
        public int MONSTER_SPAWN_TIER_TWO { get; } = 0x5B83;
        public int MONSTER_SPAWN_TIER_THREE { get; } = 0x5BBB;

        public int MONSTER_QTY_ONE { get; } = 0x80EF;
        public int MONSTER_QTY_TWO { get; } = 0x8100;

        public int LB_TEXT_OFFSET { get; } = 0x156ca;
        public int LB_HELP_TEXT_OFFSET { get; } = 0x162D4;
        public int MANTRA_OFFSET { get; } = 0x16DD4;
        public int MANTRA_POINTERS_OFFSET { get; } = 0x17594;
        public int SHRINE_TEXT_OFFSET { get; } = 0x16df2;

        public int WHITE_STONE_LOCATION_TEXT { get; } = 0x17434;
        public int BLACK_STONE_LOCATION_TEXT { get; } = 0x174F9;

        public int SHOP_LOCATION_OFFSET { get; } = 0x11F7F;

        public int DEMON_SPAWN_TRIGGER_X1_OFFSET { get; } = 0x2F17;
        public int DEMON_SPAWN_TRIGGER_X2_OFFSET { get; } = 0x2F1E;
        public int DEMON_SPAWN_TRIGGER_Y1_OFFSET { get; } = 0x2F25;
        public int DEMON_SPAWN_TRIGGER_Y2_OFFSET { get; } = 0x2F2C;
        public int DEMON_SPAWN_LOCATION_X_OFFSET { get; } = 0x29EA;

        public int BALLOON_SPAWN_TRIGGER_X_OFFSET { get; } = 0x29A8;
        public int BALLOON_SPAWN_TRIGGER_Y_OFFSET { get; } = 0x29AF;

        public int BALLOON_SPAWN_LOCATION_X_OFFSET { get; } = 0x29BE;
        public int BALLOON_SPAWN_LOCATION_Y_OFFSET { get; } = 0x29C3;

        public int LBC_DUNGEON_EXIT_X_OFFSET { get; } = 0x4766;
        public int LBC_DUNGEON_EXIT_Y_OFFSET { get; } = 0x476B;

        public int ITEM_USE_TRIGGER_BELL_X_OFFSET { get; } = 0x0693;
        public int ITEM_USE_TRIGGER_BELL_Y_OFFSET { get; } = 0x069A;
        public int ITEM_USE_TRIGGER_BOOK_X_OFFSET { get; } = 0x06CC;
        public int ITEM_USE_TRIGGER_BOOK_Y_OFFSET { get; } = 0x06D3;
        public int ITEM_USE_TRIGGER_CANDLE_X_OFFSET { get; } = 0x0711;
        public int ITEM_USE_TRIGGER_CANDLE_Y_OFFSET { get; } = 0x0718;
        public int ITEM_USE_TRIGGER_SKULL_X_OFFSET { get; } = 0x07E3;
        public int ITEM_USE_TRIGGER_SKULL_Y_OFFSET { get; } = 0x07EA;

        public int WHIRLPOOL_EXIT_X_OFFSET { get; } = 0x7A92;
        public int WHIRLPOOL_EXIT_Y_OFFSET { get; } = 0x7A97;

        public int ABYSS_EJECTION_LOCATIONS_X { get; } = 0xFEAD; // Length 13 - Not sure what these are for yet. Appear to be exit coords for when you fail tests in the Abyss https://github.com/ergonomy-joe/u4-decompiled/blob/c2c2108fa3bb346bcd1d8c207c526f33a4c8f5ef/SRC/U4_END.C#L37
        public int ABYSS_EJECTION_LOCATIONS_Y { get; } = 0xFEBB;

        public int SPELL_RECIPE_OFFSET { get; } = 0x11A29;
        public static int RUNE_IMAGE_INDEX2 { get; } = 0xFB85;
        public static int RUNE_IMAGE_INDEX { get; } = 0x17551;

        // Originally blink didn't have an upperbound https://github.com/ergonomy-joe/u4-decompiled/blob/c2c2108fa3bb346bcd1d8c207c526f33a4c8f5ef/SRC/U4_SPELL.C#L179
        public int BLINK_CAST_EXCLUSION_X1_OFFSET => throw new NotImplementedException();

        public int BLINK_CAST_EXCLUSION_X2_OFFSET => throw new NotImplementedException();

        public int BLINK_CAST_EXCLUSION_Y1_OFFSET => throw new NotImplementedException();

        public int BLINK_CAST_EXCLUSION_Y2_OFFSET => throw new NotImplementedException();

        public int BLINK_DESTINATION_EXCLUSION_X1_OFFSET => throw new NotImplementedException();

        public int BLINK_DESTINATION_EXCLUSION_X2_OFFSET => throw new NotImplementedException();

        public int BLINK_DESTINATION_EXCLUSION_Y1_OFFSET => throw new NotImplementedException();

        public int BLINK_DESTINATION_EXCLUSION_Y2_OFFSET => throw new NotImplementedException();

        public int ENABLE_MIX_QUANTITY_OFFSET => throw new NotImplementedException();

        public int BLINK_DESTINATION2_EXCLUSION_X1_OFFSET => throw new NotImplementedException();

        public int BLINK_DESTINATION2_EXCLUSION_X2_OFFSET => throw new NotImplementedException();

        public int BLINK_DESTINATION2_EXCLUSION_Y1_OFFSET => throw new NotImplementedException();

        public int BLINK_DESTINATION2_EXCLUSION_Y2_OFFSET => throw new NotImplementedException();

        public int ENABLE_SLEEP_BACKOFF_OFFSET => throw new NotImplementedException();

        public int ENABLE_ACTIVE_PLAYER_1_OFFSET => throw new NotImplementedException();

        public int ENABLE_HIT_CHANCE_OFFSET => throw new NotImplementedException();

        public int ENABLE_DIAGONAL_ATTACK_OFFSET => throw new NotImplementedException();

        public int ENABLE_SACRIFICE_FIX_OFFSET => throw new NotImplementedException();

        public int ENABLE_PRINCIPLE_ITEM_REORDER_OFFSET => throw new NotImplementedException();

        public int ENCODED_FLAGS_OFFSET => throw new NotImplementedException();

        public int SEED_OFFSET => throw new NotImplementedException();

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
