using System.Collections.Generic;

/// <summary>
/// 一个报价单
/// </summary>
[System.Serializable]
public class PriceData
{
    /// <summary>
    /// 所属方案id
    /// </summary>
    public string targetServerId;
    /// <summary>
    /// 如果方案没有正式id时候使用 所属方案临时id
    /// </summary>
    public int targetTempId;
    public string targetId
    {
        get {
            if (string.IsNullOrEmpty(targetServerId))
            {
                return targetTempId.ToString();
            }
            return targetServerId;
        }
    }
    public bool isTargetUnUpload
    {
        get
        {
            if (string.IsNullOrEmpty(targetServerId))
            {
                return true;
            }
            return false;
        }
    }
    /// <summary>
    /// 设为主报价
    /// </summary>
    public bool isSetMainOffer;
    /// <summary>
    /// 报价命名
    /// </summary>
    public string name;
    /// <summary>
    /// 报价描述
    /// </summary>
    public string describe;
    /// <summary>
    /// 单个报价单
    /// </summary>
    public List<ItemData> priceList;
    /// <summary>
    /// 报价服务端推送的Id
    /// </summary>
    public string serverId;
    /// <summary>
    /// 服务端未分配正式id前的临时id
    /// </summary>
    public int tempId;
    /// <summary>
    /// 即新建方案-未分配正式id的方案
    /// </summary>
    public bool isNew;
    public string stringId
    {
        get
        {
            if (string.IsNullOrEmpty(serverId))
            {
                return tempId.ToString();
            }
            return serverId;
        }
    }
    /// <summary>
    /// 是否未上传服务端
    /// </summary>
    public bool isUnUpload
    {
        get {
            if (string.IsNullOrEmpty(serverId))
            {
                return true;
            }
            return false;
        }
    }

    public void Deserialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "name":
                    name = MyJsonTool.getStringValue(dic, key);
                    break;
                case "describe":
                    describe = MyJsonTool.getStringValue(dic, key);
                    break;
                case "priceList":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        if (obj == null)
                        {
                            priceList = null;
                            break;
                        }
                        priceList = new List<ItemData>();
                        List<object> arr = obj as List<object>;
                        for (int i = 0; i < arr.Count; i++)
                        {
                            ItemPrice data = new ItemPrice();
                            data.Deserialize(arr[i] as Dictionary<string, object>);
                            priceList.Add(data);
                        }
                    }
                    break;
                case "targetServerId":
                    targetServerId = MyJsonTool.getStringValue(dic, key);
                    break;
                case "targetTempId":
                    targetTempId = MyJsonTool.getIntValue(dic, key);
                    break;
                case "serverId":
                    serverId = MyJsonTool.getStringValue(dic, key);
                    break;
                case "tempId":
                    tempId = MyJsonTool.getIntValue(dic, key);
                    break;
                case "isSetMainOffer":
                    isSetMainOffer = MyJsonTool.getBoolValue(dic, key);
                    break;
                case "isNew":
                    isNew = MyJsonTool.getBoolValue(dic, key);
                    break;
                default:
                    break;
            }

        }
    }
}
