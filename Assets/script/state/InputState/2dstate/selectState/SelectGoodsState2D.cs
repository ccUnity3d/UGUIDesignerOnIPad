using UnityEngine;
using System.Collections;
using System;

public class SelectGoodsState2D : SelectState {

    public const string NAME = "SelectGoodsState";
    public SelectGoodsState2D():base(NAME)
    {
        selectMachine = SelectGoods2D_StateMachine.Instance;
        goods2Dmachine.selectGoodsState2D = this;
    }
    public ProductData _target;
    public ProductData target
    {
        get {
            return _target;
        }
        set {
            _target = value;
            onTargetChange(_target);
        }
    }
    public GoodsVO targetVO;
    public Product targetProduct;
    public GameObject targetObj;
    public GameObject rotateObj;
    private SelectGoods2D_StateMachine goods2Dmachine
    {
        get
        {
            return selectMachine as SelectGoods2D_StateMachine;
        }
    }

    private WallData targetWall;
    private Vector2 lastMousePos = Vector2.zero;
    
    private WallHelpFunc wallFunc = WallHelpFunc.Instance;
    private Vector2 targetSize;
    private Vector3 targetOffset;
    private bool onSetDis = false;
    private bool onPagegroup = false;
    private bool downed;

    public override void enter()
    {
        base.enter();
        target = (viewTarget as GoodsView).data;
        //machine.selectGoods
        targetObj = viewTarget.gameObject;
        targetVO = mainpagedata.getGoods(target.seekId);
        targetProduct = mainpagedata.getProduct(target.seekId);
        Vector3 size = targetProduct.size;
        targetSize = Vector2.right * size.x + Vector2.up * size.z;
        targetOffset = size.x / viewTarget.transform.localScale.x * Vector3.right
            + size.z / viewTarget.transform.localScale.y * Vector3.up;
        OptionsController.Instance.selectMachine = goods2Dmachine;
        //MainPageUIController.Instance;
        if(rotateObj == null) rotateObj = prefabs.GetNewInstance_rotate();
        rotateObj.SetActive(true);
        rotateObj.transform.position = viewTarget.transform.TransformPoint(targetOffset / 2 - Vector3.forward);
        goods2Dmachine.setState(EditTypeOnSelect.Free);
        downed = false;
    }
   
    protected override GoodsType getGoodsType()
    {
        return GoodsType.Product;
    }

    public override void mUpdate()
    {
        if (goods2Dmachine.CurrentState.needCloseSelectGoodsState2DUpdate)
        {
            return;
        }

        base.mUpdate();

        if (changed) return;
        if (uguiHitUI.uiHited == true) return;
        if (canMove == false)
        {
            if (Input.GetMouseButtonUp(0) == false)
            {
                return;
            }
            if (downed == false) return;
        }

        Vector2 pos = GetScreenToWorldPos(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            saved = false;
            lastMousePos = pos;
            downed = true;
        }

        if (Input.GetMouseButton(0) == false && Input.GetMouseButtonUp(0) == false)
        {
            return;
        }
        if (lastMousePos == pos)
        {
            return;
        }

        if (saved == false) saved = true;

        targetWall = null;
        float Angle = targetObj.transform.rotation.eulerAngles.z;
        Vector3 world2d = pos;
        float dis = float.MaxValue;
        for (int i = 0; i < data.wallList.Count; i++)
        {
            if (targetVO.type != 2)
            {
                float disi = GoodsWallDis(data.wallList[i], world2d, Angle);
                if (disi < dis)
                {
                    dis = disi;
                    targetWall = data.wallList[i];
                }
            }
            else
            {
                GoodsInWall(data.wallList[i], ref world2d, ref Angle);
            }
        }

        if (targetWall != null)
        {
            if (targetVO.type != 2)
            {
                GoodsOnWall(targetWall, ref world2d, ref Angle);
            }
            //else
            //{
            //    GoodsInWall(targetWall, ref world2d, ref Angle);
            //}
        }

        world2d.z = -5;
        targetObj.transform.position = world2d;
        rotateObj.transform.position = viewTarget.transform.TransformPoint(targetOffset / 2 - Vector3.forward);
        //if (Input.GetMouseButtonUp(0))
        //{

        //}

        Vector3 world3d = world2d.x * Vector3.right + world2d.y * Vector3.forward;
        target.position = world3d;
        target.rotate = Angle;
        target.targetWall = targetWall;
        RefreshView();
    }

    public override void mPhoneUpdate()
    {
        if (goods2Dmachine.CurrentState.needCloseSelectGoodsState2DUpdate)
        {
            return;
        }
        
        base.mPhoneUpdate();

        if (changed) { return; }

        if (uguiHitUI.uiHited == true || Input.touchCount != 1)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);
        bool inputUp = touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended;
        if (canMove == false)
        {
            if (inputUp == false) return;
            if (downed == false) return;
        }

        if (touch.phase == TouchPhase.Began)
        {
            saved = false;
            downed = true;
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

            targetWall = null;
            float Angle = targetObj.transform.rotation.eulerAngles.z;
            Vector3 world2d = inputCamera.ScreenToWorldPoint(touch.position);
            float dis = float.MaxValue;
            for (int i = 0; i < data.wallList.Count; i++)
            {
                if (targetVO.type != 2)
                {
                    float disi = GoodsWallDis(data.wallList[i], world2d, Angle);
                    if (disi < dis)
                    {
                        dis = disi;
                        targetWall = data.wallList[i];
                    }
                }
                else
                {
                    GoodsInWall(data.wallList[i], ref world2d, ref Angle);
                }
            }

            if (targetWall != null)
            {
                if (targetVO.type != 2)
                {
                    GoodsOnWall(targetWall, ref world2d, ref Angle);
                }
            }

            world2d.z = -5;
            targetObj.transform.position = world2d;
            rotateObj.transform.position = viewTarget.transform.TransformPoint(targetOffset / 2 - Vector3.forward);
            //if (inputUp == true)
            //{

            //}
            Vector3 world3d = world2d.x * Vector3.right + world2d.y * Vector3.forward;
            target.position = world3d;
            target.rotate = Angle;
            target.targetWall = targetWall;
            RefreshView();
        }
    }

    private float GoodsWallDis(WallData wall,Vector2 world, float Angle)
    {
        float angle = wall.GetWallAngle();
        float deltaAngle = angle - Angle;
        float xifudis = targetSize.y / 2;
        int times45 = (int)(deltaAngle / 45);
        if (times45 % 2 != 0)
        {
            times45 += 1;
        }
        deltaAngle = times45 * 45;
        int times90 = times45 / 2;
        if (times90 % 2 != 0)
        {
            xifudis = targetSize.x / 2;
        }
        float dis = xifudis + wall.width / 2;
        float walldis = wall.GetDis(world);
        if (walldis < dis)
        {
            Vector2 pos = wall.GetDisPoint(world);
            if (wallFunc.PointOnWall(pos, wall) 
                //|| (wall.point1.pos - pos).magnitude < xifudis
                //|| (wall.point2.pos - pos).magnitude < xifudis
                )
            {
                return Mathf.Abs(walldis - dis);
                //world = wall.GetDisPoint(world, dis);
                //if (targetWall == null)
                //{
                //    Vector3 eulerAngles = targetObj.transform.rotation.eulerAngles;
                //    Angle = angle - deltaAngle;
                //    eulerAngles.z = Angle;
                //    targetObj.transform.rotation = Quaternion.Euler(eulerAngles);
                //    targetWall = wall;
                //}
            }
        }
        return float.MaxValue;
    }

    private void GoodsOnWall(WallData wall, ref Vector3 world, ref float Angle)
    {
        float angle = wall.GetWallAngle();
        float deltaAngle = angle - Angle;
        float xifudis = targetSize.y / 2;
        int times45 = (int)(deltaAngle / 45);
        if (times45 % 2 != 0)
        {
            times45 += 1;
        }
        deltaAngle = times45 * 45;
        int times90 = times45 / 2;
        if (times90 % 2 != 0)
        {
            xifudis = targetSize.x / 2;
        }
        float dis = xifudis + wall.width / 2;
        if (wall.GetDis(world) < dis)
        {
            Vector2 pos = wall.GetDisPoint(world);
            if (wallFunc.PointOnWall(pos, wall) || (wall.point1.pos - pos).magnitude < xifudis
                || (wall.point2.pos - pos).magnitude < xifudis)
            {
                world = wall.GetDisPoint(world, dis);
                //if (targetWall == null)
                //{
                    Vector3 eulerAngles = targetObj.transform.rotation.eulerAngles;
                    Angle = angle - deltaAngle;
                    eulerAngles.z = Angle;
                    targetObj.transform.rotation = Quaternion.Euler(eulerAngles);
                //    targetWall = wall;
                //}
            }
        }
    }

    private void GoodsInWall(WallData wall, ref Vector3 world, ref float Angle)
    {
        float angle = wall.GetWallAngle();
        float deltaAngle = angle - Angle;
        float xifudis = targetSize.y / 2;
        int times90 = (int)(deltaAngle / 90);
        if (times90 % 2 != 0)
        {
            times90 += 1;
        }
        deltaAngle = times90 * 90;
        float dis = xifudis + wall.width / 2;
        if (wall.GetDis(world) < dis)
        {
            Vector2 pos = wall.GetDisPoint(world);
            if (wallFunc.PointOnWall(pos, wall) || (wall.point1.pos - pos).magnitude < xifudis
                || (wall.point2.pos - pos).magnitude < xifudis)
            {
                world = pos;
                if (targetWall == null)
                {
                    Vector3 eulerAngles = targetObj.transform.rotation.eulerAngles;
                    Angle = angle - deltaAngle;
                    eulerAngles.z = Angle;
                    targetObj.transform.rotation = Quaternion.Euler(eulerAngles);
                    targetSize.y = wall.width;
                    targetObj.transform.localScale = targetSize;
                    targetWall = wall;
                }
            }
        }
    }

    public override void exit()
    {
        rotateObj.SetActive(false);
        goods2Dmachine.setState(EditTypeOnSelect.Free);
        base.exit();
    }

}
