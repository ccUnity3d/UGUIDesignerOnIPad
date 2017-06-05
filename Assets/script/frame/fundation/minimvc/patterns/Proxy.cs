using System;
namespace foundation
{
    public class Proxy :Notifier, IProxy
    {
        protected EventDispatcher eventDispatcher;

        protected string _name;
        protected object _data;
        protected ASDictionary<string,Action<EventX>> _eventInterests;

        public Proxy(string proxyName)
        {
            _name = proxyName;

            eventDispatcher = new EventDispatcher(this);

            _eventInterests=new ASDictionary<string, Action<EventX>>();
        }

        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public virtual void execute(string eventType, object data = null)
        {
           
        }

        public virtual void onRegister()
        {
            
        }

        public virtual void onRemove()
        {
        }

        public ASDictionary<string,Action<EventX>> eventInterests
        {
            get { return _eventInterests; }
        }

        public bool addEventListener(string eventType, Action<EventX> handle, int priority = 0)
        {
            return eventDispatcher.addEventListener(eventType, handle, priority);
        }

        public bool removeEventListener(string eventType, Action<EventX> handle)
        {
            return eventDispatcher.removeEventListener(eventType, handle);
        }

        public bool hasEventListener(string eventType)
        {
            return eventDispatcher.hasEventListener(eventType);
        }

        public bool dispatchEvent(EventX e)
        {
            return eventDispatcher.dispatchEvent(e);
        }
    }
}
