using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomHelpFunc {

    private static RoomHelpFunc instance;
    public static RoomHelpFunc Instance
    {
        get
        {
            if (instance == null) instance = new RoomHelpFunc();
            return instance;
        }
    }

    private OriginalInputData data
    {
        get {
            return OriginalInputData.Instance;
        }
    }
    ///搜索字典
    private Dictionary<Point, List<Point>> searchableDic = new Dictionary<Point, List<Point>>();
    private LineHelpFunc linefunc = LineHelpFunc.Instance;
    private WallHelpFunc wallfunc = WallHelpFunc.Instance;


    private List<RoomData> roomList = new List<RoomData>();
    private List<WallData> getroomhelp = new List<WallData>();

    private List<Point> temp = new List<Point>();
    private List<WallData> farstWallList = new List<WallData>();

    //计算半高墙用 没有其他用途
    private List<RoomData> maxAngleRooms
    {
        get {
            return OriginalInputData.Instance.maxAngleRooms;
        }
        set {
            OriginalInputData.Instance.maxAngleRooms = value;
        }
    }

    /// <summary>
    /// 强制重新计算所有room
    /// </summary>
    public void ForceRefreshRoomData(OriginalInputData data)
    {
        roomList.Clear();
        roomList.InsertRange(0, data.roomList);
        data.roomList.Clear();
        data.roomList = GetRooms(data.wallList);
        for (int i = 0; i < data.roomList.Count; i++)
        {
            RoomData room = data.roomList[i];
            for (int k = 0; k < roomList.Count; k++)
            {
                RoomData oldRoom = roomList[k];
                if (IsRoomComeFrom(room, oldRoom))
                {
                    //room.guid = oldRoom.guid;
                    room.name = oldRoom.name;
                    room.type = oldRoom.type;
                    room.ceilingHeight = oldRoom.ceilingHeight;
                    room.floor = new MaterialData();
                    room.floor.guid = oldRoom.floor.guid;
                    room.floor.id = oldRoom.floor.id;
                    room.floor.hide = oldRoom.floor.hide;
                    room.floor.color = oldRoom.floor.color;
                    room.floor.offsetX = oldRoom.floor.offsetX;
                    room.floor.offsetY = oldRoom.floor.offsetY;
                    room.floor.rotation = oldRoom.floor.rotation;
                    room.floor.seekId = oldRoom.floor.seekId;
                    room.floor.textureURI = oldRoom.floor.textureURI;
                    room.floor.tileSize_x = oldRoom.floor.tileSize_x;
                    room.floor.tileSize_y = oldRoom.floor.tileSize_y;
                    room.ceiling = new MaterialData();
                    room.ceiling.guid = oldRoom.ceiling.guid;
                    room.ceiling.id = oldRoom.ceiling.id;
                    room.ceiling.hide = oldRoom.ceiling.hide;
                    room.ceiling.color = oldRoom.ceiling.color;
                    room.ceiling.offsetX = oldRoom.ceiling.offsetX;
                    room.ceiling.offsetY = oldRoom.ceiling.offsetY;
                    room.ceiling.rotation = oldRoom.ceiling.rotation;
                    room.ceiling.seekId = oldRoom.ceiling.seekId;
                    room.ceiling.textureURI = oldRoom.ceiling.textureURI;
                    room.ceiling.tileSize_x = oldRoom.ceiling.tileSize_x;
                    room.ceiling.tileSize_y = oldRoom.ceiling.tileSize_y;
                }
            }
        }
        roomList.Clear();

        maxAngleRooms.Clear();
        maxAngleRooms = GetMaxAngleRooms(data.wallList);
    }

    private bool IsRoomComeFrom(RoomData room, RoomData oldRoom)
    {
        for (int i = 0; i < oldRoom.sideList.Count; i++)
        {
            WallSideData oldSide = oldRoom.sideList[i];
            if (room.sideList.Contains(oldSide))
            {
                return true;
            }

            //WallData oldWall = oldSide.targetWall;

        }

        return false;
    }

    /// <summary>
    /// 找到walls围成的房间
    /// </summary>
    public RoomData GetRoom(List<Point> points)
    {
        if (points == null || points.Count < 3) return null;
        RoomData room1 = data.GetRoom(points[0], points[1]);
        RoomData room2 = data.GetRoom(points[1], points[0]);
        if (room1 == null && room2 == null) return null;
        bool isRoom1 = room1 != null ? true : false;
        bool isRoom2 = room2 != null ? true : false;
        for (int i = 1; i < points.Count; i++)
        {
            Point point1 = points[i];
            Point point2 = points[(i + 1) % points.Count];
            RoomData roomk1 = data.GetRoom(point1, point2);
            RoomData roomk2 = data.GetRoom(point2, point1);
            if (isRoom1 == true) isRoom1 = roomk1 == room1 || roomk2 == room1;
            if (isRoom2 == true) isRoom2 = roomk1 == room2 || roomk2 == room2;
            if (isRoom1 == false && isRoom2 == false) return null;
        }
        if (isRoom1) return room1;
        if (isRoom2) return room2;
        return null;
    }
    /// <summary>
    /// 找到walls围成的房间
    /// </summary>
    public RoomData GetRoom(List<WallData> walls)
    {
        if (walls.Count == 0 || walls == null)
        {
            return null;
        }
        getroomhelp.Clear();
        getroomhelp.InsertRange(0, walls);
        WallData wall0 = getroomhelp[0];
        RoomData room1 = data.GetRoom(wall0.point1, wall0.point2); 
        RoomData room2 = data.GetRoom(wall0.point2, wall0.point1);
        bool isRoom1 = room1 != null ? true : false;
        bool isRoom2 = room2 != null ? true : false;
        if (isRoom1 == false && isRoom2 == false) return null;
        for (int k = 0; k < getroomhelp.Count; k++)
        {
            WallData wallk = getroomhelp[k];
            RoomData roomk1 = data.GetRoom(wallk.point1, wallk.point2);
            RoomData roomk2 = data.GetRoom(wallk.point2, wallk.point1);
            if (isRoom1 == true) isRoom1 = roomk1 == room1 || roomk2 == room1;
            if (isRoom2 == true) isRoom2 = roomk1 == room2 || roomk2 == room2;
            if (isRoom1 == false && isRoom2 == false) return null;
        }
        if (isRoom1) return room1;
        if (isRoom2) return room2;
        return null;
    }
    /// <summary>
    /// 计算房间用
    /// </summary>
    public RoomData GetRoom(WallData wall)
    {
        RoomData data = GetRoom(wall.point1, wall.point2);
        if (data == null)
        {
            data = GetRoom(wall.point2, wall.point1);
        }
        return data;
    }
    /// <summary>
    /// 计算房间用
    /// </summary>
    public RoomData GetRoom(Point pointFrom, Point pointTo)
    {
        searchableDic.Clear();
        //string str = "\n minAnglePointList From " + pointFrom.id + " to " + pointTo.id + ":";
        //Debug.LogTest(str);
        List<Point> minAnglePointList = SearchMinAngleList(pointFrom, pointTo);
        //str += "{";
        //for (int i = 0; i < minAnglePointList.Count; i++)
        //{
        //    str += " " + minAnglePointList[i].id;
        //}
        //str += "}";
        //Debug.LogTest(str);
        return GetRoom(pointFrom, minAnglePointList);
    }

    public List<RoomData> GetCurrentRooms(WallData wall)
    {
        List<RoomData> list = new List<RoomData>();
        for (int i = 0; i < data.roomList.Count; i++)
        {
            RoomData roomi = data.roomList[i];
            if (roomi.sideList.Contains(wall.point1To2Data))
            {
                list.Add(roomi);
            }
            else if (roomi.sideList.Contains(wall.point2To1Data))
            {
                list.Add(roomi);
            }
        }
        return list;
    }
    /// <summary>
    /// 根据现有的墙找到所有的房间
    /// </summary>
    public List<RoomData> GetRooms(List<WallData> wallList)
    {
        List<RoomData> list = new List<RoomData>();
        for (int i = 0; i < wallList.Count; i++)
        {
            WallData wall = wallList[i];
            Point pointFrom = wall.point1;
            Point pointTo = wall.point2;
            if (RoomListContainWallSide(list, pointFrom, pointTo) == false)
            {
                RoomData room = GetRoom(pointFrom, pointTo);
                if (room != null)
                {
                    //Debug.LogTest("添加房间" + list.Count);
                    list.Add(room);
                }
            }

            pointFrom = wall.point2;
            pointTo = wall.point1;
            if (RoomListContainWallSide(list, pointFrom, pointTo) == false)
            {
                RoomData room = GetRoom(pointFrom, pointTo);
                if (room != null)
                {
                    //Debug.LogTest("添加房间"+ list.Count);
                    list.Add(room);
                }
            }
        }
        return list;
    }

    public WallSideData GetNearSide(RoomData side1OnRoom, WallSideData fromSide, bool FOrT)
    {
        int index = side1OnRoom.sideList.IndexOf(fromSide);
        if (FOrT == true)
        {
            index -= 1;
        }
        else {
            index += 1;
        }
        return side1OnRoom.sideList[(index + side1OnRoom.sideList.Count) % side1OnRoom.sideList.Count];
    }

    public WallSideData GetNearCrossSide(RoomData room, WallSideData fromSide, bool FOrT, List<Point> list)
    {
        WallSideData firstnearSide = GetNearSide(room, fromSide, FOrT);
        WallSideData nearSide = firstnearSide;
        temp.Clear();
        Point fromPoint;
        if (fromSide.forward == FOrT)
        {
            fromPoint = fromSide.targetWall.point1;
        }
        else
        {
            fromPoint = fromSide.targetWall.point2;
        }
        temp.Add(fromPoint);
        while (wallfunc.IsParallel(nearSide.targetWall, fromSide.targetWall) == true)
        {
            Point toPos = nearSide.targetWall.GetOtherPoint(fromPoint);
            if (toPos == null)
            {
                int index = room.pointList.IndexOf(fromPoint);
                toPos = room.pointList[(index + 1)% room.pointList.Count];
                if (toPos == null)
                {
                    Debug.Log("to pos also null");
                    break;
                }
            }
            fromPoint = toPos;
            temp.Add(fromPoint);
            nearSide = GetNearSide(room, nearSide, FOrT);
            if (nearSide == fromSide) {
                Debug.LogError("IsParallel死循环");
                break;
            }
        }
        if (FOrT == true)
        {
            temp.Reverse();
        }
        list.InsertRange(list.Count, temp);
        return nearSide;
    }

    public void SetArea(GameObject gameObject, List<List<Vector2>> list, Vector2 v2s0)
    {
        Vector2 center = Vector2.zero;
        //计算多边形面积用 每个三角形面积的和
        float totalArea = 0;
        for (int i = 0; i < list.Count; i++)
        {
            List<Vector2> v2s = list[i];
            Vector2 oneCenter;
            float area = linefunc.GetArea(v2s, out oneCenter);
            center += oneCenter * area;
            totalArea += area;
        }
        center = totalArea < 0.001f ? v2s0 : center / totalArea + v2s0;
        gameObject.transform.localPosition = new Vector3(center.x, center.y, gameObject.transform.localPosition.z);
        Transform tran = gameObject.transform.FindChild("text");
        TextMesh textmesh = tran.GetComponent<TextMesh>();
        textmesh.text = "" + Mathf.RoundToInt(totalArea * 100) / 100f;
    }

    public List<Vector2> SetMesh(GameObject gameObject, RoomData room)
    {
        if (room.pointList == null || room.pointList.Count < 3)
        {
            Debug.LogWarning("pointList == null || pointList.Count < 3");
            return null;
        }
        List<Vector2> v2s = GetSideVertices(room);
        return v2s;
    }

    public List<Vector2> GetVectors3D(RoomData room)
    {
        if (room.pointList == null || room.pointList.Count < 3)
        {
            Debug.LogWarning("pointList == null || pointList.Count < 3");
            return null;
        }
        List<Vector2> v2s = GetSideVertices(room);
        return v2s;
    }

    /// <summary>
    /// 顶点网格数据
    /// </summary>
    private List<Vector2> GetVertices(RoomData room)
    {
        List<Vector2> tempList = new List<Vector2>();
        int len = room.pointList.Count;
        for (int i = 0; i < len; i++)
        {
            Vector2 v2 = room.pointList[i].pos;
            tempList.Add(v2);
        }
        return tempList;
    }

    public bool PonitInRoom(Point p)
    {
        for (int i = 0; i < data.roomList.Count; i++)
        {
            if (data.roomList[i].pointList.Contains(p))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 内边界网格数据
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private List<Vector2> GetSideVertices(RoomData room)
    {
        List<Vector2> tempList = new List<Vector2>();
        int len = room.sideList.Count;
        for (int i = 0; i < len; i++)
        {
            WallSideData side1 = room.sideList[i];
            WallSideData side2 = room.sideList[(i + 1) % len];
            Vector2 v2 = wallfunc.GetWallSideCrossPos(side1, side2);
            tempList.Add(v2);
        }
        return tempList;
    }

    private bool RoomListContainWallSide(List<RoomData> list, Point pointFrom, Point pointTo)
    {
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < list[i].sideList.Count; j++)
            {
                WallSideData side = list[i].sideList[j];
                if (side.forward == true && side.targetWall.point1 == pointFrom && side.targetWall.point2 == pointTo)
                {
                    //Debug.LogTest("room"+i + " contins" + pointFrom.id + pointTo.id);
                    return true;
                }
                if (side.forward == false && side.targetWall.point2 == pointFrom && side.targetWall.point1 == pointTo)
                {
                    //Debug.LogTest("room" + i + " contins" + pointFrom.id + pointTo.id);
                    return true;
                }
            }
        }
        return false;
    }

    private float GetTotalClockAngle(List<Point> minAnglePointList)
    {
        if (minAnglePointList.Count < 3)
        {
            return 720;
        }
        float totalAngle = 0;
        for (int i = 0; i < minAnglePointList.Count; i++)
        {
            Point point1 = minAnglePointList[i];
            Point point2 = minAnglePointList[(i + 1) % minAnglePointList.Count];
            Point point3 = minAnglePointList[(i + 2) % minAnglePointList.Count];
            float angle = linefunc.GetClockAngle(point1.pos, point2.pos, point3.pos);
            totalAngle += angle;
        }
        return totalAngle;
    }

    public RoomData GetRoom(Point pointStart, List<Point> minAnglePointList)
    {
        minAnglePointList.Insert(0, pointStart);

        if (minAnglePointList.Count < 3)
        {
            return null;
        }

        ///判断围成多边形的内角和是否大于n*180  内角和(n-2)*180 外角和(n+2)*180;
        float totalAngle = GetTotalClockAngle(minAnglePointList);
        if (totalAngle > 180 * minAnglePointList.Count - 300f)
        {
            //Debug.LogTest("内角和"+ totalAngle + "大于n*180");
            return null;
        }
        RoomData room = new RoomData();
        room.pointList = minAnglePointList;
        for (int i = 0; i < minAnglePointList.Count; i++)
        {
            Point pointFrom = minAnglePointList[i];
            Point pointTo = minAnglePointList[(i + 1) % minAnglePointList.Count];
            WallData wall = wallfunc.GetWallData(pointFrom, pointTo);
            if (wall == null)
            {
                Debug.Log("wall == null"+pointFrom.pos + "/" + pointTo.pos);
                continue;
            }

            if (wall.point1 == pointFrom)
            {
                room.sideList.Add(wall.point1To2Data);
            }
            else
            {
                room.sideList.Add(wall.point2To1Data);
            }
        }
        //SetRoofAndFloor(room);
        return room;
    }

    private void SetRoofAndFloor(RoomData room)
    {
        //room.floor.target = room;
        //room.roof.target = room;
        //room
    }

    /// <summary>
    /// 顺时针角最小的点
    /// </summary>
    private List<Point> SearchMinAngleList(Point pointFrom, Point pointTo)
    {
        AddFasle(pointFrom, pointTo);
        List<Point> list = new List<Point>();
        list.Add(pointTo);
        //Debug.LogTest("add " + pointTo.id);
        LoopSearch(pointFrom, list, pointFrom, pointTo);
        return list;
    }

    private void LoopSearch(Point start, List<Point> list, Point pointFrom, Point pointTo)
    {
        Point minAnglePoint = SearchMinAnglePoint(pointFrom, pointTo);
        if (minAnglePoint != null)
        {
            //Debug.LogTest("minAnglePoint " + minAnglePoint.id);
            AddFasle(pointTo, minAnglePoint);
        }
        else
        {
            //Debug.LogTest("minAnglePoint = null");
        }
        if (minAnglePoint == null || list.Contains(minAnglePoint) == true)
        {
            //Debug.LogTest("remove " + list[list.Count-1].id);
            list.RemoveAt(list.Count - 1);
            if (list.Count == 0)
            {
                //Debug.LogTest("没有循环");
                return;
            }
            else if (list.Count == 1)
            {
                LoopSearch(start, list, start, list[0]);
            }
            else
            {
                LoopSearch(start, list, list[list.Count - 2], list[list.Count - 1]);
            }
        }
        else
        {
            //AddFasle(pointTo, minAnglePoint);
            if (minAnglePoint == start)
            {
                //Debug.LogTest("有循环");
                return;
            }
            list.Add(minAnglePoint);
            //Debug.LogTest("add " + minAnglePoint.id);
            LoopSearch(start, list, pointTo, minAnglePoint);
        }
    }

    /// <summary>
    /// 选相邻点中最小顺时针角的点
    /// </summary>
    /// <param name="pointFrom"></param>
    /// <param name="pointTo"></param>
    /// <returns></returns>
    private Point SearchMinAnglePoint(Point pointFrom, Point pointTo)
    {
        Point nearest = null;
        float minAngle = 400;
        List<Point> list = data.GetNearestPoints(pointTo);
        for (int i = 0; i < list.Count; i++)
        {
            Point near = list[i];
            if (near == pointFrom || GetSearchable(pointTo, near) == false)
            {
                //Debug.LogTest("near " + near.id + " 不可搜索");
                continue;
            }
            float angle = linefunc.GetClockAngle(pointFrom.pos, pointTo.pos, near.pos);
            //Debug.LogTest("angle "+ pointFrom.id + " " + pointTo.id + " " + near.id + " = " + angle);
            if (angle < minAngle)
            {
                nearest = near;
                ////之前大意判断过后忘记赋值
                minAngle = angle;
            }
            //Debug.LogTest("nearest " + nearest.id);
        }
        return nearest;
    }

    private bool GetSearchable(Point pointFrom, Point pointTo)
    {
        if (searchableDic.ContainsKey(pointFrom) && searchableDic[pointFrom].Contains(pointTo))
        {
            return false;
        }
        return true;
    }

    private void AddFasle(Point point1, Point point2)
    {
        //Debug.LogTest("AddFasle " + point1.id+" "+point2.id);
        if (searchableDic.ContainsKey(point1) == false)
        {
            searchableDic.Add(point1, new List<Point>());
        }
        if (searchableDic[point1].Contains(point2) == false)
        {
            searchableDic[point1].Add(point2);
        }
    }




    #region 计算最大包围的
    /// <summary>
    /// 最大包围的
    /// </summary>
    private List<RoomData> GetMaxAngleRooms(List<WallData> wallList)
    {
        List<RoomData> list = new List<RoomData>();
        for (int i = 0; i < wallList.Count; i++)
        {
            WallData wall = wallList[i];
            Point pointFrom = wall.point1;
            Point pointTo = wall.point2;
            if (RoomListContainWallSide(list, pointFrom, pointTo) == true && RoomListContainWallSide(list, pointTo, pointFrom) == true)
            {
                continue;
            }
            if (RoomListContainWallSide(list, pointFrom, pointTo) == false)
            {
                RoomData room = GetMaxAnleRoom(pointFrom, pointTo);
                if (room == null)
                {

                }
                else
                {
                    list.Add(room);

                }
            }
            if (RoomListContainWallSide(list, pointTo, pointFrom) == false)
            {
                RoomData room = GetMaxAnleRoom(pointTo, pointFrom);
                if (room != null)
                {
                    list.Add(room);
                }
            }
        }
        return list;
    }

    private RoomData GetMaxAnleRoom(Point pointFrom, Point pointTo)
    {
        searchableDic.Clear();
        List<Point> maxAnglePointList = SearchMaxAngleList(pointFrom, pointTo);
        return GetMaxAngleRoom(pointFrom, maxAnglePointList);
    }

    private List<Point> SearchMaxAngleList(Point pointFrom, Point pointTo)
    {
        AddFasle(pointFrom, pointTo);
        List<Point> list = new List<Point>();
        list.Add(pointTo);
        LoopSearch(pointFrom, list, pointFrom, pointTo);
        return list;
    }
    public RoomData GetMaxAngleRoom(Point pointStart, List<Point> maxAnglePointList)
    {
        maxAnglePointList.Insert(0, pointStart);

        if (maxAnglePointList.Count < 3)
        {
            return null;
        }

        ///判断围成多边形的内角和是否大于n*180  内角和(n-2)*180 外角和(n+2)*180;
        float totalAngle = GetTotalClockAngle(maxAnglePointList);
        if (totalAngle < 180 * maxAnglePointList.Count - 300f)
        {
            //Debug.LogTest("内角和"+ totalAngle + "大于n*180");
            return null;
        }
        RoomData room = new RoomData();
        room.pointList = maxAnglePointList;
        for (int i = 0; i < maxAnglePointList.Count; i++)
        {
            Point pointFrom = maxAnglePointList[i];
            Point pointTo = maxAnglePointList[(i + 1) % maxAnglePointList.Count];
            WallData wall = wallfunc.GetWallData(pointFrom, pointTo);
            if (wall == null)
            {
                Debug.Log("wall == null" + pointFrom.pos + "/" + pointTo.pos);
                continue;
            }

            if (wall.point1 == pointFrom)
            {
                room.sideList.Add(wall.point1To2Data);
            }
            else
            {
                room.sideList.Add(wall.point2To1Data);
            }
        }
        //SetRoofAndFloor(room);
        return room;
    }


    public bool OnMaxAngleRoom(WallData walldata)
    {
        for (int i = 0; i < maxAngleRooms.Count; i++)
        {
            RoomData room = maxAngleRooms[i];
            if (room.sideList.Contains(walldata.point1To2Data))
            {
                return true;
            }
            if (room.sideList.Contains(walldata.point2To1Data))
            {
                return true;
            }
        }

        return false;
    }

    public List<WallData> GetFarstWalls(Camera camera)
    {
        farstWallList.Clear();
        for (int i = 0; i < maxAngleRooms.Count; i++)
        {
            RoomData room = maxAngleRooms[i];
            GetFast(room.pointList, camera, farstWallList);
        }
        return farstWallList;
        //camera
    }

    private List<float> ylist = new List<float>();
    private void GetFast(List<Point> pointList, Camera camera, List<WallData> farstWallList)
    {
        //找到映射到屏幕的左右最远的两个点
        float min = float.MaxValue;
        float max = float.MinValue;
        int minP = 0;
        int maxP = 0;
        ylist.Clear();
        for (int i = 0; i < pointList.Count; i++)
        {
            Vector3 worldPos = GetVector3(pointList[i].pos);
            Vector3 screenPos = camera.WorldToScreenPoint(worldPos);
            ylist.Add(screenPos.y);
            if (min > screenPos.x)
            {
                min = screenPos.x;
                minP = i;
            }
            if (max < screenPos.x)
            {
                max = screenPos.x;
                maxP = i;
            }
        }

        ////找到映射到屏幕的左右最远的两个点之间距相机较远的其他连通点
        int from = minP < maxP ? minP : maxP;
        int to = minP < maxP ? maxP : minP;
        float y1 = 0;
        for (int i = from; i < to; i++)
        {
            y1 += ylist[i];
        }
        float y2 = 0;
        for (int i = from; i > to - pointList.Count; i--)
        {
            int index = i;
            if (index < 0) index += pointList.Count;
            y2 += ylist[index];
        }
        if (y1 / (to - from) >= y2 / (from + pointList.Count - to))
        {
            for (int i = from; i < to; i++)
            {
                WallData wall = data.GetWall(pointList[i], pointList[i + 1]);
                farstWallList.Add(wall);
            }
        }
        else
        {
            for (int i = from; i > to - pointList.Count; i--)
            {
                int index = i;
                if (index < 0) index += pointList.Count;
                int index2 = index - 1;
                if (index2 < 0) index2 += pointList.Count;
                WallData wall = data.GetWall(pointList[index], pointList[index2]);
                farstWallList.Add(wall);
            }
        }
    }

    private Vector2 GetVector2(Vector3 position)
    {
        return Vector2.right * position.x + Vector2.up * position.z;
    }

    private Vector3 GetVector3(Vector2 pos)
    {
        return Vector3.right * pos.x + Vector3.forward * pos.y;
    }

    #endregion
}
