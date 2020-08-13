using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace U4DosRandomizer
{
    public class Title
    {
        private SHA256 Sha256 = SHA256.Create();
        private byte[] titleBytes;
        private const string filename = "TITLE.EXE";

        public void Load(UltimaData data)
        {
            //WriteHashes();
            var hashes = ReadHashes();

            var file = $"ULT\\{filename}";

            var hash = HashHelper.GetHashSha256(file);
            if (hashes[filename] == HashHelper.BytesToString(hash))
            {
                File.Copy(file, $"{file}.orig", true);
            }
            else
            {
                hash = HashHelper.GetHashSha256($"{file}.orig");
                if (hashes[filename] != HashHelper.BytesToString(hash))
                {
                    throw new FileNotFoundException($"Original version of {file} not found.");
                }
            }

            var titleStream = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open);
            titleBytes = titleStream.ReadAllBytes();

            for (int offset = 0; offset < 8; offset++)
            {
                data.StartingPositions.Add(new Coordinate(titleBytes[START_X_OFFSET + offset], titleBytes[START_Y_OFFSET + offset]));
            }
        }

        public Dictionary<string, string> ReadHashes()
        {
            var hashJson = System.IO.File.ReadAllText("hashes\\title_hash.json");

            var hashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(hashJson);

            return hashes;
        }

        public void WriteHashes()
        {
            var file = $"ULT\\{filename}";

            var townTalkHash = new Dictionary<string, string>();

            var hash = HashHelper.GetHashSha256(file);
            Console.WriteLine($"{file}: {HashHelper.BytesToString(hash)}");
            townTalkHash.Add(Path.GetFileName(file), HashHelper.BytesToString(hash));

            string json = JsonConvert.SerializeObject(townTalkHash); // the dictionary is inside client object
                                                                     //write string to file
            System.IO.File.WriteAllText(@"title_hash.json", json);
        }

        public void Update(UltimaData data)
        {
            for (int offset = 0; offset < 8; offset++)
            {
                titleBytes[START_X_OFFSET + offset] = data.StartingPositions[offset].X;
                titleBytes[START_Y_OFFSET + offset] = data.StartingPositions[offset].Y;
            }
        }

        public void Save()
        {
            var titleOut = new System.IO.BinaryWriter(new System.IO.FileStream($"ULT\\{filename}", System.IO.FileMode.Truncate));

            titleOut.Write(titleBytes);

            titleOut.Close();
        }

        public static int START_X_OFFSET = 0x70dc;
        public static int START_Y_OFFSET = 0x70e4;
    }
}
