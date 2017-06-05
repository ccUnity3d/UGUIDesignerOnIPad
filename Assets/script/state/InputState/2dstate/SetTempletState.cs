using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SetTempletState : InputState2D {

    public const string NAME = "SetTempletState";
    public SetTempletState() : base(NAME)
    {

    }
    private InputTemplateData targetData;
    private int type;
    private List<Point> list = new List<Point>();


    private List<WallData> itemList = new List<WallData>();
    private List<Vector2> startPosList = new List<Vector2>();
    private Vector2 oldPos;
    private Vector2 startPos;
    private Vector2 offset;

    private List<Vector2> mideToSidePosList = new List<Vector2>();
    private int lastCount = 0;

    private bool enterWithDown = true;
    private bool firstUpRunned = false;

    public override void enter()
    {
        base.enter();
        type = machine.templateType;
        targetBasedata = targetData = mainpagedata.GetData(type);
        if (targetBasedata == null)
        {
            setState(FreeState2D.NAME);
            return;
        }
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
        for (int i = 0; i < list.Count; i++)
        {
            startPosList.Add(list[i].pos);
        }
    }

    public override void mUpdate()
    {
        base.mUpdate();
        if (uguiHitUI.uiHited == true) return;
        if (Input.GetMouseButtonDown(0))
        {
            undoHelper.save();
            creatRoom(Input.mousePosition);
            awake();
            RefreshView();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            #region 合并公共点线面 点在线上切分 线交叉切分
            for (int i = 0; i < list.Count; i++)
            {
                Point p = list[i];

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
            for (int i = 0; i < list.Count; i++)
            {
                Point p = list[i];

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
            for (int i = 0; i < list.Count; i++)
            {
                Point p = list[i];
                Point p2 = list[(i + 1) % list.Count];
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

            ///非选中房间点切分选中房间线
            itemList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                WallData item = data.GetWall(list[i], list[(i + 1) % list.Count]);
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
            RefreshView();
            setState(FreeState2D.NAME);
            mianpageMachine.setState(MainPageFreeState.Name);
        }
        else if(Input.GetMouseButton(0))
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
            #region pos != oldPos
            if (pos != oldPos)
            {
                offset = pos - startPos;

                #region 纠正offset
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].pos = startPosList[i] + offset;
                }             

                for (int i = 0; i < list.Count; i++)
                {
                    Point point = list[i];
                    point.pos = startPosList[i] + offset;
                    for (int k = 0; k < data.pointList.Count; k++)
                    {
                        Point pointk = data.pointList[k];
                        if (list.Contains(pointk))
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
                    for (int i = 0; i < list.Count; i++)
                    {
                        Point point = list[i];
                        for (int k = 0; k < data.wallList.Count; k++)
                        {
                            WallData wallk = data.wallList[k];
                            if (list.Contains(wallk.point1) && list.Contains(wallk.point2))
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
                    for (int i = 0; i < list.Count; i++)
                    {
                        WallData wall = data.GetWall(list[i], list[(i + 1) % list.Count]);
                        for (int k = 0; k < data.pointList.Count; k++)
                        {
                            Point pointk = data.pointList[k];
                            if (list.Contains(pointk))
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
                                    mindisWallToPoint = list[i];
                                    minDis = dis;
                                    offset = list[i].pos - startPosList[i] + pointk.pos - disP;
                                }
                            }
                        }
                    }
                }
                #endregion

                #endregion

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].pos = startPosList[i] + offset;
                }
                oldPos = pos;
                RefreshView();
            }
            #endregion
        }
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    private void creatRoom(Vector2 pos)
    {
        list.Clear();
        Vector2 v2 = Camera.main.ScreenToWorldPoint(pos);
        List<Vector2> poslist = targetData.offposList;
        for (int i = 0; i < poslist.Count; i++)
        {
            Vector2 posi = poslist[i] + v2;
            Point point = new Point(posi);
            data.AddPoint(point);
            list.Add(point);
        }
        RoomData room = new RoomData();
        room.pointList = new List<Point>();
        room.pointList.InsertRange(0, list);
        for (int i = 0; i < list.Count; i++)
        {
            Point point1 = list[i];
            Point point2 = list[(i + 1) % list.Count];
            WallData wall = data.AddWall(point1, point2, defaultSetting.DefaultHeight, defaultSetting.DefaultWidth);
            room.sideList.Add(wall.point1To2Data);
        }
        data.roomList.Add(room);

        //中线转成内线
        mideToSidePosList.Clear();
        for (int i = 0; i < list.Count; i++)
        {
            Point point0 = list[(i - 1 + list.Count) % list.Count];
            Point point1 = list[i];
            Point point2 = list[(i + 1) % list.Count];
            WallData wallF = data.GetWall(point0, point1);
            WallData wallT = data.GetWall(point1, point2);
            Vector2 tempV2 = linefunc.GetDisPoint(wallF.a, wallF.b, wallF.c2, point1.pos.x, point1.pos.y);
            Vector2 V2 = linefunc.GetTwoLineCrossPoint(wallF.a, wallF.b, wallF.c2, wallT.a, wallT.b, wallT.c2, tempV2);
            mideToSidePosList.Add(V2);
        }
        for (int i = 0; i < list.Count; i++)
        {
            list[i].pos = mideToSidePosList[i];
        }
    }

    public override void enterPhone()
    {
        base.enterPhone();
        firstUpRunned = false;
        if (Input.touchCount > 0)
        {
            enterWithDown = true;
        }
        else {
            enterWithDown = false;
        }
    }
    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();

        if (uguiHitUI.uiHited == true) return;

        if (Input.touchCount != 1)
        {
            lastCount = Input.touchCount;
            return;
        }

        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            //Debug.Log("SetTempletState TouchPhase.Began");
            undoHelper.save();
            creatRoom(touch.position);
            awake();
            RefreshView();
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            if (enterWithDown == true && firstUpRunned == false)
            {
                firstUpRunned = true;
                return;
            }
            //Debug.Log("SetTempletState TouchPhase.Ended " + Time.realtimeSinceStartup);
            #region 合并公共点线面 点在线上切分 线交叉切分
            for (int i = 0; i < list.Count; i++)
            {
                Point p = list[i];

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
            for (int i = 0; i < list.Count; i++)
            {
                Point p = list[i];

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
            for (int i = 0; i < list.Count; i++)
            {
                Point p = list[i];
                Point p2 = list[(i + 1) % list.Count];
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

            ///非选中房间点切分选中房间线
            itemList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                WallData item = data.GetWall(list[i], list[(i + 1) % list.Count]);
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
            RefreshView();
            //Debug.Log("SetTempletState setState FreeState2D.NAME");
            setState(FreeState2D.NAME);
            mianpageMachine.setState(MainPageFreeState.Name);
        }
        else if (touch.phase == TouchPhase.Moved)
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
            #region pos != oldPos
            if (pos != oldPos)
            {
                offset = pos - startPos;

                #region 纠正offset
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].pos = startPosList[i] + offset;
                }

                for (int i = 0; i < list.Count; i++)
                {
                    Point point = list[i];
                    point.pos = startPosList[i] + offset;
                    for (int k = 0; k < data.pointList.Count; k++)
                    {
                        Point pointk = data.pointList[k];
                        if (list.Contains(pointk))
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
                    for (int i = 0; i < list.Count; i++)
                    {
                        Point point = list[i];
                        for (int k = 0; k < data.wallList.Count; k++)
                        {
                            WallData wallk = data.wallList[k];
                            if (list.Contains(wallk.point1) && list.Contains(wallk.point2))
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
                    for (int i = 0; i < list.Count; i++)
                    {
                        WallData wall = data.GetWall(list[i], list[(i + 1) % list.Count]);
                        for (int k = 0; k < data.pointList.Count; k++)
                        {
                            Point pointk = data.pointList[k];
                            if (list.Contains(pointk))
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
                                    mindisWallToPoint = list[i];
                                    minDis = dis;
                                    offset = list[i].pos - startPosList[i] + pointk.pos - disP;
                                }
                            }
                        }
                    }
                }
                #endregion

                #endregion

                for (int i = 0; i < list.Count; i++)
                {
                    list[i].pos = startPosList[i] + offset;
                }
                oldPos = pos;
                RefreshView();
            }
            #endregion
        }
    }

    public override void exit()
    {
        list.Clear();
        base.exit();
    }
    
}
