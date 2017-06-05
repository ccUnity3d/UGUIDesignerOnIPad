using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 墙体 直线方程ax+by+c=0
/// </summary>
[System.Serializable]
public class WallData : ObjData
{
    private static int number = 0;
    /// <summary>
    /// 显示尺寸距离墙边线的距离 定值
    /// </summary>
    private readonly float wordDis = 0.24f;

    public float length
    {
        get {
            return Vector2.Distance(point1.pos, point2.pos);
        }
    }
    /// <summary>
    /// 墙厚度
    /// </summary>
    public float width;
    /// <summary>
    /// 墙高度
    /// </summary>
    public float height;

    [System.NonSerialized]
    public Point point1;
    [System.NonSerialized]
    public Point point2;
    /// <summary>
    /// 承重墙
    /// </summary>
    public bool isSupporting = true;

    /// <summary>
    /// 点1到点2右侧墙壁数据
    /// </summary>
    public WallSideData point1To2Data = new WallSideData(true);
    /// <summary>
    /// 点2到点1右侧墙壁数据
    /// </summary>
    public WallSideData point2To1Data = new WallSideData(false);


    //序列化专用
    public int point1Guid;
    public int point2Guid;




    public void BeforetSerializeFieldDo()
    {
        point1Guid = point1.guid;
        point2Guid = point2.guid;
        point1To2Data.BeforetSerializeFieldDo();
        point2To1Data.BeforetSerializeFieldDo();
    }



    public WallData ()
	{
        this.guid = number;
        number++;
	}

    public WallData(Point point1, Point point2):this()
    {
        this.point1 = point1;
        this.point2 = point2;
    }

    public bool equle(WallData data)
    {
        return equle(data.point1, data.point2);
    }

    public bool equle(Point tempPoint1, Point tempPoint2)
    {
        if (tempPoint1 == point1 && tempPoint2 == point2)
        {
            return true;
        }
        if (tempPoint1 == point2 && tempPoint2 == point1)
        {
            return true;
        }
        return false;
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
                case "width":
                    width = MyJsonTool.getFloatValue(dic, key);
                    break; ;
                case "height":
                    height = MyJsonTool.getFloatValue(dic, key);
                    break; ;
                case "point1Guid":
                    point1Guid = MyJsonTool.getIntValue(dic, key);
                    break; ;
                case "point2Guid":
                    point2Guid = MyJsonTool.getIntValue(dic, key);
                    break; ;
                case "point1To2Data":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        point1To2Data.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break; ;
                case "point2To1Data":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        point2To1Data.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break;
                default:
                    break;
            }
        }
    }

    //public Vector2 GetDisPoint(Vector2 v2)
    //{
    //    float A = a;
    //    float B = b;
    //    float C = c;

    //    float x0 = v2.x;
    //    float y0 = v2.y;

    //    return linefunc.GetDisPoint(A, B, C, x0, y0);
    //}

    /// <summary>
    /// 参数 a = y2 - y1;
    /// </summary>
    public float a
    {
        get
        {
            return point2.pos.y - point1.pos.y;
        }
    }

    /// <summary>
    /// 参数 b = -(x2 - x1);
    /// </summary>
    public float b
    {
        get
        {
            return -(point2.pos.x - point1.pos.x);
        }
    }

    /// <summary>
    /// 参数 c = -x1 * y2 + x2 * y1;
    /// </summary>
    public float c
    {
        get
        {
            return -point1.pos.x * point2.pos.y + point2.pos.x * point1.pos.y;
        }
    }

    private LineHelpFunc linefunc
    {
        get {
            return LineHelpFunc.Instance;
        }
    }

    /// <summary>
    /// point1To2Data直线 a * x + b * y + c1 = 0; GetDis(point1) = Width/2;
    /// </summary>
    public float c1
    {
        get
        {
            //c1 = a * x0 + b * y0 +/- width/2f * Mathf.Sqrt(a * a + b * b);原理：point1点到直线ax+by+c1=0的距离为width/2；
            return getC(width / 2f, true);
        }
    }

    /// <summary>
    /// point2To1Data直线 a * x + b * y + c2 = 0;GetDis(point1) = Width/2;
    /// </summary>
    public float c2
    {
        get
        {
            return getC(width / 2f, false);
        }
    }

    /// <summary>
    /// 右侧显示尺寸的方程c值(包括墙半宽)
    /// </summary>
    public float c3
    {
        get
        {
            return getC(width / 2f + wordDis, true);
        }
    }

    /// <summary>
    /// 左侧显示尺寸的方程c值(包括墙半宽)
    /// </summary>
    public float c4
    {
        get
        {
            return getC(width / 2f + wordDis, false);
        }
    }


    /// <summary>
    /// 右侧显示尺寸的方程c值(不包括墙宽)
    /// </summary>
    public float c5
    {
        get
        {
            return getC(wordDis, true);
        }
    }

    /// <summary>
    /// 左侧显示尺寸的方程c值(不包括墙宽)
    /// </summary>
    public float c6
    {
        get
        {
            return getC(wordDis, false);
        }
    }

    /// <summary>
    /// 右侧显示尺寸的方程c值(包括墙宽)
    /// </summary>
    public float c7
    {
        get
        {
            return getC(width + wordDis, true);
        }
    }

    /// <summary>
    /// 左侧显示尺寸的方程c值(包括墙宽)
    /// </summary>
    public float c8
    {
        get
        {
            return getC(width + wordDis, false);
        }
    }

    private float getC(float dis, bool clockwise = true)
    {
        float A = a;
        float B = b;

        float x0 = point1.pos.x;
        float y0 = point1.pos.y;        

        float sqrt = Mathf.Sqrt(A * A + B * B);
        float tempc = dis * sqrt - A * x0 - B * y0;

        Vector2 hitPoint = linefunc.GetDisPoint(A, B, tempc, x0, y0);

        int side = clockwise == true ? 1 : -1;
        if (GetPointOnWallSide(hitPoint) == side)
        {
            return tempc;
        }

        return -dis * sqrt - A * x0 - B * y0;
    }

    /// <summary>
    /// 1.顺时针 0.共线 -1.逆时针  v2在墙的哪一侧？
    /// </summary>
    public int GetPointOnWallSide(Vector2 v2)
    {
        return linefunc.Clockwise(point1.pos, point2.pos, v2);
    }

    /// <summary>
    /// 墙倾斜角
    /// </summary>
    public float GetWallAngle()
    {
        return linefunc.GetAngle(point1.pos - point2.pos);
    }
    /// <summary>
    /// 点v2到这个直线的距离
    /// </summary>
    /// <param name="v2">点</param>
    public float GetDis(Vector2 v2)
    {
        float A = a; float B = b; float C = c;
        float x0 = v2.x; float y0 = v2.y;
        return linefunc.GetDis(A, B, C, x0, y0);
    }

    /// <summary>
    /// 点v2到这个直线的投影取距离垂点为dis的点  【ax+by+c=0与a(y-y0)=b(x-x0)的交点】
    /// </summary>
    /// <param name="v2">点</param>
    /// <returns></returns>
    public Vector2 GetDisPoint(Vector2 v2, float dis = 0)
    {
        float A = a;
        float B = b;
        float C = c;
        float x0 = v2.x;
        float y0 = v2.y;
        Vector2 point = linefunc.GetDisPoint(A, B, C, x0, y0);
        if (dis == 0) return point;

        float x1 = point.x;
        float y1 = point.y;

        float sqrt = Mathf.Sqrt(A * A + B * B);
        float tempc = dis * sqrt - A * x1 - B * y1;

        Vector2 hitPoint = linefunc.GetDisPoint(A, B, tempc, x1, y1);

        if (GetPointOnWallSide(v2) >= 0)
        {
            if (GetPointOnWallSide(hitPoint) == 1)
            {
                return hitPoint;
            }
            else
            {
                tempc = -dis * sqrt - A * x1 - B * y1;
                hitPoint = linefunc.GetDisPoint(A, B, tempc, x1, y1);
                return hitPoint;
            }
        }
        else
        {
            if (GetPointOnWallSide(hitPoint) == -1)
            {
                return hitPoint;
            }
            else
            {
                tempc = -dis * sqrt - A * x1 - B * y1;
                hitPoint = linefunc.GetDisPoint(A, B, tempc, x1, y1);
                return hitPoint;
            }
        }
        ///不能这样写 totalDis太小容易出大误差 ==0时候还有错
        //Vector2 direct = (v2 - point);
        //float totalDis = direct.magnitude;
        //if (totalDis == 0) return point;
        //return point + direct * (dis / totalDis);
    }

    public bool Contines(Point item)
    {
        return point1 == item || point2 == item;
    }

    public Point GetOtherPoint(Point item)
    {
        if (point1 == item)
        {
            return point2;
        }
        if (point2 == item)
        {
            return point1;
        }
        //Debug.LogError("GetOtherPoint(Point) == null");
        return null;
    }
}