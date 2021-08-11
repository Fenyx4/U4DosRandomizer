using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class Dungeons
    {
        public Dictionary<string, Dungeon> dungeons = new Dictionary<string, Dungeon>();
        //private static Dictionary<string, int> dungeonIdx = new Dictionary<string, int>()
        //{
        //    { "deceit", 0 },
        //    { "despise", 1 },
        //    { "destard", 2 },
        //    { "wrong", 3 },
        //    { "covetous", 4 },
        //    { "shame", 5 },
        //    { "hythloth", 6 },
        //    { "abyss", 7 }
        //};

        public void Load(string path, UltimaData data)
        {
            var files = Directory.GetFiles(path, "*.DNG");
            foreach (var file in files)
            {
                if (!file.Contains("CAMP.DNG"))
                {
                    FileHelper.TryBackupOriginalFile(file, false);

                    using (var dngStream = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open))
                    {

                        Dungeon dungeon = new Dungeon(dngStream, data);

                        dungeons.Add(Path.GetFileNameWithoutExtension(file), dungeon);
                    }
                }
            }
        }

        public void Save(string path)
        {
            foreach (var dungeonName in dungeons.Keys)
            {
                var dungeonBytes = new List<byte>();
                dungeonBytes.AddRange(dungeons[dungeonName].Save());

                var file = Path.Combine(path, $"{dungeonName.ToUpper()}.DNG");
                File.WriteAllBytes(file, dungeonBytes.ToArray());
            }
        }

        public void Update(UltimaData ultimaData, int seed)
        {
            var random = new Random(seed);
            var wilson = new WilsonMazeGenerator();
            //TODO - Do something here
            foreach( var dungeon in dungeons.Values)
            {
                wilson.GenerateMaze(dungeon, 8, 8, 8, random);
            }
        }

        public static void Restore(string path)
        {
            var files = Directory.GetFiles(path, "*.DNG");
            foreach (var file in files)
            {
                if (!file.Contains("CAMP.DNG"))
                {
                    FileHelper.Restore(file);
                }
            }
        }
    }
}
