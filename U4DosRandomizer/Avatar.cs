using System;
using System.Collections.Generic;
using System.Text;

namespace U4DosRandomizer
{
    public class Avatar
    {
        public static void MoveMoongates(byte[,] worldMapUlt, byte[] avatar)
        {
            //throw in a lava to make it easy to find
            for (int offset = 0; offset < 8; offset++)
            {
                worldMapUlt[200, 200 + offset] = 76;
            }

            int moongateXOffset = 0x0fad1;
            int moongateYOffset = 0x0fad9;

            for (byte offset = 0; offset < 8; offset++)
            {
                avatar[moongateXOffset + offset] = 200;
                avatar[moongateYOffset + offset] = Convert.ToByte(200 + offset);
            }

            return;
        }
    }
}
