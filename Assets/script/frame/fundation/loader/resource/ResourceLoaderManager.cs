using System.Collections.Generic;

namespace foundation
{
    public delegate RFLoader GETLoaderHandler(string url, string id);

    public class ResourceLoaderManager
    {
        public static IFileVersion fileVersion;
        private static ResourceLoaderManager ins;
        public static ILoaderFactory loaderFactory = null;

        public static ResourceLoaderManager sharedInstance()
        {
            if (ins == null)
            {
                ins = new ResourceLoaderManager();
            }
            return ins;
        }

        /// <summary>
        /// 正在加载中的加载器
        /// </summary>
        private Dictionary<string, RFLoader> _loadingPool;
        private CoreLoaderQueue _coreLoaderQueue;
        private int _loadingCount = 0;

        /// <summary>
        ///  加载数据量 
        /// </summary>
		private uint _loadedMemory = 0;
		
		public uint loadedMemory
        {
            get
            {
                return _loadedMemory;
            }
        }

        public ResourceLoaderManager()
        {
            _loadingPool = new Dictionary<string, RFLoader>();
            _coreLoaderQueue = new CoreLoaderQueue();
        }

        public RFLoader getLoader(string url, string id)
        {
            RFLoader loader=null;
            if (_loadingPool.TryGetValue(id, out loader)==false)
            {
                string version = "";
                if (fileVersion != null) {
                    version = fileVersion.urlversion(url);
                }

                if (loaderFactory != null) {
                    loader = loaderFactory.getLoader(url, id);
                } else {
                    loader = new AssetLoader(url, id);
                }
                loader.hashVersion = version;

                loader.addEventListener(EventX.COMPLETE, onLoaderComplete);
                loader.addEventListener(EventX.FAILED, onLoaderComplete);

                _loadingCount++;

                _loadingPool[id] = loader;
            }
            return loader;
        }

        public void onLoaderComplete(EventX e)
        {
            // 加载记录
            RFLoader loader = e.target as RFLoader;
            loader.removeEventListener(EventX.COMPLETE, onLoaderComplete);
            loader.removeEventListener(EventX.FAILED, onLoaderComplete);

            _loadingPool.Remove(loader.id);

            _loadingCount--;
        }

		internal void queue(RFLoader loader, int priority= 0)
		{
			_coreLoaderQueue.add(loader,priority);
			_coreLoaderQueue.next();
		}

        internal void dispose(RFLoader loader) {
            string id = loader.id;

            if (_loadingPool.ContainsKey(id))
            {
                _loadingPool.Remove(id);
                _loadingCount--;
            }

            loader.removeEventListener(EventX.COMPLETE, onLoaderComplete);
            loader.removeEventListener(EventX.FAILED, onLoaderComplete);

            _coreLoaderQueue.remove(loader);
        }
		
		internal bool unQueue(RFLoader loader)
		{
			return _coreLoaderQueue.remove(loader);
		}

    }
}
