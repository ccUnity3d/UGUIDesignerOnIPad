using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace foundation
{
    /// <summary>
    /// 像as一样操作的dictionary(默认实现)
    /// </summary>
    public class ASDictionary : ASDictionary<string,object>
    {
    }
    public class ASDictionary<V> : ASDictionary<string, V>
    {
    }
    /// <summary>
    ///  像as一样操作的dictionary
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class ASDictionary<K,V>: IEnumerable<K>
    {
        private Dictionary<K,V> dic=new Dictionary<K, V>();
        public bool ContainsKey(K value)
        {
            return dic.ContainsKey(value);
        }

        public bool ContainsValue(V value)
        {
            return dic.ContainsValue(value);
        }

        public bool Remove(K key)
        {
            if (dic.ContainsKey(key))
            {
                return dic.Remove(key);
            }
            return false;
        }

        public void Add(K key, V value)
        {
            if (dic.ContainsKey(key))
            {
                dic.Remove(key);
            }
            dic.Add(key, value);
        }

        /// <summary>
        /// 个数;
        /// </summary>
        public int Count
        {
            get { return dic.Count; }
        }

        public bool TryGetValue(K key, out V value)
        {
            return dic.TryGetValue(key, out value);
        }

        public V this[K key]
        {
            get
            {
                if (null == key)
                {
                    return default(V);
                }

                V data;
                dic.TryGetValue(key, out data);

                return data;
            }
            set
            {
                if (null == key)
                {
                    return;
                }

                if (dic.ContainsKey(key))
                {
                    dic.Remove(key);
                }


                dic.Add(key, value);

            }
        }

        public void Clear()
        {
            dic.Clear();
        }

        public IEnumerator<K> GetEnumerator()
        {
            return dic.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dic.Keys.GetEnumerator();
        }
    }
}