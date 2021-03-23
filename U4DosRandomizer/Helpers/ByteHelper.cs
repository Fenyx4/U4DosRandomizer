using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer.Helpers
{
    public static class ByteHelper
    {
        public static void OverwriteBytes(this byte[] bytes, List<ushort> list, int startingOffset)
        {
            for (int listItem = 0; listItem < list.Count; listItem++)
            {
                var overwriteBytes = BitConverter.GetBytes(list[listItem]);
                for (int offset = 0; offset < overwriteBytes.Length; offset++)
                {
                    bytes[startingOffset + (listItem * overwriteBytes.Length) + offset] = overwriteBytes[offset];
                }
            }
        }
    }
}
