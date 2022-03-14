using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public interface IWorldMap
    {
        void Load(string path, int v, int mapGeneratorSeed, int otherRandomSeed, UltimaData ultimaData);
        void Randomize(UltimaData ultimaData, Random random1, Random random2, Random random3);
        SixLabors.ImageSharp.Image ToImage();
        SixLabors.ImageSharp.Image ToHeightMapImage();

        public void Save(string path);

        Tile GetCoordinate(int x, int y);
        Region FindNearestRegion(ICoordinate targetTile, UltimaData data, out IList<ITile> outPath);
        SixLabors.ImageSharp.Image ToClothMap(UltimaData data, Random random);
    }
}