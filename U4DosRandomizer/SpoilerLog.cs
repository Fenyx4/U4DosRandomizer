using System;
using System.IO;

namespace U4DosRandomizer
{
    public class SpoilerLog
    {
        private StreamWriter spoilerWriter;

        public SpoilerLog(StreamWriter spoilerWriter)
        {
            this.spoilerWriter = spoilerWriter;
        }

        internal void WriteFlags(Flags flags)
        {
            //throw new NotImplementedException();
        }

        internal void Add(SpoilerCategory category, string v)
        {
            spoilerWriter.WriteLine(v);
            //throw new NotImplementedException();
        }
    }
}