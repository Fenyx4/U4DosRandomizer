using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace U4DosRandomizer.Benchmarks
{
    public class WorldMapBenchmark
    {
        WorldMap worldMap = null;
        public WorldMapBenchmark()
        {
            var worldMapDS = new DiamondSquare(WorldMap.SIZE, 184643518.256878, 82759876).getData();
            worldMap = new WorldMap();
            worldMap.Load("C:\\Program Files (x86)\\GOG Galaxy\\Games\\Ultima 4", worldMapDS);
        }

        [Benchmark]
        public SixLabors.ImageSharp.Image IndividualPixels() => worldMap.ToImage();

        //[Benchmark]
        //public SixLabors.ImageSharp.Image RowSpan() => worldMap.ToBitmapRowSpan();
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<WorldMapBenchmark>();
        }
    }
}
