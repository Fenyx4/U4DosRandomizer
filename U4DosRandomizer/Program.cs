using Microsoft.Extensions.CommandLineUtils;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                    Flags flags = new Flags();
                    flags.Overworld = overworld;
                    flags.MiniMap = minimapArg.HasValue();
                    flags.SpellRemove = spellRemoveArg.Value();
                    flags.DngStone = dngStoneArg.HasValue();
                    flags.MixQuantity = minQuantityArg.HasValue();
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
            WorldMap.Restore(path);
            Avatar.Restore(path);
            Title.Restore(path);
            Talk.Restore(path);
            Dungeons.Restore(path);
        }

        private static void Randomize(int seed, string path, Flags flags)
        {
            System.IO.File.AppendAllText(@"seed.txt", seed.ToString() + Environment.NewLine);
            Console.WriteLine("Seed: " + seed);
            var random = new Random(seed);

            var randomValues = new List<int>();
            for(int i = 0; i < 50; i++)
            {
                randomValues.Add(random.Next());
            }

            var ultimaData = new UltimaData();

            var worldMap = new WorldMap();
            worldMap.Load(path, flags.Overworld, randomValues[0], new Random(randomValues[1]), new Random(randomValues[2]));

            var avatar = new Avatar();
            avatar.Load(path, ultimaData);

            var title = new Title();
            title.Load(path, ultimaData);

            var talk = new Talk();
            talk.Load(path);

            var dungeons = new Dungeons();
            dungeons.Load(path, ultimaData);

            worldMap.Randomize(ultimaData, new Random(randomValues[3]), new Random(randomValues[4]));

            // Other stones
            if (flags.DngStone)
            {
                foreach (var dungeonName in dungeons.dungeons.Keys)
                {
                    if (dungeonName.ToLower() != "abyss" && dungeonName.ToLower() != "hythloth")
                    {
                        var dungeon = dungeons.dungeons[dungeonName];
                        var stones = dungeon.GetTiles(DungeonTileInfo.AltarOrStone);
                        foreach (var stone in stones)
                        {
                            stone.SetTile(DungeonTileInfo.Nothing);
                        }
                        var possibleDungeonLocations = dungeon.GetTiles(DungeonTileInfo.Nothing);
                        var dungeonLoc = possibleDungeonLocations[random.Next(0, possibleDungeonLocations.Count - 1)];
                        dungeonLoc.SetTile(DungeonTileInfo.AltarOrStone);
                    }
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
                    ultimaData.SpellsRecipes[arr[i]-'a'] = 0;
                }
            }

            //worldMap.TestAbyssEjection();

            //Console.WriteLine(Talk.GetSextantText(ultimaData.LCB[0]));

            title.Update(ultimaData);
            talk.Update(ultimaData, avatar);
            avatar.Update(ultimaData, flags);
            dungeons.Update(ultimaData);

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
            FileStream stream = new FileStream("ULT\\WORLD.MAP", FileMode.Open);
            var world = new byte[256 * 256];
            stream.Read(world, 0, 256 * 256);
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
