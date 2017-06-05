using foundation;
using System;
using UnityEngine;

namespace clayui
{
    public interface ITabItem : IEventDispatcher
    {
       /// <summary>
        /// 相关视图; 
       /// </summary>
		GameObject target{
            get;
            set;
        }
		
		/// <summary>
        /// 选择状态 
		/// </summary>
		bool selected{
            get;
            set;
        }
		
		/// <summary>
        ///  是否可用;
		/// </summary>
		bool enabled{
            get;
            set;
        }
		
		/// <summary>
        /// 索引; 
		/// </summary>
        uint index
        {
            get;
            set;
        }
    }
}
