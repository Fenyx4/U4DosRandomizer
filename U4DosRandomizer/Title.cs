using System.IO;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class Title
    {
        private byte[] titleBytes;
        private const string filename = "TITLE.EXE";

        public void Load(string path, UltimaData data)
        {
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            var titleStream = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open);
            titleBytes = titleStream.ReadAllBytes();

            for (int offset = 0; offset < 8; offset++)
            {
                data.StartingPositions.Add(new Coordinate(titleBytes[START_X_OFFSET + offset], titleBytes[START_Y_OFFSET + offset]));
            }
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
        //    System.IO.File.WriteAllText(@"title_hash.json", json);
        //}

        public void Update(UltimaData data)
        {
            for (int offset = 0; offset < 8; offset++)
            {
                titleBytes[START_X_OFFSET + offset] = data.StartingPositions[offset].X;
                titleBytes[START_Y_OFFSET + offset] = data.StartingPositions[offset].Y;
            }
        }

        public void Save(string path)
        {
            var file = Path.Combine(path, filename);
            var titleOut = new System.IO.BinaryWriter(new System.IO.FileStream(file, System.IO.FileMode.Truncate));

            titleOut.Write(titleBytes);

            titleOut.Close();
        }

        public static int START_X_OFFSET = 0x70dc;
        public static int START_Y_OFFSET = 0x70e4;
    }
}
