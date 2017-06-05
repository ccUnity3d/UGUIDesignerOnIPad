
using System;
using UnityEngine;

public class CameraState2D : CameraState
{
    public const string NAME = "CameraState2D";
    public CameraState2D():base(NAME)
    {
        defaultData = new CameraData(LayerMask.GetMask("UI"), Vector3.forward * -30, Vector3.zero, true, 5);
        data = new CameraData(LayerMask.GetMask("UI"), Vector3.forward * -30, Vector3.zero, true, 5);
    }

    private float oldDis = 0;
    private int currentCount = 0;
    private Vector2 oldPlacePos;

    private View2D view2D
    {
        get {
            return View2D.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        data.setCamera(target);
        currentCount = 0;
    }

    public override void mUpdate()
    {
        base.mUpdate();
        float delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta != 0)
        {
            //ErrorDisplay.Log(delta);
            target.orthographicSize += delta;
            if (target.orthographicSize < 2)
            {
                target.orthographicSize = 2;
            }
            else if (Camera.main.orthographicSize > 50)
            {
                target.orthographicSize = 50;
            }
        }

        Vector3 v3 = Input.mousePosition;
        if (Input.GetMouseButtonDown(2))
        {
            oldPos = v3;
        }
        if (Input.GetMouseButton(2))
        {
            if (oldPos.x == v3.x && oldPos.y == v3.y)
            {
                return;
            }

            Vector2 offset = v3 - oldPos;
            oldPos = v3;
            //Debug.Log(offset + " / " + v2);
            target.transform.position -= (Vector3)offset / 100f;
            MyCallLater.Add(0.1f, RefreshView);
        }
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
        if (uguiHitUI.uiHited == true)
        {
            currentCount = 0;
            return;
        }

        bool isTwoPointMove =   inputMachine.CurrentState is PlaceGoodsState2D || inputMachine.CurrentState is SelectState 
            || inputMachine.CurrentState is SetTempletState;

        if (currentCount != Input.touchCount)
        {
            currentCount = Input.touchCount;
            if (currentCount == 1)
            {
                Touch touch0 = Input.GetTouch(0);
                oldPos = touch0.position;
            }
            else if (currentCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);
                oldDis = (touch0.position - touch1.position).magnitude;
                if(isTwoPointMove) oldPlacePos = (touch0.position + touch1.position) / 2;
            }
        }

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 v3 = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                oldPos = v3;
            }
            else if (touch.phase == TouchPhase.Moved && isTwoPointMove == false)
            {
                if (oldPos.x == v3.x && oldPos.y == v3.y)
                {
                    return;
                }

                Vector2 offset = v3 - oldPos;
                oldPos = v3;
                //Debug.Log(offset + " / " + v2);
                target.transform.position -= (Vector3)offset / 100f;
                RefreshView();
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                oldDis = (touch0.position - touch1.position).magnitude;
                if (isTwoPointMove) oldPlacePos = (touch0.position + touch1.position) / 2;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float dis = (touch0.position - touch1.position).magnitude;
                float delta = (dis - oldDis) / 100f;
                if (delta != 0)
                {
                    //ErrorDisplay.Log(delta);
                    target.orthographicSize -= delta;
                    if (target.orthographicSize < 2)
                    {
                        target.orthographicSize = 2;
                    }
                    else if (Camera.main.orthographicSize > 50)
                    {
                        target.orthographicSize = 50;
                    }
                    oldDis = dis;
                }

                if (isTwoPointMove == true)
                {
                    Vector2 pos = (touch0.position + touch1.position) / 2;
                    Vector2 offset = target.ScreenToWorldPoint(pos) - target.ScreenToWorldPoint(oldPlacePos);
                    if (offset != Vector2.zero)
                    {
                        Vector3 moved = -offset;
                        target.transform.position += moved;
                        oldPlacePos = pos;
                        RefreshView();
                    }
                }

            }

        }
    }

    public override void ResetCamera(MyEvent obj)
    {
        ResetCameraData(null);
        enter();
    }

    public override void ResetCameraData(Vector2 pos)
    {
        defaultData.position = (Vector3)pos + Vector3.forward * defaultData.position.z;
        base.ResetCameraData(pos);
    }

    private void RefreshView()
    {
        view2D.RefreshView();
    }

    public override void exit()
    {
        data.ReadCamera(target);
        base.exit();
    }

}
