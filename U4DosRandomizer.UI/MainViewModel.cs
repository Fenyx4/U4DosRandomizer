using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls.Shapes;
using System.Windows.Input;

namespace U4DosRandomizer.UI
{

    public partial class MainViewModel : ObservableObject
    {
        readonly IFolderPicker folderPicker;
        readonly IPopupService popupService;

        public MainViewModel(IFolderPicker folderPicker, IPopupService popupService)
        {
            Clicked = new Command(OnClicked);
            seed = "";
            ultimaIVInstallPath = "";
            this.folderPicker = folderPicker;
            this.popupService = popupService;
        }

        [ObservableProperty]
        public bool generateClothMap;

        [ObservableProperty]
        public bool miniMap;

        [ObservableProperty]
        public bool vgaPatch;
        
        [ObservableProperty]
        public bool spoilerLog;

        [ObservableProperty]
        public string seed;

        [ObservableProperty]
        public string ultimaIVInstallPath;

        // -----------

        // Randomize

        [ObservableProperty]
        public int overWorld = 5;

        [ObservableProperty]
        public bool startingWeapons;

        [ObservableProperty]
        public int dungeon = 1;

        [ObservableProperty]
        public bool runes;

        [ObservableProperty]
        public bool mystics;

        [ObservableProperty]
        public bool mantras;

        [ObservableProperty]
        public bool wordOfPassage;

        [ObservableProperty]
        public bool randomizeSpells;

        [ObservableProperty]
        public bool principleItems;

        // ----------- Difficulty -------------

        [ObservableProperty]
        public string spellRemove;

        [ObservableProperty]
        public bool sleepLockAssist;

        [ObservableProperty]
        public bool awakenUpgrade;

        [ObservableProperty]
        public bool diagonalAttack;

        [ObservableProperty]
        public bool appleHitChance;

        [ObservableProperty]
        public int monsterDamage = 2;

        [ObservableProperty]
        public int weaponDamage = 0;

        [ObservableProperty]
        public int questItems;

        [ObservableProperty]
        public bool earlierMonsters;

        [ObservableProperty]
        public int karmaValue;

        [ObservableProperty]
        public bool karmaValueOn;

        [ObservableProperty]
        public int karmaPercentage;

        [ObservableProperty]
        public bool monsterQty;

        [ObservableProperty]
        public bool noRequireFullParty;

        [ObservableProperty]
        public bool requireMystics;

        [ObservableProperty]
        public bool sextant;

        [ObservableProperty]
        public int herbPrice;

        // ---------- Quality of life -------------

        [ObservableProperty]
        public bool mixQuantity;

        [ObservableProperty]
        public bool activePlayer;

        [ObservableProperty]
        public bool townSaves;

        [ObservableProperty]
        public bool other;

        // ---------- Bug Fixes --------

        [ObservableProperty]
        public bool fixes;

        [ObservableProperty]
        public bool hythlothFix;

        [ObservableProperty]
        public bool sacrificeFix;

        [ObservableProperty]
        public bool daemonTrigger;

        [ObservableProperty]
        public bool shopOverflow;






        public ICommand Clicked { get; }

        [RelayCommand]
        async Task PickFolder(CancellationToken cancellationToken)
        {
            var folderPickerResult = await folderPicker.PickAsync(cancellationToken);
            if (folderPickerResult.IsSuccessful)
            {
                UltimaIVInstallPath = folderPickerResult.Folder.Path;
                //await Toast.Make($"Folder picked: Name - {folderPickerResult.Folder.Name}, Path - {folderPickerResult.Folder.Path}", ToastDuration.Long).Show(cancellationToken);
            }
            //else
            //{
            //    await Toast.Make($"Folder is not picked, {folderPickerResult.Exception.Message}").Show(cancellationToken);
            //}
        }

        public async void OnClicked()
        {
            var seedInt = Environment.TickCount;
            if ( !string.IsNullOrEmpty(seed) )
            {
                seedInt = int.Parse(seed);
            }


            int? karmaValueInt = null;
            if ( karmaValueOn)
            {
                karmaValueInt = karmaValue;
            }

            var path = ultimaIVInstallPath;
            if (!Directory.Exists(path))
            {
                popupService.ShowPopup("Path provided for game directory does not exist");
                return;
            }
            //path = "F:\\Gog-games\\Ultima 4";
            if (!path.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) &&
                    !path.EndsWith(System.IO.Path.AltDirectorySeparatorChar.ToString()))
            {
                path = path + System.IO.Path.DirectorySeparatorChar;
            }

            if (!File.Exists(System.IO.Path.Combine(path, "WORLD.MAP")))
            {
                popupService.ShowPopup("Could not find WORLD.MAP please provide path to the game's install directory.");
                return;
            }

            //if (restoreArg.HasValue())
            //{
            //    Restore(path);
            //}
            //else
            //{
            Flags flags = new(seedInt, 9);
            flags.Overworld = overWorld;
            flags.MiniMap = miniMap;
            flags.Dungeon = dungeon;
            flags.SpellRemove = spellRemove;
            flags.StartingWeapons = startingWeapons;
            flags.DngStone = false; // deprecated
            flags.MixQuantity = mixQuantity;
            flags.Fixes = fixes;
            flags.FixHythloth = hythlothFix;
            flags.SleepLockAssist = sleepLockAssist;
            flags.ActivePlayer = activePlayer;
            flags.HitChance = appleHitChance;
            flags.DiagonalAttack = diagonalAttack;
            flags.SacrificeFix = sacrificeFix;
            flags.Runes = runes;
            flags.Mystics = mystics;
            flags.Mantras = mantras;
            flags.WordOfPassage = wordOfPassage;
            flags.QuestItemPercentage = questItems;
            flags.KarmaSetPercentage = karmaPercentage;
            flags.KarmaValue = karmaValueInt;
            flags.MonsterDamage = monsterDamage;
            flags.WeaponDamage = weaponDamage;
            flags.EarlierMonsters = earlierMonsters;
            flags.MonsterQty = monsterQty;
            flags.NoRequireFullParty = noRequireFullParty;
            flags.RandomizeSpells = randomizeSpells;
            flags.HerbPrices = herbPrice;
            flags.Sextant = sextant;
            flags.ClothMap = generateClothMap;
            flags.PrincipleItems = principleItems;
            flags.TownSaves = townSaves;
            flags.DaemonTrigger = daemonTrigger;
            flags.RequireMysticWeapons = requireMystics;
            flags.AwakenUpgrade = awakenUpgrade;
            flags.ShopOverflowFix = shopOverflow;
            flags.Other = other;
            flags.SpoilerLog = spoilerLog;
            flags.VGAPatch = vgaPatch;

            //Program.Randomize(seed, path, flags, encodedArg.Value());
            //path = "F:\\Gog-games\\Ultima 4";
            DosRandomizer.Randomize(seedInt, path, flags, "");
            //Console.WriteLine("Seed: " + seed);
            //var random = new Random(seed);
            //var worldMap = new WorldMap();
            //worldMap.SwampTest(random);
            //worldMap.Save(path);

            //var image = worldMap.ToImage();
            //image.SaveAsPng("worldMap.png");
            //}
        }
    }
}
