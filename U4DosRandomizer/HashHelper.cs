using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace U4DosRandomizer
{
    public class HashHelper
    {
        private static SHA256 Sha256 = SHA256.Create();

        public static byte[] GetHashSha256(string filename)
        {
            using (FileStream stream = File.OpenRead(filename))
            {
                return Sha256.ComputeHash(stream);
            }
        }

        // Return a byte array as a sequence of hex values.
        public static string BytesToString(byte[] bytes)
        {
            string result = "";
            foreach (byte b in bytes) result += b.ToString("x2");
            return result;
        }

        //public void WriteHashes(string path)
        //{
        //    var file = Path.Combine(path, "AVATAR.EXE");

        //    var townTalkHash = new Dictionary<string, string>();

        //    var hash = HashHelper.GetHashSha256(file);
        //    Console.WriteLine($"{file}: {HashHelper.BytesToString(hash)}");
        //    townTalkHash.Add(Path.GetFileName(file), HashHelper.BytesToString(hash));

        //    string json = JsonConvert.SerializeObject(townTalkHash); // the dictionary is inside client object
        //                                                             //write string to file
        //    System.IO.File.WriteAllText(@"avatar_hash.json", json);
        //}


        //public void WriteHashes(string path)
        //{
        //    var files = Directory.GetFiles(path, "*.TLK");

        //    var townTalkHash = new Dictionary<string, string>();

        //    foreach (var file in files)
        //    {
        //        var hash = HashHelper.GetHashSha256(file);
        //        Console.WriteLine($"{file}: {HashHelper.BytesToString(hash)}");
        //        townTalkHash.Add(Path.GetFileName(file), HashHelper.BytesToString(hash));
        //    }

        //    string json = JsonConvert.SerializeObject(townTalkHash); // the dictionary is inside client object
        //                                                             //write string to file
        //    System.IO.File.WriteAllText(@"talk_hash.json", json);
        //}
    }
}
