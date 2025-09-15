using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class Person
    {
        public byte QuestionFlag { get; internal set; }
        public byte Humility { get; internal set; }
        public byte TurningAwayProbability { get; internal set; }
        public string Name { get; internal set; }
        public string Pronoun { get; internal set; }
        public string Look { get; internal set; }
        public string Job { get; internal set; }
        public string Health { get; internal set; }
        public string Keyword1 { get; internal set; }
        public string Keyword2 { get; internal set; }
        public string Yes { get; internal set; }
        public string No { get; internal set; }
        public string Question { get; internal set; }
        public string KeywordResponse1 { get; internal set; }
        public string KeywordResponse2 { get; internal set; }
        public string Town { get; internal set; }

        public List<byte> GetBytes()
        {
            var personBytes = new List<byte>();
            personBytes.Add(QuestionFlag);
            personBytes.Add(Humility);
            personBytes.Add(TurningAwayProbability);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Name));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Pronoun));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Look));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Job));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Health));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(KeywordResponse1));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(KeywordResponse2));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Question));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Yes));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(No));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Keyword1));
            personBytes.Add(0);
            personBytes.AddRange(Encoding.ASCII.GetBytes(Keyword2));
            personBytes.Add(0);

            while (personBytes.Count < 0x120)
            {
                personBytes.Add(0);
            }

            if (personBytes.Count > 0x120)
            {
                throw new Exception($"Text for {Town}:{Name} too long by {personBytes.Count - 0x120} bytes.");
            }

            return personBytes;
        }
    }
}
