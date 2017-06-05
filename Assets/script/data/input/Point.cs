using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point : ObjData
{
    private static int number = -2;
    public Point()
    {
        number++;
        guid = number;
    }

    public Vector2 pos;

    /// <summary>
    /// 线段连通点 遍历所有墙体wall得出
    /// </summary>
    //public List<Point> nearList = new List<Point>();

    public Point(Vector2 pos):this()
    {
        this.pos = pos;
    }

    public bool equle(Vector2 po)
	{
		return pos == po;
	}

    public void DeSerialize(Dictionary<string, object> dic)
    {
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
                case "pos":
                    pos = MyJsonTool.getVector2(dic, key);
                    break;
                default:
                    break;
            }
        }
    }
}


