using System;
using System.Collections.Generic;
using System.Text;

namespace HanLP.Net.Utility
{
    /// <summary>
    /// 对数字和字节进行转换。<br>
    /// 基础知识：<br>
    /// 假设数据存储是以大端模式存储的：<br>
    /// byte: 字节类型 占8位二进制 00000000<br>
    /// char: 字符类型 占2个字节 16位二进制 byte[0] byte[1]<br>
    /// int : 整数类型 占4个字节 32位二进制 byte[0] byte[1] byte[2] byte[3]<br>
    /// long: 长整数类型 占8个字节 64位二进制 byte[0] byte[1] byte[2] byte[3] byte[4] byte[5]
    /// byte[6] byte[7]<br>
    /// float: 浮点数(小数) 占4个字节 32位二进制 byte[0] byte[1] byte[2] byte[3]<br>
    /// double: 双精度浮点数(小数) 占8个字节 64位二进制 byte[0] byte[1] byte[2] byte[3] byte[4]
    /// byte[5] byte[6] byte[7]<br>
    /// </summary>
    public class ByteUtil
    {
        /// <summary>
        /// 将一个2位字节数组转换为char字符。<br>
        /// 注意，函数中不会对字节数组长度进行判断，请自行保证传入参数的正确性。
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <returns>char字符</returns>
        public static char BytesToChar(byte[] b) {
            return BitConverter.ToChar(b, 0);
        }

        /// <summary>
        /// 将一个8位字节数组转换为双精度浮点数。<br>
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <returns>双精度浮点数</returns>
        public static double BytesToDouble(byte[] b) {
            return BitConverter.ToDouble(b, 0);
        }

        /// <summary>
        /// 读取double，高位在前
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <param name="start">start</param>
        /// <returns></returns>
        public static double BytesHighFirstToDouble(byte[] bytes, int start) {
            return BitConverter.ToDouble(bytes, start);
        }

        /// <summary>
        /// 将一个4位字节数组转换为浮点数。<br>
        /// </summary>
        /// <param name="b">字节数组</param>
        /// <returns>浮点数</returns>
        public static float BytesToFloat(byte[] b) {
            return BitConverter.ToSingle(b, 0);
        }

        /// <summary>
        /// 字节数组和整型的转换，高位在前，适用于读取writeInt的数据
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="start">整型</param>
        public static int BytesHighFirstToInt(byte[] bytes, int start) {
            return BitConverter.ToInt32(bytes, start);
        }

        /// <summary>
        /// 字节数组转char，高位在前，适用于读取writeChar的数据
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="start">整型</param>
        public static char BytesHighFirstToChar(byte[] bytes, int start) {
            return BitConverter.ToChar(bytes, start);
        }

        /// <summary>
        /// 读取float，高位在前
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="start">整型</param>
        /// <returns></returns>
        public static float BytesHighFirstToFloat(byte[] bytes, int start) {
            return BitConverter.ToSingle(bytes, start);
        }

        /// <summary>
        /// 无符号整型输出
        /// </summary>
        /// <param name="outStream"></param>
        /// <param name="uInteger"></param>
        public static void WriteUnsignedInt(System.IO.Stream outStream, uint uInteger) {
            outStream.WriteByte((byte)((uInteger >> 8) & 0xFF));
            outStream.WriteByte((byte)((uInteger >> 0) & 0xFF));
        }

        public static int ConvertTwoCharToInt(char high, char low) {
            int result = high << 16;
            result |= low;
            return result;
        }

        public static char[] ConvertIntToTwoChar(uint n) {
            char[] result = new char[2];
            result[0] = (char)(n >> 16);
            result[1] = (char)(0x0000FFFF & n);
            return result;
        }

    }
}
