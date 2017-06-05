namespace foundation
{
    public enum AssetState
    {
        DISPOSE,//被销毁
        NONE,// 未加载
        PARSING,//解析状态
        PARSED,//解析状态

        LOADING,// 加载中
        READY,// 已加载
        FAILD// 加载失败
    }
}
