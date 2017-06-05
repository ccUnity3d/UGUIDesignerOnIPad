
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单面墙壁数据
/// </summary>
[System.Serializable]
public class WallSideData : ObjData{
    private static int number = 0;
    public WallSideData()
    {
        guid = id = number;
        //Debug.Log(number);
        number++;
    }

    public WallSideData(bool forward)
    {
        this.forward = forward;
        guid = id = number;
        //Debug.Log(number);
        number++;
    }


    /// <summary>
    /// 所属墙体
    /// </summary>
    [System.NonSerialized]
    public WallData targetWall;

    /// <summary>
    /// 墙体正向
    /// </summary>
    public bool forward = true;


    public MaterialData materialData;

    public TBaseboard tBaseboard = new TBaseboard(true);
    public TBaseboard topBaseboard = new TBaseboard(false);

    //序列化专用
    public int targetWallGuid;

    public void BeforetSerializeFieldDo()
    {
        targetWallGuid = targetWall == null ? -1 : targetWall.guid;
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
                case "targetWallGuid":
                    targetWallGuid = MyJsonTool.getIntValue(dic, key);
                    break;
                case "forward":
                    forward = MyJsonTool.getBoolValue(dic, key);
                    break;
                case "materialData":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        materialData.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break;
                case "tBaseboard":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        tBaseboard.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
