using System;
using System.Collections.Generic;
using System.IO;

namespace U4DosRandomizer
{
    public class Town
    {
        private byte[,] map = new byte[32, 32];
        private byte[] npcTileIdx = new byte[32];
        private byte[] npcStartX = new byte[32];
        private byte[] npcStartY = new byte[32];
        private byte[] npcMovement = new byte[32];
        private byte[] npcOldX = new byte[32]; // Not known to used in towns
        private byte[] npcOldY = new byte[32]; // Not known to used in towns
        private byte[] npcAggressivity = new byte[32]; // Not known to used in towns
        public byte[] npcConversationIdx = new byte[32];

        public Town(FileStream twnStream, UltimaData data)
        {
            BinaryReader readBinary = new BinaryReader(twnStream);

            byte[] mapArr = new byte[32 * 32];
            readBinary.Read(mapArr, 0, 32 * 32);
            for (int i = 0; i < 32 * 32; i++)
            {
                map[i / 32, i % 32] = mapArr[i];
            }

            readBinary.Read(npcTileIdx, 0, 32);
            readBinary.Read(npcStartX, 0, 32);
            readBinary.Read(npcStartY, 0, 32);

            //Unused entries
            readBinary.Read(npcOldX, 0, 32);
            readBinary.Read(npcOldY, 0, 32);
            readBinary.Read(npcAggressivity, 0, 32);

            readBinary.Read(npcMovement, 0, 32);
            readBinary.Read(npcConversationIdx, 0, 32);
        }

        public byte[] Save()
        {
            var townBytes = new byte[32 * 32 + npcTileIdx.Length + npcStartX.Length + npcStartY.Length + npcMovement.Length + npcConversationIdx.Length + 32*3];

            int idx = 0;
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    townBytes[idx++] = map[x, y];
                }
            }

            idx = CopyBytes(townBytes, npcTileIdx, idx);
            idx = CopyBytes(townBytes, npcStartX, idx);
            idx = CopyBytes(townBytes, npcStartY, idx);
            
            idx = CopyBytes(townBytes, npcOldX, idx);
            idx = CopyBytes(townBytes, npcOldY, idx);
            idx = CopyBytes(townBytes, npcAggressivity, idx);

            idx = CopyBytes(townBytes, npcMovement, idx);
            idx = CopyBytes(townBytes, npcConversationIdx, idx);

            return townBytes;
        }

        private int CopyBytes(byte[] dest, byte[] src, int destStartIdx)
        {
            for (int i = 0; i < 32; i++)
            {
                dest[destStartIdx++] = src[i];
            }

            return destStartIdx;
        }
    }
}