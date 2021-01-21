using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using U4DosRandomizer.Helpers;
using U4DosRandomizer.Resources;

namespace U4DosRandomizer
{
    public class Dungeons
    {
        public Dictionary<string, Dungeon> dungeons = new Dictionary<string, Dungeon>();

        public Dungeons(SpoilerLog spoilerLog)
        {
            SpoilerLog = spoilerLog;
        }

        private SpoilerLog SpoilerLog { get; }

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

        public void Load(string path, UltimaData data, Flags flags)
        {
            var files = Directory.GetFiles(path, "*.DNG");
            foreach (var file in files)
            {
                if (!file.Contains("CAMP.DNG"))
                {
                    FileHelper.TryBackupOriginalFile(file, false);

                    if (flags.FixHythloth && file.Contains("HYTHLOTH"))
                    {
                        using (var deltaStream = new MemoryStream(Patches.HYTHLOTH))
                        {
                            Dungeon dungeon = new Dungeon(deltaStream, data);

                            dungeons.Add(Path.GetFileNameWithoutExtension(file), dungeon);
                        }

                        SpoilerLog.Add(SpoilerCategory.Fix, "Hythloth lvl 6 fixed.");
                    }
                    else
                    {
                        using (var dngStream = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open))
                        {

                            Dungeon dungeon = new Dungeon(dngStream, data);

                            dungeons.Add(Path.GetFileNameWithoutExtension(file), dungeon);
                        }
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

        public void Randomize(Random random, Flags flags)
        {
            // Other stones
            if (flags.DngStone)
            {
                foreach (var dungeonName in dungeons.Keys)
                {
                    if (dungeonName.ToLower() != "abyss" && dungeonName.ToLower() != "hythloth")
                    {
                        var dungeon = dungeons[dungeonName];
                        var stones = dungeon.GetTiles(DungeonTileInfo.AltarOrStone);
                        foreach (var stone in stones)
                        {
                            stone.SetTile(DungeonTileInfo.Nothing);
                        }
                        var possibleDungeonLocations = dungeon.GetTiles(DungeonTileInfo.Nothing);
                        var dungeonLoc = possibleDungeonLocations[random.Next(0, possibleDungeonLocations.Count - 1)];
                        dungeonLoc.SetTile(DungeonTileInfo.AltarOrStone);
                        SpoilerLog.Add(SpoilerCategory.Dungeon, $"{dungeonName} stone at Level {dungeonLoc.L} - {dungeonLoc.X},{dungeonLoc.Y}");
                    }
                }
            }
        }

        public void Update(UltimaData ultimaData, Flags flags)
        {
            
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
