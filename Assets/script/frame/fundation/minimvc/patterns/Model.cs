using System;
namespace foundation
{
    public class Model:IModel
    {
        protected static readonly string SINGLETON_MSG = "Model Singleton already constructed!";
        protected ASDictionary<string,IProxy> proxyMap;

        protected static IModel instance;
        public static IModel getInstance()
        {
            if (instance == null) instance = new Model();
            return instance;
        }

        public Model()
		{
            if (instance != null)
            {
                throw new Exception(SINGLETON_MSG);
            }
			instance = this;
            proxyMap = new ASDictionary<string, IProxy>();
		}

        public void registerProxy(IProxy proxy)
        {
            String proxyName = proxy.name;
			if(proxyMap.ContainsKey(proxyName)){
				throw new Exception("重复定义:"+proxyName);
			}
			proxyMap[ proxyName ] = proxy;
			proxy.onRegister();
        }

        public void registerProxyAlias(string proxyName, IProxy proxy)
        {
           if(proxy==null){
               proxyMap.Remove(proxyName);
				return;
			}
			proxyMap[ proxyName ] = proxy;
        }

        public IProxy getProxy(string proxyName)
        {
            IProxy proxy;
            if (proxyMap.TryGetValue(proxyName, out proxy))
            {
                return proxy;
            }
            return null;
        }

        public IProxy removeProxy(string proxyName)
        {
            if (proxyMap.ContainsKey(proxyName))
            {
                IProxy proxy = proxyMap[proxyName];
                proxyMap.Remove(proxyName);
                proxy.onRemove();
                return proxy;
            }
            return null;
        }

        public ASDictionary<string, IProxy> getAllPorxy()
        {
            return proxyMap;
        }

        public bool hasProxy(string proxyName)
        {
            return proxyMap.ContainsKey(proxyName);
        }
    }
}
