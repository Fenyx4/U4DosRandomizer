using System;
using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace U4DosRandomizer.Benchmarks
{
    public class WorldMapBenchmark
    {
        //WorldMap worldMap = null;
        //public WorldMapBenchmark()
        //{
        //    var worldMapDS = new DiamondSquare(WorldMap.SIZE, 184643518.256878, 82759876).getData();
        //    worldMap = new WorldMap();
        //    worldMap.Load("C:\\Program Files (x86)\\GOG Galaxy\\Games\\Ultima 4", worldMapDS);
        //}

        //[Benchmark]
        //public SixLabors.ImageSharp.Image IndividualPixels() => worldMap.ToImage();

        ////[Benchmark]
        ////public SixLabors.ImageSharp.Image RowSpan() => worldMap.ToBitmapRowSpan();
        ///
    }

    public class WrapBenchmark
    {
        public static byte WrapBefore(int input, int divisor)
        {
            return Convert.ToByte((input % divisor + divisor) % divisor);
        }
        public static int WrapAfter(int input, int divisor)
        {
            return (input % divisor + divisor) % divisor;
        }

        public static void WrapBeforeAlot()
        {
            Random random = new Random(1337);
            for(int i = 0; i < 1024*4*2; i++)
            {
                WrapBefore(random.Next(), 256);
            }
        }

        public static void WrapAfterAlot()
        {
            Random random = new Random(1337);
            for (int i = 0; i < 1024*4*2; i++)
            {
                WrapAfter(random.Next(), 256);
            }
        }

        [Benchmark]
        public void WrapBefore() => WrapBeforeAlot();

        [Benchmark]
        public void WrapAfter() => WrapAfterAlot();
    }
    //public class SimplexBenchmark
    //{
    //    int SIZE = 256;
    //    public SimplexBenchmark()
    //    {
    //        var before = SeamlessSimplexNoise.simplexnoise(1337, SIZE, SIZE, 0.0f, 0.8f);
    //        var after = SeamlessSimplexNoiseAfter.simplexnoise(1337, SIZE, SIZE, 0.0f, 0.8f);

    //        for(int x = 0; x < SIZE; x++)
    //        {
    //            for(int y = 0; y < SIZE; y++)
    //            {
    //                if(before[x,y] != after[x,y])
    //                {
    //                    throw new Exception("Before and after are different.");
    //                }
    //            }
    //        }
    //    }

    //    [Benchmark]
    //    public float[,] SimplexBefore() => SeamlessSimplexNoise.simplexnoise(1337, SIZE, SIZE, 0.0f, 0.8f);

    //    [Benchmark]
    //    public float[,] SimplexAfter() => SeamlessSimplexNoiseAfter.simplexnoise(1337, SIZE, SIZE, 0.0f, 0.8f);
    //}

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<WrapBenchmark>();
        }
    }
}
