using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class RenderEffectItemData : ItemData {

    public string meta;
    public RenderEffectItemData()
    {

    }

    public RenderEffectItemData(string meta)
    {
        this.meta = meta;
    }

    //public void Deserialize(Dictionary<string, object> dic)
    //{
    //    foreach (string key in dic.Keys)
    //    {
    //        switch (key)
    //        {
    //            case "id":
    //                id = MyJsonTool.getIntValue(dic, key);
    //                break;
    //            case "meta":
    //                meta = MyJsonTool.getStringValue(dic, key);
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}

}
