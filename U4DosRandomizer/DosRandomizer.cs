using System.Diagnostics;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    // All the code in this file is included in all platforms.
    public class DosRandomizer
    {
        public static void Restore(string path)
        {
            WorldMapGenerateMap.Restore(path);
            Avatar.Restore(path);
            Title.Restore(path);
            Talk.Restore(path);
            Dungeons.Restore(path);
        }

        public static void Randomize(int seed, string path, Flags flags, string encoded)
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

            if (!Directory.Exists(path))
            {
                throw new Exception("Directory does not exist.");
            }
            StreamWriter spoilerWriter = new StreamWriter(path + "spoiler.txt");
            SpoilerLog spoilerLog = new SpoilerLog(spoilerWriter, flags.SpoilerLog);
            System.IO.File.AppendAllText(path + @"seed.txt", seed.ToString() + " " + encoded + Environment.NewLine);
            spoilerLog.WriteFlags(flags);

            var random = new Random(seed);

            var randomValues = new List<int>();
            for (int i = 0; i < 50; i++)
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
                clothMap.SaveAsPng($"{path}clothMap-{seed}.png");
                if (flags.Overworld == 5)
                {
                    new Process
                    {
                        StartInfo = new ProcessStartInfo($"{path}clothMap-{seed}.png")
                        {
                            UseShellExecute = true
                        }
                    }.Start();
                }
            }

            if (flags.RandomizeSpells)
            {
                var recipes = new byte[2];
                ultimaData.SpellsRecipes['r' - 'a'].Byte = 0;
                ultimaData.SpellsRecipes['g' - 'a'].Byte = 0;
                while (ultimaData.SpellsRecipes['r' - 'a'].Byte == 0)
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
                    if (arr[i] > 'z' || arr[i] < 'a')
                    {
                        throw new ArgumentException("spellRemove can only contain letters.");
                    }
                    ultimaData.SpellsRecipes[arr[i] - 'a'].Byte = 0;
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

            if (flags.PrincipleItems)
            {
                //https://www.youtube.com/watch?v=GhnCj7Fvqt0
                var values = new List<Tuple<int, PrincipleItem>> { new Tuple<int, PrincipleItem>(0, ultimaData.PrincipleItemRequirements[0]), new Tuple<int, PrincipleItem>(1, ultimaData.PrincipleItemRequirements[1]), new Tuple<int, PrincipleItem>(2, ultimaData.PrincipleItemRequirements[2]) };
                values.Shuffle(random);

                for (int i = 0; i < values.Count(); i++)
                {
                    ultimaData.PrincipleItemRequirements[values[i].Item1].RequiredMask = values[(i + 1) % values.Count()].Item2.UsedMask;
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

                LogQuestItems(spoilerLog, ultimaData);
            }
            else
            {
                spoilerLog.Add(SpoilerCategory.Start, "No change to starting quest items.");
            }

            if (flags.Sextant)
            {
                ultimaData.StartingEquipment[3] = 0x01;
            }

            if (flags.KarmaSetPercentage > 0)
            {
                for (int virtue = 0; virtue < 8; virtue++)
                {
                    if (random.Next(0, 100) < flags.KarmaSetPercentage)
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

                if (sacrificeMantra.Text.Length == 2)
                {
                    numberOfTwos--;
                }
                else if (sacrificeMantra.Text.Length == 3)
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

                for (int i = 0; i < numberOfTwos; i++)
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

            if (flags.WordOfPassage)
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
                    ultimaData.HerbPrices[i] = (byte)(ultimaData.HerbPrices[i] * 3);
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
                if (!File.Exists(path + "\\TOWNMAP.SAV"))
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
                image.SaveAsPng($"{path}worldMap-{seed}.png");
                //image = worldMap.ToHeightMapImage();
                //if (image != null)
                //{
                //    image.SaveAsPng($"worldMap-hm-{seed}.png");
                //}
            }


            for (int i = 0; i < ultimaData.Items.Count; i++)
            {
                spoilerLog.Add(SpoilerCategory.Location, $"{ultimaData.ItemNames[i]} at {Talk.GetSextantText(ultimaData.Items[i], ' ')}");
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

            for (int i = 0; i < startingItems.Count; i++)
            {
                if ((ultimaData.StartingItems & (ushort)(ushortone << i)) != 0)
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
