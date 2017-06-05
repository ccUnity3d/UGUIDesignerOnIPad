namespace foundation
{
    /// <summary>
    ///  数据接口,
    ///  可代理数据的显示及操作的类可实现此接口
    /// </summary>
    public interface IDataRenderer
    {
        /// <summary>
        /// 取得数据
        /// </summary>
        object data
        {
            get;
            set;
        }
    }
}
