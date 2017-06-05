using System;
using System.Collections.Generic;

namespace foundation
{
    public class SocketRouter
    {
        private Dictionary<int, List<ListenerBox>> eventsMap;
        private Dictionary<int, List<Action<AbstractMessage>>> onceListenerMaps;

        public SocketRouter()
        {
            eventsMap = new Dictionary<int, List<ListenerBox>>();
            onceListenerMaps = new Dictionary<int, List<Action<AbstractMessage>>>();
        }

        public bool addListener(int code, Action<AbstractMessage> handle, int priority = 0)
        {
            List<ListenerBox> list;
            ListenerBox listenerBox;

            if (eventsMap.TryGetValue(code, out list) == false)
            {
                list = new List<ListenerBox>();
                eventsMap.Add(code, list);

                listenerBox = new ListenerBox(handle, priority);

                list.Add(listenerBox);
                return true;
            }

            int i = 0;
            int len = list.Count;

            while (i < len)
            {
                listenerBox = list[i];
                if (listenerBox.listener == handle)
                {
                    if (listenerBox.priority == priority)
                    {
                        return false;
                    }

                    list.RemoveAt(i);
                    len--;
                    break;
                }
                i++;
            }

            listenerBox = new ListenerBox(handle, priority);

            for (i = 0; i < len; i++)
            {
                if (priority > list[i].priority)
                {
                    list.Insert(i, listenerBox);
                    return true;
                }
            }

            list.Add(listenerBox);

            return true;
        }


        internal bool addListenerOnce(int code, Action<AbstractMessage> handle)
        {
            List<Action<AbstractMessage>> list;

            if (onceListenerMaps.TryGetValue(code, out list) == false)
            {
                list = new List<Action<AbstractMessage>>();
                list.Add(handle);
                onceListenerMaps.Add(code, list);
                return true;
            }

            if (list.IndexOf(handle) == -1)
            {
                list.Add(handle);
                return true;
            }

            return false;
        }


        public bool hasListener(int code)
        {
            return eventsMap.ContainsKey(code);
        }

        public bool removeListener(int code, Action<AbstractMessage> handle)
        {
            List<ListenerBox> list;

            if (eventsMap.TryGetValue(code, out list) == false)
            {
                return false;
            }

            ListenerBox listenerBox;
            int len = list.Count;
            int i = 0;

            while (i < len)
            {
                listenerBox = list[i];
                if (listenerBox.listener.Equals(handle))
                {
                    list.RemoveAt(i);
                    break;
                }
                else
                {
                    i++;
                }
            }

            if (list.Count == 0)
            {
                eventsMap.Remove(code);
            }

            return true;
        }


        public bool dispatch(AbstractMessage e)
        {
            bool result = false;
            int code = e.getMessageType();
            List<ListenerBox> list;

            if (eventsMap.TryGetValue(code, out list))
            {
                foreach (ListenerBox listenerBox in list.ToArray())
                {
                    listenerBox.listener(e);
                }
                result = true;
            }

            List<Action<AbstractMessage>> onceList;
            if (onceListenerMaps.TryGetValue(code, out onceList))
            {
                foreach (Action<AbstractMessage> item in onceList.ToArray())
                {
                    item(e);
                }
                onceListenerMaps.Remove(code);
                result = true;
            }
            return result;
        }
    }
}
