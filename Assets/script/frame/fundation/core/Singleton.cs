using System;
using System.Collections.Generic;
using System.Reflection;

namespace foundation
{
    /// <summary>
    ///  类的管理工具,可注册类的别名
    ///  供MVC框架使用,功能强大
    /// </summary>
    public class Singleton
    {
        private static Dictionary<string, Type> classMap = new Dictionary<string, Type>();
        private static Dictionary<string, Type> uniqueMap = new Dictionary<string, Type>();

        private static Dictionary<string, Object> uniqueInstanceMap = new Dictionary<string, object>();

        private static Dictionary<string, string> __aliasMap = new Dictionary<string, string>();

        /**
         * 注册接口 实现类; 
         * @param interfaceName 接口名称
         * @param clazz 实现类
         * @return 是否注册成功;
         * 
         */
        public static void registerClass<T>(string interfaceName) where T : new()
        {
            Type clazz = typeof(T);
            string fullClassName = clazz.FullName;

            __aliasMap[fullClassName] = interfaceName;
            classMap[interfaceName] = clazz;

        }

        public static void registerClass(Type type, string interfaceName)
        {
            string fullClassName = type.FullName;
            __aliasMap[fullClassName] = interfaceName;
            classMap[interfaceName] = type;
        }

        public static void unregisterClass(string interfaceName)
        {
            if (classMap.ContainsKey(interfaceName))
            {
                classMap.Remove(interfaceName);
            }
        }


        /// <summary>
        /// 注册一个实例与名称对应的关系;
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <param name="instance"></param>
        public static void registerSingleton(string interfaceName, object instance)
        {
            if (instance == null)
            {
                uniqueInstanceMap.Remove(interfaceName);
                return;
            }
            Type clazz = instance.GetType();
            string fullClassName = clazz.FullName;
            classMap[interfaceName] = clazz;

            uniqueMap[interfaceName] = clazz;
            uniqueMap[fullClassName] = clazz;

            __aliasMap[fullClassName] = interfaceName;
            __aliasMap[interfaceName] = interfaceName;
            uniqueInstanceMap[interfaceName] = instance;
        }

        public static void registerSingleton<T>(string interfaceName) where T : new()
        {
            Type clazz = typeof(T);

            string fullClassName = clazz.FullName;
            classMap[interfaceName] = clazz;

            __aliasMap[fullClassName] = interfaceName;
            __aliasMap[interfaceName] = interfaceName;

            uniqueMap[interfaceName] = clazz;
            uniqueMap[fullClassName] = clazz;
        }


        /// <summary>
        /// 取得别名;
        /// </summary>
        /// <returns>
        /// 别名,null为没有注册过别名
        /// </returns>
        /// <param name='fullName'>
        /// Full name.
        /// </param>
        internal static string getAliasName(string fullName)
        {
            string aliasName;

            if (__aliasMap.TryGetValue(fullName, out aliasName) == true)
            {
                return aliasName;
            }

            return null;
        }


        /**
         * 取得一个实现接口的实例
         * @param interfaceName
         * @return 
         * 
         */
        internal static object getOneInstance(string interfaceName)
        {
            object target = null;
            if (uniqueInstanceMap.TryGetValue(interfaceName, out target))
            {
                return target;
            }

            Type c;
            if (uniqueMap.TryGetValue(interfaceName, out c) == true)
            {
                object ins = Activator.CreateInstance(c);
                uniqueInstanceMap[interfaceName] = ins;
                uniqueInstanceMap[c.FullName] = ins;
                return ins;
            }

            c = getClass(interfaceName);

            if (c == null)
            {
                return null;
            }

            return Activator.CreateInstance(c);
        }

        /**
         * 取得一个实现接口的类 
         * @param interfaceName
         * @return 
         * 
         */
        public static Type getClass(string interfaceName)
        {
            Type c;
            if (classMap.TryGetValue(interfaceName, out c) == false)
            {
                c = Type.GetType(interfaceName);
            }
            return c;
        }


        public static bool isInUnique(string interfaceName)
        {
            if (uniqueInstanceMap.ContainsKey(interfaceName))
            {
                return true;
            }

            return uniqueMap.ContainsKey(interfaceName);
        }

        /**
         * 取得实现接口的单例,必须含有 getInstance的静态方法;
         * @param interfaceName 接口名称;
         * @return 返回实现的单例
         * 
         */
        public static object getInstance(string interfaceName)
        {
            object ins;
            if (uniqueInstanceMap.TryGetValue(interfaceName, out ins) == false)
            {
                Type c;
                if (uniqueMap.TryGetValue(interfaceName, out c) == false)
                {
                    c = getClass(interfaceName);
                }

                if (c == null)
                {
                    return null;
                }
                uniqueInstanceMap[interfaceName] = ins = Activator.CreateInstance(c);
            }
            return ins;
        }


        public static T getInstance<T>() where T : new()
        {
            Type clazz = typeof(T);
            string fullName = clazz.FullName;

            T instance = (T)Singleton.getInstance(fullName);

            if (instance == null)
            {
                instance = (T)Activator.CreateInstance(clazz);
                uniqueInstanceMap[fullName] = instance;
                uniqueInstanceMap[clazz.FullName] = instance;
            }
            return instance;
        }


    }
}
