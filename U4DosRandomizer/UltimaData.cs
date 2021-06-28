using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace U4DosRandomizer
{
    public class UltimaData
    {
        public ReadOnlyCollection<string> LocationNames { get; private set; }
        public ReadOnlyCollection<string> ItemNames { get; private set; }
        public ReadOnlyCollection<Tile> LCB { get; private set; }
        bool lcbSet = false;
        public void SetLCB(List<Tile> value)
        {
            if (lcbSet)
            {
                throw new System.Exception("Can only set LCB once.");
            }
            else
            {
                LCB = value.AsReadOnly();
            }
        }
        public ReadOnlyCollection<TileDirtyWrapper> Castles { get; private set; }
        bool castlesSet = false;
        public void SetCastles(List<TileDirtyWrapper> value)
        {
            if (castlesSet)
            {
                throw new System.Exception("Can only set castles once.");
            }
            else
            {
                Castles = value.AsReadOnly();
            }
        }
        public ReadOnlyCollection<TileDirtyWrapper> Towns { get; private set; }
        bool townsSet = false;
        public void SetTowns(List<TileDirtyWrapper> value)
        {
            if (townsSet)
            {
                throw new System.Exception("Can only set towns once.");
            }
            else
            {
                Towns = value.AsReadOnly();
            }
        }
        public ReadOnlyCollection<TileDirtyWrapper> Shrines { get; private set; }
        bool shrinesSet = false;
        public void SetShrines(List<TileDirtyWrapper> value)
        {
            if (shrinesSet)
            {
                throw new System.Exception("Can only set shrines once.");
            }
            else
            {
                Shrines = value.AsReadOnly();
            }
        }
        public ReadOnlyCollection<Tile> Moongates { get; private set; }
        bool moongatesSet = false;
        public void SetMoongates(List<Tile> value)
        {
            if (moongatesSet)
            {
                throw new System.Exception("Can only set moongates once.");
            }
            else
            {
                Moongates = value.AsReadOnly();
            }
        }
        public ReadOnlyCollection<Tile> Dungeons { get; private set; }
        bool dungeonsSet = false;
        public void SetDungeons(List<Tile> value)
        {
            if (dungeonsSet)
            {
                throw new System.Exception("Can only set dungeons once.");
            }
            else
            {
                Dungeons = value.AsReadOnly();
            }
        }

        public ReadOnlyCollection<Item> Items { get; private set; }

        bool itemsSet = false;
        public void SetItems(List<Item> value)
        {
            if (itemsSet)
            {
                throw new System.Exception("Can only set items once.");
            }
            else
            {
                Items = value.AsReadOnly();
            }
        }

        public Dictionary<int, ItemOption> ItemOptions { get; private set; }

        public List<string> ShrineText { get; set; }
        public List<Coordinate> StartingPositions { get; set; }
        public List<string> LBText { get; internal set; }
        public List<string> Mantras { get; internal set; }
        public string WordOfPassage { get; internal set; }
        public List<Character> StartingCharacters { get; internal set; }
        public byte DaemonSpawnX1 { get; internal set; }
        public byte DaemonSpawnX2 { get; internal set; }
        public byte DaemonSpawnY1 { get; internal set; }
        public byte DaemonSpawnY2 { get; internal set; }
        public byte DaemonSpawnLocationX { get; internal set; }
        public ICoordinate BalloonSpawn { get; internal set; }
        public List<Coordinate> PirateCove { get; internal set; }
        public Coordinate PirateCoveSpawnTrigger { get; internal set; }
        public Coordinate WhirlpoolExit { get; internal set; }
        public List<ByteDirtyWrapper> SpellsRecipes { get; internal set; }
        bool spellRecipesSet = false;

        public void SetSpellsRecipes(List<Item> value)
        {
            if (spellRecipesSet)
            {
                throw new System.Exception("Can only set spell recipes once.");
            }
            else
            {
                Items = value.AsReadOnly();
            }
        }
        public byte BlinkCastExclusionX1 { get; internal set; }
        public byte BlinkCastExclusionX2 { get; internal set; }
        public byte BlinkCastExclusionY1 { get; internal set; }
        public byte BlinkCastExclusionY2 { get; internal set; }
        public byte BlinkDestinationExclusionX1 { get; internal set; }
        public byte BlinkDestinationExclusionX2 { get; internal set; }
        public byte BlinkDestinationExclusionY1 { get; internal set; }
        public byte BlinkDestinationExclusionY2 { get; internal set; }
        public byte BlinkDestinationExclusion2X1 { get; internal set; }
        public byte BlinkDestinationExclusion2X2 { get; internal set; }
        public byte BlinkDestinationExclusion2Y1 { get; internal set; }
        public byte BlinkDestinationExclusion2Y2 { get; internal set; }
        public List<Coordinate> AbyssEjectionLocations { get; internal set; }
        public List<List<byte>> ShopLocations { get; set; }

        public uint StartingFood { get; internal set; }
        public ushort StartingGold { get; internal set; }
        public List<byte> StartingKarma { get; internal set; }
        public List<ushort> StartingEquipment { get; internal set; }
        public List<ushort> StartingArmor { get; internal set; }
        public List<ushort> StartingWeapons { get; internal set; }
        public List<ushort> StartingReagents { get; internal set; }
        public List<ushort> StartingMixtures { get; internal set; }
        public ushort StartingItems { get; internal set; }
        public byte StartingStones { get; internal set; }
        public byte StartingRunes { get; internal set; }


        public UltimaData()
        {
            ShrineText = new List<string>();
            StartingPositions = new List<Coordinate>();
            LBText = new List<string>();
            Mantras = new List<string>();
            PirateCove = new List<Coordinate>();
            AbyssEjectionLocations = new List<Coordinate>();
            ShopLocations = new List<List<byte>>();
            StartingCharacters = new List<Character>();
            StartingKarma = new List<byte>();
            StartingEquipment = new List<ushort>();
            StartingArmor = new List<ushort>();
            StartingWeapons = new List<ushort>();
            StartingReagents = new List<ushort>();
            StartingMixtures = new List<ushort>();
            ItemOptions = new Dictionary<int, ItemOption>();

            LocationNames = (new List<string>
            {
                 "Britannia",
                 "Lycaeum",
                 "Empath Abbey",
                 "Serpents Hold",
                 "Moonglow",
                 "Britain",
                 "Jhelom",
                 "Yew",
                 "Minoc",
                 "Trinsic",
                 "Skara Brae",
                 "Magincia",
                 "Paws",
                 "Buccaneer's Den",
                 "Vesper",
                 "Cove",
                 "Deciet",
                 "Despise",
                 "Destard",
                 "Wrong",
                 "Covetous",
                 "Shame",
                 "Hythloth",
                 "The Great Stygian Abyss",
                 "Honesty",
                 "Compassion",
                 "Valor",
                 "Justice",
                 "Sacrifice",
                 "Honor",
                 "Spirituality",
                 "Humility"
            }).AsReadOnly();

            ItemNames = (new List<string>
            {
                "MANDRAKE",
                "MANDRAKE2",
                "NIGHTSHADE",
                "NIGHTSHADE2",
                "BELL",
                "HORN",
                "WHEEL",
                "SKULL",
                "BLACK_STONE",
                "WHITE_STONE",
                "BOOK",
                "CANDLE",
                "TELESCOPE",
                "ARMOR",
                "WEAPON",
                "RUNE_HONESTY",
                "RUNE_COMPASSION",
                "RUNE_VALOR",
                "RUNE_JUSTICE",
                "RUNE_SACRIFICE",
                "RUNE_HONOR",
                "RUNE_SPIRITUALITY",
                "RUNE_HUMILITY"
            }).AsReadOnly();

        }
        public ICoordinate GetLocation(int i)
        {
            if(i == 0)
            {
                return LCB[1];
            }
            if(i < LOC_TOWNS-1)
            {
                return Castles[i-1];
            }
            else if (i < LOC_DUNGEONS-1)
            {
                return Towns[i - LOC_TOWNS+1];
            }
            else if(i < LOC_SHRINES-1)
            {
                return Dungeons[i - LOC_DUNGEONS+1];
            }
            else
            {
                return Shrines[i - LOC_SHRINES+1];
            }
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
        public static int ITEM_RUNE_HONESTY { get; } = 15;
        public static int ITEM_RUNE_COMPASSION { get; } = 16;
        public static int ITEM_RUNE_VALOR { get; } = 17;
        public static int ITEM_RUNE_JUSTICE { get; } = 18;
        public static int ITEM_RUNE_SACRIFICE { get; } = 19;
        public static int ITEM_RUNE_HONOR { get; } = 20;
        public static int ITEM_RUNE_SPIRITUALITY { get; } = 21;
        public static int ITEM_RUNE_HUMILITY { get; } = 22;
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

        public int LOC_BUILDINGS { get; } = 0x01;

        public int LOC_CASTLES { get; } = 0x01;
        public int LOC_LCB { get; } = 0x01;
        public int LOC_LYCAEUM { get; } = 0x02;
        public int LOC_EMPATH { get; } = 0x03;
        public int LOC_SERPENT { get; } = 0x04;

        public int LOC_TOWNS { get; } = 0x05;
        public int LOC_MOONGLOW { get; } = 0x05;
        public int LOC_BRITAIN { get; } = 0x06;
        public int LOC_JHELOM { get; } = 0x07;
        public int LOC_YEW { get; } = 0x08;
        public int LOC_MINOC { get; } = 0x09;
        public int LOC_TRINSIC { get; } = 0x0a;
        public int LOC_SKARA { get; } = 0x0b;
        public int LOC_MAGINCIA { get; } = 0x0c;
        public int LOC_PAWS { get; } = 0x0d;
        public int LOC_DEN { get; } = 0x0e;
        public int LOC_VESPER { get; } = 0x0f;
        public int LOC_COVE { get; } = 0x10;

        public int LOC_DUNGEONS { get; } = 0x11;
        public int LOC_DECEIT { get; } = 0x11;
        public int LOC_DESPISE { get; } = 0x12;
        public int LOC_DESTARD { get; } = 0x13;
        public int LOC_WRONG { get; } = 0x14;
        public int LOC_COVETOUS { get; } = 0x15;
        public int LOC_SHAME { get; } = 0x16;
        public int LOC_HYTHLOTH { get; } = 0x17;
        public int LOC_ABYSS { get; } = 0x18;

        public int LOC_SHRINES { get; } = 0x19;
        public int LOC_HONESTY { get; } = 0x19;
        public int LOC_COMPASSION { get; } = 0x1a;
        public int LOC_VALOR { get; } = 0x1b;
        public int LOC_JUSTICE { get; } = 0x1c;
        public int LOC_SACRIFICE { get; } = 0x1d;
        public int LOC_HONOR { get; } = 0x1e;
        public int LOC_SPIRITUALITY { get; } = 0x1f;
        public int LOC_HUMILITY { get; } = 0x20;
        public string WordTruth { get; internal set; }
        public string WordLove { get; internal set; }
        public string WordCourage { get; internal set; }
        /*
* https://github.com/ergonomy-joe/u4-decompiled/blob/master/SRC/U4_LOC.H
* 0 - Britannia
* 1 - Lycaeum
* 2 - Empath Abbey
* 3 - Serpents Hold
* 4 - Moonglow
* 5 - Britain
* 6 - Jhelom
* 7 - Yew
* 8 - Minoc
* 9 - Trinsic
* 10 - Skara Brae
* 11 - Magincia
* 12 - Paws
* 13 - Buccaneer's Den
* 14 - Vesper
* 15 - Cove
* 16 - Deciet
* 17 - Despise
* 18 - Destard
* 19 - Wrong
* 20 - Covetous
* 21 - Shame
* 22 - Hythloth
* 23 - The Great Stygian Abyss
* 24 - Honesty
* 25 - Compassion
* 26 - Valor
* 27 - Justice
* 28 - Sacrifice
* 29 - Honor
* 30 - Spirituality
* 31 - Humility
*/
    }
}