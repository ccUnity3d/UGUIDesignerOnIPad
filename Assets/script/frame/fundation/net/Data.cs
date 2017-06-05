using UnityEngine;
using System;
using System.IO;
using foundation;


    /**
     * 业务层的数据包装区 业务消息的read和write都会被封装到Message中
     * 
     * @author youxun
     * 
     */

    public class Data
    {
        private SocketWriteBuffer writeBuffer;
        private SocketReadBuffer readBuffer;

        public Data(SocketWriteBuffer writeBuffer, SocketReadBuffer readBuffer)
        {
            this.writeBuffer = writeBuffer;
            this.readBuffer = readBuffer;
        }

        public Data Reuse(SocketWriteBuffer writeBuffer, SocketReadBuffer readBuffer)
        {
            this.writeBuffer = writeBuffer;
            this.readBuffer = readBuffer;
            return this;
        }

        public void writeBool(bool value)
        {
            writeBuffer.writeByte(value ? (byte) 1 : (byte) 0);
        }

        public void writeInt(int value)
        {
            writeBuffer.writeInt(value);
        }

        public void writeSInt(int value)
        {
            Utils.writeSInt(writeBuffer, value);
        }

        public void writeFloat(float value)
        {
            writeBuffer.writeFloat(value);
        }

        public void writeLong(long value)
        {
            writeBuffer.writeLong(value);
        }

        public void writeString(string value)
        {
            writeBuffer.writeString(value);
        }

        public void writeCompressString(String s)
        {
            if (s != null)
            {
                String compress;
                try
                {
                    compress = GZipStrUtil.CompressString(s);
                }
                catch (IOException e)
                {
                    DebugX.Log("dara  writeCompressString ~" + e);
                    compress = s;
                    Debug.LogException(e);
                }
                if (s.Length > compress.Length)
                {
                    Debug.Log(1.0f*compress.Length/s.Length + GZipStrUtil.DecompressString(compress));
                    // 压缩了
                    this.writeInt(1);
                    this.writeString(compress);
                }
                else
                {
                    // 没有压缩
                    this.writeInt(0);
                    this.writeString(s);
                }

            }
            else
            {
                this.writeInt(0);
                this.writeInt(0);
            }

        }

        public bool getBool()
        {
            byte b = readBuffer.readByte();
            return b == (byte) 1 ? true : false;
        }

        public int getInt()
        {
            //        return readBuffer.getInt();
            return Utils.readRawVarint32(readBuffer);
        }

        public int getSInt()
        {
            return Utils.readSInt(readBuffer);
        }

        public long getLong()
        {
            return readBuffer.getLong();
        }

        public float getFloat()
        {
            return readBuffer.getFloat();
        }

        public String getString()
        {
            return readBuffer.getString();
        }

        public String getCompressString()
        {
            int compress = this.getInt();
            String s = this.getString();
            if (s == null || "".Equals(s))
            {
                return null;
            }
            if (compress == 1)
            {
                try
                {
                    s = GZipStrUtil.DecompressString(s);
                }
                catch (IOException e)
                {
                    DebugX.Log("dara  getCompressString ~" + e);
                }
            }
            return s;
        }

        public byte[] getBytes()
        {
            return writeBuffer.Data;
        }
    }
