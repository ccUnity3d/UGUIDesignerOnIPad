using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SchemeManifest :JsonSchemeManifestData  {

    public SchemeManifest()
    {

    }
    public static SchemeManifest Instance
    {
        get
        {
            return UndoHelper.Instance.currentData.schemeManifest;
        }
    }

    /// <summary>
    /// 样板间id
    /// </summary>
    public string templateId;

    // 效果图
    public List<string> images = new List<string> ();
    // 设置主图
    public List<string> mainImages;
    // 价格<所有报价单>   点击生成报价是一个报价单， 这个报价单生成多个
    /// <summary>
    /// 报价单
    /// </summary>
    public List<string> prices = new List<string>() ;
    // 分享
    //public List<ShareOffer> shareOffer = new List<ShareOffer>();

    public int tempId;
    /// <summary>
    /// 101004保存方案用
    /// </summary>
    public bool isNew = true;
    public string meta;//"data:image/jpeg;base64,"  +  texture.ToEncoding();
    public MyTimeData version;

    [System.NonSerialized]
    public List<string> tempEffectMetas = new List<string>();
    /// <summary>
    /// 是否为样板间 enterType = 4,5;
    /// </summary>
    [System.NonSerialized]
    public bool isTemplate;

    public bool isUnUpLoaded
    {
        get
        {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            return false;
        }
    }

    public override void Deserialize(Dictionary<string, object> dic)
    {
        //foreach (string key in dic.Keys)
        //{
        //    switch (key)
        //    {
        //        case "name":
        //            name = MyJsonTool.getStringValue(dic, key);
        //            break;
        //        case "id":
        //            id = MyJsonTool.getStringValue(dic,key);
        //            break;
        //        case "description":
        //            description = MyJsonTool.getStringValue(dic, key);
        //            break;
        //        case "images":
        //            images = MyJsonTool.getStringListValue(dic,key);
        //            break;
        //        case "mainImages":
        //            mainImages = MyJsonTool.getStringListValue(dic, key);
        //            break;
        //        case "prices":
        //            prices = MyJsonTool.getStringListValue(dic, key);
        //            break;
        //        default:
        //            break;
        //    }
            
        //}
    }

}
[System.Serializable]
public class ItemPrice : ItemData
{
    /// <summary>
    /// 产品 ID
    /// </summary>
    public string seekId;
    /// <summary>
    /// 产品 ID
    /// </summary>
    public string image;//imageresize[0]
    /// <summary>
    /// 产品 ID
    /// </summary>
    public string productName;//name
    /// <summary>
    /// 产品数量
    /// </summary>
    public int count;
    
    [System.NonSerialized]
    ///// <summary>
    ///// 编辑项是否包含/选中此项
    ///// </summary>
    public bool isSelect = false;
    public void Deserialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "id":
                    id = MyJsonTool.getIntValue(dic, key);
                    break;
                case "seekId":
                    seekId = MyJsonTool.getStringValue(dic, key);
                    break;
                case "image":
                    image = MyJsonTool.getStringValue(dic, key);
                    break;
                case "productName":
                    productName = MyJsonTool.getStringValue(dic, key);
                    break;
                case "count":
                    count = MyJsonTool.getIntValue(dic, key);
                    break;
                default:
                    break;
            }
        }
    }
}
//[System.Serializable]
//public class ShareOffer : JsonSchemeManifestData
//{
//    // 分享 标题
//    public string title;
//    // 分享 案例名
//    public string schemeName;
//    // 分享描述
//    public string desShare;

//    public PriceData data;

//    public override void Deserialize(Dictionary<string, object> dic)
//    {
//        foreach (string key in dic.Keys)
//        {
//            switch (key)
//            {
//                case "title":
//                    title = MyJsonTool.getStringValue(dic,key);
//                    break;
//                case "schemeName":
//                    schemeName = MyJsonTool.getStringValue(dic, key);
//                    break;
//                case "desShare":
//                    desShare = MyJsonTool.getStringValue(dic,key);
//                    break;
//                default:
//                    break;
//            }
//        }
//    }
//}

[System.Serializable]
public abstract class JsonSchemeManifestData
{
    // id
    public string id = "";
    // 方案名字
    public string name = "";
    // 方案描述
    public string description = "";

    public abstract void Deserialize(Dictionary<string,object> dic);

}
