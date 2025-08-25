using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace U4DosRandomizer.UI
{
    public partial class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            Clicked = new Command(OnClicked);
        }

        [ObservableProperty]
        public bool generateClothMap;

        public ICommand Clicked { get; }

        public void OnClicked()
        {
            var seed = Environment.TickCount;
            //if (seedArg.HasValue())
            //{
            //    if (!int.TryParse(seedArg.Value(), out seed))
            //    {
            //        throw new InvalidCastException("Seed must be a number");
            //    }
            //}

            var overworld = 5;
            //if (overworldArg.HasValue())
            //{
            //    if (!int.TryParse(overworldArg.Value(), out overworld))
            //    {
            //        throw new InvalidCastException("Overworld argument must be a number");
            //    }
            //}

            var dungeon = 1;
            //if (dungeonArg.HasValue())
            //{
            //    if (!int.TryParse(dungeonArg.Value(), out dungeon))
            //    {
            //        throw new InvalidCastException("Dungeon argument must be a number");
            //    }
            //}

            var questItems = 0;
            //if (questItemsArg.HasValue())
            //{
            //    if (!int.TryParse(questItemsArg.Value(), out questItems) && questItems >= 0 && questItems <= 100)
            //    {
            //        throw new InvalidCastException("QuestItems argument must be a number between 0 and 100 inclusive");
            //    }
            //}

            var karmaPercentage = 0;
            //if (karmaPercentageArg.HasValue())
            //{
            //    if (!int.TryParse(karmaPercentageArg.Value(), out karmaPercentage) && karmaPercentage >= 0 && karmaPercentage <= 100)
            //    {
            //        throw new InvalidCastException("KarmaPercentage argument must be a number between 0 and 100 inclusive");
            //    }
            //}

            int? karmaValue = null;
            var karmaValueTmp = 0;
            //if (karmaValueArg.HasValue())
            //{
            //    if (!int.TryParse(karmaValueArg.Value(), out karmaValueTmp) && karmaValueTmp >= 1 && karmaValueTmp <= 100)
            //    {
            //        throw new InvalidCastException("KarmaValue argument must be a number between 1 and 100 inclusive");
            //    }

            karmaValue = karmaValueTmp;
            //}

            int monsterDamage = 2;
            var monsterDamageTmp = 2;
            //if (monsterDamageArg.HasValue())
            //{
            //    if (!int.TryParse(monsterDamageArg.Value(), out monsterDamageTmp) && monsterDamageTmp >= 0 && monsterDamageTmp <= 3)
            //    {
            //        throw new InvalidCastException("MonsterDamage argument must be a number between 0 and 3 inclusive");
            //    }

            monsterDamage = monsterDamageTmp;
            //}

            int weaponDamage = 2;
            var weaponDamageTmp = 2;
            //if (weaponDamageArg.HasValue())
            //{
            //    if (!int.TryParse(weaponDamageArg.Value(), out weaponDamageTmp) && weaponDamageTmp >= 1 && weaponDamageTmp <= 3)
            //    {
            //        throw new InvalidCastException("WeaponDamage argument must be a number between 1 and 3 inclusive");
            //    }

            weaponDamage = weaponDamageTmp;
            //}

            var herbPrices = 0;
            //if (herbArg.HasValue())
            //{
            //    if (!int.TryParse(herbArg.Value(), out herbPrices))
            //    {
            //        throw new InvalidCastException("Herb Prices argument must be a number");
            //    }
            //}

            var path = Directory.GetCurrentDirectory();
            path = "F:\\Gog-games\\Ultima 4";
            //if (pathArg.HasValue())
            //{
            //    if (!Directory.Exists(pathArg.Value()))
            //    {
            //        throw new ArgumentException("Path provided does not exist");
            //    }
            //    else
            //    {
            //        path = pathArg.Value();
            //    }
            //}
            if (!File.Exists(Path.Combine(path, "WORLD.MAP")))
            {
                //Console.Write("Could not find WORLD.MAP please provide path:  ");
                //path = Console.ReadLine().Trim();

                //if (!Directory.Exists(path))
                //{
                //    throw new ArgumentException("Path provided does not exist");
                //}
            }

            //if (restoreArg.HasValue())
            //{
            //    Restore(path);
            //}
            //else
            //{
            Flags flags = new(seed, 9);
            flags.Overworld = overworld;
            //flags.MiniMap = minimapArg.HasValue();
            flags.MiniMap = false;
            flags.Dungeon = dungeon;
            //flags.SpellRemove = spellRemoveArg.Value();
            flags.SpellRemove = "";
            //flags.StartingWeapons = startingWeapons.HasValue();
            flags.StartingWeapons = false;
            //flags.DngStone = dngStoneArg.HasValue();
            flags.DngStone = false;
            //flags.MixQuantity = minQuantityArg.HasValue();
            flags.MixQuantity = false;
            //flags.Fixes = fixesArg.HasValue();
            flags.Fixes = false;
            //flags.FixHythloth = hythlothFixArg.HasValue();
            flags.FixHythloth = false;
            //flags.SleepLockAssist = sleepLockAssistArg.HasValue();
            flags.SleepLockAssist = false;
            //flags.ActivePlayer = activePlayerArg.HasValue();
            flags.ActivePlayer = false;
            //flags.HitChance = appleHitChanceArg.HasValue();
            flags.HitChance = false;
            //flags.DiagonalAttack = diagonalAttackArg.HasValue();
            flags.DiagonalAttack = false;
            //flags.SacrificeFix = sacrificeFixArg.HasValue();
            flags.SacrificeFix = false;
            //flags.Runes = runesArg.HasValue();
            flags.Runes = false;
            flags.Mystics = false;
            //flags.Mystics = mysticsArg.HasValue();
            flags.Mystics = false;
            //flags.Mantras = mantrasArg.HasValue();
            flags.Mantras = false;
            //flags.WordOfPassage = wordOfPassageArg.HasValue();
            flags.WordOfPassage = false;
            flags.QuestItemPercentage = questItems;
            flags.KarmaSetPercentage = karmaPercentage;
            flags.KarmaValue = karmaValue;
            flags.MonsterDamage = monsterDamage;
            flags.WeaponDamage = weaponDamage;
            //flags.EarlierMonsters = earlierMonstersArg.HasValue();
            flags.EarlierMonsters = false;
            //flags.MonsterQty = monsterQtyArg.HasValue();
            flags.MonsterQty = false;
            //flags.NoRequireFullParty = noRequireFullPartyArg.HasValue();
            flags.NoRequireFullParty = false;
            //flags.RandomizeSpells = randomizeSpellsArg.HasValue();
            flags.RandomizeSpells = false;
            flags.HerbPrices = herbPrices;
            //flags.Sextant = sextantArg.HasValue();
            flags.Sextant = false;
            //flags.ClothMap = clothMapArg.HasValue();
            flags.ClothMap = generateClothMap;
            //flags.PrincipleItems = principleItemsArg.HasValue();
            flags.PrincipleItems = false;
            //flags.TownSaves = townSavesArg.HasValue();
            flags.TownSaves = false;
            //flags.DaemonTrigger = daemonTriggerArg.HasValue();
            flags.DaemonTrigger = false;
            //flags.RequireMysticWeapons = requireMysticsArg.HasValue();
            flags.RequireMysticWeapons = false;
            //flags.AwakenUpgrade = awakenUpgradeArg.HasValue();
            flags.AwakenUpgrade = false;
            //flags.ShopOverflowFix = shopOverflowArg.HasValue();
            flags.ShopOverflowFix = false;
            //flags.Other = otherArg.HasValue();
            flags.Other = true;
            //flags.SpoilerLog = spoilerLogArg.HasValue();
            flags.SpoilerLog = true;
            //flags.VGAPatch = vgaPatchArg.HasValue();
            flags.VGAPatch = false;

            if (flags.DngStone)
            {
                flags.Dungeon = 3;
            }

            //Program.Randomize(seed, path, flags, encodedArg.Value());
            path = "F:\\Gog-games\\Ultima 4";
            DosRandomizer.Randomize(seed, path, flags, "");
            //Console.WriteLine("Seed: " + seed);
            //var random = new Random(seed);
            //var worldMap = new WorldMap();
            //worldMap.SwampTest(random);
            //worldMap.Save(path);

            //var image = worldMap.ToImage();
            //image.SaveAsPng("worldMap.png");
            //}
        }
    }
}
