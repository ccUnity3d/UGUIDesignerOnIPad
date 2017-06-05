using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class CallLater
    {
        private static CallLater instance = new CallLater();

        private List<Action> handlerList;
        private List<float> timeList;

        public CallLater()
        {
            handlerList = new List<Action>();
            timeList = new List<float>();
        }

        public static void Add(Action handler,float deleTime=0)
        {
            instance.add(deleTime, handler);
        }

        public static void Add(float deleTime, Action handler)
        {
            instance.add(deleTime, handler);
        }

        public static void Remove(Action handler)
        {
            instance.remove(handler);
        }
        private void remove(Action handler)
        {
            int index = handlerList.IndexOf(handler);
            if (index != -1)
            {
                handlerList.RemoveAt(index);
                timeList.RemoveAt(index);
            }
        }


        private void add(float deletime, Action handler)
        {
            if (handlerList.IndexOf(handler) != -1)
            {
                return;
            }
            handlerList.Add(handler);
            timeList.Add(Time.time + deletime);
            if (handlerList.Count == 1)
            {
                TickManager.add(render);
            }
        }

        private List<Action> tempHandle=new List<Action>();
        private void render()
        {
            int len = handlerList.Count;
            tempHandle.Clear();

            float currentTime = Time.time;
            for (int i = len-1; i >-1; i--)
            {
                if (currentTime > timeList[i])
                {
                    tempHandle.Add(handlerList[i]);
                    handlerList.RemoveAt(i);
                    timeList.RemoveAt(i);
                }
            }

            len = tempHandle.Count;
            for (int i = len-1; i >-1; i--)
            {
                tempHandle[i]();
            }

            if (handlerList.Count == 0)
            {
                TickManager.remove(render);
            }
        }
    }
}
