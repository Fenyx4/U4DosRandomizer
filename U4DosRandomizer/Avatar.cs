using Octodiff.Core;
using Octodiff.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using U4DosRandomizer.Helpers;
using U4DosRandomizer.Resources;

namespace U4DosRandomizer
{
    public class Avatar
    {
        private const string filename = "AVATAR.EXE";
        private byte[] avatarBytes;

        public Avatar(SpoilerLog spoilerLog)
        {
            SpoilerLog = spoilerLog;
        }

        public void Load(string path, UltimaData data, IWorldMap worldMap)
        {
            var file = Path.Combine(path, filename);

            FileHelper.TryBackupOriginalFile(file);

            // Apply delta file to create new file
            var newFilePath2 = file;
            var newFileOutputDirectory = Path.GetDirectoryName(newFilePath2);
            if (!Directory.Exists(newFileOutputDirectory))
                Directory.CreateDirectory(newFileOutputDirectory);
            var deltaApplier = new DeltaApplier { SkipHashCheck = false };
            using (var basisStream = new FileStream($"{file}.orig", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var deltaStream = new MemoryStream(Patches.AVATAR_EXE))
                {
                    using (var newFileStream = new FileStream(newFilePath2, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, new ConsoleProgressReporter()), newFileStream);
                    }
                }
            }

            using (var avatarStream = new System.IO.FileStream(file, System.IO.FileMode.Open))
            {
                avatarBytes = avatarStream.ReadAllBytes();
            }

            AvatarOffset = new AvatarOffsetsNew(avatarBytes, $"{file}.orig");


            // Items
            var items = new List<Item>();
            for (int offset = 0; offset < 23; offset++)
            {
                items.Add(new Item(avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5],
                                    avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5 + 1],
                                    avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5 + 2]));
            }
            data.SetItems(items);

            // Moongates
            var moongates = new List<Tile>();
            for (byte offset = 0; offset < 8; offset++)
            {
                moongates.Add(worldMap.GetCoordinate(avatarBytes[AvatarOffset.MOONGATE_X_OFFSET + offset], avatarBytes[AvatarOffset.MOONGATE_Y_OFFSET + offset]));
            }
            data.SetMoongates(moongates);

            // LCB
            var lcb = new List<Tile>();
            var lcbLoc = worldMap.GetCoordinate(avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_LCB - 1], avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_LCB - 1]);
            lcb.Add(lcbLoc);
            lcb.Add(worldMap.GetCoordinate(lcbLoc.X - 1, lcbLoc.Y));
            lcb.Add(worldMap.GetCoordinate(lcbLoc.X + 1, lcbLoc.Y));
            data.SetLCB(lcb);

            // Castles
            var castles = new List<TileDirtyWrapper>();
            for (byte offset = 0; offset < 3; offset++)
            {
                castles.Add(new TileDirtyWrapper(worldMap.GetCoordinate(avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_CASTLES + offset], avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_CASTLES + offset]), worldMap));
            }
            data.SetCastles(castles);

            // Towns
            var towns = new List<TileDirtyWrapper>();
            for (byte offset = 0; offset < 8+4; offset++)
            {
                towns.Add(new TileDirtyWrapper(worldMap.GetCoordinate(avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_TOWNS + offset - 1], avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_TOWNS + offset - 1]), worldMap));
            }
            data.SetTowns(towns);

            // Shrines
            var shrines = new List<TileDirtyWrapper>();
            for (byte offset = 0; offset < 8; offset++)
            {
                shrines.Add(new TileDirtyWrapper(worldMap.GetCoordinate(avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_SHRINES + offset - 1], avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_SHRINES + offset - 1]), worldMap));
            }
            data.SetShrines(shrines);

            // Dungeons
            var dungeons = new List<Tile>();
            for (byte offset = 0; offset < 8; offset++)
            {
                dungeons.Add(worldMap.GetCoordinate(avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_DUNGEONS + offset - 1], avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_DUNGEONS + offset - 1]));
            }
            data.SetDungeons(dungeons);

            // Balloon Spawn
            data.BalloonSpawn = worldMap.GetCoordinate(avatarBytes[AvatarOffset.BALLOON_SPAWN_LOCATION_X_OFFSET], avatarBytes[AvatarOffset.BALLOON_SPAWN_LOCATION_Y_OFFSET]);

            OriginalShrineText = new List<string>();
            OriginalShrineTextStartOffset = new List<int>();
            var shrineTextBytes = new List<byte>();
            var textOffset = AvatarOffset.SHRINE_TEXT_OFFSET;
            for (int i = 0; i < 24; i++)
            {
                OriginalShrineTextStartOffset.Add(textOffset);
                for (; avatarBytes[textOffset] != 0x0A && avatarBytes[textOffset] != 0x00; textOffset++)
                {
                    shrineTextBytes.Add(avatarBytes[textOffset]);
                }
                OriginalShrineText.Add(System.Text.Encoding.Default.GetString(shrineTextBytes.ToArray()));
                shrineTextBytes.Clear();
                if (avatarBytes[textOffset] == 0x0A)
                {
                    textOffset++;
                }
                textOffset++;
            }
            data.ShrineText.Clear();
            data.ShrineText.AddRange(OriginalShrineText);

            OriginalLBText = new List<string>();
            OriginalLBTextStartOffset = new List<int>();
            var lbTextBytes = new List<byte>();
            textOffset = AvatarOffset.LB_TEXT_OFFSET;
            // He has more text than 19 but there is some weird stuff after 19 that doesn't get turned into text well. And as far as I can tell we won't need any text after 19
            for (int i = 0; i < 19; i++)
            {
                OriginalLBTextStartOffset.Add(textOffset);
                for (; avatarBytes[textOffset] != 0x00 && avatarBytes[textOffset] != 0xAB; textOffset++)
                {
                    lbTextBytes.Add(avatarBytes[textOffset]);
                }
                OriginalLBText.Add(System.Text.Encoding.Default.GetString(lbTextBytes.ToArray()));
                lbTextBytes.Clear();
                if (avatarBytes[textOffset] == 0x0A || avatarBytes[textOffset] == 0xAB)
                {
                    textOffset++;
                }
                textOffset++;
            }
            data.LBText.Clear();
            data.LBText.AddRange(OriginalLBText);

            var mantraTextBytes = new List<byte>();
            textOffset = AvatarOffset.MANTRA_OFFSET;
            MantraMaxSize = 0;
            for (int i = 0; i < 8; i++)
            {
                for (; avatarBytes[textOffset] != 0x00; textOffset++)
                {
                    mantraTextBytes.Add(avatarBytes[textOffset]);
                }
                data.Mantras.Add(System.Text.Encoding.Default.GetString(mantraTextBytes.ToArray()));
                MantraMaxSize += data.Mantras[i].Length + 1;
                mantraTextBytes.Clear();

                textOffset++;
            }

            var wordOfPassageTextBytes = new List<byte>();

            for (int offSet = 0; offSet < 9; offSet++)
            {
                wordOfPassageTextBytes.Add(avatarBytes[AvatarOffset.WORD_OF_PASSAGE+offSet]);
            }
            data.WordOfPassage = System.Text.Encoding.Default.GetString(wordOfPassageTextBytes.ToArray());

            data.DaemonSpawnX1 = avatarBytes[AvatarOffset.DEMON_SPAWN_TRIGGER_X1_OFFSET];
            data.DaemonSpawnX2 = avatarBytes[AvatarOffset.DEMON_SPAWN_TRIGGER_X2_OFFSET];
            data.DaemonSpawnY1 = avatarBytes[AvatarOffset.DEMON_SPAWN_TRIGGER_Y1_OFFSET];
            data.DaemonSpawnY2 = avatarBytes[AvatarOffset.DEMON_SPAWN_TRIGGER_Y2_OFFSET];
            data.DaemonSpawnLocationX = avatarBytes[AvatarOffset.DEMON_SPAWN_LOCATION_X_OFFSET];

            for(int i = 0; i < 8; i++)
            {
                data.PirateCove.Add(new Coordinate(avatarBytes[i + AvatarOffset.PIRATE_COVE_X_OFFSET], avatarBytes[i + AvatarOffset.PIRATE_COVE_Y_OFFSET]));
            }

            data.PirateCoveSpawnTrigger = new Coordinate(avatarBytes[AvatarOffset.PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1], avatarBytes[AvatarOffset.PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1]);

            data.WhirlpoolExit = new Coordinate(avatarBytes[AvatarOffset.WHIRLPOOL_EXIT_X_OFFSET], avatarBytes[AvatarOffset.WHIRLPOOL_EXIT_Y_OFFSET]);

            data.SpellsRecipes = new List<ByteDirtyWrapper>();
            for (int i = 0; i < 26; i++)
            {
                data.SpellsRecipes.Add(new ByteDirtyWrapper(avatarBytes[AvatarOffset.SPELL_RECIPE_OFFSET + i]));
            }

            data.BlinkCastExclusionX1 = avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_X1_OFFSET];
            data.BlinkCastExclusionX2 = avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_X2_OFFSET];
            data.BlinkCastExclusionY1 = avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_Y1_OFFSET];
            data.BlinkCastExclusionY2 = avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_Y2_OFFSET];

            data.BlinkDestinationExclusionX1 = avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_X1_OFFSET];
            data.BlinkDestinationExclusionX2 = avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_X2_OFFSET];
            data.BlinkDestinationExclusionY1 = avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_Y1_OFFSET];
            data.BlinkDestinationExclusionY2 = avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_Y2_OFFSET];
                      
            data.BlinkDestinationExclusion2X1 = avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_X1_OFFSET];
            data.BlinkDestinationExclusion2X2 = avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_X2_OFFSET];
            data.BlinkDestinationExclusion2Y1 = avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_Y1_OFFSET];
            data.BlinkDestinationExclusion2Y2 = avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_Y2_OFFSET];

            for (int i = 0; i < 13; i++)
            {
                data.AbyssEjectionLocations.Add(new Coordinate(avatarBytes[i + AvatarOffset.ABYSS_EJECTION_LOCATIONS_X], avatarBytes[i + AvatarOffset.ABYSS_EJECTION_LOCATIONS_Y]));
            }

            for (int townIdx = 0; townIdx < 16; townIdx++)
            {
                data.ShopLocations.Add(new List<byte>());
                for(int shopIdx = 0; shopIdx < 8; shopIdx++)
                {
                    data.ShopLocations[townIdx].Add(avatarBytes[townIdx * 8 + shopIdx + AvatarOffset.SHOP_LOCATION_OFFSET]);
                }
            }
        }

        internal static void Restore(string path)
        {
            var file = Path.Combine(path, filename);
            FileHelper.Restore(file);
        }

        public void Update(UltimaData data, Flags flags)
        {
            // Items
            for (var offset = 0; offset < data.Items.Count; offset++)
            {
                avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5] = data.Items[offset].Location;
                avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5 + 1] = data.Items[offset].X;
                avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5 + 2] = data.Items[offset].Y;
            }

            // Use these items at the entrance to the abyss
            avatarBytes[AvatarOffset.ITEM_USE_TRIGGER_BELL_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].X;
            avatarBytes[AvatarOffset.ITEM_USE_TRIGGER_BELL_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].Y;
            avatarBytes[AvatarOffset.ITEM_USE_TRIGGER_BOOK_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].X;
            avatarBytes[AvatarOffset.ITEM_USE_TRIGGER_BOOK_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].Y;
            avatarBytes[AvatarOffset.ITEM_USE_TRIGGER_CANDLE_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].X;
            avatarBytes[AvatarOffset.ITEM_USE_TRIGGER_CANDLE_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].Y;
            avatarBytes[AvatarOffset.ITEM_USE_TRIGGER_SKULL_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].X;
            avatarBytes[AvatarOffset.ITEM_USE_TRIGGER_SKULL_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 1].Y;

            ////throw in a lava to make it easy to find
            //for (int offset = 0; offset < 8; offset++)
            //{
            //    worldMapUlt[200, 200 + offset] = 76;
            //}
            // Moongates
            for (byte offset = 0; offset < data.Moongates.Count; offset++)
            {
                avatarBytes[AvatarOffset.MOONGATE_X_OFFSET + offset] = data.Moongates[offset].X;
                avatarBytes[AvatarOffset.MOONGATE_Y_OFFSET + offset] = data.Moongates[offset].Y;
            }

            avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_LCB - 1] = data.LCB[0].X;
            avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_LCB - 1] = data.LCB[0].Y;

            avatarBytes[AvatarOffset.DEATH_EXIT_X_OFFSET] = data.LCB[0].X;
            avatarBytes[AvatarOffset.DEATH_EXIT_Y_OFFSET] = data.LCB[0].Y;

            for (var offset = 0; offset < data.Castles.Count; offset++)
            {
                avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_CASTLES + offset] = data.Castles[offset].X;
                avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_CASTLES + offset] = data.Castles[offset].Y;
            }

            for (var offset = 0; offset < data.Towns.Count; offset++)
            {
                avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_TOWNS + offset - 1] = data.Towns[offset].X;
                avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_TOWNS + offset - 1] = data.Towns[offset].Y;
            }

            for (var offset = 0; offset < data.Shrines.Count; offset++)
            {
                // Skip Spirituality
                if (data.Shrines[offset] != null)
                {
                    avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_SHRINES + offset - 1] = data.Shrines[offset].X;
                    avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_SHRINES + offset - 1] = data.Shrines[offset].Y;
                }
            }

            for (var offset = 0; offset < data.Dungeons.Count; offset++)
            {
                avatarBytes[AvatarOffset.AREA_X_OFFSET + data.LOC_DUNGEONS + offset - 1] = data.Dungeons[offset].X;
                avatarBytes[AvatarOffset.AREA_Y_OFFSET + data.LOC_DUNGEONS + offset - 1] = data.Dungeons[offset].Y;
            }

            avatarBytes[AvatarOffset.BALLOON_SPAWN_TRIGGER_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 2].X;
            avatarBytes[AvatarOffset.BALLOON_SPAWN_TRIGGER_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 2].Y;
            avatarBytes[AvatarOffset.LBC_DUNGEON_EXIT_X_OFFSET] = data.Dungeons[data.Dungeons.Count - 2].X;
            avatarBytes[AvatarOffset.LBC_DUNGEON_EXIT_Y_OFFSET] = data.Dungeons[data.Dungeons.Count - 2].Y;

            avatarBytes[AvatarOffset.BALLOON_SPAWN_LOCATION_X_OFFSET] = data.BalloonSpawn.X;
            avatarBytes[AvatarOffset.BALLOON_SPAWN_LOCATION_Y_OFFSET] = data.BalloonSpawn.Y;

            var avatarBytesList = new List<byte>(avatarBytes);
            for (int i = 0; i < OriginalShrineText.Count; i++)
            {
                if (data.ShrineText[i].Length > OriginalShrineText[i].Length)
                {
                    throw new Exception($"Shrine text \"{data.ShrineText[i]}\" is too long.");
                }
                data.ShrineText[i] = data.ShrineText[i].PadRight(OriginalShrineText[i].Length, ' ');
                
                avatarBytesList.RemoveRange(OriginalShrineTextStartOffset[i], OriginalShrineText[i].Length);
                avatarBytesList.InsertRange(OriginalShrineTextStartOffset[i], Encoding.ASCII.GetBytes(data.ShrineText[i]));

            }

            for (int i = 0; i < OriginalLBText.Count; i++)
            {
                if (data.LBText[i].Length > OriginalLBText[i].Length)
                {
                    throw new Exception($"LB text \"{data.LBText[i]}\" is too long.");
                }
                data.LBText[i] = data.LBText[i].PadRight(OriginalLBText[i].Length, ' ');

                avatarBytesList.RemoveRange(OriginalLBTextStartOffset[i], OriginalLBText[i].Length);
                avatarBytesList.InsertRange(OriginalLBTextStartOffset[i], Encoding.ASCII.GetBytes(data.LBText[i]));

            }
            avatarBytes = avatarBytesList.ToArray();

            var currentMantraOffset = 0;
            var mantraSize = 0;
            for(int i = 0; i < data.Mantras.Count; i++)
            {
                avatarBytes[AvatarOffset.MANTRA_POINTERS_OFFSET+i*2] = (byte)(avatarBytes[AvatarOffset.MANTRA_POINTERS_OFFSET] + mantraSize);
                mantraSize += data.Mantras[i].Length + 1;

                var textBytes = Encoding.ASCII.GetBytes(data.Mantras[i]);
                for(int j = 0; j < textBytes.Length; j++)
                {
                    avatarBytes[AvatarOffset.MANTRA_OFFSET + currentMantraOffset] = textBytes[j];
                    currentMantraOffset++;
                }
                avatarBytes[AvatarOffset.MANTRA_OFFSET + currentMantraOffset] = 0x00;
                currentMantraOffset++;

                if (currentMantraOffset > MantraMaxSize)
                {
                    throw new Exception($"Mantra text is too long.");
                }
            }

            var wordOfPassageBytes = Encoding.ASCII.GetBytes(data.WordOfPassage.ToLower());
            for (int j = 0; j < wordOfPassageBytes.Length; j++)
            {
                avatarBytes[AvatarOffset.WORD_OF_PASSAGE + j] = wordOfPassageBytes[j];
            }


            avatarBytes[AvatarOffset.DEMON_SPAWN_TRIGGER_X1_OFFSET] = data.DaemonSpawnX1;
            avatarBytes[AvatarOffset.DEMON_SPAWN_TRIGGER_X2_OFFSET] = data.DaemonSpawnX2;
            avatarBytes[AvatarOffset.DEMON_SPAWN_TRIGGER_Y1_OFFSET] = data.DaemonSpawnY1;
            avatarBytes[AvatarOffset.DEMON_SPAWN_TRIGGER_Y2_OFFSET] = data.DaemonSpawnY2;
            avatarBytes[AvatarOffset.DEMON_SPAWN_LOCATION_X_OFFSET] = data.DaemonSpawnLocationX;

            for(int i = 0; i < data.PirateCove.Count; i++)
            {
                avatarBytes[AvatarOffset.PIRATE_COVE_X_OFFSET + i] = data.PirateCove[i].X;
                avatarBytes[AvatarOffset.PIRATE_COVE_Y_OFFSET + i] = data.PirateCove[i].Y;
            }

            avatarBytes[AvatarOffset.PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET1] = data.PirateCoveSpawnTrigger.X;
            avatarBytes[AvatarOffset.PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET1] = data.PirateCoveSpawnTrigger.Y;
            avatarBytes[AvatarOffset.PIRATE_COVE_SPAWN_TRIGGER_X_OFFSET2] = data.PirateCoveSpawnTrigger.X;
            avatarBytes[AvatarOffset.PIRATE_COVE_SPAWN_TRIGGER_Y_OFFSET2] = data.PirateCoveSpawnTrigger.Y;

            avatarBytes[AvatarOffset.WHIRLPOOL_EXIT_X_OFFSET] = data.WhirlpoolExit.X;
            avatarBytes[AvatarOffset.WHIRLPOOL_EXIT_Y_OFFSET] = data.WhirlpoolExit.Y;

            for (int i = 0; i < data.SpellsRecipes.Count; i++)
            {
                avatarBytes[AvatarOffset.SPELL_RECIPE_OFFSET + i] = data.SpellsRecipes[i].Byte;
            }

            // Cast exclusion isn't precise enough so allow them to cast anywhere and exclude the destination
            avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_X1_OFFSET] = data.BlinkCastExclusionX1;
            avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_X2_OFFSET] = data.BlinkCastExclusionX2;
            avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_Y1_OFFSET] = data.BlinkCastExclusionY1;
            avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_Y2_OFFSET] = data.BlinkCastExclusionY2;

            avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_X1_OFFSET] = data.BlinkDestinationExclusionX1;
            avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_X2_OFFSET] = data.BlinkDestinationExclusionX2;
            avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_Y1_OFFSET] = data.BlinkDestinationExclusionY1;
            avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_Y2_OFFSET] = data.BlinkDestinationExclusionY2;

            avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_X1_OFFSET] = data.BlinkDestinationExclusion2X1;
            avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_X2_OFFSET] = data.BlinkDestinationExclusion2X2;
            avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_Y1_OFFSET] = data.BlinkDestinationExclusion2Y1;
            avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_Y2_OFFSET] = data.BlinkDestinationExclusion2Y2;

            avatarBytes[AvatarOffset.ENABLE_MIX_QUANTITY_OFFSET] = flags.MixQuantity ? (byte)0x0 : (byte)0x9;
            if (flags.MixQuantity)
            {
                SpoilerLog.Add(SpoilerCategory.Feature, $"Mix Quantity Enabled");
            }

            avatarBytes[AvatarOffset.ENABLE_SLEEP_BACKOFF_OFFSET] = flags.SleepLockAssist ? (byte)0x0 : (byte)0x9;
            if (flags.SleepLockAssist)
            {
                SpoilerLog.Add(SpoilerCategory.Feature, $"Sleep Lock Assist Enabled");
            }

            avatarBytes[AvatarOffset.ENABLE_ACTIVE_PLAYER_1_OFFSET] = flags.ActivePlayer ? (byte)0x0 : (byte)0x9;
            if (flags.ActivePlayer)
            {
                SpoilerLog.Add(SpoilerCategory.Feature, $"Active Player Enabled");
            }

            avatarBytes[AvatarOffset.ENABLE_HIT_CHANCE_OFFSET] = flags.HitChance ? (byte)0x0 : (byte)0x9;
            if (flags.HitChance)
            {
                SpoilerLog.Add(SpoilerCategory.Feature, $"Apple II Hit Chance Enabled");
            }

            avatarBytes[AvatarOffset.ENABLE_DIAGONAL_ATTACK_OFFSET] = flags.DiagonalAttack ? (byte)0x0 : (byte)0x9;
            if (flags.DiagonalAttack)
            {
                SpoilerLog.Add(SpoilerCategory.Feature, $"Diagonal Attack Enabled");
            }

            avatarBytes[AvatarOffset.ENABLE_SACRIFICE_FIX_OFFSET] = flags.SacrificeFix ? (byte)0x0 : (byte)0x9;
            if (flags.SacrificeFix)
            {
                SpoilerLog.Add(SpoilerCategory.Fix, $"Sacrifice Fix Enabled");
            }

            for (int i = 0; i < data.AbyssEjectionLocations.Count; i++)
            {
                //Console.WriteLine(Talk.GetSextantText(data.AbyssEjectionLocations[i]));
                avatarBytes[AvatarOffset.ABYSS_EJECTION_LOCATIONS_X + i] = data.AbyssEjectionLocations[i].X;
                avatarBytes[AvatarOffset.ABYSS_EJECTION_LOCATIONS_Y + i] = data.AbyssEjectionLocations[i].Y;
            }

            for (int townIdx = 0; townIdx < 16; townIdx++)
            {
                for (int shopIdx = 0; shopIdx < 8; shopIdx++)
                {
                    avatarBytes[townIdx * 8 + shopIdx + AvatarOffset.SHOP_LOCATION_OFFSET] = data.ShopLocations[townIdx][shopIdx];
                }
            }

            var encodeBytes = Encoding.ASCII.GetBytes(flags.GetEncoded());
            for (int encodeIdx = 0; encodeIdx < encodeBytes.Length; encodeIdx++)
            {
                avatarBytes[AvatarOffset.ENCODED_FLAGS_OFFSET + encodeIdx] = encodeBytes[encodeIdx];
            }

            var seedBytes = Encoding.ASCII.GetBytes(flags.Seed.ToString());
            for (int seedIdx = 0; seedIdx < seedBytes.Length; seedIdx++)
            {
                avatarBytes[AvatarOffset.SEED_OFFSET + seedIdx] = seedBytes[seedIdx];
            }

            if (flags.Runes)
            {
                for (int i = 0; i < 8; i++)
                {
                    avatarBytes[AvatarOffset.CITY_RUNE_MASK_PAIRS_OFFSET + i * 2] = data.Items[i + UltimaData.ITEM_RUNE_HONESTY].Location;
                }
            }

            if (flags.MonsterDamage != 2)
            {
                avatarBytes[AvatarOffset.MONSTER_DAMAGE_BITSHIFT_OFFSET] = 0xB1;
                avatarBytes[AvatarOffset.MONSTER_DAMAGE_BITSHIFT_OFFSET+1] = (byte)flags.MonsterDamage;
                avatarBytes[AvatarOffset.MONSTER_DAMAGE_BITSHIFT_OFFSET+2] = 0xD3;
            }

            if(flags.WeaponDamage != 2)
            {
                var multiplier = 1.0f;
                switch (flags.WeaponDamage)
                {
                    case 1:
                        multiplier = 1.5f;
                        break;
                    case 3:
                        multiplier = 0.5f;
                        break;
                }

                for (int i = 0; i < 16; i++)
                {
                    var originalDamage = avatarBytes[AvatarOffset.WEAPON_DAMAGE_OFFSET + i];
                    var newDamage = avatarBytes[AvatarOffset.WEAPON_DAMAGE_OFFSET + i] * multiplier;
                    avatarBytes[AvatarOffset.WEAPON_DAMAGE_OFFSET + i] = (byte)Math.Max(0x01, Math.Min(0xFF, avatarBytes[AvatarOffset.WEAPON_DAMAGE_OFFSET + i] * multiplier));
                }
            }

            if (flags.EarlierMonsters)
            {
                ushort tierCutover = 1000;
                var tierCutoverBytes = BitConverter.GetBytes(tierCutover);
                for (int offset = 0; offset < tierCutoverBytes.Length; offset++)
                {
                    avatarBytes[AvatarOffset.MONSTER_SPAWN_TIER_ONE + offset] = tierCutoverBytes[offset];
                }

                tierCutover = 2000;
                tierCutoverBytes = BitConverter.GetBytes(tierCutover);
                for (int offset = 0; offset < tierCutoverBytes.Length; offset++)
                {
                    avatarBytes[AvatarOffset.MONSTER_SPAWN_TIER_TWO + offset] = tierCutoverBytes[offset];
                }

                tierCutover = 3000;
                tierCutoverBytes = BitConverter.GetBytes(tierCutover);
                for (int offset = 0; offset < tierCutoverBytes.Length; offset++)
                {
                    avatarBytes[AvatarOffset.MONSTER_SPAWN_TIER_THREE + offset] = tierCutoverBytes[offset];
                }
            }

            if (flags.MonsterQty)
            {
                var cmd = new byte[] { 0xB8, 0x10, 0x00, 0x90, 0x90 };
                for(int i = 0; i < cmd.Length; i++)
                {
                    avatarBytes[AvatarOffset.MONSTER_QTY_ONE + i] = cmd[i];
                }

                cmd = new byte[] { 0x3D, 0x08, 0x00, 0x7D, 0xE4, 0x90 };
                for (int i = 0; i < cmd.Length; i++)
                {
                    avatarBytes[AvatarOffset.MONSTER_QTY_TWO + i] = cmd[i];
                }
            }
        }

        public void Save(string path)
        {
            var exePath = Path.Combine(path, filename);
            using (var avatarOut = new System.IO.BinaryWriter(new System.IO.FileStream(exePath, System.IO.FileMode.Truncate)))
            {
                avatarOut.Write(avatarBytes);
            }
        }

        private List<string> OriginalShrineText { get; set; }
        private List<int> OriginalShrineTextStartOffset { get; set; }
        public List<string> OriginalLBText { get; private set; }
        public List<int> OriginalLBTextStartOffset { get; private set; }
        public int MantraMaxSize { get; private set; }
        public IAvatarOffset AvatarOffset { get; private set; }
        private SpoilerLog SpoilerLog { get; }
    }
}

