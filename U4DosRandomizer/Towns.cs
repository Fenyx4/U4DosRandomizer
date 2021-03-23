using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class Towns
    {
        public Dictionary<string, Town> towns = new Dictionary<string, Town>();

        public Towns(SpoilerLog spoilerLog)
        {
            SpoilerLog = spoilerLog;
        }

        private SpoilerLog SpoilerLog { get; }

        public void Load(string path, UltimaData data)
        {
            var files = Directory.GetFiles(path, "*.ULT");
            foreach (var file in files)
            {
                FileHelper.TryBackupOriginalFile(file, false);

                using (var twnStream = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open))
                {

                    Town town = new Town(twnStream, data);

                    towns.Add(Path.GetFileNameWithoutExtension(file), town);
                }
            }
        }

        public void Save(string path)
        {
            foreach (var townName in towns.Keys)
            {
                var townBytes = new List<byte>();
                townBytes.AddRange(towns[townName].Save());

                var file = Path.Combine(path, $"{townName.ToUpper()}.ULT");
                File.WriteAllBytes(file, townBytes.ToArray());
            }
        }

        public void Update(UltimaData ultimaData, Flags flags)
        {
            if(flags.Fixes)
            {
                towns["SERPENT"].npcConversationIdx[28] = 02;
                towns["SERPENT"].npcConversationIdx[29] = 02;
                SpoilerLog.Add(SpoilerCategory.Fix, $"Serpent's Hold gate guard fix");
            }
        }

        public static void Restore(string path)
        {
            var files = Directory.GetFiles(path, "*.ULT");
            foreach (var file in files)
            {
                FileHelper.Restore(file);
            }
        }
    }
}
