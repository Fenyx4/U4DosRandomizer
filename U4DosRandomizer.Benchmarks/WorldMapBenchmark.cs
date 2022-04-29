using System;
using System.Collections.Generic;
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

    public class DungeonsBenchmark
    {
        public static void FillOneFloor()
        {
            byte[,,] map = new byte[8, 8, 8];
            List<byte[]> rooms = new List<byte[]>();

            for (int l = 0; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        map[l, x, y] = DungeonTileInfo.Nothing;
                    }
                }
            }

            //map[0, 1, 1] = DungeonTileInfo.LadderUp;
            //map[0, 6, 6] = DungeonTileInfo.LadderDown;
            //map[1, 6, 6] = DungeonTileInfo.LadderUp;

            var dungeon = new Dungeon(map, rooms, null);

            Dungeons.Fill(dungeon.GetTile(0, 1, 1), dungeon);
        }

        public static void FillTwoFloors()
        {
            byte[,,] map = new byte[8, 8, 8];
            List<byte[]> rooms = new List<byte[]>();

            for (int l = 0; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        map[l, x, y] = DungeonTileInfo.Nothing;
                    }
                }
            }

            map[0, 1, 1] = DungeonTileInfo.LadderUp;
            map[0, 6, 6] = DungeonTileInfo.LadderDown;
            map[1, 6, 6] = DungeonTileInfo.LadderUp;

            var dungeon = new Dungeon(map, rooms, null);

            Dungeons.Fill(dungeon.GetTile(0, 1, 1), dungeon);
        }

        public static void FillAllFloors()
        {
            byte[,,] map = new byte[8, 8, 8];
            List<byte[]> rooms = new List<byte[]>();

            for (int l = 0; l < 8; l++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        map[l, x, y] = DungeonTileInfo.Nothing;
                    }
                }
            }

            map[0, 1, 1] = DungeonTileInfo.LadderUp;

            map[0, 6, 6] = DungeonTileInfo.LadderDown;
            map[1, 6, 6] = DungeonTileInfo.LadderUp;

            map[1, 5, 6] = DungeonTileInfo.LadderDown;
            map[2, 5, 6] = DungeonTileInfo.LadderUp;

            map[2, 4, 6] = DungeonTileInfo.LadderDown;
            map[3, 4, 6] = DungeonTileInfo.LadderUp;

            map[3, 6, 6] = DungeonTileInfo.LadderDown;
            map[4, 6, 6] = DungeonTileInfo.LadderUp;

            map[4, 5, 6] = DungeonTileInfo.LadderDown;
            map[5, 5, 6] = DungeonTileInfo.LadderUp;

            map[5, 4, 6] = DungeonTileInfo.LadderDown;
            map[6, 4, 6] = DungeonTileInfo.LadderUp;

            map[6, 6, 6] = DungeonTileInfo.LadderDown;
            map[7, 6, 6] = DungeonTileInfo.LadderUp;

            var dungeon = new Dungeon(map, rooms, null);

            Dungeons.Fill(dungeon.GetTile(0, 1, 1), dungeon);
        }

        [Benchmark]
        public void DungeonsFill() => FillOneFloor();

        //[Benchmark]
        //public void DungeonsFillTwoFloors() => FillTwoFloors();

        //[Benchmark]
        //public void DungeonsFillAllFloors() => FillAllFloors();
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
            //var summary = BenchmarkRunner.Run<WrapBenchmark>();
            var summary = BenchmarkRunner.Run<DungeonsBenchmark>();
        }
    }
}
