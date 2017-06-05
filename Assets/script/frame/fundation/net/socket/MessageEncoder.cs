namespace foundation
{
    using System;

    public class MessageEncoder
    {
        //public static SimpleEncrypt simpleEncrypt = new SimpleEncrypt(SimpleEncrypt.INIT_KEY);


        /**
         * 生成消息
         */
        public static byte[] encode(AbstractMessage obj)
        {
            //把数据包的长度写入byte数组中
            //把结构体对象转换成数据包，也就是字节数组
            Data myData = new Data(new SocketWriteBuffer(), null);
            obj.encode(myData);
            byte[] data = myData.getBytes();
            //data = simpleEncrypt.encode(data);
            byte[] head = encodeHead(obj, data);
            //此时就有了两个字节数组，一个是标记数据包的长度字节数组， 一个是数据包字节数组，
            //同时把这两个字节数组合并成一个字节数组

            byte[] newByte = new byte[head.Length + data.Length];
            Array.Copy(head, 0, newByte, 0, head.Length);
            Array.Copy(data, 0, newByte, head.Length, data.Length);
            return newByte;
        }

        /**
         * 消息头生成
         */
        public static byte[] encodeHead(AbstractMessage obj, byte[] data)
        {
            byte[] head = new byte[Utils.SIZE_OF_INT * 2];
            byte[] sizeByte = Utils.intToBytes(head.Length + data.Length);
            //Array.Reverse(sizeByte);
            byte[] mainByte = Utils.intToBytes(obj.getMessageType());
            //byte [] subByte = Utils.intToBytes(obj.getSub());
            Array.Copy(sizeByte, 0, head, 0, sizeByte.Length);
            Array.Copy(mainByte, 0, head, sizeByte.Length, mainByte.Length);
            //Array.Copy(subByte,0,head,(sizeByte.Length+mainByte.Length),subByte.Length);
            return head;
        }

    }

}
