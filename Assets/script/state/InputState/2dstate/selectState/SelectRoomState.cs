using UnityEngine;
using System.Collections;
using foundation;
using System;
using System.Collections.Generic;

public class SelectRoomState : SelectState
{
    public const string NAME = "SelectRoomState";
    public SelectRoomState():base(NAME)
    {
        roomMachine = SelectRoom_StateMachine.Instance;
        roomMachine.selectRoomState = this;
    }

    public RoomData _target;
    public RoomData target
    {
        get {
            return _target;
        }
        set
        {
            _target = value;
            onTargetChange(_target);
        }
    }
    private Vector2 oldPos = Vector2.zero;
    private Vector2 startPos = Vector2.zero;
    private bool moved = false;
    private Vector2 offset;

    private List<Vector2> resetStartPosList = new List<Vector2>();
    private List<Vector2> startPosList = new List<Vector2>();

    //辅助计算
    private List<WallData> itemList = new List<WallData>();
    private SelectRoom_StateMachine roomMachine;
    private List<Point> resetList = new List<Point>();//把list调节成一个交叉点为第一个点的包围

    public override void enter()
    {
        base.enter();
        target = (viewTarget as RoomView).data;
        optionsController.selectMachine = roomMachine;
        roomMachine.setState(EditTypeOnSelect.Free);
        awake();
    }

    public override void exit()
    {
        roomMachine.setState(EditTypeOnSelect.Free);
        base.exit();
    }

    protected override GoodsType getGoodsType()
    {
        return GoodsType.Room;
    }

    private void awake()
    {
        startPosList.Clear();
        Vector3 inputPos;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            inputPos = Input.GetTouch(0).position;
        }
        else
        {
            inputPos = Input.mousePosition;
        }
        oldPos = startPos = GetScreenToWorldPos(inputPos);
        for (int i = 0; i < target.pointList.Count; i++)
        {
            startPosList.Add(target.pointList[i].pos);
        }
        moved = false;
    }
    
    public override void mUpdate()
    {
        base.mUpdate();

        if (changed) { return; }
        if (uguiHitUI.uiHited == true) return;
        if (canMove == false)
        {
            if (Input.GetMouseButtonUp(0) == false) return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            awake();
            saved = false;
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            Vector2 pos = GetScreenToWorldPos(Input.mousePosition);
            float minDis = 0.06f;
            ///房间的第index个点有吸附 或者index和（index+1）%count的那堵墙有吸附
            int minDisPointIndex = -1;
            ///边界点靠近点
            Point mindisPoint = null;
            ///边界点靠近墙
            WallData mindisWall = null;
            ///墙靠近边界点 
            Point mindisWallToPoint = null;

            bool roomChanged = false;

            #region pos != oldPos
            if (pos != oldPos)
            {
                if (saved == false) saved = true;
                moved = true;
                offset = pos - startPos;

                #region//重置点顺序
                int minCopyIndex = 0;
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point pointi = target.pointList[i];
                    List<Point> nearList = data.GetNearestPoints(pointi);
                    if (nearList.Count > 2)
                    {
                        minCopyIndex = i;
                        break;
                    }
                }
                if (minCopyIndex != 0)
                {
                    resetList.Clear();
                    for (int i = minCopyIndex; i < target.pointList.Count + minCopyIndex; i++)
                    {
                        resetList.Add(target.pointList[i % target.pointList.Count]);
                    }
                    target.pointList.Clear();
                    target.pointList.InsertRange(0, resetList);

                    resetStartPosList.Clear();
                    for (int i = minCopyIndex; i < startPosList.Count + minCopyIndex; i++)
                    {
                        resetStartPosList.Add(startPosList[i % startPosList.Count]);
                    }
                    startPosList.Clear();
                    startPosList.InsertRange(0, resetStartPosList);
                }
                #endregion

                #region 纠正offset
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point pointi = target.pointList[i];
                    Point pointF = target.pointList[(i - 1 + target.pointList.Count) % target.pointList.Count];
                    Point pointT = target.pointList[(i + 1) % target.pointList.Count];
                    List<Point> nearList = data.GetNearestPoints(pointi);
                    if (nearList.Count > 2)
                    {
                        roomChanged = true;
                        WallData wallF = data.GetWall(pointi, pointF);
                        WallData wallT = data.GetWall(pointi, pointT);
                        if (data.GetRoom(wallF.point1To2Data) == null || data.GetRoom(wallF.point2To1Data) == null) data.RemoveWall(wallF);
                        if (data.GetRoom(wallT.point1To2Data) == null || data.GetRoom(wallT.point2To1Data) == null) data.RemoveWall(wallT);

                        Point newPoint = new Point(pointi.pos);
                        data.AddPoint(newPoint);
                        data.AddWall(pointF, newPoint, wallF.height, wallF.width);
                        data.AddWall(newPoint, pointT, wallT.height, wallT.width);

                        target.pointList[i] = newPoint;

                        //去除房间移走后剩余线的共线的多余点
                        nearList = data.GetNearestPoints(pointi);
                        if (nearList.Count == 2)
                        {
                            CheckRemove(nearList[0], pointi, nearList[1]);
                        }

                        //去除选中房间共线的多余点
                        bool isRemoved = CheckRemove(pointF, newPoint, pointT);
                        if (isRemoved == true)
                        {
                            target.pointList.RemoveAt(i);
                            startPosList.RemoveAt(i);
                            i--;
                        }
                    }
                }
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    target.pointList[i].pos = startPosList[i] + offset;
                }

                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point point = target.pointList[i];
                    point.pos = startPosList[i] + offset;
                    for (int k = 0; k < data.pointList.Count; k++)
                    {
                        Point pointk = data.pointList[k];
                        if (target.pointList.Contains(pointk))
                        {
                            continue;
                        }
                        float sqrDis = (point.pos - pointk.pos).sqrMagnitude;
                        if (sqrDis < minDis)
                        {
                            minDisPointIndex = i;
                            mindisPoint = pointk;
                            minDis = sqrDis;
                            offset = pointk.pos - startPosList[i];
                        }
                    }
                }

                if (minDisPointIndex == -1)
                {
                    minDis = 0.2f;
                    for (int i = 0; i < target.pointList.Count; i++)
                    {
                        Point point = target.pointList[i];
                        for (int k = 0; k < data.wallList.Count; k++)
                        {
                            WallData wallk = data.wallList[k];
                            if (target.pointList.Contains(wallk.point1) && target.pointList.Contains(wallk.point2))
                            {
                                continue;
                            }
                            float dis = linefunc.GetDis(wallk.a, wallk.b, wallk.c, point.pos.x, point.pos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(wallk.a, wallk.b, wallk.c, point.pos.x, point.pos.y);
                                if (wallfunc.PointOnWall(disP, wallk))
                                {
                                    minDisPointIndex = i;
                                    mindisWall = wallk;
                                    minDis = dis;
                                    offset = disP - startPosList[i];
                                }
                            }
                        }
                    }
                }

                #region 墙吸附于点
                if (minDisPointIndex == -1)
                {
                    minDis = 0.2f;
                    for (int i = 0; i < target.pointList.Count; i++)
                    {
                        WallData wall = data.GetWall(target.pointList[i], target.pointList[(i + 1) % target.pointList.Count]);
                        for (int k = 0; k < data.pointList.Count; k++)
                        {
                            Point pointk = data.pointList[k];
                            if (target.pointList.Contains(pointk))
                            {
                                continue;
                            }
                            float dis = linefunc.GetDis(wall.a, wall.b, wall.c, pointk.pos.x, pointk.pos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(wall.a, wall.b, wall.c, pointk.pos.x, pointk.pos.y);
                                if (wallfunc.PointOnWall(disP, wall))
                                {
                                    minDisPointIndex = i;
                                    mindisWallToPoint = target.pointList[i];
                                    minDis = dis;
                                    offset = target.pointList[i].pos - startPosList[i] + pointk.pos - disP;
                                }
                            }
                        }
                    }
                }
                #endregion

                #endregion

                for (int i = 0; i < target.pointList.Count; i++)
                {
                    target.pointList[i].pos = startPosList[i] + offset;
                }
                oldPos = pos;
            }
            else if (Input.GetMouseButtonUp(0) == false)
            {
                return;
            }
            #endregion

            #region Input.GetMouseButtonUp
            if (Input.GetMouseButtonUp(0))
            {
                #region 合并公共点线面 点在线上切分 线交叉切分
                ///合并点
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point p = target.pointList[i];

                    for (int k = 0; k < data.pointList.Count; k++)
                    {
                        Point pointk = data.pointList[k];
                        if (pointk == p)
                        {
                            continue;
                        }
                        if (Vector2.Distance(pointk.pos, p.pos) < 0.1f)
                        {
                            data.CombinePoint(pointk, p);
                            i--;
                            //target.pointList[i] = p;
                            break;
                        }
                    }
                }
                ///选中房间点切分非选中房间线
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point p = target.pointList[i];

                    for (int k = 0; k < data.wallList.Count; k++)
                    {
                        WallData item = data.wallList[k];
                        if (item.Contines(p))
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(p, item))
                        {
                            data.CombinePointWall(p, item);
                            break;
                        }
                    }
                }

                ///选中房间线 切分非选中房间线（切分后交叉点已生成 即：后面点切分选中房间线 包含了 线切分选中房间线的部分）
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point p = target.pointList[i];
                    Point p2 = target.pointList[(i + 1) % target.pointList.Count];
                    WallData wall = data.GetWall(p, p2);
                    if (wall == null)
                    {
                        Debug.LogWarning("wall == null " + p.id + "/" + p2.id);
                        continue;
                    }
                    for (int k = 0; k < data.wallList.Count; k++)
                    {
                        WallData item = data.wallList[k];
                        if (item.Contines(p) || item.Contines(p2))
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(item.point1, wall) == true)
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(item.point2, wall) == true)
                        {
                            continue;
                        }
                        if(linefunc.GetCross(item, wall))
                        {
                            Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, wall, Vector2.zero);
                            Point point = new Point(crossP);
                            data.AddPoint(point);
                            data.CombinePointWall(point, item);
                            k--;
                        }
                    }
                }

                ///非选中房间点切分选中房间线
                itemList.Clear();
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    WallData item = data.GetWall(target.pointList[i], target.pointList[(i + 1) % target.pointList.Count]);
                    itemList.Add(item);
                }
                for (int i = 0; i < data.pointList.Count; i++)
                {
                    Point p = data.pointList[i];

                    for (int k = 0; k < itemList.Count; k++)
                    {
                        WallData itemk = itemList[k];
                        if (itemk.Contines(p))
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(p, itemk))
                        {
                            List<WallData> list = data.CombinePointWall(p, itemk);
                            itemList.RemoveAt(k);
                            itemList.InsertRange(k, list);
                            break;
                        }
                    }
                }

                #endregion

                roomfunc.ForceRefreshRoomData(data);
                target = roomfunc.GetRoom(itemList);
                if (target != null) awake();
            }

            #endregion

            if (roomChanged)
            {
                roomfunc.ForceRefreshRoomData(data);
                target = roomfunc.GetRoom(target.pointList);
                //if (target != null) awake();
            }

            RefreshView();

            if (target == null || data.Contins(target) == false)
            {
                Debug.LogWarning("target == null");
                setState(FreeState2D.NAME);
            }
        }
    }

    private bool CheckRemove(Point pointF, Point ifRemove, Point pointT)
    {
        WallData wallF = data.GetWall(pointF, ifRemove);
        WallData wallT = data.GetWall(pointT, ifRemove);
        bool isParallel = wallfunc.IsParallel(wallF, wallT);
        if (isParallel == true)
        {
            data.AddWall(pointF, pointT, wallF.height, wallF.width);
            data.RemoveWall(wallF);
            data.RemoveWall(wallT);
            data.RemovePoint(ifRemove);
        }
        return isParallel;
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();

        if (changed) return; 
        if (uguiHitUI.uiHited == true) return;
        if (Input.touchCount != 1) return;
        Touch touch = Input.GetTouch(0);
        bool inputUp = touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended;
        if (canMove == false)
        {
            if (inputUp == false) return;
        }

        if (touch.phase == TouchPhase.Began)
        {
            awake();
            saved = false;
        }
        else if (touch.phase != TouchPhase.Stationary)
        {
            Vector2 pos = GetScreenToWorldPos(touch.position);
            float minDis = 0.06f;
            ///房间的第index个点有吸附 或者index和（index+1）%count的那堵墙有吸附
            int minDisPointIndex = -1;
            ///边界点靠近点
            Point mindisPoint = null;
            ///边界点靠近墙
            WallData mindisWall = null;
            ///墙靠近边界点 
            Point mindisWallToPoint = null;

            bool roomChanged = false;

            #region pos != oldPos
            if (pos != oldPos)
            {
                if (saved == false) saved = true;
                moved = true;
                offset = pos - startPos;

                #region//重置点顺序
                int minCopyIndex = 0;
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point pointi = target.pointList[i];
                    List<Point> nearList = data.GetNearestPoints(pointi);
                    if (nearList.Count > 2)
                    {
                        minCopyIndex = i;
                        break;
                    }
                }
                if (minCopyIndex != 0)
                {
                    resetList.Clear();
                    for (int i = minCopyIndex; i < target.pointList.Count + minCopyIndex; i++)
                    {
                        resetList.Add(target.pointList[i % target.pointList.Count]);
                    }
                    target.pointList.Clear();
                    target.pointList.InsertRange(0, resetList);


                    resetStartPosList.Clear();
                    for (int i = minCopyIndex; i < startPosList.Count + minCopyIndex; i++)
                    {
                        resetStartPosList.Add(startPosList[i % startPosList.Count]);
                    }
                    startPosList.Clear();
                    startPosList.InsertRange(0, resetStartPosList);
                }
                #endregion

                #region 纠正offset

                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point pointi = target.pointList[i];
                    Point pointF = target.pointList[(i - 1 + target.pointList.Count) % target.pointList.Count];
                    Point pointT = target.pointList[(i + 1) % target.pointList.Count];
                    List<Point> nearList = data.GetNearestPoints(pointi);
                    if (nearList.Count > 2)
                    {
                        roomChanged = true;
                        WallData wallF = data.GetWall(pointi, pointF);
                        WallData wallT = data.GetWall(pointi, pointT);
                        if (data.GetRoom(wallF.point1To2Data) == null || data.GetRoom(wallF.point2To1Data) == null) data.RemoveWall(wallF);
                        if (data.GetRoom(wallT.point1To2Data) == null || data.GetRoom(wallT.point2To1Data) == null) data.RemoveWall(wallT);

                        Point newPoint = new Point(pointi.pos);
                        data.AddPoint(newPoint);
                        data.AddWall(pointF, newPoint, wallF.height, wallF.width);
                        data.AddWall(newPoint, pointT, wallT.height, wallT.width);

                        target.pointList[i] = newPoint;

                        //去除房间移走后剩余线的共线的多余点
                        nearList = data.GetNearestPoints(pointi);
                        if (nearList.Count == 2)
                        {
                            CheckRemove(nearList[0], pointi, nearList[1]);
                        }

                        //去除选中房间共线的多余点
                        bool isRemoved = CheckRemove(pointF, newPoint, pointT);
                        if (isRemoved == true)
                        {
                            target.pointList.RemoveAt(i);
                            startPosList.RemoveAt(i);
                            i--;
                        }
                    }
                }
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    target.pointList[i].pos = startPosList[i] + offset;
                }

                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point point = target.pointList[i];
                    point.pos = startPosList[i] + offset;
                    for (int k = 0; k < data.pointList.Count; k++)
                    {
                        Point pointk = data.pointList[k];
                        if (target.pointList.Contains(pointk))
                        {
                            continue;
                        }
                        float sqrDis = (point.pos - pointk.pos).sqrMagnitude;
                        if (sqrDis < minDis)
                        {
                            minDisPointIndex = i;
                            mindisPoint = pointk;
                            minDis = sqrDis;
                            offset = pointk.pos - startPosList[i];
                        }
                    }
                }

                if (minDisPointIndex == -1)
                {
                    minDis = 0.2f;
                    for (int i = 0; i < target.pointList.Count; i++)
                    {
                        Point point = target.pointList[i];
                        for (int k = 0; k < data.wallList.Count; k++)
                        {
                            WallData wallk = data.wallList[k];
                            if (target.pointList.Contains(wallk.point1) && target.pointList.Contains(wallk.point2))
                            {
                                continue;
                            }
                            float dis = linefunc.GetDis(wallk.a, wallk.b, wallk.c, point.pos.x, point.pos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(wallk.a, wallk.b, wallk.c, point.pos.x, point.pos.y);
                                if (wallfunc.PointOnWall(disP, wallk))
                                {
                                    minDisPointIndex = i;
                                    mindisWall = wallk;
                                    minDis = dis;
                                    offset = disP - startPosList[i];
                                }
                            }
                        }
                    }
                }

                #region 墙吸附于点
                if (minDisPointIndex == -1)
                {
                    minDis = 0.2f;
                    for (int i = 0; i < target.pointList.Count; i++)
                    {
                        WallData wall = data.GetWall(target.pointList[i], target.pointList[(i + 1) % target.pointList.Count]);
                        for (int k = 0; k < data.pointList.Count; k++)
                        {
                            Point pointk = data.pointList[k];
                            if (target.pointList.Contains(pointk))
                            {
                                continue;
                            }
                            float dis = linefunc.GetDis(wall.a, wall.b, wall.c, pointk.pos.x, pointk.pos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(wall.a, wall.b, wall.c, pointk.pos.x, pointk.pos.y);
                                if (wallfunc.PointOnWall(disP, wall))
                                {
                                    minDisPointIndex = i;
                                    mindisWallToPoint = target.pointList[i];
                                    minDis = dis;
                                    offset = target.pointList[i].pos - startPosList[i] + pointk.pos - disP;
                                }
                            }
                        }
                    }
                }
                #endregion

                #endregion

                for (int i = 0; i < target.pointList.Count; i++)
                {
                    target.pointList[i].pos = startPosList[i] + offset;
                }
                oldPos = pos;
            }
            #endregion

            #region touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                #region 合并公共点线面

                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point p = target.pointList[i];

                    for (int k = 0; k < data.pointList.Count; k++)
                    {
                        Point pointk = data.pointList[k];
                        if (pointk == p)
                        {
                            continue;
                        }
                        if (Vector2.Distance(pointk.pos, p.pos) < 0.1f)
                        {
                            data.CombinePoint(pointk, p);
                            i--;
                            //target.pointList[i] = p;
                            break;
                        }
                    }
                }
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point p = target.pointList[i];

                    for (int k = 0; k < data.wallList.Count; k++)
                    {
                        WallData item = data.wallList[k];
                        if (item.Contines(p))
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(p, item))
                        {
                            data.CombinePointWall(p, item);
                            break;
                        }
                    }
                }

                for (int i = 0; i < target.pointList.Count; i++)
                {
                    Point p = target.pointList[i];
                    Point p2 = target.pointList[(i + 1) % target.pointList.Count];
                    WallData wall = data.GetWall(p, p2);
                    if (wall == null)
                    {
                        Debug.LogError("wall == null " + p.id + "/" + p2.id);
                        continue;
                    }
                    for (int k = 0; k < data.wallList.Count; k++)
                    {
                        WallData item = data.wallList[k];
                        if (item.Contines(p) || item.Contines(p2))
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(item.point1, wall) == true)
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(item.point2, wall) == true)
                        {
                            continue;
                        }
                        if (linefunc.GetCross(item, wall))
                        {
                            Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, wall, Vector2.zero);
                            Point point = new Point(crossP);
                            data.AddPoint(point);
                            data.CombinePointWall(point, item);
                            k--;
                        }
                    }
                }

                itemList.Clear();
                for (int i = 0; i < target.pointList.Count; i++)
                {
                    WallData item = data.GetWall(target.pointList[i], target.pointList[(i + 1) % target.pointList.Count]);
                    itemList.Add(item);
                }

                for (int i = 0; i < data.pointList.Count; i++)
                {
                    Point p = data.pointList[i];

                    for (int k = 0; k < itemList.Count; k++)
                    {
                        WallData itemk = itemList[k];
                        if (itemk.Contines(p))
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(p, itemk))
                        {
                            List<WallData> list = data.CombinePointWall(p, itemk);
                            itemList.RemoveAt(k);
                            itemList.InsertRange(k, list);
                            break;
                        }
                    }
                }

                #endregion

                roomfunc.ForceRefreshRoomData(data);
                target = roomfunc.GetRoom(itemList);
                if (target != null) awake();
            }
            #endregion

            if (roomChanged)
            {
                roomfunc.ForceRefreshRoomData(data);
                target = roomfunc.GetRoom(target.pointList);
                //if (target != null) awake();
            }

            RefreshView();

            if (target == null || data.Contins(target) == false)
            {
                setState(FreeState2D.NAME);
            }
        }

    }

}
