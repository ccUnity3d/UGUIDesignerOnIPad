using System;
using System.Collections.Generic;

namespace foundation
{
    public class SecondTimerUtil
    {
        private static Dictionary<Action<int>, int> set = new Dictionary<Action<int>, int>();
        private static List<Action<int>> removes = new List<Action<int>>();

        public static bool add(Action<int> value, int time)
        {
            if (removes.Contains(value))
            {
                removes.Remove(value);
            }

            if (set.ContainsKey(value))
            {
                set[value] = time;
            }
            else
            {
                set.Add(value, time);
                if (set.Count == 1)
                {
                    TickManager.add(tick);
                }
            }
            return true;
        }

        public static bool remove(Action<int> value)
        {
            if (set.ContainsKey(value))
            {
                set.Remove(value);
            }

            if (set.Count == 0)
            {
                pre = DateTime.MinValue;
                TickManager.remove(tick);
            }

            return true;
        }

        private static DateTime pre = DateTime.MinValue;

        public static void tick()
        {
            if (pre == DateTime.MinValue)
            {
                pre = DateTime.Now;
            }
            DateTime cur = DateTime.Now;
            int temp;
            int dis = (int) ((cur.Ticks - pre.Ticks)/10000);
            if (dis > 999)
            {
                pre = cur;

                List<Action<int>> buffer = new List<Action<int>>(set.Keys);
                foreach (Action<int> item in buffer)
                {
                    if (set.TryGetValue(item, out temp) == true)
                    {
                        temp -= dis;
                        if (temp < 0)
                        {
                            removes.Add(item);
                        }
                        else
                        {
                            set[item] = temp;
                        }

                        item.Invoke(temp);
                    }
                }

                foreach (Action<int> item in removes)
                {
                    set.Remove(item);
                }

                removes.Clear();
            }

        }

    }

}
