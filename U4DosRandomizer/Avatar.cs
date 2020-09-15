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

        public void Load(string path, UltimaData data)
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

            var avatarStream = new System.IO.FileStream(file, System.IO.FileMode.Open);
            avatarBytes = avatarStream.ReadAllBytes();

            AvatarOffset = new AvatarOffsetsNew(avatarBytes, $"{file}.orig");

            for (int offset = 0; offset < 24; offset++)
            {
                var item = new Item();
                item.Location = avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5];
                item.X = avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5 + 1];
                item.Y = avatarBytes[AvatarOffset.ITEM_LOCATIONS_OFFSET + offset * 5 + 2];
                data.Items.Add(item);
            }

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

            data.SpellsRecipes = new List<byte>();
            for (int i = 0; i < 26; i++)
            {
                data.SpellsRecipes.Add(avatarBytes[AvatarOffset.SPELL_RECIPE_OFFSET + i]);
            }

            data.BlinkExclusionX1 = avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_X1_OFFSET];
            data.BlinkExclusionX2 = avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_X2_OFFSET];
            data.BlinkExclusionY1 = avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_Y1_OFFSET];
            data.BlinkExclusionY2 = avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_Y2_OFFSET];

            data.BlinkExclusion2X1 = avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_X1_OFFSET];
            data.BlinkExclusion2X2 = avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_X2_OFFSET];
            data.BlinkExclusion2Y1 = avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_Y1_OFFSET];
            data.BlinkExclusion2Y2 = avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_Y2_OFFSET];
        }

        internal static void Restore(string path)
        {
            var file = Path.Combine(path, filename);
            FileHelper.Restore(file);
        }

        public void Update(UltimaData data, Flags flags)
        {
            for (var offset = 0; offset < 24; offset++)
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
            for (byte offset = 0; offset < data.Moongates.Count; offset++)
            {
                avatarBytes[AvatarOffset.MOONGATE_X_OFFSET + offset] = data.Moongates[offset].X;
                avatarBytes[AvatarOffset.MOONGATE_Y_OFFSET + offset] = data.Moongates[offset].Y;
            }

            avatarBytes[AvatarOffset.AREA_X_OFFSET + AvatarOffset.LOC_LCB - 1] = data.LCB[0].X;
            avatarBytes[AvatarOffset.AREA_Y_OFFSET + AvatarOffset.LOC_LCB - 1] = data.LCB[0].Y;

            for (var offset = 0; offset < data.Castles.Count; offset++)
            {
                avatarBytes[AvatarOffset.AREA_X_OFFSET + AvatarOffset.LOC_CASTLES + offset] = data.Castles[offset].X;
                avatarBytes[AvatarOffset.AREA_Y_OFFSET + AvatarOffset.LOC_CASTLES + offset] = data.Castles[offset].Y;
            }

            for (var offset = 0; offset < data.Towns.Count; offset++)
            {
                avatarBytes[AvatarOffset.AREA_X_OFFSET + AvatarOffset.LOC_TOWNS + offset - 1] = data.Towns[offset].X;
                avatarBytes[AvatarOffset.AREA_Y_OFFSET + AvatarOffset.LOC_TOWNS + offset - 1] = data.Towns[offset].Y;
            }

            for (var offset = 0; offset < data.Shrines.Count; offset++)
            {
                // Skip Spirituality
                if (data.Shrines[offset] != null)
                {
                    avatarBytes[AvatarOffset.AREA_X_OFFSET + AvatarOffset.LOC_SHRINES + offset - 1] = data.Shrines[offset].X;
                    avatarBytes[AvatarOffset.AREA_Y_OFFSET + AvatarOffset.LOC_SHRINES + offset - 1] = data.Shrines[offset].Y;
                }
            }

            for (var offset = 0; offset < data.Dungeons.Count; offset++)
            {
                avatarBytes[AvatarOffset.AREA_X_OFFSET + AvatarOffset.LOC_DUNGEONS + offset - 1] = data.Dungeons[offset].X;
                avatarBytes[AvatarOffset.AREA_Y_OFFSET + AvatarOffset.LOC_DUNGEONS + offset - 1] = data.Dungeons[offset].Y;
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
                avatarBytes[AvatarOffset.SPELL_RECIPE_OFFSET + i] = data.SpellsRecipes[i];
            }

            // Cast exclusion isn't precise enough so allow them to cast anywhere and exclude the destination
            avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_X1_OFFSET] = 0x01;
            avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_X2_OFFSET] = 0x01;
            avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_Y1_OFFSET] = 0x01;
            avatarBytes[AvatarOffset.BLINK_CAST_EXCLUSION_Y2_OFFSET] = 0x01;

            avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_X1_OFFSET] = data.BlinkExclusionX1;
            avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_X2_OFFSET] = data.BlinkExclusionX2;
            avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_Y1_OFFSET] = data.BlinkExclusionY1;
            avatarBytes[AvatarOffset.BLINK_DESTINATION_EXCLUSION_Y2_OFFSET] = data.BlinkExclusionY2;

            avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_X1_OFFSET] = data.BlinkExclusion2X1;
            avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_X2_OFFSET] = data.BlinkExclusion2X2;
            avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_Y1_OFFSET] = data.BlinkExclusion2Y1;
            avatarBytes[AvatarOffset.BLINK_DESTINATION2_EXCLUSION_Y2_OFFSET] = data.BlinkExclusion2Y2;

            avatarBytes[AvatarOffset.ENABLE_MIX_QUANTITY_OFFSET] = flags.MixQuantity ? (byte)0x0 : (byte)0x9;
        }

        public void Save(string path)
        {
            var exePath = Path.Combine(path, filename);
            var avatarOut = new System.IO.BinaryWriter(new System.IO.FileStream(exePath, System.IO.FileMode.Truncate));

            //var binPath = Path.Combine(path, "AVATAR.bin");
            //var avatarOut2 = new System.IO.BinaryWriter(new System.IO.FileStream(binPath, System.IO.FileMode.Truncate));

            avatarOut.Write(avatarBytes);
            //avatarOut2.Write(avatarBytes);

            avatarOut.Close();
            //avatarOut2.Close();
        }

        private List<string> OriginalShrineText { get; set; }
        private List<int> OriginalShrineTextStartOffset { get; set; }
        public List<string> OriginalLBText { get; private set; }
        public List<int> OriginalLBTextStartOffset { get; private set; }
        public IAvatarOffset AvatarOffset { get; private set; }
    }
}

