using System;
using UnityEngine;

namespace foundation
{


    public class AssetResource : EventDispatcher, IAutoReleaseRef
    {
        protected AssetState _status = AssetState.NONE;      // 资源状态
        internal LoaderXDataType parserType;
        /// <summary>
        /// 用于定义数据(供临时使用); 
        /// </summary>
        public object userData;

        protected object _data;

        protected string _id;
        protected string _url;

        public bool destroyOnClear = false;

        protected RFLoader loader;

        public bool isLoaded
        {
            get
            {
                return (_status == AssetState.READY || _status == AssetState.FAILD);
            }
        }

        public bool isReady
        {
            get
            {
                return _status == AssetState.READY;
            }
        }

        public AssetResource(string url, string uri = null)
        {
            _url = url;

            if (uri == null)
            {
                uri = url;
            }
            _id = uri;
        }

        public string url
        {
            get
            {
                return _url;
            }
        }

        public string id
        {
            get
            {
                return _id;
            }
        }

        public AssetState status
        {
            get
            {
                return _status;
            }
        }


        public void load(int priority = 0, bool progress = false, uint retryCount = 0)
        {
            if (isLoaded)
            {
                if (_status == AssetState.FAILD)
                {
                    //重新加载;
                    _status = AssetState.LOADING;
                    _loadImp(priority, progress, retryCount);       // 开始加载
                    return;
                }

                this.dispatchEvent(new EventX(EventX.COMPLETE, _data));
                return;
            }

            if (_status != AssetState.NONE && _status != AssetState.FAILD)
            {
                return;
            }

            if (string.IsNullOrEmpty(url))
            {
                _status = AssetState.FAILD;
                this.dispatchEvent(new EventX(EventX.FAILED, "url is empty"));
                return;
            }

            _status = AssetState.LOADING;

            _loadImp(priority, progress, retryCount);       // 开始加载
        }



        protected virtual void _loadImp(int priority = 0, bool progress = false, uint retryCount = 0)
        {
            ResourceLoaderManager loaderManager = ResourceLoaderManager.sharedInstance();
            RFLoader newLoader = loaderManager.getLoader(url, id);
            if (newLoader != loader && loader != null)
            {
                loader.removeEventListener(EventX.COMPLETE, loadComplete);
                loader.removeEventListener(EventX.FAILED, failedLoad);
                if (progress)
                {
                    loader.removeEventListener(EventX.PROGRESS, progressHandle);
                }
            }
            loader = newLoader;
            if (retryCount > 0)
            {
                loader.autoRetry = true;
                loader.autoTryLimit = retryCount;
            }
            loader.checkProgress = progress;
            loader.addEventListener(EventX.COMPLETE, loadComplete);
            loader.addEventListener(EventX.FAILED, failedLoad);
            if (progress)
            {
                loader.addEventListener(EventX.PROGRESS, progressHandle);
            }
            loaderManager.queue(loader, priority);
        }

        protected void progressHandle(EventX e)
        {
            this.dispatchEvent(e);
        }

        /// <summary>
        /// 加载资源失败; 
        /// </summary>
        /// <param name="ev"></param>
        protected void failedLoad(EventX ev)
        {
            IEventDispatcher e = ev.target as IEventDispatcher;
            e.removeEventListener(EventX.COMPLETE, loadComplete);
            e.removeEventListener(EventX.PROGRESS, progressHandle);
            e.removeEventListener(EventX.FAILED, failedLoad);

            _data = null;

            resourceComplete(ev.type);
        }

        /**
         *  加载资源成功 
         * @param event
         * 
         */

        protected virtual void loadComplete(EventX e)
        {
            loader = e.target as RFLoader;
            loader.removeEventListener(EventX.COMPLETE, loadComplete);
            loader.removeEventListener(EventX.PROGRESS, progressHandle);
            loader.removeEventListener(EventX.FAILED, failedLoad);
            bool b = true;
            string eventType = e.type;

            _data = loader.parser(parserType);
            if (_data != null)
            {
                b = parser(_data);
            }

            if (_data == null)
            {
                _data = "_data is null";
                eventType = EventX.FAILED;
            }

            if (b)
            {
                resourceComplete(eventType);
            }
        }

        protected virtual bool parser(object rawData)
        {
            return true;
        }

        protected void resourceComplete(string eventType)
        {
            if (eventType == EventX.COMPLETE)
            {
                _status = AssetState.READY;
            }
            else
            {
                _status = AssetState.FAILD;
            }

            this.dispatchEvent(new EventX(eventType, _data));
        }

        /// <summary>
        /// 停止加载过程(其它不变); 
        /// </summary>
        public void stop()
        {
            if (loader != null)
            {
                ResourceLoaderManager.sharedInstance().unQueue(loader);
                loader.cancel();
            }
            if (_status != AssetState.READY)
            {
                _status = AssetState.FAILD;
            }
        }

        public void __dispose()
        {
            if (loader != null)
            {
                loader.removeEventListener(EventX.COMPLETE, loadComplete);
                loader.removeEventListener(EventX.FAILED, failedLoad);
                ResourceLoaderManager.sharedInstance().dispose(loader);
                loader.dispose();
                loader = null;

            }

            if (_data != null)
            {
                //todo data;
            }

            _status = AssetState.NONE;

            this.simpleDispatch(EventX.DISPOSE);
            _clear();
            _disposed = true;
        }

        private bool _disposed = false;
        public bool isDispose
        {
            get { return _disposed; }
        }

        private int _reference = 0;


        /// <summary>
        /// 加入资源引用计数; 
        /// </summary>
        /// <returns></returns>
        public int release()
        {
            if (--_reference < 1)
            {
                _reference = 0;

                //当前状态在加载,并且 还没有推送到浏览器;
                if (_status == AssetState.LOADING)
                {
                    if (ResourceLoaderManager.sharedInstance().unQueue(loader))
                    {
                        _status = AssetState.NONE;
                    }
                    else if (loader != null)
                    {
                        loader.cancel(true);
                        _status = AssetState.FAILD;
                    }
                }

                AutoReleasePool.add(this);
            }
            return _reference;
        }

        public int retain()
        {
            if (++_reference == 1)
            {
                AutoReleasePool.remove(this);
            }
            return _reference;
        }
        public int retainCount
        {
            get
            {
                return _reference;
            }
        }

        public object data
        {
            get { return _data; }
        }

        public void resetData(object o)
        {
            _data = o;
        }

        public object getNewInstance()
        {
            object result = null;
            try
            {
                result = GameObject.Instantiate(_data as UnityEngine.Object) as object;
            }
            catch (Exception e)
            {
                DebugX.LogError("getNewInstance Error:{0}",e.Message);
            }

            return result;
        }

        public Sprite getSpriteByName(string name)
        {
			object[] list = (object[]) _data;
            for (int i = 0; i < list.Length; i++)
            {
                Sprite item = list[i] as Sprite;
                if (item.name == name)
                {
                    return item;
					//break;
                }
            }
            return null;
        }
    }
}
