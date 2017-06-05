using System;
using System.Collections.Generic;

namespace foundation
{
    public class QueueHandle
    {
        private static Stack<SignalNode> nodePool=new Stack<SignalNode>();
		private static int MAX=1000;
		
		internal SignalNode firstNode;
		internal SignalNode lastNode;
		
		protected Dictionary<Action<EventX>, SignalNode> maping;
		
		protected int len =0;
		
		internal bool dispatching=false;

        public QueueHandle()
        {
        }


        public int length{
            get
            {
                return len;
            }
		}

        private static Stack<List<SignalNode>> signalNodeListPool = new Stack<List<SignalNode>>();
        private static List<SignalNode> getSignalNodeList()
        {
            if (signalNodeListPool.Count>0)
            {
                List<SignalNode> temp= signalNodeListPool.Pop();
                temp.Clear();
                return temp;
            }

            return new List<SignalNode>();
        }

        private static void recycle(List<SignalNode> node)
        {
            if (signalNodeListPool.Count <300)
            {
                signalNodeListPool.Push(node);
            }
        }

        public void dispatch(EventX e)
        {
            if (len > 0)
            {
                dispatching = true;
                SignalNode t = firstNode;

                List<SignalNode> temp=getSignalNodeList();

                while (t != null)
                {
                    if (t.active == 1)
                    {
                        t.data(e);
                    }
                    temp.Add(t);
                     t = t.next;
                }
                dispatching = false;

                foreach (SignalNode item in temp)
                {
                    if (item.active == 0)
                    {
                        _remove(item, item.data);
                    }
                    else if (item.active == 2)
                    {
                        item.active = 1;
                    }
                }


                recycle(temp);
            }
        }



        protected bool addHandle(Action<EventX> value)
        {
            if (maping == null) {
                maping = new Dictionary<Action<EventX>, SignalNode>();
            }
            SignalNode t =null;
            if (maping.TryGetValue(value, out t)) {
                if (t.active == 0) {
                    if (dispatching) {
                        t.active = 2;
                    } else {
                        t.active = 1;
                    }
                    return true;
                }
                return false;
            }

            t = getSignalNode();
            t.data = value;
            maping.Add(value, t);

            if (dispatching) {
                t.active = 2;
            }

            if (lastNode != null)
            {
                lastNode.next = t;
                t.pre = lastNode;
                lastNode = t;
            }
            else
            {
                firstNode = lastNode = t;
            }

            len++;

            return true;
        }

        protected SignalNode getSignalNode()
        {
            SignalNode t;
            if (nodePool.Count > 0)
            {
                t = nodePool.Pop();
                t.next = t.pre = null;
                t.active = 1;
            }
            else
            {
                t = new SignalNode();
            }
            return t;
        }


        protected bool removeHandle(Action<EventX> value)
		{
			if(lastNode==null || maping==null){
				return false;
			}

            SignalNode t = null;
			if( maping.TryGetValue(value,out t)==false || t.active==0){
				return false;
			}
			
			if(dispatching){
				t.active=0;
				return true;
			}

            return _remove(t, value);
        }

        public bool hasHandle(Action<EventX> value) {
            if (maping==null)
            {
                return false;
            }
            return maping[value] != null;
        }
		
		protected bool _remove(SignalNode t, Action<EventX> value){
		    if (t == null)
		    {
		        DebugX.Log("d");
		    }
          
            SignalNode pre =t.pre;
            SignalNode next=t.next;
			if(pre !=null){
				pre.next=next;
			}else{    
                firstNode =next;
			}

		   

            if (next !=null){
				next.pre=pre;
			}else{
				lastNode=pre;
			}
			t.active=0;
		    
            maping.Remove(value);

            if (nodePool .Count< MAX){
				t.data=null;
				nodePool.Push(t);
			}
			len--;
         
            return true;
		}
		
		
		public void _clear()
		{		
			if(null==firstNode){				
				return;
			}

            SignalNode t =firstNode;
			while(t !=null){
				t.data=null;
				if(nodePool.Count<MAX){
					nodePool.Push(t);
				}else{
					break;
				}
				t=t.next;
			}
			
			maping=null;
			firstNode=lastNode=null;
			len=0;
		}

    }
}
