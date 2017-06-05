using System;
using System.Collections.Generic;
using UnityEngine;

public class WallHelpFunc {

    private static WallHelpFunc instance;
    public static WallHelpFunc Instance
    {
        get
        {
            if (instance == null) instance = new WallHelpFunc();
            return instance;
        }
    }

    private OriginalInputData data
    {
        get
        {
            return OriginalInputData.Instance;
        }
    }
    public DefaultSettings defaultSettings
    {
        get {
            return DefaultSettings.Instance;
        }
    }
    private LineHelpFunc linefunc
    {
        get {
            return LineHelpFunc.Instance;
        }
    }
    private InputHelperData helperData
    {
        get{
            return InputHelperData.Instance;
        }
    }
    private RoomHelpFunc roomfunc
    {
        get { return RoomHelpFunc.Instance; }
    }
    public List<Vector2> GetWallVerticesLocalPos(List<Vector2> arr, float width, float size)
    {
        List<Vector2> tempList = new List<Vector2>();
        for (int i = 0; i < arr.Count; i++)
        {
            tempList.Add(Vector2.zero);
        }

        float dis1 = (arr[5] - arr[0]).magnitude;
        float dis2 = (arr[4] - arr[0]).magnitude;
        float dis4 = (arr[2] - arr[0]).magnitude;
        float dis5 = (arr[1] - arr[0]).magnitude;

        float value1 = dis1 * dis1 - width * width / 4;
        float value2 = dis2 * dis2 - width * width / 4;
        float value4 = dis4 * dis4 - width * width / 4;
        float value5 = dis5 * dis5 - width * width / 4;

        tempList[3] = new Vector2(0, (arr[3] - arr[0]).magnitude);
        tempList[5] = new Vector2(width / 2f, value5 > 0 ? Mathf.Sqrt(value5) : 0);
        tempList[4] = new Vector2(width / 2f, value4 > 0 ? Mathf.Sqrt(value4) : 0);
        tempList[2] = new Vector2(-width / 2f, value2 > 0 ? Mathf.Sqrt(value2) : 0);
        tempList[1] = new Vector2(-width / 2f, value1 > 0 ? Mathf.Sqrt(value1) : 0);

        float angle1 = linefunc.GetCrossAngle(arr[5] - arr[0], arr[3] - arr[0]);
        float angle5 = linefunc.GetCrossAngle(arr[1] - arr[0], arr[3] - arr[0]);

        if (angle1 >= 90)
        {
            tempList[1] = new Vector2(tempList[1].x, -tempList[1].y);
        }
        if (angle5 >= 90)
        {
            tempList[5] = new Vector2(tempList[5].x, - tempList[5].y);
        }
        
        for (int i = 0; i < tempList.Count; i++)
        {
            tempList[i] = tempList[i] / size;
        }
        return tempList;
    }

    public Vector3 GetPos(WallSideData wallside)
    {
        WallData wall = wallside.targetWall;
        List<Vector2> v2s = GetWallVerticesPos(wall);
        Vector2 v21 = wallside.forward == true ? v2s[1] : v2s[4];
        Vector2 v22 = wallside.forward == true ? v2s[2] : v2s[5];
        float c = wallside.forward == true ? wall.c5 : wall.c6;
        Vector3 v2start = linefunc.GetDisPoint(wall.a, wall.b, c, v21.x, v21.y);
        Vector3 v2end = linefunc.GetDisPoint(wall.a, wall.b, c, v22.x, v22.y);
        return (v2start + v2end) / 2;
    }

    public float GetSideLength(WallSideData wallside)
    {
        WallData wall = wallside.targetWall;
        List<Vector2> v2s = GetWallVerticesPos(wall);
        Vector2 v21 = wallside.forward == true ? v2s[1] : v2s[4];
        Vector2 v22 = wallside.forward == true ? v2s[2] : v2s[5];
        float c = wallside.forward == true ? wall.c5 : wall.c6;
        Vector3 v2start = linefunc.GetDisPoint(wall.a, wall.b, c, v21.x, v21.y);
        Vector3 v2end = linefunc.GetDisPoint(wall.a, wall.b, c, v22.x, v22.y);
        float angle = linefunc.GetAngle(v2end - v2start);
        float dis = Mathf.RoundToInt(Vector2.Distance(v2start, v2end) * 1000);
        dis = dis * (int)defaultSettings.DefaultUnit / 1000;
        return dis;
    }

    public List<WallData> GetWallList(Point target)
    {
        List<WallData> list = new List<WallData>();
        for (int i = 0; i < data.wallList.Count; i++)
        {
            WallData item = data.wallList[i];
            if (item.Contines(target)) {
                list.Add(item);
            }
        }
        return list;
    }

    public bool IsParallel(WallData p1Wall, WallData target)
    {
        return linefunc.IsParallel(p1Wall.a, p1Wall.b, target.a, target.b);
    }

    /// <summary>
    /// Vector2[6]{point1, point1RightPos, point2LeftPos, point2, point2RightPos, point1LeftPos}
    /// </summary>
    public List<Vector2> GetWallVerticesPos(WallData wallData)
    {
        //计算point1左右两侧的边线点
        WallData[] Point1NearestAngleWall = GetNearestWall(wallData.point1, wallData.point2);
        //计算point2左右两侧的边线点
        WallData[] Point2NearestAngleWall = GetNearestWall(wallData.point2, wallData.point1);

        Vector2 point1LeftPos = Vector2.zero;
        Vector2 point1RightPos = Vector2.zero;
        if (Point1NearestAngleWall == null)
        {
            point1LeftPos = linefunc.GetDisPoint(wallData.a, wallData.b, wallData.c2, wallData.point1.pos.x, wallData.point1.pos.y);
            point1RightPos = linefunc.GetDisPoint(wallData.a, wallData.b, wallData.c1, wallData.point1.pos.x, wallData.point1.pos.y);
        }
        else
        {
            WallData minAngleWall = Point1NearestAngleWall[0];
            WallData maxAngleWall = Point1NearestAngleWall[1];

            if (wallData.point1 == minAngleWall.point1)
            {
                point1LeftPos = linefunc.GetTwoLineCrossPoint(wallData.a, wallData.b, wallData.c2, minAngleWall.a, minAngleWall.b, minAngleWall.c1, wallData.point1.pos);
            }
            else
            {
                point1LeftPos = linefunc.GetTwoLineCrossPoint(wallData.a, wallData.b, wallData.c2, minAngleWall.a, minAngleWall.b, minAngleWall.c2, wallData.point1.pos);
            }

            if (wallData.point1 == maxAngleWall.point1)
            {
                point1RightPos = linefunc.GetTwoLineCrossPoint(wallData.a, wallData.b, wallData.c1, maxAngleWall.a, maxAngleWall.b, maxAngleWall.c2, wallData.point1.pos);
            }
            else
            {
                point1RightPos = linefunc.GetTwoLineCrossPoint(wallData.a, wallData.b, wallData.c1, maxAngleWall.a, maxAngleWall.b, maxAngleWall.c1, wallData.point1.pos);
            }
        }

        Vector2 point2LeftPos = Vector2.zero;
        Vector2 point2RightPos = Vector2.zero;
        if (Point2NearestAngleWall == null)
        {
            point2LeftPos = linefunc.GetDisPoint(wallData.a, wallData.b, wallData.c1, wallData.point2.pos.x, wallData.point2.pos.y);
            point2RightPos = linefunc.GetDisPoint(wallData.a, wallData.b, wallData.c2, wallData.point2.pos.x, wallData.point2.pos.y);
        }
        else
        {
            WallData minAngleWall = Point2NearestAngleWall[0];
            WallData maxAngleWall = Point2NearestAngleWall[1];

            if (wallData.point2 == minAngleWall.point1)
            {
                point2LeftPos = linefunc.GetTwoLineCrossPoint(wallData.a, wallData.b, wallData.c1, minAngleWall.a, minAngleWall.b, minAngleWall.c1, wallData.point2.pos);
            }
            else
            {
                point2LeftPos = linefunc.GetTwoLineCrossPoint(wallData.a, wallData.b, wallData.c1, minAngleWall.a, minAngleWall.b, minAngleWall.c2, wallData.point2.pos);
            }

            if (wallData.point2 == maxAngleWall.point1)
            {
                point2RightPos = linefunc.GetTwoLineCrossPoint(wallData.a, wallData.b, wallData.c2, maxAngleWall.a, maxAngleWall.b, maxAngleWall.c2, wallData.point2.pos);
            }
            else
            {
                point2RightPos = linefunc.GetTwoLineCrossPoint(wallData.a, wallData.b, wallData.c2, maxAngleWall.a, maxAngleWall.b, maxAngleWall.c1, wallData.point2.pos);
            }
        }

        List<Vector2> tempList = new List<Vector2>();
        tempList.Add(wallData.point1.pos);
        tempList.Add(point1RightPos);
        tempList.Add(point2LeftPos);
        tempList.Add(wallData.point2.pos);
        tempList.Add(point2RightPos);
        tempList.Add(point1LeftPos);
        ///point1
        ///加上 整合 从point1到point2右边的两点 按顺序
        ///加上 point2
        ///加上 整合 从point2到point1右边的两点 按顺序
        ///组合成 逆时针 六维数组
        return tempList;
    }

    public Vector2 GetWallSideCrossPos(WallSideData side1, WallSideData side2)
    {
        float c1 = side1.forward == true ? side1.targetWall.c1 : side1.targetWall.c2;
        float c2 = side2.forward == true ? side2.targetWall.c1 : side2.targetWall.c2;
        Vector2 pos = side1.forward == true ? side1.targetWall.point2.pos : side1.targetWall.point1.pos;
        return linefunc.GetTwoLineCrossPoint(side1.targetWall.a, side1.targetWall.b, c1, side2.targetWall.a, side2.targetWall.b, c2, pos);
    }

    private WallData[] GetNearestWall(Point point, Point other)
    {
        float maxAngle = -1;
        float minAngle = 361;
        WallData maxAngleWall = null;
        WallData minAngleWall = null;
        List<Point> list = data.GetNearestPoints(point);
        for (int i = 0; i < list.Count; i++)
        {
            Point item = list[i];
            if (item == other)
            {
                continue;
            }

            WallData temp = GetWallData(point, item);
            if (temp == null)
            {
                Debug.LogWarning("GetWallData(point, wallData.point2) == null; i = " + i);
                continue;
            }
            float angle = GetAngle(other, point, temp);
            if (angle > maxAngle)
            {
                maxAngleWall = temp;
                maxAngle = angle;
            }
            if (angle < minAngle)
            {
                minAngleWall = temp;
                minAngle = angle;
            }
        }

        if (minAngleWall == maxAngleWall && maxAngleWall == null)
        {
            //没有相临墙
            return null;
        }

        return new WallData[2] { minAngleWall, maxAngleWall };

    }

    private float GetAngle(Point end, Point start, WallData temp)
    {
        float angle1 = linefunc.GetAngle(end.pos - start.pos);
        Point tempEnd = temp.point1;
        if (tempEnd == start)
        {
            tempEnd = temp.point2;
        }
        float angle2 = linefunc.GetAngle(tempEnd.pos - start.pos);
        float angle = angle2 - angle1;
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    public WallData GetWallData(Point tempPoint1, Point tempPoint2)
    {
        return data.GetWall(tempPoint1, tempPoint2);
    }

    public WallData GetHelpWallData(Point pointFrom, Point pointTo)
    {
        WallData wallData = null;
        for (int i = 0; i < helperData.helpLineList.Count; i++)
        {
            WallData item = data.wallList[i];
            if (item.equle(pointFrom, pointTo))
            {
                wallData = item;
            }
        }
        return wallData;
    }
    public bool PointOnWall(Point p, WallData wall)
    {
        return PointOnWall(p.pos, wall);
    }
    public bool PointOnWall(Vector2 v2, WallData wall)
    {
        float wallLen = Vector2.Distance(wall.point1.pos, wall.point2.pos);
        float len1 = Vector2.Distance(wall.point1.pos, v2);
        float len2 = Vector2.Distance(wall.point2.pos, v2);
        if (len1 + len2 - wallLen > 0.01f)
        {
            return false;
        }
        return true;
    }

    //public void CombineWall(List<WallData> wallList)
    //{
    //    if (wallList == null || wallList.Count == 0)
    //    {
    //        return;
    //    }

    //    for (int i = 0; i < wallList.Count; i++)
    //    {
    //        CombineWall(wallList[i], false);
    //    }

    //    roomfunc.ForceRefreshRoomData(data);
    //    View2D.Instance.RefreshView(); 
    //}


    public void WallPointOnOther(WallData wall)
    {

    }

    public void WallSplitOther(WallData wall)
    {
        
    }

    public void WallOnPoint(WallData wall)
    {

    }

    public void WallBeSplit(WallData wall)
    {

    }
}
