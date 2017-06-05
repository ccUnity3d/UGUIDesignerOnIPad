using System;
using System.Collections.Generic;
//using foundationSerialization;

namespace foundation
{
    public class AssetsManager
    {
        /// <summary>
        /// 资源列表
        /// </summary>
        private Dictionary<string, AssetResource> _resourceMap = new Dictionary<string, AssetResource>();

        private static Dictionary<LoaderXDataType, Type> resourceTypeMapping = new Dictionary<LoaderXDataType, Type>();

        private ResourceLoaderManager _loaderManager ;
		public ResourceLoaderManager loaderManager {
            get{
                return _loaderManager;
            }
        }
        public AssetsManager()
        {
            if (_instance != null)
            {
                throw new Exception();
            }
            _loaderManager = ResourceLoaderManager.sharedInstance();


            regist<PrefabAssetResource>(LoaderXDataType.PREFAB);
            regist<PrefabAssetResource>(LoaderXDataType.SPRITE);
            regist<InnerAssetResource>(LoaderXDataType.RESOURCE);
        }

		static private AssetsManager _instance;
		static public AssetsManager instance()
		{
			if(_instance==null){
				_instance = new AssetsManager();
			}
			return _instance;
		}

        public static void regist<T>(LoaderXDataType type) where T:AssetResource
        {
            Type clz = typeof (T);
            if (resourceTypeMapping.ContainsKey(type))
            {
                resourceTypeMapping.Remove(type);
            }

            resourceTypeMapping.Add(type, clz);
        }

        public static void bindEventHandle(IEventDispatcher resource, Action<EventX> x, bool isBind= true){
			if(isBind){
				resource.addEventListener(EventX.COMPLETE,x);
				resource.addEventListener(EventX.FAILED,x);
			}else{
				resource.removeEventListener(EventX.COMPLETE,x);
				resource.removeEventListener(EventX.FAILED,x);
			}
		}
		
		public AssetResource findResource(string uri)
		{
            if (uri == null)
            {
                return null;
            }
            AssetResource res=null;
		    string key = uri.ToLower();
            if(_resourceMap.TryGetValue(key,out res))
            {
                return res;
            }

            return null;
		}
		
		private AssetResource _getResource(string url, string uri= null, LoaderXDataType autoCreateType= LoaderXDataType.BYTES){
			if(uri==null){
				uri=url;
			}

            AssetResource res =findResource(uri);
			if(res==null){
                Type cls = null;

			    if (resourceTypeMapping.TryGetValue(autoCreateType, out cls) == false)
			    {
			        res = new AssetResource(url, uri);
			    }
			    else
			    {
			        res = (AssetResource) Activator.CreateInstance(cls, url, uri);
			    }

			    res.parserType = autoCreateType;
                res.addEventListener(EventX.DISPOSE,resourceDisposeHandle);

			    string key = uri.ToLower();
				_resourceMap[key]=res;
			}
			return res;
		}
		
		public static AssetResource getResource(string url, string uri= null, LoaderXDataType autoCreateType = LoaderXDataType.BYTES)
        {
			return instance()._getResource(url, uri, autoCreateType);
		}
		
		public void _dispose(string uri)
		{
            AssetResource res = null;
            if(_resourceMap.TryGetValue(uri, out res)) {
                _resourceMap.Remove(uri);

                res.removeEventListener(EventX.DISPOSE,resourceDisposeHandle);
				res.__dispose();

                //trace("dispose",uri);
            }
            else{
                //RFTraceWarn(uri+" 不在AssetsManager里!");
			}
			
		}

        public static void dispose(string uri)
        {
            instance()._dispose(uri);
        }

        private void resourceDisposeHandle(EventX e)
        {
            AssetResource res = e.target as AssetResource;
            res.removeEventListener(EventX.DISPOSE, resourceDisposeHandle);

            string uri = res.id;
            if (_resourceMap.ContainsKey(uri))
            {
                _resourceMap.Remove(uri);
            }
        }

		public void cleanAll(bool isDispose=false){
            foreach (AssetResource res in _resourceMap.Values)
            {
                res.removeEventListener(EventX.DISPOSE, resourceDisposeHandle);
                if (isDispose)
                {
                    res.__dispose();
                }
                else
                {
                    res.stop();
                }
            }
            _resourceMap.Clear();
        }
    }
}
