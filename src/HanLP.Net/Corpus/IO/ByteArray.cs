using System;
using System.Text;
using HanLP.Net.Utility;
using static HanLP.Net.Utility.Predefine;

namespace HanLP.Net.Corpus.IO
{
    public class ByteArray
    {
        byte[] bytes;
        int offset;

        public ByteArray(byte[] bytes) {
            this.bytes = bytes;
        }

        /// <summary>
        /// 从文件读取一个字节数组
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ByteArray CreateByteArray(string path) {
            byte[] bytes = IOUtil.ReadBytes(path);
            if (bytes == null) return null;
            return new ByteArray(bytes);
        }

        /// <summary>
        /// 获取全部字节
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes() {
            return bytes;
        }

        /// <summary>
        /// 读取一个int
        /// </summary>
        /// <returns></returns>
        public int NextInt() {
            int result = ByteUtil.BytesHighFirstToInt(bytes, offset);
            offset += 4;
            return result;
        }

        public double NextDouble() {
            double result = ByteUtil.BytesHighFirstToDouble(bytes, offset);
            offset += 8;
            return result;
        }

        public char NextChar() {
            char result = ByteUtil.BytesHighFirstToChar(bytes, offset);
            offset += 2;
            return result;
        }

        public byte NextByte() {
            return bytes[offset++];
        }

        public bool HasMore() {
            return offset < bytes.Length;
        }

        /// <summary>
        /// 读取一个String，注意这个String是双字节版的，在字符之前有一个整型表示长度
        /// </summary>
        /// <returns></returns>
        public String NextString() {
            char[] buffer = new char[NextInt()];
            for (int i = 0; i < buffer.Length; ++i) {
                buffer[i] = NextChar();
            }
            return new String(buffer);
        }

        public float NextFloat() {
            float result = ByteUtil.BytesHighFirstToFloat(bytes, offset);
            offset += 4;
            return result;
        }

        /// <summary>
        /// 读取一个无符号短整型
        /// </summary>
        public int NextUnsignedShort() {
            byte a = NextByte();
            byte b = NextByte();
            return (((a & 0xff) << 8) | (b & 0xff));
        }

        /// <summary>
        /// 读取一个UTF字符串
        /// </summary>
        public String NextUTF() {
            int utflen = NextUnsignedShort();
            byte[] bytearr = null;
            char[] chararr = null;
            bytearr = new byte[utflen];
            chararr = new char[utflen];

            int c, char2, char3;
            int count = 0;
            int chararr_count = 0;

            for (int i = 0; i < utflen; ++i) {
                bytearr[i] = NextByte();
            }

            while (count < utflen) {
                c = (int)bytearr[count] & 0xff;
                if (c > 127) break;
                count++;
                chararr[chararr_count++] = (char)c;
            }

            while (count < utflen) {
                c = (int)bytearr[count] & 0xff;
                switch (c >> 4) {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        /* 0xxxxxxx*/
                        count++;
                        chararr[chararr_count++] = (char)c;
                        break;
                    case 12:
                    case 13:
                        /* 110x xxxx   10xx xxxx*/
                        count += 2;
                        if (count > utflen)
                            logger.Warn(
                                    "malformed input: partial character at end");
                        char2 = (int)bytearr[count - 1];
                        if ((char2 & 0xC0) != 0x80)
                            logger.Warn(
                                    "malformed input around byte " + count);
                        chararr[chararr_count++] = (char)(((c & 0x1F) << 6) |
                                (char2 & 0x3F));
                        break;
                    case 14:
                        /* 1110 xxxx  10xx xxxx  10xx xxxx */
                        count += 3;
                        if (count > utflen)
                            logger.Warn(
                                    "malformed input: partial character at end");
                        char2 = (int)bytearr[count - 2];
                        char3 = (int)bytearr[count - 1];
                        if (((char2 & 0xC0) != 0x80) || ((char3 & 0xC0) != 0x80))
                            logger.Warn(
                                    "malformed input around byte " + (count - 1));
                        chararr[chararr_count++] = (char)(((c & 0x0F) << 12) |
                                ((char2 & 0x3F) << 6) |
                                ((char3 & 0x3F) << 0));
                        break;
                    default:
                        /* 10xx xxxx,  1111 xxxx */
                        logger.Warn(
                                "malformed input around byte " + count);
                        break;
                }
            }
            // The number of chars produced may be less than utflen
            return new string(chararr, 0, chararr_count);
        }

        public int GetOffset() {
            return offset;
        }

        public int GetLength() {
            return bytes.Length;
        }

        public void Close() {
            bytes = null;
        }
    }
}
