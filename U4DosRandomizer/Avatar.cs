using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class Avatar
    {
        private const string filename = "AVATAR.EXE";
        private byte[] avatarBytes;

        public void Load(string path, UltimaData data)
        {
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            var avatarStream = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open);
            avatarBytes = avatarStream.ReadAllBytes();

            for (int offset = 0; offset < 24; offset++)
            {
                var item = new Item();
                item.Location = avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5];
                item.X = avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5 + 1];
                item.Y = avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5 + 2];
                data.Items.Add(item);
            }

            OriginalShrineText = new List<string>();
            OriginalShrineTextStartOffset = new List<int>();
            var shrineTextBytes = new List<byte>();
            var textOffset = SHRINE_TEXT_OFFSET;
            for (int i = 0; i < 24; i++)
            {
                OriginalShrineTextStartOffset.Add(textOffset);
                for (; avatarBytes[textOffset] != 0x0A && avatarBytes[textOffset] != 0x00; textOffset++)
                {
                    shrineTextBytes.Add(avatarBytes[textOffset]);
                }
                OriginalShrineText.Add(System.Text.Encoding.Default.GetString(shrineTextBytes.ToArray()));
                shrineTextBytes.Clear();
                if (avatarBytes[textOffset] == 0x0A)
                {
                    textOffset++;
                }
                textOffset++;
            }
            data.ShrineText.Clear();
            data.ShrineText.AddRange(OriginalShrineText);

            OriginalLBText = new List<string>();
            OriginalLBTextStartOffset = new List<int>();
            var lbTextBytes = new List<byte>();
            textOffset = LB_TEXT_OFFSET;
            // He has more text than 19 but there is some weird stuff after 19 that doesn't get turned into text well. And as far as I can tell we won't need any text after 19
            for (int i = 0; i < 19; i++)
            {
                OriginalLBTextStartOffset.Add(textOffset);
                for (; avatarBytes[textOffset] != 0x00 && avatarBytes[textOffset] != 0xAB; textOffset++)
                {
                    lbTextBytes.Add(avatarBytes[textOffset]);
                }
                OriginalLBText.Add(System.Text.Encoding.Default.GetString(lbTextBytes.ToArray()));
                lbTextBytes.Clear();
                if (avatarBytes[textOffset] == 0x0A || avatarBytes[textOffset] == 0xAB)
                {
                    textOffset++;
                }
                textOffset++;
            }
            data.LBText.Clear();
            data.LBText.AddRange(OriginalLBText);

            data.DaemonSpawnX1 = avatarBytes[DEMON_SPAWN_TRIGGER_X1_OFFSET];
            data.DaemonSpawnX2 = avatarBytes[DEMON_SPAWN_TRIGGER_X2_OFFSET];
            data.DaemonSpawnY1 = avatarBytes[DEMON_SPAWN_TRIGGER_Y1_OFFSET];
            data.DaemonSpawnY2 = avatarBytes[DEMON_SPAWN_TRIGGER_Y2_OFFSET];
            data.DaemonSpawnLocationX = avatarBytes[DEMON_SPAWN_LOCATION_X_OFFSET];

            for(int i = 0; i < 8; i++)
            {
                data.PirateCove.Add(new Coordinate(avatarBytes[i + PIRATE_COVE_X_OFFSET], avatarBytes[i + PIRATE_COVE_Y_OFFSET]));
            }

            data.PirateCoveSpawnTrigger = new Coordinate(avatarBytes[PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1], avatarBytes[PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1]);

            data.WhirlpoolExit = new Coordinate(avatarBytes[WHIRLPOOL_EXIT_X_OFFSET], avatarBytes[WHIRLPOOL_EXIT_Y_OFFSET]);
        }

        internal static void Restore(string path)
        {
            var file = Path.Combine(path, filename);
            FileHelper.Restore(file);
        }

        public void Update(UltimaData data)
        {
            for (var offset = 0; offset < 24; offset++)
            {
                avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5] = data.Items[offset].Location;
                avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5 + 1] = data.Items[offset].X;
                avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5 + 2] = data.Items[offset].Y;
            }

            // Use these items at the entrance to the abyss
            avatarBytes[ITEM_USE_TRIGGER_BELL_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].X;
            avatarBytes[ITEM_USE_TRIGGER_BELL_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].Y;
            avatarBytes[ITEM_USE_TRIGGER_BOOK_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].X;
            avatarBytes[ITEM_USE_TRIGGER_BOOK_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].Y;
            avatarBytes[ITEM_USE_TRIGGER_CANDLE_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].X;
            avatarBytes[ITEM_USE_TRIGGER_CANDLE_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].Y;
            avatarBytes[ITEM_USE_TRIGGER_SKULL_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].X;
            avatarBytes[ITEM_USE_TRIGGER_SKULL_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].Y;

            ////throw in a lava to make it easy to find
            //for (int offset = 0; offset < 8; offset++)
            //{
            //    worldMapUlt[200, 200 + offset] = 76;
            //}
            for (byte offset = 0; offset < data.Moongates.Count; offset++)
            {
                avatarBytes[MOONGATE_X_OFFSET + offset] = data.Moongates[offset].X;
                avatarBytes[MOONGATE_Y_OFFSET + offset] = data.Moongates[offset].Y;
            }

            avatarBytes[AREA_X_OFFSET + LOC_LCB - 1] = data.LCB[0].X;
            avatarBytes[AREA_Y_OFFSET + LOC_LCB - 1] = data.LCB[0].Y;

            for (var offset = 0; offset < data.Castles.Count; offset++)
            {
                avatarBytes[AREA_X_OFFSET + LOC_CASTLES + offset] = data.Castles[offset].X;
                avatarBytes[AREA_Y_OFFSET + LOC_CASTLES + offset] = data.Castles[offset].Y;
            }

            for (var offset = 0; offset < data.Towns.Count; offset++)
            {
                avatarBytes[AREA_X_OFFSET + LOC_TOWNS + offset - 1] = data.Towns[offset].X;
                avatarBytes[AREA_Y_OFFSET + LOC_TOWNS + offset - 1] = data.Towns[offset].Y;
            }

            for (var offset = 0; offset < data.Shrines.Count; offset++)
            {
                // Skip Spirituality
                if (data.Shrines[offset] != null)
                {
                    avatarBytes[AREA_X_OFFSET + LOC_SHRINES + offset - 1] = data.Shrines[offset].X;
                    avatarBytes[AREA_Y_OFFSET + LOC_SHRINES + offset - 1] = data.Shrines[offset].Y;
                }
            }

            for (var offset = 0; offset < data.Dungeons.Count; offset++)
            {
                avatarBytes[AREA_X_OFFSET + LOC_DUNGEONS + offset - 1] = data.Dungeons[offset].X;
                avatarBytes[AREA_Y_OFFSET + LOC_DUNGEONS + offset - 1] = data.Dungeons[offset].Y;
            }

            avatarBytes[BALLOON_SPAWN_TRIGGER_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 2].X;
            avatarBytes[BALLOON_SPAWN_TRIGGER_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 2].Y;
            avatarBytes[LBC_DUNGEON_EXIT_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 2].X;
            avatarBytes[LBC_DUNGEON_EXIT_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 2].Y;

            avatarBytes[BALLOON_SPAWN_LOCATION_X_OFFSET] = data.BalloonSpawn.X;
            avatarBytes[BALLOON_SPAWN_LOCATION_Y_OFFSET] = data.BalloonSpawn.Y;

            var avatarBytesList = new List<byte>(avatarBytes);
            for (int i = 0; i < OriginalShrineText.Count; i++)
            {
                if (data.ShrineText[i].Length > OriginalShrineText[i].Length)
                {
                    throw new Exception($"Shrine text \"{data.ShrineText[i]}\" is too long.");
                }
                data.ShrineText[i] = data.ShrineText[i].PadRight(OriginalShrineText[i].Length, ' ');
                
                avatarBytesList.RemoveRange(OriginalShrineTextStartOffset[i], OriginalShrineText[i].Length);
                avatarBytesList.InsertRange(OriginalShrineTextStartOffset[i], Encoding.ASCII.GetBytes(data.ShrineText[i]));

            }

            for (int i = 0; i < OriginalLBText.Count; i++)
            {
                if (data.LBText[i].Length > OriginalLBText[i].Length)
                {
                    throw new Exception($"LB text \"{data.LBText[i]}\" is too long.");
                }
                data.LBText[i] = data.LBText[i].PadRight(OriginalLBText[i].Length, ' ');

                avatarBytesList.RemoveRange(OriginalLBTextStartOffset[i], OriginalLBText[i].Length);
                avatarBytesList.InsertRange(OriginalLBTextStartOffset[i], Encoding.ASCII.GetBytes(data.LBText[i]));

            }
            avatarBytes = avatarBytesList.ToArray();

            avatarBytes[DEMON_SPAWN_TRIGGER_X1_OFFSET] = data.DaemonSpawnX1;
            avatarBytes[DEMON_SPAWN_TRIGGER_X2_OFFSET] = data.DaemonSpawnX2;
            avatarBytes[DEMON_SPAWN_TRIGGER_Y1_OFFSET] = data.DaemonSpawnY1;
            avatarBytes[DEMON_SPAWN_TRIGGER_Y2_OFFSET] = data.DaemonSpawnY2;
            avatarBytes[DEMON_SPAWN_LOCATION_X_OFFSET] = data.DaemonSpawnLocationX;

            for(int i = 0; i < data.PirateCove.Count; i++)
            {
                avatarBytes[PIRATE_COVE_X_OFFSET + i] = data.PirateCove[i].X;
                avatarBytes[PIRATE_COVE_Y_OFFSET + i] = data.PirateCove[i].Y;
            }

            avatarBytes[PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1] = data.PirateCoveSpawnTrigger.X;
            avatarBytes[PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1] = data.PirateCoveSpawnTrigger.Y;
            avatarBytes[PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET2] = data.PirateCoveSpawnTrigger.X;
            avatarBytes[PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET2] = data.PirateCoveSpawnTrigger.Y;

            avatarBytes[WHIRLPOOL_EXIT_X_OFFSET] = data.WhirlpoolExit.X;
            avatarBytes[WHIRLPOOL_EXIT_Y_OFFSET] = data.WhirlpoolExit.Y;

        }

        public void Save(string path)
        {
            var exePath = Path.Combine(path, filename);
            var avatarOut = new System.IO.BinaryWriter(new System.IO.FileStream(exePath, System.IO.FileMode.Truncate));

            //var binPath = Path.Combine(path, "AVATAR.bin");
            //var avatarOut2 = new System.IO.BinaryWriter(new System.IO.FileStream(binPath, System.IO.FileMode.Truncate));

            avatarOut.Write(avatarBytes);
            //avatarOut2.Write(avatarBytes);

            avatarOut.Close();
            //avatarOut2.Close();
        }

        // https://wiki.ultimacodex.com/wiki/Ultima_IV_Internal_Formats#AVATAR.EXE
        private static int MOONGATE_X_OFFSET = 0x0fad1;
        private static int MOONGATE_Y_OFFSET = 0x0fad9;
        private static int AREA_X_OFFSET = 0x0fb01; // towns, cities, castles, dungeons, shrines
        private static int AREA_Y_OFFSET = 0x0fb21;
        private static int LOC_BUILDINGS = 0x01;

        private static int LOC_CASTLES = 0x01;
        private static int LOC_LCB = 0x01;
        private static int LOC_LYCAEUM = 0x02;
        private static int LOC_EMPATH = 0x03;
        private static int LOC_SERPENT = 0x04;

        public static int LOC_TOWNS = 0x05;
        public static int LOC_MOONGLOW = 0x05;
        public static int LOC_BRITAIN = 0x06;
        public static int LOC_JHELOM = 0x07;
        public static int LOC_YEW = 0x08;
        public static int LOC_MINOC = 0x09;
        public static int LOC_TRINSIC = 0x0a;
        public static int LOC_SKARA = 0x0b;
        public static int LOC_MAGINCIA = 0x0c;
        public static int LOC_PAWS = 0x0d;
        public static int LOC_DEN = 0x0e;
        public static int LOC_VESPER = 0x0f;
        public static int LOC_COVE = 0x10;

        private static int LOC_DUNGEONS = 0x11;
        private static int LOC_DECEIT = 0x11;
        private static int LOC_DESPISE = 0x12;
        private static int LOC_DESTARD = 0x13;
        private static int LOC_WRONG = 0x14;
        private static int LOC_COVETOUS = 0x15;
        private static int LOC_SHAME = 0x16;
        private static int LOC_HYTHLOTH = 0x17;
        private static int LOC_ABYSS = 0x18;

        public static int LOC_SHRINES = 0x19;
        public static int LOC_HONESTY = 0x19;
        public static int LOC_COMPASSION = 0x1a;
        public static int LOC_VALOR = 0x1b;
        public static int LOC_JUSTICE = 0x1c;
        public static int LOC_SACRIFICE = 0x1d;
        public static int LOC_HONOR = 0x1e;
        public static int LOC_SPIRITUALITY = 0x1f;
        public static int LOC_HUMILITY = 0x20;
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
        private static int PIRATE_COVE_X_OFFSET = 0x0fba9; // length 8
        private static int PIRATE_COVE_Y_OFFSET = 0x0fbb1; // length 8
        private static int PIRATE_COVE_SHIP_TILES = 0x0fbb9; // length 8 (Direction pirates are facing)
        private static int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1 = 0x03084;
        private static int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1 = 0x0308B;
        private static int PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET2 = 0x03123;
        private static int PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET2 = 0x0312A;
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
        public static int ITEM_MANDRAKE = 0;
        public static int ITEM_MANDRAKE2 = 1;
        public static int ITEM_NIGHTSHADE = 2;
        public static int ITEM_NIGHTSHADE2 = 3;
        public static int ITEM_BELL = 4; // Bell of Courage
        public static int ITEM_HORN = 5; // Silver Horn
        public static int ITEM_WHEEL = 6; // Wheel of H.M.S. Cape
        public static int ITEM_SKULL = 7; // Skull of Mondain
        public static int ITEM_BLACK_STONE = 8;
        public static int ITEM_WHITE_STONE = 9;
        public static int ITEM_BOOK = 10; // Book of Truth
        public static int ITEM_CANDLE = 11; //
        public static int ITEM_TELESCOPE = 12; // telescope (Crashes if moved (probably fine in any other town))
        public static int ITEM_ARMOR = 13; // Mystic Armor
        public static int ITEM_WEAPON = 14; // Mystic Weapon
        public static int ITEM_RUNE_HONESTY = 15;
        public static int ITEM_RUNE_COMPASSION = 16;
        public static int ITEM_RUNE_VALOR = 17;
        public static int ITEM_RUNE_JUSTICE = 18;
        public static int ITEM_RUNE_SACRIFICE = 19;
        public static int ITEM_RUNE_HONOR = 20;
        public static int ITEM_RUNE_SPIRITUALITY = 21;
        public static int ITEM_RUNE_HUMILITY = 22;
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

        private static int LB_TEXT_OFFSET = 0x156ca;
        private static int SHRINE_TEXT_OFFSET = 0x16df2;

        private static int WHITE_STONE_LOCATION_TEXT = 0x17434;
        private static int BLACK_STONE_LOCATION_TEXT = 0x174F9;

        private static int DEMON_SPAWN_TRIGGER_X1_OFFSET = 0x2F17;
        private static int DEMON_SPAWN_TRIGGER_X2_OFFSET = 0x2F1E;
        private static int DEMON_SPAWN_TRIGGER_Y1_OFFSET = 0x2F25;
        private static int DEMON_SPAWN_TRIGGER_Y2_OFFSET = 0x2F2C;
        private static int DEMON_SPAWN_LOCATION_X_OFFSET = 0x29EA;

        private static int BALLOON_SPAWN_TRIGGER_X_OFFSET = 0x29A8;
        private static int BALLOON_SPAWN_TRIGGER_Y_OFFSET = 0x29AF;

        private static int BALLOON_SPAWN_LOCATION_X_OFFSET = 0x29BE;
        private static int BALLOON_SPAWN_LOCATION_Y_OFFSET = 0x29C3;

        private static int LBC_DUNGEON_EXIT_X_OFFSET = 0x4766;
        private static int LBC_DUNGEON_EXIT_Y_OFFSET = 0x476B;

        private static int ITEM_USE_TRIGGER_BELL_X_OFFSET = 0x0693;
        private static int ITEM_USE_TRIGGER_BELL_Y_OFFSET = 0x069A;
        private static int ITEM_USE_TRIGGER_BOOK_X_OFFSET = 0x06CC;
        private static int ITEM_USE_TRIGGER_BOOK_Y_OFFSET = 0x06D3;
        private static int ITEM_USE_TRIGGER_CANDLE_X_OFFSET = 0x0711;
        private static int ITEM_USE_TRIGGER_CANDLE_Y_OFFSET = 0x0718;
        private static int ITEM_USE_TRIGGER_SKULL_X_OFFSET = 0x07E3;
        private static int ITEM_USE_TRIGGER_SKULL_Y_OFFSET = 0x07EA;

        private static int WHIRLPOOL_EXIT_X_OFFSET = 0x7A92;
        private static int WHIRLPOOL_EXIT_Y_OFFSET = 0x7A97;

        private static int UNKNOWN_EXIT_LOCATIONS_X = 0xFEAD; // Length 13 - Not sure what these are for yet. Appear to be exit coords for when you fail tests in the Abyss https://github.com/ergonomy-joe/u4-decompiled/blob/c2c2108fa3bb346bcd1d8c207c526f33a4c8f5ef/SRC/U4_END.C#L37
        private static int UNKNOWN_EXIT_LOCATIONS_Y = 0xFEBA;

        private List<string> OriginalShrineText { get; set; }
        private List<int> OriginalShrineTextStartOffset { get; set; }
        public List<string> OriginalLBText { get; private set; }
        public List<int> OriginalLBTextStartOffset { get; private set; }

    }
}

