using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 门窗范围数据
/// </summary>
public class DoorData : ObjData
{
    /// <summary>
    /// 整体位置 相对墙壁坐标的偏移量
    /// </summary>
    public Vector2 pos;
    /// <summary>
    /// 缩放系数
    /// </summary>
    public Vector3 scale = Vector3.one;
    /// <summary>
    /// 旋转
    /// </summary>
    public float rotate;

    /// <summary>
    /// 产品
    /// </summary>
    public Product product;

    /// <summary>
    /// 所在墙体
    /// </summary>
    public WallData target;
    public int host
    {
        get
        {
            if (target == null)
            {
                return -1;
            }
            return target.guid;
        }
    }

    /// <summary>
    /// 模型尺寸
    /// </summary>
    public Vector3 size = Vector3.one;
    /// <summary>
    /// uv点列表
    /// </summary>
    public Vector2[] points;
    /// <summary>
    /// 产品id
    /// </summary>
    public string seekId;

    /// <summary>
    /// 门口朝向
    /// </summary>
    public int swing = 0;
}

