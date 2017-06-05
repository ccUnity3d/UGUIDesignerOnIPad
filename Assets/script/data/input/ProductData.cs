using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
/// <summary>
/// 存储数据
/// </summary>
public class ProductData : ObjData {

    //俯视图坐标 没高度p1=(x,0,z)  转化2d p2 =(x=p1.x,y=p1.z)
    public Vector3 position = Vector3.zero;
    /// <summary>
    /// 产品的俯视旋转角 UV计算用不到
    /// </summary>
    public float rotate = 0;

    ///// <summary>
    ///// 缩放系数
    ///// </summary>
    //public Vector3 scale = Vector3.one;
    /// <summary>
    /// 模型缩放系数
    /// </summary>
    public Vector3 scale = Vector3.one;

    /// <summary>
    /// 离地高度 默认为product defaltHeight
    /// </summary>
    public float height;

    public string seekId = "";

    [System.NonSerialized]
    public WallData targetWall = null;

    /// <summary>
    /// 产品类型 0.不吸附；1.吸附； 2.门窗， 3.吊顶；
    /// </summary>
    public int type = 0;

    private float forTop = 180;


    //序列化专用
    public int targetWallGuid;

    public void BeforetSerializeFieldDo()
    {
        targetWallGuid = targetWall == null ? -1 : targetWall.guid;
    }

    public ProductData()
    {

    }

    public ProductData(int goodsId, Vector3 position, float rotate, string seekId, float defaultHeight, int type)
    {
        this.id = goodsId;
        this.position = position;
        this.rotate = rotate;
        this.seekId = seekId;
        this.height = defaultHeight;
        this.type = type;
    }

    public void setData(GameObject instanceObj, Vector3 size, float y, bool is2D = false)
    {
        if (is2D)
        {
            Vector3 pos = position.x * Vector3.right + position.z * Vector3.up + (-5 - y) * Vector3.forward;
            instanceObj.transform.position = pos;
            instanceObj.transform.rotation = Quaternion.Euler(rotate * Vector3.forward);
            Vector3 scale = Vector3.right * this.scale.x * size.x + Vector3.up * this.scale.z * size.z + Vector3.forward;
            if (type == 2 && targetWall != null) { scale.y = targetWall.width; }
            instanceObj.transform.localScale = scale;
        }
        else
        {
            instanceObj.transform.position = position + Vector3.up * (y - position.y);
            instanceObj.transform.rotation = Quaternion.Euler(- (rotate - forTop) * Vector3.up);
            Vector3 scale = Vector3.right * this.scale.x * size.x + Vector3.up * this.scale.y * size.y + Vector3.forward * this.scale.z * size.z;
            //if (type == 2 && targetWall != null) { scale.y = targetWall.width; }
            instanceObj.transform.localScale = scale;
            //Debug.LogWarning(-rotate);
        }
    }

    public Vector3 GetV3Withheight()
    {
        return position + Vector3.up * (height - position.y);
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
                case "targetWallGuid":
                    targetWallGuid = MyJsonTool.getIntValue(dic, key);
                    break;
                case "seekId":
                    seekId = MyJsonTool.getStringValue(dic, key);
                    break;
                case "position":
                    position = MyJsonTool.getVector3(dic, key);
                    break;
                case "scale":
                    scale = MyJsonTool.getVector3(dic, key);
                    break;
                case "rotate":
                    rotate = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "height":
                    height = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "type":
                    type = MyJsonTool.getIntValue(dic, key);
                    break;
                default:
                    break;
            }
        }
    }
}
