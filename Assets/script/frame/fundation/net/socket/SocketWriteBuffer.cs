using System;

namespace foundation
{
    /**
     * 写消息的buffer,1024长度,超出了后果自负
     */

    public class SocketWriteBuffer
    {
        private int index = 0;
        private byte[] data;

        public SocketWriteBuffer()
        {
            this.data = new byte[1024];
        }

        public void writeByte(byte value)
        {
            data[index] = value;
            index += Utils.SIZE_OF_BYTE;
        }

        public void writeInt(int value)
        {
//        byte[] sizeByte = Utils.intToBytes(value);
//        Array.Copy(sizeByte, 0, data, index, sizeByte.Length);
//        index += Utils.SIZE_OF_INT;
            Utils.writeRawVarint32(this, value);
        }

        public void writeLong(long value)
        {
            byte[] sizeByte = Utils.longToBytes(value);
            Array.Copy(sizeByte, 0, data, index, sizeByte.Length);
            index += Utils.SIZE_OF_LONG;
        }

        public void writeFloat(float value)
        {
            byte[] sizeByte = Utils.floatToBytes(value);
            Array.Copy(sizeByte, 0, data, index, sizeByte.Length);
            index += Utils.SIZE_OF_FLOAT;
        }

        public void writeString(string value)
        {
            if (value == null)
            {
                this.writeInt(0);
                return;
            }
            /* string的二进制流 */
            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(value);
            this.writeInt(strBytes.Length);
            /* string的数据 */
            Array.Copy(strBytes, 0, data, index, strBytes.Length);
            index += strBytes.Length;
        }

        public byte[] Data
        {
            get
            {
                byte[] result = new byte[index];
                for (int i = 0; i < index; i++)
                {
                    result[i] = data[i];
                }
                return result;
            }
        }

        public int getLength()
        {
            return index;
        }
    }


}