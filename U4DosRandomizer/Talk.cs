using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace U4DosRandomizer
{
    public class Talk
    {
        public void Load()
        {
            var files = Directory.GetFiles("ULT", "*.TLK");

            var towns = new Dictionary<string, List<Person>>();
            var personBytes = new byte[0x120];
            foreach(var file in files)
            {
                var talk = new System.IO.FileStream(file, System.IO.FileMode.Open);

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
                    person.Keyword1 = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                    person.Keyword2 = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                    person.Question = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                    person.Yes = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));
                    person.No = System.Text.Encoding.Default.GetString(personBytes.ReadUntilNull(++read, ref read));

                    persons.Add(person);
                }

                towns.Add(Path.GetFileNameWithoutExtension(file), persons);
            }

        }
    }
}
