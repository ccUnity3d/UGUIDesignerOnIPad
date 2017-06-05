using UnityEngine;
using System.Collections;
using System;

public class SelectGoodsState3D : SelectState3D {

    public const string NAME = "SelectGoodsState3D";
    public SelectGoodsState3D() : base(NAME)
    {
        selectMachine = SelectGoods3D_StateMachine.Instance;
        selectGoods3DMachine.selectGoods3DState = this;
    }

    private SelectGoods3D_StateMachine selectGoods3DMachine
    {
        get {
            return (selectMachine as SelectGoods3D_StateMachine);
        }
    }

    private ProductData _target;
    public ProductData target
    {
        get { return _target;}
        set { _target = value;onTargetChange(_target); }
    }
    private Vector2 lastMousePos = Vector2.zero;

    private GoodsVO targetVO;
    public Product targetProduct;

    private WallHelpFunc wallFunc = WallHelpFunc.Instance;
    private Vector3 targetSize;

    private GameObject targetObj = null;
    private WallData targetWall = null;
    private GameObject selectEmtyObj;
    private RaycastHit[] hits = new RaycastHit[20];
    private SimpleOutterLoader loader;


    private GameObject rotateObj;
    private GameObject disFloorObj;
    private float forTop = 180;
    private readonly int[] EmtyArr = new int[] { };

    protected override GoodsType getGoodsType()
    {
        return GoodsType.Product;
    }

    public override void enter()
    {
        base.enter();
        if (viewTarget == null)
        {
            target = viewData as ProductData;
            targetWall = target.targetWall;
            targetProduct = mainpagedata.getProduct(target.seekId);
            targetSize = targetProduct.size;
            targetVO = mainpagedata.getGoods(target.seekId);
            optionsController.selectMachine = selectGoods3DMachine;

            GameObject go;
            if (selectEmtyObj == null)
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                selectEmtyObj = new GameObject("selectEmtyObj");
                go.transform.parent = selectEmtyObj.transform;
            }
            else { go = selectEmtyObj.transform.GetChild(0).gameObject; }
            selectEmtyObj.gameObject.SetActive(true);
            Vector3 size = targetProduct.size;
            go.transform.localScale = size;
            go.transform.localPosition = size.y / 2 * Vector3.up;
            targetObj = selectEmtyObj;

            Vector3 v3 = target.position;
            v3.y = target.height;
            switch (targetVO.type)
            {
                case 3://吊顶 
                    {
                        v3.y = target.height - targetSize.y;
                    }
                    break;
                case 4://地毯 
                    {
                        v3.y = -0.001f + target.height;
                    }
                    break;
                case 5://天花板
                    {
                        v3.y = target.height - 0.001f;
                    }
                    break;
                default:
                    break;
            }
            targetObj.transform.position = v3;

            string outUrl = targetVO.modelUri;
            loader = LoaderPool.WaitOutterLoad(outUrl, SimpleLoadDataType.prefabAssetBundle, onloaded, null);
            //在3D刷新时移除 3D自己刷新会去加载拥有的模型  若不加入 加载过程中被刷新会在加载完成时候出现多余项
            view3D.loaders.Add(loader);
            return;
        }
        target = (viewTarget as Goods3DView).data;
        targetWall = target.targetWall;
        targetVO = mainpagedata.getGoods(target.seekId);
        targetProduct = mainpagedata.getProduct(target.seekId);
        targetSize = targetProduct.size;
        optionsController.selectMachine = selectGoods3DMachine;
        targetObj = viewTarget.gameObject;

        RefreshRotateAndMoveHeight(true);

        selectGoods3DMachine.setState(EditTypeOnSelect.Free);

        cameraMachion.addEventListener(CameraEvent.ViewChange, OnViewChange);
    }

    private void OnViewChange(MyEvent obj)
    {
        RefreshRotateAndMoveHeight();
    }

    private void RefreshRotateAndMoveHeight(bool changeRotateScale = false)
    {
        if (rotateObj == null) rotateObj = prefabs.GetNewInstance_rotate3D();
        rotateObj.SetActive(true);
        rotateObj.transform.position = targetObj.transform.position + 0.01f * Vector3.up;

        if (disFloorObj == null)
        {
            disFloorObj = prefabs.GetNewInstance_disFloor();
        }
        MoveUpView3D view = disFloorObj.GetComponent<MoveUpView3D>();
        view.SetData(selectMachine);
        disFloorObj.SetActive(true);
        Vector3 pos = targetObj.transform.position + Vector3.up * (targetSize.y + 0.5f);
        Vector3 screenViewPos = prefabs.mainCamera.WorldToScreenPoint(pos) * 2048f / Screen.width;
        disFloorObj.transform.localPosition = Vector2.right * screenViewPos.x + Vector2.up * screenViewPos.y;

        if (changeRotateScale == true)
        {
            Vector3 scale = Vector2.one * Mathf.Sqrt(targetProduct.size.x * targetProduct.size.x + targetProduct.size.z * targetProduct.size.z);
            //大小换成mesh控制 scale不变
            //rotateObj.transform.localScale = scale;
            MeshHelpFunc.Instance.SetRadiusMesh(rotateObj, scale.magnitude / 2);
        }
    }

    public void Refresh()
    {
        RefreshRotateAndMoveHeight();
        RefreshView();
    }

    protected override void RefreshView()
    {
        base.RefreshView();
    }

    //public void LookAt()
    //{
    //    if (disFloorObj != null) disFloorObj.transform.LookAt(prefabs.mainCamera.transform);
    //}
    
    public override void exit()
    {
        if (loader != null && loader.state == SimpleLoadedState.Loading)
        {
            Debug.LogWarning("Cancel loader " + loader.httpUrl);
            loader.Cancel();
            loader = null;
        }
        if(selectEmtyObj != null) selectEmtyObj.gameObject.SetActive(false);
        if(rotateObj!=null) rotateObj.SetActive(false);
        if (disFloorObj != null) disFloorObj.SetActive(false);
        selectGoods3DMachine.setState(EditTypeOnSelect.Free);
        cameraMachion.removeEventListener(CameraEvent.ViewChange, OnViewChange);
        base.exit();
    }

    private void onloaded(object obj)
    {
        SimpleOutterLoader loader = obj as SimpleOutterLoader;
        //在3D刷新时移除 3D自己刷新会去加载拥有的模型
        view3D.loaders.Remove(loader);
        if (loader.state == SimpleLoadedState.Failed)
        {
            return;
        }

        selectEmtyObj.gameObject.SetActive(false);
        viewTarget = view3D.getGoodsView(target);
        targetObj = viewTarget.gameObject;
        targetObj.transform.parent = prefabs.parentTran_3D;
        targetObj.transform.position = selectEmtyObj.transform.position;

        RefreshRotateAndMoveHeight(true);

        selectGoods3DMachine.setState(EditTypeOnSelect.Free);
        //LoaderPool.WaitOutterLoad 是等待加载 完成后秩序做自己用的 其他的加载的地方自己会做view3D.AddProductGameObj(targetVO.id, targetObj);

        cameraMachion.addEventListener(CameraEvent.ViewChange, OnViewChange);
    }

    private void awake()
    {
        Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);
        lastMousePos = ray.origin;
    }

    public override void mUpdate()
    {
        //if (selectGoods3DMachine.CurrentState.needCloseUpdate)
        //{
        //    return;
        //}

        base.mUpdate();

        if (viewTarget == null) return;
        if (changed) return; 
        if (uguiHitUI.uiHited) return;
        if (canMove == false)
        {
            if (Input.GetMouseButtonUp(0) == false) return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            saved = false;
            awake();
            return;
        }


        if (Input.GetMouseButtonUp(0) == false)
        {
            Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);
            if (Input.GetMouseButton(0) == false)
            {
                return;
            }
            Vector2 pos = ray.origin;
            if (lastMousePos == pos)
            {
                return;
            }
            lastMousePos = pos;
            if (saved == false) saved = true;

            Vector3 world3d = Vector3.one;
            int count = Physics.RaycastNonAlloc(ray, hits);
            if (count > 1 || (count == 1 && hits[0].collider.gameObject != targetObj))
            {
                float dis = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    if (hits[i].collider.gameObject == targetObj)
                    {
                        continue;
                    }
                    float itemDis = Vector3.Distance(hits[i].point, inputCamera.transform.position);
                    if (itemDis < dis)
                    {
                        dis = itemDis;
                        world3d = hits[i].point;
                        world3d.y = 0;
                    }
                }
            }
            else {
                Vector3 v3 = inputCamera.transform.position;
                Vector3 v32 = ray.origin;
                world3d.y = 0;
                float lerp = (world3d.y - v32.y) / (v3.y - v32.y);
                world3d.x = lerp * (v3.x - v32.x) + v32.x;
                world3d.z = lerp * (v3.z - v32.z) + v32.z;
            }

            float Angle = -targetObj.transform.rotation.eulerAngles.y + forTop;

            world3d.y = target.height;

            switch (targetVO.type)
            {
                case 1://吸附墙 
                    {
                        Adsorb(ref world3d, ref Angle);
                    }
                    break;
                case 2://门窗 
                    {
                        Inlay(ref world3d, ref Angle);
                    }
                    break;
                case 3://吊顶 
                    {
                        world3d.y = defaultSetting.DefaultHeight + target.height - targetSize.y;
                    }
                    break;
                case 4://地毯 
                    {
                        world3d.y = -0.001f + target.height;
                    }
                    break;
                case 5://天花板
                    {
                        world3d.y = defaultSetting.DefaultHeight + target.height - 0.001f;
                    }
                    break;
                case 6://吸附各个平面 暂时不做
                    {
                        //world3d.y = defaultSetting.DefaultHeight - 0.01f;
                    }
                    break;
                default://不吸附
                    break;
            }

            if (world3d == Vector3.one)
            {
                Debug.LogWarning("world3d = " + world3d);
            }
            targetObj.transform.position = world3d;

            RefreshRotateAndMoveHeight();
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Vector3 world2d = world3d.x * Vector3.right + world3d.y * Vector3.forward;
            target.position = targetObj.transform.position;
            target.rotate = -targetObj.transform.rotation.eulerAngles.y + forTop;
            target.targetWall = targetWall;
            RefreshView();
            return;
        }
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();

        if (viewTarget == null) return;

        if (changed) return;
        if (uguiHitUI.uiHited) return;
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
            awake();
        }
        else
        {
            if (inputUp == false)
            {
                Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);
                Vector2 pos = ray.origin;
                if (lastMousePos == pos)
                {
                    return;
                }
                lastMousePos = pos;
                if (saved == false) saved = true;


                Vector3 world3d = Vector3.one;
                int count = Physics.RaycastNonAlloc(ray, hits);
                if (count > 1 || (count == 1 && hits[0].collider.gameObject != targetObj))
                {
                    float dis = float.MaxValue;
                    for (int i = 0; i < count; i++)
                    {
                        if (hits[i].collider.gameObject == targetObj)
                        {
                            continue;
                        }
                        float itemDis = Vector3.Distance(hits[i].point, inputCamera.transform.position);
                        if (itemDis < dis)
                        {
                            dis = itemDis;
                            world3d = hits[i].point;
                            world3d.y = 0;
                        }
                    }
                }
                else {
                    Vector3 v3 = inputCamera.transform.position;
                    Vector3 v32 = ray.origin;
                    world3d.y = 0;
                    float lerp = (world3d.y - v32.y) / (v3.y - v32.y);
                    world3d.x = lerp * (v3.x - v32.x) + v32.x;
                    world3d.z = lerp * (v3.z - v32.z) + v32.z;
                }

                float Angle = -targetObj.transform.rotation.eulerAngles.y + forTop;

                world3d.y = target.height;
                switch (targetVO.type)
                {
                    case 1://吸附墙 
                        {
                            Adsorb(ref world3d, ref Angle);
                        }
                        break;
                    case 2://门窗 
                        {
                            //if (targetProduct.entityType == "window")
                            //{
                            //    world3d.y = 1;
                            //}
                            Inlay(ref world3d, ref Angle);
                        }
                        break;
                    case 3://吊顶 
                        {
                            world3d.y = defaultSetting.DefaultHeight + target.height - targetSize.y;
                        }
                        break;
                    case 4://地毯 
                        {
                            world3d.y = -0.001f + target.height;
                        }
                        break;
                    case 5://天花板
                        {
                            world3d.y = defaultSetting.DefaultHeight + target.height - 0.001f;
                        }
                        break;
                    case 6://吸附各个平面 暂时不做
                        {
                            //world3d.y = defaultSetting.DefaultHeight - 0.01f;
                        }
                        break;
                    default://不吸附
                        break;
                }


                targetObj.transform.position = world3d;

                RefreshRotateAndMoveHeight();
            }
            else
            {
                target.position = targetObj.transform.position;
                target.rotate = -targetObj.transform.rotation.eulerAngles.y + forTop;
                target.targetWall = targetWall;
                RefreshView();
            }
        }
    }

    /// <summary>
    /// 内嵌
    /// </summary>
    private void Inlay(ref Vector3 world, ref float Angle)
    {
        targetWall = null;
        Vector2 world2d = Vector2.zero;
        world2d.x = world.x;
        world2d.y = world.z;
        for (int i = 0; i < data.wallList.Count; i++)
        {
            WallData wall = data.wallList[i];
            float angle = wall.GetWallAngle();
            float deltaAngle = angle - Angle;
            float xifudis = targetSize.z / 2;
            int times90 = (int)(deltaAngle / 90);
            if (times90 % 2 != 0)
            {
                times90 += 1;
            }
            deltaAngle = times90 * 90;
            //int times180 = times90 / 2;
            float dis = xifudis + wall.width / 2;
            if (wall.GetDis(world2d) < dis)
            {
                Vector2 pos = wall.GetDisPoint(world2d);
                if (wallFunc.PointOnWall(pos, wall) || (wall.point1.pos - pos).magnitude < xifudis
                    || (wall.point2.pos - pos).magnitude < xifudis)
                {
                    world.x = pos.x;
                    world.z = pos.y;
                    if (targetWall == null)
                    {
                        Vector3 eulerAngles = targetObj.transform.rotation.eulerAngles;
                        Angle = angle - deltaAngle;
                        eulerAngles.y = - (Angle - forTop);//Angle 替换成Angle-180
                        targetObj.transform.rotation = Quaternion.Euler(eulerAngles);
                        targetWall = wall;
                    }
                }
            }
        }
        if (targetWall != null)
        {

        }
    }

    /// <summary>
    /// 吸附
    /// </summary>
    private void Adsorb(ref Vector3 world, ref float Angle)
    {
        targetWall = null;
        Vector2 world2d = Vector2.zero;
        world2d.x = world.x;
        world2d.y = world.z;
        for (int i = 0; i < data.wallList.Count; i++)
        {
            WallData wall = data.wallList[i];
            float angle = wall.GetWallAngle();
            float deltaAngle = angle - Angle;
            float xifudis = targetSize.z / 2;
            int times90 = Mathf.RoundToInt(deltaAngle / 90);
            deltaAngle = times90 * 90;
            if (times90 % 2 != 0)
            {
                xifudis = targetSize.x / 2;
            }
            float dis = xifudis + wall.width / 2;
            if (wall.GetDis(world2d) < dis)
            {
                Vector2 pos = wall.GetDisPoint(world2d);
                if (wallFunc.PointOnWall(pos, wall) || (wall.point1.pos - pos).magnitude < xifudis
                    || (wall.point2.pos - pos).magnitude < xifudis)
                {
                    world2d = wall.GetDisPoint(world2d, dis);
                    world.x = world2d.x;
                    world.z = world2d.y;
                    if (targetWall == null)
                    {
                        Vector3 eulerAngles = targetObj.transform.rotation.eulerAngles;
                        Angle = angle - deltaAngle;
                        eulerAngles.y =  - (Angle - forTop);
                        targetObj.transform.rotation = Quaternion.Euler(eulerAngles);
                        targetWall = wall;
                    }
                }
            }
        }
    }

    public Vector3 GetDisFloorPos()
    {
        Vector3 pos = targetObj.transform.position + Vector3.up * (targetSize.y + 0.5f);
        return pos;
    }
    
}
