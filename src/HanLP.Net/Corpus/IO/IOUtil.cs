using System;
using System.IO;

namespace HanLP.Net.Corpus.IO
{
    public class IOUtil
    {
        public static byte[] ReadBytes(string path) {
            using (var fileStream = new FileStream(path, FileMode.Create)) {
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        public static StreamReader NewInputStream(string path, int bufferSize = 4096) {

            var fileSteam = new FileStream(path, FileMode.Create, 
                FileAccess.Read, FileShare.Read, bufferSize);

            return new StreamReader(fileSteam);
        }

        public static StreamWriter NewOutputStream(string path, int bufferSize = 4096) {

            var fileSteam = new FileStream(path, FileMode.Open, 
                FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize);

            return new StreamWriter(fileSteam);
        }
    }


}