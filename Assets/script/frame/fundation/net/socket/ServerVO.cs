namespace foundation
{
    public class ServerVO
    {
        /// <summary>
        ///     点击该服务器对应的服务器ip
        /// </summary>
        public string ip;

        /// <summary>
        ///     服务器名字
        /// </summary>
        public string name;

        /// <summary>
        ///     端口号
        /// </summary>
        public int port;

        /// <summary>
        ///     区
        /// </summary>
        public int region;

        /// <summary>
        /// 
        /// </summary>
        public int serverID;

        /// <summary>
        ///     服务器状态，0良好，1爆满
        /// </summary>
        public int status;

        public int type;
    }
}
