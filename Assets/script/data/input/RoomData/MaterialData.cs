using System;
using System.Collections.Generic;

[System.Serializable]
public class MaterialData : ObjData
{
    public MaterialData()
    {
    }

    public MaterialData(int id, string textureURI)
    {
        this.id = id;
        this.textureURI = textureURI;   
    }

    public string stringId = "";
    public string seekId = "local";
    public string textureURI = "3D/texture2D/defaultfloor.assetbundle";
    public int color = 16777215;
    public float tileSize_x = 1;
    public float tileSize_y = 1;
    /// <summary>
    /// [0, 360)
    /// </summary>
    public float rotation = 0;
    public float offsetX = 0;
	public float offsetY = 0;

    public void Reset()
    {
        color = 16777215;
        tileSize_x = 1;
        tileSize_y = 1;
        rotation = 0;
        offsetX = 0;
        offsetY = 0;
    }

    public void DeSerialize(Dictionary<string, object> dic)
    {
        if (dic == null) return;
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
                case "stringId":
                    stringId = MyJsonTool.getStringValue(dic, key);
                    break;
                case "seekId":
                    seekId = MyJsonTool.getStringValue(dic, key);
                    break;
                case "textureURI":
                    textureURI = MyJsonTool.getStringValue(dic, key);
                    break;
                case "color":
                    color = MyJsonTool.getIntValue(dic, key);
                    break;
                case "tileSize_x":
                    tileSize_x = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "tileSize_y":
                    tileSize_y = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "rotation":
                    rotation = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "offsetX":
                    offsetX = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "offsetY":
                    offsetY = MyJsonTool.getFloatValue(dic, key);
                    break;
                default:
                    break;
            }
        }
    }
}
