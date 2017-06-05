using System;
using System.Reflection;

namespace foundation
{
    public class Facade:IFacade
    {
        protected static readonly string SINGLETON_MSG = "Facade Singleton already constructed!";
        protected ASDictionary<string, object> mvcInjectProxyLock;
        protected ASDictionary<string, object> mvcInjectMediatorLock;
        protected IInject injecter;

        protected IModel model;
        protected IView view;

        protected static IFacade ins;
        public static IFacade getInstance()
        {
            if (ins == null) ins = new Facade();
            return ins;
        }

        public Facade()
        {
            if (ins != null)
            {
                throw new Exception(SINGLETON_MSG);
            }
            ins = this;

            injecter = new MVCInject();

            mvcInjectProxyLock = new ASDictionary<string, object>();
            mvcInjectMediatorLock = new ASDictionary<string, object>();

            initializeFacade();
        }

        protected void initializeFacade()
        {
            initializeModel();
            initializeView();
        }

        /// <summary>
        /// 初始化数据代理管理器(没有特殊需求,不建议重写此方法);
        /// </summary>
        protected virtual void initializeModel()
        {
            if (model != null) return;
            model = Model.getInstance();
        }


        /// <summary>
        /// 初始化事件触发器(没有特殊需求,不建议重写此方法); 
        /// </summary>
        protected virtual void initializeView()
        {
            if (view != null) return;
            view = View.getInstance();
        }

        public void registerProxy(IProxy proxy)
        {
            model.registerProxy(proxy);
            view.registerEvent(proxy.eventInterests,true);
        }

        public IProxy getProxy(string proxyName)
        {
            IProxy proxy = model.getProxy(proxyName);
            if (proxy == null)
            {
                Type cls = Singleton.getClass(proxyName);
                if (cls != null)
                {
                    proxy = (IProxy)Activator.CreateInstance(cls);
                    mvcInjectProxyLock[proxyName] = proxy;
                    inject(proxy);
                    registerProxy(proxy);

                    mvcInjectProxyLock.Remove(proxyName);
                }
            }
            return proxy;
        }

        public IProxy removeProxy(string proxyName)
        {
            IProxy proxy = model.removeProxy(proxyName);
            if (proxy != null)
            {
                view.registerEvent(proxy.eventInterests,false);
            }
            return proxy;
        }

        public bool hasProxy(string proxyName)
        {
            return model.hasProxy(proxyName);
        }

        public void registerMediator(IMediator mediator)
        {
            view.registerMediator(mediator);
        }

        public IMediator getMediator(string mediatorName)
        {
            IMediator mediator = view.getMediator(mediatorName) as IMediator;
            if (mediator == null)
            {
                Type cls = Singleton.getClass(mediatorName);
                if (cls != null)
                {
                    mediator = (IMediator)Activator.CreateInstance(cls);
                    mvcInjectMediatorLock[mediatorName] = mediator;

                    inject(mediator);
                    registerMediator(mediator);

                    mvcInjectMediatorLock.Remove(mediatorName);
                }
                else
                {
                    if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.WindowsEditor)
                    {
                        UnityEngine.Debug.Log(mediatorName + "未注册");
                    }

                    DebugX.Log(mediatorName + "未注册");

                }
            }
            return mediator;
        }

        public IMediator removeMediator(string mediatorName)
        {
            return view.removeMediator(mediatorName);
        }

        public bool hasMediator(string mediatorName)
        {
            return view.hasMediator(mediatorName);
        }

        public bool dispatchEvent(EventX e)
        {
            if (!view.hasEventListener(e.type))
            {
                return false;
            }
            return view.dispatchEvent(e);
        }

        public bool hasEventListener(string eventType)
        {
            return view.hasEventListener(eventType);
        }

        public void registerEvent(ASDictionary<string,Action<EventX>> eventInterester,bool isBind=true)
        {
            view.registerEvent(eventInterester);
        }

        public void registerProxyEvent(ASDictionary<string, Action<EventX>> eventInterester,IProxy proxy, bool isBind = true)
        {
            if (eventInterester == null)
            {
                return;
            }

            if (isBind)
            {
                foreach (string eventType in eventInterester)
                {
                    proxy.addEventListener(eventType, eventInterester[eventType]);
                }
            }
            else
            {
                foreach (string eventType in eventInterester)
                {
                    proxy.addEventListener(eventType, eventInterester[eventType]);
                }
            }
        }

        public IMediator toggleMediator(string mediatorName, int type = -1)
        {
            IMediator mediator = getMediator(mediatorName);
            if (mediator == null)
            {
                return null;
            }

            if (mediator is IAsync)
            {
                IAsync async = mediator as IAsync;
                if (async.isReady==false)
                {
                    async.addReayHandle(delegate (EventX msg)
                    {
                        toggleMediator(mediatorName, type);
                    });
                    async.startSync();
                    return mediator;
                }
            }
            IPanel view = mediator.getView();

            switch (type)
            {
                case 1:
                    if (view.isShow == false)
                    {
                        view.show();
                    }
                    else
                    {
                        view.bringTop();
                    }
                    break;
                case 0:
                    if (view.isShow)
                    {
                        view.hide();
                    }
                    break;
                case -1:
                    if (view.isShow)
                    {
                        view.hide();
                    }
                    else
                    {
                        view.show();
                    }
                    break;
            }
            return mediator;
        }

        public IMediator executeMediator(string mediatorName, string eventType, object data = null,int type=1)
        {
            IMediator mediator = getMediator(mediatorName);
            if (mediator == null)
            {
                return null;
            }
            if (mediator is IAsync)
            {
                IAsync async = mediator as IAsync;
                if (!async.isReady)
                {
                    async.addReayHandle(delegate(EventX msg)
                    {
                        executeMediator(mediatorName, eventType, data,type);
                    });
                    async.startSync();
                    return mediator;
                }
            }

            if (type == 1)
            {
                IPanel view = mediator.getView(); //调用这种打开方式直接给予view打开处理，ayy改动
                view.show();
            }

            MethodInfo handle;
            if (ReflectionAccessor.TryGetMethod(mediator, eventType, out handle))
            {
                handle.Invoke(mediator, new object[] { data });
                return mediator;
            }

            mediator.execute(eventType, data);
            return mediator;
        }

        public IProxy executeProxy(string proxyName, string eventType, object data = null)
        {
            IProxy proxy = getProxy(proxyName);
            if (proxy == null) return null;

            if (proxy is IAsync)
            {
                IAsync async = proxy as IAsync;
                if (!async.isReady)
                {
                    async.addReayHandle(delegate(EventX msg)
                    {
                        executeProxy(proxyName, eventType, data);
                    });
                    async.startSync();
                    return proxy;
                }
            }

            MethodInfo handle;
            if (ReflectionAccessor.TryGetMethod(proxy, eventType, out handle))
            {
                handle.Invoke(proxy, new object[] { data });
                return proxy;
            }

            proxy.execute(eventType, data);
            return proxy;
        }

        public virtual void autoInitialize(string currentState)
        {

        }

        private ASList<IMediator> temps = new ASList<IMediator>();
        public virtual void changeState(string currentState)
        {
            ASDictionary<string, IMediator> mediatorMap = getMeditorDic();

            temps.Clear();
            foreach (string name in mediatorMap)
            {
                temps.Add(mediatorMap[name]);
            }
            foreach (IMediator mediator in temps)
            {
                IPanel panel = mediator.getView();
                if (panel == null)
                {
                    continue;
                }


                if (panel.isShow == true)
                {
                    ASList<string> sceneStates = panel.getSceneState();
                    if (sceneStates != null && sceneStates.Count != 0)
                    {
                        if (!sceneStates.Contains(currentState))
                        {
                            mediator.getView().hide();
                            continue;
                        }
                    }
                    panel.changeState(currentState);
                }
            }
        }

        public object inject(object target)
        {
            if (injecter != null)
            {
                return injecter.inject(target);
            }
            return target;
        }

        public object getInjectLock(string className)
        {
            object value;
            if (mvcInjectProxyLock.TryGetValue(className, out value))
            {
                return value;
            }
            if (mvcInjectMediatorLock.TryGetValue(className, out value))
            {
                return value;
            }
            return null;
        }

        public void clear()
        {
            view.clear();

            foreach (string key in model.getAllPorxy())
            {
                removeProxy(key);
            }
        }

        public bool simpleDispatch(string eventType, object data)
        {
            if (!view.hasEventListener(eventType))
            {
                return false;
            }

            //从事件池中取一个项，用于事件发布,发布完后，再压进事件池;
            EventX e = EventX.fromPool(eventType, data);
            Boolean b = view.dispatchEvent(e);
            EventX.toPool(e);

            return b;
        }

        public ASDictionary<string, IMediator> getMeditorDic()
        {
            return view.getMediatorDic();
        }

        public bool addEventListener(string type, Action<EventX> listener, int priority = 0)
        {
            return view.addEventListener(type, listener, priority);
        }

        public bool removeEventListener(string type, Action<EventX> listener)
        {
            return view.removeEventListener(type, listener);
        }

        public static T GetProxy<T>(string name) where T : Proxy
        {
            if (ins == null)
            {
                return null;
            }

            return ins.getProxy(name) as T;
        }
        public static T GetMediator<T>(string name) where T : Mediator
        {
            if (ins == null)
            {
                return null;
            }
            return ins.getMediator(name) as T;
        }

        public static bool AddEventListener(string type, Action<EventX> listener, int priority = 0)
        {
            if (ins == null)
            {
                return false;
            }
            return ins.addEventListener(type, listener, priority);
        }

        public static bool RemoveEventListener(string type, Action<EventX> listener)
        {
            if (ins==null)
            {
                return false;
            }
            return ins.removeEventListener(type, listener);
        }

        public static bool SimpleDispatch(string type, object data = null)
        {
            if (ins == null)
            {
                return false;
            }
            return ins.simpleDispatch(type,data);
        }

        public static bool DispatchEvent(EventX e)
        {
            if (ins == null)
            {
                return false;
            }
            return ins.dispatchEvent(e);
        }
    }
}
