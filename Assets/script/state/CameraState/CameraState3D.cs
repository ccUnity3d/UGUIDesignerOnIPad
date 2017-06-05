using UnityEngine;
using System.Collections;
using foundation;
using System;
using System.Collections.Generic;

public class CameraState3D : CameraState
{
    public const string NAME = "CameraState3D";
    public CameraState3D():base(NAME)
    {
        defaultData = new CameraData(LayerMask.GetMask("Default"), Vector3.forward * -1.3f + Vector3.up * 7.3f, Vector3.right * 53);
        data = new CameraData(LayerMask.GetMask("Default"), Vector3.forward * -1.3f + Vector3.up * 7.3f, Vector3.right * 53);
        followData = new CameraData(LayerMask.GetMask("Default"), Vector3.up * 1 + Vector3.forward * -1, Vector3.right * 10);
        followDefaultData = new CameraData(LayerMask.GetMask("Default"), Vector3.up * 1 + Vector3.forward * -1, Vector3.right * 10);
    }

    private View3D view3D
    {
        get { return View3D.Instance; }
    }

    protected readonly float rotateMultiX = 0.2f;//0.2
    protected readonly float rotateMultiY = 0.1f;//0.2
    protected readonly float scaleMulti = 0.1f;//0.01

    protected readonly float rotateMultiXEditor = 0.2f;//0.2
    protected readonly float rotateMultiYEditor = 0.1f;//0.2
    protected readonly float scaleMultiEditor = 0.3f;//0.01

    //protected readonly float rotateMultiXEditor = 0.2f;//0.2
    //protected readonly float rotateMultiYEditor = 0.2f;//0.2
    //protected readonly float scaleMultiEditor = 0.05f;//0.01

    /// <summary>
    /// 相机视野聚焦点（视野中心）
    /// </summary>
    private Vector3 cameraFocasCenter = Vector3.zero;
    private CameraData followDefaultData;
    private CameraData followData;
    private Vector3 followFocasCenter = Vector3.zero;

    private Vector3 v3pos = new Vector3(0, 6.7f, -3.5f);
    private Vector3 v3rotat = new Vector3(53, 0, 0);

    /// <summary>
    /// 初始高度 计算得出
    /// </summary>
    private float height = 0;
    /// <summary>
    /// 初始角度
    /// </summary>
    private float width = 0;
    /// <summary>
    /// 初始垂直视角
    /// </summary>
    private float angle = 0;

    /// <summary>
    /// 旋转时记录输入的旧位置
    /// </summary>
    private Vector3 oldPos1 = Vector3.zero;

    private Vector3 screenCenter = new Vector2(Screen.width/2, Screen.height/2);

    private int currentCount;
    private float oldDis;


    private List<Vector2> offsetList = new List<Vector2>();
    private const int offsetCount = 5;

    public bool follow = false;
    public Vector3 focaseP
    {
        get
        {
            return follow ? followFocasCenter : cameraFocasCenter;
        }
        set {
            if (follow){
                followFocasCenter = value;
            } else {
                cameraFocasCenter = value;
            }
        }
    }

    private CameraData curData
    {
        get
        {
            return follow ? followData : data;
        }
    }
    private CameraData curDefaultData
    {
        get
        {
            return follow ? followDefaultData : defaultData;
        }
    }

    public override void enter()
    {
        base.enter();
        curData.setCamera(target);
        target.transform.LookAt(focaseP);
        if (width == 0) width = settings.defaultViewWidth;
        if (angle == 0) angle = settings.defaultFreeViewAngle;

        offsetList.Clear();
    }

    public override void mUpdate()
    {
        base.mUpdate();
        Vector3 v3 = Input.mousePosition;
        if (uguiHitUI.uiHited == true)
        {
            oldPos1 = v3;
            return;
        }
        float delta = Input.GetAxis("Mouse ScrollWheel");
        if (delta != 0)
        {
            Vector3 pos = target.transform.position;
            float dis = Vector3.Distance(pos, cameraFocasCenter);
            float f = (dis - settings.minFreeViewDis) / (settings.maxFreeViewDis - settings.minFreeViewDis);
            float speed = 0.005f;
            if (f * scaleMultiEditor > speed)
            {
                speed = f * scaleMultiEditor;
            }
            //Debug.LogWarning("speed = " + speed);
            f -= delta * speed;
            dis = Mathf.Lerp(settings.minFreeViewDis, settings.maxFreeViewDis, f);
            pos = cameraFocasCenter + dis * (pos - cameraFocasCenter).normalized;
            SetMainCameraPos(pos);
            machine.dispatchEvent(new CameraEvent(CameraEvent.ScaleChange));
            machine.dispatchEvent(new CameraEvent(CameraEvent.ViewChange));
        }

        bool changed = false;
        bool isRightPointRotate = inputMachine.CurrentState is SetMaterialState || inputMachine.CurrentState is PlaceGoodsState3D;// || inputMachine.CurrentState is SelectState3D;

        SelectGoodsState3D state = inputMachine.CurrentState as SelectGoodsState3D;
        if (state != null)
        {
            isRightPointRotate = state.canMove || state.onhandle;
        }

        if (Input.GetMouseButtonDown(0) || (isRightPointRotate == true && Input.GetMouseButtonDown(1)))
        {
            oldPos1 = v3;
            offsetList.Clear();
            MyMono.MyStopCoroutine(MoveRotate, this);
        }
        if (
            (isRightPointRotate == false && (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))) 
            || 
            (isRightPointRotate == true && (Input.GetMouseButton(1) || Input.GetMouseButtonUp(1)))
           )
        {
            if (oldPos1 == v3)
            {
                return;
            }
            Vector2 offset = v3 - oldPos1;
            offsetList.Add(offset);
            if (offsetList.Count > offsetCount)
            {
                offsetList.RemoveAt(0);
            }
            oldPos1 = v3;
            Rotate(offset);
            if (
                (isRightPointRotate == false && Input.GetMouseButtonUp(0))
                ||
                (isRightPointRotate == true && Input.GetMouseButtonUp(1))
               )
            {
                MyMono.MyStartCoroutine(MoveRotate, this);
            }
            //Vector3 pos = target.transform.position;
            //float dis = Vector3.Distance(pos, focaseP);
            //Vector3 euler = target.transform.rotation.eulerAngles;
            //euler.x -= offset.y/2 * rotateMultiYEditor;
            //euler.y += offset.x/2 * rotateMultiXEditor;
            //euler.y = euler.y > 360 ? euler.y - 360 : euler.y;
            //euler.y = euler.y < -360 ? euler.y + 360 : euler.y;
            //if (euler.x < 180)
            //{
            //    euler.x = euler.x > settings.maxFreeViewAngle ? settings.maxFreeViewAngle : euler.x;
            //}
            //else
            //{
            //    euler.x -= 360;
            //}
            //euler.x = euler.x < settings.minFreeViewAngle ? settings.minFreeViewAngle : euler.x;
            //target.transform.rotation = Quaternion.Euler(euler);
            //Vector3 rotatePos = GetRayPoint(Screen.width / 2 * Vector2.right + Screen.height / 2 * Vector2.up, dis - target.nearClipPlane);
            //target.transform.position += focaseP - rotatePos;
            //MyCallLater.Add(0.5f, RefreshView);
            changed = true;
        }

        if (Input.GetMouseButtonDown(2))
        {
            ///记录移动时输入的旧位置
            oldPos = v3;
            changed = true;
            MyMono.MyStopCoroutine(MoveRotate, this);
        }
        if (Input.GetMouseButton(2))
        {
            if (oldPos == v3)
            {
                return;
            }
            Vector3 pos = target.transform.position;
            float dis = Vector3.Distance(pos, focaseP);
            Vector3 offset = GetRayPoint(v3, dis) - GetRayPoint(oldPos, dis);
            Vector3 moved = -offset;
            oldPos = v3;
            Vector3 modedPos = pos + moved;
            modedPos.y = modedPos.y < settings.minFreeViewDis ? settings.minFreeViewDis : modedPos.y;
            modedPos.y = modedPos.y > settings.maxFreeViewDis ? settings.maxFreeViewDis : modedPos.y;
            SetMainCameraPos(modedPos, false);
            moved = modedPos - pos;
            focaseP += moved;
            changed = true;
            machine.dispatchEvent(new CameraEvent(CameraEvent.PositionChange));
            machine.dispatchEvent(new CameraEvent(CameraEvent.ViewChange));
        }


        //if (changed && state != null)
        //{
        //    state.LookAt();
        //}
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
        if (uguiHitUI.uiHited == true)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                Vector3 v3 = touch.position;
                oldPos1 = v3;
            }
            return;
        }
        bool changed = false;
        bool isUnnormalRotate = inputMachine.CurrentState is SetMaterialState || inputMachine.CurrentState is PlaceGoodsState3D;// || inputMachine.CurrentState is SelectState3D;
        SelectGoodsState3D state = inputMachine.CurrentState as SelectGoodsState3D;
        if (state != null)
        {
            isUnnormalRotate = state.canMove || state.onhandle;
        }

        if (currentCount != Input.touchCount)
        {
            currentCount = Input.touchCount;
            if (currentCount == 1)
            {
                Touch touch0 = Input.GetTouch(0);
                oldPos1 = touch0.position;
            }
            else if (currentCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);
                oldDis = (touch0.position - touch1.position).magnitude;
                oldPos = (touch0.position + touch1.position) / 2f;
            }
            changed = true;
        }


        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 v3 = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                oldPos1 = v3;
                offsetList.Clear();
                MyMono.MyStopCoroutine(MoveRotate, this);
            }
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended) && isUnnormalRotate == false)
            {
                if (oldPos1.x == v3.x && oldPos1.y == v3.y)
                {
                    return;
                }
                Vector2 offset = v3 - oldPos1;
                offsetList.Add(offset);
                if (offsetList.Count > offsetCount)
                {
                    offsetList.RemoveAt(0);
                }
                oldPos1 = v3;
                Rotate(offset);
                changed = true;
                if (touch.phase == TouchPhase.Ended)
                {
                    MyMono.MyStartCoroutine(MoveRotate, this);
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                oldDis = (touch0.position - touch1.position).magnitude;
                ///移动时的旧位置
                oldPos = (touch0.position + touch1.position) / 2f;
                changed = true;
                MyMono.MyStopCoroutine(MoveRotate, this);
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float dis = (touch0.position - touch1.position).magnitude;
                float delta = (dis - oldDis) / 100f;
                if (delta != 0)
                {
                    Vector3 pos = target.transform.position;
                    float curdis = Vector3.Distance(pos, cameraFocasCenter);
                    float f = (curdis - settings.minFreeViewDis) / (settings.maxFreeViewDis - settings.minFreeViewDis);
                    float speed = 0.005f;
                    if (f * scaleMulti > speed)
                    {
                        speed = f * scaleMulti;
                    }
                    f -= delta * speed;
                    curdis = Mathf.Lerp(settings.minFreeViewDis, settings.maxFreeViewDis, f);
                    pos = cameraFocasCenter + curdis * (pos - cameraFocasCenter).normalized;
                    SetMainCameraPos(pos);
                    changed = true;
                    machine.dispatchEvent(new CameraEvent(CameraEvent.ScaleChange));
                    machine.dispatchEvent(new CameraEvent(CameraEvent.ViewChange));
                }
                oldDis = dis;

                Vector2 curpos = (touch0.position + touch1.position) / 2f;
                Vector2 deltaPos = curpos - (Vector2)oldPos;
                if (deltaPos != Vector2.zero)
                {
                    Vector3 pos = target.transform.position;
                    float focaseDis = Vector3.Distance(pos, focaseP);
                    Vector3 offset = GetRayPoint(curpos, focaseDis) - GetRayPoint(oldPos, focaseDis);
                    Vector3 moved = -offset;
                    oldPos = curpos;
                    Vector3 modedPos = pos + moved;
                    modedPos.y = modedPos.y < settings.minFreeViewDis ? settings.minFreeViewDis : modedPos.y;
                    modedPos.y = modedPos.y > settings.maxFreeViewDis ? settings.maxFreeViewDis : modedPos.y;
                    SetMainCameraPos(modedPos, false);
                    moved = modedPos - pos;
                    focaseP += moved;
                    changed = true;
                    machine.dispatchEvent(new CameraEvent(CameraEvent.PositionChange));
                    machine.dispatchEvent(new CameraEvent(CameraEvent.ViewChange));
                }
            }
        }

        //if (changed && state != null)
        //{
        //    state.LookAt();
        //}

        //Vector3 v3 = Input.mousePosition;
        
        //if (Input.GetMouseButtonDown(2))
        //{
        //    oldPos = v3;
        //}
        //if (Input.GetMouseButton(2))
        //{
        //    if (oldPos == v3)
        //    {
        //        return;
        //    }
        //    Vector3 pos = target.transform.position;
        //    float dis = Vector3.Distance(pos, focaseP);
        //    Vector3 offset = GetRayPoint(v3, dis) - GetRayPoint(oldPos, dis);
        //    Vector3 moved = -offset;
        //    oldPos = v3;
        //    Vector3 modedPos = pos + moved;
        //    modedPos.y = modedPos.y < settings.minFreeViewDis ? settings.minFreeViewDis : modedPos.y;
        //    modedPos.y = modedPos.y > settings.maxFreeViewDis ? settings.maxFreeViewDis : modedPos.y;
        //    SetMainCameraPos(modedPos, false);
        //    moved = modedPos - pos;
        //    focaseP += moved;
        //}
    }

    private IEnumerator MoveRotate(params object[] objs)
    {
        Vector2 endDis = Vector2.zero;
        for (int i = offsetList.Count - 1; i >= 0; i--)
        {
            if (offsetList[i] == Vector2.zero)
            {
                endDis = Vector2.zero;
                break;
            }
            if (i == offsetList.Count - 1)
            {
                endDis = offsetList[i];
            }
            else
            {
                if (Vector2.Dot(endDis, offsetList[i]) < 0)
                {
                    endDis = offsetList[i];
                }
            }
        }
        endDis = endDis / 2;
        if (endDis.sqrMagnitude > 30)
        {
            //Debug.LogWarning("totalDis = " + totalDis);
            Vector2 offset = endDis;
            float totalDetaTime = Time.deltaTime;
            offset = Vector2.Lerp(endDis, Vector2.zero, totalDetaTime);
            do
            {
                bool onside = Rotate(offset);
                if (onside)
                {
                    break;
                }
                //Debug.LogWarning("Rotate : " + offset);
                yield return new WaitForEndOfFrame();
                totalDetaTime += Time.deltaTime;
                offset = Vector2.Lerp(endDis, Vector2.zero, totalDetaTime * 2);
                if (GlobalConfig.running == false)
                {
                    break;
                }
            }
            while (offset != Vector2.zero);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>是否到达角度边界</returns>
    private bool Rotate(Vector2 offset)
    {
        Vector3 pos = target.transform.position;
        float dis = Vector3.Distance(pos, focaseP);
        Vector3 euler = target.transform.rotation.eulerAngles;
        euler.x -= offset.y / 2 * rotateMultiY;
        euler.y += offset.x / 2 * rotateMultiX;
        euler.y = euler.y > 360 ? euler.y - 360 : euler.y;
        euler.y = euler.y < -360 ? euler.y + 360 : euler.y;
        euler.x = euler.x > settings.maxFreeViewAngle ? settings.maxFreeViewAngle : euler.x;
        euler.x = euler.x < settings.minFreeViewAngle ? settings.minFreeViewAngle : euler.x;
        target.transform.rotation = Quaternion.Euler(euler);
        Vector3 rotatePos = GetRayPoint(Screen.width / 2 * Vector2.right + Screen.height / 2 * Vector2.up, dis - target.nearClipPlane);
        target.transform.position += focaseP - rotatePos;

        machine.dispatchEvent(new CameraEvent(CameraEvent.RotateChange));
        machine.dispatchEvent(new CameraEvent(CameraEvent.ViewChange));

        //MyCallLater.Add(0.5f, RefreshView);
        if (euler.x == settings.minFreeViewAngle || euler.x == settings.maxFreeViewAngle)
        {
            return true;
        }
        return false;
    }

    private Vector3 ScreenToWorldPoint(Vector3 mousePos)
    {
        Vector3 pos = target.transform.position;
        Vector3 aim = focaseP;
        float dis = Vector3.Distance(pos, aim);
        pos = GetRayPoint(mousePos, dis);
        return pos;
    }


    private Vector3 GetRayPoint(Vector3 v3, float dis)
    {
        return target.ScreenPointToRay(v3).GetPoint(dis);
    }

    private void SetMainCameraPos(Vector3 pos, bool focas = true)
    {
        target.transform.position = pos;
        if (focas)
        {
            target.transform.LookAt(focaseP);
        }
    }

    public override void ResetCameraData(MyEvent obj)
    {
        curData.cullingMask = curDefaultData.cullingMask;
        curData.orthographic = curDefaultData.orthographic;
        curData.orthographicSize = curDefaultData.orthographicSize;
        curData.cullingMask = curDefaultData.cullingMask;
        curData.position = curDefaultData.position;
        curData.rotation = curDefaultData.rotation;
    }

    private void RefreshView()
    {
        if (GlobalConfig.running == false)
        {
            return;
        }
        view3D.RefreshView();
    }

    public override void ResetCamera(MyEvent obj)
    {
        ResetCameraData(null);
        enter();
    }

    public override void ResetCameraData(Vector2 pos)
    {
        Vector3 newPos = Vector3.right * pos.x + Vector3.forward * pos.y;
        curDefaultData.position = data.position = v3pos + newPos;
        curDefaultData.rotation = data.rotation = Quaternion.Euler(v3rotat);
        focaseP = newPos;
        base.ResetCameraData(pos);
    }

    public override void exit()
    {
        MyMono.MyStopCoroutine(MoveRotate, this);
        curData.ReadCamera(target);
        base.exit();
    }
}
