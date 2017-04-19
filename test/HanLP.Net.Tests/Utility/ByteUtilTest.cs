using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Xunit;
using HanLP.Net.Utility;

namespace HanLP.Net.Tests.Utility
{
    public class ByteUtilTest : TestBase
    {

        public string DATA_TEST_outStream_BIN = "data/test/outStream.bin";

        public ByteUtilTest() {
            DATA_TEST_outStream_BIN = Path.Combine(Config.DataRootPath, DATA_TEST_outStream_BIN);
            var testPath = Directory.GetParent(DATA_TEST_outStream_BIN).ToString();
            if (!Directory.Exists(testPath)){
                Directory.CreateDirectory(testPath);
            }
        }

        [Fact]
        public void TestReadDouble() {
            var outStream = new BinaryWriter(new FileStream(DATA_TEST_outStream_BIN, FileMode.OpenOrCreate));
            outStream.Write(0.123456789d);
            outStream.Write(3389);
            outStream.Flush();
            outStream.Dispose();

            var readStream = new BinaryReader(new FileStream(DATA_TEST_outStream_BIN, FileMode.Open));
            var doubleResult = readStream.ReadDouble();
            var intResult = readStream.ReadInt32();
            readStream.Dispose();

            Assert.Equal(doubleResult, 0.123456789d);
            Assert.Equal(intResult, 3389);
        }

        [Fact]
        public void TestReadUTF() {
            var outStream = new BinaryWriter(new FileStream(DATA_TEST_outStream_BIN, FileMode.OpenOrCreate));
            outStream.Write("hankcs你好123");
            outStream.Write(3389);
            outStream.Flush();
            outStream.Dispose();

            var readStream = new BinaryReader(new FileStream(DATA_TEST_outStream_BIN, FileMode.Open));
            var stringResult = readStream.ReadString();
            readStream.Dispose();

            Assert.Equal(stringResult, "hankcs你好123");
        }

        [Fact]
        public void TestReadUnsignedShort() {

            var outStream = new BinaryWriter(new FileStream(DATA_TEST_outStream_BIN, FileMode.OpenOrCreate));
            ushort utflen = 123;
            outStream.Write(utflen);
            outStream.Flush();
            outStream.Dispose();

            var readStream = new BinaryReader(new FileStream(DATA_TEST_outStream_BIN, FileMode.Open));
            var uintResult = readStream.ReadUInt16();
            readStream.Dispose();

            Assert.Equal(uintResult, utflen);
        }

        [Fact]
        public void TestConvertCharToInt() {
            for (uint i = 0; i < 50; ++i) {
                uint n = i;
                char[] twoChar = ByteUtil.ConvertIntToTwoChar(n);
                Assert.Equal(n, ByteUtil.ConvertTwoCharToInt(twoChar[0], twoChar[1]));
            }
        }
    }
}
