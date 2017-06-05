using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using foundation;

public class SelectPointState : SelectState {
    public const string NAME = "SelectPointState";
    public SelectPointState():base(NAME)
    {
        pointMachine = SelectPoint_StateMachine.Instance;
        pointMachine.selectPointState = this;
    }

    public Point _target;
    public Point target
    {
        get {
            return _target;
        }
        set {
            _target = value;
            onTargetChange(_target);
        }
    }
    private Vector2 lastMousePos = Vector2.zero;
    private SelectPoint_StateMachine pointMachine;

    private List<WallData> targetList = new List<WallData>();

    public override void enter()
    {
        base.enter();
        target = (viewTarget as PointView).data;
        optionsController.selectMachine = pointMachine;
        pointMachine.setState(EditTypeOnSelect.Free);
    }

    protected override GoodsType getGoodsType()
    {
        return GoodsType.Point;
    }

    public override void mUpdate()
    {
        base.mUpdate();

        if (changed) return; 

        if (uguiHitUI.uiHited == true) return;

        if (canMove == false)
        {
            if (Input.GetMouseButtonUp(0) == false) return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            saved = false;
        }

        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
        {
            //Debug.LogWarning("Input.GetMouseButton(0)");
            Vector2 pos = GetScreenToWorldPos(Input.mousePosition);
            if (lastMousePos == pos && Input.GetMouseButtonUp(0) == false)
            {
                return;
            }
            lastMousePos = pos;

            if (saved == false) saved = true;

            Vector2 v2 = pos;
            Point nearPoint = null;
            WallData nearWall = null;
            //bool nearPointAxis = false;
            //bool cross = false;

            float minDis = 0.3f;
            for (int i = 0; i < data.pointList.Count; i++)
            {
                Point item = data.pointList[i];
                if (item == target)
                {
                    continue;
                }
                float dis = Vector2.Distance(item.pos, pos);
                if (dis < minDis)
                {
                    nearPoint = item;
                    v2 = item.pos;
                    minDis = dis;
                }
            }

            if (nearPoint == null)
            {
                minDis = 0.25f;
                for (int i = 0; i < data.wallList.Count; i++)
                {
                    WallData item = data.wallList[i];
                    if (item.Contines(target) == true)
                    {
                        continue;
                    }
                    float dis = item.GetDis(pos);
                    if (dis > minDis)
                    {
                        continue;
                    }
                    Vector2 nearV2 = item.GetDisPoint(pos);
                    if (wallfunc.PointOnWall(nearV2, item))
                    {
                        nearWall = item;
                        v2 = nearV2;
                        break;
                    }
                }
            }
            if (nearPoint == null && nearWall == null)
            {
                minDis = 0.15f;
                for (int i = 0; i < data.pointList.Count; i++)
                {
                    Point item = data.pointList[i];
                    if (item == target)
                    {
                        continue;
                    }
                    if (Mathf.Abs(pos.x - item.pos.x) < minDis)
                    {
                        v2.x = item.pos.x;
                        //nearPointAxis = true;
                    }
                    if (Mathf.Abs(pos.y - item.pos.y) < minDis)
                    {
                        v2.y = item.pos.y;
                        //nearPointAxis = true;
                    }
                }
            }
            //for (int i = 0; i < data.wallList.Count; i++)
            //{
            //    WallData wall = data.wallList[i];

            //    if (wall.Contines(target))
            //    {
            //        continue;
            //    }
            //    if (nearPoint != null && wall.Contines(nearPoint))
            //    {
            //        continue;
            //    }
            //    if (nearWall != null && wall == nearWall)
            //    {
            //        continue;
            //    }

            //    List<Point> list = data.GetNearestPoints(target);
            //    for (int k = 0; k < list.Count; k++)
            //    {
            //        if (linefunc.GetCross(wall, list[k], v2) == true)
            //        {
            //            cross = true;
            //            break;
            //        }
            //    }
            //    if (cross == true)
            //    {
            //        break;
            //    }
            //}

            ////if (cross == false)
            ////{
            bool equl = false;
            if (target.pos == v2)
            {
                equl = true;
            }
            target.pos = v2;
            bool up = false;
            if (Input.GetMouseButtonUp(0))
            {
                canMove = false;
                up = true;
                if (nearPoint != null)
                {
                    data.CombinePoint(nearPoint, target);
                    if (target == null || data.Contins(target) == false)
                    {
                        setState(FreeState2D.NAME);
                    }
                }
                else if (nearWall != null)
                {
                    data.CombinePointWall(target, nearWall);
                }

                targetList.Clear();
                ///target 临边新位置产生的切分（不包括临边顶点的切分）
                List<Point> list = data.GetNearestPoints(target);
                for (int k = 0; k < list.Count; k++)
                {
                    WallData targetWall = data.GetWall(list[k], target);
                    targetList.Add(targetWall);
                    ///target 临边切分其他墙体
                    for (int i = 0; i < data.wallList.Count; i++)
                    {
                        WallData item = data.wallList[i];
                        if (item == targetWall) { continue; }
                        bool p1OnItem = false;
                        bool p2OnItem = false;
                        bool targetCrossItem = false;
                        if (item.Contines(targetWall.point1) == true || item.Contines(targetWall.point2) == true)
                        {
                            if (item.Contines(targetWall.point1) == false && wallfunc.PointOnWall(targetWall.point1, item))
                            {
                                p1OnItem = true;
                            }

                            if (item.Contines(targetWall.point2) == false && wallfunc.PointOnWall(targetWall.point2, item))
                            {
                                p2OnItem = true;
                            }
                        }
                        else
                        {
                            if (wallfunc.PointOnWall(targetWall.point1, item))
                            {
                                p1OnItem = true;
                            }
                            if (wallfunc.PointOnWall(targetWall.point2, item))
                            {
                                p2OnItem = true;
                            }
                            if (p1OnItem == false && p2OnItem == false)
                            {
                                if (wallfunc.PointOnWall(item.point1, targetWall) == false && wallfunc.PointOnWall(item.point2, targetWall) == false && linefunc.GetCross(item, targetWall))
                                {
                                    targetCrossItem = true;
                                }
                            }
                        }

                        if (p1OnItem == true && p2OnItem == true)
                        {
                            data.RemovePoint(targetWall.point1);
                            data.RemovePoint(targetWall.point2);
                            targetWall = item;
                        }
                        else if (p1OnItem == true)
                        {
                            data.CombinePointWall(targetWall.point1, item);
                        }
                        else if (p2OnItem == true)
                        {
                            data.CombinePointWall(targetWall.point2, item);
                        }
                        else if (targetCrossItem == true)
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, targetWall);
                            if (isParallel == false)///targetCrossItem == true 好像永远不会出现item, target平行的情况
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, targetWall, Vector2.zero);
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
                    //for (int i = 0; i < data.wallList.Count; i++)
                    //{
                    //    WallData wall = data.wallList[i];
                    //    if (wall.Contines(list[k]) || wall.Contines(target)) continue;
                    //    if (linefunc.GetCross(wall, targetWall) == true)
                    //    {
                    //        Vector2 cross = linefunc.GetTwoLineCrossPoint(wall, targetWall, Vector2.zero);
                    //        Point crossP = new Point(cross);
                    //        data.AddPoint(crossP);
                    //        data.CombinePointWall(crossP, wall);
                    //        i--;
                    //        //continue; 
                    //    }
                    //}
                }
  
                ///其他点切分target
                for (int i = 0; i < data.pointList.Count; i++)
                {
                    Point point = data.pointList[i];
                    for (int k = 0; k < targetList.Count; k++)
                    {
                        WallData wall = targetList[k];
                        if (point == wall.point1 || point == wall.point2)
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(point, wall))
                        {
                            List<WallData> newTargetwalls = data.CombinePointWall(point, wall);
                            targetList.RemoveAt(k);
                            targetList.InsertRange(k, newTargetwalls);
                            k--;
                        }
                    }
                }
                

                roomfunc.ForceRefreshRoomData(data);

            }

            if (up == false && equl == true)
            {
                return;
            }

            RefreshView();
            //}
            //else
            //{
            //}
        }
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
            saved = false;
        }
        else
        {
            //Debug.LogWarning("Input.GetMouseButton(0)");
            Vector2 pos = GetScreenToWorldPos(touch.position);
            if (lastMousePos == pos && inputUp == false)
            {
                return;
            }
            lastMousePos = pos;
            if (saved == false) saved = true;
            Vector2 v2 = pos;
            Point nearPoint = null;
            WallData nearWall = null;

            float minDis = 0.3f;
            for (int i = 0; i < data.pointList.Count; i++)
            {
                Point item = data.pointList[i];
                if (item == target)
                {
                    continue;
                }
                float dis = Vector2.Distance(item.pos, pos);
                if (dis < minDis)
                {
                    nearPoint = item;
                    v2 = item.pos;
                    minDis = dis;
                }
            }

            if (nearPoint == null)
            {
                minDis = 0.25f;
                for (int i = 0; i < data.wallList.Count; i++)
                {
                    WallData item = data.wallList[i];
                    if (item.Contines(target) == true)
                    {
                        continue;
                    }
                    float dis = item.GetDis(pos);
                    if (dis > minDis)
                    {
                        continue;
                    }
                    Vector2 nearV2 = item.GetDisPoint(pos);
                    if (wallfunc.PointOnWall(nearV2, item))
                    {
                        nearWall = item;
                        v2 = nearV2;
                        break;
                    }
                }
            }
            if (nearPoint == null && nearWall == null)
            {
                minDis = 0.15f;
                for (int i = 0; i < data.pointList.Count; i++)
                {
                    Point item = data.pointList[i];
                    if (item == target)
                    {
                        continue;
                    }
                    if (Mathf.Abs(pos.x - item.pos.x) < minDis)
                    {
                        v2.x = item.pos.x;
                        //nearPointAxis = true;
                    }
                    if (Mathf.Abs(pos.y - item.pos.y) < minDis)
                    {
                        v2.y = item.pos.y;
                        //nearPointAxis = true;
                    }
                }
            }
            

            bool equl = false;
            if (target.pos == v2)
            {
                equl = true;
            }
            target.pos = v2;
            bool up = false;
            if (inputUp == true)
            {
                canMove = false;
                up = true;
                if (nearPoint != null)
                {
                    data.CombinePoint(nearPoint, target);
                    if (target == null || data.Contins(target) == false)
                    {
                        setState(FreeState2D.NAME);
                    }
                }
                else if (nearWall != null)
                {
                    data.CombinePointWall(target, nearWall);
                }

                targetList.Clear();
                ///target 临边新位置产生的切分（不包括临边顶点的切分）
                List<Point> list = data.GetNearestPoints(target);
                for (int k = 0; k < list.Count; k++)
                {
                    WallData targetWall = data.GetWall(list[k], target);
                    targetList.Add(targetWall);
                    ///target 临边切分其他墙体
                    for (int i = 0; i < data.wallList.Count; i++)
                    {
                        WallData item = data.wallList[i];
                        if (item == targetWall) { continue; }
                        bool p1OnItem = false;
                        bool p2OnItem = false;
                        bool targetCrossItem = false;
                        if (item.Contines(targetWall.point1) == true || item.Contines(targetWall.point2) == true)
                        {
                            if (item.Contines(targetWall.point1) == false && wallfunc.PointOnWall(targetWall.point1, item))
                            {
                                p1OnItem = true;
                            }

                            if (item.Contines(targetWall.point2) == false && wallfunc.PointOnWall(targetWall.point2, item))
                            {
                                p2OnItem = true;
                            }
                        }
                        else
                        {
                            if (wallfunc.PointOnWall(targetWall.point1, item))
                            {
                                p1OnItem = true;
                            }
                            if (wallfunc.PointOnWall(targetWall.point2, item))
                            {
                                p2OnItem = true;
                            }
                            if (p1OnItem == false && p2OnItem == false)
                            {
                                if (wallfunc.PointOnWall(item.point1, targetWall) == false && wallfunc.PointOnWall(item.point2, targetWall) == false && linefunc.GetCross(item, targetWall))
                                {
                                    targetCrossItem = true;
                                }
                            }
                        }

                        if (p1OnItem == true && p2OnItem == true)
                        {
                            data.RemovePoint(targetWall.point1);
                            data.RemovePoint(targetWall.point2);
                            targetWall = item;
                        }
                        else if (p1OnItem == true)
                        {
                            data.CombinePointWall(targetWall.point1, item);
                        }
                        else if (p2OnItem == true)
                        {
                            data.CombinePointWall(targetWall.point2, item);
                        }
                        else if (targetCrossItem == true)
                        {
                            bool isParallel = linefunc.IsTwoLineParallel(item, targetWall);
                            if (isParallel == false)///targetCrossItem == true 好像永远不会出现item, target平行的情况
                            {
                                Vector2 crossP = linefunc.GetTwoLineCrossPoint(item, targetWall, Vector2.zero);
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
                }

                ///其他点切分target
                for (int i = 0; i < data.pointList.Count; i++)
                {
                    Point point = data.pointList[i];
                    for (int k = 0; k < targetList.Count; k++)
                    {
                        WallData wall = targetList[k];
                        if (point == wall.point1 || point == wall.point2)
                        {
                            continue;
                        }
                        if (wallfunc.PointOnWall(point, wall))
                        {
                            List<WallData> newTargetwalls = data.CombinePointWall(point, wall);
                            targetList.RemoveAt(k);
                            targetList.InsertRange(k, newTargetwalls);
                            k--;
                        }
                    }
                }

                roomfunc.ForceRefreshRoomData(data);
            }

            if (up == false && equl == true)
            {
                return;
            }

            RefreshView();
            if (target == null || data.Contins(target) == false)
            {
                setState(FreeState2D.NAME);
                return;
            }
        }
    }

    protected override void RefreshView()
    {
        base.RefreshView();
    }

    public override void exit()
    {
        pointMachine.setState(EditTypeOnSelect.Free);
        base.exit();
    }


}
