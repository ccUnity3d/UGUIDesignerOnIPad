using UnityEngine;
using System.Collections;
using System;

public class SelectGoods3D_MoveUpState : SelectGoods3D_State
{
    //public override bool needCloseGoodsUpdate
    //{
    //    get
    //    {
    //        return true;
    //    }
    //}
    protected Mode2DPrefabs prefabs
    {
        get { return Mode2DPrefabs.Instance; }
    }
    protected DefaultSettings settings
    {
        get { return DefaultSettings.Instance; }
    }

    private float startHeight;
    private Vector2 startPos;
    private float startY;

    private Vector2 oldPos;
    private float multiply;

    public override void enter()
    {
        base.enter();
        machine.selectGoods3DState.onhandle = true;
        if (GlobalConfig.isMyDebug == true)
        {
#if !UNITY_ANDROID && !UNITY_IPHONE  //安卓  
            if (Input.GetMouseButton(0) == false)
            {
                setState(EditTypeOnSelect.Free);
                return;
            }

            startPos = Input.mousePosition;
            oldPos = startPos;

            MyTickManager.Add(update);
#else
            if (Input.touchCount != 1)
            {
                setState(EditTypeOnSelect.Free);
                return;
            }

            startPos = Input.GetTouch(0).position;

            MyTickManager.Add(phoneUpdate);
#endif
        }
        else {
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                if (Input.GetMouseButton(0) == false)
                {
                    setState(EditTypeOnSelect.Free);
                    return;
                }

                startPos = Input.mousePosition;
                oldPos = startPos;

                MyTickManager.Add(update);
            }
            else {

                if (Input.touchCount != 1)
                {
                    setState(EditTypeOnSelect.Free);
                    return;
                }

                startPos = Input.GetTouch(0).position;

                MyTickManager.Add(phoneUpdate);
            }
        }
        

        startHeight = target.height;
        Ray ray = prefabs.mainCamera.ScreenPointToRay(startPos);
        startY = ray.origin.y;
        

        Vector3 cameraPos = prefabs.mainCamera.transform.position;
        Vector3 pos = machine.selectGoods3DState.GetDisFloorPos();
        float dis = Vector3.Distance(cameraPos, pos);
        Vector3 screenPos = prefabs.mainCamera.ScreenPointToRay(Vector3.right*Screen.width/2 + Vector3.up*Screen.height/2).origin;
        float screenDis = Vector3.Distance(cameraPos, screenPos);
        multiply = dis / screenDis * 2;

    }

    public override void exit()
    {
        machine.selectGoods3DState.onhandle = false;
        if (GlobalConfig.isMyDebug == true)
        {
#if !UNITY_ANDROID && !UNITY_IPHONE  //安卓  
            MyTickManager.Remove(update);
#else
            MyTickManager.Remove(phoneUpdate);
#endif
        }
        else {
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                MyTickManager.Remove(update);
            }
            else {
                MyTickManager.Remove(phoneUpdate);
            }
        }
        base.exit();
    }
    private void update()
    {
        if (Input.GetMouseButton(0) == false)
        {
            setState(EditTypeOnSelect.Free);
            return;
        }

        Vector2 pos = Input.mousePosition;
        if (oldPos == pos)
        {
            return;
        }
        oldPos = pos;

        Ray ray = prefabs.mainCamera.ScreenPointToRay(pos);
        CaculateMove(ray.origin.y);

    }

    private void phoneUpdate()
    {
        if (Input.touchCount != 1)
        {
            setState(EditTypeOnSelect.Free);
            return;
        }

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Moved)
        {
            return;
        }

        Vector2 pos = touch.position;
        Ray ray = prefabs.mainCamera.ScreenPointToRay(pos);
        CaculateMove(ray.origin.y);
    }

    private void CaculateMove(float y2)
    {
        if (saved == false) saved = true;

        float deltaH = (y2 - startY) * multiply;

        DoMove(deltaH);
    }

    private void DoMove(float deltaH)
    {
        target.height = startHeight + deltaH;
        if (target.height < settings.goodsMinheight)
        {
            target.height = settings.goodsMinheight;
        }else if (target.height > settings.goodsMaxheight)
        {
            target.height = settings.goodsMaxheight;
        }
        machine.selectGoods3DState.Refresh();
    }

}
