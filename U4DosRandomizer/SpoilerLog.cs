using System;
using System.IO;

namespace U4DosRandomizer
{
    public class SpoilerLog
    {
        private StreamWriter spoilerWriter;
        private bool enabled;

        public SpoilerLog(StreamWriter spoilerWriter, bool enabled)
        {
            this.spoilerWriter = spoilerWriter;
            this.enabled = enabled;
        }

        internal void WriteFlags(Flags flags)
        {
            //throw new NotImplementedException();
        }

        internal void Add(SpoilerCategory category, string v)
        {
            if (enabled)
            {
                spoilerWriter.WriteLine(v);
            }
        }
    }
}