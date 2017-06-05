using System;
using System.Reflection;
using UnityEngine;

namespace foundation
{
    public class MVCInject : IInject
    {
        public static ISocket socket;
        public object inject(object injectable)
        {
            Type contract = injectable.GetType();

            FieldInfo[] fields = contract.GetFields();

            Type m = typeof (MVCAttribute);
            IMediator mediator = injectable as IMediator;

            foreach (FieldInfo info in fields)
            {
                object[] attrs = info.GetCustomAttributes(m, true);
                foreach (object attr in attrs)
                {
                    if (attr is MVCAttribute == false) continue;
                    object valueObj = autoMVC(info.FieldType);
                    if (valueObj == null) continue;

                    info.SetValue(injectable, valueObj);
                    if (mediator == null) continue;

                    if (info.Name == "view")
                    {
                        mediator.setView(valueObj as IPanel);
                    }
                    else if (info.Name == "model")
                    {
                        mediator.setModel(valueObj as IProxy);
                    }
                }
            }

            if (socket == null)
            {
                return injectable;
            }

            MethodInfo[] methods = contract.GetMethods();
            CMDAttribute cmdAttr;
            int code;

            m = typeof (CMDAttribute);
            foreach (MethodInfo info in methods)
            {
                object[] attrs = info.GetCustomAttributes(m, false);
                foreach (object attr in attrs)
                {
                    cmdAttr = attr as CMDAttribute;
                    if (cmdAttr == null) continue;

                    code = cmdAttr.code;
                    if (code < 1)
                    {
                        code = int.Parse(info.Name.Split('_')[1]);
                    }

                    Type type = typeof (Action<AbstractMessage>);
                    Action<AbstractMessage> action =
                        (Action<AbstractMessage>) Delegate.CreateDelegate(type, injectable, info);

                    socket.addListener(code, action);
                    break;
                }
            }

            return injectable;
        }

        protected object autoMVC(Type type)
        {
            string fullName = type.FullName;
            string beanID = Singleton.getAliasName(fullName);
            object source;

            if (beanID == null)
            {
                if (type.IsSubclassOf(typeof (Component)))
                {
                    Debug.Log(fullName);
                    return null;
                }

                if (Singleton.isInUnique(fullName) == true)
                {
                    return Singleton.getInstance(fullName);
                }

                if (type.IsClass)
                {
                    //Debug.Log(fullName);
                    return Activator.CreateInstance(type);
                }
                else
                {
                    throw new Exception("请为非类类型指定别名:" + fullName);
                }
            }

            IFacade facade = Facade.getInstance();

            if (facade.hasMediator(beanID))
            {
                return facade.getMediator(beanID);
            }
            if (facade.hasProxy(beanID))
            {
                return facade.getProxy(beanID);
            }

            source = facade.getInjectLock(beanID);
            if (source == null)
            {
                source = Singleton.getOneInstance(beanID);
                if (source is IInjectable)
                {
                    source = inject(source);
                }
                if (source is IMediator)
                {
                    facade.registerMediator(source as IMediator);
                }
                else if (source is IProxy)
                {
                    facade.registerProxy(source as IProxy);
                }
            }
            return source;
        }
    }
}
