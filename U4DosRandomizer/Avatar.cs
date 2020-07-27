using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class Avatar
    {
        public static void MoveMoongates(byte[,] worldMapUlt, byte[] avatar)
        {
            //throw in a lava to make it easy to find
            for (int offset = 0; offset < 8; offset++)
            {
                worldMapUlt[200, 200 + offset] = 76;
            }

            int moongateXOffset = 0x0fad1;
            int moongateYOffset = 0x0fad9;

            for (byte offset = 0; offset < 8; offset++)
            {
                avatar[MOONGATE_X_OFFSET + offset] = 200;
                avatar[MOONGATE_Y_OFFSET + offset] = Convert.ToByte(200 + offset);
            }

            return;
        }

        public static void MoveTowns(byte[,] worldMapUlt, byte[] avatar, UltimaData data)
        {
            // throw in a town to make it easier to find
            for (int offset = 0; offset < 32; offset++)
            {
                byte tile = 10;
                if (offset < 4)
                {
                    tile = 11;
                }
                else if (offset < 16)
                {
                    tile = 10;
                }
                else if (offset < 24)
                {
                    tile = 9;
                }
                else if (offset < 24+8)
                {
                    tile = 30;
                }
                worldMapUlt[202, 200 + offset] = tile;
            }

            for (byte offset = 0; offset < 32; offset++)
            {
                avatar[AREA_X_OFFSET + offset] = 202;
                avatar[AREA_Y_OFFSET + offset] = Convert.ToByte(200 + offset);
            }
        }

        public static void PlaceAllItems(byte[] avatar)
        {
            for (byte offset = 0; offset < 24; offset++)
            {
                avatar[ITEM_LOCATIONS_OFFSET + (offset * 5)] = 0;
                avatar[ITEM_LOCATIONS_OFFSET + (offset * 5) + 1] = 201;
                avatar[ITEM_LOCATIONS_OFFSET + (offset * 5) + 2] = Convert.ToByte(200 + offset);
            }

            return;
        }

        // https://wiki.ultimacodex.com/wiki/Ultima_IV_Internal_Formats#AVATAR.EXE
        private static int MOONGATE_X_OFFSET = 0x0fad1;
        private static int MOONGATE_Y_OFFSET = 0x0fad9;
        private static int AREA_X_OFFSET = 0x0fb01; // towns, cities, castles, dungeons, shrines
        private static int AREA_Y_OFFSET = 0x0fb21;
        /*
         * https://github.com/ergonomy-joe/u4-decompiled/blob/master/SRC/U4_LOC.H
         * 0 - Britannia (from the west???)
         * 1 - Lycaeum (also west)
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
        private static int PIRATE_COVE_X_OFFSET = 0x0fba9; // length 8
        private static int PIRATE_COVE_Y_OFFSET = 0x0fbb1; // length 8
        private static int PIRATE_COVE_SHIP_TILES = 0x0fbb9; // length 8 (What is this?)
        private static int MONSTER_HP_OFFSET = 0x11685; // length 52
        private static int MONSTER_LEADER_TYPES_OFFSET = 0x116b9; // length 36
        private static int MONSTER_ENCOUNTER_SIZE_OFFSET = 0x116dd; // length 36
        private static int ALTAR_EXIT_DESTINATION = 0x118c5; // length 12 : altar room exit destinations 
        /*
         *     0-3 = truth (north, east, south, west)
         *     4-7 = love
         *     8-11 = courage
         */
        private static int AMBUSH_MONSTER_TYPES = 0x11963; //length 8 : ambush monster types
        private static int CITY_RUNE_MASK_PAIRS_OFFSET = 0x11baf; // length 16 : city/runemask pairs (city id, corresponding rune bitmask)
        private static int ITEM_LOCATIONS_OFFSET = 0x11bcb; // length 120 : 24 five-byte item location records (see below)
        /*
         * Each item location record has the following structure:

            Offset	Length (in bytes)	Purpose
            0x0	1	Item Location (same encoding as party location in PARTY.SAV, e.g. 0 for surface)
            0x1	1	X Coordinate of Item
            0x2	1	Y Coordinate of Item
            0x3	2	 ??? (a pointer?)
         */
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
        private static int WHITE_STONE_LOCATION_TEXT = 0x17434;
        private static int BLACK_STONE_LOCATION_TEXT = 0x174F9;
    }
}

