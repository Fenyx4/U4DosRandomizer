using System;

namespace U4DosRandomizer
{
    public interface IMazeGenerator
    {
        void GenerateMaze(string dungeonName, Dungeon dungeon, int numLevels, int width, int height, Random rand);
    }
}