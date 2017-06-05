using System;

namespace foundation
{
    public interface IView:IEventDispatcher
    {
        /// <summary>
        ///  注册一个视图中介; 
        /// </summary>
        /// <param name="mediator"></param>
		void registerMediator( IMediator mediator );
		
		/// <summary>
		/// 注册一个别名; 
		/// </summary>
		/// <param name="mediatorName"></param>
		/// <param name="mediator"></param>
		void registerMediatorAlias( string mediatorName, IMediator mediator );
		
		/// <summary>
		/// 取得 一个视图中介;
		/// </summary>
		/// <param name="mediatorName"></param>
		/// <returns></returns>
		IMediator getMediator( string mediatorName );
		
		/// <summary>
		/// 删除一个视图中介;
		/// </summary>
		/// <param name="mediatorName"></param>
		/// <returns></returns>
		IMediator removeMediator( string mediatorName ) ;
		
		/// <summary>
		/// 是否存在相应的视图中介;
		/// </summary>
		/// <param name="mediatorName"></param>
		/// <returns></returns>
		bool hasMediator( string mediatorName ) ;
		
			
		void registerEvent(ASDictionary<string,Action<EventX>> eventMapping,bool isBind=true);
		
		/// <summary>
		/// 清理它管理的所有内容; 
		/// </summary>
		void clear();

        ASDictionary<string, IMediator> getMediatorDic();
    }
}
