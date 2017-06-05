using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class AutoReleasePool
    {
        public static int TIMEOUT = 30000;
        public static bool enabled = true;

        private static AutoReleasePool instance = new AutoReleasePool();

        private Dictionary<IAutoReleaseRef, long> pool;

        private bool isRunning = false;


        public AutoReleasePool()
        {
            pool = new Dictionary<IAutoReleaseRef, long>();
        }

        public static void add(IAutoReleaseRef value)
        {
            if (!enabled) {
                value.__dispose();
                return;
            }
            instance._add(value);
        }

        public static void remove(IAutoReleaseRef value)
        {
            if (!enabled) {
                return;
            }
            instance._remove(value);
        }


        public static void forceAll() {
            if (instance == null) {
                instance = new AutoReleasePool();
            }
            instance._forceAll();
        }

        private void _forceAll()
        {
            foreach(IAutoReleaseRef res in pool.Keys)
            {
                res.__dispose();
            }
            pool.Clear();

            isRunning = false;
            TickManager.remove(tick);
        }

        private void _add(IAutoReleaseRef value)
        {
            if (pool.ContainsKey(value))
            {
                return;
            }
            pool.Add(value, DateTime.Now.Ticks);

            if (isRunning == false) {
                isRunning = true;
                TickManager.add(tick);
            }
        }


        private void _remove(IAutoReleaseRef value)
        {
            if (pool.ContainsKey(value) == false)
            {
                return;
            }
            pool.Remove(value);
        }


        private float timeCount=0;
        private void tick() {

            if ((timeCount += Time.deltaTime) < 3000) {
                return;
            }

            timeCount = 0;
            long now = DateTime.Now.Ticks;
            int total = 0;

            List<IAutoReleaseRef> clearList = new List<IAutoReleaseRef>();
            foreach (IAutoReleaseRef res in pool.Keys)
            {
                total++;
                if (now - pool[res] > TIMEOUT) {
                    clearList.Add(res);
                }
            }


            int len = clearList.Count;
            if (len>0) {
                foreach(IAutoReleaseRef res in clearList)
                {
                    res.__dispose();
                    pool.Remove(res);
                }
            }

            if (total == len) {
                isRunning = false;
                TickManager.remove(tick);
            }
        }
    }
}
