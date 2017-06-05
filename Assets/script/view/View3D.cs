using foundation;
using System;
using System.Collections.Generic;
using UnityEngine;

public class View3D : BaseView
{
    public View3D()
    {

    }

    public List<ProductData> selectGroup = new List<ProductData>();

    public ObjData selectObjData;
    public GameObject selectWallSide;
    public GameObject selectFloor;
    public GameObject selectCeiling;
    public GameObject selectGoods;

    public bool topHide = false;

    public bool halfWall = false;
    private float halfWallHeight
    {
        get {
            return defaultSetting.HalfWallHeight;
        }
    }

    //
    private List<Goods3DView> goodsViewList = new List<Goods3DView>();
    private List<WallSideView> wallSideViewList = new List<WallSideView>();
    
    //
    private List<GameObject> wallupLines = new List<GameObject>();
    private List<GameObject> wallsideLines = new List<GameObject>();
    private List<GameObject> wallsideTLines = new List<GameObject>();
    private List<GameObject> wallsideTopLines = new List<GameObject>();

    private List<GameObject> floors = new List<GameObject>();
    private List<GameObject> ceilings = new List<GameObject>();
    private List<GameObject> wallEndLines = new List<GameObject>();
    private List<GameObject> DoorWindowSideLines = new List<GameObject>();

    //
    private Dictionary<int, List<GameObject>> products = new Dictionary<int, List<GameObject>>();

    private List<Vector2> tempupDoorsUVList = new List<Vector2>();
    private List<Vector2> tempDoorsUVList = new List<Vector2>();

    private Vector2 uvHelpCenter = Vector2.zero;
    private Vector2 uvHelpPos = Vector2.zero;
    private Vector2 uvHelpExtents = Vector2.zero;
    private Vector2 caculateV2 = Vector2.zero;

    private List<int> removeListId = new List<int>();

    private List<Vector3> tempv3s = new List<Vector3>();

    /// <summary>
    /// 正在加载的3D模型 
    /// 刷新前若未完成需要关闭的项（view自己的列表 也可以是外部使用到加载并实例化的loader）
    /// </summary>
    public List<SimpleOutterLoader> loaders = new List<SimpleOutterLoader>();

    private static View3D instance;
    private int[] EmtyArr = new int[] { };
    private List<Vector2> tempUVs = new List<Vector2>()
    {
         Vector2.up, Vector2.one
        ,Vector2.up*0.9f, Vector2.right + Vector2.up * 0.9f
        ,Vector2.up*0.1f, Vector2.right + Vector2.up * 0.1f
        ,Vector2.zero, Vector2.right
    };
    private List<int> tempTTriangles = new List<int>()
    {
         0,1,2//2,1,0//
        ,3,2,1//1,2,3//
        ,2,3,4//4,3,2//
        ,5,4,3//3,4,5//
        ,4,5,6//6,5,4//
        ,7,6,5//5,6,7//
    };
    private List<int> tempTopTriangles = new List<int>()
    {
         2,1,0//0,1,2//
        ,1,2,3//3,2,1//
        ,4,3,2//2,3,4//
        ,3,4,5//5,4,3//
        ,6,5,4//4,5,6//
        ,5,6,7//7,6,5//
    };

    public static View3D Instance
    {
        get
        {
            if (instance == null) instance = new View3D();
            return instance;
        }
    }

    public void ClearView()
    {
        selectObjData = null;

        ClearList(goodsViewList);
        ClearList(wallSideViewList);

        ClearList(wallupLines);
        ClearList(wallsideLines);
        ClearList(wallsideTLines);
        ClearList(wallsideTopLines);

        ClearList(floors);
        ClearList(ceilings);
        ClearList(wallEndLines);
        ClearList(DoorWindowSideLines);

        foreach (List<GameObject> item in products.Values)
        {
            ClearList(item);
        }
        products.Clear();

    }
    private void ClearList<T>(List<T> list) where T : ObjView
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject.Destroy(list[i].gameObject);
        }
        list.Clear();
    }
    private void ClearList(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject.Destroy(list[i]);
        }
        list.Clear();
    }

    public override void RefreshView()
    {
        ///关闭正在加载的
        //Debug.Log("view3D RefreshView");
        RefreshGameObjs();
        base.RefreshView();
        int materalT = 0;//0 null 1 floor 2 ceiling

        #region room
        for (int i = 0; i < data.roomList.Count; i++)
        {
            RoomData room = data.roomList[i];
            List<Vector2> v2s = roomfunc.GetVectors3D(room);
            Vector2 v2 = v2s[0];
            for (int k = 0; k < v2s.Count; k++)
            {
                v2s[k] = v2s[k] - v2;
            }
            
            MaterialData materialData = room.floor;
            FloorView floor = floors[i].GetComponent<FloorView>();
            floors[i].transform.localPosition = new Vector3(v2.x, -0.002f, v2.y);
            linefunc.SetMesh3D(v2s, floors[i], materialData.offsetX, materialData.offsetY, materialData.rotation, materialData.tileSize_x, materialData.tileSize_y, true, true);
            floor.SetData(materialData);
            floor.name = "floor " + i;
            floors[i].SetActive(true);
            SetTextureTool.SetTexture(floors[i], materialData.textureURI, materialData.seekId);
            if (selectObjData != null && materialData == selectObjData)
            {
                materalT = 1;
                if (selectFloor == null) selectFloor = prefabs.GetNewInstance_selectQuad().gameObject;
                selectFloor.transform.localPosition = floors[i].transform.localPosition + Vector3.up * 0.001f;
                linefunc.SetMesh3D(v2s, selectFloor, materialData.offsetX, materialData.offsetY, materialData.rotation, materialData.tileSize_x, materialData.tileSize_y, true, true);
                selectFloor.SetActive(true);
            }

            materialData = room.ceiling;
            CeilingView ceiling = ceilings[i].GetComponent<CeilingView>();
            ceilings[i].transform.localPosition = new Vector3(v2.x, defaultSetting.DefaultHeight, v2.y);
            linefunc.SetMesh3D(v2s, ceilings[i], materialData.offsetX, materialData.offsetY, materialData.rotation, materialData.tileSize_x, materialData.tileSize_y, true, false);
            ceiling.SetData(room.ceiling);
            ceiling.name = "ceiling " + i;
            ceilings[i].SetActive(true);
            if (topHide == false)
            {
                ceilings[i].SetActive(true);
                if (halfWall == true)
                {
                    //半高墙不隐藏顶
                    //ceilings[i].SetActive(false);
                }
            }
            else
            {
                //隐藏顶
                ceilings[i].SetActive(false);
            }
            SetTextureTool.SetTexture(ceilings[i], materialData.textureURI, materialData.seekId);
            if (selectObjData != null && materialData == selectObjData)
            {
                materalT = 2;
                if (selectCeiling == null) selectCeiling = prefabs.GetNewInstance_selectQuad().gameObject;
                selectCeiling.transform.localPosition = ceilings[i].transform.localPosition - Vector3.up * 0.001f;
                linefunc.SetMesh3D(v2s, selectCeiling, materialData.offsetX, materialData.offsetY, materialData.rotation, materialData.tileSize_x, materialData.tileSize_y, true, false);
                selectCeiling.SetActive(true);
            }
        }

        if (materalT != 1 && selectFloor != null)
        {
            selectFloor.SetActive(false);
        }
        if (materalT != 2 && selectCeiling != null)
        {
            selectCeiling.SetActive(false);
        }
        #endregion

        #region wall
        for (int i = 0; i < data.wallList.Count; i++)
        {
            WallData walldata = data.wallList[i];

            GameObject wallline = wallupLines[i];
            GameObject wallLeft = wallsideLines[2 * i];
            GameObject wallRight = wallsideLines[2 * i + 1];
            GameObject wallLeftTLine = wallsideTLines[2 * i];
            GameObject wallRightTLine = wallsideTLines[2 * i + 1];
            GameObject wallLeftTopLine = wallsideTopLines[2 * i];
            GameObject wallRightTopLine = wallsideTopLines[2 * i + 1];
            GameObject wallLeftEndp1 = wallEndLines[4 * i];
            GameObject wallLeftEndp2 = wallEndLines[4 * i + 1];
            GameObject wallRightEndp1 = wallEndLines[4 * i + 2];
            GameObject wallRightEndp2 = wallEndLines[4 * i + 3];

            if (walldata.hide == true)
            {
                wallline.SetActive(false);
                wallLeft.SetActive(false);
                wallRight.SetActive(false);
                wallLeftTLine.SetActive(false);
                wallRightTLine.SetActive(false);
                wallLeftTopLine.SetActive(false);
                wallRightTopLine.SetActive(false);
                wallLeftEndp1.SetActive(false);
                wallLeftEndp2.SetActive(false);
                wallRightEndp1.SetActive(false);
                wallRightEndp2.SetActive(false);
                continue;
            }
            else {
                wallline.SetActive(true);
                wallLeft.SetActive(true);
                wallRight.SetActive(true);
                wallLeftTLine.SetActive(true);
                wallRightTLine.SetActive(true);
                wallLeftTopLine.SetActive(true);
                wallRightTopLine.SetActive(true);
                wallLeftEndp1.SetActive(true);
                wallLeftEndp2.SetActive(true);
                wallRightEndp1.SetActive(true);
                wallRightEndp2.SetActive(true);
            }
            SetObjWithData(walldata, wallline, wallLeft, wallRight, wallLeftEndp1, wallLeftEndp2, wallRightEndp1, wallRightEndp2
                , wallLeftTLine, wallRightTLine, wallLeftTopLine, wallRightTopLine);
        }

        if (selectObjData == null || selectObjData is WallSideData == false)
        {
            if (selectWallSide != null) selectWallSide.SetActive(false);
        }
        #endregion

    }
    
    private void LoadMode(ProductData product)
    {
        GoodsVO vo = mainPageData.getGoods(product.seekId);
        if (vo == null)
        {
            Debug.LogWarning("proxy.getGood(id) == null id = " + product.id);
            return;
        }
        //InnerAssetResource resource = new InnerAssetResource(vo.modelUri);
        //resource.userData = new object[2] { vo, product };
        //AssetsManager.bindEventHandle(resource, LoadSuccess, true);
        //resource.load();
        SimpleOutterLoader loader = LoaderPool.OutterLoad(vo.modelUri, SimpleLoadDataType.prefabAssetBundle, onloaded, product, onloadedBforClone);
        loaders.Add(loader);
    }


    private void onloadedBforClone(GameObject prefab, object bringData)
    {
        if (prefab == null) return;
        BoxCollider boxColli = prefab.AddComponent<BoxCollider>();
        ProductData productdata = bringData as ProductData;
        Product product = mainPageData.getProduct(productdata.seekId);
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

    public void AddProductGameObj(int id, GameObject targetObj)
    {
        //Debug.LogWarning("AddProductGameObj " + id);
        if (products.ContainsKey(id) == false) products.Add(id, new List<GameObject>());
        products[id].Add(targetObj);
    }

    private void onloaded(object obj)
    {
        SimpleOutterLoader loader = obj as SimpleOutterLoader;
        loaders.Remove(loader);
        ProductData productData = loader.bringData as ProductData;
        GameObject instanceObj = loader.loadedData as GameObject;
        if (instanceObj == null)
        {
            Debug.LogWarning(loader.uri);
            return;
        }
        Goods3DView goodsView = instanceObj.AddComponent<Goods3DView>();
        goodsView.SetData(productData);
        //Debug.LogWarning("AddProductGameObj targetObj");
        AddProductGameObj(productData.id, instanceObj);
        Product product = mainPageData.getProduct(productData.seekId);
        float y = productData.height;
        switch (productData.type)
        {
            case 2://门窗 
                {
                    //if (product.entityType == "window")
                    //{
                    //    y = 1;
                    //}
                }
                break;
            case 3://吊顶 
                {
                    if (product == null) break;
                    y = productData.height + defaultSetting.DefaultHeight - product.size.y;
                }
                break;
            case 4://地毯 
                {
                    y = productData.height + -0.001f;
                }
                break;
            case 5://天花板
                {
                    y = productData.height + defaultSetting.DefaultHeight - 0.001f;
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
        productData.setData(instanceObj, Vector3.one, y);
        instanceObj.transform.parent = prefabs.parentTran_3D;
        //BoxCollider boxColli = instanceObj.AddComponent<BoxCollider>();
        instanceObj.transform.localScale = productData.scale;
        //Product product = mainPageData.getProduct(productData.seekId);
        //boxColli.size = product.size;
        //boxColli.center = boxColli.center + Vector3.up * (boxColli.size.y / 2 - boxColli.center.y);
        instanceObj.SetActive(!productData.hide);
        goodsViewList.Add(goodsView);
    }
    

    public void SetObjWithData(WallData walldata, GameObject wallbody, GameObject wallLeft, GameObject wallRight, 
        GameObject wallLeftEndp1, GameObject wallLeftEndp2, GameObject wallRightEndp1, GameObject wallRightEndp2
        , GameObject wallLeftTLine, GameObject wallRightTLine, GameObject wallLeftTopLine, GameObject wallRightTopLine)
    {

        float height = defaultSetting.DefaultHeight;
        List<WallData> farstWalls = roomfunc.GetFarstWalls(prefabs.mainCamera);
        if (halfWall == true && farstWalls.IndexOf(walldata) == -1)
        {
            height = halfWallHeight;
        }

        //walldata边线形成的 多边形 的 顶点集合
        List<Vector2> v2s = wallfunc.GetWallVerticesPos(walldata);
        Vector2 v2s0 = v2s[0];
        Vector2 v2s1 = v2s[1];
        Vector2 v2s2 = v2s[2];
        Vector2 v2s3 = v2s[3];
        Vector2 v2s4 = v2s[4];
        Vector2 v2s5 = v2s[5];
        Vector3 pos;
        float dis;
        float angle1 = linefunc.GetAngle(v2s3 - v2s0);
        float angle2 = linefunc.GetAngle(v2s0 - v2s3);

        ///墙两边的倾斜角度偏出量
        float angle = linefunc.GetClockAngle(v2s1, v2s0, v2s3);
        float len = Vector2.Distance(v2s1, v2s0);
        float rightMin = len * Mathf.Cos(angle * Mathf.PI / 180f);
        angle = linefunc.GetClockAngle(v2s2, v2s3, v2s0);
        len = Vector2.Distance(v2s2, v2s3);
        float rightMax = -len * Mathf.Cos(angle * Mathf.PI / 180f);
        angle = linefunc.GetClockAngle(v2s4, v2s3, v2s0);
        len = Vector2.Distance(v2s4, v2s3);
        float leftMin = len * Mathf.Cos(angle * Mathf.PI / 180f);
        angle = linefunc.GetClockAngle(v2s5, v2s0, v2s3);
        len = Vector2.Distance(v2s5, v2s0);
        float leftMax = -len * Mathf.Cos(angle * Mathf.PI / 180f);
        float totalDis = Vector2.Distance(walldata.point1.pos, walldata.point2.pos);

        #region 踢脚线 顶角线
        //踢脚线厚度
        TBaseboard tBaseboardRight = walldata.point1To2Data.tBaseboard;
        TBaseboard tBaseboardLeft = walldata.point2To1Data.tBaseboard;

        float multiRight = tBaseboardRight.width / walldata.width;
        Vector2 RightTp1 = multiRight * (v2s1 - v2s0) + v2s1;
        Vector2 RightTp2 = multiRight * (v2s2 - v2s3) + v2s2;
        float multiLeft = tBaseboardLeft.width / walldata.width;
        Vector2 LeftTp1 = multiLeft * (v2s4 - v2s3) + v2s4;
        Vector2 LeftTp2 = multiLeft * (v2s5 - v2s0) + v2s5;


        //踢脚线
        if (tBaseboardRight.hide == false)
        {
            wallRightTLine.SetActive(true);
            SetBaseboardMesh(tBaseboardRight, wallRightTLine, v2s1, v2s2, RightTp1, RightTp2);
        }
        else
        {
            wallRightTLine.SetActive(false);
        }
        if (tBaseboardLeft.hide == false)
        {
            wallLeftTLine.SetActive(true);
            SetBaseboardMesh(tBaseboardLeft, wallLeftTLine, v2s4, v2s5, LeftTp1, LeftTp2);
        }
        else
        {
            wallLeftTLine.SetActive(false);
        }
       
        //顶角线
        TBaseboard topBaseboardRight = walldata.point1To2Data.topBaseboard;
        if (topBaseboardRight.hide == false)
        {
            wallRightTopLine.SetActive(true);
            SetBaseboardMesh(topBaseboardRight, wallRightTopLine, v2s1, v2s2, RightTp1, RightTp2);
        }
        else
        {
            wallRightTopLine.SetActive(false);
        }
        TBaseboard topBaseboardLeft = walldata.point2To1Data.topBaseboard;
        if (topBaseboardLeft.hide == false)
        {
            wallLeftTopLine.SetActive(true);
            SetBaseboardMesh(topBaseboardLeft, wallLeftTopLine, v2s4, v2s5, LeftTp1, LeftTp2);
        }
        else
        {
            wallLeftTopLine.SetActive(false);
        }
        #endregion


        #region wallRight
        List<Vector2> uppointListRight = GetWallUpDoorsUVs(walldata, true, rightMin, rightMax, leftMin, leftMax, totalDis);//new List<Vector2>();//
        List<Vector2> pointListRight = GetWallDoorsUVs(walldata, true, rightMin, rightMax, leftMin, leftMax, totalDis);
        pos = v2s1;
        dis = (v2s2 - v2s1).magnitude;
        wallRight.transform.localPosition = GetVector3(pos.x, 0, pos.y);
        wallRight.transform.localRotation = Quaternion.Euler(-Vector3.up * angle1);
        MaterialData materialDataRight = walldata.point1To2Data.materialData;
        linefunc.SetMesh(dis, height, wallRight, pointListRight, uppointListRight, materialDataRight.offsetX, materialDataRight.offsetY, materialDataRight.rotation, materialDataRight.tileSize_x, materialDataRight.tileSize_y);
        wallRight.SetActive(true);
        WallSideView rightSide = wallRight.GetComponent<WallSideView>();
        rightSide.SetData(walldata.point1To2Data);
        if (selectObjData != null && walldata.point1To2Data == selectObjData)
        {
            Vector2 posR = walldata.GetDisPoint(pos, walldata.width/2 + 0.001f);
            if (selectWallSide == null) selectWallSide = prefabs.GetNewInstance_selectQuad().gameObject;
            selectWallSide.transform.localPosition = GetVector3(posR.x, 0, posR.y);
            selectWallSide.transform.localRotation = wallRight.transform.localRotation;
            linefunc.SetMesh(dis, height, selectWallSide, pointListRight, uppointListRight);
            selectWallSide.SetActive(true);
        }
        SetTextureTool.SetTexture(wallRight, walldata.point1To2Data.materialData.textureURI, walldata.point1To2Data.materialData.seekId);
        #endregion

        #region wallLeft
        List<Vector2> uppointListLeft = GetWallUpDoorsUVs(walldata, false, rightMin, rightMax, leftMin, leftMax, totalDis);//new List<Vector2>();//
        List<Vector2> pointListLeft = GetWallDoorsUVs(walldata, false, rightMin, rightMax, leftMin, leftMax, totalDis);
        pos = v2s4;
        dis = (v2s5 - v2s4).magnitude;
        wallLeft.transform.localPosition = GetVector3(pos.x, 0, pos.y);
        wallLeft.transform.localRotation = Quaternion.Euler(-Vector3.up * angle2);
        MaterialData materialDataLeft = walldata.point2To1Data.materialData;
        linefunc.SetMesh(dis, height, wallLeft, pointListLeft, uppointListLeft, materialDataLeft.offsetX,materialDataLeft.offsetY, materialDataLeft.rotation, materialDataLeft.tileSize_x, materialDataLeft.tileSize_y);
        wallLeft.SetActive(true);
        WallSideView leftSide = wallLeft.GetComponent<WallSideView>();
        leftSide.SetData(walldata.point2To1Data);
        if (selectObjData != null && walldata.point2To1Data == selectObjData)
        {
            Vector2 posL = walldata.GetDisPoint(pos, walldata.width / 2 + 0.001f);
            if (selectWallSide == null) selectWallSide = prefabs.GetNewInstance_selectQuad().gameObject;
            selectWallSide.transform.localPosition = GetVector3(posL.x, 0, posL.y);
            selectWallSide.transform.localRotation = leftSide.transform.localRotation;
            linefunc.SetMesh(dis, height, selectWallSide, pointListLeft, uppointListLeft);
            selectWallSide.SetActive(true);
        }
        SetTextureTool.SetTexture(wallLeft, walldata.point2To1Data.materialData.textureURI, walldata.point2To1Data.materialData.seekId);
        #endregion

        #region wallbody
        pos = v2s0;
        wallbody.transform.localPosition = GetVector3(pos.x, height, pos.y);
        wallbody.transform.localRotation = Quaternion.Euler(-Vector3.up * (angle1 - 90) + Vector3.right*90f);
        List<Vector2> newV2s = wallfunc.GetWallVerticesLocalPos(v2s, walldata.width, 1);
        linefunc.SetMesh(newV2s, wallbody);
        wallbody.SetActive(true);
        #endregion 

        #region wallend
        pos = v2s0;
        dis = (v2s5 - v2s0).magnitude;
        wallLeftEndp1.transform.localPosition = GetVector3(pos.x, height, pos.y);
        float angleLeftP1 = linefunc.GetAngle(v2s5 - v2s0);
        wallLeftEndp1.transform.localRotation = Quaternion.Euler(-Vector3.up * angleLeftP1 + Vector3.right * 180);
        linefunc.SetMesh(dis, height, wallLeftEndp1);

        pos = v2s0;
        dis = (v2s1 - v2s0).magnitude;
        wallRightEndp1.transform.localPosition = GetVector3(pos.x, 0, pos.y);
        float angleRightP1 = linefunc.GetAngle(v2s1 - v2s0);
        wallRightEndp1.transform.localRotation = Quaternion.Euler(-Vector3.up * angleRightP1);
        linefunc.SetMesh(dis, height, wallRightEndp1);

        pos = v2s3;
        dis = (v2s2 - v2s3).magnitude;
        wallLeftEndp2.transform.localPosition = GetVector3(pos.x, height, pos.y);
        float angleLeftP2 = linefunc.GetAngle(v2s2 - v2s3);
        wallLeftEndp2.transform.localRotation = Quaternion.Euler(-Vector3.up * angleLeftP2 + Vector3.right * 180);
        linefunc.SetMesh(dis, height, wallLeftEndp2);

        pos = v2s3;
        dis = (v2s4 - v2s3).magnitude;
        wallRightEndp2.transform.localPosition = GetVector3(pos.x, 0, pos.y);
        float angleRightP2 = linefunc.GetAngle(v2s4 - v2s3);
        wallRightEndp2.transform.localRotation = Quaternion.Euler(-Vector3.up * angleRightP2);
        linefunc.SetMesh(dis, height, wallRightEndp2);
        #endregion

        #region wallProduct
        int wallproductIndex = 0;
        for (int i = 0; i < data.productDataList.Count; i++)
        {
            ProductData productdata = data.productDataList[i];
            float scaleX = Mathf.Abs(productdata.scale.x);
            float scaleY = Mathf.Abs(productdata.scale.y);
            Product product = mainPageData.getProduct(productdata.seekId);
            float sizeX = Mathf.Abs(productdata.scale.x);
            float sizeY = Mathf.Abs(productdata.scale.y);
            if (productdata.type == 2 && productdata.targetWall != null)
            {
                if (productdata.targetWall == walldata)
                {
                    float productDis = (walldata.point1.pos - GetVector2(productdata.position.x, productdata.position.z)).magnitude;
                    Vector3 pos1 = GetVector3(walldata.point1.pos.x, productdata.height, walldata.point1.pos.y);
                    Vector3 pos2 = GetVector3(walldata.point2.pos.x, productdata.height, walldata.point2.pos.y);
                    float lerpValue1 = (productDis - scaleX * sizeX / 2) / totalDis;
                    float lerpValue2 = (productDis + scaleX * sizeX / 2) / totalDis;
                    Vector3 V1 = Vector3.Lerp(pos1, pos2, lerpValue1);//(产品在墙内部靠近point1的点)
                    Vector3 V2 = Vector3.Lerp(pos1, pos2, lerpValue2);//(产品在墙内部靠近point2的点)
                    GameObject wallrightDoordown = DoorWindowSideLines[8 * wallproductIndex];
                    GameObject wallrightDoorup = DoorWindowSideLines[8 * wallproductIndex + 2];
                    GameObject wallrightDoorright = DoorWindowSideLines[8 * wallproductIndex + 4];
                    GameObject wallrightDoorleft = DoorWindowSideLines[8 * wallproductIndex + 6];
                    GameObject wallleftDoordown = DoorWindowSideLines[8 * wallproductIndex + 1];
                    GameObject wallleftDoorup = DoorWindowSideLines[8 * wallproductIndex + 3];
                    GameObject wallleftDoorright = DoorWindowSideLines[8 * wallproductIndex + 5];
                    GameObject wallleftDoorleft = DoorWindowSideLines[8 * wallproductIndex + 7];
                    wallrightDoordown.SetActive(true);
                    wallrightDoorup.SetActive(true);
                    wallrightDoorright.SetActive(true);
                    wallrightDoorleft.SetActive(true);
                    wallleftDoordown.SetActive(true);
                    wallleftDoorup.SetActive(true);
                    wallleftDoorright.SetActive(true);
                    wallleftDoorleft.SetActive(true);

                    linefunc.SetMesh(product.size.x, walldata.width / 2, wallrightDoordown);
                    linefunc.SetMesh2(product.size.x, walldata.width / 2, wallrightDoorup);
                    linefunc.SetMesh(walldata.width / 2, product.size.y, wallrightDoorright);
                    linefunc.SetMesh2(walldata.width / 2, product.size.y, wallrightDoorleft);
                    linefunc.SetMesh(product.size.x, walldata.width / 2, wallleftDoordown);
                    linefunc.SetMesh2(product.size.x, walldata.width / 2, wallleftDoorup);
                    linefunc.SetMesh(walldata.width / 2, product.size.y, wallleftDoorright);
                    linefunc.SetMesh2(walldata.width / 2, product.size.y, wallleftDoorleft);

                    wallrightDoordown.transform.localPosition = V2;
                    wallrightDoorup.transform.localPosition = /*V1*/V2 + Vector3.up * product.size.y;
                    wallrightDoorright.transform.localPosition = V2;
                    wallrightDoorleft.transform.localPosition = V1;// + Vector3.up * product.size.y;
                    wallleftDoordown.transform.localPosition = V1;
                    wallleftDoorup.transform.localPosition = /*V2*/V1 + Vector3.up * product.size.y;
                    wallleftDoorright.transform.localPosition = V1;
                    wallleftDoorleft.transform.localPosition = V2;// + Vector3.up * product.size.y;

                    wallrightDoordown.transform.localRotation = Quaternion.Euler(Vector3.up * -angle2 + Vector3.right * 90f);
                    wallrightDoorup.transform.localRotation = wallrightDoordown.transform.localRotation;//Quaternion.Euler(Vector3.up * -angle1 + Vector3.right * -90f);
                    wallrightDoorright.transform.localRotation = Quaternion.Euler(Vector3.up * (-angle1 + 90f));
                    wallrightDoorleft.transform.localRotation = wallrightDoorright.transform.localRotation;//Quaternion.Euler(Vector3.up * (-angle1 + 90f) + Vector3.right * 180f);
                    wallleftDoordown.transform.localRotation = Quaternion.Euler(Vector3.up * -angle1 + Vector3.right * 90f);
                    wallleftDoorup.transform.localRotation = wallleftDoordown.transform.localRotation;//Quaternion.Euler(Vector3.up * -angle2 + Vector3.right * -90f);
                    wallleftDoorright.transform.localRotation = Quaternion.Euler(Vector3.up * (-angle2 + 90f));
                    wallleftDoorleft.transform.localRotation = wallleftDoorright.transform.localRotation;//Quaternion.Euler(Vector3.up * (-angle2 + 90f) + Vector3.right * 180f);

                    wallrightDoordown.name = "1";
                    wallrightDoorup.name = "2";
                    wallrightDoorright.name = "3";
                    wallrightDoorleft.name = "4";
                    wallleftDoordown.name = "5";
                    wallleftDoorup.name = "6";
                    wallleftDoorright.name = "7";
                    wallleftDoorleft.name = "8";

                    wallrightDoordown.name = "1";
                    wallrightDoorup.name = "2";
                    wallrightDoorright.name = "3";
                    wallrightDoorleft.name = "4";
                    wallleftDoordown.name = "5";
                    wallleftDoorup.name = "6";
                    wallleftDoorright.name = "7";
                    wallleftDoorleft.name = "8";

                    wallrightDoordown.SetActive(true);
                    wallrightDoorup.SetActive(true);
                    wallrightDoorright.SetActive(true);
                    wallrightDoorleft.SetActive(true);
                    wallleftDoordown.SetActive(true);
                    wallleftDoorup.SetActive(true);
                    wallleftDoorright.SetActive(true);
                    wallleftDoorleft.SetActive(true);

                    //wallrightDoordown.GetComponent<Renderer>().material.mainTexture = ;
                    //wallrightDoorup.GetComponent<Renderer>().material.mainTexture = ;
                    //wallrightDoorright.GetComponent<Renderer>().material.mainTexture = ;
                    //wallrightDoorleft.GetComponent<Renderer>().material.mainTexture = ;
                    //wallleftDoordown.GetComponent<Renderer>().material.mainTexture = ;
                    //wallleftDoorup.GetComponent<Renderer>().material.mainTexture = ;
                    //wallleftDoorright.GetComponent<Renderer>().material.mainTexture = ;
                    //wallleftDoorleft.GetComponent<Renderer>().material.mainTexture = ;

                }
                wallproductIndex++;
            }
        }
        #endregion

    }

    private Vector2 GetVector2(Vector3 pos)
    {
        return pos.x * Vector2.right + pos.z * Vector2.up;
    }
    private Vector2 GetVector2(float x, float y)
    {
        return x * Vector2.right + y * Vector2.up;
    }
    private Vector3 GetVector3(float x, float y, float z)
    {
        return x * Vector3.right + y * Vector3.up + z * Vector3.forward;
    }

    /// <summary>
    /// 产品在墙中的UV 计算左右边界的偏移量和边缘限制   仅窗户与墙上边缘产生交叉的部分
    /// </summary>
    /// <param name="walldata"></param>
    private List<Vector2> GetWallUpDoorsUVs(WallData walldata, bool right, float rightMin, float rightMax, float leftMin, float leftMax, float totalDis)
    {
        tempupDoorsUVList.Clear();
        List<ProductData> ProductDataList = data.GetWallProdust(walldata);
        if (ProductDataList.Count == 0)
        {
            return tempupDoorsUVList;
        }

        float height = defaultSetting.DefaultHeight;
        List<WallData> farstWalls = roomfunc.GetFarstWalls(prefabs.mainCamera);
        if (halfWall == true && farstWalls.IndexOf(walldata) == -1)
        {
            height = halfWallHeight;
        }

        if (right)
        {
            ProductDataList.Sort(
                delegate (ProductData data1, ProductData data2)
                {
                    float dis1 = Vector2.Distance(walldata.point1.pos, GetVector2(data1.position));
                    float dis2 = Vector2.Distance(walldata.point1.pos, GetVector2(data2.position));
                    return dis1 <= dis2 ? 1 : -1;
                }
            );
            float rightTotalDis = totalDis - rightMin + rightMax;

            for (int i = 0; i < ProductDataList.Count; i++)
            {
                ProductData productData = ProductDataList[i];
                float scaleX = Mathf.Abs(productData.scale.x);
                float scaleY = Mathf.Abs(productData.scale.y);
                Vector2 productPos2D = GetVector2(productData.position);
                float dis = Vector2.Distance(walldata.point1.pos, productPos2D);
                float dis2 = Vector2.Distance(walldata.point2.pos, productPos2D);
                if (dis < dis2 && wallfunc.PointOnWall(productPos2D, walldata) == false)
                {
                    dis = -dis;
                }
                Product product = mainPageData.getProduct(productData.seekId);
                float sizeX = Mathf.Abs(product.size.x);
                float sizeY = Mathf.Abs(product.size.y);
                if (product == null)
                {
                    Debug.LogWarning("seekId = " + productData.seekId + "不存在");
                    continue;
                }

                if (productData.height + sizeY < height || productData.height > height)
                {
                    continue;
                }

                uvHelpCenter.x = dis - rightMin;
                uvHelpCenter.y = productData.height;
                uvHelpExtents.x = scaleX * sizeX / 2;
                uvHelpExtents.y = scaleY * sizeY;
                
     
                uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -leftMax - rightMin + 0.1f);
                uvHelpPos.y = height;
                tempupDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -leftMax - rightMin + 0.1f);
                uvHelpPos.y = uvHelpCenter.y;
                tempupDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Min(uvHelpCenter.x + uvHelpExtents.x, rightTotalDis - 0.1f, rightTotalDis - (leftMin + rightMax) - 0.1f);
                uvHelpPos.y = uvHelpCenter.y;
                tempupDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Min(uvHelpCenter.x + uvHelpExtents.x, rightTotalDis - 0.1f, rightTotalDis - (leftMin + rightMax) - 0.1f);
                uvHelpPos.y = height;
                tempupDoorsUVList.Add(uvHelpPos);
            }
        }
        else
        {
            ProductDataList.Sort(
                delegate (ProductData data1, ProductData data2)
                {
                    float dis1 = Vector2.Distance(walldata.point2.pos, GetVector2(data1.position));
                    float dis2 = Vector2.Distance(walldata.point2.pos, GetVector2(data2.position));
                    return dis1 <= dis2 ? 1 : -1;
                }
            );
            float leftTotalDis = totalDis - leftMin + leftMax;
            for (int i = 0; i < ProductDataList.Count; i++)
            {
                ProductData productData = ProductDataList[i];
                float scaleX = Mathf.Abs(productData.scale.x);
                float scaleY = Mathf.Abs(productData.scale.y);
                //productData.height = 0.3f;
                //float dis = Vector2.Distance(walldata.point2.pos, GetVector2(productData.position));
                Vector2 productPos2D = GetVector2(productData.position);
                float dis = Vector2.Distance(walldata.point2.pos, productPos2D);
                float dis2 = Vector2.Distance(walldata.point1.pos, productPos2D);
                if (dis < dis2 && wallfunc.PointOnWall(productPos2D, walldata) == false)
                {
                    dis = -dis;
                }

                Product product = mainPageData.getProduct(productData.seekId);
                float sizeX = Mathf.Abs(product.size.x);
                float sizeY = Mathf.Abs(product.size.y);
                if (product == null)
                {
                    Debug.LogWarning("seekId = " + productData.seekId + "不存在");
                    continue;
                }

                if (productData.height + sizeY < height || productData.height > height)
                {
                    continue;
                }


                uvHelpCenter.x = dis - leftMin;
                uvHelpCenter.y = productData.height;
                uvHelpExtents.x = scaleX * sizeX / 2;
                uvHelpExtents.y = scaleY * sizeY;


                uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -rightMax - leftMin + 0.1f);
                uvHelpPos.y = height;
                tempupDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -rightMax - leftMin + 0.1f);
                uvHelpPos.y = uvHelpCenter.y;
                tempupDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Min(uvHelpCenter.x + uvHelpExtents.x, leftTotalDis - 0.1f, leftTotalDis - (rightMin + leftMax) - 0.1f);
                uvHelpPos.y = uvHelpCenter.y;
                tempupDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Min(uvHelpCenter.x + uvHelpExtents.x, leftTotalDis - 0.1f, leftTotalDis - (rightMin + leftMax) - 0.1f);
                uvHelpPos.y = height;
                tempupDoorsUVList.Add(uvHelpPos);

            }
        }
        return tempupDoorsUVList;
    }


    /// <summary>
    /// 产品在墙中的UV 计算左右边界的偏移量和边缘限制
    /// </summary>
    /// <param name="walldata"></param>
    private List<Vector2> GetWallDoorsUVs(WallData walldata, bool right, float rightMin, float rightMax, float leftMin, float leftMax, float totalDis)
    {
        tempDoorsUVList.Clear();
        List<ProductData> ProductDataList = data.GetWallProdust(walldata);
        if (ProductDataList.Count == 0)
        {
            return tempDoorsUVList;
        }

        float height = defaultSetting.DefaultHeight;
        List<WallData> farstWalls = roomfunc.GetFarstWalls(prefabs.mainCamera);
        if (halfWall == true && farstWalls.IndexOf(walldata) == -1)
        {
            height = halfWallHeight;
        }

        if (right)
        {
            ProductDataList.Sort(
                delegate(ProductData data1, ProductData data2)
                {
                    float dis1 = Vector2.Distance(walldata.point1.pos, GetVector2(data1.position));
                    float dis2 = Vector2.Distance(walldata.point1.pos, GetVector2(data2.position));
                    return dis1 <= dis2 ? 1 : -1;
                }
            );
            float rightTotalDis = totalDis - rightMin + rightMax;
            for (int i = 0; i < ProductDataList.Count; i++)
            {
                ProductData productData = ProductDataList[i];
                float scaleX = Mathf.Abs(productData.scale.x);
                float scaleY = Mathf.Abs(productData.scale.y);
                Vector2 productPos2D = GetVector2(productData.position);
                float dis = Vector2.Distance(walldata.point1.pos, productPos2D);
                float dis2 = Vector2.Distance(walldata.point2.pos, productPos2D);
                if (dis < dis2 && wallfunc.PointOnWall(productPos2D, walldata) == false)
                {
                    dis = -dis;
                }
                Product product = mainPageData.getProduct(productData.seekId);
                float sizeX = Mathf.Abs(product.size.x);
                float sizeY = Mathf.Abs(product.size.y);
                if (product == null)
                {
                    Debug.LogWarning("seekId = " + productData.seekId + "不存在");
                    continue;
                }
                if (productData.height + sizeY <= 0)
                {
                    continue;
                }
                //if (productData.height >= height)
                //{
                //    continue;
                //}
                //if (product.size.y >= height)
                //{
                //    continue;
                //}
                if (productData.height + sizeY >= height)
                {
                    continue;
                }

                uvHelpCenter.x = dis - rightMin;
                uvHelpCenter.y = productData.height;
                uvHelpExtents.x = scaleX * sizeX / 2;
                uvHelpExtents.y = scaleY * sizeY;

                if (productData.height != 0)
                {
                    uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -leftMax - rightMin + 0.1f);
                    uvHelpPos.y = 0;
                    tempDoorsUVList.Add(uvHelpPos);

                    uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -leftMax - rightMin + 0.1f);
                    uvHelpPos.y = productData.height;
                    tempDoorsUVList.Add(uvHelpPos);
                }

                uvHelpPos.x = Mathf.Min(uvHelpCenter.x + uvHelpExtents.x, rightTotalDis - 0.1f, rightTotalDis - (leftMin + rightMax) - 0.1f);
                uvHelpPos.y = uvHelpCenter.y;
                tempDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Min(uvHelpCenter.x + uvHelpExtents.x, rightTotalDis - 0.1f, rightTotalDis - (leftMin + rightMax) - 0.1f);
                uvHelpPos.y = uvHelpCenter.y + uvHelpExtents.y;
                tempDoorsUVList.Add(uvHelpPos);


                uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -leftMax - rightMin + 0.1f);
                uvHelpPos.y = uvHelpCenter.y + uvHelpExtents.y;
                tempDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -leftMax - rightMin + 0.1f);
                uvHelpPos.y = uvHelpCenter.y;
                tempDoorsUVList.Add(uvHelpPos);

                if (productData.height != 0)
                {
                    uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -leftMax - rightMin + 0.1f);
                    uvHelpPos.y = 0;
                    tempDoorsUVList.Add(uvHelpPos);
                }
            }
        }
        else
        {
            ProductDataList.Sort(
                delegate (ProductData data1, ProductData data2)
                {
                    float dis1 = Vector2.Distance(walldata.point2.pos, GetVector2(data1.position));
                    float dis2 = Vector2.Distance(walldata.point2.pos, GetVector2(data2.position));
                    return dis1 <= dis2 ? 1 : -1;
                }
            );
            float leftTotalDis = totalDis - leftMin + leftMax;
            for (int i = 0; i < ProductDataList.Count; i++)
            {
                ProductData productData = ProductDataList[i];
                float scaleX = Mathf.Abs(productData.scale.x);
                float scaleY = Mathf.Abs(productData.scale.y);
                //productData.height = 0.3f;
                //float dis = Vector2.Distance(walldata.point2.pos, GetVector2(productData.position));
                Vector2 productPos2D = GetVector2(productData.position);
                float dis = Vector2.Distance(walldata.point2.pos, productPos2D);
                float dis2 = Vector2.Distance(walldata.point1.pos, productPos2D);
                if (dis < dis2 && wallfunc.PointOnWall(productPos2D, walldata) == false)
                {
                    dis = -dis;
                }

                Product product = mainPageData.getProduct(productData.seekId);
                float sizeX = Mathf.Abs(product.size.x);
                float sizeY = Mathf.Abs(product.size.y);
                if (product == null)
                {
                    Debug.LogWarning("seekId = " + productData.seekId + "不存在");
                    continue;
                }
                if (productData.height + sizeY <= 0)
                {
                    continue;
                }
                if (productData.height >= height)
                {
                    continue;
                }
                if (sizeY >= height)
                {
                    continue;
                }
                if (productData.height + sizeY >= height)
                {
                    continue;
                }

                uvHelpCenter.x = dis - leftMin;
                uvHelpCenter.y = productData.height;
                uvHelpExtents.x = scaleX * sizeX / 2;
                uvHelpExtents.y = scaleY * sizeY;

                if (productData.height > 0)
                {
                    uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -rightMax - leftMin + 0.1f);
                    uvHelpPos.y = 0;
                    tempDoorsUVList.Add(uvHelpPos);

                    uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -rightMax - leftMin + 0.1f);
                    uvHelpPos.y = productData.height;
                    tempDoorsUVList.Add(uvHelpPos);
                }
                uvHelpPos.x = Mathf.Min(uvHelpCenter.x + uvHelpExtents.x, leftTotalDis - 0.1f, leftTotalDis - (rightMin + leftMax) - 0.1f);
                uvHelpPos.y = uvHelpCenter.y;
                tempDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Min(uvHelpCenter.x + uvHelpExtents.x, leftTotalDis - 0.1f, leftTotalDis - (rightMin + leftMax) - 0.1f);
                uvHelpPos.y = uvHelpCenter.y + uvHelpExtents.y;
                tempDoorsUVList.Add(uvHelpPos);


                uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -rightMax - leftMin + 0.1f);
                uvHelpPos.y = uvHelpCenter.y + uvHelpExtents.y;
                tempDoorsUVList.Add(uvHelpPos);

                uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -rightMax - leftMin + 0.1f);
                uvHelpPos.y = uvHelpCenter.y;
                tempDoorsUVList.Add(uvHelpPos);

                if (productData.height > 0)
                {
                    uvHelpPos.x = Mathf.Max(uvHelpCenter.x - uvHelpExtents.x, 0.1f, -rightMax - leftMin + 0.1f);
                    uvHelpPos.y = 0;
                    tempDoorsUVList.Add(uvHelpPos);
                }
            }
        }
        return tempDoorsUVList;
    }

    public Goods3DView getGoodsView(ProductData data)
    {
        for (int i = 0; i < goodsViewList.Count; i++)
        {
            if (goodsViewList[i].data == data)
            {
                return goodsViewList[i];
            }
        }
        return null;
    }

    protected override void RefreshGameObjs()
    {
        base.RefreshGameObjs();
        
        SetGameObjCountEquleList(data.wallList.Count, wallupLines, prefabs.GetNewInstance_walltop);

        SetGameObjCountEquleList(data.wallList.Count, wallsideLines, prefabs.GetNewInstance_WallSide, 2);
        SetGameObjCountEquleList(data.wallList.Count, wallsideTLines, prefabs.GetNewInstance_tBoard, 2);
        SetGameObjCountEquleList(data.wallList.Count, wallsideTopLines, prefabs.GetNewInstance_tBoard, 2);

        SetGameObjCountEquleList(data.wallList.Count, wallEndLines, prefabs.GetNewInstance_walltop, 4);

        SetGameObjCountEquleList(data.roomList.Count, floors, prefabs.GetNewInstance_Floor);

        SetGameObjCountEquleList(data.roomList.Count, ceilings, prefabs.GetNewInstance_Ceiling);

        SetGameObjCountEquleList(data.InWallProductCount(), DoorWindowSideLines, prefabs.GetNewInstance_DoorSide, 8);
        for (int i = 0; i < wallsideTLines.Count; i++)
        {
            wallsideTLines[i].SetActive(false);
        }
        for (int i = 0; i < wallsideTopLines.Count; i++)
        {
            wallsideTopLines[i].SetActive(false);
        }
        for (int i = 0; i < DoorWindowSideLines.Count; i++)
        {
            DoorWindowSideLines[i].SetActive(false);
        }

        for (int i = 0; i < data.productDataList.Count; i++)
        {
            ProductData productdata = data.productDataList[i];
            int id = productdata.id;
            if (products.ContainsKey(id) == false)
            {
                products.Add(id, new List<GameObject>());
            }
        }

        for (int i = 0; i < loaders.Count; i++)
        {
            Debug.LogWarning("Cancel loader " + loaders[i].httpUrl);
            loaders[i].Cancel();
        }
        loaders.Clear();
        removeListId.Clear();
        goodsViewList.Clear();
       
        foreach (int id in products.Keys)
        {
            List<int> hasIndexList = new List<int>();
            for (int i = 0; i < data.productDataList.Count; i++)
            {
                ProductData productData = data.productDataList[i];
                if (productData.id == id)
                {
                    hasIndexList.Add(i);
                    if (products.ContainsKey(id) && products[id].Count >= hasIndexList.Count)
                    {
                        GameObject go = products[id][hasIndexList.Count - 1];
                        Product product = mainPageData.getProduct(productData.seekId);
                        float y = productData.height;
                        switch (productData.type)
                        {
                            case 2://门窗 
                                {
                                    //if (product.entityType == "window")
                                    //{
                                    //    y = 1;
                                    //}
                                }
                                break;
                            case 3://吊顶 
                                {
                                    if (product == null) break;
                                    y = productData.height + defaultSetting.DefaultHeight - product.size.y;
                                }
                                break;
                            case 4://地毯 
                                {
                                    y = productData.height + -0.001f;
                                }
                                break;
                            case 5://天花板
                                {
                                    y = productData.height + defaultSetting.DefaultHeight - 0.001f;
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
                        productData.setData(go, Vector3.one, y);
                        Goods3DView goodsView = go.GetComponent<Goods3DView>();
                        goodsView.SetData(productData);
                        go.SetActive(!productData.hide);
                        goodsViewList.Add(goodsView);
                        if (halfWall == true && (productData.type == 3 || productData.type == 5))
                        {
                            //半高墙不隐藏顶部物品
                            //go.SetActive(false);
                        }
                    }
                }
            }
            int minCount = hasIndexList.Count < products[id].Count ? hasIndexList.Count : products[id].Count;
            for (int i = minCount; i < products[id].Count; i++)
            {
                //Debug.LogWarning("GameObject.DestroyImmediate " + id, true);
                GameObject.DestroyImmediate(products[id][i], true);
                products[id].RemoveAt(i);
                i--;
            }
            for (int i = minCount; i < hasIndexList.Count; i++)
            {
                int index = hasIndexList[i];
                ProductData product = data.productDataList[index];
                LoadMode(product);
            }
            if (products[id].Count == 0)
            {
                removeListId.Add(id);
            }
        }
        for (int i = 0; i < removeListId.Count; i++)
        {
            products.Remove(removeListId[i]);
        }

    }

    private void SetBaseboardMesh(TBaseboard baseboardLeft, GameObject wallTLine, Vector2 v2s1, Vector2 v2s2, Vector2 tp1, Vector2 tp2)
    {
        bool isT = baseboardLeft.isT;
        float defaultHeight = defaultSetting.DefaultHeight;
        Vector3 pos = GetVector3(v2s1.x, isT == false ? defaultHeight - baseboardLeft.disRoot : 0, v2s1.y);
        tempv3s.Clear();
        tempv3s.Add(GetVector3(v2s1.x, isT == false ? pos.y - baseboardLeft.height : pos.y + baseboardLeft.height, v2s1.y));
        tempv3s.Add(GetVector3(v2s2.x, isT == false ? pos.y - baseboardLeft.height : pos.y + baseboardLeft.height, v2s2.y));
        tempv3s.Add(GetVector3(tp1.x, isT == false ? pos.y - baseboardLeft.height : pos.y + baseboardLeft.height, tp1.y));
        tempv3s.Add(GetVector3(tp2.x, isT == false ? pos.y - baseboardLeft.height : pos.y + baseboardLeft.height, tp2.y));
        tempv3s.Add(GetVector3(tp1.x, pos.y, tp1.y));
        tempv3s.Add(GetVector3(tp2.x, pos.y, tp2.y));
        tempv3s.Add(GetVector3(v2s1.x, pos.y, v2s1.y));
        tempv3s.Add(GetVector3(v2s2.x, pos.y, v2s2.y));

        for (int i = 0; i < tempv3s.Count; i++)
        {
            tempv3s[i] = tempv3s[i] - pos;
        }

        List<int> triangles = tempTTriangles;
        if(isT == false) triangles = tempTopTriangles;
        for (int i = 0; i < tempUVs.Count; i++)
        {
            Vector2 v2 = tempUVs[i];
            if (i % 2 == 1)
            {
                v2.x = baseboardLeft.multi * Vector2.Distance(v2s1, v2s2);
                tempUVs[i] = v2;
            }
        }

        Mesh mesh = wallTLine.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(tempv3s);
        mesh.SetUVs(0, tempUVs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider meshColli = wallTLine.GetComponentInChildren<MeshCollider>(true);
        if (meshColli != null)
        {
            meshColli.convex = false;
            Mesh newMesh = new Mesh();
            newMesh.SetTriangles(EmtyArr, 0);
            newMesh.SetVertices(tempv3s);
            newMesh.SetUVs(0, tempUVs);
            newMesh.SetTriangles(triangles, 0);
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            meshColli.sharedMesh = newMesh;
        }

        wallTLine.transform.position = pos;
        wallTLine.GetComponent<Renderer>().material.mainTexture = prefabs.GetNewTBoardTexture(baseboardLeft.isT, baseboardLeft.index);
    }

    public override void sleep()
    {
        base.sleep();
    }
}
