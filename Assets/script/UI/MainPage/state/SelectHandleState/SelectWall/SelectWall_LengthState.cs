using UnityEngine;
using System;
using System.Collections.Generic;

public class SelectWall_LengthState : SelectWall_State
{
    private SetLengthHandleView currentWall
    {
        get {
            return bringData as SetLengthHandleView;
        }
    }

    private MainPageUIController control
    {
        get
        {
            return MainPageUIController.Instance;
        }
    }
    
    public Action deleExit;

    public void setData(SetLengthHandleView view, Action exit)
    {
        this.bringData = view;
        this.deleExit = exit;
        control.OpenInput(currentWall.text.text, currentWall.text, onChange);
    }

    private WallData helpWall = new WallData(new Point(), new Point());
    private float p1A
    {
        get
        {
            return currentWall.data.targetWall.a;
        }
    }
    private float p1B
    {
        get
        {
            return currentWall.data.targetWall.b;
        }
    }
    private float p1C
    {
        get
        {
            return currentWall.data.targetWall.c;
        }
    }
    private float p2A
    {
        get
        {
            return currentWall.otherdata.targetWall.a;
        }
    }
    private float p2B
    {
        get
        {
            return currentWall.otherdata.targetWall.b;
        }
    }
    private float p2C
    {
        get
        {
            return currentWall.otherdata.targetWall.c;
        }
    }




    public override void enter()
    {
        base.enter();

        ///功能在setData中 因为要传参 所以enter不满足 底层需要改进
        ///这里可以做一些不使用传递过来的参数的事情
    }

    public override void exit()
    {
        control.CloseInput();
        this.bringData = null;
        if (deleExit != null)
        {
            deleExit();
            deleExit = null;
        }
        base.exit();
    }
    
    private void onChange()
    {
        float len;
        int multi = (int)defaultSettings.DefaultUnit;
        if (float.TryParse(currentWall.text.text, out len))
        {
            if (len * 1000 / multi < 10 || len * 1000 / multi > 100000)
            {
                currentWall.text.color = Color.red;
            }
            else
            {
                currentWall.text.color = Color.black;
            }
        }
        else
        {
            currentWall.text.color = Color.red;
        }
    }

    public void Ok()
    {
        if (target == null || currentWall == null)
        {
            Debug.LogWarning("target == null || currentWall == null");
            return;
        }
        if (target != currentWall.data.targetWall)
        {
            OnRoom();
            return;
        }
        float length = float.Parse(currentWall.text.text) / (int)defaultSettings.DefaultUnit;
        float magnitude = wallfunc.GetSideLength(currentWall.data) / (int)defaultSettings.DefaultUnit;
        if (magnitude != length)
        {
            bool p1OnRoom = data.PointOnRoom(target.point1) != null ? true : false;
            bool p2OnRoom = data.PointOnRoom(target.point2) != null ? true : false;
            if (p1OnRoom == true || p2OnRoom == false)
            {
                undoHelper.save();
                target.point2.pos = target.point2.pos + (target.point2.pos - target.point1.pos) / target.length * (length - magnitude);
                RefreshView();
            }
            else
            {
                undoHelper.save();
                target.point1.pos = target.point1.pos + (target.point1.pos - target.point2.pos) / target.length * (length - magnitude);
                RefreshView();
            }
        }
    }
    
    public void OnRoom()
    {
        float length = float.Parse(currentWall.text.text) / (int)defaultSettings.DefaultUnit;
        float magnitude = wallfunc.GetSideLength(currentWall.data)/ (int)defaultSettings.DefaultUnit;
        if (magnitude != length)
        {
            undoHelper.save();

            Vector2 offset = Vector2.zero;
            WallData wall = currentWall.data.targetWall;
            bool isP1Move = currentWall.isParallelPointList.Contains(wall.point1);
            if (isP1Move)
            {
                offset = (wall.point1.pos - wall.point2.pos) / wall.length * (length - magnitude);
            }
            else
            {
                offset = (wall.point2.pos - wall.point1.pos) / wall.length * (length - magnitude);
            }

            List<Point> parallelList =  currentWall.isParallelPointList;

            if (wallfunc.IsParallel(currentWall.data.targetWall, currentWall.otherdata.targetWall) == true)
            {
                for (int i = 1; i < parallelList.Count - 1; i++)
                {
                    parallelList[i].pos += offset;
                }

                Point p0 = parallelList[0];
                Point pn = parallelList[parallelList.Count - 1];
                Point newp0 = p0;
                Point newpn = pn;
                List<Point> p0nearList = data.GetNearestPoints(p0);
                List<Point> pnnearList = data.GetNearestPoints(pn);
                if (p0nearList.Count > 2)
                {
                    newp0 = new Point(p0.pos);
                    data.AddPoint(newp0);
                    data.AddWall(p0, newp0, wall.height, wall.width);
                    WallData wall0 = data.GetWall(p0, parallelList[1]);
                    parallelList[0] = newp0;
                    if (p0 == wall0.point1)
                    {
                        wall0.point1 = newp0;
                    }
                    else
                    {
                        wall0.point2 = newp0;
                    }
                }
                if (pnnearList.Count > 2)
                {
                    newpn = new Point(pn.pos);
                    data.AddPoint(newpn);
                    data.AddWall(pn, newpn, wall.height, wall.width);
                    WallData walln = data.GetWall(parallelList[parallelList.Count - 2], pn);
                    parallelList[parallelList.Count - 1] = newpn;
                    if (pn == walln.point1)
                    {
                        walln.point1 = newpn;
                    }
                    else
                    {
                        walln.point2 = newpn;
                    }                    
                }
                newp0.pos += offset;
                newpn.pos += offset;
            }
            else {
                for (int i = 1; i < parallelList.Count; i++)
                {
                    Point item = parallelList[i];
                    Point itemF = parallelList[i - 1];
                    WallData itemWall = data.GetWall(item, itemF);
                    if (itemWall == null)
                    {
                        Debug.Log("item itemF GetitemWall == null");
                        return;
                    }
                    List<RoomData> rooms = roomfunc.GetCurrentRooms(itemWall);
                    if (rooms.Count <= 1)
                    {
                        data.RemoveWall(itemWall);
                    }
                }

                WallData startTarget;
                //边界处理
                {
                    Point p0 = parallelList[0];
                    Point pn = parallelList[parallelList.Count - 1];
                    Point newp0 = p0;
                    Point newpn = pn;
                    List<Point> p0nearList = data.GetNearestPoints(p0);
                    List<Point> pnnearList = data.GetNearestPoints(pn);
                    if (p0nearList.Count > 1)
                    {
                        newp0 = new Point(p0.pos);
                        data.AddPoint(newp0);
                        data.AddWall(p0, newp0, wall.height, wall.width);
                    }
                    if (pnnearList.Count > 1)
                    {
                        newpn = new Point(pn.pos);
                        data.AddPoint(newpn);
                        data.AddWall(pn, newpn, wall.height, wall.width);
                    }
                    startTarget = data.AddWall(newp0, newpn, target.height, target.width);
                    view2D.selectObjData = startTarget;
                }

                //Vector2 pos = startTarget.point1.pos + offset;
                //Vector2 disPos = startTarget.GetDisPoint(pos, 0);
                //offset = pos - disPos;
                helpWall.point1.pos = startTarget.point1.pos + offset;
                helpWall.point2.pos = startTarget.point2.pos + offset;
                Vector2 pos1 = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
                Vector2 pos2 = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
                if (currentWall.ForT == true)
                {
                    startTarget.point1.pos = pos1;
                    startTarget.point2.pos = pos2;
                }
                else
                {
                    startTarget.point1.pos = pos2;
                    startTarget.point2.pos = pos1;
                }
            }
            roomfunc.ForceRefreshRoomData(data);

            RefreshView();
        }
    }
}


/*

    //private Vector2 offset = Vector2.zero;
    //private List<Point> p1Nearlist;
    //private List<Point> p2Nearlist;
    //private List<WallData> targets;
    //private WallData helpWall = new WallData();
    //private LineHelpFunc linefunc = LineHelpFunc.Instance;
    #region 引用WallState数据
    //private WallData startdisWall
    //{
    //    get
    //    {
    //        return machine.selectWallState.startdisWall;
    //    }
    //}
    //private float olddis
    //{
    //    get
    //    {
    //        return machine.selectWallState.olddis;
    //    }
    //    set
    //    {
    //        machine.selectWallState.olddis = value;
    //    }
    //}
    //private Vector2 p1StartPos
    //{
    //    get
    //    {
    //        return machine.selectWallState.p1StartPos;
    //    }
    //}
    //private Vector2 p2StartPos
    //{
    //    get
    //    {
    //        return machine.selectWallState.p2StartPos;
    //    }
    //}
    //private bool p1Twoside
    //{
    //    get
    //    {
    //        return machine.selectWallState.p1Twoside;
    //    }
    //}
    //private bool p1parallel
    //{
    //    get
    //    {
    //        return machine.selectWallState.p1parallel;
    //    }
    //}
    //private bool p2Twoside
    //{
    //    get
    //    {
    //        return machine.selectWallState.p2Twoside;
    //    }
    //}
    //private bool p2parallel
    //{
    //    get
    //    {
    //        return machine.selectWallState.p2parallel;
    //    }
    //}
    //private WallData p1Wall
    //{
    //    get
    //    {
    //        return machine.selectWallState.p1Wall;
    //    }
    //    set
    //    {
    //        machine.selectWallState.p1Wall = value;
    //    }
    //}
    //private WallData p2Wall
    //{
    //    get
    //    {
    //        return machine.selectWallState.p2Wall;
    //    }
    //    set
    //    {
    //        machine.selectWallState.p2Wall = value;
    //    }
    //}
    //private Point p1WallOtherP
    //{
    //    get
    //    {
    //        return machine.selectWallState.p1WallOtherP;
    //    }
    //    set
    //    {
    //        machine.selectWallState.p1WallOtherP = value;
    //    }
    //}
    //private Point p2WallOtherP
    //{
    //    get
    //    {
    //        return machine.selectWallState.p2WallOtherP;
    //    }
    //    set
    //    {
    //        machine.selectWallState.p2WallOtherP = value;
    //    }
    //}
    //private float p1A
    //{
    //    get
    //    {
    //        return machine.selectWallState.p1A;
    //    }
    //}
    //private float p1B
    //{
    //    get
    //    {
    //        return machine.selectWallState.p1B;
    //    }
    //}
    //private float p1C
    //{
    //    get
    //    {
    //        return machine.selectWallState.p1C;
    //    }
    //}
    //private float p2A
    //{
    //    get
    //    {
    //        return machine.selectWallState.p2A;
    //    }
    //}
    //private float p2B
    //{
    //    get
    //    {
    //        return machine.selectWallState.p2B;
    //    }
    //}
    //private float p2C
    //{
    //    get
    //    {
    //        return machine.selectWallState.p2C;
    //    }
    //}
    #endregion

        //float length = float.Parse(currentWall.text.text) / 1000f;

        //int index = machine.OnRoomNearWallIndex;
        //if (index == 0)
        //{

        //}
        //else
        //{
        //    RoomData room;
        //    WallData tempWall;
        //    if (index == 1)
        //    {
        //        room = data.WallSideOnRoom(target.point1To2Data);
        //        tempWall = room.NextSide(target.point1To2Data).targetWall;
        //    }
        //    else if (index == 2)
        //    {
        //        room = data.WallSideOnRoom(target.point1To2Data);
        //        tempWall = room.LastSide(target.point1To2Data).targetWall;
        //    }
        //    else if (index == 3)
        //    {
        //        room = data.WallSideOnRoom(target.point2To1Data);
        //        tempWall = room.NextSide(target.point2To1Data).targetWall;
        //    }
        //    else
        //    {
        //        room = data.WallSideOnRoom(target.point2To1Data);
        //        tempWall = room.LastSide(target.point2To1Data).targetWall;
        //    }
        //    Point p1 = tempWall.point1;
        //    Point p2 = tempWall.point2;
        //    if (target.Contines(p1))
        //    {
        //        p1 = tempWall.point2;
        //        p2 = tempWall.point1;
        //    }

        //    Vector2 pos = p1.pos + (p2.pos - p1.pos) * length / (p2.pos - p1.pos).magnitude;
        //    //Vector2 disPos = target.GetDisPoint(pos);
        //    Vector2 disPos = startdisWall.GetDisPoint(pos);
        //    float dis = Vector2.Distance(disPos, pos);
        //    if (dis != olddis)
        //    {
        //        undoHelper.save();
        //        #region 使用上方的移动后的处理
        //        offset = pos - disPos;
        //        ///加上偏移量后的值 不包括纠正
        //        Vector2 p1curpos = p1StartPos + offset;
        //        Vector2 p2curpos = p2StartPos + offset;
        //        ///纠正后的值
        //        Vector2 p1pos = p1curpos;
        //        Vector2 p2pos = p2curpos;

        //        p1Nearlist = data.GetNearestPoints(target.point1);
        //        p1Nearlist.Remove(target.point2);
        //        p2Nearlist = data.GetNearestPoints(target.point2);
        //        p2Nearlist.Remove(target.point1);
        //        if ((p1Nearlist.Count > 1 && p1Twoside == false) || (p1parallel == true && p1Wall == null))
        //        {
        //            float height = p1Wall == null ? target.height : p1Wall.height;
        //            float width = p1Wall == null ? target.width : p1Wall.width;
        //            Point point = new Point(target.point1.pos);
        //            data.pointList.Add(point);
        //            p1Wall = data.AddWall(target.point1, point, height, width);
        //            p1WallOtherP = target.point1;
        //            target.point1 = point;
        //        }
        //        if ((p2Nearlist.Count > 1 && p2Twoside == false) || (p2parallel == true && p2Wall == null))
        //        {
        //            float height = p2Wall == null ? target.height : p2Wall.height;
        //            float width = p2Wall == null ? target.width : p2Wall.width;
        //            Point point = new Point(target.point2.pos);
        //            data.pointList.Add(point);
        //            p2Wall = data.AddWall(target.point2, point, height, width);
        //            p2WallOtherP = target.point2;
        //            target.point2 = point;
        //        }


        //        helpWall.point1.pos = p1curpos;
        //        helpWall.point2.pos = p2curpos;
        //        if (p1parallel == false)
        //        {
        //            bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
        //            if (cross == false)
        //            {
        //                p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
        //            }
        //        }
        //        if (p2parallel == false)
        //        {
        //            bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
        //            if (cross == false)
        //            {
        //                p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
        //            }
        //        }


        //        float minDis = 0.15f;
        //        ///边界点靠近墙
        //        WallData nearWall = null;
        //        ///墙靠近边界点 
        //        Point mindisPoint_wall = null;

        //        for (int i = 0; i < data.wallList.Count; i++)
        //        {
        //            WallData item = data.wallList[i];
        //            if (item == target) { continue; }
        //            bool p1IsPeparallel = linefunc.IsParallel(item.a, item.b, p1Wall.a, p1Wall.b);
        //            if (p1IsPeparallel == false)
        //            {
        //                bool isParallel = linefunc.IsTwoLineParallel(item, p1Wall);
        //                if (isParallel == false)
        //                {
        //                    Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p1Wall, Vector2.zero);
        //                    float crossPDis = Vector2.Distance(p1curpos, crossP);
        //                    if (crossPDis <= minDis)
        //                    {
        //                        helpWall.point1.pos = crossP;
        //                        helpWall.point2.pos = p2curpos + crossP - p1curpos;
        //                        minDis = crossPDis;
        //                        nearWall = item;
        //                        //Debug.LogWarning("point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
        //                        p1pos = crossP;
        //                        bool isParallelHelpP2 = linefunc.IsTwoLineParallel(p2Wall, helpWall);
        //                        if (isParallelHelpP2 == false)
        //                        {
        //                            p2pos = linefunc.GetTwoLineCrossPoint(p2Wall, helpWall, Vector2.zero);
        //                        }
        //                        //Debug.LogWarning("-------point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
        //                    }
        //                }
        //            }

        //            bool p2IsPeparallel = linefunc.IsParallel(item.a, item.b, p2Wall.a, p2Wall.b);
        //            if (p2IsPeparallel == false)
        //            {
        //                bool isParallel = linefunc.IsTwoLineParallel(item, p2Wall);
        //                if (isParallel == false)
        //                {
        //                    Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p2Wall, Vector2.zero);
        //                    float crossPDis = Vector2.Distance(p2curpos, crossP);
        //                    if (crossPDis <= minDis)
        //                    {
        //                        helpWall.point1.pos = p1curpos + crossP - p2curpos;
        //                        helpWall.point2.pos = crossP;
        //                        minDis = crossPDis;
        //                        nearWall = item;
        //                        //Debug.LogWarning("point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
        //                        bool isParallelHelpP1 = linefunc.IsTwoLineParallel(p1Wall, helpWall);
        //                        if (isParallelHelpP1 == false)
        //                        {
        //                            p1pos = linefunc.GetTwoLineCrossPoint(p1Wall, helpWall, Vector2.zero);
        //                        }
        //                        p2pos = crossP;
        //                        //Debug.LogWarning("=======point1 " + helpWall.point1.pos + "point2 " + helpWall.point2.pos);
        //                    }
        //                }
        //            }
        //        }
        //        if (nearWall == null)
        //        {
        //            minDis = 0.15f;
        //            for (int i = 0; i < data.pointList.Count; i++)
        //            {
        //                Point p = data.pointList[i];
        //                if (target.point1 == p || target.point2 == p) { continue; }
        //                helpWall.point1.pos = p1curpos;
        //                helpWall.point2.pos = p2curpos;
        //                float tempdis = linefunc.GetDis(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
        //                if (tempdis <= minDis)
        //                {
        //                    Vector2 disP = linefunc.GetDisPoint(helpWall.a, helpWall.b, helpWall.c, p.pos.x, p.pos.y);
        //                    if (wallfunc.PointOnWall(disP, target))
        //                    {
        //                        helpWall.point1.pos = p1curpos + p.pos - disP;
        //                        helpWall.point2.pos = p2curpos + p.pos - disP;
        //                        bool cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p1A, p1B);
        //                        if (cross == false)
        //                        {
        //                            p1pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p1A, p1B, p1C, Vector2.zero);
        //                        }
        //                        cross = linefunc.IsTwoLineParallel(helpWall.a, helpWall.b, p2A, p2B);
        //                        if (cross == false)
        //                        {
        //                            p2pos = linefunc.GetTwoLineCrossPoint(helpWall.a, helpWall.b, helpWall.c, p2A, p2B, p2C, Vector2.zero);
        //                        }

        //                        mindisPoint_wall = p;
        //                        minDis = tempdis;
        //                    }
        //                }
        //            }
        //        }

        //        if (Vector2.Dot((p1pos - p2pos), (target.point1.pos - target.point2.pos)) <= 0.05f)
        //        {
        //            olddis = dis;
        //            return;
        //        }

        //        target.point1.pos = p1pos;
        //        target.point2.pos = p2pos;
        //        olddis = dis;
        //        #endregion
        //        #region 合并
        //        for (int i = 0; i < data.pointList.Count; i++)
        //        {
        //            Point p = data.pointList[i];
        //            if (target.point1 == p || target.point2 == p || machine.selectWallState.PonitInRoom(p) == false) { continue; }

        //            if (Vector2.Distance(target.point1.pos, p.pos) < 0.1f)
        //            {
        //                p.pos = target.point1.pos;
        //                data.CombinePoint(target.point1, p);
        //                i--;
        //                target = data.GetWall(p, target.point2);
        //                continue;
        //            }
        //            if (Vector2.Distance(target.point2.pos, p.pos) < 0.1f)
        //            {
        //                p.pos = target.point2.pos;
        //                data.CombinePoint(target.point2, p, false);
        //                i--;
        //                target = data.GetWall(target.point1, p);
        //            }
        //        }
        //        for (int i = 0; i < data.wallList.Count; i++)
        //        {
        //            WallData item = data.wallList[i];
        //            if (item == target) { continue; }
        //            bool p1OnItem = false;
        //            bool p2OnItem = false;
        //            bool targetCrossItem = false;
        //            //bool ItemCrossP1wall = false;
        //            //bool ItemCrossP2wall = false;
        //            if (item.Contines(target.point1) == true || item.Contines(target.point2) == true)
        //            {
        //                if (item.Contines(target.point1) == false && wallfunc.PointOnWall(target.point1, item))
        //                {
        //                    p1OnItem = true;
        //                }

        //                if (item.Contines(target.point2) == false && wallfunc.PointOnWall(target.point2, item))
        //                {
        //                    p2OnItem = true;
        //                }
        //            }
        //            else
        //            {
        //                if (wallfunc.PointOnWall(target.point1, item))
        //                {
        //                    p1OnItem = true;
        //                }
        //                if (wallfunc.PointOnWall(target.point2, item))
        //                {
        //                    p2OnItem = true;
        //                }
        //                if (p1OnItem == false && p2OnItem == false)
        //                {
        //                    if (wallfunc.PointOnWall(item.point1, target) == false && wallfunc.PointOnWall(item.point2, target) == false && linefunc.GetCross(item, target))
        //                    {
        //                        targetCrossItem = true;
        //                    }
        //                }
        //            }

        //            if (p1OnItem == true)
        //            {
        //                data.CombinePointWall(target.point1, item);
        //                i--;
        //                continue;
        //            }
        //            else if (p2OnItem == true)
        //            {
        //                data.CombinePointWall(target.point2, item);
        //                i--;
        //                continue;
        //            }
        //            else if (targetCrossItem == true)
        //            {
        //                bool isParallel = linefunc.IsTwoLineParallel(item, target);
        //                if (isParallel == false)
        //                {
        //                    Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, target, Vector2.zero);
        //                    Point point = new Point(crossP);
        //                    data.AddPoint(point);
        //                    data.CombinePointWall(point, item);
        //                    i--;
        //                    continue;
        //                }
        //            }
        //            else if (p1Wall != null && wallfunc.PointOnWall(item.point1, p1Wall) == false && wallfunc.PointOnWall(item.point2, p1Wall) == false && linefunc.GetCross(item, p1Wall))
        //            {
        //                bool isParallel = linefunc.IsTwoLineParallel(item, p1Wall);
        //                if (isParallel == false)
        //                {
        //                    Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p1Wall, Vector2.zero);
        //                    Point point = new Point(crossP);
        //                    data.AddPoint(point);
        //                    data.CombinePointWall(point, item);
        //                    i--;
        //                    continue;
        //                }
        //            }
        //            else if (p2Wall != null && wallfunc.PointOnWall(item.point1, p2Wall) == false && wallfunc.PointOnWall(item.point2, p2Wall) == false && linefunc.GetCross(item, p2Wall))
        //            {
        //                bool isParallel = linefunc.IsTwoLineParallel(item, p2Wall);
        //                if (isParallel == false)
        //                {
        //                    Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, p2Wall, Vector2.zero);
        //                    Point point = new Point(crossP);
        //                    data.AddPoint(point);
        //                    data.CombinePointWall(point, item);
        //                    i--;
        //                    continue;
        //                }
        //            }
        //        }

        //        p1Nearlist = data.GetNearestPoints(target.point1);
        //        p1Nearlist.Remove(target.point2);
        //        targets.Clear();
        //        if (p1Twoside == true)
        //        {
        //            targets.Add(data.GetWall(target.point1, p1Nearlist[0]));
        //            targets.Add(data.GetWall(target.point1, p1Nearlist[1]));
        //        }
        //        else
        //        {
        //            if (p1Wall != null) targets.Add(p1Wall);
        //        }
        //        for (int i = 0; i < data.pointList.Count; i++)
        //        {
        //            Point p = data.pointList[i];
        //            for (int k = 0; k < targets.Count; k++)
        //            {
        //                WallData itemTarget = targets[k];
        //                if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
        //                if (wallfunc.PointOnWall(p, itemTarget))
        //                {
        //                    List<WallData> list = data.CombinePointWall(p, itemTarget);
        //                    targets.RemoveAt(k);
        //                    targets.InsertRange(k, list);
        //                    k--;
        //                }
        //            }
        //        }

        //        targets.Clear();
        //        p2Nearlist = data.GetNearestPoints(target.point2);
        //        p2Nearlist.Remove(target.point1);
        //        if (p2Twoside == true)
        //        {
        //            targets.Add(data.GetWall(target.point2, p2Nearlist[0]));
        //            targets.Add(data.GetWall(target.point2, p2Nearlist[1]));
        //        }
        //        else
        //        {
        //            if (p2Wall != null) targets.Add(p2Wall);
        //        }
        //        for (int i = 0; i < data.pointList.Count; i++)
        //        {
        //            Point p = data.pointList[i];
        //            for (int k = 0; k < targets.Count; k++)
        //            {
        //                WallData itemTarget = targets[k];
        //                if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
        //                if (wallfunc.PointOnWall(p, itemTarget))
        //                {
        //                    List<WallData> list = data.CombinePointWall(p, itemTarget);
        //                    targets.RemoveAt(k);
        //                    targets.InsertRange(k, list);
        //                    k--;
        //                }
        //            }
        //        }

        //        targets.Clear();
        //        targets.Add(target);
        //        for (int i = 0; i < data.pointList.Count; i++)
        //        {
        //            Point p = data.pointList[i];
        //            for (int k = 0; k < targets.Count; k++)
        //            {
        //                WallData itemTarget = targets[k];
        //                if (itemTarget.point1 == p || itemTarget.point2 == p) { continue; }
        //                if (wallfunc.PointOnWall(p, itemTarget))
        //                {
        //                    List<WallData> list = data.CombinePointWall(p, itemTarget);
        //                    targets.RemoveAt(k);
        //                    targets.InsertRange(k, list);
        //                    k--;
        //                }
        //            }
        //        }
        //        target = targets[0];
        //        roomfunc.ForceRefreshRoomData(data);
        //        #endregion
        //        RefreshView();
        //    }

        //    if (target == null || data.Contins(target) == false)
        //    {
        //        inputMachine.setState(FreeState2D.NAME);
        //    }
        //}
*/
