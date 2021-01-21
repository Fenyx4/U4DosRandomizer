using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class WorldMapUnchanged : WorldMapAbstract, IWorldMap
    {
        private SpoilerLog SpoilerLog { get; }

        public WorldMapUnchanged(SpoilerLog spoilerLog)
        {
            this.SpoilerLog = spoilerLog;
        }

        public override void Load(string path, int mapSeed, Random mapGeneratorSeed, Random randomMap)
        {
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            _worldMapTiles = new byte[SIZE, SIZE];

            int chunkwidth = 32;
            int chunkSize = chunkwidth * chunkwidth;
            byte[] chunk; // = new byte[chunkSize];
            using (var worldMap = new System.IO.BinaryReader(new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open)))
            {
                for (int chunkCount = 0; chunkCount < 64; chunkCount++)
                {
                    chunk = worldMap.ReadBytes(chunkSize);

                    // Copy the chunk over
                    for (int i = 0; i < chunkSize; i++)
                    {
                        _worldMapTiles[i % chunkwidth + chunkCount % 8 * chunkwidth, i / chunkwidth + chunkCount / 8 * chunkwidth] = chunk[i];
                    }
                }
            }
        }

        public override void Randomize(UltimaData ultimaData, Random random1, Random random2)
        {
            SpoilerLog.Add(SpoilerCategory.Location, "Locations unchanged");
            return;
        }
    }
}
