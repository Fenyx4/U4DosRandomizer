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

        public int ITEM_MANDRAKE { get; } = 0;
        public int ITEM_MANDRAKE2 { get; } = 1;
        public int ITEM_NIGHTSHADE { get; } = 2;
        public int ITEM_NIGHTSHADE2 { get; } = 3;
        public int ITEM_BELL { get; } = 4; // Bell of Courage
        public int ITEM_HORN { get; } = 5; // Silver Horn
        public int ITEM_WHEEL { get; } = 6; // Wheel of H.M.S. Cape
        public int ITEM_SKULL { get; } = 7; // Skull of Mondain
        public int ITEM_BLACK_STONE { get; } = 8;
        public int ITEM_WHITE_STONE { get; } = 9;
        public int ITEM_BOOK { get; } = 10; // Book of Truth
        public int ITEM_CANDLE { get; } = 11; //
        public int ITEM_TELESCOPE { get; } = 12; // telescope (Crashes if moved (probably fine in any other town))
        public int ITEM_ARMOR { get; } = 13; // Mystic Armor
        public int ITEM_WEAPON { get; } = 14; // Mystic Weapon
        public int ITEM_RUNE_HONESTY { get; } = 15;
        public int ITEM_RUNE_COMPASSION { get; } = 16;
        public int ITEM_RUNE_VALOR { get; } = 17;
        public int ITEM_RUNE_JUSTICE { get; } = 18;
        public int ITEM_RUNE_SACRIFICE { get; } = 19;
        public int ITEM_RUNE_HONOR { get; } = 20;
        public int ITEM_RUNE_SPIRITUALITY { get; } = 21;
        public int ITEM_RUNE_HUMILITY { get; } = 22;
        /*
         * https://github.com/ergonomy-joe/u4-decompiled/blob/master/SRC/U4_SRCH.C#L246
         * 0 - Mandrake
         * 1 - Mandrake
         * 2 - Nightshade
         * 3 - Nightshade
         * 4 - Bell of Courage
         * 5 - Silver Horn
         * 6 - Wheel of H.M.S. Cape
         * 7 - Skull of Mondain
         * 8 - Black Stone
         * 9 - White Stone
         * 10 - Book of Truth
         * 11 - Candle of Love
         * 12 - telescope (Crashes if moved)
         * 13 - Mystic Armor
         * 14 - Mystic Weapon
         * 15 - Rune of the Great Stygian Abyss
         * 16 - Rune of the Great Stygian Abyss
         * 17 - Rune of the Great Stygian Abyss
         * 18 - Rune of the Great Stygian Abyss
         * 19 - Rune of the Great Stygian Abyss
         * 20 - Rune of the Great Stygian Abyss
         * 21 - Rune of the Great Stygian Abyss
         * 22 - Rune of the Great Stygian Abyss
         * 23 - ??
         * All runes on the surface are bugged to be Great Stygian Abyss. I'll figure out which are which later although it doesn't really matter. They just have to be located in the right town.
         */
    }
}