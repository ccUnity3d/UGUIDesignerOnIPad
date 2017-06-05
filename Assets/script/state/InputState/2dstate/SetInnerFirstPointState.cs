using UnityEngine;
using System.Collections;

public class SetInnerFirstPointState : InputState2D
{
    public const string NAME = "SetInnerFirstPointState";
    private bool moved;

    public SetInnerFirstPointState():base(NAME)
    {

    }
    public override void enter()
    {
        base.enter();
    }

    public override void enterNotPhone()
    {
        base.enterNotPhone();
        helperData.sleep();
    }

    public override void enterPhone()
    {
        base.enterPhone();
        phoneHelperData.sleep();
    }



    public override void mUpdate()
    {
        base.mUpdate();

        if (Input.GetMouseButtonDown(1))
        {
            //machine.dispatchEvent(new InputStateEvent(InputStateEvent.DrawWallEnd, true));
            return;
        }

        Vector2 v2 = GetScreenToWorldPos(Input.mousePosition);
        Point point = null;
        OnWallType type = OnWallType.NotOnWall;
        v2 = AdjustPoint(ref point, ref type, v2);

        if (Input.GetMouseButtonDown(0))
        {
            if (uguiHitUI.uiHited == true)
            {
                return;
            }
            if (point == null)
            {
                point = new Point();
                point.pos = v2;
                data.AddPoint(point);
            }

            helperData.startOnWallType = type;
            helperData.currentPoint = point;

            setState(SetInnerOtherPointState.NAME);
        }
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
        if (uguiHitUI.uiHited == true || Input.touchCount != 1)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began)
        {
            return;
        }
        Vector2 v2 = GetScreenToWorldPos(touch.position);
        Point point = null;
        OnWallType type = OnWallType.NotOnWall;
        v2 = AdjustPoint(ref point, ref type, v2);
        if (point == null)
        {
            point = new Point();
            point.pos = v2;
            data.AddPoint(point);
        }
        phoneHelperData.currentPoint = point;
        setState(SetInnerOtherPointState.NAME);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    /// <param name="mousePos">鼠标位置</param>
    /// <returns></returns>
    private Vector2 AdjustPoint(ref Point point, ref OnWallType type, Vector2 mousePos)
    {
        Vector2 v2 = mousePos;
        float minDis = 0.06f;
        for (int i = 0; i < data.pointList.Count; i++)
        {
            float dis = Vector2.SqrMagnitude(data.pointList[i].pos - mousePos);
            if (dis < minDis)
            {
                point = data.pointList[i];
                v2 = point.pos;
                minDis = dis;
                type = OnWallType.OnWallPoint;
            }
        }

        if (point == null)
        {
            minDis = 0.25f;
            for (int i = 0; i < data.wallList.Count; i++)
            {
                WallData wall = data.wallList[i];
                float dis = wall.GetDis(mousePos);
                if (dis < minDis)
                {
                    v2 = wall.GetDisPoint(mousePos);
                    minDis = dis;

                    if (wallfunc.PointOnWall(v2, wall))
                    {
                        type = OnWallType.OnWallSide;
                    }
                }
            }
        }
        return v2;
    }

    public override void exit()
    {
        base.exit();
    }

}
