using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace U4DosRandomizer.Helpers
{
    public class FileHelper
    {
        private const string fileHashes = "{\"BRITAIN.TLK\":\"61f30252a9fd7adcab316f634a38be55b79320b1adbdf32a0e7074b5996d81bb\",\"COVE.TLK\":\"6f0f36e358fb1d47482ad68b6b3834c79278a9290e10e39edc6a238a89deba59\",\"DEN.TLK\":\"d99f96e9233ad340b2b611b158c8b97e4685dbfe17b134f6306d74649eacc8b8\",\"EMPATH.TLK\":\"aa34c9f0b1195ad09e8467575ee7ae957d07934a924fba80ced1e3e55efa4dcc\",\"JHELOM.TLK\":\"5f0d01365c1bbee4c6b5671c26572a40461bc951d774693e6c5895d5912e0a4d\",\"LCB.TLK\":\"41842efb6b6ac57e172ca801c48744e94af4ae0dc20fbcc5064870e9c43d7503\",\"LYCAEUM.TLK\":\"34f4cd03c719106ab9b4761f772b13243bd8cf5c11057ca6719f50d2f51ffc29\",\"MAGINCIA.TLK\":\"b614469723843105c6466fb5a21cffeb620f39db691a16bfa7360f148b33f273\",\"MINOC.TLK\":\"4dcec9c31ef49689baaf9ba02aaf9cf2a5ce4ebbcd92acc8f782d20711d480a2\",\"MOONGLOW.TLK\":\"f5bf3df504e317bddac1a7073e3a804e24066820b9d19fe79a7ff51a945eaff9\",\"PAWS.TLK\":\"e5db787bc4336fdb8d4a5b873d5852504d014ee0aada7c285c23ca905e0d8421\",\"SERPENT.TLK\":\"996e1733ec5e0f585da238159dba3a947f267a1297ee92bb067efdaa6744e69d\",\"SKARA.TLK\":\"66a6a7b8b3bbca93d780bf1fd1ad4be8f8418926adf944c19f2e95d4cd0c0f45\",\"TRINSIC.TLK\":\"03faac9f4a3a3b77164c4e9a799cf589f90dfef7a8a3310b42b13892cf6664e3\",\"VESPER.TLK\":\"bb236798f643e419c2af0e1f88aa78c2040ec97b3c836ef56f25636bb4b7d2d7\",\"YEW.TLK\":\"f807c9a8928c66cc81f10746cce3f4df350cc12b74610c81a6ac139d0294ec40\",\"AVATAR.EXE\":\"84ec144394cb561b047d6ef80b7bd8e8652f0159891ff273a3c051b3f9dfd368\",\"TITLE.EXE\":\"d372686cf4b2500cc9087462c5a565bd261ca8b2f61b8e7be0814852fc2a3218\",\"WORLD.MAP\":\"255ed28a43fa25549bbc43a8ecf242e11264f9809d6ca5896f234ff0ebe6cb70\"}";
        private static Dictionary<string, string> hashes = null;

        public static void TryBackupOriginalFile(string file)
        {
            if (hashes == null)
            {
                hashes = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileHashes);
            }
            var hash = HashHelper.GetHashSha256(file);
            if (hashes[Path.GetFileName(file)] == HashHelper.BytesToString(hash))
            {
                File.Copy(file, $"{file}.orig", true);
            }
            else if(File.Exists($"{file}.orig"))
            {
                hash = HashHelper.GetHashSha256($"{file}.orig");
                if (hashes[Path.GetFileName(file)] != HashHelper.BytesToString(hash))
                {
                    throw new FileNotFoundException($"Original version of {file} not found.");
                }
            }
            else
            {
                throw new FileNotFoundException($"Original version of {file} not found. Has this file been modified since U4 was installed?");
            }
        }
    }
}
