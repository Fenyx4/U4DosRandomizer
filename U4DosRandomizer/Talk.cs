using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using U4DosRandomizer.Helpers;

namespace U4DosRandomizer
{
    public class Talk
    {
        private Dictionary<string, List<Person>> towns = new Dictionary<string, List<Person>>();

        public void Load(string path)
        {
            var files = Directory.GetFiles(path, "*.TLK");

            var personBytes = new byte[0x120];
            foreach(var file in files)
            {
                FileHelper.TryBackupOriginalFile(file, false);

                var persons = new List<Person>();
                using (var talk = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open))
                {
                    Person person = null;
                    while (talk.Read(personBytes, 0, 0x120) != 0)
                    {
                        person = new Person();
                        person.QuestionFlag = personBytes[0];
                        person.Humility = personBytes[1];
                        person.TurningAwayProbability = personBytes[2];
                        var read = 3;
                        person.Name = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(read, ref read));
                        person.Pronoun = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.Look = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.Job = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.Health = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.KeywordResponse1 = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.KeywordResponse2 = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.Question = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.Yes = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.No = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.Keyword1 = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                        person.Keyword2 = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));

                        persons.Add(person);
                    }
                }

                towns.Add(Path.GetFileNameWithoutExtension(file), persons);
            }
        }

        public void Update(UltimaData ultimaData, Avatar avatar, Flags flags)
        {
            if (flags.Overworld == 5 || flags.Overworld == 1)
            {
                Person person = null;
                // --- Items ---
                if (ultimaData.Items[ultimaData.ITEM_BELL].Changed)
                {
                    person = FindPerson("Garam");
                    person.KeywordResponse2 = ReplaceSextantText(person.KeywordResponse2, GetSextantText(ultimaData.Items[ultimaData.ITEM_BELL]));
                }

                if (ultimaData.Items[ultimaData.ITEM_SKULL].Changed)
                {
                    person = FindPerson("Jude");
                    person.Yes = ReplaceSextantText(person.Yes, GetSextantText(ultimaData.Items[ultimaData.ITEM_SKULL]));
                }

                if (ultimaData.Items[ultimaData.ITEM_NIGHTSHADE].Changed)
                {
                    person = FindPerson("Virgil");
                    person.KeywordResponse2 = ReplaceSextantText(person.KeywordResponse2, GetSextantText(ultimaData.Items[ultimaData.ITEM_NIGHTSHADE]));
                }

                if (ultimaData.Towns[avatar.AvatarOffset.LOC_MAGINCIA - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    person = FindPerson("Shawn");
                    person.No = ReplaceSextantText(person.No, GetSextantText(ultimaData.Towns[avatar.AvatarOffset.LOC_MAGINCIA - avatar.AvatarOffset.LOC_TOWNS]));
                }

                // Mandrake
                // TODO make response descriptive
                if (ultimaData.Items[ultimaData.ITEM_MANDRAKE].Changed)
                {
                    person = FindPerson("Calumny");
                    person.KeywordResponse2 = $"Mandrake is found near {GetSextantText(ultimaData.Items[ultimaData.ITEM_MANDRAKE])}\nand\n{GetSextantText(ultimaData.Items[ultimaData.ITEM_MANDRAKE2])} ";
                }

                // Horn
                // TODO make response descriptive
                if (ultimaData.Items[ultimaData.ITEM_HORN].Changed)
                {
                    person = FindPerson("Malchor");
                    person.KeywordResponse2 = $"Some say that\nthe silver horn\nis buried at\n{GetSextantText(ultimaData.Items[ultimaData.ITEM_HORN])}";
                }

                // Wheel
                // TODO make response descriptive
                if (ultimaData.Items[ultimaData.ITEM_WHEEL].Changed)
                {
                    person = FindPerson("Lassorn");
                    person.KeywordResponse2 = $"She went down in\nthe deep waters\nat\n{GetSextantText(ultimaData.Items[ultimaData.ITEM_WHEEL])}!";
                }

                // TODO Black stone currently at the moongate will need to change this text if we ever do randomize it
                person = FindPerson("Merlin");

                // White stone
                // TODO make response descriptive
                if (ultimaData.Items[ultimaData.ITEM_WHITE_STONE].Changed)
                {
                    person = FindPerson("Isaac");
                    person.KeywordResponse2 = $"The white stone\nsits atop the\nmountains at\n{GetSextantText(ultimaData.Items[ultimaData.ITEM_WHITE_STONE])}.\nIt can only be\nreached by one\nwho floats\nwithin the\nclouds.";
                    ultimaData.ShrineText[6 * 3 + 2] = $"If thou dost seek the White Stone search not under the ground but at {GetSextantText(ultimaData.Items[ultimaData.ITEM_WHITE_STONE]).Replace('\n', ' ')}";
                }

                // TODO Book, candle, runes, mystic armor and mystic weapons I'm leaving along for now. Not randomizing stuff in towns yet.


                // --- End Items ---

                // --- Shrines ---
                // Humility
                // TODO make response descriptive
                if (ultimaData.Shrines[avatar.AvatarOffset.LOC_HUMILITY - avatar.AvatarOffset.LOC_SHRINES].IsDirty())
                {
                    person = FindPerson("Simple");
                    person.KeywordResponse2 = $"The shrine lies\nnear\n{GetSextantText(ultimaData.Shrines[avatar.AvatarOffset.LOC_HUMILITY - avatar.AvatarOffset.LOC_SHRINES])} and\nis guarded by\nendless hoards\nof daemons!";
                    person = FindPerson("Wierdrum");
                    person.KeywordResponse2 = $"Yes, I have been\nto the shrine,\nit lies near\n{GetSextantText(ultimaData.Shrines[avatar.AvatarOffset.LOC_HUMILITY - avatar.AvatarOffset.LOC_SHRINES])}!";
                }

                // Compassion
                // TODO make response descriptive
                if (ultimaData.Shrines[avatar.AvatarOffset.LOC_COMPASSION - avatar.AvatarOffset.LOC_SHRINES].IsDirty())
                {
                    person = FindPerson("Shapero");
                    person.Yes = $"Find the shrine\nof compassion\nat\n{GetSextantText(ultimaData.Shrines[avatar.AvatarOffset.LOC_COMPASSION - avatar.AvatarOffset.LOC_SHRINES])}!";
                }

                // Sacrifice
                // TODO make response descriptive
                if (ultimaData.Shrines[avatar.AvatarOffset.LOC_SACRIFICE - avatar.AvatarOffset.LOC_SHRINES].IsDirty())
                {
                    person = FindPerson("Merida");
                    person.No = $"The shrine is at\n{GetSextantText(ultimaData.Shrines[avatar.AvatarOffset.LOC_SACRIFICE - avatar.AvatarOffset.LOC_SHRINES])}!";
                }

                // Justice
                // TODO make response descriptive
                if (ultimaData.Shrines[avatar.AvatarOffset.LOC_JUSTICE - avatar.AvatarOffset.LOC_SHRINES].IsDirty())
                {
                    person = FindPerson("Druid");
                    person.KeywordResponse2 = $"The shrine is at\n{GetSextantText(ultimaData.Shrines[avatar.AvatarOffset.LOC_JUSTICE - avatar.AvatarOffset.LOC_SHRINES])}!";
                }

                // Honesty
                // TODO make response descriptive
                if (ultimaData.Shrines[avatar.AvatarOffset.LOC_HONESTY - avatar.AvatarOffset.LOC_SHRINES].IsDirty())
                {
                    person = FindPerson("Calabrini");
                    person.No = $"Perhaps, the\nshrine which\nlies at\n{GetSextantText(ultimaData.Shrines[avatar.AvatarOffset.LOC_HONESTY - avatar.AvatarOffset.LOC_SHRINES])}!";
                }

                // Honor
                // TODO make response descriptive
                if (ultimaData.Shrines[avatar.AvatarOffset.LOC_HONOR - avatar.AvatarOffset.LOC_SHRINES].IsDirty())
                {
                    person = FindPerson("Dergin");
                    person.No = $"The shrine lies at\n{GetSextantText(ultimaData.Shrines[avatar.AvatarOffset.LOC_HONOR - avatar.AvatarOffset.LOC_SHRINES])}!";
                }

                // TODO Spirituality - Do I move this one?
                person = FindPerson("the Ankh of\nSpirituality");

                // Valor
                // No on gives the directions to Valor so I grabbed his reponse that talked about the shrine and usurped it
                // TODO make response descriptive
                if (ultimaData.Shrines[avatar.AvatarOffset.LOC_VALOR - avatar.AvatarOffset.LOC_SHRINES].IsDirty())
                {
                    person = FindPerson("Sir Hrothgar");
                    person.No = $"Thou should seek\nthe shrine of\nvalor at\n{GetSextantText(ultimaData.Shrines[avatar.AvatarOffset.LOC_VALOR - avatar.AvatarOffset.LOC_SHRINES])}!";
                }

                // --- End Shrines ---

                // --- Towns and Castles ---
                // TODO make response descriptive
                if (ultimaData.Castles[0].IsDirty())
                {
                    ultimaData.LBText[3] = $"He says:\nMany truths can\nbe learned at\nthe Lycaeum.  It\nlies to the\n{CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Castles[0])}!\n";
                }
                if (ultimaData.Castles[1].IsDirty())
                {
                    ultimaData.LBText[4] = $"He says:\nLook for the\nmeaning of Love\nat Empath Abbey.\nThe Abbey sits\n{CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Castles[1])}!\n";
                }
                if (ultimaData.Castles[2].IsDirty())
                {
                    ultimaData.LBText[5] = $"\n\nHe says:\nSerpent's Castle\nto the {CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Castles[2])}\nis where\nCourage should\nbe sought!\n";
                }
                if (ultimaData.Towns[avatar.AvatarOffset.LOC_MOONGLOW - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    ultimaData.LBText[6] = $"\nHe says:\nThe towne\nof Moonglow to\nthe {CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Towns[avatar.AvatarOffset.LOC_MOONGLOW - avatar.AvatarOffset.LOC_TOWNS])} is\nwhere the virtue\nof Honesty\nthrives!\n";
                }
                if (ultimaData.Towns[avatar.AvatarOffset.LOC_BRITAIN - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    ultimaData.LBText[7] = $"\n\nHe says:\nThe bards in\nBritain to the\n{CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Towns[avatar.AvatarOffset.LOC_BRITAIN - avatar.AvatarOffset.LOC_TOWNS])}\nare well versed\nin\nCompassion!\n";
                }
                if (ultimaData.Towns[avatar.AvatarOffset.LOC_JHELOM - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    ultimaData.LBText[8] = $"\n\nHe says:\nMany valiant\nfighters come\nfrom Jhelom\nto the \n{CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Towns[avatar.AvatarOffset.LOC_JHELOM - avatar.AvatarOffset.LOC_TOWNS])}!\n";
                }
                if (ultimaData.Towns[avatar.AvatarOffset.LOC_YEW - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    ultimaData.LBText[9] = $"\n\n\nHe says:\nIn the city of\nYew, to the\n{CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Towns[avatar.AvatarOffset.LOC_YEW - avatar.AvatarOffset.LOC_TOWNS])}, \nJustice is\nserved!\n";
                }
                if (ultimaData.Towns[avatar.AvatarOffset.LOC_MINOC - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    ultimaData.LBText[10] = $"\nHe says:\nMinoc, towne of\nself-sacrifice,\nlies {CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Towns[avatar.AvatarOffset.LOC_MINOC - avatar.AvatarOffset.LOC_TOWNS])}!\n";
                }
                if (ultimaData.Towns[avatar.AvatarOffset.LOC_TRINSIC - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    ultimaData.LBText[11] = $"\nHe says:\nThe Paladins who\nstrive for Honor\nare oft seen in\nTrinsic, to the {CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Towns[avatar.AvatarOffset.LOC_TRINSIC - avatar.AvatarOffset.LOC_TOWNS])}!\n";
                }
                if (ultimaData.Towns[avatar.AvatarOffset.LOC_SKARA - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    ultimaData.LBText[12] = $"\nHe says:\nIn Skara Brae\nthe Spiritual\npath is taught.\nFind it to the\n{CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Towns[avatar.AvatarOffset.LOC_SKARA - avatar.AvatarOffset.LOC_TOWNS])}!\n";
                }
                if (ultimaData.Towns[avatar.AvatarOffset.LOC_MAGINCIA - avatar.AvatarOffset.LOC_TOWNS].IsDirty())
                {
                    ultimaData.LBText[13] = $"\n\n\nHe says:\nHumility is the\nfoundation of\nVirtue!  The\nruins of proud\nMagincia are a\ntestimony unto\nthe Virtue of\nHumility!\n\nFind the Ruins\nof Magincia to\nthe {CoordinateToCardinal(ultimaData.LCB[0], ultimaData.Towns[avatar.AvatarOffset.LOC_MAGINCIA - avatar.AvatarOffset.LOC_TOWNS])}!\n";
                }

                // --- End Towns and Castles ---

                // --- Other ---
                // TODO: Pirate location? Bucaneer's Den?
                person = FindPerson("Wilmoore");
            }
            else if (flags.Overworld == 2)
            {
                var talkToLocation = new Dictionary<Tuple<byte, byte, byte>, Tuple<string, string>>();
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0xB6, 0x36), new Tuple<string,string>("<Item> is found in the Bloody Plains where the ground is always damp.", "<Item> is found in the Bloody Plains where the ground is always damp. Search on the darkest of nights!"));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0x64, 0xA5), new Tuple<string,string>("<Item> is found in the Fens of the Dead where the ground is always damp.", "<Item> is found in the Fens of the Dead where the ground is always damp. Search on the darkest of nights!"));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0x2E, 0x95), new Tuple<string,string>("<Item> may be found only near lat-J'F\" long-C'O\"!", "<Item> may be found only near lat-J'F\" long-C'O\" only on the darkest of nights!"));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0xCD, 0x2C), new Tuple<string,string>("<Item> may be found in the forest outside the shrine in the lake east of the Bloody Plains!", "<Item> may be found in the forest outside the shrine in the lake east of the Bloody Plains only on the darkest of nights!"));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0xB0, 0xD0), new Tuple<string,string>("<Item> lies at the bottom of a deep well at sea found at lat-N'A\" long-L'A\".", "<Item> lies at the bottom of a deep well at sea found at lat-N'A\" long-L'A\" but can only be found on the darkest of nights."));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0x2D, 0xAD), new Tuple<string,string>("Some say that <Item> is buried on a small isle off the tip of Spiritwood.", "Some say that <Item> is buried on a small isle off the tip of Spiritwood and can be found when the moons go dark."));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0x60, 0xD7), new Tuple<string,string>("Search the deep waters of the bay in the Cape of Heroes!", "Search the deep waters of the bay in the Cape of Heroes when the moons go dark!"));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0xC5, 0xF5), new Tuple<string,string>("It can be found at lat-P'F\" long-M'F\"!", "It can be found at lat-P'F\" long-M'F\" on the darkest night!"));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0xE0, 0x85), new Tuple<string,string>("Stand where the gate of both moons dark shall appear.", "Stand where the gate of both moons dark shall appear. Search when the moons go dark!"));
                talkToLocation.Add(new Tuple<byte, byte, byte>(0x00, 0x40, 0x50), new Tuple<string, string>("<Item> sits atop the Serpent's Spine. It can only be reached by one who floats within the clouds.", "<Item> sits atop the Serpent's Spine. It can only be reached by one who floats within the clouds and when the moons go dark."));

                var item = ultimaData.Items[ultimaData.ITEM_BELL];
                var talkString = talkToLocation[new Tuple<byte, byte, byte>(item.Location, item.X, item.Y)].Item1;
                talkString = talkString.Replace("<Item>", "the bell of courage").CapitalizeFirstLetter();
                var person = FindPerson("Garam");
                person.KeywordResponse2 = talkString;

                item = ultimaData.Items[ultimaData.ITEM_SKULL];
                talkString = talkToLocation[new Tuple<byte, byte, byte>(item.Location, item.X, item.Y)].Item2;
                talkString = talkString.Replace("<Item>", "the skull").CapitalizeFirstLetter();
                person = FindPerson("Jude");
                person.Yes = talkString;

                item = ultimaData.Items[ultimaData.ITEM_NIGHTSHADE];
                talkString = talkToLocation[new Tuple<byte, byte, byte>(item.Location, item.X, item.Y)].Item2;
                talkString = talkString.Replace("<Item>", "nightshade").CapitalizeFirstLetter();
                person = FindPerson("Virgil");
                person.KeywordResponse2 = talkString;

                item = ultimaData.Items[ultimaData.ITEM_MANDRAKE];
                talkString = talkToLocation[new Tuple<byte, byte, byte>(item.Location, item.X, item.Y)].Item1;
                talkString = talkString.Replace("<Item>", "mandrake").CapitalizeFirstLetter();
                person = FindPerson("Calumny");
                person.KeywordResponse2 = talkString;

                item = ultimaData.Items[ultimaData.ITEM_HORN];
                talkString = talkToLocation[new Tuple<byte, byte, byte>(item.Location, item.X, item.Y)].Item1;
                talkString = talkString.Replace("<Item>", "the silver horn").CapitalizeFirstLetter();
                person = FindPerson("Malchor");
                person.KeywordResponse2 = talkString;

                item = ultimaData.Items[ultimaData.ITEM_WHEEL];
                talkString = talkToLocation[new Tuple<byte, byte, byte>(item.Location, item.X, item.Y)].Item1;
                talkString = talkString.Replace("<Item>", "the magical wheel").CapitalizeFirstLetter();
                person = FindPerson("Lassorn");
                person.KeywordResponse2 = talkString;

                item = ultimaData.Items[ultimaData.ITEM_BLACK_STONE];
                talkString = talkToLocation[new Tuple<byte, byte, byte>(item.Location, item.X, item.Y)].Item2;
                talkString = talkString.Replace("<Item>", "the black stone").CapitalizeFirstLetter();
                person = FindPerson("Merlin");
                person.KeywordResponse1 = talkString;

                item = ultimaData.Items[ultimaData.ITEM_WHITE_STONE];
                talkString = talkToLocation[new Tuple<byte, byte, byte>(item.Location, item.X, item.Y)].Item1;
                talkString = talkString.Replace("<Item>", "the white stone").CapitalizeFirstLetter();
                person = FindPerson("Isaac");
                person.KeywordResponse2 = talkString;
                ultimaData.ShrineText[6 * 3 + 2] = "If thou dost seek the White Stone rest at the Inn of Spirits.";
            }

        }

        internal static void Restore(string path)
        {
            var files = Directory.GetFiles(path, "*.TLK");

            foreach (var file in files)
            {
                FileHelper.Restore(file);
            }
        }

        // https://github.com/ergonomy-joe/u4-decompiled/blob/c2c2108fa3bb346bcd1d8c207c526f33a4c8f5ef/SRC/U4_LOCAT.C#L20
        public static string GetSextantText(ICoordinate item)
        {
            //lat-N'A" long-L'A"
            return $"lat-{(char)((item.Y >> 4) +'A')}'{(char)((item.Y & 0xF) + 'A')}\"\nlong-{(char)((item.X >> 4) + 'A')}'{(char)((item.X & 0xF) + 'A')}\"";
        }

        private string ReplaceSextantText(string text, string latLong)
        {
            var rx = new Regex("lat-[A-Z]'[A-Z]\".long-[A-Z]'[A-Z]\"", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var result = rx.Replace(text, latLong);
            return result;
        }

        public static string CoordinateToCardinal(ICoordinate origin, ICoordinate destination)
        {
            //var distanceSquared = ((destination.X - origin.X) * (destination.X - origin.X) + (destination.Y - origin.Y) * (destination.Y - origin.Y));
            var delta_x = destination.X - origin.X;
            var delta_y = destination.Y - origin.Y;
            var radians = Math.Atan2(delta_y, delta_x);

            radians -= (3 * Math.PI) / 2;
            while (radians < 0)
            {
                radians += 2 * Math.PI;
            }

            double degrees = (180 / Math.PI) * radians;

            //https://gist.github.com/adrianstevens/8163205
            string[] cardinals = { "north", "northeast", "east", "southeast", "south", "southwest", "west", "northwest", "north" };
            //var idx = (int)Math.Round(((double)radians % (Math.PI * 2) / (Math.PI / 4)));
            return cardinals[(int)Math.Round(((double)degrees % 360) / 45)];
        }

        private Person FindPerson(string name)
        {
            var person = towns.Values.SelectMany(l => l).Where(p => p.Name.ToLower() == name.ToLower()).SingleOrDefault();

            if(person == null)
            {
                throw new Exception($"Unable to find {name}. Have your files been modified?");
            }

            return person;
        }

        public void Save(string path)
        {
            foreach(var townName in towns.Keys)
            {
                var townBytes = new List<byte>();

                foreach(var person in towns[townName])
                {
                    var personBytes = new List<byte>();
                    personBytes.Add(person.QuestionFlag);
                    personBytes.Add(person.Humility);
                    personBytes.Add(person.TurningAwayProbability);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Name));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Pronoun));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Look));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Job));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Health));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.KeywordResponse1));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.KeywordResponse2));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Question));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Yes));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.No));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Keyword1));
                    personBytes.Add(0);
                    personBytes.AddRange(Encoding.ASCII.GetBytes(person.Keyword2));
                    personBytes.Add(0);

                    while (personBytes.Count < 0x120)
                    {
                        personBytes.Add(0);
                    }
                    
                    if (personBytes.Count > 0x120)
                    {
                        throw new Exception($"Text for {townName}:{person.Name} too long by {personBytes.Count - 0x120} bytes.");
                    }

                    townBytes.AddRange(personBytes);
                }

                var file = Path.Combine(path, $"{townName.ToUpper()}.TLK");
                File.WriteAllBytes(file, townBytes.ToArray());
            }
        }

    }
}
