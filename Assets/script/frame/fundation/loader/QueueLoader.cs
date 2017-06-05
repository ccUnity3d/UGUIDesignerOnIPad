using System;
using System.Collections.Generic;

namespace foundation
{
    public class QueueLoader:EventDispatcher
    {
        private static  Stack<QueueLoader> queueLoaderPool=new Stack<QueueLoader>();
        private Queue<AssetResource> queue=new Queue<AssetResource>();
        private Dictionary<string, Action<EventX>> resultActions=new Dictionary<string, Action<EventX>>();
        private  List<AssetResource> runningList=new List<AssetResource>(); 
        private  Dictionary<string, AssetResource> urlMapping=new Dictionary<string, AssetResource>();
        public uint retryCount = 0;
        private int threadCount = 2;
        private int running = 0;

        private int total = 0;
        private int loaded = 0;
        private bool isStart=false;

        public static QueueLoader Get()
        {
            if (queueLoaderPool.Count>0)
            {
                return queueLoaderPool.Pop();
            }
            return new QueueLoader();
        }

        public static void Recycle(QueueLoader value)
        {
            value.recycle();
            if (queueLoaderPool.Count<100)
            {
                queueLoaderPool.Push(value);
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="uri"></param>
        /// <param name="type"></param>
        /// <param name="resultHandle">同url只返回一次回调</param>
        /// <returns></returns>
        public AssetResource add(string url, string uri, LoaderXDataType type, Action<EventX> resultHandle = null)
        {
            AssetResource resource = null;
            if (urlMapping.TryGetValue(uri, out resource))
            {
                return resource;
            }
            resource = AssetsManager.getResource(url, uri, type);
            if (resource == null)
            {
                return null;
            }
            innerAdd(uri,resource, resultHandle);
            return resource;
        }

       

        public void add(AssetResource resource, Action<EventX> resultHandle = null)
        {
            if (resource == null)
            {
                return;
            }
            string uri = resource.id;
            if (urlMapping.ContainsKey(uri))
            {
                return;
            }
            innerAdd(uri,resource,resultHandle);
        }

        private void innerAdd(string uri, AssetResource resource, Action<EventX> resultHandle)
        {
            urlMapping.Add(uri, resource);
            queue.Enqueue(resource);
            if (resultHandle != null && resultActions.ContainsKey(uri) == false)
            {
                resultActions.Add(uri, resultHandle);
            }

            if (running == 0 && isStart)
            {
                loadNext();
            }
        }

        public void start(int threadCount=2)
        {
            isStart = true;
            this.threadCount = threadCount;
            this.total = queue.Count;
            loadNext();
        }

        private List<AssetResource> newRunnigList=new List<AssetResource>();
        private  void loadNext()
        {
            int hasCount = queue.Count;
            if (running == 0 && hasCount == 0)
            {
                this.simpleDispatch(EventX.COMPLETE);
                return;
            }
           
            while (running < threadCount && hasCount> 0)
            {
                AssetResource resource = queue.Dequeue();
                runningList.Add(resource);
                resource.addEventListener(EventX.COMPLETE, itemComplete);
                resource.addEventListener(EventX.FAILED, itemComplete);
                //resource.addEventListener(AssetEvent.PROGRESS, itemProgress);
                running++;
                hasCount--;
                newRunnigList.Add(resource);
            }
            int len = newRunnigList.Count;
            for (int i = 0; i < len; i++)
            {
                newRunnigList[i].load(0,false,retryCount);
            }
            newRunnigList.Clear();

           
        }

        private void itemProgress(EventX obj)
        {
            
        }

        private void itemComplete(EventX e)
        {
            AssetResource resource =e.target as AssetResource;
            resource.removeEventListener(EventX.COMPLETE, itemComplete);
            resource.removeEventListener(EventX.FAILED, itemComplete);
            //resource.removeEventListener(AssetEvent.PROGRESS, itemProgress);
            int index=runningList.IndexOf(resource);
            if (index!=-1)
            {
                runningList.RemoveAt(index);
            }

            if (isStart == false)
            {
                return;
            }
            running--;

            loaded = total - queue.Count;
            this.dispatchEvent(new EventX(EventX.PROGRESS, (float)loaded /total));

            Action<EventX> action = null;
            if (resultActions.TryGetValue(resource.id, out action))
            {
                action(e);
            }

            CallLater.Add(loadNext);
        }

        

        public Dictionary<string, AssetResource> getMaping()
        {
            return urlMapping;
        } 

        public void recycle()
        {
            isStart = false;

            if (runningList.Count > 0)
            {
                foreach (AssetResource resource in runningList)
                {
                    resource.removeEventListener(EventX.COMPLETE, itemComplete);
                    resource.removeEventListener(EventX.FAILED, itemComplete);
                }
                runningList.Clear();
            }

            urlMapping.Clear();

            running = 0;
            total = 0;
            queue.Clear();
            resultActions.Clear();
            _clear();
        }
    }
}
