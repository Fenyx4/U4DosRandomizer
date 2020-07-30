using System.Collections.Generic;
using System.IO;


namespace U4DosRandomizer
{
    public static class ArrayExtensions
    {
        public static byte[] ReadUntilNull(this byte[] bytes, int offset, ref int read)
        {
            var byteList = new List<byte>();

            var idx = offset;
            for(; idx < bytes.Length; idx++)
            {
                if(bytes[idx] == 0)
                {
                    break;
                }
                byteList.Add(bytes[idx]);
            }

            read += idx - offset;

            return byteList.ToArray();
        }
    }
}
