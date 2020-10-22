using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class WorldMapShuffleLocations : WorldMapAbstract, IWorldMap
    {

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
            // Towns & Villages & starting locations
            for (int i = 0; i < ultimaData.Towns.Count; i++)
            {
                var val = random1.Next(0, (ultimaData.Towns.Count - 1) - i);
                Swap(ultimaData.Towns, val, (ultimaData.Towns.Count - 1) - i);
            }
            // Move starting positions and abyss ejection locations to Towns
            for (int i = 0; i < 8; i++)
            {
                var validPositions = GetPathableTilesNear(ultimaData.Towns[i], 3, IsWalkable);
                var loc = validPositions[random1.Next(0, validPositions.Count)];
                ultimaData.StartingPositions[i].X = loc.X;
                ultimaData.StartingPositions[i].Y = loc.Y;
                ultimaData.AbyssEjectionLocations[i].X = loc.X;
                ultimaData.AbyssEjectionLocations[i].Y = loc.Y;
            }

            // Castles
            for (int i = 0; i < ultimaData.Castles.Count; i++)
            {
                var val = random1.Next(0, (ultimaData.Castles.Count - 1) - i);
                Swap(ultimaData.Castles, val, (ultimaData.Castles.Count - 1) - i);
                Swap(ultimaData.AbyssEjectionLocations, val+8, (8 + ultimaData.Castles.Count - 1) - i);
            }

            // Shrines
            // Don't move humility
            for (int i = 0; i < ultimaData.Shrines.Count - 1; i++)
            {
                var val = random1.Next(0, (ultimaData.Shrines.Count - 2) - i);
                Swap(ultimaData.Shrines, val, (ultimaData.Shrines.Count - 2) - i);
            }

            // Moongates
            for (int i = 0; i < ultimaData.Moongates.Count; i++)
            {
                var val = random1.Next(0, (ultimaData.Moongates.Count - 1) - i);
                Swap(ultimaData.Moongates, val, (ultimaData.Moongates.Count - 1) - i);
            }

            // Dungeons
            // Don't move Abyss
            for (int i = 0; i < ultimaData.Dungeons.Count - 1; i++)
            {
                var val = random1.Next(0, (ultimaData.Dungeons.Count - 2) - i);
                Swap(ultimaData.Dungeons, val, (ultimaData.Dungeons.Count - 2) - i);
            }

            // Items
            var swappableItems = new List<int>() { ultimaData.ITEM_MANDRAKE, ultimaData.ITEM_HORN, ultimaData.ITEM_MANDRAKE2, ultimaData.ITEM_NIGHTSHADE, ultimaData.ITEM_NIGHTSHADE2, ultimaData.ITEM_SKULL, ultimaData.ITEM_BELL, ultimaData.ITEM_WHEEL, ultimaData.ITEM_BLACK_STONE, ultimaData.ITEM_WHITE_STONE };
            for (int i = 0; i < 10; i++)
            {
                var val = random1.Next(0, (10 - 1) - i);
                Swap(ultimaData.Items, val, (10 - 1) - i);
            }

            return;
        }

        private void Swap(ReadOnlyCollection<Item> locations, int a, int b)
        {
            var x = locations[a].X;
            var y = locations[a].Y;
            var loc = locations[a].Location;

            locations[a].X = locations[b].X;
            locations[a].Y = locations[b].Y;
            locations[a].Location = locations[b].Location;

            locations[b].X = x;
            locations[b].Y = y;
            locations[b].Location = loc;

        }
        private void Swap(ReadOnlyCollection<TileDirtyWrapper> locations, int a, int b)
        {
            var x = locations[a].X;
            var y = locations[a].Y;
            var tile = locations[a].GetTile();

            locations[a].SetTile(locations[b].GetTile());
            locations[a].X = locations[b].X;
            locations[a].Y = locations[b].Y;

            locations[b].SetTile(tile);
            locations[b].X = x;
            locations[b].Y = y;
        }

        private void Swap(ReadOnlyCollection<Tile> locations, int a, int b)
        {
            var x = locations[a].X;
            var y = locations[a].Y;
            var tile = locations[a].GetTile();

            locations[a].SetTile(locations[b].GetTile());
            locations[a].X = locations[b].X;
            locations[a].Y = locations[b].Y;

            locations[b].SetTile(tile);
            locations[b].X = x;
            locations[b].Y = y;
        }

        private void Swap(List<Coordinate> locations, int a, int b)
        {
            var x = locations[a].X;
            var y = locations[a].Y;

            locations[a].X = locations[b].X;
            locations[a].Y = locations[b].Y;

            locations[b].X = x;
            locations[b].Y = y;
        }
    }
}
