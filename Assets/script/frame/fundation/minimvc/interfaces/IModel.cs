
namespace foundation
{
    public interface IModel
    {
       /// <summary>
       /// 注册数据代理; 
       /// </summary>
       /// <param name="proxy"></param>
		void registerProxy( IProxy proxy);
		
		/// <summary>
		/// 注册一个别名; 
		/// </summary>
		/// <param name="proxyName"></param>
		/// <param name="proxy"></param>
		void registerProxyAlias(string proxyName,IProxy proxy);
		
		/// <summary>
		/// 取得数据代理;
		/// </summary>
		/// <param name="proxyName"></param>
		/// <returns></returns>
		IProxy getProxy( string proxyName ) ;
		
		/// <summary>
		/// 删除数据代理; 
		/// </summary>
		/// <param name="proxyName"></param>
		/// <returns></returns>
		IProxy removeProxy( string proxyName ) ;
		
		/// <summary>
		/// 取得所有代理定义 
		/// </summary>
		/// <returns></returns>
		ASDictionary<string ,IProxy> getAllPorxy();
		
	    /// <summary>
	    /// 是否存在相应的数据代理; 
	    /// </summary>
	    /// <param name="proxyName"></param>
	    /// <returns></returns>
		bool hasProxy( string proxyName );
    }
}
