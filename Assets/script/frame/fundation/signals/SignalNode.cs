using  System;
namespace foundation
{
    public class SignalNode
    {
        public SignalNode next;
		
		public SignalNode pre;
		
		public Action<EventX> data;

        /// <summary>
        /// 0:将删除;
        /// 1:正在运行
        /// 2:将加入; 
        /// </summary>
        internal int active = 1;

		public int priority =0;
	
    }
}
