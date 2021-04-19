using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class WorldMapShuffleLocations : WorldMapAbstract, IWorldMap
    {
        private SpoilerLog SpoilerLog { get; }

        public WorldMapShuffleLocations(SpoilerLog spoilerLog)
        {
            this.SpoilerLog = spoilerLog;
        }

        public override void Load(string path, int mapSeed, int mapGeneratorSeed, int otherRandomSeed)
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

            //RemoveAvatarIsle();
        }

        public override void Randomize(UltimaData ultimaData, Random random1, Random random2)
        {
            var swapPool = new List<Tuple<TileDirtyWrapper, ICoordinate, int>>();

            for (int i = 0; i < ultimaData.Castles.Count; i++)
            {
                //SpoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.Castles[i].Y:X2}");
                swapPool.Add(new Tuple<TileDirtyWrapper, ICoordinate, int>(ultimaData.Castles[i].Copy(this), ultimaData.AbyssEjectionLocations[8 + i], ultimaData.LOC_LYCAEUM + i));
            }

            for (int i = 0; i < ultimaData.Towns.Count; i++)
            {
                //SpoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.Towns[i].Y:X2}");
                if (i != 9 && i != 11)
                {
                    ICoordinate startingPosition = null;
                    if (i < 8)
                    {
                        startingPosition = ultimaData.StartingPositions[i];
                    }
                    else
                    {
                        // No starting positions for villages. Make one.
                        var validPositions = GetPathableTilesNear(ultimaData.Towns[i], 3, IsWalkable);
                        startingPosition = validPositions[random1.Next(0, validPositions.Count)];
                    }
                    swapPool.Add(new Tuple<TileDirtyWrapper, ICoordinate, int>(ultimaData.Towns[i].Copy(this), startingPosition, ultimaData.LOC_TOWNS + i));
                }
            }

            //for (int i = 0; i < swapPool.Count; i++)
            //{
            //    SpoilerLog.Add(SpoilerCategory.Location, $"{swapPool[i].Item1.Y:X2}");
            //}

            for (int i = 0; i < swapPool.Count; i++)
            {
                var val = random1.Next(0, (swapPool.Count - 1) - i);
                Swap(swapPool, val, (swapPool.Count - 1) - i);
            }

            for (int i = 0; i < swapPool.Count; i++)
            {
                if(i < ultimaData.Castles.Count)
                {
                    ultimaData.Castles[i].X = swapPool[i].Item1.X;
                    ultimaData.Castles[i].Y = swapPool[i].Item1.Y;
                    ultimaData.Castles[i].SetTile(0x0B);
                    ultimaData.AbyssEjectionLocations[8 + i].X = swapPool[i].Item2.X;
                    ultimaData.AbyssEjectionLocations[8 + i].Y = swapPool[i].Item2.Y;

                    SpoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.LocationNames[i + ultimaData.LOC_LYCAEUM - 1]} moved to {ultimaData.LocationNames[swapPool[i].Item3-1]}");
                }
                else
                {
                    int townIdx = i-3;
                    if(i >= 12)
                    {
                        townIdx++;
                    }
                    if (i >= 13)
                    {
                        townIdx++;
                    }
                    byte tile = (byte)(townIdx > 8 ? 0x0C : 0x0A);
                    if(townIdx == 7)
                    {
                        tile = 0x1D;
                    }
                    ultimaData.Towns[townIdx].X = swapPool[i].Item1.X;
                    ultimaData.Towns[townIdx].Y = swapPool[i].Item1.Y;
                    ultimaData.Towns[townIdx].SetTile(tile);
                    if (townIdx < 8)
                    {
                        ultimaData.AbyssEjectionLocations[i].X = swapPool[i].Item2.X;
                        ultimaData.AbyssEjectionLocations[i].Y = swapPool[i].Item2.Y;
                        ultimaData.StartingPositions[townIdx].X = swapPool[i].Item2.X;
                        ultimaData.StartingPositions[townIdx].Y = swapPool[i].Item2.Y;
                    }
                    SpoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.LocationNames[townIdx + ultimaData.LOC_TOWNS - 1]} moved to {ultimaData.LocationNames[swapPool[i].Item3-1]}");
                }
                
            }

            // Shrines
            // Don't move humility
            var swapValues = new List<int>();
            for (int i = 0; i < ultimaData.Shrines.Count; i++)
            {
                swapValues.Add(i);
            }

            for (int i = 0; i < ultimaData.Shrines.Count - 1; i++)
            {
                var val = random1.Next(0, (ultimaData.Shrines.Count - 2) - i);
                Swap(ultimaData.Shrines, val, (ultimaData.Shrines.Count - 2) - i);
                Swap(swapValues, val, (ultimaData.Shrines.Count - 2) - i);
            }

            for (int i = 0; i < ultimaData.Shrines.Count; i++)
            {
                SpoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.LocationNames[i + ultimaData.LOC_SHRINES - 1]} shrine moved to {ultimaData.LocationNames[swapValues[i] + ultimaData.LOC_SHRINES - 1]}");
            }

            // Moongates
            swapValues = new List<int>();
            var originalNewMoonX = ultimaData.Moongates[0].X;
            var originalNewMoonY = ultimaData.Moongates[0].Y;
            for (int i = 0; i < ultimaData.Moongates.Count; i++)
            {
                swapValues.Add(i);
            }
            for (int i = 0; i < ultimaData.Moongates.Count; i++)
            {
                var val = random1.Next(0, (ultimaData.Moongates.Count - 1) - i);
                Swap(ultimaData.Moongates, val, (ultimaData.Moongates.Count - 1) - i);
                Swap(swapValues, val, (ultimaData.Moongates.Count - 1) - i);
            }
            for (int i = 0; i < ultimaData.Shrines.Count; i++)
            {
                SpoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.LocationNames[i + ultimaData.LOC_TOWNS - 1]} moongate moved to {ultimaData.LocationNames[swapValues[i] + ultimaData.LOC_TOWNS - 1]}");
            }

            // Dungeons
            // Don't move Abyss
            swapValues = new List<int>();
            for (int i = 0; i < ultimaData.Dungeons.Count; i++)
            {
                swapValues.Add(i);
            }
            for (int i = 0; i < ultimaData.Dungeons.Count - 1; i++)
            {
                var val = random1.Next(0, (ultimaData.Dungeons.Count - 2) - i);
                Swap(ultimaData.Dungeons, val, (ultimaData.Dungeons.Count - 2) - i);
                Swap(swapValues, val, (ultimaData.Dungeons.Count - 2) - i);
            }
            for (int i = 0; i < ultimaData.Dungeons.Count; i++)
            {
                SpoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.LocationNames[i + ultimaData.LOC_DUNGEONS - 1]} moved to {ultimaData.LocationNames[swapValues[i] + ultimaData.LOC_DUNGEONS - 1]}");
            }

            // Items
            swapValues = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                swapValues.Add(i);
            }
            for (int i = 0; i < 10; i++)
            {
                var val = random1.Next(0, (10 - 1) - i);
                Swap(ultimaData.Items, val, (10 - 1) - i);
                Swap(swapValues, val, (10 - 1) - i);
            }
            for (int i = 0; i < 10; i++)
            {
                SpoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.ItemNames[i]} moved to {ultimaData.ItemNames[swapValues[i]]}");
            }

            //Whatever is now at the original new moon moongate location gets moved to whereever the new moongate is now
            var item = ultimaData.Items.Single(x => x.X == originalNewMoonX && x.Y == originalNewMoonY && x.Location == 0x00);
            item.X = ultimaData.Moongates[0].X;
            item.Y = ultimaData.Moongates[0].Y;

            return;
        }

        private void RemoveAvatarIsle()
        {
            for (int x = 256 - 50; x < 256; x++)
            {
                for (int y = 256 - 100; y < 256; y++)
                {
                    _worldMapTiles[x,y] = TileInfo.Deep_Water;
                }
            }
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

        public static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
    }
}
