using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class Party
    {
        private byte[] partyBytes;
        private const string filename = "PARTY.NEW";

        public void Load(string path, UltimaData data)
        {
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            using (var partyStream = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open))
            {
                partyBytes = partyStream.ReadAllBytes();
            }

            for(int i = 0; i < 8; i++)
            {
                var character = new Character();
                character.Hp = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_HP_OFFSET);
                character.MaxHp = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_MAX_HP_OFFSET);
                character.XP = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_XP_OFFSET);
                character.Str = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_STR_OFFSET);
                character.Dex = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_DEX_OFFSET);
                character.Int = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_INT_OFFSET);
                character.Mp = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_MP_OFFSET);
                character.Weapon = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_WEAPON_OFFSET);
                character.Armor = BitConverter.ToUInt16(partyBytes, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_ARMOR_OFFSET);

                var nameTextBytes = new List<byte>();
                var textOffset = CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_NAME_OFFSET;
                for (; partyBytes[textOffset] != 0x00; textOffset++)
                {
                    nameTextBytes.Add(partyBytes[textOffset]);
                }
                character.Name = System.Text.Encoding.Default.GetString(nameTextBytes.ToArray());
                nameTextBytes.Clear();

                character.Sex = (partyBytes[CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_SEX_OFFSET] == 0xb ? 'M' : 'F');
                character.Class = partyBytes[CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_CLASS_OFFSET];
                character.Status = (char)partyBytes[CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_STATUS_OFFSET];

                data.StartingCharacters.Add(character);
            }

            data.StartingFood = BitConverter.ToUInt32(partyBytes, FOOD_OFFSET);
            data.StartingGold = BitConverter.ToUInt16(partyBytes, GOLD_OFFSET);
            data.StartingItems = BitConverter.ToUInt16(partyBytes, ITEMS1_OFFSET);
            for (int offset = 0; offset < 4; offset++)
            {
                data.StartingEquipment.Add(BitConverter.ToUInt16(partyBytes, EQUIPMENT_OFFSET + (offset * 2)));
            }
            for (int offset = 0; offset < 8; offset++)
            {
                data.StartingArmor.Add(BitConverter.ToUInt16(partyBytes, ARMOR_OFFSET + (offset * 2)));
            }
            for (int offset = 0; offset < 16; offset++)
            {
                data.StartingWeapons.Add(BitConverter.ToUInt16(partyBytes, WEAPONS_OFFSET + (offset * 2)));
            }
            for (int offset = 0; offset < 8; offset++)
            {
                data.StartingReagents.Add(BitConverter.ToUInt16(partyBytes, REAGENTS_OFFSET + (offset * 2)));
            }
            for (int offset = 0; offset < 26; offset++)
            {
                data.StartingMixtures.Add(BitConverter.ToUInt16(partyBytes, MIXTURES_OFFSET + (offset * 2)));
            }

            data.StartingStones = partyBytes[STONES_OFFSET];
            data.StartingRunes = partyBytes[RUNES_OFFSET];
        }

        //public Dictionary<string, string> ReadHashes()
        //{
        //    var file = Path.Combine("hashes", "title_hash.json");
        //    var hashJson = System.IO.File.ReadAllText(file);

        //    var hashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(hashJson);

        //    return hashes;
        //}

        //public void WriteHashes(string path)
        //{
        //    var file = Path.Combine(path, filename);

        //    var townTalkHash = new Dictionary<string, string>();

        //    var hash = HashHelper.GetHashSha256(file);
        //    Console.WriteLine($"{file}: {HashHelper.BytesToString(hash)}");
        //    townTalkHash.Add(Path.GetFileName(file), HashHelper.BytesToString(hash));

        //    string json = JsonConvert.SerializeObject(townTalkHash); // the dictionary is inside client object
        //                                                             //write string to file
        //    System.IO.File.WriteAllText(@"party_hash.json", json);
        //}

        public void Update(UltimaData data)
        {
            for (int i = 0; i < data.StartingCharacters.Count; i++)
            {
                var character = data.StartingCharacters[i];
                partyBytes.OverwriteBytes(character.Hp, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_HP_OFFSET);
                partyBytes.OverwriteBytes(character.MaxHp, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_MAX_HP_OFFSET);
                partyBytes.OverwriteBytes(character.XP, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_XP_OFFSET);
                partyBytes.OverwriteBytes(character.Str, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_STR_OFFSET);
                partyBytes.OverwriteBytes(character.Dex, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_DEX_OFFSET);
                partyBytes.OverwriteBytes(character.Int, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_INT_OFFSET);
                partyBytes.OverwriteBytes(character.Mp, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_MP_OFFSET);
                partyBytes.OverwriteBytes(character.Weapon, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_WEAPON_OFFSET);
                partyBytes.OverwriteBytes(character.Armor, CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_ARMOR_OFFSET);

                var textBytes = Encoding.ASCII.GetBytes(character.Name);
                if (textBytes.Length > 16)
                {
                    throw new Exception($"Part name {character.Name} is too long.");
                }
                int j;
                for (j = 0; j < textBytes.Length; j++)
                {
                    partyBytes[CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_NAME_OFFSET + j] = textBytes[j];
                }
                partyBytes[CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_NAME_OFFSET + j] = 0x00;

                partyBytes[CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_SEX_OFFSET] = (character.Sex == 'M' ? (byte)0xb : (byte)0xc);

                partyBytes[CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_CLASS_OFFSET] = character.Class;
                partyBytes[CHARACTER_RECORDS_OFFSET + (i * CHAR_RECORD_LENGTH) + CHARACTER_STATUS_OFFSET] = (byte)character.Status;
            }

            var foodBytes = BitConverter.GetBytes(data.StartingFood);
            for (int offset = 0; offset < foodBytes.Length; offset++)
            {
                partyBytes[FOOD_OFFSET + offset] = foodBytes[offset];
            }

            partyBytes.OverwriteBytes(data.StartingGold, GOLD_OFFSET);
            partyBytes.OverwriteBytes(data.StartingItems, ITEMS1_OFFSET);

            partyBytes.OverwriteBytes(data.StartingEquipment, EQUIPMENT_OFFSET);
            partyBytes.OverwriteBytes(data.StartingArmor, ARMOR_OFFSET);
            partyBytes.OverwriteBytes(data.StartingWeapons, WEAPONS_OFFSET);
            partyBytes.OverwriteBytes(data.StartingReagents, REAGENTS_OFFSET);
            partyBytes.OverwriteBytes(data.StartingMixtures, MIXTURES_OFFSET);

            partyBytes[STONES_OFFSET] = data.StartingStones;
            partyBytes[RUNES_OFFSET] = data.StartingRunes;
        }

        public void Save(string path)
        {
            var file = Path.Combine(path, filename);
            using (var titleOut = new System.IO.BinaryWriter(new System.IO.FileStream(file, System.IO.FileMode.Truncate)))
            {
                titleOut.Write(partyBytes);
            }
        }

        public static int CHAR_RECORD_LENGTH = 39;
        public static int CHARACTER_HP_OFFSET = 0x0;
        public static int CHARACTER_MAX_HP_OFFSET = 0x2;
        public static int CHARACTER_XP_OFFSET = 0x4;
        public static int CHARACTER_STR_OFFSET = 0x6;
        public static int CHARACTER_DEX_OFFSET = 0x8;
        public static int CHARACTER_INT_OFFSET = 0xA;
        public static int CHARACTER_MP_OFFSET = 0xC;
        public static int CHARACTER_WEAPON_OFFSET = 0x10;
        public static int CHARACTER_ARMOR_OFFSET = 0x12;
        public static int CHARACTER_NAME_OFFSET = 0x14;
        public static int CHARACTER_SEX_OFFSET = 0x24;
        public static int CHARACTER_CLASS_OFFSET = 0x25;
        public static int CHARACTER_STATUS_OFFSET = 0x26;

        public static int CHARACTER_RECORDS_OFFSET = 0x8;
        public static int FOOD_OFFSET = 0x140;
        public static int GOLD_OFFSET = 0x144;
        public static int EQUIPMENT_OFFSET = 0x156;
        public static int ARMOR_OFFSET = 0x15E;
        public static int WEAPONS_OFFSET = 0x16E;
        public static int REAGENTS_OFFSET = 0x18E;
        public static int MIXTURES_OFFSET = 0x19E;
        public static int ITEMS1_OFFSET = 0x1D2;
        public static int ITEMS2_OFFSET = 0x1D3;
        public static int STONES_OFFSET = 0x1D6;
        public static int RUNES_OFFSET = 0x1D7;

        public static byte[] AllowedWeaponsMask = {
	            /*weapons class masks*/
	            0xFF,0xFF,0xFF,0xFF,0x7F,0x6F,0x6F,0x7E,0x7E,0xFF,0x2C,0x0C,0x2E,0x5E,0xD0,0xFF };
        public static byte[] AllowedArmorMask = {
	            /*D_2344*//*armor class masks*/
	            0xFF,0xFF,0x7F,0x2C,0x2C,0x24,0x04,0xFF
            };

        public Party(SpoilerLog spoilerLog)
        {
            SpoilerLog = spoilerLog;
        }

        private SpoilerLog SpoilerLog { get; }

        internal static void Restore(string path)
        {
            var file = Path.Combine(path, filename);
            FileHelper.Restore(file);
        }
    }
}