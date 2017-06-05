
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// 自己的总数据 整个设计
/// </summary>
public class OriginalProjectData
{
    public OriginalProjectData()
    {
        version = new MyTimeData(DateTime.Now);
    }

    public string id;
    public string templateId;
    public string meta;//"data:image/jpeg;base64,"
    public OriginalInputData data;
    public string name;
    public string description;

    public List<string> priceIdList;

    /// <summary>
    /// 为未上传json有很多 做区分使用的Id
    /// </summary>
    public int tempId = 0;
    /// <summary>
    /// 是否为新建 101004保存方案用
    /// </summary>
    public bool isNew;

    public MyTimeData version;

    /// <summary>
    /// 是否为样板间 101004保存方案用-是否为另存新方案
    /// </summary>
    [NonSerialized]
    public bool isTemplate = false;
    [NonSerialized]
    public List<string> tempEffectMetas = new List<string>();

    /// <summary>
    /// 是否为未上传
    /// </summary>
    public bool isUnUpload
    {
        get {
            if (string.IsNullOrEmpty(id))
            {
                return true;
            }
            return false;
        }
    }

    public void DeSerialize(Dictionary<string, object> dic)
    {
        if (dic == null)
        {
            Debug.LogError("OriginalProjectData dic.DeSerialize(dic), dic = null");
            return;
        }
        data = new OriginalInputData();
        version = new MyTimeData();
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "id":
                    id = MyJsonTool.getStringValue(dic, key);
                    break;
                case "meta":
                    meta = MyJsonTool.getStringValue(dic, key);
                    break;
                case "data":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        data.isOld = false;
                        if (dic.ContainsKey("version") == false)
                        {
                            data.isOld = true;
                        }
                        data.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break;
                case "name":
                    name = MyJsonTool.getStringValue(dic, key);
                    break;
                case "description":
                    description = MyJsonTool.getStringValue(dic, key);
                    break;
                case "priceIdList":
                    priceIdList = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "tempId":
                    tempId = MyJsonTool.getIntValue(dic, key);
                    break;
                case "idNew":
                    isNew = MyJsonTool.getBoolValue(dic, key);
                    //isNew = false;
                    break;
                case "version":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        version.DeSerialize(obj as Dictionary<string, object>);
                    //isNew = false;
                    }
                    break;
                default:
                    break;
            }
        }
    }

}
