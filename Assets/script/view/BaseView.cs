using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseView
{

    protected OriginalInputData data
    {
        get {
            return OriginalInputData.Instance;
        }
    }

    protected LineHelpFunc linefunc
    {
        get
        {
            return LineHelpFunc.Instance;
        }
    }

    protected WallHelpFunc wallfunc
    {
        get
        {
            return WallHelpFunc.Instance;
        }
    }
    protected RoomHelpFunc roomfunc
    {
        get
        {
            return RoomHelpFunc.Instance;
        }
    }
    protected Mode2DPrefabs prefabs
    {
        get
        {
            return Mode2DPrefabs.Instance;
        }
    }
    protected DefaultSettings defaultSetting
    {
        get {
            return DefaultSettings.Instance;
        }
    }

    protected MainPageUIData mainPageData
    {
        get
        {
            return MainPageUIData.Instance;
        }
    }

    public virtual void RefreshView()
    {

    }

    public virtual void sleep()
    {

    }

    protected virtual void RefreshGameObjs()
    {

    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="count">数据长度</param>
    /// <param name="list">物体列表</param>
    /// <param name="func">创建物体的方法</param>
    /// <param name="number">一个数据创建物体数量</param>
    protected void SetGameObjCountEquleList(int count, List<GameObject> list, Func<Transform> func, int number = 1)
    {
        for (int i = count * number; i < list.Count;)
        {
            GameObject go = list[i];
            list.RemoveAt(i);
            GameObject.Destroy(go);
        }
        for (int i = list.Count; i < count * number; i = list.Count)
        {
            for (int j = 0; j < number; j++)
            {
                GameObject go = func().gameObject;
                list.Add(go);
            }
        }
    }

    //protected virtual void SetGameObjDicEquleList(List<ProductData> productList, Dictionary<int, GameObject> products)
    //{
    //    List<int> removeItems = new List<int>();
    //    foreach (int id in products.Keys)
    //    {
    //        bool has = false;
    //        for (int i = 0; i < productList.Count; i++)
    //        {
    //            if (productList[i].id == id)
    //            {
    //                has = true;
    //                break;
    //            }
    //        }
    //        if (has == false)
    //        {
    //            removeItems.Add(id);
    //        }
    //    }
    //    for (int i = 0; i < removeItems.Count; i++)
    //    {
    //        int id = removeItems[i];
    //        GameObject.Destroy(products[id]);
    //        products.Remove(id);
    //    }

    //    for (int i = 0; i < productList.Count; i++)
    //    {
    //        int id = productList[i].id;
    //        if (products.ContainsKey(id) == false)
    //        {
    //            GameObject go = GetProducts(id);
    //            products.Add(id, go);
    //        }
    //    }
    //}
    
}