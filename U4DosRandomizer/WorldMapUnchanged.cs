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

        public override void Load(string path, int mapSeed, int mapGeneratorSeed, int otherRandomSeed, UltimaData ultimaData)
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

        public override void Randomize(UltimaData ultimaData, Random random1, Random random2, Random random3)
        {
            SpoilerLog.Add(SpoilerCategory.Location, "Locations unchanged");

            // Used to output abyss shape
            //int xSize = 32 + 10 - 2;
            //int ySize = 64 + 17 - 1;

            //int xOffset = 256 - xSize - 2;
            //int yOffset = 256 - ySize - 1;

            //using (var worldFile = new System.IO.BinaryWriter(new System.IO.FileStream("abyssnew", System.IO.FileMode.OpenOrCreate)))
            //{             
            //    Console.Write(xSize);
            //    Console.Write(ySize);
            //    for (int y = 0; y < ySize; y++)
            //    {
            //        for (int x = 0; x < xSize; x++)
            //        {
            //            _worldMapTiles[x, y] = _worldMapTiles[x + xOffset, y + yOffset];
            //            worldFile.Write(_worldMapTiles[x, y]);
            //            Console.Write(_worldMapTiles[x, y]);
            //        }
            //        Console.WriteLine();
            //    }
            //}

            return;
        }
    }
}
