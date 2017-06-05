using foundation;
using System;
using System.Collections.Generic;

namespace foundation
{
    public class View:EventDispatcher,IView
    {
        protected static readonly string SINGLETON_MSG = "View Singleton already constructed!";
        protected static IView instance;

        protected ASDictionary<string, IMediator> mediatorMap ;
        public static IView getInstance()
		{
            if (instance == null) instance = new View();
			return instance;
		}

        private View( )
		{
            if (instance != null)
            {
                throw new Exception(SINGLETON_MSG);
            }
			instance = this;
            mediatorMap = new ASDictionary<string, IMediator>();
		}

        public void registerMediator(IMediator mediator)
        {
            String name = mediator.name;
			if ( mediatorMap.ContainsKey(name) ) {
				throw new Exception("重复定义:"+name);
			}
			mediatorMap[ name ] = mediator;
			if(mediator is ISpecialEventInterests){
				registerEvent((mediator as ISpecialEventInterests).specialEventInterests,true);
			}
			mediator.onRegister();
        }

        public void registerMediatorAlias(string mediatorName, IMediator mediator)
        {
            if(mediator ==null){
                mediatorMap.Remove(mediatorName);
				return;
			}
            mediatorMap[ mediatorName] = mediator;
        }

        public IMediator getMediator(string mediatorName)
        {
            IMediator mediator;
            if (mediatorMap.TryGetValue(mediatorName, out mediator))
            {
                return mediator;
            }
            return null;
        }

        public IMediator removeMediator(string mediatorName)
        {
            IMediator mediator;
            if (mediatorMap.TryGetValue(mediatorName,out mediator)) 
			{
				if(mediator is ISpecialEventInterests){
                    registerEvent((mediator as ISpecialEventInterests).specialEventInterests,false);
				}
                mediatorMap.Remove(mediatorName);
				mediator.onRemove();
                return mediator;
			}
			return null;
        }

        public bool hasMediator(string mediatorName)
        {
            return mediatorMap.ContainsKey(mediatorName);
        }

        public void registerEvent(ASDictionary<string, Action<EventX>> eventInterests, bool isBind=true)
        {
			if(eventInterests==null){
				return;
			}

            if (isBind)
            {
                foreach (string eventType in eventInterests)
                {
                    this.addEventListener(eventType, eventInterests[eventType]);
                }
            }
            else
            {
                foreach (string eventType in eventInterests)
                {
                    this.removeEventListener(eventType, eventInterests[eventType]);
                }
            } 
        }

        public void clear()
        {
            foreach(string key in mediatorMap){
                removeMediator(key);
            }
        }

        public ASDictionary<string, IMediator> getMediatorDic()
        {
            return mediatorMap;
        }
    }
}
