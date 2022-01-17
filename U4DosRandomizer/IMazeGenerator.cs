using System;

namespace U4DosRandomizer
{
    public interface IMazeGenerator
    {
        void GenerateMaze(Dungeon dungeon, int numLevels, int width, int height, Random rand);
    }
}