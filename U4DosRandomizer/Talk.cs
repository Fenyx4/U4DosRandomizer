using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace U4DosRandomizer
{
    public class Talk
    {
        private SHA256 Sha256 = SHA256.Create();
        private Dictionary<string, List<Person>> towns = new Dictionary<string, List<Person>>();

        public void Load()
        {
            var hashes = ReadHashes();
            var files = Directory.GetFiles("ULT", "*.TLK");

            var personBytes = new byte[0x120];
            foreach(var file in files)
            {
                var hash = GetHashSha256(file);
                if(hashes[Path.GetFileName(file)] == BytesToString(hash))
                {
                    File.Copy(file, $"{file}.orig", true);
                }
                else
                {
                    hash = GetHashSha256($"{file}.orig");
                    if(hashes[Path.GetFileName(file)] != BytesToString(hash))
                    {
                        throw new FileNotFoundException($"Original version of {file} not found.");
                    }
                }

                var talk = new System.IO.FileStream($"{file}.orig", System.IO.FileMode.Open);

                Person person = null;
                var persons = new List<Person>();
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

                towns.Add(Path.GetFileNameWithoutExtension(file), persons);
            }
        }

        public void Update(UltimaData ultimaData)
        {
            Person garam = FindPerson("Garam");
            garam.KeywordResponse2 = $"The bell of courage lies at the bottom of a deep well at sea found at {GetSextantText(ultimaData.Items[Avatar.ITEM_BELL])}";

        }

        // https://github.com/ergonomy-joe/u4-decompiled/blob/c2c2108fa3bb346bcd1d8c207c526f33a4c8f5ef/SRC/U4_LOCAT.C#L20
        private string GetSextantText(Item item)
        {
            //lat-N'A" long-L'A"
            return $"lat-{(char)((item.Y >> 4) +'A')}'{(char)((item.Y & 0xF) + 'A')}\" long-{(char)((item.X >> 4) + 'A')}'{(char)((item.X & 0xF) + 'A')}\"";
        }

        private Person FindPerson(string name)
        {
            var person = towns.Values.SelectMany(l => l).Where(p => p.Name.ToLower() == name.ToLower()).Single();

            return person;
        }

        public void Save()
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

                File.WriteAllBytes($"ULT\\{townName.ToUpper()}.TLK", townBytes.ToArray());
            }
        }

        public Dictionary<string, string> ReadHashes()
        {
            var hashJson = System.IO.File.ReadAllText("hashes\\talk_hash.json");

            var hashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(hashJson);

            return hashes;
        }

        public void WriteHashes()
        {
            var files = Directory.GetFiles("ULT", "*.TLK");

            var townTalkHash = new Dictionary<string, string>();

            foreach (var file in files)
            {
                var hash = GetHashSha256(file);
                Console.WriteLine($"{file}: {BytesToString(hash)}");
                townTalkHash.Add(Path.GetFileName(file), BytesToString(hash));
            }

            string json = JsonConvert.SerializeObject(townTalkHash); // the dictionary is inside client object
                                                                    //write string to file
            System.IO.File.WriteAllText(@"talk_hash.json", json);
        }

        private byte[] GetHashSha256(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return Sha256.ComputeHash(stream);
            }
        }

        // Return a byte array as a sequence of hex values.
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }
    }
}
