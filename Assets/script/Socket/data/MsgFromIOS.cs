
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MsgFromIOS {

    /// <summary>
    /// 自定义显式转换
    /// </summary>
    /// <param name="json"></param>
    public static explicit operator MsgFromIOS(string json)
    {
        Dictionary<string, object> obj = MiniJsonExtensions.dictionaryFromJson(json);
        MsgFromIOS data = new MsgFromIOS();
        data.Deserialize(obj);
        return data;
    }
    //自定义隐式转换
    //public static implicit operator MsgFromIOS(UnityEngine.Object v)
    //{
    //    throw new NotImplementedException();
    //}

    /// <summary>
    /// "201"+"001"
    /// </summary>
    public string code = "201001";
    public InfoFromIOS info = null;

    public virtual void Deserialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "code":
                    code = MyJsonTool.getStringValue(dic, key);
                    break;
                case "info":
                    object value = MyJsonTool.getValue(dic, key);
                    info = new InfoFromIOS();
                    info.Deserialize(value as Dictionary<string, object>);
                    break;
                default:
                    Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                    break;
            }
        }
    }

    [System.Serializable]
    public class InfoFromIOS
    {
        /// <summary>
        /// 方案id enterType==4or5表示样板间id
        /// </summary>
        public string projectId;
        /// <summary>
        /// 方案id
        /// </summary>
        //public string templateId;
        /// <summary>
        /// 1.进入空方案 2.方案2D界面 3.进入方案3D界面  4  5 
        /// </summary>
        public string enterType;

        /// <summary>
        /// 0.失败 1.成功
        /// </summary>
        public string result;

        public object goodsArr;

        public string json;

        public object msg;

        public string CollectType;

        public string priceId;

        /// <summary>
        /// 第一次上传携带临时id
        /// </summary>
        public int tempId;

        public object userInfo;

        public virtual void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "projectId":
                        projectId = MyJsonTool.getStringValue(dic, key);
                        break;
                    //case "templateId":
                    //    templateId = MyJsonTool.getStringValue(dic, key);
                    //    break;
                    case "enterType":
                        enterType = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "result":
                        result = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "goodsArr":
                        goodsArr = MyJsonTool.getValue(dic, key);
                        break;
                    case "json":
                        json = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "msg":
                        msg = MyJsonTool.getValue(dic, key);
                        break;
                    case "CollectType":
                        CollectType = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "priceId":
                        priceId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "tempId":
                        tempId = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "userInfo":
                        userInfo = MyJsonTool.getValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class GoodsInfo
    {
        public string id = "001";//Product-id
        public string count = "1";//ProductData-count
                                  /// <summary>
                                  /// 本地路径  不一定加载成功 不成功的话使用model的远程路径
                                  /// </summary>
        public string path = "asfjasdlhg";//天超的缓存缩略图ProductData-imageresized[0]缓存到IOS数据库的路径
        public Product model;

        public void WriteToSelectProductData(SelectProductData selctproduct)
        {
            selctproduct.seekId = id;
            selctproduct.localthumbnail = path;
            selctproduct.count = int.Parse(count);
        }

        internal void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "count":
                        count = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "path":
                        path = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "model":
                        {
                            object obj = MyJsonTool.getValue(dic, key);
                            model = new Product();
                            model.DeSerialize(obj as Dictionary<string, object>);
                        }
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }

    }

}
