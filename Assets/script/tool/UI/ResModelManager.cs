using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResModelManager  {

    // 字典用来存储
    static Dictionary<string , GameObject > myDics = new Dictionary<string, GameObject> ();

    #region 添加模型预设 
    public void AddModel(GameObject myGameObject)
    {
        if (!myDics.ContainsValue(myGameObject))
        {
            myDics.Add(myGameObject.name, myGameObject);
        }
    }
    #endregion

    #region 移除模型预设
    public void MoveModel(GameObject myGameObject)
    {
        if (myDics.ContainsValue(myGameObject))
        {
            myDics.Remove(myGameObject.name);
        }
    }
    #endregion


}
