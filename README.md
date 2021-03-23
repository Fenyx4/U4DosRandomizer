# U4DosRandomizer
Randomizer for Ultima IV
Currently this randomizer randomizes the over world map and deals with all the repercussions of the overworld map being randomized.

Requires Ultima IV which is available for free at https://www.gog.com/game/ultima_4.

1. Download the randomizer from https://github.com/Fenyx4/U4DosRandomizer/releases.
1. Unzip and drop the executable in the Ultima IV directory (same folder as the WORLD.MAP file).
1. Backup your original game. This has an option to restore but don't trust it we're still in alpha.
1. Run the executable.

```
Options:
  -s |--s <seed>     The seed for the randomizer. Same seed will produce the same map. Defaults to random value.
  -p |--p <path>     Path to Ultima 4 installation.  Leaving blank will assume it is the working directory.
  -r |--r            Restore original Ultima 4 files.
  --miniMap          Output a minimap of the overworld.
  -o |--overworld    Sets randomization level for Overworld map. 1 for no change. 2 for shuffle overworld locations. 5 for randomize the entire map. Defaults to 5.
  --spellRemove      Put in the letters of the spells you want removed. e.g. "--spellRemove zed" would remove zdown, energy field and dispel.
  --mixQuantity      Lets you input how much of a spell you want to mix.
  --dngStone         Randomize the location of stones in the dungeons
  --fixes            Collection of non-gameplay fixes.
  --hythlothFix      Fixes an issue with Hythloth dungeon room.
  --sleepLockAssist  Helps prevent sleeplock in battles.
  --activePlayer     Allow selecting which characters are active in combat.
  --appleHitChance   Change hit chance to behave like the original Apple II version.
  --diagonalAttack   Allow diagonal attacks in combat.
  --sacrificeFix     Adds a way to gain sacrifice which the shrine says should work.
  --questItems       Percentage chance to start with a quest item.
  --karmaValue       Value to override starting karma value for a virtue. Leave blank for random.
  --karmaPercentage  Percentage chance to override a starting karma value for a virtue. Default 0 (no override).
  --spoilerLog       Output a spoiler log.
  -? | -h | --help   Show help information
```
  
All the files the randomizer changes get backed up with the extension ".orig" added. Run with '-r' to restore to the original game.

Example: https://imgur.com/qNRxpSy

## Helping

```-b "E:\Projects\U4DosRandomizer\u4_decompile\SRC\ORIGINAL\AVATAR.EXE" -s "E:\Projects\U4DosRandomizer\u4_decompile\SRC\ORIGINAL\AVATAR.EXE.sig" -n "E:\Projects\U4DosRandomizer\u4_decompile\SRC\ORIGINAL\U4_MAIN.EXE" -d "E:\Projects\U4DosRandomizer\u4_decompile\SRC\ORIGINAL\AVATAR.EXE.octodiff"```
```-b "E:\Projects\U4DosRandomizer\u4_decompile\SRC-TITLE\ORIGINAL\TITLE.EXE.orig" -s "E:\Projects\U4DosRandomizer\u4_decompile\SRC-TITLE\ORIGINAL\TITLE.EXE.sig" -n "E:\Projects\U4DosRandomizer\u4_decompile\SRC-TITLE\ORIGINAL\TITLE_0.EXE" -d "E:\Projects\U4DosRandomizer\u4_decompile\SRC-TITLE\ORIGINAL\TITLE.EXE.octodiff"```

"The generation of random numbers is too important to be left to chance." - Robert R. Coveyou