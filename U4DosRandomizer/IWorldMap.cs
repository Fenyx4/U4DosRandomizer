using System;

namespace U4DosRandomizer
{
    public interface IWorldMap
    {
        void Load(string path, int v, Random random1, Random random2);
        void Randomize(UltimaData ultimaData, Random random1, Random random2);
        SixLabors.ImageSharp.Image ToImage();

        public void Save(string path);

        Tile GetCoordinate(int x, int y);
    }
}