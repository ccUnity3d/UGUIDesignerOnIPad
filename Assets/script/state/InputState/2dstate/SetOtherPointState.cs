using UnityEngine;
using foundation;
using System.Collections.Generic;
using System;

public class SetOtherPointState : InputState2D{

    public const string NAME = "SetOtherPointState";
    public SetOtherPointState():base(NAME)
    {

    }

    private Vector2 lastInputPos = Vector2.zero;
    private List<WallData> targets = new List<WallData>();
    private WallData targetWall = null;
    private bool pointOk = true;

    public override void enter()
    {
        base.enter();
        pointOk = true;
        //machine.addEventListener(InputStateEvent.SetWallData, SetWallData);
    }

    public override void enterPhone()
    {
        base.enterPhone();
        cursor = prefabs.GetNewInstance_circle();
        Vector3 v3 = phoneHelperData.currentPoint.pos;
        v3.z = 0;
        cursor.position = v3;
    }

    public override void enterNotPhone()
    {
        base.enterNotPhone();
        cursor = prefabs.GetNewInstance_circle();
        Vector3 v3 = GetScreenToWorldPos(Input.mousePosition);
        v3.z = 0;
        cursor.position = v3;
    }

    public override void mUpdate()
    {
        base.mUpdate();

        if (Input.GetMouseButtonDown(1))
        {
            if (helperData.helpLineList.Count <= 0)
            {
                setState(FreeState2D.NAME);
                setState(SetFirstPointState.NAME);
                //machine.dispatchEvent(new InputStateEvent(InputStateEvent.DrawWallEnd, true));
            }
            else
            {
                helperData.endOnWallType = OnWallType.NotOnWall;
                //machine.dispatchEvent(new InputStateEvent(InputStateEvent.DrawWallEnd, false));
                ChoseWallDirect(); 
                setState(SetFirstPointState.NAME);
            }
            return;
        }

        Vector2 v2 = GetScreenToWorldPos(Input.mousePosition);
        if (lastInputPos == v2 && Input.GetMouseButtonDown(0) == false &&
            Input.GetKeyDown(KeyCode.Return) == false && helpView2D.getInputChange() == false)
        {
            return;
        }

        lastInputPos = v2;
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("000");
        //}

        Point point = null;
        OnWallType type = OnWallType.NotOnWall;
        bool crossed = false;

        v2 = AdjustPoint(ref point, ref crossed, ref type, v2);

        if (crossed == false && Input.GetMouseButtonDown(0))
        {
            if (uguiHitUI.uiHited == false)
            {
                if (point == null)
                {
                    point = new Point();
                    point.pos = v2;
                    data.AddPoint(point);

                    ///点在helpLine上时候切割helpLine
                    WallData wall = null;
                    Vector2 pos = Vector2.zero;
                    for (int i = 0; i < helperData.helpLineList.Count; i++)
                    {
                        WallData itemWall = helperData.helpLineList[i];
                        if (itemWall.GetDis(v2) < 0.01f)
                        {
                            Vector2 itemPos = itemWall.GetDisPoint(v2);
                            if (wallfunc.PointOnWall(itemPos, itemWall))
                            {
                                wall = itemWall;
                                pos = itemPos;
                                break;
                            }
                        }
                    }

                    if (wall != null)
                    {
                        helperData.helpLineList.Remove(wall);
                        WallData data1 = new WallData();
                        data1.point1 = wall.point1;
                        data1.point2 = point;
                        data1.height = wall.height;
                        data1.width = wall.width;
                        helperData.helpLineList.Add(data1);

                        WallData data2 = new WallData();
                        data2.point1 = point;
                        data2.point2 = wall.point2;
                        data2.height = wall.height;
                        data2.width = wall.width;
                        helperData.helpLineList.Add(data2);
                    }
                }
                else
                {
                    if (point == helperData.currentPoint)
                    {
                        Debug.Log("point == helperData.currentPoint");
                        return;
                    }
                    for (int i = 0; i < helperData.helpLineList.Count; i++)
                    {
                        WallData wall = helperData.helpLineList[i];
                        if (wall.equle(point, helperData.currentPoint))
                        {
                            Debug.Log("the line has already contions");
                            return;
                        }
                    }
                }

                WallData line = new WallData();
                line.point1 = helperData.currentPoint;
                line.point2 = point;
                line.height = defaultSetting.DefaultHeight;
                line.width = defaultSetting.DefaultWidth;
                helperData.helpLineList.Add(line);
                helperData.currentPoint = point;

                if (type != OnWallType.NotOnWall)
                {
                    helperData.endOnWallType = type;
                    //machine.dispatchEvent(new InputStateEvent(InputStateEvent.DrawWallEnd, false));
                    ChoseWallDirect(false);
                    if (GetRoom(line) == true)
                    {
                        mianpageMachine.setState(MainPageFreeState.Name);
                        //setState(FreeState2D.NAME);
                    }                    
                    return;
                }
            }
        }

        cursor.position = v2;
        helpView2D.RefreshView(v2);

        if (crossed == true)
        {
            if (helperData.mouseLineDisable == false)
            {
                helpView2D.SetMouseLineDisable();
                helperData.mouseLineDisable = true;
            }
        }
        else {
            if (helperData.mouseLineDisable == true)
            {
                helpView2D.SetMouseLineable();
                helperData.mouseLineDisable = false;
            }
        }
    }

    private bool GetRoom(WallData targetWall)
    {
        if (targetWall == null) return false;
        for (int i = 0; i < data.roomList.Count; i++)
        {
            if (data.roomList[i].sideList.Contains(targetWall.point1To2Data))
            {
                return true;
            }
            if (data.roomList[i].sideList.Contains(targetWall.point2To1Data))
            {
                return true;
            }
        }
        return false;
    }

    public override void mPhoneUpdate()//
    {
        base.mPhoneUpdate();
        if (Input.touchCount == 0 || uguiHitUI.uiHited == true)
        {
            return;
        }
        if (pointOk == false)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);
        Vector2 v2 = GetScreenToWorldPos(touch.position);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
            default:
                return;
        }

        undoHelper.save();

        bool cross = false;
        Point P2 = null;
        OnWallType type = OnWallType.NotOnWall;
        v2 = AdjustPhonePoint(ref P2, ref cross, ref type, v2);

        List<Point> nearestList = data.GetNearestPoints(phoneHelperData.currentPoint);
        if (P2 == null)
        {
            P2 = new Point();
            P2.pos = v2;
            data.AddPoint(P2);

            for (int i = 0; i < nearestList.Count; i++)
            {
                WallData wall = data.GetWall(nearestList[i], phoneHelperData.currentPoint);
                if (wallfunc.PointOnWall(P2, wall))
                {
                    data.CombinePointWall(P2, wall);
                    break;
                }
            }
        }
        else
        {
            if (P2 == phoneHelperData.currentPoint)
            {
                return;
            }
            else if (nearestList.Contains(P2))
            {
                phoneHelperData.currentPoint = P2;
                mianpageMachine.setState(MainPageFreeState.Name);
                //setState(FreeState2D.NAME);
                return;
            }
        }
        targetWall = data.AddWall(phoneHelperData.currentPoint, P2, defaultSetting.DefaultHeight, defaultSetting.DefaultWidth);

        pointOk = false;
        SetWallController.Instance.Open(SetLength, targetWall.length, targetWall.width, targetWall.height, true, false, false);

        CaculatCut(targetWall);

        phoneHelperData.currentPoint = P2;

        if (cursor != null) GameObject.Destroy(cursor.gameObject);

        if (type != OnWallType.NotOnWall || cross == true)
        {
            if (GetRoom(targetWall) == true)
            {
                mianpageMachine.setState(MainPageFreeState.Name);
                //setState(FreeState2D.NAME);
                //setState(SetFirstPointState.NAME);
            }
        }
    }

    /// <summary>
    /// targetWall 切割判断 刷新视图
    /// </summary>
    private void CaculatCut(WallData targetWall)
    {
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
                    if (wallfunc.PointOnWall(item.point1, targetWall) == false
                        && wallfunc.PointOnWall(item.point2, targetWall) == false
                        && linefunc.GetCross(item, targetWall))
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
                if (isParallel == false)
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

        targets.Clear();
        targets.Add(targetWall);
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

        roomfunc.ForceRefreshRoomData(data);

        RefreshView();
    }

    private void SetLength(float arg1, float arg2, float arg3)
    {
        pointOk = true;
        if (targetWall == null) return;
        float len = arg1 / (int)defaultSetting.DefaultUnit;
        if (len == targetWall.length)
        {
            return;
        }
        undoHelper.save();
        targetWall.point2.pos = len / targetWall.length * (targetWall.point2.pos - targetWall.point1.pos) + targetWall.point1.pos;
        CaculatCut(targetWall);
    }


    private Vector2 AdjustPhonePoint(ref Point point, ref bool crossed, ref OnWallType type, Vector2 inputPos)
    {
        Vector2 v2 = inputPos;
        float minDis = 0.09f;
        //bool nearPointAxis = false;
        WallData nearWall = null;
        for (int i = 0; i < data.pointList.Count; i++)
        {
            float dis = Vector2.SqrMagnitude(data.pointList[i].pos - inputPos);
            if (dis < minDis)
            {
                point = data.pointList[i];
                v2 = point.pos;
                minDis = dis;
                type = OnWallType.OnWallPoint;
            }
        }

        bool obliqueChooseX = false;
        if (point == null)
        {
            if (defaultSetting.oblique == false)
            {
                float xDis = v2.x - phoneHelperData.currentPoint.pos.x;
                float yDis = v2.y - phoneHelperData.currentPoint.pos.y;
                if (Mathf.Abs(xDis) <= Mathf.Abs(yDis))
                {
                    v2.x = inputPos.x = phoneHelperData.currentPoint.pos.x;
                    obliqueChooseX = true;
                }
                else {
                    v2.y = inputPos.y = phoneHelperData.currentPoint.pos.y;
                }
            }

            minDis = 0.09f;
            for (int i = 0; i < data.pointList.Count; i++)
            {
                float dis = Vector2.SqrMagnitude(data.pointList[i].pos - inputPos);
                if (dis < minDis)
                {
                    point = data.pointList[i];
                    v2 = point.pos;
                    minDis = dis;
                    type = OnWallType.OnWallPoint;
                }
            }
        }

        if (point == null)
        {
            minDis = 0.3f;
            for (int i = 0; i < data.wallList.Count; i++)
            {
                WallData item = data.wallList[i];
                float dis = item.GetDis(inputPos);
                if (dis > minDis)
                {
                    continue;
                }
                //minDis = dis;
                Vector2 nearV2 = item.GetDisPoint(inputPos);
                if (wallfunc.PointOnWall(nearV2, item))
                {
                    type = OnWallType.OnWallSide;
                    nearWall = item;
                    v2 = nearV2;
                }
            }
        }
        
        if (point == null && nearWall == null)
        {
            minDis = 0.2f;
            for (int i = 0; i < data.pointList.Count; i++)
            {
                Point item = data.pointList[i];
                if (Mathf.Abs(inputPos.x - item.pos.x) < minDis)
                {
                    if (defaultSetting.oblique == true || obliqueChooseX == false)
                    {
                        v2.x = item.pos.x;
                    }
                    //nearPointAxis = true;
                }
                if (Mathf.Abs(inputPos.y - item.pos.y) < minDis)
                {
                    if (defaultSetting.oblique == true || obliqueChooseX == true)
                    {
                        v2.y = item.pos.y;
                    }
                    //nearPointAxis = true;
                }
            }
        }

        for (int i = 0; i < data.wallList.Count; i++)
        {
            WallData wall = data.wallList[i];

            if (point != null && wall.Contines(point))
            {
                continue;
            }
            else if (nearWall != null && wall == nearWall)
            {
                continue;
            }

            if (linefunc.GetCross(wall, phoneHelperData.currentPoint, v2) == true)
            {
                crossed = true;
            }
        }
        return v2;
    }

    //private void SetWallData(EventX obj)
    //{

    //}

    /// <summary>
    /// 两向量夹角
    /// </summary>
    /// <param name="vector1"></param>
    /// <param name="vector2"></param>
    /// <returns></returns>
    private float GetAngle(Vector2 vector1, Vector2 vector2)
    {
        float cosValue = Vector2.Dot(vector1, vector2)/ (vector1.magnitude*vector2.magnitude);
        float angleValue = Mathf.Acos(cosValue);
        angleValue = angleValue * 180f / Mathf.PI;
        return angleValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="mousePos">鼠标位置</param>
    /// <returns></returns>
    private Vector2 AdjustPoint(ref Point point, ref bool crossed, ref OnWallType type, Vector2 mousePos)
    {
        Vector2 v2 = mousePos;
        float minDis = 0.09f;
        //bool nearPointAxis = false;
        WallData nearWall = null;
        WallData nearHelpWall = null;
        for (int i = 0; i < data.pointList.Count; i++)
        {
            float dis = Vector2.SqrMagnitude(data.pointList[i].pos - mousePos);
            if (dis < minDis)
            {
                point = data.pointList[i];
                v2 = point.pos;
                minDis = dis;

                type = OnWallType.OnJustHelpWallPoint;
                for (int j = 0; j < data.wallList.Count; j++)
                {
                    if (data.wallList[j].Contines(point))
                    {
                        type = OnWallType.OnWallPoint;
                        break;
                    }
                }                
            }
        }
        bool obliqueChooseX = false;
        if (point == null)
        {
            if (defaultSetting.oblique == false)
            {
                float xDis = v2.x - helperData.currentPoint.pos.x;
                float yDis = v2.y - helperData.currentPoint.pos.y;
                if (Mathf.Abs(xDis) <= Mathf.Abs(yDis))
                {
                    v2.x = mousePos.x = helperData.currentPoint.pos.x;
                    obliqueChooseX = true;
                }
                else {
                    v2.y = mousePos.y = helperData.currentPoint.pos.y;
                }

                minDis = 0.09f;
                for (int i = 0; i < data.pointList.Count; i++)
                {
                    float dis = Vector2.SqrMagnitude(data.pointList[i].pos - mousePos);
                    if (dis < minDis)
                    {
                        point = data.pointList[i];
                        v2 = point.pos;
                        minDis = dis;

                        type = OnWallType.OnJustHelpWallPoint;
                        for (int j = 0; j < data.wallList.Count; j++)
                        {
                            if (data.wallList[j].Contines(point))
                            {
                                type = OnWallType.OnWallPoint;
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (point == null)
        {
            minDis = 0.3f;
            for (int i = 0; i < data.wallList.Count; i++)
            {
                WallData item = data.wallList[i];
                float dis = item.GetDis(mousePos);
                if (dis > minDis)
                {
                    continue;
                }
                //minDis = dis;
                Vector2 nearV2 = item.GetDisPoint(mousePos);
                if (wallfunc.PointOnWall(nearV2, item))
                {
                    type = OnWallType.OnWallSide;
                    nearWall = item;
                    v2 = nearV2;
                }
            }
        }

        if (point == null && nearWall == null)
        {
            minDis = 0.25f;
            for (int i = 0; i < helperData.helpLineList.Count; i++)
            {
                WallData item = helperData.helpLineList[i];
                float dis = item.GetDis(mousePos);
                if (dis > minDis)
                {
                    continue;
                }
                //minDis = dis;
                Vector2 nearV2 = item.GetDisPoint(mousePos);
                if (wallfunc.PointOnWall(nearV2, item))
                {
                    type = OnWallType.OnHelpWallSide;
                    nearHelpWall = item;
                    v2 = nearV2;
                }
            }
        }

        if (helperData.helpLineList.Count >= 1)
        {
            //向量1
            Vector2 vector1 = v2 - helperData.currentPoint.pos;
            WallData wall = helperData.helpLineList[helperData.helpLineList.Count - 1];
            Vector2 vector2 = wall.point1.pos - wall.point2.pos;
            float angle = GetAngle(vector1, vector2);
            if (angle < 15)
            {
                crossed = true;
            }
        }

        if (point == null && nearWall == null && nearHelpWall == null)
        {
            minDis = 0.2f;
            for (int i = 0; i < data.pointList.Count; i++)
            {
                Point item = data.pointList[i];
                if (Mathf.Abs(mousePos.x - item.pos.x) < minDis)
                {
                    if(defaultSetting.oblique == true || obliqueChooseX == false)
                    {
                        v2.x = item.pos.x;
                    }
                    //nearPointAxis = true;
                }
                if (Mathf.Abs(mousePos.y - item.pos.y) < minDis)
                {
                    if (defaultSetting.oblique == true || obliqueChooseX == true)
                    {
                        v2.y = item.pos.y;
                    }
                    //nearPointAxis = true;
                }
            }
        }

        for (int i = 0; i < data.wallList.Count; i++)
        {
            WallData wall = data.wallList[i];

            if (point != null && wall.Contines(point))
            {
                continue; 
            }
            else if (nearWall != null && wall == nearWall)
            {
                continue;
            }

            if (linefunc.GetCross(wall, helperData.currentPoint, v2) == true)
            {
                crossed = true;
            }
        }

        for (int i = 0; i < helperData.helpLineList.Count; i++)
        {
            WallData wall = helperData.helpLineList[i];

            if (point != null && wall.Contines(point))
            {
                continue;
            }
            else if (nearHelpWall != null && wall == nearHelpWall)
            {
                continue;
            }

            if (linefunc.GetCross(wall, helperData.currentPoint, v2) == true)
            {
                crossed = true;
            }
        }

        return v2;
    }

    public override void exit()
    {
        //TODO：mainPageController.CloseInput
        
        if (cursor != null) GameObject.Destroy(cursor.gameObject);
        base.exit();
    }

    public override void exitPhone()
    {
        SetWallController.Instance.Close();
        phoneHelperData.sleep();
        for (int i = 0; i < data.pointList.Count; i++)
        {
            bool contine = false;
            Point item = data.pointList[i];
            for (int j = 0; j < data.wallList.Count; j++)
            {
                WallData wall = data.wallList[j];
                if (wall.Contines(item))
                {
                    contine = true;
                    break;
                }
            }
            if (contine == false)
            {
                data.RemovePoint(item);
                i--;
            }
        }
        base.exitPhone();
    }

    public override void exitNotPhone()
    {
        for (int i = 0; i < data.pointList.Count; i++)
        {
            bool contine = false;
            Point item = data.pointList[i];
            for (int j = 0; j < data.wallList.Count; j++)
            {
                WallData wall = data.wallList[j];
                if (wall.Contines(item))
                {
                    contine = true;
                    break;
                }
            }
            //for (int j = 0; j < helperData.helpLineList.Count; j++)
            //{
            //    WallData wall = helperData.helpLineList[j];
            //    if (wall.Contines(item))
            //    {
            //        contine = true;
            //        break;
            //    }
            //}
            if (contine == false)
            {
                data.RemovePoint(item);
                i--;
            }
        }
        base.exitNotPhone();
    }


    public void ChoseWallDirect(bool forceClose = true)
    {
        undoHelper.save();

        Point p = helperData.currentPoint;

        helpView2D.SetWallType(0);

        roomfunc.ForceRefreshRoomData(data);//RefreshRoomData();
        
        helpView2D.sleep();

        RefreshView();

        helperData.currentPoint = p;

        if (forceClose == true)
        {
            setState(FreeState2D.NAME);
        }
    }


    /// <summary>
    /// 根据helpLineList推导出新增room和被改变的room
    /// </summary>
    //private void RefreshRoomData()
    //{

    //    if (helperData.startOnWallType != OnWallType.OnWallSide)
    //    {
    //        if (helperData.endOnWallType == OnWallType.NotOnWall)
    //        {
    //            return;
    //        }

    //        if (helperData.startOnWallType == OnWallType.NotOnWall && helperData.endOnWallType == OnWallType.OnWallPoint)
    //        {
    //            return;
    //        }

    //        if (helperData.endOnWallType == OnWallType.OnJustHelpWallPoint || helperData.endOnWallType == OnWallType.OnHelpWallSide)
    //        {
    //            WallData wall = helperData.helpLineList[helperData.helpLineList.Count - 1];
    //            RoomData room;
    //            room = roomfunc.GetRoom(wall);
    //            if (room != null) data.roomList.Add(room);
    //            return;
    //        }
    //    }
    //    ///情况太多而且复杂 直接所有房间重新计算  也可以上面全注释掉所有情况全部走这里
    //    roomfunc.ForceRefreshRoomData(data);
    //}
}
