using foundation;
using System;

namespace foundation
{
    public class Notifier : INotifier
    {
        protected IFacade facade = Facade.getInstance();
        public Notifier()
        {
        }

        public bool simpleDispatch(string eventType, object data)
        {
            return facade.simpleDispatch(eventType, data);
        }
    }
}
