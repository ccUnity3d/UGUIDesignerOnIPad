using UnityEngine;
using foundation;
using System;

public class PlaceGoodsState3D : InputState
{
    public const string NAME = "PlaceState3D";
    public PlaceGoodsState3D() : base(NAME)
    {

    }

    private GoodsVO targetVO;
    private Product targetProduct;

    private WallHelpFunc wallFunc = WallHelpFunc.Instance;
    private GameObject targetObj = null;
    private WallData targetWall = null;
    private Vector3 targetSize = Vector3.one;
    private float defaultHeight;
    private GameObject selectEmtyObj;
    private Vector2 lastMousePos = Vector2.zero;
    private RaycastHit[] hits = new RaycastHit[20];
    private SimpleOutterLoader loader;
    private float forTop = 180;

    public override void enter()
    {
        base.enter();
        //machine.selectGoods
        targetVO = mainpagedata.getGoods(machine.selectGoods[0].seekId);
        if (targetVO == null)
        {
            Debug.LogError("targetVO == null id = " + machine.selectGoods[0].seekId);
            return;
        }
        targetProduct = mainpagedata.getProduct(targetVO.seekId);
        if (targetProduct == null)
        {
            Debug.LogError("targetProduct == null id = " + machine.selectGoods[0].seekId);
            return;
        }
        GameObject go;
        if (selectEmtyObj == null)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            selectEmtyObj = new GameObject("placeEmtyObj");
            go.transform.parent = selectEmtyObj.transform;
        }
        else { go = selectEmtyObj.transform.GetChild(0).gameObject; }
        selectEmtyObj.gameObject.SetActive(true);
        Vector3 scale = targetProduct.size;
        go.transform.localScale = scale;
        go.transform.localPosition = scale.y / 2 * Vector3.up;
        targetObj = selectEmtyObj;
        LoadMode();
    }

    public override void exit()
    {
        if (loader != null)
        {
            if (loader.state == SimpleLoadedState.Loading)
            {
                Debug.LogWarning("Cancel loader " + loader.httpUrl);
                loader.Cancel();
            }
            else
            {

            }
            loader = null;
        }

        if (targetObj == selectEmtyObj)
        {
            selectEmtyObj.gameObject.SetActive(false);
        }
        else {
            ResourcesPool.Dispos(targetObj);
        }
        //if (targetObj != selectEmtyObj) GameObject.Destroy(targetObj);
        base.exit();
    }

    private void LoadMode()
    {
        //LoaderPool.InnerLoad(targetVO.modelUri, SimpleLoadDataType.prefabAssetBundle, onloaded, new object[1] { targetVO });
        string outUrl = targetVO.modelUri;// "http://midea-products.oss-cn-shanghai.aliyuncs.com/8b7df9b0-28f5-4c88-a412-324284ccc923/model.assetbundle";//10.0.21.27/yy/model.assetbundle";
        loader = LoaderPool.OutterLoad(outUrl, SimpleLoadDataType.prefabAssetBundle, onloaded, targetVO, onloadedBforClone);
    }

    private void onloadedBforClone(GameObject prefab, object bringData)
    {
        BoxCollider boxColli = prefab.AddComponent<BoxCollider>();
        GoodsVO vo = bringData as GoodsVO;
        Product product = mainpagedata.getProduct(vo.seekId);
        boxColli.size = product.size;
        boxColli.center = boxColli.center + Vector3.up * (boxColli.size.y / 2 - boxColli.center.y);
        Bounds bounds = default(Bounds);
        foreach (Transform item in prefab.transform)
        {
            Renderer renderer = item.GetComponent<Renderer>();
            if (renderer == null) continue;
            if (item.name == "shadow")
            {
                //if (product.entityType == "flooring")
                //{
                    item.gameObject.SetActive(false);
                //}
                //else {
                //    Shader shader = Shader.Find("Unlit/Transparent Colored");
                //    renderer.sharedMaterial.shader = shader;
                //}
            }
            else if (item.name == "snap_plane")
            {
                renderer.enabled = false; 
                //item.gameObject.AddComponent<BoxCollider>();
            }
            else {
                if (item.name == "glass")
                {
                    Shader shader = Shader.Find("Standard");
                    Material material = renderer.sharedMaterial;
                    material.shader = shader;
                    material.SetFloat("_Metallic", 0.1f);
                    material.SetFloat("_Glossiness", 0.88f);

                    //RenderingMode.Transparent:  
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;

                    //item.gameObject.AddComponent<BoxCollider>();
                }
                else
                {
                    Shader shader = Shader.Find("Unlit/Transparent Cutout");
                    renderer.sharedMaterial.shader = shader;
                }
                MeshFilter meshFilter = item.gameObject.GetComponent<MeshFilter>();
                if (meshFilter == null) continue;
                Bounds meshBounds = meshFilter.sharedMesh.bounds;
                if (bounds == default(Bounds))
                {
                    bounds = meshBounds;
                }
                else
                {
                    float minx = bounds.min.x;
                    float minz = bounds.min.z;
                    float maxx = bounds.max.x;
                    float maxz = bounds.max.z;
                    if (meshBounds.min.x < minx)
                    {
                        minx = meshBounds.min.x;
                    }
                    if (meshBounds.min.z < minz)
                    {
                        minz = meshBounds.min.z;
                    }
                    if (meshBounds.max.x > maxx)
                    {
                        maxx = meshBounds.max.x;
                    }
                    if (meshBounds.max.z > maxz)
                    {
                        maxz = meshBounds.max.z;
                    }
                    bounds.center = Vector3.right * (maxx + minx) / 2 + Vector3.forward * (maxz + minz) / 2;
                    bounds.extents = Vector3.right * (maxx - minx) / 2 + Vector3.forward * (maxz - minz) / 2;
                }
            }
        }
        if (product.contentTypeDetail.StartsWith("folding door") == true)
        {
            return;
        }
        if (bounds != default(Bounds))
        {
            Vector3 center = prefab.transform.InverseTransformPoint(bounds.center);
            center.y = 0;
            foreach (Transform item in prefab.transform)
            {
                item.localPosition = -center;
            }
        }
    }

    private void onloaded(object obj)
    {
        SimpleOutterLoader loader = obj as SimpleOutterLoader;
        if (loader.state == SimpleLoadedState.Failed)
        {
            return;
        }
        Debug.LogWarning("onloaded " + loader.httpUrl);

        GoodsVO vo = loader.bringData as GoodsVO;
        targetObj = loader.loadedData as GameObject;
        if (targetObj == null)
        {
            Debug.LogWarning(loader.uri);
            return;
        }
        selectEmtyObj.gameObject.SetActive(false);
        //targetObj.AddComponent<Goods3DView>();
        targetSize = targetProduct.size;
        //targetProduct = (targetVO.seekId);
        //targetObj.transform.rotation = Quaternion.Euler(Vector3.zero);
        targetObj.transform.parent = prefabs.parentTran_3D;
        targetObj.transform.localScale = Vector3.one;
        defaultHeight = targetProduct.defaultHeight;
        targetObj.transform.position = selectEmtyObj.transform.position;
        targetObj.transform.rotation = Quaternion.Euler(Vector3.up * (0 - forTop));
        //foreach (Transform item in targetObj.transform)
        //{
        //    if (item.name == "shadow")
        //    {
        //        item.localPosition = Vector3.up * -0.01f;
        //    }
        //}
        //Debug.LogWarning("view3D.AddProductGameObj(targetVO.id, targetObj);");
        //view3D.AddProductGameObj(targetVO.id, targetObj);
    }

    public override void mUpdate()
    {
        base.mUpdate();
        if (uguiHitUI.uiHited == true)
        {
            return;
        }
        
        Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);

        Vector2 pos = ray.origin;
        if (lastMousePos == pos && Input.GetMouseButtonDown(0) == false)
        {
            return;
        }
        lastMousePos = pos;

        Vector3 world = Vector3.one;
        
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
                    world = hits[i].point;
                    world.y = 0;
                }
            }
        }
        else {
            Vector3 v3 = inputCamera.transform.position;
            Vector3 v32 = ray.origin;
            world.y = 0;
            float lerp = (world.y - v32.y) / (v3.y - v32.y);
            world.x = lerp * (v3.x - v32.x) + v32.x;
            world.z = lerp * (v3.z - v32.z) + v32.z;
        }

        float Angle = - targetObj.transform.rotation.eulerAngles.y + forTop;

        world.y = targetProduct.defaultHeight;

        switch (targetVO.type)
        {
            case 1://吸附墙 
                {
                    Adsorb(ref world, ref Angle);
                }
                break;
            case 2://门窗 
                {
                    //if (targetProduct.entityType == "window")
                    //{
                    //    world.y = 1;
                    //}
                    Inlay(ref world, ref Angle);
                }
                break;
            case 3://吊顶 
                {
                    world.y = defaultSetting.DefaultHeight - targetSize.y + targetProduct.defaultHeight;
                }
                break;
            case 4://地毯 
                {
                    world.y = -0.001f + targetProduct.defaultHeight;
                }
                break;
            case 5://天花板
                {
                    world.y = defaultSetting.DefaultHeight - 0.001f+ targetProduct.defaultHeight;
                }
                break;
            case 6://吸附各个平面 暂时不做
                {
                    //world.y = defaultSetting.DefaultHeight - 0.01f;
                }
                break;
            default://不吸附
                break;
        }

        targetObj.transform.position = world;
        if (Input.GetMouseButtonDown(0))
        {
            undoHelper.save();
            GoodsVO vo = mainpagedata.getGoods(machine.selectGoods[0].seekId);
            ProductData productData = data.AddProduct(vo.id, world, Angle, targetProduct, targetWall, targetVO.type);
            RefreshView();//必须先刷新
            ToSelectGoodsState(productData);
        }
    }


    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
        if (uguiHitUI.uiHited == true)
        {
            return;
        }
        //Debug.LogWarning("Input.touchCount = "+ Input.touchCount);
        if (Input.touchCount != 1)
        {
            return;
        }
        Touch touch = Input.GetTouch(0);
        Ray ray = inputCamera.ScreenPointToRay(touch.position);
        Vector2 pos = ray.origin;
        if (lastMousePos == pos && touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
        {
            return;
        }
        lastMousePos = pos;

        Vector3 world = Vector3.one;
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
                    world = hits[i].point;
                    world.y = 0;
                }
            }
        }
        else {
            Vector3 v3 = inputCamera.transform.position;
            Vector3 v32 = ray.origin;
            world.y = 0;
            float lerp = (world.y - v32.y) / (v3.y - v32.y);
            world.x = lerp * (v3.x - v32.x) + v32.x;
            world.z = lerp * (v3.z - v32.z) + v32.z;
        }

        float Angle = -targetObj.transform.rotation.eulerAngles.y + forTop;

        world.y = targetProduct.defaultHeight;

        switch (targetVO.type)
        {
            case 1://吸附墙 
                {
                    Adsorb(ref world, ref Angle);
                }
                break;
            case 2://门窗 
                {
                    //if (targetProduct.entityType == "window")
                    //{
                    //    world.y = 1;
                    //}
                    Inlay(ref world, ref Angle);
                }
                break;
            case 3://吊顶 
                {
                    world.y = defaultSetting.DefaultHeight - targetSize.y + targetProduct.defaultHeight;
                }
                break;
            case 4://地毯 
                {
                    world.y = -0.001f + defaultSetting.DefaultHeight + targetProduct.defaultHeight;
                }
                break;
            case 5://天花板
                {
                    world.y = defaultSetting.DefaultHeight - 0.001f + targetProduct.defaultHeight;
                }
                break;
            case 6://吸附各个平面 暂时不做
                {
                    //world.y = defaultSetting.DefaultHeight - 0.01f;
                }
                break;
            default://不吸附
                break;
        }

        targetObj.transform.position = world;
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            undoHelper.save();

            Product product = targetProduct;
            ProductData productData = data.AddProduct(targetVO.id, world, Angle, product, targetWall, targetVO.type);
            RefreshView();//必须先刷新
            ToSelectGoodsState(productData);
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
            float deltaAngle = angle - (Angle - forTop);
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
                        eulerAngles.y = -(Angle - forTop);
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
                        eulerAngles.y = - (Angle - forTop);
                        targetObj.transform.rotation = Quaternion.Euler(eulerAngles);
                        targetWall = wall;
                    }
                }
            }
        }
    }

    private void ToSelectGoodsState(ProductData productData)
    {
        GoodsView view_2 = view2D.getGoodsView(productData);
        Goods3DView view_3 = view3D.getGoodsView(productData);
        SelectState state = getSelectState(SelectGoodsState2D.NAME);
        state.viewTarget = view_2;
        SelectState3D state3D = getSelectState3D(SelectGoodsState3D.NAME);
        state3D.viewTarget = view_3;
        if (view_3 == null)
        {
            state3D.viewData = productData;
        }
        setState(SelectGoodsState3D.NAME);
    }

}