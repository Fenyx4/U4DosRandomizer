using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using U4DosRandomizer.Resources;

namespace U4DosRandomizer
{
    public abstract class WorldMapAbstract : IWorldMap
    {
        protected const string filename = "WORLD.MAP";
        public const int SIZE = 256;
        protected byte[,] _worldMapTiles;
        protected byte[,] _clothMapTiles;

        public abstract void Load(string path, int v, int mapGeneratorSeed, int otherRandomSeed, UltimaData ultimaData);

        public abstract void Randomize(UltimaData ultimaData, Random random1, Random random2, Random random3);

        public void Save(string path)
        {
            var file = Path.Combine(path, filename);
            using (var worldFile = new System.IO.BinaryWriter(new System.IO.FileStream(file, System.IO.FileMode.OpenOrCreate)))
            {
                WriteMapToOriginalFormat(worldFile);
            }
        }

        private void WriteMapToOriginalFormat(System.IO.BinaryWriter worldFile)
        {
            int chunkwidth = 32;
            int chunkSize = chunkwidth * chunkwidth;
            byte[] chunk = new byte[chunkSize];

            for (int chunkCount = 0; chunkCount < 64; chunkCount++)
            {
                // Copy the chunk over
                for (int i = 0; i < chunkSize; i++)
                {
                    chunk[i] = _worldMapTiles[i % chunkwidth + chunkCount % 8 * chunkwidth, i / chunkwidth + chunkCount / 8 * chunkwidth];
                }

                worldFile.Write(chunk);
            }
        }

        //https://stackoverflow.com/questions/3041366/shortest-distance-between-points-on-a-toroidally-wrapped-x-and-y-wrapping-ma
        public int DistanceSquared(ICoordinate destination, ICoordinate origin)
        {
            var deltaX = Math.Abs(destination.X - origin.X);
            if (deltaX > SIZE / 2)
            {
                deltaX = SIZE - deltaX;
            }
            var deltaY = Math.Abs(destination.Y - origin.Y);
            if (deltaY > SIZE / 2)
            {
                deltaY = SIZE - deltaY;
            }
            var distanceSquared = (deltaX * deltaX + deltaY * deltaY);

            return distanceSquared;
        }

        public int DistanceSquared(int destX, int destY, int originX, int originY)
        {
            var deltaX = Math.Abs(destX - originX);
            if (deltaX > SIZE / 2)
            {
                deltaX = SIZE - deltaX;
            }
            var deltaY = Math.Abs(destY - originY);
            if (deltaY > SIZE / 2)
            {
                deltaY = SIZE - deltaY;
            }
            var distanceSquared = (deltaX * deltaX + deltaY * deltaY);

            return distanceSquared;
        }

        public Region FindNearestRegion(ICoordinate targetTile, UltimaData data, out IList<ITile> outPath)
        {
            outPath = null;
            return null;
        }

        public List<ITile> GetAllMatchingTiles(Func<Tile, bool> criteria, int minX = 0, int maxX = SIZE, int minY = 0, int maxY = SIZE)
        {
            var tiles = new List<ITile>();
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minX; y < maxX; y++)
                {
                    var tile = GetCoordinate(x, y);
                    if (criteria(tile))
                    {
                        tiles.Add(tile);
                    }
                }
            }

            return tiles;
        }

        public static bool IsMatchingTile(ITile coord, List<byte> validTiles)
        {
            return validTiles.Contains(coord.GetTile());
        }

        public static bool IsWalkable(ITile coord)
        {
            return (coord.GetTile() >= TileInfo.Swamp && coord.GetTile() <= TileInfo.Hills) || (coord.GetTile() >= TileInfo.Dungeon_Entrance && coord.GetTile() <= TileInfo.Village);
        }

        public bool IsWalkable(byte x, byte y)
        {
            return IsWalkable(GetCoordinate(x, y));
        }

        public static bool IsWalkableOrSailable(ITile coord)
        {
            return (coord.GetTile() >= TileInfo.Swamp && coord.GetTile() <= TileInfo.Hills) || (coord.GetTile() >= TileInfo.Dungeon_Entrance && coord.GetTile() <= TileInfo.Village) || coord.GetTile() == TileInfo.Deep_Water || coord.GetTile() == TileInfo.Medium_Water;
        }

        public static bool IsGrassOrSailable(ITile coord)
        {
            return coord.GetTile() == TileInfo.Grasslands || coord.GetTile() == TileInfo.Deep_Water || coord.GetTile() == TileInfo.Medium_Water;
        }

        public static bool IsGrass(ITile coord)
        {
            return coord.GetTile() == TileInfo.Grasslands;
        }

        public Tile GetCoordinate(int x, int y)
        {
            return new Tile(x, y, _worldMapTiles, v => Wrap(v));
        }

        public static byte Wrap(int input)
        {
            return Wrap(input, SIZE);
        }

        public static byte Wrap(int input, int divisor)
        {
            return Convert.ToByte((input % divisor + divisor) % divisor);
        }

        public static int WrapInt(int input, int divisor)
        {
            return (input % divisor + divisor) % divisor;
        }

        public static bool Between(byte x, int v1, int v2)
        {
            if (v1 <= v2)
            {
                return x >= v1 && x <= v2;
            }
            else
            {
                return x >= v1 || x <= v2;
            }

            //return ((v1 <= v2) && (x >= v1 && x <= v2)) || ((v1 > v2) && (x >= v1 || x <= v2));
        }

        public SixLabors.ImageSharp.Image ToImage()
        {
            var image = new SixLabors.ImageSharp.Image<Rgba32>(WorldMapGenerateMap.SIZE, WorldMapGenerateMap.SIZE);
            image.ProcessPixelRows(pixelAccessor =>
            {
                for (int y = 0; y < WorldMapGenerateMap.SIZE; y++)
                {
                    Span<Rgba32> pixelRowSpan = pixelAccessor.GetRowSpan(y);
                    for (int x = 0; x < WorldMapGenerateMap.SIZE; x++)
                    {
                        if (colorMap.ContainsKey(_worldMapTiles[x, y]))
                        {
                            pixelRowSpan[x] = colorMap[_worldMapTiles[x, y]];
                        }
                        else
                        {
                            pixelRowSpan[x] = SixLabors.ImageSharp.Color.White;
                        }

                    }
                }
            });
            

            return image;
        }

        public Image ToClothMap(UltimaData data, Random random)
        {
            using (Image<Rgba32> deep_water = Image.Load<Rgba32>(ClothMap.deep_water))
            {
                using (Image<Rgba32> grass = Image.Load<Rgba32>(ClothMap.grass))
                {
                    var image = new Image<Rgba32>(WorldMapGenerateMap.SIZE*4, WorldMapGenerateMap.SIZE*4);

                    image.ProcessPixelRows(deep_water, grass, (imageAccessor, deepWaterAccessor, grassAccessor) =>
                    {
                        for (int y = 0; y < WorldMapGenerateMap.SIZE * 4; y++)
                        {
                            Span<Rgba32> deepWaterRowSpan = deepWaterAccessor.GetRowSpan(y);
                            Span<Rgba32> grassRowSpan = grassAccessor.GetRowSpan(y);
                            Span<Rgba32> pixelRowSpan = imageAccessor.GetRowSpan(y);
                            for (int x = 0; x < WorldMapGenerateMap.SIZE * 4; x++)
                            {
                                //if (colorMap.ContainsKey(_worldMapTiles[x, y]))
                                //{
                                //    pixelRowSpan[x] = colorMap[_worldMapTiles[x, y]];
                                //}
                                //else
                                //{
                                //    pixelRowSpan[x] = SixLabors.ImageSharp.Color.White;
                                //}
                                if (_worldMapTiles[x / 4, y / 4] == TileInfo.Deep_Water)
                                {
                                    pixelRowSpan[x] = deepWaterRowSpan[x];
                                }
                                else
                                {
                                    pixelRowSpan[x] = grassRowSpan[x];
                                }

                            }
                        }
                    });

                    
                    return image;
                }
            }
        }

        public SixLabors.ImageSharp.Image ToHeightMapImage()
        {
            return null;
        }

        static public Dictionary<byte, SixLabors.ImageSharp.Color> colorMap = new Dictionary<byte, SixLabors.ImageSharp.Color>()
        {
            {TileInfo.Deep_Water, SixLabors.ImageSharp.Color.FromRgb(0, 0, 112) },
            {TileInfo.Medium_Water, SixLabors.ImageSharp.Color.FromRgb(20,20,112) },
            {TileInfo.Shallow_Water, SixLabors.ImageSharp.Color.FromRgb(60,60,112) },
            {TileInfo.Swamp, SixLabors.ImageSharp.Color.FromRgb(112, 0, 112) },
            {TileInfo.Grasslands, SixLabors.ImageSharp.Color.FromRgb(18, 112+18, 18) },
            {TileInfo.Scrubland, SixLabors.ImageSharp.Color.FromRgb(68, 112+68, 68) },
            {TileInfo.Forest, SixLabors.ImageSharp.Color.FromRgb(108,112+108,108) },
            {TileInfo.Hills, SixLabors.ImageSharp.Color.FromRgb(112+45,112+45,112+45) },
            {TileInfo.Mountains, SixLabors.ImageSharp.Color.FromRgb(112+15,112+15,112+15) },
            {TileInfo.Fire_Field, SixLabors.ImageSharp.Color.Orange },
            {TileInfo.Lava_Flow, SixLabors.ImageSharp.Color.Red },
            //{TileInfo.Slime_2, SixLabors.ImageSharp.Color.Purple },
        };

        public byte[,] ToArray()
        {
            byte[,] copy = new byte[SIZE, SIZE];
            for(int x = 0; x < SIZE; x++)
            {
                for(int y = 0; y < SIZE; y++)
                {
                    copy[x, y] = _worldMapTiles[x, y];
                }
            }

            return copy;
        }

        public List<ITile> GetPathableTilesNear(ITile goal, int distance, Func<ITile, bool> isWalkableGround)
        {
            var possibleTiles = GetTilesNear(goal, distance);
            var results = new HashSet<ITile>();

            results = Search.GetSuccessfulPaths(SIZE, SIZE, goal, possibleTiles, c => { return isWalkableGround(c); });


            return results.ToList();
        }

        public HashSet<ITile> GetTilesNear(ITile tile, int distance)
        {
            var results = new HashSet<ITile>();
            for(int x = -distance; x <= distance; x++)
            {
                for (int y = -distance; y <= distance; y++)
                {
                    int x_res = tile.X + x;
                    int y_res = tile.Y + y;
                    results.Add(new Tile(x_res, y_res, _worldMapTiles, v => Wrap(v)));
                }
            }

            return results;
        }
    }
}
