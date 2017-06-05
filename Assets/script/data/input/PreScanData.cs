using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PreScanData : ItemData {

    public Vector3 position;
    public Vector3 rotate;
    public string imagemeta;
    
    public bool follow;
    /// <summary>
    /// 先使用follow后使用focaseP
    /// </summary>
    public Vector3 focaseP;

    public PreScanData()
    {

    }
    public PreScanData(Vector3 position, Vector3 rotate, string imagemeta)
    {
        this.position = position;
        this.rotate = rotate;
        this.imagemeta = imagemeta;
    }

    public void DeSerialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "id":
                    id = MyJsonTool.getIntValue(dic, key);
                    break;
                case "position":
                    position = MyJsonTool.getVector3(dic, key);
                    break;
                case "rotate":
                    rotate = MyJsonTool.getVector3(dic, key);
                    break;
                case "imagemeta":
                    imagemeta = MyJsonTool.getStringValue(dic, key);
                    break;
                case "follow":
                    follow = MyJsonTool.getBoolValue(dic, key);
                    break;
                case "focaseP":
                    focaseP = MyJsonTool.getVector3(dic, key);
                    break;
                default:
                    break;
            }
        }
    }
}
