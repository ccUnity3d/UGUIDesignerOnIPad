using System;

namespace foundation
{
    /**
     * 读消息的buffer
     */

    public class SocketReadBuffer
    {
        private int index;
        private byte[] data;

        public SocketReadBuffer(byte[] data, int index)
        {
            this.data = data;
            this.index = index;
        }

        public SocketReadBuffer Reuse(byte[] data, int index)
        {
            this.data = data;
            this.index = index;
            return this;
        }

        public byte readByte()
        {
            byte result = this.data[this.index];
            this.index += Utils.SIZE_OF_BYTE;
            return result;
        }

        public int getInt()
        {
            byte[] intBytes = new byte[4];
            Array.Copy(data, this.index, intBytes, 0, intBytes.Length);
            Array.Reverse(intBytes);
            int intValue = System.BitConverter.ToInt32(intBytes, 0);
            this.index += Utils.SIZE_OF_INT;
            return intValue;
        }

        public long getLong()
        {
            byte[] longBytes = new byte[8];
            Array.Copy(data, this.index, longBytes, 0, longBytes.Length);
            Array.Reverse(longBytes);
            long longValue = System.BitConverter.ToInt64(longBytes, 0);
            this.index += Utils.SIZE_OF_LONG;
            return longValue;
        }

        public float getFloat()
        {
            byte[] bytes = new byte[4];
            Array.Copy(data, this.index, bytes, 0, bytes.Length);
            Array.Reverse(bytes);
            float value = System.BitConverter.ToSingle(bytes, 0);
            this.index += Utils.SIZE_OF_FLOAT;
            return value;
        }

        public string getString()
        {
            int length = Utils.readRawVarint32(this);
            if (length <= 0)
            {
                return "";
            }
            string str = ToString(data, this.index, length);
            this.index += length;
            return str;
        }

        public static string ToString(byte[] data, int index, int length)
        {
            byte[] strBytes = new byte[length];
            Array.Copy(data, index, strBytes, 0, strBytes.Length);
            string s = System.Text.Encoding.UTF8.GetString(strBytes);
            return s;
        }
    }

}