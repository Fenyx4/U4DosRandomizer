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
            EncodedChanged = new Command(OnEncodedChanged);
            seed = "";
            ultimaIVInstallPath = "";
            GenerateClothMap = true;
            this.folderPicker = folderPicker;
            this.popupService = popupService;
        }

        [ObservableProperty]
        public bool busy;

        // Settings

        [ObservableProperty]
        public bool generateClothMap;

        [ObservableProperty]
        public bool miniMap;

        [ObservableProperty]
        public bool vgaPatch;
        
        [ObservableProperty]
        public bool spoilerLog;

        [ObservableProperty]
        public string encoded;

        [ObservableProperty]
        public string seed;

        [ObservableProperty]
        public string ultimaIVInstallPath;

        // -----------

        // Randomize

        [ObservableProperty]
        public string overWorld = "5";

        [ObservableProperty]
        public bool startingWeapons;

        [ObservableProperty]
        public string dungeon = "1";

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
        public string monsterDamage = "2";

        [ObservableProperty]
        public string weaponDamage = "0";

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
        public string herbPrice = "0";

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
        public ICommand EncodedChanged { get; }

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

        public async void OnEncodedChanged()
        {
            try
            {
                
                Convert.FromBase64String(Encoded);
                Flags flags = new Flags(7, 9);
                flags.DecodeAndSet(encoded);

                Seed = flags.Seed.ToString();

                OverWorld = flags.Overworld.ToString();
                MiniMap = flags.MiniMap;
                Dungeon = flags.Dungeon.ToString();
                //SpellRemove = flags.SpellRemove;
                StartingWeapons = flags.StartingWeapons;
                MixQuantity = flags.MixQuantity;
                Fixes = flags.Fixes;
                HythlothFix = flags.FixHythloth;
                SleepLockAssist = flags.SleepLockAssist;
                ActivePlayer = flags.ActivePlayer;
                AppleHitChance = flags.HitChance;
                DiagonalAttack = flags.DiagonalAttack;
                SacrificeFix = flags.SacrificeFix;
                Runes = flags.Runes;
                Mystics = flags.Mystics;
                Mantras = flags.Mantras;
                WordOfPassage = flags.WordOfPassage;
                QuestItems = flags.QuestItemPercentage;
                KarmaPercentage = flags.KarmaSetPercentage;
                if (flags.KarmaValue is null)
                {
                    KarmaValueOn = false;
                    KarmaValue = 0;
                }
                else
                {
                    KarmaValueOn = true;
                    KarmaValue = flags.KarmaValue.Value;
                }
                MonsterDamage = flags.MonsterDamage.ToString();
                WeaponDamage = flags.WeaponDamage.ToString();
                EarlierMonsters = flags.EarlierMonsters;
                MonsterQty = flags.MonsterQty;
                NoRequireFullParty = flags.NoRequireFullParty;
                RandomizeSpells = flags.RandomizeSpells;
                HerbPrice = flags.HerbPrices.ToString();
                Sextant = flags.Sextant;
                GenerateClothMap = flags.ClothMap;
                PrincipleItems = flags.PrincipleItems;
                TownSaves = flags.TownSaves;
                DaemonTrigger = flags.DaemonTrigger;
                RequireMystics = flags.RequireMysticWeapons;
                AwakenUpgrade = flags.AwakenUpgrade;
                ShopOverflow = flags.ShopOverflowFix;
                Other = flags.Other;
                SpoilerLog = flags.SpoilerLog;
                VgaPatch = flags.VGAPatch;

            }
            catch(Exception e)
            {

            }
            
        }

        public async void OnClicked()
        {
            //OverWorld = "1";
            //return;

            var seedInt = Environment.TickCount;
            if ( !string.IsNullOrEmpty(Seed) )
            {
                seedInt = int.Parse(Seed);
            }


            int? karmaValueInt = null;
            if ( KarmaValueOn)
            {
                karmaValueInt = KarmaValue;
            }

            var path = UltimaIVInstallPath;
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
            flags.Overworld = int.Parse(OverWorld);
            flags.MiniMap = MiniMap;
            flags.Dungeon = int.Parse(Dungeon);
            flags.SpellRemove = SpellRemove;
            flags.StartingWeapons = StartingWeapons;
            flags.DngStone = false; // deprecated
            flags.MixQuantity = MixQuantity;
            flags.Fixes = Fixes;
            flags.FixHythloth = HythlothFix;
            flags.SleepLockAssist = SleepLockAssist;
            flags.ActivePlayer = ActivePlayer;
            flags.HitChance = AppleHitChance;
            flags.DiagonalAttack = DiagonalAttack;
            flags.SacrificeFix = SacrificeFix;
            flags.Runes = Runes;
            flags.Mystics = Mystics;
            flags.Mantras = Mantras;
            flags.WordOfPassage = WordOfPassage;
            flags.QuestItemPercentage = QuestItems;
            flags.KarmaSetPercentage = KarmaPercentage;
            flags.KarmaValue = karmaValueInt;
            flags.MonsterDamage = int.Parse(MonsterDamage);
            flags.WeaponDamage = int.Parse(WeaponDamage);
            flags.EarlierMonsters = EarlierMonsters;
            flags.MonsterQty = MonsterQty;
            flags.NoRequireFullParty = NoRequireFullParty;
            flags.RandomizeSpells = RandomizeSpells;
            flags.HerbPrices = int.Parse(HerbPrice);
            flags.Sextant = Sextant;
            flags.ClothMap = GenerateClothMap;
            flags.PrincipleItems = PrincipleItems;
            flags.TownSaves = TownSaves;
            flags.DaemonTrigger = DaemonTrigger;
            flags.RequireMysticWeapons = RequireMystics;
            flags.AwakenUpgrade = AwakenUpgrade;
            flags.ShopOverflowFix = ShopOverflow;
            flags.Other = Other;
            flags.SpoilerLog = SpoilerLog;
            flags.VGAPatch = VgaPatch;

            //Program.Randomize(seed, path, flags, encodedArg.Value());
            //path = "F:\\Gog-games\\Ultima 4";
            Busy = true;
            popupService.ShowBusy();
            await Task.Run(() => DosRandomizer.Randomize(seedInt, path, flags, ""));
            popupService.StopBusy();
            Busy = false;
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
