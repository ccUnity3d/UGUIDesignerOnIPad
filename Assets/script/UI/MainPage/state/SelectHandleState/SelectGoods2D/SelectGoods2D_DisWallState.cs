using UnityEngine;
using System.Collections.Generic;

public class SelectGoods2D_DisWallState : SelectGoods2D_State
{
    private int oldTouchCount;
    private Collider2D colli;
    private WallView _disView = null;
    private WallView disView
    {
        get { return _disView; }
        set {
            _disView = value;
            //Debug.LogWarning("_disView = " + _disView);
        }
    }
    private Vector2 nearestP;

    private ToggleButton toggleButton;
    private OptionsPage optionsPage { get { return OptionsPage.Instance; } }
    private GameObject obj;


    private List<Vector2> vertexs = new List<Vector2>();
    private float minDis;
    private Vector2 minPos;
    private float extentsLength;
    private float firstAngle;

    protected LineHelpFunc lineFunc
    {
        get {
            return LineHelpFunc.Instance;
        }
    }
   
    public override bool needCloseSelectGoodsState2DUpdate
    {
        get
        {
            return true;
        }
    }

    protected UGUIHitUI uguiHitUI
    {
        get {
            return UGUIHitUI.Instance;
        }
    }

//    protected bool uiHit
//    {
//        get
//        {
//            if (GlobalConfig.isMyDebug == true)
//            {
//#if !UNITY_ANDROID && !UNITY_IPHONE  //安卓  
//                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
//#else
//                if (Input.touchCount == 0)
//                {
//                    return false;
//                }

//                UnityEngine.EventSystems.PointerEventData pointdata;
//                if (MyStandaloneInputModule.current.tryGetMyMousePointerEventData(out pointdata) == false)
//                {
//                    return false;
//                }
//                List<UnityEngine.EventSystems.RaycastResult> list = new List<UnityEngine.EventSystems.RaycastResult>();
//                UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointdata, list);
//                if (list.Count == 0)
//                {
//                    return false;
//                }
//                return true;
//#endif
//            }
//            else {
//                if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
//                {
//                    return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
//                }

//                if (Input.touchCount == 0)
//                {
//                    return false;
//                }

//                UnityEngine.EventSystems.PointerEventData pointdata;
//                if (MyStandaloneInputModule.current.tryGetMyMousePointerEventData(out pointdata) == false)
//                {
//                    return false;
//                }
//                List<UnityEngine.EventSystems.RaycastResult> list = new List<UnityEngine.EventSystems.RaycastResult>();
//                UnityEngine.EventSystems.EventSystem.current.RaycastAll(pointdata, list);
//                if (list.Count == 0)
//                {
//                    return false;
//                }
//                return true;
//            }
//        }
//    }

    public override void Ready(GameObject skin)
    {
        base.Ready(skin);
        obj = optionsPage.distanceWall.gameObject;
        toggleButton = optionsPage.disWall;
    }

    public override void enter()
    {
        base.enter();

        if (toggleButton.onDown == false)
        {
            setState(EditTypeOnSelect.Free);
            return;
        }
        //Debug.Log(obj.name);

        colli = null;
        disView = null;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            MyTickManager.Add(phoneUpdate);
        }
        else {
            MyTickManager.Add(update);
        }
        awake();
        
        //选择一个墙 更改距离
    }

    public override void exit()
    {
        toggleButton.onDown = false;
        UITool.SetActionFalse(obj);
        mainPageContr.CloseInput();
        view2D.selectGoodsdisedWalldata = null;
        if (machine.nextIsCurrent == false)
        {
            RefreshView();
        }
        optionsPage.inputUnit.onClick.RemoveListener(onOpenInput);
        optionsPage.ok.onClick.RemoveListener(Ok);
        optionsPage.cancel.onClick.RemoveListener(Cancel);
        MyTickManager.Remove(update);
        base.exit();
    }

    private void awake()
    {
        Vector3 scale = Vector3.Scale(targetProduct.size, target.scale) / 2;
        Vector2 direct = Vector2.right * Mathf.Abs(scale.x) + Vector2.up * Mathf.Abs(scale.z);
        extentsLength = direct.magnitude;
        firstAngle = lineFunc.GetRadian(direct);
        CaculatVertexsPos();
    }

    private void CaculatVertexsPos()
    {
        float angle1 = firstAngle + target.rotate;
        float angle2 = Mathf.PI - firstAngle + target.rotate;
        float angle3 = Mathf.PI + firstAngle + target.rotate;
        float angle4 = 2 * Mathf.PI - firstAngle + target.rotate;
        
        Vector2 pos = GetV2WithoutY(target.position);
        Vector2 p1 = pos + Vector2.right * extentsLength * Mathf.Cos(angle1) + Vector2.up * extentsLength * Mathf.Sin(angle1);
        Vector2 p2 = pos + Vector2.right * extentsLength * Mathf.Cos(angle2) + Vector2.up * extentsLength * Mathf.Sin(angle2);
        Vector2 p3 = pos + Vector2.right * extentsLength * Mathf.Cos(angle3) + Vector2.up * extentsLength * Mathf.Sin(angle3);
        Vector2 p4 = pos + Vector2.right * extentsLength * Mathf.Cos(angle4) + Vector2.up * extentsLength * Mathf.Sin(angle4);

        vertexs.Clear();
        vertexs.Add(p1);
        vertexs.Add(p2);
        vertexs.Add(p3);
        vertexs.Add(p4);
    }

    private void update()
    {
        if (uguiHitUI.uiHited == true)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) == true)
        { 
            //disView = null;
            Vector2 v2 = inputCamera.ScreenToWorldPoint(Input.mousePosition);
            colli = Physics2D.OverlapPoint(v2);
        }
        else if (Input.GetMouseButtonUp(0) == true)
        {
            if (colli == null)
            {
                return;
            }
            Vector2 v2 = inputCamera.ScreenToWorldPoint(Input.mousePosition);
            Collider2D colli2 = Physics2D.OverlapPoint(v2);
            if (colli2 == colli)
            {
                WallView view = colli.GetComponent<WallView>();
                if (view != null)
                {
                    disView = view;
                    view2D.selectGoodsdisedWalldata = view.data;
                    OpenDisInput();
                    RefreshView();
                }
            }
        }
    }

    private void phoneUpdate()
    {
        if (uguiHitUI.uiHited == true)
        {
            return;
        }

        if (Input.touchCount != 1)
        {
            oldTouchCount = Input.touchCount;
            return;
        }
        Touch touch = Input.GetTouch(0);
        if (oldTouchCount > 1 || touch.phase == TouchPhase.Began)
        {
            //disView = null;
            Vector2 v2 = inputCamera.ScreenToWorldPoint(touch.position);
            colli = Physics2D.OverlapPoint(v2);
        }
        else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            if (colli == null)
            {
                return;
            }
            Vector2 v2 = inputCamera.ScreenToWorldPoint(touch.position);
            Collider2D colli2 = Physics2D.OverlapPoint(v2);
            if (colli2 == colli)
            {
                WallView view = colli.GetComponent<WallView>();
                if (view != null)
                {
                    disView = view;
                    view2D.selectGoodsdisedWalldata = view.data;
                    OpenDisInput();
                    RefreshView();
                }
            }
        }
        oldTouchCount = Input.touchCount;
    }

    private void OpenDisInput()
    {
        minDis = float.MaxValue;
        Vector2 pos = GetV2WithoutY(target.position);
        minPos = pos;
        WallData wall = disView.data;
        int sidetype = lineFunc.Clockwise(pos, wall.point1.pos, wall.point2.pos);
        if (sidetype == 0)
        {
            sidetype = 1;
        }
        for (int i = 0; i < vertexs.Count; i++)
        {
            Vector2 vertexspos = vertexs[i];
            float dis = wall.GetDis(vertexspos);
            int sidetypei = lineFunc.Clockwise(vertexspos, wall.point1.pos, wall.point2.pos);

            if (sidetypei == 0)
            {
                dis = 0;
            }
            else if (sidetypei != sidetype)
            {
                dis = -dis;
            }
            dis -= wall.width / 2;
            if (dis < minDis)
            {
                minDis = dis;
                minPos = vertexspos;
            }
            else if (dis == minDis)
            {
                if (vertexspos.x < minPos.x || vertexspos.y < minPos.y) minPos = vertexspos;
            }
        }

        UITool.SetActionTrue(obj);
        Cancel();
        optionsPage.inputUnit.onClick.AddListener(onOpenInput);
        optionsPage.ok.onClick.AddListener(Ok);
        optionsPage.cancel.onClick.AddListener(Cancel);
    }

    private void Cancel()
    {
        float value = Mathf.Round(minDis * 1000f);
        value = value * (int)defaultSet.DefaultUnit / 1000f;
        optionsPage.inputValue.text = (value).ToString();
    }

    private void Ok()
    {
        //移动到新位置
        float toMinDis = float.Parse(optionsPage.inputValue.text);
        toMinDis = Mathf.Round(toMinDis * 1000f / (int)defaultSet.DefaultUnit);
        float newmindis = toMinDis / 1000f;
        //float newmindis = toMinDis / (int)defaultSet.DefaultUnit;
        float needdis = newmindis + disView.data.width / 2;
        Vector2 newminPos = disView.data.GetDisPoint(minPos, needdis);
        if (minDis < 0)//＜0取对称值
        {
            Vector2 Pos0 = disView.data.GetDisPoint(minPos);
            newminPos = 2 * Pos0 - newminPos;
        }
        Vector2 offset = newminPos - minPos;

        target.position += GetV3WithoutY(offset);
        RefreshView();
        //重新计算最近点
        //awake();
        //OpenDisInput();

        CaculatVertexsPos();
        minDis = newmindis;
        minPos = newminPos;
        float minDisFloat = minDis * (int)defaultSet.DefaultUnit;
        mainPageContr.OpenInput(minDisFloat, optionsPage.inputValue, InputDis);
    }

    private void onOpenInput()
    {
        //optionsPage.inputValue.text = null;
        mainPageContr.OpenInput(0, optionsPage.inputValue, InputDis);
    }

    private void InputDis()
    {
        //对输入值控制
        //optionsPage.inputValue.text;
    }

    private Vector3 GetV3WithoutY(Vector2 v2)
    {
        return Vector3.right* v2.x + Vector3.forward * v2.y;
    }

    private Vector2 GetV2WithoutY(Vector3 v3)
    {
        return Vector2.right * v3.x + Vector2.up * v3.z;
    }

}
