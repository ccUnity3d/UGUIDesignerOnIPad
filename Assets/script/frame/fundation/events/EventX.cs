using System.Collections.Generic;

namespace foundation
{
    /// <summary>
    /// 简单事件类,主要附带一个数据项
    /// </summary>
    public class EventX : MiEventX
    {
        static private Stack<EventX> sEventPool = new Stack<EventX>();

        public const string ADDED = "added";
        public const string START = "start";

        public const string READY = "ready";

        public const string MEDIATOR_READY = "mediator_ready";

        public const string EXIT = "exit";
        public const string ENTER = "enter";

        public const string ADDED_TO_STAGE = "addedToStage";
        public const string ENTER_FRAME = "enterFrame";

        public const string REMOVED = "removed";
        public const string REMOVED_FROM_STAGE = "removedFromStage";

        public const string TRIGGERED = "triggered";

        public const string FLATTEN = "flatten";
        public const string RESIZE = "resize";
        public const string COMPLETE = "complete";
        public const string ROOT_CREATED = "rootCreated";
        public const string CHANGE = "change";
        public const string CANCEL = "cancel";
        public const string SCROLL = "scroll";
        public const string OPEN = "open";
        public const string CLOSE = "close";
        public const string SELECT = "select";
        public const string DISPOSE = "dispose";
        public const string DATA = "data";

        public const string ERROR = "error";

        public const string TIMEOUT = "timeout";
        public const string FAILED = "failed";
        public const string PROGRESS = "progress";

        public const string CONNECTION = "connection";
        public const string REQUESTSERVER = "requestserver";
        public const string REMOVE_FROM_JUGGLER = "remove_from_juggler";
        public const string ITEM_CLICK = "itemClick";

        private IEventDispatcher mCurrentTarget;

        private bool mBubbles;
        private bool mStopsPropagation;
        private bool mStopsImmediatePropagation;
       

        public EventX(string type, object data = null, bool bubbles = false)
            : base(type, data)
        {
            mBubbles = bubbles;
        }

        public void stopPropagation()
        {
            mStopsPropagation = true;
        }

        public void stopImmediatePropagation()
        {
            mStopsPropagation = mStopsImmediatePropagation = true;
        }

        public bool bubbles
        {
            get
            {
                return mBubbles;
            }
        }

        public IEventDispatcher currentTarget
        {
            get
            {
                return mCurrentTarget;
            }
        }

        internal void setCurrentTarget(IEventDispatcher value)
        {
            mCurrentTarget = value;
        }

        internal void setData(object value) { mData = value; }

        internal bool stopsPropagation
        {
            get
            {
                return mStopsPropagation;
            }
        }

        internal bool stopsImmediatePropagation
        {
            get
            {
                return mStopsImmediatePropagation;
            }
        }

        internal static EventX fromPool(string type, object data = null, bool bubbles = false)
        {
            EventX e;
            if (sEventPool.Count > 0)
            {
                e = sEventPool.Pop();
                e.reset(type, bubbles, data);
                return e;
            }
            else return new EventX(type, data, bubbles);
        }

        /** @private */
        internal static void toPool(EventX e)
        {
            if (sEventPool.Count < 100)
            {
                e.mData = e.mTarget = e.mCurrentTarget = null;
                sEventPool.Push(e); // avoiding 'push'
            }
        }

        /** @private */
        internal EventX reset(string type, bool bubbles = false, object data = null)
        {
            mType = type;
            mBubbles = bubbles;
            mData = data;
            mTarget = mCurrentTarget = null;
            mStopsPropagation = mStopsImmediatePropagation = false;
            return this;
        }

        public EventX clone()
        {
            return new EventX(type, data, bubbles);
        }

    }
}
