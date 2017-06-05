using System;

namespace foundation
{
    public class Utils
    {
        public static int SIZE_OF_INT = 4;
        public static int SIZE_OF_FLOAT = 4;
        public static int SIZE_OF_BYTE = 1;
        public static int SIZE_OF_LONG = 8;
        public static bool _debug = false;

        public static byte[] intToBytes(int intValue)
        {
            byte[] bytes = new byte[4];

            bytes[0] = (byte) (intValue >> 24);
            bytes[1] = (byte) (intValue >> 16);
            bytes[2] = (byte) (intValue >> 8);
            bytes[3] = (byte) intValue;
            return bytes;
        }

        public static byte[] longToBytes(long longValue)
        {
            byte[] bytes = new byte[Utils.SIZE_OF_LONG];
            for (int i = 0; i < 8; i++)
            {
                bytes[i] = (byte) (longValue >> 8*(7 - i));
            }
            return bytes;
        }

        public static byte[] floatToBytes(float value)
        {
            byte[] bytes = System.BitConverter.GetBytes(value);
            Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] StringToBytes(string stringValue)
        {
            /* string的二进制流 */
            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(stringValue);
            byte[] result = new byte[strBytes.Length + SIZE_OF_INT];
            /* string的长度 */
            Array.Copy(intToBytes(strBytes.Length), 0, result, 0, SIZE_OF_INT);
            /* string的数据 */
            Array.Copy(strBytes, 0, result, SIZE_OF_INT, strBytes.Length);
            return result;
        }

        public static int bytesToInt(byte[] intBytes)
        {
            Array.Reverse(intBytes);
            int intValue = System.BitConverter.ToInt32(intBytes, 0);
            return intValue;
        }

        /**
	 * 写入变长的32位int变量
	 * 
	 * @param output
	 * @param value
	 */

        public static void writeRawVarint32(SocketWriteBuffer output, int value)
        {
            int i = 0;
            while (i < 5)
            {
                if ((value & ~0x7F) == 0)
                {
                    output.writeByte((byte) value);
                    return;
                }
                else
                {
                    output.writeByte((byte) ((value & 0x7F) | 0x80));
                    value = UnsignedRightShift(value, 7);
                }
                i++;
            }
        }

        /**
	 * 无符号的右移
	 * 
	 */

        public static int UnsignedRightShift(int x, int y)
        {
            int mask = 0x7fffffff; //Integer.MAX_VALUE
            for (int i = 0; i < y; i++)
            {
                x >>= 1;
                x &= mask;
            }
            return x;
        }

        /**
	 * 读取变长的32位int变量
	 * 
	 * @param input
	 * @return
	 */

        public static int readRawVarint32(SocketReadBuffer input)
        {
            byte tmp = input.readByte();
            if ((tmp & 0x80) == 0)
            {
                return tmp;
            }
            int result = tmp & 0x7f;
            tmp = input.readByte();
            if ((tmp & 0x80) == 0)
            {
                result |= tmp << 7;
            }
            else
            {
                result |= (tmp & 0x7f) << 7;
                tmp = input.readByte();
                if ((tmp & 0x80) == 0)
                {
                    result |= tmp << 14;
                }
                else
                {
                    result |= (tmp & 0x7f) << 14;
                    tmp = input.readByte();
                    if ((tmp & 0x80) == 0)
                    {
                        result |= tmp << 21;
                    }
                    else
                    {
                        result |= (tmp & 0x7f) << 21;
                        tmp = input.readByte();
                        result |= tmp << 28;
                        if ((tmp & 0x80) != 0)
                        {
                            // Discard upper 32 bits.
                            for (int i = 0; i < 5; i++)
                            {
                                if ((input.readByte() & 0x80) == 0)
                                {
                                    return result;
                                }
                            }
                            // throw
                            // InvalidProtocolBufferException.malformedVarint();
                        }
                    }
                }
            }
            return result;
        }

        /**
	 * Encode a ZigZag-encoded 32-bit value. ZigZag encodes signed integers into
	 * values that can be efficiently encoded with varint. (Otherwise, negative
	 * values must be sign-extended to 64 bits to be varint encoded, thus always
	 * taking 10 bytes on the wire.)
	 * 
	 * @param n
	 *            A signed 32-bit integer.
	 * @return An unsigned 32-bit integer, stored in a signed int because Java
	 *         has no explicit unsigned support.
	 */

        public static int encodeZigZag32(int n)
        {
            // Note: the right-shift must be arithmetic
            return (n << 1) ^ (n >> 31);
        }

        /**
	 * Decode a ZigZag-encoded 32-bit value. ZigZag encodes signed integers into
	 * values that can be efficiently encoded with varint. (Otherwise, negative
	 * values must be sign-extended to 64 bits to be varint encoded, thus always
	 * taking 10 bytes on the wire.)
	 * 
	 * @param n
	 *            An unsigned 32-bit integer, stored in a signed int because
	 *            Java has no explicit unsigned support.
	 * @return A signed 32-bit integer.
	 */

        public static int decodeZigZag32(int n)
        {
            return UnsignedRightShift(n, 1) ^ -(n & 1);
        }

        /**
	 * 有符号int写入buff,变长
	 * 
	 * @param output
	 * @param value
	 */

        public static void writeSInt(SocketWriteBuffer output, int value)
        {
            writeRawVarint32(output, encodeZigZag32(value));
        }

        /**
	 * 有符号int读取,变长
	 * 
	 * @param input
	 * @return
	 */

        public static int readSInt(SocketReadBuffer input)
        {
            return decodeZigZag32(readRawVarint32(input));
        }

    }


}