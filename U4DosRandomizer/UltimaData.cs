using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class UltimaData
    {
        public List<Tile> LCB { get; set; }
        public List<Tile> Castles { get; set; }
        public List<Tile> Towns { get; set; }
        public List<Tile> Shrines { get; set; }
        public List<Tile> Moongates { get; set; }
        public List<Tile> Dungeons { get; set; }
        public List<Item> Items { get; set; }
        public List<string> ShrineText { get; set; }
        public List<Coordinate> StartingPositions { get; set; }
        public List<string> LBText { get; internal set; }
        public byte DaemonSpawnX1 { get; internal set; }
        public byte DaemonSpawnX2 { get; internal set; }
        public byte DaemonSpawnY1 { get; internal set; }
        public byte DaemonSpawnY2 { get; internal set; }
        public byte DaemonSpawnLocationX { get; internal set; }
        public ICoordinate BalloonSpawn { get; internal set; }
        public List<Coordinate> PirateCove { get; internal set; }
        public Coordinate PirateCoveSpawnTrigger { get; internal set; }
        public Coordinate WhirlpoolExit { get; internal set; }
        public List<byte> SpellsRecipes { get; internal set; }
        public byte BlinkExclusionX1 { get; internal set; }
        public byte BlinkExclusionX2 { get; internal set; }
        public byte BlinkExclusionY1 { get; internal set; }
        public byte BlinkExclusionY2 { get; internal set; }
        public byte BlinkExclusion2X1 { get; internal set; }
        public byte BlinkExclusion2X2 { get; internal set; }
        public byte BlinkExclusion2Y1 { get; internal set; }
        public byte BlinkExclusion2Y2 { get; internal set; }
        public List<Coordinate> AbyssEjectionLocations { get; internal set; }

        public UltimaData()
        {
            LCB = new List<Tile>();
            Castles = new List<Tile>();
            Towns = new List<Tile>();
            Shrines = new List<Tile>();
            Moongates = new List<Tile>();
            Dungeons = new List<Tile>();
            Items = new List<Item>();
            ShrineText = new List<string>();
            StartingPositions = new List<Coordinate>();
            LBText = new List<string>();
            PirateCove = new List<Coordinate>();
            AbyssEjectionLocations = new List<Coordinate>();
        }
    }
}