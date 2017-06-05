using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace foundation
{
    public class ObjectUtils
    {
        public static object getValue(object from, string property)
        {
            if (from is ASObject)
            {
                object value;
                (from as ASObject).TryGetValue(property, out value);
                return value;
            }

            Type fromType = from.GetType();
            FieldInfo fieldInfo = fromType.GetField(property, BindingFlags.Instance | BindingFlags.Public);

            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(from);
            }

            return null;
        }

        public static string getStringValue(object from, string property)
        {
            object value = getValue(from, property);

            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }

        public static int getIntValue(object from, string property)
        {
            object value = getValue(from, property);

            if (value == null)
            {
                return 0;
            }

            return int.Parse(value.ToString());
        }

        public static float getFloatValue(object from, string property)
        {
            object value = getValue(from, property);

            if (value == null)
            {
                return 0;
            }

            return float.Parse(value.ToString());
        }

        public static void copyFrom(object obj, object from)
        {
            Type toType = obj.GetType();

            FieldInfo toInfo;
            object value = null;
            if (from is ASObject)
            {
                ASObject fromData = (from as ASObject);
                string[] keys = fromData.Keys.ToArray();

                foreach (string name in keys)
                {
                    toInfo = toType.GetField(name);
                    if (toInfo != null)
                    {
                        fromData.TryGetValue(name, out value);
                        setValue(toInfo, name, obj, value);
                    }
                }

                return;
            }

            Type fromType = from.GetType();
            FieldInfo[] infos = fromType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo info in infos)
            {
                toInfo = toType.GetField(info.Name);
                if (toInfo != null)
                {
                    value = info.GetValue(from);

                    setValue(toInfo, info.Name, obj, value);
                }
            }

        }


        private static void setValue(FieldInfo toInfo, string name, object obj, object value)
        {
            try
            {
                toInfo.SetValue(obj, value);
            }
            catch (Exception e)
            {
                Debug.Log(obj.ToString() + ":" + name + " 类型转换失败,需要为:" + toInfo.FieldType + " value:" + value + "\n" +
                          e.Message);
            }

        }
    }
}
