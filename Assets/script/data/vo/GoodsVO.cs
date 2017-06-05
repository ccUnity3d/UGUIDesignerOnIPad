using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GoodsVO {
    public GoodsVO(){}
    public GoodsVO(int id, string uri, string uri2D, string modelUrl, string ContentType, int state, int count, string seekId = "")
    {
        this.id = id;
        this.uri = uri;
        this.uri2D = uri2D;
        this.modelUri = modelUrl;
        //this.type = entityType;//material_floor seating flooring ceiling surface window door wall_attachment ceiling_attachment    hole Assembly

        this.state = state;
        this.count = count;
        this.seekId = seekId;
        switch (ContentType)
        {
            //case "material_floor":
            case "seating":
            case "wall_attachment":
            case "":
            case "attach to wall":
            case "wall-attached":
                this.type = 1;
                break;
            case "window":
            case "door":
                this.type = 2;
                break;
            case "ceiling_attachment":
            case "attach to ceiling":
            case "ceiling-attached":
                this.type = 3;
                break;
            case "flooring":
            case "rug":
            case "on the floor":
            case "floor-based":
                this.type = 4;
                break;
            case "ceiling":
                this.type = 5;
                break;
            case "surface":
            case "on top of others":
                this.type = 6;
                break;
            case "hole":
            case "Assembly":
            default:
                this.type = 0;
                break;
        }
    }
    public long guid;

    public int id;

    /// <summary>
    /// UI图标
    /// </summary>
    public string uri;

    /// <summary>
    /// 2D视图路径
    /// </summary>
    public string uri2D;
    /// <summary>
    /// 模型路径
    /// </summary>
    public string modelUri;

    /// <summary>
    /// 0.不吸附 1.吸附 2.门窗 3.吊顶 4.地毯 5.天花板 6.各个平面
    /// </summary>
    public int type;

    /// <summary>
    /// 物品状态可用 不可用
    /// </summary>
    public int state;

    /// <summary>
    /// 序列帧展示数量
    /// </summary>
    public int count;

    /// <summary>
    /// 产品id
    /// </summary>
    public string seekId;
}
