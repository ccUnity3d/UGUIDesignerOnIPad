using System;
using System.Collections.Generic;

namespace foundation
{
    public class Signal:QueueHandle
    {
        public Signal()
        {

        }

        public bool add(Action<EventX> value, int priority = 0)
        {
            if (maping == null)
            {
                maping = new Dictionary<Action<EventX>, SignalNode>();
            }

            SignalNode t = null;
            if (maping.TryGetValue(value,out t))
            {
                //如果已被删除过程中又被添加(好神奇的逻辑,但必然会有这种情况，不是可能);
                if (t.active == 0)
                {
                    if (dispatching)
                    {
                        t.active = 2;
                    }
                    else
                    {
                        t.active = 1;
                    }
                    return true;
                }
                return false;
            }

            SignalNode newNode = getSignalNode();

            newNode.data = value;
            newNode.priority = priority;

            maping[value] = newNode;

            if (dispatching)
            {
                newNode.active = 2;
            }

            if (firstNode == null)
            {
                len = 1;
                lastNode = firstNode = newNode;
                return true;
            }

            SignalNode findNode = null;
            if (priority > lastNode.priority)
            {
                t = firstNode;
                SignalNode pre;
                //var next:SignalNode;
                while (null != t)
                {
                    if (priority > t.priority)
                    {
                        pre = t.pre;
                        //next=t.next;
                        newNode.next = t;
                        t.pre = newNode;

                        if (null != pre)
                        {
                            pre.next = newNode;
                            newNode.pre = pre;
                        }
                        else
                        {
                            firstNode = newNode;
                        }

                        findNode = t;

                        break;
                    }
                    t = t.next;
                }
            }

            if (null == findNode)
            {
                lastNode.next = newNode;
                newNode.pre = lastNode;
                lastNode = newNode;
            }
            len++;
            return true;
        }
		
		
		public bool remove(Action<EventX> value){
			return removeHandle(value);
		}
    }
}
