using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;
using U4DosRandomizer;

namespace U4DosRandomizer.CLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: true);
            CommandOption seedArg = commandLineApplication.Option(
                "-s |--s <seed>",
                "The seed for the randomizer. "
                + " Same seed will produce the same map. Defaults to random value.",
                CommandOptionType.SingleValue);
            CommandOption pathArg = commandLineApplication.Option(
                "-p |--p <path>",
                "Path to Ultima 4 installation. "
                + " Leaving blank will assume it is the working directory.",
                CommandOptionType.SingleValue);
            CommandOption restoreArg = commandLineApplication.Option(
                "-r |--r",
                "Restore original Ultima 4 files. ",
                CommandOptionType.NoValue);
            CommandOption encodedArg = commandLineApplication.Option(
                "-e |--encoded",
                "Encoded flags. Overrides all other flags. ",
                CommandOptionType.SingleValue);
            CommandOption minimapArg = commandLineApplication.Option(
                "--miniMap",
                "Output a minimap of the overworld. ",
                CommandOptionType.NoValue);
            CommandOption overworldArg = commandLineApplication.Option(
                "-o |--overworld",
                "Sets randomization level for Overworld map. 1 for no change. 2 for shuffle overworld locations. 5 for randomize the entire map. Defaults to 5.",
                CommandOptionType.SingleValue);
            CommandOption spellRemoveArg = commandLineApplication.Option(
                "--spellRemove",
                "Put in the letters of the spells you want removed. e.g. \"--spellRemove zed\" would remove zdown, energy field and dispel. ",
                CommandOptionType.SingleValue);
            CommandOption startingWeapons = commandLineApplication.Option(
                "--startingWeaponsArmor",
                "Randomize the weapons and armor player and companions start with.",
                CommandOptionType.NoValue);
            CommandOption minQuantityArg = commandLineApplication.Option(
                "--mixQuantity",
                "Lets you input how much of a spell you want to mix. ",
                CommandOptionType.NoValue);
            CommandOption dungeonArg = commandLineApplication.Option(
                "-d |--dungeon",
                "Sets randomization level for Dungeon maps. 1 for no change. 2 for make dungeons super simple. 3 for randomize location of dungeon stones. 5 for randomize the entire map. Defaults to 1.",
                CommandOptionType.SingleValue);
            CommandOption dngStoneArg = commandLineApplication.Option(
                "--dngStone",
                "Randomize the location of stones in the dungeons (deprecated use --dungeon)",
                CommandOptionType.NoValue);
            CommandOption fixesArg = commandLineApplication.Option(
                "--fixes",
                "Collection of non-gameplay fixes.",
                CommandOptionType.NoValue);
            CommandOption hythlothFixArg = commandLineApplication.Option(
                "--hythlothFix",
                "Fixes an issue with Hythloth dungeon room.",
                CommandOptionType.NoValue);
            CommandOption sleepLockAssistArg = commandLineApplication.Option(
                "--sleepLockAssist",
                "Helps prevent sleeplock in battles.",
                CommandOptionType.NoValue);
            CommandOption activePlayerArg = commandLineApplication.Option(
                "--activePlayer",
                "Allow selecting which characters are active in combat.",
                CommandOptionType.NoValue);
            CommandOption appleHitChanceArg = commandLineApplication.Option(
                "--appleHitChance",
                "Change hit chance to behave like the original Apple II version.",
                CommandOptionType.NoValue);
            CommandOption diagonalAttackArg = commandLineApplication.Option(
                "--diagonalAttack",
                "Allow diagonal attacks in combat.",
                CommandOptionType.NoValue);
            CommandOption sacrificeFixArg = commandLineApplication.Option(
                "--sacrificeFix",
                "Adds a way to gain sacrifice which the shrine says should work.",
                CommandOptionType.NoValue);
            CommandOption runesArg = commandLineApplication.Option(
                "--runes",
                "Randomize the location of the runes.",
                CommandOptionType.NoValue);
            CommandOption mysticsArg = commandLineApplication.Option(
                "--mystics",
                "Randomize the location of the mystics.",
                CommandOptionType.NoValue);
            CommandOption mantrasArg = commandLineApplication.Option(
                "--mantras",
                "Randomize the mantras.",
                CommandOptionType.NoValue);
            CommandOption wordOfPassageArg = commandLineApplication.Option(
                "--wordOfPassage",
                "Randomize the Word of Passage.",
                CommandOptionType.NoValue);
            CommandOption questItemsArg = commandLineApplication.Option(
                "--questItems <0-100>",
                "Percentage chance to start with a quest item.",
                CommandOptionType.SingleValue);
            CommandOption karmaValueArg = commandLineApplication.Option(
                "--karmaValue <value>",
                "Value to override starting karma value for a virtue. Leave blank for random.",
                CommandOptionType.SingleValue);
            CommandOption karmaPercentageArg = commandLineApplication.Option(
                "--karmaPercentage <0-100>",
                "Percentage chance to override a starting karma value for a virtue. Default 0 (no override).",
                CommandOptionType.SingleValue);
            CommandOption monsterDamageArg = commandLineApplication.Option(
                "--monsterDamage <0-3>",
                "Value to change how much damage monsters do. Allowed values 0-3. 0 is quad damage. 1 is more damge. 2 is default. 3 is less damage.",
                CommandOptionType.SingleValue);
            CommandOption weaponDamageArg = commandLineApplication.Option(
                "--weaponDamage <1-3>",
                "Value to change how much damage weapons do. Allowed values 1-3. 1 is more damge. 2 is default. 3 is less damage.",
                CommandOptionType.SingleValue);
            CommandOption earlierMonstersArg = commandLineApplication.Option(
                "--earlierMonsters",
                "Make more difficult monsters appear earlier.",
                CommandOptionType.NoValue);
            CommandOption monsterQtyArg = commandLineApplication.Option(
                "--monsterQty",
                "More monsters from the start.",
                CommandOptionType.NoValue);
            CommandOption noRequireFullPartyArg = commandLineApplication.Option(
                "--noRequireFullParty",
                "Don't require the full party.",
                CommandOptionType.NoValue);
            CommandOption randomizeSpellsArg = commandLineApplication.Option(
                "--randomizeSpells",
                "Randomizes the gate and resurrection spells that you learn in game.",
                CommandOptionType.NoValue);
            CommandOption herbArg = commandLineApplication.Option(
                "--herbPrice",
                "Changes how herb prices are modified. 1 for cheap reagents. 2 for shuffle reagent prices. 3 for expensive reagents.",
                CommandOptionType.SingleValue);
            CommandOption sextantArg = commandLineApplication.Option(
                "--sextant",
                "Start with a sextant.",
                CommandOptionType.NoValue);
            CommandOption clothMapArg = commandLineApplication.Option(
                "--clothMap",
                "Cloth map of the world.",
                CommandOptionType.NoValue);
            CommandOption principleItemsArg = commandLineApplication.Option(
                "--principleItems",
                "Randomize the order of the Principle Items.",
                CommandOptionType.NoValue);
            CommandOption townSavesArg = commandLineApplication.Option(
                "--townSaves",
                "Enable saving in towns.",
                CommandOptionType.NoValue);
            CommandOption daemonTriggerArg = commandLineApplication.Option(
                "--daemonTrigger",
                "Fix daemon spawn in Abyss",
                CommandOptionType.NoValue);
            CommandOption requireMysticsArg = commandLineApplication.Option(
                "--requireMystics",
                "Require Mystics in the Abyss",
                CommandOptionType.NoValue);
            CommandOption awakenUpgradeArg = commandLineApplication.Option(
                "--awakenUpgrade",
                "Awaken spell awakens all characters.",
                CommandOptionType.NoValue);
            CommandOption shopOverflowArg = commandLineApplication.Option(
                "--shopOverflow",
                "Don't allow overflow exploit in shops.",
                CommandOptionType.NoValue);
            CommandOption otherArg = commandLineApplication.Option(
                "--other",
                "Allow other gender like in Ultima III.",
                CommandOptionType.NoValue);
            CommandOption vgaPatchArg = commandLineApplication.Option(
                "--vgaPatch",
                "VGA patch compatibility. Run randomizer after applying VGA patch.",
                CommandOptionType.NoValue);

            CommandOption spoilerLogArg = commandLineApplication.Option(
                "--spoilerLog",
                "Output a spoiler log.",
                CommandOptionType.NoValue);

            commandLineApplication.HelpOption("-? | -h | --help");

            commandLineApplication.OnExecute(() =>
            {
                var seed = Environment.TickCount;
                if (seedArg.HasValue())
                {
                    if (!int.TryParse(seedArg.Value(), out seed))
                    {
                        throw new InvalidCastException("Seed must be a number");
                    }
                }

                var overworld = 5;
                if (overworldArg.HasValue())
                {
                    if (!int.TryParse(overworldArg.Value(), out overworld))
                    {
                        throw new InvalidCastException("Overworld argument must be a number");
                    }
                }

                var dungeon = 1;
                if (dungeonArg.HasValue())
                {
                    if (!int.TryParse(dungeonArg.Value(), out dungeon))
                    {
                        throw new InvalidCastException("Dungeon argument must be a number");
                    }
                }

                var questItems = 0;
                if (questItemsArg.HasValue())
                {
                    if (!int.TryParse(questItemsArg.Value(), out questItems) && questItems >= 0 && questItems <= 100)
                    {
                        throw new InvalidCastException("QuestItems argument must be a number between 0 and 100 inclusive");
                    }
                }

                var karmaPercentage = 0;
                if (karmaPercentageArg.HasValue())
                {
                    if (!int.TryParse(karmaPercentageArg.Value(), out karmaPercentage) && karmaPercentage >= 0 && karmaPercentage <= 100)
                    {
                        throw new InvalidCastException("KarmaPercentage argument must be a number between 0 and 100 inclusive");
                    }
                }

                int? karmaValue = null;
                var karmaValueTmp = 0;
                if (karmaValueArg.HasValue())
                {
                    if (!int.TryParse(karmaValueArg.Value(), out karmaValueTmp) && karmaValueTmp >= 1 && karmaValueTmp <= 100)
                    {
                        throw new InvalidCastException("KarmaValue argument must be a number between 1 and 100 inclusive");
                    }

                    karmaValue = karmaValueTmp;
                }

                int monsterDamage = 2;
                var monsterDamageTmp = 2;
                if (monsterDamageArg.HasValue())
                {
                    if (!int.TryParse(monsterDamageArg.Value(), out monsterDamageTmp) && monsterDamageTmp >= 0 && monsterDamageTmp <= 3)
                    {
                        throw new InvalidCastException("MonsterDamage argument must be a number between 0 and 3 inclusive");
                    }

                    monsterDamage = monsterDamageTmp;
                }

                int weaponDamage = 2;
                var weaponDamageTmp = 2;
                if (weaponDamageArg.HasValue())
                {
                    if (!int.TryParse(weaponDamageArg.Value(), out weaponDamageTmp) && weaponDamageTmp >= 1 && weaponDamageTmp <= 3)
                    {
                        throw new InvalidCastException("WeaponDamage argument must be a number between 1 and 3 inclusive");
                    }

                    weaponDamage = weaponDamageTmp;
                }

                var herbPrices = 0;
                if (herbArg.HasValue())
                {
                    if (!int.TryParse(herbArg.Value(), out herbPrices))
                    {
                        throw new InvalidCastException("Herb Prices argument must be a number");
                    }
                }

                var path = Directory.GetCurrentDirectory();
                if (pathArg.HasValue())
                {
                    if (!Directory.Exists(pathArg.Value()))
                    {
                        throw new ArgumentException("Path provided does not exist");
                    }
                    else
                    {
                        path = pathArg.Value();
                    }
                }

                if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                    !path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                {
                    path = path + Path.DirectorySeparatorChar;
                }

                if (!File.Exists(Path.Combine(path, "WORLD.MAP")))
                {
                    Console.Write("Could not find WORLD.MAP please provide path:  ");
                    path = Console.ReadLine().Trim();

                    if (!Directory.Exists(path))
                    {
                        throw new ArgumentException("Path provided does not exist");
                    }
                }

                if (restoreArg.HasValue())
                {
                    U4DosRandomizer.DosRandomizer.Restore(path);
                }
                else
                {
                    U4DosRandomizer.Flags flags = new U4DosRandomizer.Flags(seed, 9);
                    flags.Overworld = overworld;
                    flags.MiniMap = minimapArg.HasValue();
                    flags.Dungeon = dungeon;
                    flags.SpellRemove = spellRemoveArg.Value();
                    flags.StartingWeapons = startingWeapons.HasValue();
                    flags.DngStone = dngStoneArg.HasValue();
                    flags.MixQuantity = minQuantityArg.HasValue();
                    flags.Fixes = fixesArg.HasValue();
                    flags.FixHythloth = hythlothFixArg.HasValue();
                    flags.SleepLockAssist = sleepLockAssistArg.HasValue();
                    flags.ActivePlayer = activePlayerArg.HasValue();
                    flags.HitChance = appleHitChanceArg.HasValue();
                    flags.DiagonalAttack = diagonalAttackArg.HasValue();
                    flags.SacrificeFix = sacrificeFixArg.HasValue();
                    flags.Runes = runesArg.HasValue();
                    flags.Mystics = mysticsArg.HasValue();
                    flags.Mantras = mantrasArg.HasValue();
                    flags.WordOfPassage = wordOfPassageArg.HasValue();
                    flags.QuestItemPercentage = questItems;
                    flags.KarmaSetPercentage = karmaPercentage;
                    flags.KarmaValue = karmaValue;
                    flags.MonsterDamage = monsterDamage;
                    flags.WeaponDamage = weaponDamage;
                    flags.EarlierMonsters = earlierMonstersArg.HasValue();
                    flags.MonsterQty = monsterQtyArg.HasValue();
                    flags.NoRequireFullParty = noRequireFullPartyArg.HasValue();
                    flags.RandomizeSpells = randomizeSpellsArg.HasValue();
                    flags.HerbPrices = herbPrices;
                    flags.Sextant = sextantArg.HasValue();
                    flags.ClothMap = clothMapArg.HasValue();
                    flags.PrincipleItems = principleItemsArg.HasValue();
                    flags.TownSaves = townSavesArg.HasValue();
                    flags.DaemonTrigger = daemonTriggerArg.HasValue();
                    flags.RequireMysticWeapons = requireMysticsArg.HasValue();
                    flags.AwakenUpgrade = awakenUpgradeArg.HasValue();
                    flags.ShopOverflowFix = shopOverflowArg.HasValue();
                    flags.Other = otherArg.HasValue();
                    flags.SpoilerLog = spoilerLogArg.HasValue();
                    flags.VGAPatch = vgaPatchArg.HasValue();

                    if (flags.DngStone)
                    {
                        flags.Dungeon = 3;
                    }

                    U4DosRandomizer.DosRandomizer.Randomize(seed, path, flags, encodedArg.Value());
                    //Console.WriteLine("Seed: " + seed);
                    //var random = new Random(seed);
                    //var worldMap = new WorldMap();
                    //worldMap.SwampTest(random);
                    //worldMap.Save(path);

                    //var image = worldMap.ToImage();
                    //image.SaveAsPng("worldMap.png");
                }


                return 0;
            });
            commandLineApplication.Execute(args);
        }

    }
}


