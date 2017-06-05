using System;
using System.Collections.Generic;

namespace foundation
{
    public class EventDispatcher : IEventDispatcher
    {
        internal Dictionary<string, Signal> mEventListeners = null;
        internal IEventDispatcher mTarget;

        /** Creates an EventDispatcher. */
        public EventDispatcher(IEventDispatcher target = null)
        {
            if (target == null)
            {
                target = this;
            }
            this.mTarget = target;
        }


        public bool addEventListener(string type, Action<EventX> listener, int priority = 0)
        {
            if (mEventListeners == null)
            {
                mEventListeners = new Dictionary<string, Signal>();
            }
            Signal signal;
            if (mEventListeners.TryGetValue(type, out signal) == false)
            {
                signal = new Signal();
                mEventListeners.Add(type, signal);
            }

            return signal.add(listener, priority);
        }

        public bool dispatchEvent(EventX e)
        {
            bool bubbles = e.bubbles;

            if (!bubbles && (mEventListeners == null || mEventListeners.ContainsKey(e.type) == false)) {
                return false;
            }
 
            IEventDispatcher previousTarget = e.target;
            e.setTarget(mTarget);

            bool b = invokeEvent(e);

            if (previousTarget != null) e.setTarget(previousTarget);

            return b;
        }

        protected void innerDirectDispatchEvent(EventX e)
        {
            dispatchEvent(e);
        }

        internal bool invokeEvent(EventX e)
        {
            if (mEventListeners == null)
            {
                return false;
            }

            Signal signal;

            if (mEventListeners.TryGetValue(e.type, out signal) == false)
            {
                return false;
            }

            SignalNode t = signal.firstNode;
            if (t == null) {
                return false;
            }

            List<Action<EventX>> temp = new List<Action<EventX>>();

            int i = 0;
            while (t != null) {
                temp.Add(t.data);
                t = t.next;
                i++;
            }

            e.setCurrentTarget(mTarget);


            Action<EventX> listener;
            for (int j = 0; j < i; j++) {

                listener = temp[j];

                listener(e);

                if (e.stopsImmediatePropagation) {

                    return true;
                }
            }

            return e.stopsPropagation;
        }

        public bool hasEventListener(string type)
        {
            if (mEventListeners == null)
            {
                return false;
            }

            Signal signal;
            if (mEventListeners.TryGetValue(type, out signal) == true)
            {
                return signal != null && signal.firstNode != null;
            }
            return false;
        }

        public bool removeEventListener(string type, Action<EventX> listener)
        {

            if (mEventListeners != null)
            {
                Signal signal=null;
                if (mEventListeners.TryGetValue(type, out signal) == false)
                {
                    return false;
                }
                signal.remove(listener);
            }
            return true;
        }

        public void removeEventListeners(string type= null)
		{
            if (type != null && mEventListeners != null) {
                mEventListeners.Remove(type);
            } else {
                mEventListeners = null;
            }
		}

        public virtual void dispose()
        {
            _clear();
        }

        public bool simpleDispatch(string type, object data = null)
        {
            if (hasEventListener(type) == false)
            {
                return false;
            }
            EventX e = EventX.fromPool(type, data, false);
            bool b = dispatchEvent(e);
            EventX.toPool(e);
            return b;
        }


        internal void _clear()
        {
            if (mEventListeners == null)
            {
                return;
            }

            foreach(Signal signal in mEventListeners.Values)
            {
                signal._clear();
            }

            mEventListeners = null;
        }
    }
}
