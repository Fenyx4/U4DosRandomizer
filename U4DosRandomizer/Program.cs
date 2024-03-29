using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    class Program
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
                    if(!Directory.Exists(pathArg.Value()))
                    {
                        throw new ArgumentException("Path provided does not exist");
                    }
                    else 
                    {
                        path = pathArg.Value();
                    }
                }
                if(!File.Exists(Path.Combine(path, "WORLD.MAP")))
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
                    Restore(path);
                }
                else
                {
                    Flags flags = new Flags(seed, 9);
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

                    if(flags.DngStone)
                    {
                        flags.Dungeon = 3;
                    }
                    
                    Randomize(seed, path, flags, encodedArg.Value());
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

        private static void Restore(string path)
        {
            WorldMapGenerateMap.Restore(path);
            Avatar.Restore(path);
            Title.Restore(path);
            Talk.Restore(path);
            Dungeons.Restore(path);
        }

        private static void Randomize(int seed, string path, Flags flags, string encoded)
        {
            //Console.WriteLine("Seed: " + seed);

            string json = JsonConvert.SerializeObject(flags);
            if (string.IsNullOrWhiteSpace(encoded))
            {
                encoded = flags.GetEncoded();
            }
            else
            {
                flags.DecodeAndSet(encoded);
                json = JsonConvert.SerializeObject(flags);
                seed = flags.Seed;
            }
            Console.WriteLine("Flags JSON  : " + json);
            Console.WriteLine("Flags Base64: " + encoded);

            StreamWriter spoilerWriter = new StreamWriter("spoiler.txt");
            SpoilerLog spoilerLog = new SpoilerLog(spoilerWriter, flags.SpoilerLog);
            System.IO.File.AppendAllText(@"seed.txt", seed.ToString() + " " + encoded + Environment.NewLine);
            spoilerLog.WriteFlags(flags);

            var random = new Random(seed);

            var randomValues = new List<int>();
            for(int i = 0; i < 50; i++)
            {
                randomValues.Add(random.Next());
            }

            var ultimaData = new UltimaData();

            IWorldMap worldMap = null;

            //WorldMapGenerateMap.PrintWorldMapInfo(path);

            if (flags.Overworld == 5)
            {
                worldMap = new WorldMapGenerateMap(spoilerLog);
                
            }
            else if (flags.Overworld == 1)
            {
                worldMap = new WorldMapUnchanged(spoilerLog);
            }
            else if (flags.Overworld == 2)
            {
                worldMap = new WorldMapShuffleLocations(spoilerLog);
            }
            worldMap.Load(path, randomValues[0], randomValues[1], randomValues[2], ultimaData);

            var avatar = new Avatar(spoilerLog);
            avatar.Load(path, ultimaData, worldMap, flags);

            var title = new Title(spoilerLog);
            title.Load(path, ultimaData);

            var talk = new Talk(spoilerLog);
            talk.Load(path);

            var dungeons = new Dungeons(spoilerLog);
            dungeons.Load(path, ultimaData, flags);

            var party = new Party(spoilerLog);
            party.Load(path, ultimaData);

            var towns = new Towns(spoilerLog);
            towns.Load(path, ultimaData);

            if (flags.Fixes)
            {
                spoilerLog.Add(SpoilerCategory.Fix, "Serpent Hold's Healer");
                ultimaData.ShopLocations[UltimaData.LOC_SERPENT - 1][5] = 0x12;
            }

            worldMap.Randomize(ultimaData, new Random(randomValues[3]), new Random(randomValues[4]), new Random(randomValues[7]));
            dungeons.Randomize(new Random(randomValues[6]), flags);

            if (flags.ClothMap)
            {
                var clothMap = worldMap.ToClothMap(ultimaData, new Random(randomValues[5]));
                clothMap.SaveAsPng($"clothMap-{seed}.png");
                if (flags.Overworld == 5)
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo($"clothMap-{seed}.png")
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
            }

            if(flags.RandomizeSpells)
            {
                var recipes = new byte[2];
                ultimaData.SpellsRecipes['r' - 'a'].Byte = 0;
                ultimaData.SpellsRecipes['g' - 'a'].Byte = 0;
                while(ultimaData.SpellsRecipes['r' - 'a'].Byte == 0)
                {
                    random.NextBytes(recipes);
                    ultimaData.SpellsRecipes['r' - 'a'].Byte = (byte)(recipes[0] | recipes[1]);
                }
                while (ultimaData.SpellsRecipes['g' - 'a'].Byte == 0)
                {
                    random.NextBytes(recipes);
                    ultimaData.SpellsRecipes['g' - 'a'].Byte = (byte)(recipes[0] | recipes[1]);
                }
            }
            if (!String.IsNullOrWhiteSpace(flags.SpellRemove))
            {
                var arr = flags.SpellRemove.ToLower().ToArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    if(arr[i] > 'z' || arr[i] < 'a')
                    {
                        throw new ArgumentException("spellRemove can only contain letters.");
                    }
                    ultimaData.SpellsRecipes[arr[i]-'a'].Byte = 0;
                }
            }

            if (flags.StartingWeapons)
            {
                for (int charIdx = 0; charIdx < 8; charIdx++)
                {
                    var selected = false;
                    while (!selected)
                    {
                        // -1 so Mystic weapons and armors aren't included
                        var weapon = random.Next(1, Party.AllowedWeaponsMask.Length - 1);
                        //If weapon is allowed
                        if ((Party.AllowedWeaponsMask[weapon] & (0x80 >> ultimaData.StartingCharacters[charIdx].Class)) != 0)
                        {
                            ultimaData.StartingCharacters[charIdx].Weapon = (ushort)weapon;
                            selected = true;
                        }
                    }

                    selected = false;
                    while (!selected)
                    {
                        var armor = random.Next(1, Party.AllowedArmorMask.Length);
                        //If weapon is allowed
                        if ((Party.AllowedArmorMask[armor] & (0x80 >> ultimaData.StartingCharacters[charIdx].Class)) != 0)
                        {
                            ultimaData.StartingCharacters[charIdx].Armor = (ushort)armor;
                            selected = true;
                        }
                    }
                }
            }

            if(flags.PrincipleItems)
            {
                //https://www.youtube.com/watch?v=GhnCj7Fvqt0
                var values = new List<Tuple<int, PrincipleItem>> { new Tuple<int, PrincipleItem>(0, ultimaData.PrincipleItemRequirements[0]), new Tuple<int, PrincipleItem>(1, ultimaData.PrincipleItemRequirements[1]), new Tuple<int, PrincipleItem>(2, ultimaData.PrincipleItemRequirements[2]) };
                values.Shuffle(random);
                
                for(int i = 0; i < values.Count(); i++)
                {
                    ultimaData.PrincipleItemRequirements[values[i].Item1].RequiredMask = values[(i+1) % values.Count()].Item2.UsedMask;
                    ultimaData.PrincipleItemRequirements[values[i].Item1].Order = (values.Count() - i) % values.Count();
                }
                // Make the dependency for the first item be owning itself instead of one of the other items being used so there is an item you can start with
                ultimaData.PrincipleItemRequirements[values[0].Item1].RequiredMask = 1 << (4 - values[0].Item1);
            }

            //worldMap.TestAbyssEjection();

            //Console.WriteLine(Talk.GetSextantText(ultimaData.LCB[0]));

            //for (int i = 0; i < 8; i++)
            //{
            //    ultimaData.StartingArmor[i] = Convert.ToUInt16(i + 10);
            //}

            //for (int i = 0; i < 16; i++)
            //{
            //    ultimaData.StartingWeapons[i] = Convert.ToUInt16(i + 10);
            //}

            //ultimaData.StartingFood = 2345 * 100 + 99;
            //ultimaData.StartingGold = 1337;
            //for (int i = 0; i < 4; i++)
            //{
            //    ultimaData.StartingEquipment[i] = Convert.ToUInt16(i + 10);
            //}
            //for (int i = 0; i < 8; i++)
            //{
            //    ultimaData.StartingReagents[i] = Convert.ToUInt16(i + 10);
            //}
            //for (int i = 0; i < 26; i++)
            //{
            //    ultimaData.StartingMixtures[i] = Convert.ToUInt16(i + 10);
            //}

            //ultimaData.StartingItems = 0XFFFF;
            if (flags.QuestItemPercentage > 0)
            {
                ushort ushortone = 1;

                ultimaData.StartingItems = 0;
                for (ushort i = 0; i < 16; i++)
                {
                    if (random.Next(0, 100) < flags.QuestItemPercentage)
                    {
                        ultimaData.StartingItems |= (ushort)(ushortone << i);
                    }
                }
                // Never have the skull destroyed
                ultimaData.StartingItems &= (ushort)(~(ushortone << 1));
                // Don' pre-use bell, book and candle
                ultimaData.StartingItems &= (ushort)(~(ushortone << 10));
                ultimaData.StartingItems &= (ushort)(~(ushortone << 11));
                ultimaData.StartingItems &= (ushort)(~(ushortone << 12));

                ultimaData.StartingRunes = 0;
                for (ushort i = 0; i < 8; i++)
                {
                    if (random.Next(0, 100) < flags.QuestItemPercentage)
                    {
                        ultimaData.StartingRunes |= (byte)(1 << i);
                    }
                }

                ultimaData.StartingStones = 0;
                for (ushort i = 0; i < 8; i++)
                {
                    if (random.Next(0, 100) < flags.QuestItemPercentage)
                    {
                        ultimaData.StartingStones |= (byte)(1 << i);
                    }
                }

                LogQuestItems(spoilerLog,ultimaData);
            }
            else
            {
                spoilerLog.Add(SpoilerCategory.Start, "No change to starting quest items.");
            }

            if(flags.Sextant)
            {
                ultimaData.StartingEquipment[3] = 0x01;
            }

            if(flags.KarmaSetPercentage > 0)
            {
                for(int virtue = 0; virtue < 8; virtue++ )
                {
                    if(random.Next(0, 100) < flags.KarmaSetPercentage)
                    {
                        ultimaData.StartingKarma[virtue] = (flags.KarmaValue.HasValue ? (byte)flags.KarmaValue.Value : (byte)random.Next(0, 100));
                        spoilerLog.Add(SpoilerCategory.Start, $"{ultimaData.ItemNames[virtue + 15]} karma at {ultimaData.StartingKarma[virtue]}");
                    }
                    else
                    {
                        spoilerLog.Add(SpoilerCategory.Start, $"{ultimaData.ItemNames[virtue + 15]} karma unchanged.");
                    }
                }
            }

            var usedSpots = new List<Item>();
            if (flags.Runes)
            {
                spoilerLog.Add(SpoilerCategory.Feature, $"Rune locations randomized");
                var usedLocations = new List<byte>();
                for (int i = UltimaData.ITEM_RUNE_HONESTY; i < 8 + UltimaData.ITEM_RUNE_HONESTY; i++)
                {
                    var possibleOptions = ItemOptions.ItemToItemOptions[i].Where(x => !usedSpots.Contains(x.Item)).ToList();
                    possibleOptions = possibleOptions.Where(x => !usedLocations.Contains(x.Item.Location)).ToList();
                    var selectedItemOption = possibleOptions[random.Next(0, possibleOptions.Count)];
                    ultimaData.Items[i].X = selectedItemOption.Item.X;
                    ultimaData.Items[i].Y = selectedItemOption.Item.Y;
                    ultimaData.Items[i].Location = selectedItemOption.Item.Location;

                    ultimaData.ItemOptions.Add(i, selectedItemOption);
                    usedSpots.Add(selectedItemOption.Item);
                    usedLocations.Add(selectedItemOption.Item.Location);
                }
            }
            else
            {
                for (int i = UltimaData.ITEM_RUNE_HONESTY; i < 8 + UltimaData.ITEM_RUNE_HONESTY; i++)
                {
                    usedSpots.Add(ItemOptions.ItemToItemOptions[i][0].Item);
                }
            }

            if (flags.Mystics)
            {
                spoilerLog.Add(SpoilerCategory.Feature, $"Mystics locations randomized");
                for (int i = UltimaData.ITEM_ARMOR; i < 2 + UltimaData.ITEM_ARMOR; i++)
                {
                    var possibleOptions = ItemOptions.ItemToItemOptions[i].Where(x => !usedSpots.Contains(x.Item)).ToList();
                    var selectedItemOption = possibleOptions[random.Next(0, possibleOptions.Count)];
                    ultimaData.Items[i].X = selectedItemOption.Item.X;
                    ultimaData.Items[i].Y = selectedItemOption.Item.Y;
                    ultimaData.Items[i].Location = selectedItemOption.Item.Location;

                    ultimaData.ItemOptions.Add(i, selectedItemOption);
                    usedSpots.Add(selectedItemOption.Item);
                }
            }
            else
            {
                for (int i = UltimaData.ITEM_ARMOR; i < 2 + UltimaData.ITEM_ARMOR; i++)
                {
                    usedSpots.Add(ItemOptions.ItemToItemOptions[i][0].Item);
                }
            }

            if (flags.Mantras)
            {
                int numberOfTwos = 3;
                int numberOfThrees = 4;
                int numberOfFours = 1;
                // Grab Sacrifice first since it is special
                var mantrasWithLimericks = talk.Mantras.Where(x => x.Limerick.Length > 0).ToList();
                var sacrificeMantra = mantrasWithLimericks[random.Next(0, mantrasWithLimericks.Count)];

                talk.Mantras.Remove(sacrificeMantra);

                if(sacrificeMantra.Text.Length == 2)
                {
                    numberOfTwos--;
                }
                else if(sacrificeMantra.Text.Length == 3)
                {
                    numberOfThrees--;
                }
                else if (sacrificeMantra.Text.Length == 4)
                {
                    numberOfFours--;
                }

                var possibleTwos = talk.Mantras.Where(x => x.Text.Length == 2).ToList();
                var possibleThrees = talk.Mantras.Where(x => x.Text.Length == 3).ToList();
                var possibleFours = talk.Mantras.Where(x => x.Text.Length == 4).ToList();

                var possibleMantras = new List<Mantra>();

                for(int i = 0; i < numberOfTwos; i++)
                {
                    var nextIdx = random.Next(0, possibleTwos.Count);
                    possibleMantras.Add(possibleTwos[nextIdx]);
                    possibleTwos.RemoveAt(nextIdx);
                }

                for (int i = 0; i < numberOfThrees; i++)
                {
                    var nextIdx = random.Next(0, possibleThrees.Count);
                    possibleMantras.Add(possibleThrees[nextIdx]);
                    possibleThrees.RemoveAt(nextIdx);
                }

                for (int i = 0; i < numberOfFours; i++)
                {
                    var nextIdx = random.Next(0, possibleFours.Count);
                    possibleMantras.Add(possibleFours[nextIdx]);
                    possibleFours.RemoveAt(nextIdx);
                }

                possibleMantras.Shuffle(random);
                possibleMantras.Insert(4, sacrificeMantra);
                talk.Mantras = possibleMantras;

                for (int i = 0; i < 8; i++)
                {
                    ultimaData.Mantras[i] = talk.Mantras[i].Text.ToLower();
                }
            }

            if(flags.WordOfPassage)
            {
                var selection = talk.WordsOfPassage[random.Next(0, talk.WordsOfPassage.Count)];
                ultimaData.WordTruth = selection.Item1;
                ultimaData.WordLove = selection.Item2;
                ultimaData.WordCourage = selection.Item3;
                ultimaData.WordOfPassage = selection.Item1 + selection.Item2 + selection.Item3;
            }

            if (flags.HerbPrices == 2)
            {
                ultimaData.HerbPrices.Shuffle(random);
            }
            else if (flags.HerbPrices == 1)
            {
                ultimaData.HerbPrices.Shuffle(random);
                for (int i = 0; i < ultimaData.HerbPrices.Count; i++)
                {
                    ultimaData.HerbPrices[i] = 1;
                }
            }
            else if (flags.HerbPrices == 3)
            {
                ultimaData.HerbPrices.Shuffle(random);
                for (int i = 0; i < ultimaData.HerbPrices.Count; i++)
                {
                    ultimaData.HerbPrices[i] = (byte)(ultimaData.HerbPrices[i]*3);
                }
            }
            
            //ultimaData.StartingStones = 0XFF;
            //ultimaData.StartingRunes = 0XFF;

            title.Update(ultimaData, flags, encoded);
            talk.Update(ultimaData, avatar, flags, worldMap);
            avatar.Update(ultimaData, flags);
            dungeons.Update(ultimaData, flags);
            party.Update(ultimaData);
            towns.Update(ultimaData, flags);

            towns.Save(path);
            party.Save(path);
            dungeons.Save(path);
            title.Save(path);
            talk.Save(path);
            avatar.Save(path);
            worldMap.Save(path);

            if (flags.TownSaves)
            {
                if(!File.Exists(path + "\\TOWNMAP.SAV"))
                {
                    File.Copy(path + "\\BRITAIN.ULT", path + "\\TOWNMAP.SAV");
                }

                if (!File.Exists(path + "\\LCB_1.SAV"))
                {
                    File.Copy(path + "\\LCB_1.ULT", path + "\\LCB_1.SAV");
                }

                if (!File.Exists(path + "\\LCB_2.SAV"))
                {
                    File.Copy(path + "\\LCB_2.ULT", path + "\\LCB_2.SAV");
                }
            }

                if (flags.MiniMap)
            {
                var image = worldMap.ToImage();
                image.SaveAsPng($"worldMap-{seed}.png");
                //image = worldMap.ToHeightMapImage();
                //if (image != null)
                //{
                //    image.SaveAsPng($"worldMap-hm-{seed}.png");
                //}
            }

            spoilerWriter.Close();

            //PrintWorldMapInfo();
        }

        private static void LogQuestItems(SpoilerLog spoilerLog, UltimaData ultimaData)
        {
            ushort ushortone = 1;
            var startingItems = new List<string>
            {
                "Skull",
                "Skull destroyed",
                "Candle",
                "Book",
                "Bell",
                "Courage Key",
                "Love Key",
                "Truth Key",
                "Silver Horn",
                "Wheel of the H.M.S. Cape",
                "Candle used",
                "Book used",
                "Bell used"
            };

            for(int i = 0; i < startingItems.Count; i++)
            {
                if((ultimaData.StartingItems & (ushort)(ushortone << i)) != 0)
                {
                    spoilerLog.Add(SpoilerCategory.Start, startingItems[i]);
                }
                else
                {
                    spoilerLog.Add(SpoilerCategory.Start, $"No {startingItems[i]}");
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if ((ultimaData.StartingRunes & (ushort)(ushortone << i)) != 0)
                {
                    spoilerLog.Add(SpoilerCategory.Start, ultimaData.ItemNames[i + 15]);
                }
                else
                {
                    spoilerLog.Add(SpoilerCategory.Start, $"No {ultimaData.ItemNames[i + 15]}");
                }
            }

            var startingStones = new List<string>
            {
                "Blue",
                "Yellow",
                "Red",
                "Green",
                "Orange",
                "Purple",
                "White",
                "Black"
            };

            for (int i = 0; i < startingStones.Count; i++)
            {
                if ((ultimaData.StartingStones & (ushort)(ushortone << i)) != 0)
                {
                    spoilerLog.Add(SpoilerCategory.Start, startingStones[i]);
                }
                else
                {
                    spoilerLog.Add(SpoilerCategory.Start, $"No {startingStones[i]}");
                }
            }

        }

        

        
    }
}


