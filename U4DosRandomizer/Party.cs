using System;
using System.Collections.Generic;
using System.IO;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class Party
    {
        private byte[] titleBytes;
        private const string filename = "PARTY.NEW";

        public void Load(string path, UltimaData data)
        {
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            using (var titleStream = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open))
            {
                titleBytes = titleStream.ReadAllBytes();
            }

            data.StartingFood = BitConverter.ToUInt32(titleBytes, FOOD_OFFSET);
            data.StartingGold = BitConverter.ToUInt16(titleBytes, GOLD_OFFSET);
            data.StartingItems = BitConverter.ToUInt16(titleBytes, ITEMS1_OFFSET);
            for (int offset = 0; offset < 4; offset++)
            {
                data.StartingEquipment.Add(BitConverter.ToUInt16(titleBytes, EQUIPMENT_OFFSET + (offset * 2)));
            }
            for (int offset = 0; offset < 8; offset++)
            {
                data.StartingArmor.Add(BitConverter.ToUInt16(titleBytes, ARMOR_OFFSET + (offset * 2)));
            }
            for (int offset = 0; offset < 16; offset++)
            {
                data.StartingWeapons.Add(BitConverter.ToUInt16(titleBytes, WEAPONS_OFFSET + (offset * 2)));
            }
            for (int offset = 0; offset < 8; offset++)
            {
                data.StartingReagents.Add(BitConverter.ToUInt16(titleBytes, REAGENTS_OFFSET + (offset * 2)));
            }
            for (int offset = 0; offset < 26; offset++)
            {
                data.StartingMixtures.Add(BitConverter.ToUInt16(titleBytes, MIXTURES_OFFSET + (offset * 2)));
            }

            data.StartingStones = titleBytes[STONES_OFFSET];
            data.StartingRunes = titleBytes[RUNES_OFFSET];
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
            var foodBytes = BitConverter.GetBytes(data.StartingFood);
            for (int offset = 0; offset < foodBytes.Length; offset++)
            {
                titleBytes[FOOD_OFFSET + offset] = foodBytes[offset];
            }

            var goldBytes = BitConverter.GetBytes(data.StartingGold);
            for (int offset = 0; offset < goldBytes.Length; offset++)
            {
                titleBytes[GOLD_OFFSET + offset] = goldBytes[offset];
            }

            var itemBytes = BitConverter.GetBytes(data.StartingItems);
            for (int offset = 0; offset < itemBytes.Length; offset++)
            {
                titleBytes[ITEMS1_OFFSET + offset] = itemBytes[offset];
            }

            titleBytes.OverwriteBytes(data.StartingEquipment, EQUIPMENT_OFFSET);
            titleBytes.OverwriteBytes(data.StartingArmor, ARMOR_OFFSET);
            titleBytes.OverwriteBytes(data.StartingWeapons, WEAPONS_OFFSET);
            titleBytes.OverwriteBytes(data.StartingReagents, REAGENTS_OFFSET);
            titleBytes.OverwriteBytes(data.StartingMixtures, MIXTURES_OFFSET);

            titleBytes[STONES_OFFSET] = data.StartingStones;
            titleBytes[RUNES_OFFSET] = data.StartingRunes;
        }

        public void Save(string path)
        {
            var file = Path.Combine(path, filename);
            using (var titleOut = new System.IO.BinaryWriter(new System.IO.FileStream(file, System.IO.FileMode.Truncate)))
            {
                titleOut.Write(titleBytes);
            }
        }

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

        internal static void Restore(string path)
        {
            var file = Path.Combine(path, filename);
            FileHelper.Restore(file);
        }
    }
}