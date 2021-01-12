using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace U4DosRandomizer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            CommandLineApplication commandLineApplication = new CommandLineApplication(throwOnUnexpectedArg: false);
            CommandOption seedArg = commandLineApplication.Option(
                "-s |--s <seed>",
                "The seed for the randomizer. "
                + " Same seed will produce the same map.",
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
            CommandOption minimapArg = commandLineApplication.Option(
                "--miniMap",
                "Output a minimap of the overworld. ",
                CommandOptionType.NoValue);
            CommandOption overworldArg = commandLineApplication.Option(
                "-o |--overworld",
                "Sets randomization level for Overworld map. ",
                CommandOptionType.SingleValue);
            CommandOption spellRemoveArg = commandLineApplication.Option(
                "--spellRemove",
                "Put in the letters of the spells you want removed. e.g. \"--spellRemove zed\" would remove zdown, energy field and dispel. ",
                CommandOptionType.SingleValue);
            CommandOption minQuantityArg = commandLineApplication.Option(
                "--mixQuantity",
                "Lets you input how much of a spell you want to mix. ",
                CommandOptionType.NoValue);
            CommandOption dngStoneArg = commandLineApplication.Option(
                "--dngStone",
                "Randomize the location of stones in the dungeons ",
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
            CommandOption questItemsArg = commandLineApplication.Option(
                "--questItems",
                "Percentage chance to start with a quest item.",
                CommandOptionType.SingleValue);
            CommandOption karmaValueArg = commandLineApplication.Option(
                "--karmaValue",
                "Value to override starting karma value for a virtue. Leave blank for random.",
                CommandOptionType.SingleValue);
            CommandOption karmaPercentageArg = commandLineApplication.Option(
                "--karmaPercentage",
                "Percentage chance to override a starting karma value for a virtue. Default 0 (no override).",
                CommandOptionType.SingleValue);

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
                    flags.SpellRemove = spellRemoveArg.Value();
                    flags.DngStone = dngStoneArg.HasValue();
                    flags.MixQuantity = minQuantityArg.HasValue();
                    flags.Fixes = fixesArg.HasValue();
                    flags.FixHythloth = hythlothFixArg.HasValue();
                    flags.SleepLockAssist = sleepLockAssistArg.HasValue();
                    flags.ActivePlayer = activePlayerArg.HasValue();
                    flags.HitChance = appleHitChanceArg.HasValue();
                    flags.DiagonalAttack = diagonalAttackArg.HasValue();
                    flags.SacrificeFix = sacrificeFixArg.HasValue();
                    flags.QuestItemPercentage = questItems;
                    flags.KarmaSetPercentage = karmaPercentage;
                    flags.KarmaValue = karmaValue;
                    Randomize(seed, path, flags);
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

        private static void Randomize(int seed, string path, Flags flags)
        {
            //Console.WriteLine("Seed: " + seed);

            string json = JsonConvert.SerializeObject(flags);
            Console.WriteLine("Flags JSON  : " + json);
            var encoded = flags.GetEncoded();
            Console.WriteLine("Flags Base64: " + encoded);

            System.IO.File.AppendAllText(@"seed.txt", seed.ToString() + " " + "encoded" + Environment.NewLine);
            //flags.DecodeAndSet(encoded);
            //json = JsonConvert.SerializeObject(flags);
            //Console.WriteLine("Flags JSON  : " + json);

            var random = new Random(seed);

            var randomValues = new List<int>();
            for(int i = 0; i < 50; i++)
            {
                randomValues.Add(random.Next());
            }

            var ultimaData = new UltimaData();

            IWorldMap worldMap = null;

            if (flags.Overworld == 5)
            {
                worldMap = new WorldMapGenerateMap();
            }
            else if (flags.Overworld == 1)
            {
                worldMap = new WorldMapUnchanged();
            }
            else if (flags.Overworld == 2)
            {
                worldMap = new WorldMapShuffleLocations();
            }
            worldMap.Load(path, randomValues[0], new Random(randomValues[1]), new Random(randomValues[2]));

            var avatar = new Avatar();
            avatar.Load(path, ultimaData, worldMap);

            var title = new Title();
            title.Load(path, ultimaData);

            var talk = new Talk();
            talk.Load(path);

            var dungeons = new Dungeons();
            dungeons.Load(path, ultimaData, flags);

            var party = new Party();
            party.Load(path, ultimaData);

            var towns = new Towns();
            towns.Load(path, ultimaData);

            if (flags.Fixes)
            {
                ultimaData.ShopLocations[avatar.AvatarOffset.LOC_SERPENT - 1][5] = 0x12;
            }

            worldMap.Randomize(ultimaData, new Random(randomValues[3]), new Random(randomValues[4]));

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
            }

            if(flags.KarmaSetPercentage > 0)
            {
                for(int virtue = 0; virtue < 8; virtue++ )
                {
                    if(random.Next(0, 100) < flags.KarmaSetPercentage)
                    {
                        ultimaData.StartingKarma[virtue] = (flags.KarmaValue.HasValue ? (byte)flags.KarmaValue.Value : (byte)random.Next(0, 100));
                    }
                }
            }

            
            //ultimaData.StartingStones = 0XFF;
            //ultimaData.StartingRunes = 0XFF;

            title.Update(ultimaData, flags, encoded);
            talk.Update(ultimaData, avatar, flags);
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

            if (flags.MiniMap)
            {
                var image = worldMap.ToImage();
                image.SaveAsPng($"worldMap-{seed}.png");
            }

            //PrintWorldMapInfo();
        }
        
        private static void PrintWorldMapInfo()
        {
            var world = new byte[256 * 256];
            using (FileStream stream = new FileStream("ULT\\WORLD.MAP", FileMode.Open))
            { 
                stream.Read(world, 0, 256 * 256);
            }
            var worldList = world.ToList();

            worldList.Sort();

            var worldTileCount = new Dictionary<byte, int>();
            for (int i = 0; i < worldList.Count(); i++)
            {
                if (worldTileCount.ContainsKey(worldList[i]))
                {
                    worldTileCount[worldList[i]] = worldTileCount[worldList[i]] + 1;
                }
                else
                {
                    worldTileCount[worldList[i]] = 1;
                }
            }

            foreach (var key in worldTileCount.Keys)
            {
                //var output = $"{shapes[key]}:".PadRight(31) + $" {worldTileCount[key]/(256.0*256.0)}";
                var output = $"{{{key},{worldTileCount[key] / (256.0 * 256.0)}}}";
                Console.WriteLine(output);
            }
        }

        static public double Linear(double x, double x0, double x1, double y0, double y1)
        {
            if ((x1 - x0) == 0)
            {
                return (y0 + y1) / 2;
            }
            return y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        }

        
    }
}
