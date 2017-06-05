
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// 房间
/// </summary>
public class RoomData : ObjData
{
    public string name = "";

    public string type = "";

    [System.NonSerialized]
    /// <summary>
    /// 所有墙面
    /// </summary>
    public List<WallSideData> sideList = new List<WallSideData>();

    /// <summary>
    /// 地板(地板的范围有墙体围成 即sideList所决定)
    /// </summary>
    public MaterialData floor = new MaterialData();

    public float ceilingHeight = 0;

    /// <summary>
    /// 屋顶 范围有墙体围成 即sideList所决定
    /// </summary>
    public MaterialData ceiling = new MaterialData();

    [System.NonSerialized]
    public List<Point> pointList;

    private DefaultSettings settings = DefaultSettings.Instance;


    //序列化专用
    public List<int> sideListGuids;
    public List<int> pointListGuids;

    public void BeforetSerializeFieldDo()
    {
        pointListGuids = new List<int>();
        for (int i = 0; i < pointList.Count; i++)
        {
            pointListGuids.Add(pointList[i].guid);
        }
        sideListGuids = new List<int>();
        for (int i = 0; i < sideList.Count; i++)
        {
            sideListGuids.Add(sideList[i].guid);
        }

    }


    public RoomData()
    {
        ceilingHeight = settings.DefaultHeight;
    }

    public bool equal(RoomData room)
    {
        if (room.sideList.Count != sideList.Count)
        {
            return false;
        }

        for (int i = 0; i < room.sideList.Count; i++)
        {
            if (sideList.Contains(room.sideList[i]) == false)
            {
                return false; 
            }
        }

        return true;
    }


    /// <summary>
    /// 获取side边的下一个边
    /// </summary>
    public WallSideData NextSide(WallSideData side)
    {
        int index = sideList.IndexOf(side);
        if (index == -1) { return null; }
        return sideList[(index + 1) % sideList.Count];
    }
    /// <summary>
    /// 获取side边的前一个边
    /// </summary>
    public WallSideData LastSide(WallSideData side)
    {
        int index = sideList.IndexOf(side);
        if (index == -1) { return null; }
        return sideList[(index - 1) % sideList.Count];
    }

    public void Replace(Point oldpoint, Point newpoint)
    {
        int index = pointList.IndexOf(oldpoint);
        if (index == -1)
        {
            Debug.Log("oldpoint not in room");
            return;
        }
        pointList[index] = newpoint;
    }

    public void DeSerialize(Dictionary<string, object> dic)
    {
        if (dic == null)
        {
            Debug.LogWarning("RoomData DeSerialize(dic == null)");
            return;
        }
        sideListGuids = new List<int>();
        pointListGuids = new List<int>();
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
                case "name":
                    name = MyJsonTool.getStringValue(dic, key);
                    break; 
                case "type":
                    type = MyJsonTool.getStringValue(dic, key);
                    break; 
                case "ceilingHeight":
                    ceilingHeight = MyJsonTool.getFloatValue(dic, key);
                    break; 
                case "ceiling":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        ceiling.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break; 
                case "floor":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        floor.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break; 
                case "sideListGuids":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        if (obj == null) break;
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            int Guid = int.Parse(list[i].ToString());
                            sideListGuids.Add(Guid);
                        }
                    }
                    break;
                case "pointListGuids":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        if (obj == null) break;
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            int Guid = int.Parse(list[i].ToString());
                            pointListGuids.Add(Guid);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
