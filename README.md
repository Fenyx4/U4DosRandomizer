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
  -s |--s <seed>    The seed for the randomizer.  Same seed will produce the same map. Defaults to random value.
  -p |--p <path>    Path to Ultima 4 installation.  Leaving blank will assume it is the working directory.
  -r |--r           Restore original Ultima 4 files.
  --miniMap         Output a minimap of the overworld.
  -o |--overworld   Sets randomization level for Overworld map. 1 - No change. 2 - Shuffle castles, towns, moongates, shrines, items. 5 - Randomly generate whole map. Defaults to 5.
  --spellRemove     Put in the letters of the spells you want removed. e.g. "--spellRemove zed" would remove zdown, energy field and dispel.
  --mixQuantity     Lets you input how much of a spell you want to mix.
  --dngStone        Randomize the location of stones in the dungeons
  -? | -h | --help  Show help information
```
  
All the files the randomizer changes get backed up with the extension ".orig" added. Run with '-r' to restore to the original game.

Example: https://imgur.com/qNRxpSy

"The generation of random numbers is too important to be left to chance." - Robert R. Coveyou