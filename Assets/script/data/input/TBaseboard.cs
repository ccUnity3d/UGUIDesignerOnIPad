using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TBaseboard : ObjData
{
    public TBaseboard(bool isT)
    {
        this.hide = true;
        this.isT = isT;
    }

    public bool isT = true;
    public int type = 0;
    public float width = 0.01f;
    public float height = 0.08f;
    public float disRoot = 0;

    public MaterialData materialData;

    //临时本地数据序号
    public int index = 0;

    public float multi
    {
        get
        {
            return 1 / (width * 2 + height);
        }
    }

    public void DeSerialize(Dictionary<string, object> dic)
    {
        materialData = new MaterialData();
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "guid":
                    guid = MyJsonTool.getIntValue(dic, key);
                    break;
                case "id":
                    id = MyJsonTool.getIntValue(dic, key);
                    break;
                case "hide":
                    hide = MyJsonTool.getBoolValue(dic, key);
                    break;
                case "isT":
                    isT = MyJsonTool.getBoolValue(dic, key);
                    break;
                case "type":
                    type = MyJsonTool.getIntValue(dic, key);
                    break;
                case "width":
                    width = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "height":
                    height = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "disRoot":
                    disRoot = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "materialData":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        materialData.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break;
                case "index":
                    index = MyJsonTool.getIntValue(dic, key);
                    break;
                default:
                    break;
            }
        }
    }
}
