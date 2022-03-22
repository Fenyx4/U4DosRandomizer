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
            IMazeGenerator wipeDungeons = new WipeMazeGenerator();
            foreach (var dungeonName in dungeons.Keys)
            {
                wipeDungeons.GenerateMaze(dungeonName, dungeons[dungeonName], 8, 8, 8, random);
            }

            //var hsDungeons = new HitomezashiStitchMazeGenerator();
            //foreach (var dungeonName in dungeons.Keys)
            //{
            //    hsDungeons.GenerateMaze(dungeonName, dungeons[dungeonName], 8, 8, 8, random);
            //}

            // Other stones
            if (flags.Dungeon == 3)
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

            if (flags.Dungeon == 2)
            {
                var noDungeons = new NoDungeonGenerator();
                foreach (var dungeonName in dungeons.Keys)
                {
                    noDungeons.GenerateMaze(dungeonName, dungeons[dungeonName], 8, 8, 8, random);
                }
            }
            //var wilson = new WilsonMazeGenerator();
            ////TODO - Do something here
            //foreach( var dungeon in dungeons.Values)
            //{
            //    wilson.GenerateMaze(dungeon, 8, 8, 8, random);
            //}

            AddLadders("COVETOUS", dungeons["COVETOUS"], 8, 8, 8, random);
        }

        private void AddLadders(string dungeonName, Dungeon dungeon, int numLevels, int width, int height, Random random)
        {
            dungeon.SetTile(0, 1, 1, DungeonTileInfo.LadderUp);
            for(int l = 0; l < numLevels-1; l++)
            {
                var possibleLocations = new List<DungeonTile>();
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var tile = dungeon.GetTile(l, x, y);
                        var underTile = dungeon.GetTile(l+1, x, y);
                        if ((tile.GetTile() == DungeonTileInfo.Nothing || tile.GetTile() == DungeonTileInfo.LadderUp) && underTile.GetTile() == DungeonTileInfo.Nothing)
                        {
                            possibleLocations.Add(tile);
                        }
                    }
                }

                //Filter possible locations. Only allow ones that can reach a ladder up.
                var filteredPossibleLocations = new List<DungeonTile>();
                foreach( var location in possibleLocations)
                {
                    var path = Search.GetPath(dungeon.GetWidth(), dungeon.GetHeight(), location,
                        c => { return c.GetTile() == DungeonTileInfo.LadderUp; },
                        c => { return (c.GetTile() == DungeonTileInfo.LadderUp || c.GetTile() == DungeonTileInfo.Nothing) && ((DungeonTile)c).L == l; });
                    if( path.Count > 0 )
                    {
                        filteredPossibleLocations.Add(location);
                    }
                }

                var evenlyDistributedLadderOptions = GetEvenlyDistributed(random, random.Next(2)+1, dungeon, filteredPossibleLocations, 
                    (a, b) => {
                            var path = Search.GetPath(dungeon.GetWidth(), dungeon.GetHeight(), a,
                             a => { return a.GetTile() == DungeonTileInfo.LadderUp || a.Equals(b); },
                             a => { return (a.GetTile() == DungeonTileInfo.LadderUp || a.GetTile() == DungeonTileInfo.Nothing) && ((DungeonTile)a).L == l; });
                            return path.Count == 0 ? int.MaxValue : path.Count;
                        },
                    8);
                foreach (var ladder in evenlyDistributedLadderOptions)
                {
                    if(ladder.GetTile() == DungeonTileInfo.LadderUp || ladder.GetTile() == DungeonTileInfo.LadderBoth)
                    {
                        ladder.SetTile(DungeonTileInfo.LadderBoth);
                    }
                    else
                    {
                        ladder.SetTile(DungeonTileInfo.LadderDown);
                    }
                    dungeon.GetTile(ladder.L+1, ladder.X, ladder.Y).SetTile(DungeonTileInfo.LadderUp);
                }
            }
        }

        public delegate int GetDistance(ITile a, ITile b);

        private List<DungeonTile> GetEvenlyDistributed(Random random, int totalResults, Dungeon dungeon, List<DungeonTile> possibleLocations, GetDistance distanceCalc, int numCandidates = 10)
        {
            // Using Mitchell's best-candidate algorithm - https://bost.ocks.org/mike/algorithms/
            var results = new List<DungeonTile>();

            int randomIdx = random.Next(0, possibleLocations.Count);
            var original = possibleLocations[randomIdx];

            var usedLocations = new List<ITile>();

            results.Add(original);
            usedLocations.Add(original);

            for (int i = 1; i < totalResults; i++)
            {
                DungeonTile bestCandidate = null;
                var bestDistance = 0;
                
                for (int sample = 0; sample < numCandidates; sample++)
                {

                    randomIdx = random.Next(0, possibleLocations.Count);
                    DungeonTile selection = possibleLocations[randomIdx];

                    var distance = distanceCalc(FindClosest(selection, usedLocations, distanceCalc), selection);

                    if (distance > bestDistance)
                    {
                        bestDistance = distance;
                        bestCandidate = selection;
                    }
                }

                var result = bestCandidate;
                if (result != null)
                {
                    results.Add(result);
                    usedLocations.Add(result);
                }
            }

            return results;
        }

        private ITile FindClosest(ITile selection, List<ITile> locations, GetDistance distanceCalc)
        {
            ITile closest = null;
            var closestDistance = int.MaxValue;
            for (int i = 0; i < locations.Count(); i++)
            {
                var distance = distanceCalc(selection, locations[i]);
                if (distance >= 0 && distance < closestDistance)
                {
                    closest = locations[i];
                    closestDistance = distance;
                }
            }

            return closest;
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
