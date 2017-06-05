using UnityEngine;
using System.Collections;
using System;

public class SelectGoods3D_RotateState : SelectGoods3D_State
{
    private Vector2 startPos;
    private Vector2 oldPos;
    private float startAngle = 0;
    private Vector2 rotateToScreenPos;
    private GameObject rotateicon;

    public Mode2DPrefabs prefabs {
        get
        {
            return Mode2DPrefabs.Instance;
        }
    }

    public LineHelpFunc lineFunx
    {
        get
        {
            return LineHelpFunc.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        startAngle = target.rotate;
        rotateToScreenPos = prefabs.mainCamera.WorldToScreenPoint(target.GetV3Withheight());

        if (GlobalConfig.isMyDebug == true)
        {
#if !UNITY_ANDROID && !UNITY_IPHONE  //安卓  
             if (Input.GetMouseButton(0) == false)
            {
                setState(EditTypeOnSelect.Free);
                return;
            }

            oldPos = startPos = Input.mousePosition;
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
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount != 1)
                {
                    setState(EditTypeOnSelect.Free);
                    return;
                }

                startPos = Input.GetTouch(0).position;

                MyTickManager.Add(phoneUpdate);
            }
            else {
                if (Input.GetMouseButton(0) == false)
                {
                    setState(EditTypeOnSelect.Free);
                    return;
                }

                oldPos = startPos = Input.mousePosition;
                MyTickManager.Add(update);
            }
        }

        if (rotateicon == null) rotateicon = prefabs.GetNewInstance_rotateicon();
        Vector3 worldpos = prefabs.uiCamera.ScreenToWorldPoint(startPos);
        worldpos.z = 0;
        rotateicon.transform.position = worldpos;
        rotateicon.SetActive(true);

    }

    public override void exit()
    {
        if (GlobalConfig.isMyDebug == true)
        {
#if !UNITY_ANDROID && !UNITY_IPHONE  //安卓  
            MyTickManager.Remove(update);
#else
            MyTickManager.Remove(phoneUpdate);
#endif
        }
        else {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                MyTickManager.Remove(phoneUpdate);
            }
            else {
                MyTickManager.Remove(update);
            }
        }
        if (rotateicon != null) rotateicon.SetActive(false);
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

        Rotate(pos);
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

        Rotate(pos);
    }


    private void Rotate(Vector2 pos)
    {
        if (saved == false) saved = true;
        float deltaAngle = lineFunx.GetClockAngle(startPos, rotateToScreenPos, pos);
        float rotate = startAngle + deltaAngle;

        //45度5%误差内吸附
        float multi = rotate / 45;
        int times = (int)multi;
        float other = multi - times;
        if (other > 0.9f)
        {
            rotate = (times + 1) * 45;
        }
        else if (other < 0.1f)
        {
            rotate = times * 45;
        }

        target.rotate = rotate;
        machine.selectGoods3DState.Refresh();
        Vector3 worldpos = prefabs.uiCamera.ScreenToWorldPoint(pos);
        worldpos.z = 0;
        rotateicon.transform.position = worldpos;
        //float Angle = -targetObj.transform.rotation.eulerAngles.y;

    }

}
