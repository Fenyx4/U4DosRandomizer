using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace U4DosRandomizer
{
    public class Avatar
    {
        private SHA256 Sha256 = SHA256.Create();
        private byte[] avatarBytes;

        public void Load(UltimaData data)
        {
            //WriteHashes();
            var hashes = ReadHashes();

            var file = "ULT\\AVATAR.EXE";

            var hash = HashHelper.GetHashSha256(file);
            if (hashes["AVATAR.EXE"] == HashHelper.BytesToString(hash))
            {
                File.Copy(file, $"{file}.orig", true);
            }
            else
            {
                hash = HashHelper.GetHashSha256($"{file}.orig");
                if (hashes["AVATAR.EXE"] != HashHelper.BytesToString(hash))
                {
                    throw new FileNotFoundException($"Original version of {file} not found.");
                }
            }

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
        }

        public Dictionary<string, string> ReadHashes()
        {
            var hashJson = System.IO.File.ReadAllText("hashes\\avatar_hash.json");

            var hashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(hashJson);

            return hashes;
        }

        public void WriteHashes()
        {
            var file = "ULT\\AVATAR.EXE";

            var townTalkHash = new Dictionary<string, string>();

            var hash = HashHelper.GetHashSha256(file);
            Console.WriteLine($"{file}: {HashHelper.BytesToString(hash)}");
            townTalkHash.Add(Path.GetFileName(file), HashHelper.BytesToString(hash));

            string json = JsonConvert.SerializeObject(townTalkHash); // the dictionary is inside client object
                                                                     //write string to file
            System.IO.File.WriteAllText(@"avatar_hash.json", json);
        }

        public void Update(UltimaData data)
        {
            for (var offset = 0; offset < 24; offset++)
            {
                avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5] = data.Items[offset].Location;
                avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5 + 1] = data.Items[offset].X;
                avatarBytes[ITEM_LOCATIONS_OFFSET + offset * 5 + 2] = data.Items[offset].Y;
            }

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
                if (offset != 6)
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
        }

        public void Save()
        {
            var avatarOut = new System.IO.BinaryWriter(new System.IO.FileStream("ULT\\AVATAR.EXE", System.IO.FileMode.OpenOrCreate));

            avatarOut.Write(avatarBytes);

            avatarOut.Close();
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

        private static int LOC_TOWNS = 0x05;
        private static int LOC_MOONGLOW = 0x05;
        private static int LOC_BRITAIN = 0x06;
        private static int LOC_JHELOM = 0x07;
        private static int LOC_YEW = 0x08;
        private static int LOC_MINOC = 0x09;
        private static int LOC_TRINSIC = 0x0a;
        private static int LOC_SKARA = 0x0b;
        public static int LOC_MAGINCIA = 0x0c;
        private static int LOC_PAWS = 0x0d;
        private static int LOC_DEN = 0x0e;
        private static int LOC_VESPER = 0x0f;
        private static int LOC_COVE = 0x10;

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
        private static int WHITE_STONE_LOCATION_TEXT = 0x17434;
        private static int BLACK_STONE_LOCATION_TEXT = 0x174F9;
    }
}

