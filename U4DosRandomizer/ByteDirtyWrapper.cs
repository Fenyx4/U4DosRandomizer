using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class ByteDirtyWrapper
    {
        private byte _original;

        public byte Byte { get; internal set; }

        public ByteDirtyWrapper(byte val)
        {
            _original = val;
            Byte = val;
        }

        public bool IsDirty()
        {
            return !Byte.Equals(_original);
        }
    }
}
