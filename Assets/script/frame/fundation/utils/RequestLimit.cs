using System;
using System.Collections.Generic;

namespace foundation
{
    /// <summary>
    ///  请求限制
    ///  用于不让用户反复快速的点击类型操作
    /// </summary>
    public class RequestLimit
    {
		private static Dictionary<object,long> dic;
		public RequestLimit()
		{
			dic=new Dictionary<object,long>();
		}
	
        /// <summary>
        /// /**
        /// </summary>
        /// <param name="o">锁定的对像(可以是任何类型,它会被当做一个key) </param>
        /// <param name="time">time 锁定对像 毫秒数 </param>
        /// <returns>是否已解锁 true为没有被限制,false 被限制了</returns>
		public static bool check(object o,int time=500){
			long value;
            long now = DateTime.Now.Ticks;
			if(dic.TryGetValue(o,out value)==false){
                dic[o] = time + now;
                return true;
            }
			
			if(value==0){
                dic[o] = time + now;
				return true;
			}

            long i = value - now;
			if(i>0){
				return false;
			}

            dic[o] = time + now;
			return true;
		}
    }
}
