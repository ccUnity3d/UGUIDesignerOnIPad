using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using foundation;

public class SelectWallState : SelectState
{
    public const string NAME = "SelectWallState";
    public SelectWallState():base(NAME)
    {
        wallMachine = SelectWall_StateMachine.Instance;
        wallMachine.selectWallState = this;
    }

    public WallData _target;
    #region 定义
    /// <summary>
    /// 当前选中墙
    /// </summary>
    public WallData target
    {
        set {
            _target = value;
            onTargetChange(_target);
        }
        get {
            return _target;
        }
    }
    /// <summary>
    /// p1初始位置
    /// </summary>
    public Vector2 p1StartPos = Vector2.zero;
    /// <summary>
    /// p2初始位置
    /// </summary>
    public Vector2 p2StartPos = Vector2.zero;

    /// <summary>
    /// 鼠标初始点的坐标
    /// </summary>
    private Vector2 startPos = Vector2.zero;
    /// <summary>
    /// 鼠标上一个记录位置
    /// </summary>
    private Vector2 oldPos = Vector2.zero;
    /// <summary>
    /// 初始距离墙
    /// </summary>
    public WallData startdisWall = new WallData(new Point(), new Point());
    /// <summary>
    /// 之前的距离
    /// </summary>
    public float olddis = 0;
    /// <summary>
    /// 墙所在房间 可能为空
    /// </summary>
    private RoomData targetRoom = null;
    /// <summary>
    /// 偏移量 辅助计算
    /// </summary>
    public Vector2 offset;
    /// <summary>
    /// 临时墙 辅助计算
    /// </summary>
    private WallData helpWall = new WallData(new Point(), new Point());
    /// <summary>
    /// targetRoom中跟p1点相连的相邻墙
    /// </summary>
    public WallData p1Wall = null;
    /// <summary>
    /// targetRoom中跟p2点相连的相邻墙
    /// </summary>
    public WallData p2Wall = null;
    /// <summary>
    /// targetRoom中跟p1点相连的相邻墙的另一个顶点
    /// </summary>
    public Point p1WallOtherP = null;
    /// <summary>
    /// targetRoom中跟p2点相连的相邻墙的另一个顶点
    /// </summary>
    public Point p2WallOtherP = null;

    public float p1A = 0;
    public float p1B = 0;
    public float p1C = 0;
    public float p2A = 0;
    public float p2B = 0;
    public float p2C = 0;
    /// <summary>
    /// targetRoom中跟p1点相连的相邻墙 是否与targetwall平行
    /// </summary>
    public bool p1parallel = false;
    /// <summary>
    /// targetRoom中跟p2点相连的相邻墙 是否与targetwall平行
    /// </summary>
    public bool p2parallel = false;

    private Point P1
    {
        get {
            return target.point1;
        }
    }
    private Point P2
    {
        get
        {
            return target.point2;
        }
    }

    /// <summary>
    /// target被分割成多段
    /// </summary>
    private List<WallData> targets = new List<WallData>();
    /// <summary>
    /// p1的除p2外所有相邻点
    /// </summary>
    private List<Point> p1Nearlist = null;
    /// <summary>
    /// p2的除p1外所有相邻点
    /// </summary>
    private List<Point> p2Nearlist = null;
    /// <summary>
    /// p1点的除p2外只有两个顶点并且这两个端点与p1共线 
    /// </summary>
    public bool p1Twoside = false;
    /// <summary>
    /// p2点的除p1外只有两个顶点并且这两个端点与p2共线
    /// </summary>
    public bool p2Twoside = false;
    /// <summary>
    /// target是否发生移动
    /// </summary>
    private bool moved = false;

    private SelectWall_StateMachine wallMachine;
    private bool down;

    #endregion

    public override void enter()
    {
        base.enter();
        target = (viewTarget as WallView).data;
        optionsController.selectMachine = wallMachine;
        wallMachine.setState(EditTypeOnSelect.Free);
        awake();
        canMove = true;
        //DAKAI
        SetWallController.Instance.Open(SetWidth, target.length, target.width, target.height, false, true, false);
    }

    private void SetWidth(float arg1, float width, float arg3)
    {
        target.width = width / (int)defaultSetting.DefaultUnit;
        RefreshView();
    }

    public override void exit()
    {
        //GUANBI
        SetWallController.Instance.Close();
        wallMachine.setState(EditTypeOnSelect.Free);
        base.exit();
    }

    protected override GoodsType getGoodsType()
    {
        return GoodsType.Wall;
    }

    private void awake()
    {
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
        p1StartPos = P1.pos;
        p2StartPos = P2.pos;
        targetRoom = null;
        for (int i = 0; i < data.roomList.Count; i++)
        {
            RoomData room = data.roomList[i];
            if (room.sideList.Contains(target.point1To2Data))
            {
                targetRoom = room;
            }
            if (room.sideList.Contains(target.point2To1Data))
            {
                if (targetRoom == null) targetRoom = room;
            }
        }

        p1Nearlist = data.GetNearestPoints(P1);
        p1Nearlist.Remove(P2);
        p2Nearlist = data.GetNearestPoints(P2);
        p2Nearlist.Remove(P1);

        if (targetRoom != null)
        {
            p1Wall = null;
            p2Wall = null;

            startdisWall.point1.pos = P1.pos;
            startdisWall.point2.pos = P2.pos;
            Vector2 dispos = startdisWall.GetDisPoint(oldPos);
            olddis = Vector2.Distance(oldPos, dispos);
            //p1WallOtherP = null;
            //p2WallOtherP = null;
            for (int i = 0; i < targetRoom.sideList.Count; i++)
            {
                WallSideData side = targetRoom.sideList[i];
                if (side.targetWall.Contines(P1) && side.targetWall.Contines(P2))
                {
                    continue;
                }
                if (side.targetWall.Contines(P1))
                {
                    p1parallel = wallfunc.IsParallel(target, side.targetWall);
                    if (p1parallel == false)
                    {
                        p1Wall = side.targetWall;
                        p1A = p1Wall.a;
                        p1B = p1Wall.b;
                        p1C = p1Wall.c;
                        //p1WallOtherP = p1Wall.point1 == P1 ? p1Wall.point2 : p1Wall.point1;
                    }
                    else
                    {
                        WallData wall = side.targetWall;
                        p1A = wall.b;
                        p1B = -wall.a;
                        p1C = wall.a * P1.pos.y - wall.b * P1.pos.x;
                        //p1WallOtherP = wall.point1 == P1 ? wall.point1 : wall.point2;
                    }
                    continue;
                }
                if (side.targetWall.Contines(P2))
                {
                    p2parallel = wallfunc.IsParallel(target, side.targetWall);
                    if (p2parallel == false)
                    {
                        p2Wall = side.targetWall;
                        p2A = p2Wall.a;
                        p2B = p2Wall.b;
                        p2C = p2Wall.c;
                        //p2WallOtherP = p2Wall.point1 == P2 ? p2Wall.point2 : p2Wall.point1;
                    }
                    else
                    {
                        WallData wall = side.targetWall;
                        p2A = wall.b;
                        p2B = -wall.a;
                        p2C = wall.a * P2.pos.y - wall.b * P2.pos.x;
                        //p2WallOtherP = wall.point1 == P2 ? wall.point1 : wall.point2;
                    }
                    continue;
                }
            }
        }
        p1Twoside = false;
        p2Twoside = false;
        if (p1Nearlist.Count == 2 && p1parallel == false)
        {
            p1Twoside = wallfunc.IsParallel(data.GetWall(P1, p1Nearlist[0]), data.GetWall(P1, p1Nearlist[1]));
        }
        if (p2Nearlist.Count == 2 && p2parallel == false)
        {
            p2Twoside = wallfunc.IsParallel(data.GetWall(P2, p2Nearlist[0]), data.GetWall(P2, p2Nearlist[1]));
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
            if (Input.GetMouseButtonUp(0) == false)
            {
                if (Input.GetMouseButton(2))
                {
                    RefreshView();
                }
                return;
            }
            else if(down == false)
            {
                if (Input.GetMouseButton(2))
                {
                    RefreshView();
                }
                return;
            }
            down = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            awake();
            saved = false;
            down = true;
        }
        else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            Vector2 pos = GetScreenToWorldPos(Input.mousePosition);
            if (targetRoom == null)
            {
                #region pos != oldPos
                if (pos != oldPos)
                {
                    if (saved == false) saved = true;
                    moved = true;
                    List<Point> list1 = data.GetNearestPoints(P1);
                    List<Point> list2 = data.GetNearestPoints(P2);
                    if (list1.Count > 1)
                    {
                        Point point = new Point(P1.pos);
                        data.pointList.Add(point);
                        Point oldPoint = target.point1;
                        target.point1 = point;
                        CombineWall(oldPoint);
                    }
                    if (list2.Count > 1)
                    {
                        Point point = new Point(P2.pos);
                        data.pointList.Add(point);
                        Point oldPoint = target.point2;
                        target.point2 = point;
                        CombineWall(oldPoint);
                    }

                    offset = pos - startPos;
                    ///加上偏移量后的值 不包括纠正
                    Vector2 p1curpos = p1StartPos + offset;
                    Vector2 p2curpos = p2StartPos + offset;
                    ///纠正后的值
                    Vector2 p1pos = p1curpos;
                    Vector2 p2pos = p2curpos;

                    float minDis = 0.06f;
                    ///边界点靠近点
                    Point mindisPoint = null;
                    ///边界点靠近墙
                    WallData mindisWall = null;
                    ///墙靠近边界点 
                    Point mindisPoint_wall = null;

                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        if (P1 == p || P2 == p) { continue; }
                        float p1Dis = Vector2.SqrMagnitude(p1curpos - p.pos);
                        if (p1Dis <= minDis)
                        {
                            p1pos = p.pos;
                            p2pos = p2curpos + p.pos - p1curpos;
                            minDis = p1Dis;
                            mindisPoint = p;
                            continue;
                        }
                        float p2Dis = Vector2.SqrMagnitude(p2curpos - p.pos);
                        if (p2Dis <= minDis)
                        {
                            p2pos = p.pos;
                            p1pos = p1curpos + p.pos - p2curpos;
                            minDis = p2Dis;
                            mindisPoint = p;
                        }
                    }
                    if (mindisPoint == null)
                    {
                        minDis = 0.15f;
                        for (int i = 0; i < data.wallList.Count; i++)
                        {
                            WallData item = data.wallList[i];
                            if (item == target) { continue; }

                            float dis = linefunc.GetDis(item.a, item.b, item.c, p1curpos.x, p1curpos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(item.a, item.b, item.c, p1curpos.x, p1curpos.y);
                                if (wallfunc.PointOnWall(disP, item))
                                {
                                    p1pos = disP;
                                    p2pos = p2curpos + disP - p1curpos;
                                    mindisWall = item;
                                    minDis = dis;
                                }
                            }

                            dis = linefunc.GetDis(item.a, item.b, item.c, p2curpos.x, p2curpos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(item.a, item.b, item.c, p2curpos.x, p2curpos.y);
                                if (wallfunc.PointOnWall(disP, item))
                                {
                                    p2pos = disP;
                                    p1pos = p1curpos + disP - p2curpos;
                                    mindisWall = item;
                                    minDis = dis;
                                }
                            }
                        }
                    }

                    if (mindisPoint == null && mindisWall == null)
                    {
                        minDis = 0.15f;
                        for (int i = 0; i < data.pointList.Count; i++)
                        {
                            Point p = data.pointList[i];
                            if (P1 == p || P2 == p) { continue; }
                            helpWall.point1.pos = p1curpos;
                            helpWall.point2.pos = p2curpos;
                            float dis = linefunc.GetDis(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
                                if (wallfunc.PointOnWall(disP, target))
                                {
                                    p1pos = p1curpos + p.pos - disP;
                                    p2pos = p2curpos + p.pos - disP;
                                    mindisPoint_wall = p;
                                    minDis = dis;
                                }
                            }
                        }
                    }
                    P1.pos = p1pos;
                    P2.pos = p2pos;
                    oldPos = pos;
                }
                #endregion
                #region Input.GetMouseButtonUp
                if (Input.GetMouseButtonUp(0))
                {
                    #region
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        if (P1 == p || P2 == p) { continue; }
                        if (Vector2.Distance(P1.pos, p.pos) < 0.1f)
                        {
                            data.CombinePoint(P1, p);
                            i--;
                            target = data.GetWall(p, P2);
                            continue;
                        }
                        if (Vector2.Distance(P2.pos, p.pos) < 0.1f)
                        {
                            data.CombinePoint(P2, p, false);
                            i--;
                            target = data.GetWall(P1, p);
                        }
                    }
                    for (int i = 0; i < data.wallList.Count; i++)
                    {
                        WallData item = data.wallList[i];
                        if (item == target) { continue; }
                        bool p1OnItem = false;
                        bool p2OnItem = false;
                        bool targetCrossItem = false;
                        if (item.Contines(P1) == true || item.Contines(P2) == true)
                        {
                            if (item.Contines(P1) == false && wallfunc.PointOnWall(P1, item))
                            {
                                p1OnItem = true;
                            }

                            if (item.Contines(P2) == false && wallfunc.PointOnWall(P2, item))
                            {
                                p2OnItem = true;
                            }
                        }
                        else
                        {
                            if (wallfunc.PointOnWall(P1, item))
                            {
                                p1OnItem = true;
                            }
                            if (wallfunc.PointOnWall(P2, item))
                            {
                                p2OnItem = true;
                            }
                            if (p1OnItem == false && p2OnItem == false)
                            {
                                if (wallfunc.PointOnWall(item.point1, target) == false && wallfunc.PointOnWall(item.point2, target) == false && linefunc.GetCross(item, target))
                                {
                                    targetCrossItem = true;
                                }
                            }
                        }

                        if (p1OnItem == true && p2OnItem == true)
                        {
                            data.RemovePoint(P1);
                            data.RemovePoint(P2);
                            target = item;
                        }
                        else if (p1OnItem == true)
                        {
                            data.CombinePointWall(target.point1, item);
                        }
                        else if (p2OnItem == true)
                        {
                            data.CombinePointWall(target.point2, item);
                        }
                        else if (targetCrossItem == true)
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, target);
                            if (isParallel == false)///targetCrossItem == true 好像永远不会出现item, target平行的情况
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, target, Vector2.zero);
                                Point point = new Point(crossP);
                                data.AddPoint(point);
                                data.CombinePointWall(point, item);
                                i--;
                            }
                        }

                        if (p1OnItem == true || p2OnItem == true)
                        {
                            i--;
                        }
                    }

                    targets.Clear();
                    targets.Add(target);
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        for (int k = 0; k < targets.Count; k++)
                        {
                            WallData itemTarget = targets[k];
                            if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
                            if (wallfunc.PointOnWall(p, itemTarget))
                            {
                                List<WallData> list = data.CombinePointWall(p, itemTarget);
                                targets.RemoveAt(k);
                                targets.InsertRange(k, list);
                                k--;
                            }
                        }
                    }
                    target = targets[0];
                    roomfunc.ForceRefreshRoomData(data);
                    #endregion
                }
                #endregion
                RefreshView();
                if (target == null || data.Contins(target) == false)
                {
                    setState(FreeState2D.NAME);
                }
                return;
            }
            else
            {
                Vector2 disPos = startdisWall.GetDisPoint(pos);
                float dis = Vector2.Distance(disPos, pos);
                #region dis != olddis
                if (dis != olddis)
                {
                    if (saved == false) saved = true;
                    moved = true;
                    offset = pos - disPos;
                    ///加上偏移量后的值 不包括纠正
                    Vector2 p1curpos = p1StartPos + offset;
                    Vector2 p2curpos = p2StartPos + offset;
                    ///纠正后的值
                    Vector2 p1pos = p1curpos;
                    Vector2 p2pos = p2curpos;

                    p1Nearlist = data.GetNearestPoints(P1);
                    p1Nearlist.Remove(P2);
                    p2Nearlist = data.GetNearestPoints(P2);
                    p2Nearlist.Remove(P1);
                    if ((p1Nearlist.Count > 1 && p1Twoside == false) || (p1parallel == true && p1Wall == null))
                    {
                        float height = p1Wall == null ? target.height : p1Wall.height;
                        float width = p1Wall == null ? target.width : p1Wall.width;
                        Point point = new Point(P1.pos);
                        data.AddPoint(point);
                        p1Wall = data.AddWall(P1, point, height, width);
                        //p1WallOtherP = P1;

                        List<RoomData> rooms = roomfunc.GetCurrentRooms(target);
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            rooms[i].Replace(target.point1, point);
                        }
                        target.point1 = point;
                    }
                    if ((p2Nearlist.Count > 1 && p2Twoside == false) || (p2parallel == true && p2Wall == null))
                    {
                        float height = p2Wall == null ? target.height : p2Wall.height;
                        float width = p2Wall == null ? target.width : p2Wall.width;
                        Point point = new Point(P2.pos);
                        data.pointList.Add(point);
                        p2Wall = data.AddWall(P2, point, height, width);
                        //p2WallOtherP = P2;

                        List<RoomData> rooms = roomfunc.GetCurrentRooms(target);
                        for (int i = 0; i < rooms.Count; i++)
                        {
                            rooms[i].Replace(target.point2, point);
                        }
                        target.point2 = point;
                    }


                    helpWall.point1.pos = p1curpos;
                    helpWall.point2.pos = p2curpos;
                    if (p1parallel == false)
                    {
                        bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
                        if (cross == false)
                        {
                            p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                        }
                    }
                    if (p2parallel == false)
                    {
                        bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
                        if (cross == false)
                        {
                            p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                        }
                    }


                    float minDis = 0.15f;
                    ///边界点靠近墙
                    WallData nearWall = null;
                    ///墙靠近边界点 
                    Point mindisPoint_wall = null;

                    for (int i = 0; i < data.wallList.Count; i++)
                    {
                        WallData item = data.wallList[i];
                        if (item == target) { continue; }
                        bool p1IsPeparallel = linefunc.IsParallel(item.a, item.b, p1Wall.a, p1Wall.b);
                        if (p1IsPeparallel == false)
                        {
                            //bool isParallel = linefunc.IsTwoLineParallel(item, p1Wall);
                            //if (isParallel == false)
                            //{
                            Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p1Wall, Vector2.zero);
                            //float crossPDis = Vector2.Distance(p1curpos, crossP);
                            float crossPDis = helpWall.GetDis(crossP);
                            if (crossPDis <= minDis)
                            {
                                Vector2 dispos = helpWall.GetDisPoint(crossP);
                                Vector2 off = crossP - dispos;
                                helpWall.point1.pos += off;
                                helpWall.point2.pos += off;
                                //helpWall.point1.pos = crossP;
                                //helpWall.point2.pos = p2curpos + crossP - p1curpos;
                                minDis = crossPDis;
                                nearWall = item;
                                //Debug.LogWarning("point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
                                if (p1parallel == false)
                                {
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
                                    if (cross == false)
                                    {
                                        p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                                    }
                                }
                                if (p2parallel == false)
                                {
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
                                    if (cross == false)
                                    {
                                        p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                                    }
                                }
                                //p1pos = crossP;
                                //bool isParallelHelpP2 = linefunc.IsTwoLineParallel(p2Wall, helpWall);
                                //if (isParallelHelpP2 == false)
                                //{
                                //    p2pos = linefunc.GetTwoLineCrossPoint(p2Wall, helpWall, Vector2.zero);
                                //}
                                //Debug.LogWarning("-------point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
                            }
                            //}
                        }

                        bool p2IsPeparallel = linefunc.IsParallel(item.a, item.b, p2Wall.a, p2Wall.b);
                        if (p2IsPeparallel == false)
                        {
                            //bool isParallel = linefunc.IsTwoLineParallel(item, p2Wall);
                            //if (isParallel == false)
                            //{
                            Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p2Wall, Vector2.zero);
                            //float crossPDis = Vector2.Distance(p2curpos, crossP);
                            float crossPDis = helpWall.GetDis(crossP);
                            if (crossPDis <= minDis)
                            {
                                Vector2 dispos = helpWall.GetDisPoint(crossP);
                                Vector2 off = crossP - dispos;
                                helpWall.point1.pos += off;
                                helpWall.point2.pos += off;
                                //helpWall.point1.pos = p1curpos + crossP - p2curpos;
                                //helpWall.point2.pos = crossP;
                                minDis = crossPDis;
                                nearWall = item;
                                //Debug.LogWarning("point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
                                if (p1parallel == false)
                                {
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
                                    if (cross == false)
                                    {
                                        p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                                    }
                                }
                                if (p2parallel == false)
                                {
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
                                    if (cross == false)
                                    {
                                        p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                                    }
                                }
                                //bool isParallelHelpP1 = linefunc.IsTwoLineParallel(p1Wall, helpWall);
                                //if (isParallelHelpP1 == false)
                                //{
                                //    p1pos = linefunc.GetTwoLineCrossPoint(p1Wall, helpWall, Vector2.zero);
                                //}
                                //p2pos = crossP;
                                //Debug.LogWarning("=======point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
                            }
                            //}
                        }
                    }
                    if (nearWall == null)
                    {
                        minDis = 0.15f;
                        for (int i = 0; i < data.pointList.Count; i++)
                        {
                            Point p = data.pointList[i];
                            if (P1 == p || P2 == p) { continue; }
                            helpWall.point1.pos = p1curpos;
                            helpWall.point2.pos = p2curpos;
                            float tempdis = linefunc.GetDis(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
                            if (tempdis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
                                if (wallfunc.PointOnWall(disP, target))
                                {
                                    helpWall.point1.pos = p1curpos + p.pos - disP;
                                    helpWall.point2.pos = p2curpos + p.pos - disP;
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
                                    if (cross == false)
                                    {
                                        p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                                    }
                                    cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
                                    if (cross == false)
                                    {
                                        p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                                    }

                                    mindisPoint_wall = p;
                                    minDis = tempdis;
                                }
                            }
                        }
                    }

                    if (Vector2.Dot((p1pos - p2pos), (P1.pos - P2.pos)) <= 0.05f)
                    {
                        olddis = dis;
                        return;
                    }

                    P1.pos = p1pos;
                    P2.pos = p2pos;
                    olddis = dis;
                }
                #endregion
                #region Input.GetMouseButtonUp
                if (Input.GetMouseButtonUp(0))
                {
                    #region
                    //合并点
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        if (P1 == p || P2 == p || PonitInRoom(p) == false) { continue; }

                        if (Vector2.Distance(P1.pos, p.pos) < 0.1f)
                        {
                            p.pos = P1.pos;
                            data.CombinePoint(P1, p);
                            if (p1Wall != null) {
                                if (p1Wall.point1 == P1)
                                {
                                    p1Wall.point1 = p;
                                }
                                else if (p1Wall.point2 == P1)
                                {
                                    p1Wall.point2 = p;
                                }
                                else
                                {
                                    Debug.LogError("P1Wall Wrong");
                                }                            
                            }
                            i--;
                            target = data.GetWall(p, P2);
                            continue;
                        }
                        if (Vector2.Distance(P2.pos, p.pos) < 0.1f)
                        {
                            p.pos = P2.pos;
                            data.CombinePoint(P2, p, false);
                            if (p2Wall != null)
                            {
                                if (p2Wall.point1 == P2)
                                {
                                    p2Wall.point1 = p;
                                }
                                else if (p2Wall.point2 == P2)
                                {
                                    p2Wall.point2 = p;
                                }
                                else
                                {
                                    Debug.LogError("P2Wall Wrong");
                                }
                            }
                            i--;
                            target = data.GetWall(P1, p);
                        }
                    }

                    //target拆分墙
                    for (int i = 0; i < data.wallList.Count; i++)
                    {
                        WallData item = data.wallList[i];
                        if (item == target) { continue; }
                        bool p1OnItem = false;
                        bool p2OnItem = false;
                        bool targetCrossItem = false;
                        //bool ItemCrossP1wall = false;
                        //bool ItemCrossP2wall = false;
                        bool itemContinsP1 = item.Contines(P1);
                        bool itemContinsP2 = item.Contines(P2);
                        if (itemContinsP1 == true)
                        {
                            if (itemContinsP2) { }
                            else if (wallfunc.PointOnWall(P2, item)) p2OnItem = true;
                        }
                        else if (itemContinsP2 == true)
                        {
                            if (wallfunc.PointOnWall(P1, item)) p1OnItem = true;
                        }
                        else
                        {
                            if (wallfunc.PointOnWall(P1, item))
                            {
                                p1OnItem = true;
                            }
                            if (wallfunc.PointOnWall(P2, item))
                            {
                                p2OnItem = true;
                            }
                            if (p1OnItem == false && p2OnItem == false)
                            {
                                if (wallfunc.PointOnWall(item.point1, target) == false && wallfunc.PointOnWall(item.point2, target) == false && linefunc.GetCross(item, target))
                                {
                                    targetCrossItem = true;
                                }
                            }
                        }

                        if (p1OnItem == true)
                        {
                            data.CombinePointWall(P1, item);
                            i--;
                            continue;
                        }
                        else if (p2OnItem == true)
                        {
                            data.CombinePointWall(P2, item);
                            i--;
                            continue;
                        }
                        else if (targetCrossItem == true)
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, target);
                            if (isParallel == false)
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, target, Vector2.zero);
                                Point point = new Point(crossP);
                                data.AddPoint(point);
                                data.CombinePointWall(point, item);
                                i--;
                                continue;
                            }
                        }
                        else if (p1Wall != null && wallfunc.PointOnWall(item.point1, p1Wall) == false && wallfunc.PointOnWall(item.point2, p1Wall) == false && linefunc.GetCross(item, p1Wall))
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, p1Wall);
                            if (isParallel == false)
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p1Wall, Vector2.zero);
                                Point point = new Point(crossP);
                                data.AddPoint(point);
                                data.CombinePointWall(point, item);
                                i--;
                                continue;
                            }
                        }
                        else if (p2Wall != null && wallfunc.PointOnWall(item.point1, p2Wall) == false && wallfunc.PointOnWall(item.point2, p2Wall) == false && linefunc.GetCross(item, p2Wall))
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, p2Wall);
                            if (isParallel == false)
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p2Wall, Vector2.zero);
                                Point point = new Point(crossP);
                                data.AddPoint(point);
                                data.CombinePointWall(point, item);
                                i--;
                                continue;
                            }
                        }
                    }

                    p1Nearlist = data.GetNearestPoints(P1);
                    p1Nearlist.Remove(P2);
                    targets.Clear();
                    if (p1Twoside == true)
                    {
                        targets.Add(data.GetWall(P1, p1Nearlist[0]));
                        if(p1Nearlist.Count > 1)targets.Add(data.GetWall(P1, p1Nearlist[1]));
                    }
                    else
                    {
                        if (p1Wall != null) targets.Add(p1Wall);
                    }
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        for (int k = 0; k < targets.Count; k++)
                        {
                            WallData itemTarget = targets[k];
                            if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
                            if (wallfunc.PointOnWall(p, itemTarget))
                            {
                                List<WallData> list = data.CombinePointWall(p, itemTarget);
                                targets.RemoveAt(k);
                                targets.InsertRange(k, list);
                                k--;
                            }
                        }
                    }

                    targets.Clear();
                    p2Nearlist = data.GetNearestPoints(P2);
                    p2Nearlist.Remove(P1);
                    if (p2Twoside == true)
                    {
                        targets.Add(data.GetWall(P2, p2Nearlist[0]));
                        if (p2Nearlist.Count > 1) targets.Add(data.GetWall(P2, p2Nearlist[1]));
                    }
                    else
                    {
                        if (p2Wall != null) targets.Add(p2Wall);
                    }
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        for (int k = 0; k < targets.Count; k++)
                        {
                            WallData itemTarget = targets[k];
                            if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
                            if (wallfunc.PointOnWall(p, itemTarget))
                            {
                                List<WallData> list = data.CombinePointWall(p, itemTarget);
                                targets.RemoveAt(k);
                                targets.InsertRange(k, list);
                                k--;
                            }
                        }
                    }

                    targets.Clear();
                    targets.Add(target);
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        for (int k = 0; k < targets.Count; k++)
                        {
                            WallData itemTarget = targets[k];
                            if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
                            if (wallfunc.PointOnWall(p, itemTarget))
                            {
                                List<WallData> list = data.CombinePointWall(p, itemTarget);
                                targets.RemoveAt(k);
                                targets.InsertRange(k, list);
                                k--;
                            }
                        }
                    }
                    target = targets[0];
                    roomfunc.ForceRefreshRoomData(data);
                    awake();
                    if (target == null || data.Contins(target) == false)
                    {
                        setState(FreeState2D.NAME);
                    }
                    #endregion
                }
                #endregion 
                RefreshView();
            }
        }
        else if (Input.mouseScrollDelta != Vector2.zero)
        {
            RefreshView();
        }
        else if(Input.GetMouseButton(2))
        {
            RefreshView();
        }
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();

        if (changed) return;

        if (Input.touchCount != 1) return;

        if (uguiHitUI.uiHited == true) return;

        Touch touch = Input.GetTouch(0);
        bool inputUp = touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended;
        if (canMove == false)
        {
            if (inputUp == false)
            {
                return;
            }
            else if (down == false)
            {
                return;
            }
            down = false;
        }

        if (touch.phase == TouchPhase.Began)
        {
            awake();
            saved = false;
            down = true;
        }
        else if (touch.phase != TouchPhase.Stationary)
        {
            Vector2 pos = GetScreenToWorldPos(touch.position);
            if (targetRoom == null)
            {
                #region pos != oldPos
                if (pos != oldPos)
                {
                    if (saved == false) saved = true;
                    moved = true;
                    List<Point> list1 = data.GetNearestPoints(P1);
                    List<Point> list2 = data.GetNearestPoints(P2);
                    if (list1.Count > 1)
                    {
                        Point point = new Point(P1.pos);
                        data.pointList.Add(point);
                        Point oldPoint = target.point1;
                        target.point1 = point;
                        CombineWall(oldPoint);
                    }
                    if (list2.Count > 1)
                    {
                        Point point = new Point(P2.pos);
                        data.pointList.Add(point);
                        Point oldPoint = target.point2;
                        target.point2 = point;
                        CombineWall(oldPoint);
                    }

                    offset = pos - startPos;
                    ///加上偏移量后的值 不包括纠正
                    Vector2 p1curpos = p1StartPos + offset;
                    Vector2 p2curpos = p2StartPos + offset;
                    ///纠正后的值
                    Vector2 p1pos = p1curpos;
                    Vector2 p2pos = p2curpos;

                    float minDis = 0.06f;
                    ///边界点靠近点
                    Point mindisPoint = null;
                    ///边界点靠近墙
                    WallData mindisWall = null;
                    ///墙靠近边界点 
                    Point mindisPoint_wall = null;

                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        if (P1 == p || P2 == p) { continue; }
                        float p1Dis = Vector2.SqrMagnitude(p1curpos - p.pos);
                        if (p1Dis <= minDis)
                        {
                            p1pos = p.pos;
                            p2pos = p2curpos + p.pos - p1curpos;
                            minDis = p1Dis;
                            mindisPoint = p;
                            continue;
                        }
                        float p2Dis = Vector2.SqrMagnitude(p2curpos - p.pos);
                        if (p2Dis <= minDis)
                        {
                            p2pos = p.pos;
                            p1pos = p1curpos + p.pos - p2curpos;
                            minDis = p2Dis;
                            mindisPoint = p;
                        }
                    }
                    if (mindisPoint == null)
                    {
                        minDis = 0.15f;
                        for (int i = 0; i < data.wallList.Count; i++)
                        {
                            WallData item = data.wallList[i];
                            if (item == target) { continue; }

                            float dis = linefunc.GetDis(item.a, item.b, item.c, p1curpos.x, p1curpos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(item.a, item.b, item.c, p1curpos.x, p1curpos.y);
                                if (wallfunc.PointOnWall(disP, item))
                                {
                                    p1pos = disP;
                                    p2pos = p2curpos + disP - p1curpos;
                                    mindisWall = item;
                                    minDis = dis;
                                }
                            }

                            dis = linefunc.GetDis(item.a, item.b, item.c, p2curpos.x, p2curpos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(item.a, item.b, item.c, p2curpos.x, p2curpos.y);
                                if (wallfunc.PointOnWall(disP, item))
                                {
                                    p2pos = disP;
                                    p1pos = p1curpos + disP - p2curpos;
                                    mindisWall = item;
                                    minDis = dis;
                                }
                            }
                        }
                    }

                    if (mindisPoint == null && mindisWall == null)
                    {
                        minDis = 0.15f;
                        for (int i = 0; i < data.pointList.Count; i++)
                        {
                            Point p = data.pointList[i];
                            if (P1 == p || P2 == p) { continue; }
                            helpWall.point1.pos = p1curpos;
                            helpWall.point2.pos = p2curpos;
                            float dis = linefunc.GetDis(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
                            if (dis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
                                if (wallfunc.PointOnWall(disP, target))
                                {
                                    p1pos = p1curpos + p.pos - disP;
                                    p2pos = p2curpos + p.pos - disP;
                                    mindisPoint_wall = p;
                                    minDis = dis;
                                }
                            }
                        }
                    }
                    P1.pos = p1pos;
                    P2.pos = p2pos;
                    oldPos = pos;

                    if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                    {
                        RefreshView();
                    }
                }
                #endregion
            }
            else
            {
                Vector2 disPos = startdisWall.GetDisPoint(pos);
                float dis = Vector2.Distance(disPos, pos);
                #region dis != olddis
                if (dis != olddis)
                {
                    if (saved == false) saved = true;
                    moved = true;
                    offset = pos - disPos;
                    ///加上偏移量后的值 不包括纠正
                    Vector2 p1curpos = p1StartPos + offset;
                    Vector2 p2curpos = p2StartPos + offset;
                    ///纠正后的值
                    Vector2 p1pos = p1curpos;
                    Vector2 p2pos = p2curpos;

                    p1Nearlist = data.GetNearestPoints(P1);
                    p1Nearlist.Remove(P2);
                    p2Nearlist = data.GetNearestPoints(P2);
                    p2Nearlist.Remove(P1);
                    if ((p1Nearlist.Count > 1 && p1Twoside == false) || (p1parallel == true && p1Wall == null))
                    {
                        float height = p1Wall == null ? target.height : p1Wall.height;
                        float width = p1Wall == null ? target.width : p1Wall.width;
                        Point point = new Point(P1.pos);
                        data.pointList.Add(point);
                        p1Wall = data.AddWall(P1, point, height, width);
                        //p1WallOtherP = P1;
                        target.point1 = point;
                    }
                    if ((p2Nearlist.Count > 1 && p2Twoside == false) || (p2parallel == true && p2Wall == null))
                    {
                        float height = p2Wall == null ? target.height : p2Wall.height;
                        float width = p2Wall == null ? target.width : p2Wall.width;
                        Point point = new Point(P2.pos);
                        data.pointList.Add(point);
                        p2Wall = data.AddWall(P2, point, height, width);
                        //p2WallOtherP = P2;
                        target.point2 = point;
                    }


                    helpWall.point1.pos = p1curpos;
                    helpWall.point2.pos = p2curpos;
                    if (p1parallel == false)
                    {
                        bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
                        if (cross == false)
                        {
                            p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                        }
                    }
                    if (p2parallel == false)
                    {
                        bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
                        if (cross == false)
                        {
                            p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                        }
                    }


                    float minDis = 0.15f;
                    ///边界点靠近墙
                    WallData nearWall = null;
                    ///墙靠近边界点 
                    Point mindisPoint_wall = null;

                    for (int i = 0; i < data.wallList.Count; i++)
                    {
                        WallData item = data.wallList[i];
                        if (item == target) { continue; }
                        bool p1IsPeparallel = linefunc.IsParallel(item.a, item.b, p1Wall.a, p1Wall.b);
                        if (p1IsPeparallel == false)
                        {
                            //bool isParallel = linefunc.IsTwoLineParallel(item, p1Wall);
                            //if (isParallel == false)
                            //{
                            Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p1Wall, Vector2.zero);
                            //float crossPDis = Vector2.Distance(p1curpos, crossP);
                            float crossPDis = helpWall.GetDis(crossP);
                            if (crossPDis <= minDis)
                            {
                                Vector2 dispos = helpWall.GetDisPoint(crossP);
                                Vector2 off = crossP - dispos;
                                helpWall.point1.pos += off;
                                helpWall.point2.pos += off;
                                //helpWall.point1.pos = crossP;
                                //helpWall.point2.pos = p2curpos + crossP - p1curpos;
                                minDis = crossPDis;
                                nearWall = item;
                                //Debug.LogWarning("point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
                                if (p1parallel == false)
                                {
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
                                    if (cross == false)
                                    {
                                        p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                                    }
                                }
                                if (p2parallel == false)
                                {
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
                                    if (cross == false)
                                    {
                                        p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                                    }
                                }
                                //p1pos = crossP;
                                //bool isParallelHelpP2 = linefunc.IsTwoLineParallel(p2Wall, helpWall);
                                //if (isParallelHelpP2 == false)
                                //{
                                //    p2pos = linefunc.GetTwoLineCrossPoint(p2Wall, helpWall, Vector2.zero);
                                //}
                                //Debug.LogWarning("-------point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
                            }
                            //}
                        }

                        bool p2IsPeparallel = linefunc.IsParallel(item.a, item.b, p2Wall.a, p2Wall.b);
                        if (p2IsPeparallel == false)
                        {
                            //bool isParallel = linefunc.IsTwoLineParallel(item, p2Wall);
                            //if (isParallel == false)
                            //{
                            Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p2Wall, Vector2.zero);
                            //float crossPDis = Vector2.Distance(p2curpos, crossP);
                            float crossPDis = helpWall.GetDis(crossP);
                            if (crossPDis <= minDis)
                            {
                                Vector2 dispos = helpWall.GetDisPoint(crossP);
                                Vector2 off = crossP - dispos;
                                helpWall.point1.pos += off;
                                helpWall.point2.pos += off;
                                //helpWall.point1.pos = p1curpos + crossP - p2curpos;
                                //helpWall.point2.pos = crossP;
                                minDis = crossPDis;
                                nearWall = item;
                                //Debug.LogWarning("point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
                                if (p1parallel == false)
                                {
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
                                    if (cross == false)
                                    {
                                        p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                                    }
                                }
                                if (p2parallel == false)
                                {
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
                                    if (cross == false)
                                    {
                                        p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                                    }
                                }
                                //bool isParallelHelpP1 = linefunc.IsTwoLineParallel(p1Wall, helpWall);
                                //if (isParallelHelpP1 == false)
                                //{
                                //    p1pos = linefunc.GetTwoLineCrossPoint(p1Wall, helpWall, Vector2.zero);
                                //}
                                //p2pos = crossP;
                                //Debug.LogWarning("=======point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
                            }
                            //}
                        }
                    }
                    if (nearWall == null)
                    {
                        minDis = 0.15f;
                        for (int i = 0; i < data.pointList.Count; i++)
                        {
                            Point p = data.pointList[i];
                            if (P1 == p || P2 == p) { continue; }
                            helpWall.point1.pos = p1curpos;
                            helpWall.point2.pos = p2curpos;
                            float tempdis = linefunc.GetDis(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
                            if (tempdis <= minDis)
                            {
                                Vector2 disP = linefunc.GetDisPoint(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
                                if (wallfunc.PointOnWall(disP, target))
                                {
                                    helpWall.point1.pos = p1curpos + p.pos - disP;
                                    helpWall.point2.pos = p2curpos + p.pos - disP;
                                    bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
                                    if (cross == false)
                                    {
                                        p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                                    }
                                    cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
                                    if (cross == false)
                                    {
                                        p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                                    }

                                    mindisPoint_wall = p;
                                    minDis = tempdis;
                                }
                            }
                        }
                    }

                    if (Vector2.Dot((p1pos - p2pos), (P1.pos - P2.pos)) <= 0.05f)
                    {
                        olddis = dis;
                        return;
                    }

                    P1.pos = p1pos;
                    P2.pos = p2pos;
                    olddis = dis;

                    if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                    {
                        RefreshView();
                    }
                }
                #endregion
            }

            if (inputUp)
            {
                if (targetRoom == null)
                {
                    #region TouchPhase.Ended
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        if (P1 == p || P2 == p) { continue; }
                        if (Vector2.Distance(P1.pos, p.pos) < 0.1f)
                        {
                            data.CombinePoint(P1, p);
                            i--;
                            target = data.GetWall(p, P2);
                            continue;
                        }
                        if (Vector2.Distance(P2.pos, p.pos) < 0.1f)
                        {
                            data.CombinePoint(P2, p, false);
                            i--;
                            target = data.GetWall(P1, p);
                        }
                    }
                    for (int i = 0; i < data.wallList.Count; i++)
                    {
                        WallData item = data.wallList[i];
                        if (item == target) { continue; }
                        bool p1OnItem = false;
                        bool p2OnItem = false;
                        bool targetCrossItem = false;
                        if (item.Contines(P1) == true || item.Contines(P2) == true)
                        {
                            if (item.Contines(P1) == false && wallfunc.PointOnWall(P1, item))
                            {
                                p1OnItem = true;
                            }

                            if (item.Contines(P2) == false && wallfunc.PointOnWall(P2, item))
                            {
                                p2OnItem = true;
                            }
                        }
                        else
                        {
                            if (wallfunc.PointOnWall(P1, item))
                            {
                                p1OnItem = true;
                            }
                            if (wallfunc.PointOnWall(P2, item))
                            {
                                p2OnItem = true;
                            }
                            if (p1OnItem == false && p2OnItem == false)
                            {
                                if (wallfunc.PointOnWall(item.point1, target) == false && wallfunc.PointOnWall(item.point2, target) == false && linefunc.GetCross(item, target))
                                {
                                    targetCrossItem = true;
                                }
                            }
                        }

                        if (p1OnItem == true && p2OnItem == true)
                        {
                            data.RemovePoint(P1);
                            data.RemovePoint(P2);
                            target = item;
                        }
                        else if (p1OnItem == true)
                        {
                            data.CombinePointWall(target.point1, item);
                        }
                        else if (p2OnItem == true)
                        {
                            data.CombinePointWall(target.point2, item);
                        }
                        else if (targetCrossItem == true)
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, target);
                            if (isParallel == false)
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, target, Vector2.zero);
                                Point point = new Point(crossP);
                                data.AddPoint(point);
                                data.CombinePointWall(point, item);
                                i--;
                            }
                        }

                        if (p1OnItem == true || p2OnItem == true)
                        {
                            i--;
                        }
                    }

                    targets.Clear();
                    targets.Add(target);
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        for (int k = 0; k < targets.Count; k++)
                        {
                            WallData itemTarget = targets[k];
                            if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
                            if (wallfunc.PointOnWall(p, itemTarget))
                            {
                                List<WallData> list = data.CombinePointWall(p, itemTarget);
                                targets.RemoveAt(k);
                                targets.InsertRange(k, list);
                                k--;
                            }
                        }
                    }
                    target = targets[0];
                    roomfunc.ForceRefreshRoomData(data);
                    #endregion
                    RefreshView();
                    if (target == null || data.Contins(target) == false)
                    {
                        setState(FreeState2D.NAME);
                    }
                }
                else
                {
                    #region TouchPhase.Ended
                    //target两顶点临近已有点的处理
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        if (P1 == p || P2 == p || PonitInRoom(p) == false) { continue; }

                        if (Vector2.Distance(P1.pos, p.pos) < 0.1f)
                        {
                            p.pos = P1.pos;
                            data.CombinePoint(P1, p);
                            if (p1Wall != null)
                            {
                                if (p1Wall.point1 == P1)
                                {
                                    p1Wall.point1 = p;
                                }
                                else if (p1Wall.point2 == P1)
                                {
                                    p1Wall.point2 = p;
                                }
                                else
                                {
                                    Debug.LogError("P1Wall Wrong");
                                }
                            }
                            i--;
                            target = data.GetWall(p, P2);
                            continue;
                        }
                        if (Vector2.Distance(P2.pos, p.pos) < 0.1f)
                        {
                            p.pos = P2.pos;
                            data.CombinePoint(P2, p, false);
                            if (p2Wall != null)
                            {
                                if (p2Wall.point1 == P2)
                                {
                                    p2Wall.point1 = p;
                                }
                                else if (p2Wall.point2 == P2)
                                {
                                    p2Wall.point2 = p;
                                }
                                else
                                {
                                    Debug.LogError("P2Wall Wrong");
                                }
                            }
                            i--;
                            target = data.GetWall(P1, p);
                        }
                    }
                    //target与墙交叉和p1在墙上、p2在墙上的处理
                    for (int i = 0; i < data.wallList.Count; i++)
                    {
                        WallData item = data.wallList[i];
                        if (item == target) { continue; }
                        bool p1OnItem = false;
                        bool p2OnItem = false;
                        bool targetCrossItem = false;
                        bool itemContinsP1 = item.Contines(P1);
                        bool itemContinsP2 = item.Contines(P2);
                        if (itemContinsP1 == true)
                        {
                            if (itemContinsP2) { }
                            else if (wallfunc.PointOnWall(P2, item)) p2OnItem = true;
                        }
                        else if (itemContinsP2 == true)
                        {
                            if (wallfunc.PointOnWall(P1, item)) p1OnItem = true;
                        }
                        else
                        {
                            if (wallfunc.PointOnWall(P1, item))
                            {
                                p1OnItem = true;
                            }
                            if (wallfunc.PointOnWall(P2, item))
                            {
                                p2OnItem = true;
                            }
                            if (p1OnItem == false && p2OnItem == false)
                            {
                                if (wallfunc.PointOnWall(item.point1, target) == false && wallfunc.PointOnWall(item.point2, target) == false && linefunc.GetCross(item, target))
                                {
                                    targetCrossItem = true;
                                }
                            }
                        }

                        if (p1OnItem == true)
                        {
                            data.CombinePointWall(P1, item);
                            i--;
                            continue;
                        }
                        else if (p2OnItem == true)
                        {
                            data.CombinePointWall(P2, item);
                            i--;
                            continue;
                        }
                        else if (targetCrossItem == true)
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, target);
                            if (isParallel == false)
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, target, Vector2.zero);
                                Point point = new Point(crossP);
                                data.AddPoint(point);
                                data.CombinePointWall(point, item);
                                i--;
                                continue;
                            }
                        }
                        else if (p1Wall != null && wallfunc.PointOnWall(item.point1, p1Wall) == false && wallfunc.PointOnWall(item.point2, p1Wall) == false && linefunc.GetCross(item, p1Wall))
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, p1Wall);
                            if (isParallel == false)
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p1Wall, Vector2.zero);
                                Point point = new Point(crossP);
                                data.AddPoint(point);
                                data.CombinePointWall(point, item);
                                i--;
                                continue;
                            }
                        }
                        else if (p2Wall != null && wallfunc.PointOnWall(item.point1, p2Wall) == false && wallfunc.PointOnWall(item.point2, p2Wall) == false && linefunc.GetCross(item, p2Wall))
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, p2Wall);
                            if (isParallel == false)
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p2Wall, Vector2.zero);
                                Point point = new Point(crossP);
                                data.AddPoint(point);
                                data.CombinePointWall(point, item);
                                i--;
                                continue;
                            }
                        }
                    }

                    p1Nearlist = data.GetNearestPoints(P1);
                    p1Nearlist.Remove(P2);
                    targets.Clear();
                    if (p1Twoside == true)
                    {
                        targets.Add(data.GetWall(P1, p1Nearlist[0]));
                        if (p1Nearlist.Count > 1) targets.Add(data.GetWall(P1, p1Nearlist[1]));
                    }
                    else
                    {
                        if (p1Wall != null) targets.Add(p1Wall);
                    }
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        for (int k = 0; k < targets.Count; k++)
                        {
                            WallData itemTarget = targets[k];
                            if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
                            if (wallfunc.PointOnWall(p, itemTarget))
                            {
                                List<WallData> list = data.CombinePointWall(p, itemTarget);
                                targets.RemoveAt(k);
                                targets.InsertRange(k, list);
                                k--;
                            }
                        }
                    }

                    targets.Clear();
                    p2Nearlist = data.GetNearestPoints(P2);
                    p2Nearlist.Remove(P1);
                    if (p2Twoside == true)
                    {
                        targets.Add(data.GetWall(P2, p2Nearlist[0]));
                        if (p2Nearlist.Count > 1) targets.Add(data.GetWall(P2, p2Nearlist[1]));
                    }
                    else
                    {
                        if (p2Wall != null) targets.Add(p2Wall);
                    }
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        for (int k = 0; k < targets.Count; k++)
                        {
                            WallData itemTarget = targets[k];
                            if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
                            if (wallfunc.PointOnWall(p, itemTarget))
                            {
                                List<WallData> list = data.CombinePointWall(p, itemTarget);
                                targets.RemoveAt(k);
                                targets.InsertRange(k, list);
                                k--;
                            }
                        }
                    }

                    targets.Clear();
                    targets.Add(target);
                    for (int i = 0; i < data.pointList.Count; i++)
                    {
                        Point p = data.pointList[i];
                        for (int k = 0; k < targets.Count; k++)
                        {
                            WallData itemTarget = targets[k];
                            if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
                            if (wallfunc.PointOnWall(p, itemTarget))
                            {
                                List<WallData> list = data.CombinePointWall(p, itemTarget);
                                targets.RemoveAt(k);
                                targets.InsertRange(k, list);
                                k--;
                            }
                        }
                    }
                    target = targets[0];
                    roomfunc.ForceRefreshRoomData(data);
                    #endregion
                    RefreshView();
                    awake();
                    if (target == null || data.Contins(target) == false)
                    {
                        setState(FreeState2D.NAME);
                    }
                }
            }
        }
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

    private void CombineWall(Point p)
    {
        List<Point> list = data.GetNearestPoints(p);
        if (list.Count != 2)
        {
            return;
        }

        float listDis = Vector2.Distance(list[0].pos, list[1].pos);
        float dis0 = Vector2.Distance(list[0].pos, p.pos);
        float dis1 = Vector2.Distance(list[1].pos, p.pos);
        if (dis0 + dis1 - listDis < 0.1f)
        {
            WallData wall = data.GetWall(p, list[0]);
            data.RemovePoint(p);
            data.AddWall(list[0], list[1], wall.height, wall.width);
        }        
    }
    
}