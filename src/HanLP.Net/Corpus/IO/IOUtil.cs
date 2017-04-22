using System;
using System.IO;
using HanLP.Net.Utility;

namespace HanLP.Net.Corpus.IO
{
    public class IOUtil
    {
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


        public static byte[] ReadBytesFromFile(string filePath) {
            try {
                return File.ReadAllBytes(filePath);
            }
            catch (Exception e) {
                Predefine.logger.Warn($"读取{filePath}时候发生异常" + e.Message);
            }
            return null;
        }
    }


}