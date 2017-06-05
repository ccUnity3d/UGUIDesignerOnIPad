using UnityEngine;
using System.Collections;
using System;

public class UIController<T> : Singleton<T>, IController where T : IInstance, new()
{
    public IUIData data;
    public IPage panel;
    public IPage getPanel {get{return panel;}} 

    /// <summary>
    /// 每一次加载完成
    /// </summary>
    public virtual void ready()
    {

    }

    /// <summary>
    /// 每一次打开
    /// </summary>
    public virtual void awake()
    {

    }

    /// <summary>
    /// 每一次关闭
    /// </summary>
    public virtual void sleep()
    {

    }

    public void SetData(GameObject goClone)
    {
        panel.SetData(goClone);
        ready();
        awake();
    }
}
