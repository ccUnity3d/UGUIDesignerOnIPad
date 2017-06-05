namespace foundation
{
    /// <summary>
    ///  数据模型代码接口;
    /// </summary>
    public interface IProxy:INotifier,IEventDispatcher,IEventInterester,IInjectable
    {
		/// <summary>
        /// 名称,用于与其它代理对像的区分;
		/// </summary>
		string name{
            get;
            set;
        }
	
		/// <summary>
		/// 
		/// </summary>
		/// <param name="eventType"></param>
		/// <param name="data"></param>
		void execute(string eventType,object data=null);
		
		/// <summary>
        ///  当被注册到应用中时触发; 
		/// </summary>
		void onRegister( );
		
		/// <summary>
        /// 当从应用中删除时触发; 
		/// </summary>
		void onRemove( );
    }
}
