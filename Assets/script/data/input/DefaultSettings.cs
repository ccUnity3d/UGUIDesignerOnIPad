
using System;
using System.Collections.Generic;

[System.Serializable]
public class DefaultSettings 
{
    public static DefaultSettings Instance
    {
        get {
            return OriginalInputData.Instance.defaultsettings;
        }
    }

    /// <summary>
    /// 默认离地高
    /// </summary>
    public float DefaultDistanceGround = 0.2F;

    public float DefaultDistanceWall = 0.2F;

    /// <summary>
    /// 默认尺寸单位
    /// </summary>
    public Unit DefaultUnit = Unit.mm;

    public float DefaultLength = 2.10f;

    /// <summary>
    /// adj.斜，倾斜的，非直角的，非垂直的，斜角
    /// </summary>
    public bool oblique = false;

    /// <summary>
    /// 默认墙高
    /// </summary>
    public float DefaultHeight = 2.6f;
    /// <summary>
    /// 默认墙宽
    /// </summary>
    public float DefaultWidth = 0.24f;


    /// <summary>
    /// 3D-漫游视野高度
    /// </summary>
    public float minFollowViewHeight = 10;
    public float maxFollowViewHeight = 100000;
    public float defaultFollowViewHeight = 1600;

    /// <summary>
    /// 3D-自由视野高度
    /// </summary>
    public float minFreeViewDis= 0.5f;
    public float maxFreeViewDis= 100f;
    public float defaultFreeViewLerp = 0.016f;

    /// <summary>
    /// 3D视角横向宽度
    /// </summary>
    public float minViewWidth = 10;
    public float maxViewWidth = 110;
    public float defaultViewWidth = 60;

    /// <summary>
    /// 3D-自由视角垂直角度
    /// </summary>
    public float minFreeViewAngle = 0;
    public float maxFreeViewAngle = 87;
    public float defaultFreeViewAngle = 45;

    /// <summary>
    /// 3D-漫游视角垂直角度
    /// </summary>
    public float minFollowViewAngle = -45;
    public float maxFollowViewAngle = 45;
    public float defaultFollowViewAngle = 0;

    /// <summary>
    /// 物品摆放的最低位置
    /// </summary>
    public float goodsMinheight = 0;
    public float goodsMaxheight = 10;
    public readonly float HalfWallHeight = 0.48f;

    public void DeSerialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "DefaultUnit":
                    {
                        string value = MyJsonTool.getStringValue(dic, key);
                        switch (value)
                        {
                            case "m":
                                DefaultUnit = Unit.m;
                                break;
                            case "dm":
                                DefaultUnit = Unit.dm;
                                break;
                            case "cm":
                                DefaultUnit = Unit.cm;
                                break;
                            case "mm":
                                DefaultUnit = Unit.mm;
                                break;
                            case "ft":
                                DefaultUnit = Unit.ft;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case "oblique":
                    oblique = MyJsonTool.getBoolValue(dic, key);
                    break;
                case "DefaultDistanceGround":
                    DefaultDistanceGround = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "DefaultDistanceWall":
                    DefaultDistanceWall = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "DefaultLength":
                    DefaultLength = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "DefaultHeight":
                    DefaultHeight = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "DefaultWidth":
                    DefaultWidth = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "minFollowViewHeight":
                    minFollowViewHeight = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "maxFollowViewHeight":
                    maxFollowViewHeight = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "defaultFollowViewHeight":
                    defaultFollowViewHeight = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "minFreeViewDis":
                    minFreeViewDis = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "maxFreeViewDis":
                    maxFreeViewDis = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "defaultFreeViewLerp":
                    defaultFreeViewLerp = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "minViewWidth":
                    minViewWidth = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "maxViewWidth":
                    maxViewWidth = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "defaultViewWidth":
                    defaultViewWidth = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "minFreeViewAngle":
                    minFreeViewAngle = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "maxFreeViewAngle":
                    maxFreeViewAngle = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "defaultFreeViewAngle":
                    defaultFreeViewAngle = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "minFollowViewAngle":
                    minFollowViewAngle = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "maxFollowViewAngle":
                    maxFollowViewAngle = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "defaultFollowViewAngle":
                    defaultFollowViewAngle = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "goodsMinheight":
                    goodsMinheight = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "goodsMaxheight":
                    goodsMaxheight = MyJsonTool.getFloatValue(dic, key);
                    break;
                default:
                    break;
            }
        }
    }
}

public enum Unit
{
    m = 1,
    dm = 10,
    cm = 100,
    mm = 1000,
    ft = 10000
}
