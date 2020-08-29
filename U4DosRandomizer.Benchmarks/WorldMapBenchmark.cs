using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace U4DosRandomizer.Benchmarks
{
    public class WorldMapBenchmark
    {
        WorldMap worldMap = null;
        Tile loc = null;
        public WorldMapBenchmark()
        {
            var worldMapDS = new DiamondSquare(WorldMap.SIZE, 184643518.256878, 82759876).getData();
            worldMap = new WorldMap();
            worldMap.Load("C:\\Program Files (x86)\\GOG Galaxy\\Games\\Ultima 4", worldMapDS);
            var possible = worldMap.GetAllMatchingTiles(c => { return c.GetTile() == TileInfo.Mountains; });
            loc = possible[78];
        }

        [Benchmark]
        public List<Tile> GetRiver() => worldMap.GetRiverPath(loc, c => { return c.GetTile() == TileInfo.Deep_Water; });

        [Benchmark]
        public List<Tile> GetRiverNew() => worldMap.GetRiverPath(loc, c => { return c.GetTile() == TileInfo.Deep_Water; });
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<WorldMapBenchmark>();
        }
    }
}
