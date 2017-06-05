namespace foundation
{
    public interface IMediator:IEventInterester,IInjectable
    {
        /// <summary>
        /// 中介的名称,用于与其它中介的区分; 
        /// </summary>
		string name{
            get;
        }

		void setView(IPanel value);
		
		IPanel getView();
		
		void setModel(IProxy value);
			
		IProxy getModel();
        
		
		void execute(string eventType,object data=null);
		
		/// <summary>
        ///  当被注册到应用中触发 
		/// </summary>
		
		void onRegister();
		
	    /// <summary>
        /// 当从应用中删除时触发; 
	    /// </summary>
		void onRemove();
		
		/// <summary>
        ///  休眠;
		/// </summary>
	
		void sleep();
		
		/// <summary>
        ///  唤醒 
		/// </summary>
		void awaken();
		
    }
}
