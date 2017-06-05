using System.Collections.Generic;

namespace foundation
{
    internal class CoreLoaderQueue
    {
        public static int MAX = 100000;

        /// <summary>
        /// 默认限制数量
        /// </summary>
        public static int CONCURRENCE = 10;

        private static Stack<LoaderNode> nodePool = new Stack<LoaderNode>();
      
        private LoaderNode firstNode;
        private LoaderNode lastNode;
        private int _concurrence = 0;

        private Dictionary<RFLoader,LoaderNode> maping;

        public CoreLoaderQueue()
        {
            maping = new Dictionary<RFLoader, LoaderNode>();
        }

        public bool add(RFLoader value, int priority = 0)
        {
            LoaderNode t = null;
            if (maping.TryGetValue(value,out t))
            {
                return false;
            }

            LoaderNode newNode;
            if (nodePool.Count>0)
            {
                newNode = nodePool.Pop();
                newNode.next = newNode.pre = null;
            }
            else
            {
                newNode = new LoaderNode();
            }

            newNode.data = value;
            newNode.priority = priority;
            maping.Add(value, newNode);

            if (firstNode == null)
            {
                lastNode = firstNode = newNode;
                return true;
            }

            LoaderNode findNode = null;

            if (priority > lastNode.priority)
            {
                t = firstNode;
                LoaderNode pre;
                while (t != null)
                {
                    if (priority > t.priority)
                    {
                        pre = t.pre;
                        newNode.next = t;
                        t.pre = newNode;

                        if (pre != null)
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

            if (findNode == null)
            {
                lastNode.next = newNode;
                newNode.pre = lastNode;
                lastNode = newNode;
            }

            return true;

        }

        public bool remove(RFLoader value)
        {
            if (value == null)
            {
                return false;
            }
            LoaderNode t = null;
            if (maping.TryGetValue(value,out t)==false)
            {
                return false;
            }

            LoaderNode pre = t.pre;
            LoaderNode next = t.next;
            if (pre !=null)
            {
                pre.next = next;
            }
            else
            {
                firstNode = next;
            }

            if (next !=null)
            {
                next.pre = pre;
            }
            else
            {
                lastNode = pre;
            }

            maping.Remove(value);

            if (nodePool.Count < MAX)
            {
                t.data = null;
                nodePool.Push(t);
            }

            return false;
        }

        public void next()
        {
            if (firstNode == null)
            {
                return;
            }

            int popCount = CONCURRENCE - _concurrence;
            if (popCount < 1)
            {
                return;
            }

            RFLoader loader;
            LoaderNode t = firstNode;

            int i = 0;
            List<RFLoader> temp = new List<RFLoader>();
            while (t != null && popCount-- > 0)
            {
                nodePool.Push(t);

                loader = t.data;
                t.data = null;

                temp.Add(loader);
                maping.Remove(loader);

                i++;
                t = t.next;
            }

            if (t == null)
            {
                firstNode = lastNode = null;
            }
            else
            {
                firstNode = t;
                t.pre = null;
            }

            _concurrence += i;

            for (int j = 0; j < i; j++)
            {
                loader = temp[j];
                loader.addEventListener(EventX.COMPLETE, loadHandle, MAX);
                loader.addEventListener(EventX.FAILED, loadHandle, MAX);
                loader.addEventListener(EventX.DISPOSE, loadHandle, MAX);

                loader.load();
            }

        }

        private void loadHandle(EventX e)
        {

            _concurrence--;

            RFLoader target = e.target as RFLoader;

            //trace("loaderQueue",_concurrence,event.type,target.url);

            target.removeEventListener(EventX.COMPLETE, loadHandle);
            target.removeEventListener(EventX.FAILED, loadHandle);
            target.removeEventListener(EventX.DISPOSE, loadHandle);

            CallLater.Add(next);

        }
    }

    internal class LoaderNode
    {
        public LoaderNode next;

        public LoaderNode pre;

        public RFLoader data;

        public int priority = 0;

    }

}
