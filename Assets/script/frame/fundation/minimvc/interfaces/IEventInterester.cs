using System;

namespace foundation
{
    /// <summary>
    ///  事件关注者
    /// </summary>
    public interface IEventInterester
    {
        /**
		 *  对哪些事件观注; 
		 * @return 
		 * 
		 */
        ASDictionary<string,Action<EventX>> eventInterests{
            get;
        }
    }
}
