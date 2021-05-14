using System;
using System.Collections.Generic;

namespace U4DosRandomizer
{
    public class Flags
    {
        public int Seed { get; set; }
        public int Version { get; }
        public Flags(int seed, int version)
        {
            Seed = seed;
            Version = version;
        }

        public bool MiniMap { get; internal set; }
        public string SpellRemove { get; internal set; }
        public bool DngStone { get; internal set; }
        public bool MixQuantity { get; internal set; }
        public int Overworld { get; internal set; }
        public bool Fixes { get; internal set; }
        public int QuestItemPercentage { get; internal set; }
        public int KarmaSetPercentage { get; internal set; }
        public int? KarmaValue { get; internal set; }
        public bool FixHythloth { get; internal set; }
        public bool SleepLockAssist { get; internal set; }
        public bool ActivePlayer { get; internal set; }
        public bool HitChance { get; internal set; }
        public bool DiagonalAttack { get; internal set; }
        public bool SacrificeFix { get; internal set; }
        public bool Runes { get; internal set; }
        public bool Mantras { get; internal set; }
        public bool WordOfPassage { get; internal set; }
        public int MonsterDamage { get; internal set; }
        public int WeaponDamage { get; internal set; }
        public bool EarlierMonsters { get; internal set; }
        public bool RandomizeSpells { get; internal set; }
        public bool Sextant { get; internal set; }

        public List<int> SupportedVersions = new List<int>() { 9 };

        public string GetEncoded()
        {
            var encoded = new List<byte>();

            encoded.Add((byte)Version);

            encoded.AddRange(BitConverter.GetBytes(Seed));

            var mask = 0;
            mask = SET_MSK(mask, MiniMap, 0);
            mask = SET_MSK(mask, DngStone, 1);
            mask = SET_MSK(mask, MixQuantity, 2);
            mask = SET_MSK(mask, Fixes, 3);
            mask = SET_MSK(mask, FixHythloth, 4);
            mask = SET_MSK(mask, SleepLockAssist, 5);
            mask = SET_MSK(mask, ActivePlayer, 6);
            mask = SET_MSK(mask, HitChance, 7);
            encoded.Add((byte)mask);

            mask = 0;
            mask = SET_MSK(mask, DiagonalAttack, 0);
            mask = SET_MSK(mask, SacrificeFix, 1);
            mask = SET_MSK(mask, Runes, 2);
            mask = SET_MSK(mask, Mantras, 3);
            mask = SET_MSK(mask, WordOfPassage, 4);
            mask = SET_MSK(mask, EarlierMonsters, 5);
            mask = SET_MSK(mask, RandomizeSpells, 6);
            mask = SET_MSK(mask, Sextant, 7);
            encoded.Add((byte)mask);

            encoded.Add((byte)Overworld);
            encoded.Add((byte)QuestItemPercentage);
            encoded.Add((byte)KarmaSetPercentage);
            encoded.Add((byte)(KarmaValue.HasValue ? KarmaValue.Value + 1 : 0));
            encoded.Add((byte)MonsterDamage);
            encoded.Add((byte)WeaponDamage);

            var sr = (SpellRemove == null ? "" : SpellRemove).ToLower();
            var spellRemoveMask = 0;
            for (int idx = 0; idx < sr.Length; idx++)
            {
                spellRemoveMask = SET_MSK(spellRemoveMask, true, sr[idx] - 'a');
            }

            encoded.AddRange(BitConverter.GetBytes(spellRemoveMask));

            return Convert.ToBase64String(encoded.ToArray());
        }

        public void DecodeAndSet(string encodedString)
        {
            var encoded = Convert.FromBase64String(encodedString);

            var ver = encoded[0];
            if(!SupportedVersions.Contains(ver))
            {
                throw new Exception("These encoded flags are not compatible with this version.");
            }

            Seed = BitConverter.ToInt32(encoded, 1);

            var mask = encoded[5];
            MiniMap = TST_MSK(mask, 0);
            DngStone = TST_MSK(mask, 1);
            MixQuantity = TST_MSK(mask, 2);
            Fixes = TST_MSK(mask, 3);
            FixHythloth = TST_MSK(mask, 4);
            SleepLockAssist = TST_MSK(mask, 5);
            ActivePlayer = TST_MSK(mask, 6);
            HitChance = TST_MSK(mask, 7);
            mask = encoded[6];
            DiagonalAttack = TST_MSK(mask, 0);
            SacrificeFix = TST_MSK(mask, 1);
            Runes = TST_MSK(mask, 2);
            Mantras = TST_MSK(mask, 3);
            WordOfPassage = TST_MSK(mask, 4);
            EarlierMonsters = TST_MSK(mask, 5);
            RandomizeSpells = TST_MSK(mask, 6);
            Sextant = TST_MSK(mask, 7);

            Overworld = encoded[7];
            QuestItemPercentage = encoded[8];
            KarmaSetPercentage = encoded[9];
            KarmaValue = encoded[10];
            if (KarmaValue == 0)
            {
                KarmaValue = null;
            }
            else
            {
                KarmaValue--;
            }
            MonsterDamage = encoded[11];
            WeaponDamage = encoded[12];

            var spellRemoveMask = BitConverter.ToInt32(encoded, 13);
            SpellRemove = "";
            for (int offset = 0; offset < 26; offset++)
            {
                if(TST_MSK(spellRemoveMask, offset))
                {
                    SpellRemove += (char)('a' + offset);
                }
            }
        }

        private static int SET_MSK(int mask, bool bit, int offset)
        {
            return mask |= ((bit ? 1 : 0) << offset);
        }

        public static bool TST_MSK(int mask, int offset)
        {
            return (mask & (1 << offset)) != 0;
        }
    }
}