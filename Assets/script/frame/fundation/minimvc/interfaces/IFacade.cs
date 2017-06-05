using System;

namespace foundation
{
    public interface IFacade : INotifier, IInject,IEventDispatcher
    {
        /// <summary>
        /// 注册数据代理;
        /// </summary>
        /// <param name="proxy"></param>
        void registerProxy(IProxy proxy);

        /// <summary>
        /// 取得相应的数据代理; 
        /// </summary>
        /// <param name="proxyName"></param>
        /// <returns></returns>
        IProxy getProxy(string proxyName);

        /// <summary>
        /// 删除数据代理; 
        /// </summary>
        /// <param name="proxyName"></param>
        /// <returns></returns>
        IProxy removeProxy(string proxyName);

        /// <summary>
        /// 是否含有相应的数据代理; 
        /// </summary>
        /// <param name="proxyName"></param>
        /// <returns></returns>
        bool hasProxy(string proxyName);

        /// <summary>
        /// 注册视图代理控制器;
        /// </summary>
        /// <param name="mediator"></param>
        void registerMediator(IMediator mediator);

        /// <summary>
        ///  取得相应视图代理控制器; 
        /// </summary>
        /// <param name="mediatorName"></param>
        /// <returns></returns>
        IMediator getMediator(string mediatorName);

        /// <summary>
        /// 删除视图代理控制器;  
        /// </summary>
        /// <param name="mediatorName"></param>
        /// <returns></returns>
        IMediator removeMediator(string mediatorName);

        /// <summary>
        ///  是否含有视图代理控制器;
        /// </summary>
        /// <param name="mediatorName"></param>
        /// <returns></returns>
        bool hasMediator(string mediatorName);

        /// <summary>
        /// 启动eventInterests监听
        /// </summary>
        /// <param name="eventInterester"></param>
        /// <param name="isBind"></param>
        void registerEvent(ASDictionary<string, Action<EventX>> eventMapping, bool isBind=true);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="eventMapping"></param>
        void registerProxyEvent(ASDictionary<string, Action<EventX>> eventMapping,IProxy proxy, bool isBind = true);

        /// <summary>
        /// 显示隐藏Mediator的视图;1打开，0关闭，-1打开或关闭
        /// </summary>
        /// <param name="mediatorName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IMediator toggleMediator(string mediatorName, int type = -1);

        /// <summary>
        /// 直接运行mediator的exexute方法;
        /// </summary>
        /// <param name="mediatorName"></param>
        /// <param name="eventType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        IMediator executeMediator(string mediatorName, string eventType, object data = null,int type=1);

        /// <summary>
        /// 直接运行proxy的exexute方法; 
        /// </summary>
        /// <param name="proxyName"></param>
        /// <param name="eventType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        IProxy executeProxy(string proxyName, string eventType, object data = null);

        /// <summary>
        /// 自动初始化;
        /// </summary>
        /// <param name="key"></param>
        void autoInitialize(string key);

        void changeState(string currentState);

        /// <summary>
        /// 当前被锁定注册的对像 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        object getInjectLock(string className);

        void clear();
    }
}
