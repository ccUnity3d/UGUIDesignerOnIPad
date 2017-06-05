using foundation;
using System.Collections.Generic;
using UnityEngine;
using System;

public class View2D : BaseView {

    private static View2D instance;
    public static View2D Instance
    {
        get
        {
            if (instance == null) instance = new View2D();
            return instance;
        }
    }
    
    public ObjData selectObjData = null;
    private GameObject selectPoint = null;
    private GameObject selectWall = null;
    private GameObject selectRoom = null;
    private GameObject selectGoods = null;

    public WallData selectGoodsdisedWalldata = null;
    private GameObject selectDisedwall = null;

    public List<ProductData> selectPackedProductdatas = new List<ProductData>();
    private List<GameObject> selectPackedproducts = new List<GameObject>();

    private InputStateMachine machine
    {
        get {
            return InputStateMachine.Instance;
        }
    }
    private List<GoodsView> goodsViewList = new List<GoodsView>();

    private List<GameObject> wallLines = new List<GameObject>();
    private List<GameObject> wallsideLines = new List<GameObject>();
    private List<GameObject> wallEndLines = new List<GameObject>();
    private List<GameObject> wallPoints= new List<GameObject>();

    private List<GameObject> showLenLines = new List<GameObject>();
    private List<GameObject> showLenPoints = new List<GameObject>();
    private List<GameObject> showWords = new List<GameObject>();

    private List<GameObject> floors = new List<GameObject>();
    private List<GameObject> areas = new List<GameObject>();

    private int[] EmtyArr = new int[] { };
    private Dictionary<int, List<GameObject>> products = new Dictionary<int, List<GameObject>>();
    private List<int> hasIndexList = new List<int>();
    private List<int> removeListId = new List<int>();

    private List<GameObject> inputWallLengths = new List<GameObject>();
    private List<Point> tempSideList = new List<Point>();

    public void ClearView()
    {
        selectObjData = null;
        ClearList(goodsViewList);

        ClearList(wallLines);
        ClearList(wallsideLines);
        ClearList(wallEndLines);
        ClearList(wallPoints);

        ClearList(showLenLines);
        ClearList(showLenPoints);
        ClearList(showWords);

        ClearList(floors);
        ClearList(areas);

        ClearList(inputWallLengths);

        foreach (List<GameObject> item in products.Values)
        {
            ClearList(item); 
        }
        products.Clear();

        ClearList(inputWallLengths);

    }
    private void ClearList(List<GoodsView> list)
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
        //Debug.Log("view2D RefreshView");
        RefreshGameObjs();
        base.RefreshView();

        #region room
        for (int i = 0; i < data.roomList.Count; i++)
        {
            GameObject obj = floors[i];
            RoomView view = obj.GetComponent<RoomView>();
            RoomData room = data.roomList[i];
            view.SetData(room);
            List<Vector2> v2s = roomfunc.SetMesh(obj, room);
            Vector2 v2 = v2s[0];
            obj.transform.localPosition = new Vector3(v2.x, v2.y, obj.transform.localPosition.z);
            for (int k = 0; k < v2s.Count; k++)
            {
                v2s[k] = v2s[k] - v2;
            }
            MaterialData floor = room.floor;
            List<List<Vector2>> list = linefunc.SetMesh(v2s, obj, floor.offsetX, floor.offsetY, floor.rotation, floor.tileSize_x, floor.tileSize_y, true);
            if (list != null) roomfunc.SetArea(areas[i], list, v2);
            if (selectObjData == room)
            {
                if (selectRoom == null) selectRoom = prefabs.GetNewInstance_selectRoom().gameObject;
                selectRoom.transform.localPosition = obj.transform.localPosition - Vector3.forward;
                linefunc.SetMesh(v2s, selectRoom, floor.offsetX, floor.offsetY, floor.rotation, floor.tileSize_x, floor.tileSize_y, true);
                selectRoom.SetActive(true);
            }
            Transform tran = areas[i].transform.FindChild("name");
            TextMesh textmesh = tran.GetComponent<TextMesh>();
            textmesh.text = room.type;
        }

        if ((selectObjData == null || selectObjData is RoomData == false) && selectRoom != null)
        {
            selectRoom.SetActive(false);
        }
        #endregion

        #region wall
        for (int i = 0; i < data.wallList.Count; i++)
        {
            WallData walldata = data.wallList[i];

            GameObject wallline = wallLines[i];
            GameObject wallLeft = wallsideLines[2 * i];
            GameObject wallRight = wallsideLines[2 * i + 1];
            GameObject wallLeftEnd = wallEndLines[2 * i];
            GameObject wallRightEnd = wallEndLines[2 * i + 1];
            if (walldata.hide == true)
            {
                wallline.SetActive(false);
                wallLeft.SetActive(false);
                wallRight.SetActive(false);
                wallLeftEnd.SetActive(false);
                wallRightEnd.SetActive(false);

                showLenLines[2 * i + 1].SetActive(false);
                showLenPoints[4 * i + 2].SetActive(false);
                showLenPoints[4 * i + 3].SetActive(false);
                showWords[2 * i + 1].SetActive(false);

                showLenLines[2 * i].SetActive(false);
                showLenPoints[4 * i].SetActive(false);
                showLenPoints[4 * i + 1].SetActive(false);
                showWords[2 * i].SetActive(false);
                continue;
            }
            else {
                wallline.SetActive(true);
                wallLeft.SetActive(true);
                wallRight.SetActive(true);
                //wallLeftEnd.SetActive(true);
                //wallRightEnd.SetActive(true);

                //showLenLines[2 * i + 1].SetActive(true);
                //showLenPoints[4 * i + 2].SetActive(true);
                //showLenPoints[4 * i + 3].SetActive(true);
                //showWords[2 * i + 1].SetActive(true);

                //showLenLines[2 * i].SetActive(true);
                //showLenPoints[4 * i].SetActive(true);
                //showLenPoints[4 * i + 1].SetActive(true);
                //showWords[2 * i].SetActive(true);
            }
            List<Vector2> tempList;
            SetObjWithData(walldata, wallline, wallLeft, wallRight, wallLeftEnd, wallRightEnd, out tempList);

            RoomData side1OnRoom = data.WallSideOnRoom(walldata.point1To2Data);
            RoomData side2OnRoom = data.WallSideOnRoom(walldata.point2To1Data);
            if (side2OnRoom != null)
            {
                ///左侧显示
                GameObject showline = showLenLines[2 * i + 1];
                GameObject showLeft = showLenPoints[4 * i + 2];
                GameObject showRight = showLenPoints[4 * i + 3];
                GameObject showWord = showWords[2 * i + 1];
                SetShowObjWithData(walldata, showline, showLeft, showRight, showWord, tempList[4], tempList[5], walldata.c4);
            }
            if(side1OnRoom != null || side2OnRoom == null)
            { 
                ///右侧显示
                GameObject showline = showLenLines[2 * i];
                GameObject showLeft = showLenPoints[4 * i];
                GameObject showRight = showLenPoints[4 * i + 1];
                GameObject showWord = showWords[2 * i];
                SetShowObjWithData(walldata, showline, showLeft, showRight, showWord, tempList[1], tempList[2], walldata.c3);
            }
        }

        if ((selectObjData == null || selectObjData is WallData == false) && selectWall != null)
        {
            selectWall.SetActive(false);
        }
        if ((selectObjData is ProductData == false || selectGoodsdisedWalldata == null) && selectDisedwall!=null)
        {
            selectDisedwall.SetActive(false);
        }
        #endregion

        #region point
        for (int i = 0; i < data.pointList.Count; i++)
        {
            Point p = data.pointList[i];
            Vector2 pos = p.pos;
            //Debug.LogWarning("show "+ pos);
            wallPoints[i].transform.localPosition = new Vector3(pos.x, pos.y, wallPoints[i].transform.localPosition.z);
            wallPoints[i].name = "Point " + p.guid;
            PointView view = wallPoints[i].GetComponent<PointView>();
            if (view == null) { Debug.LogWarning("wallPoints[i].GetComponent<PointView>() == null"); continue; }
            view.SetData(p);
            if (selectObjData == p)
            {
                if (selectPoint == null) selectPoint = prefabs.GetNewInstance_selectPoint().gameObject;
                selectPoint.transform.localPosition = wallPoints[i].transform.localPosition - Vector3.forward;
                selectPoint.SetActive(true);
            }
        }
        if ((selectObjData == null || selectObjData is Point == false) && selectPoint != null)
        {
            selectPoint.SetActive(false);
        }
        #endregion

        #region setWallLength
        for (int i = 0; i < inputWallLengths.Count; i++)
        {
            inputWallLengths[i].SetActive(false);
        }
        if (selectObjData is WallData)
        {
            WallData wall = selectObjData as WallData;
            int wallIndex = data.wallList.IndexOf(wall);
            RoomData side1OnRoom = data.WallSideOnRoom(wall.point1To2Data);
            RoomData side2OnRoom = data.WallSideOnRoom(wall.point2To1Data);
            if (side1OnRoom != null)
            {
                tempSideList = new List<Point>();

                GameObject goF = inputWallLengths[3];
                SetLengthHandleView viewF = goF.GetComponent<SetLengthHandleView>();
                WallSideData indexF1 = roomfunc.GetNearCrossSide(side1OnRoom, wall.point1To2Data, true, tempSideList);
                viewF.SetData(indexF1);
                goF.SetActive(true);

                GameObject goT = inputWallLengths[4];
                SetLengthHandleView viewT = goT.GetComponent<SetLengthHandleView>();
                WallSideData indexT1 = roomfunc.GetNearCrossSide(side1OnRoom, wall.point1To2Data, false, tempSideList);
                viewT.SetData(indexT1);
                goT.SetActive(true);

                viewF.otherdata = indexT1;
                viewT.otherdata = indexF1;
                viewF.ForT = true;
                viewT.ForT = false;
                viewF.isParallelPointList = tempSideList;
                viewT.isParallelPointList = tempSideList;
            }
            if (side2OnRoom != null)
            {
                tempSideList = new List<Point>();

                GameObject goF = inputWallLengths[1];
                SetLengthHandleView viewF = goF.GetComponent<SetLengthHandleView>();
                WallSideData indexF2 = roomfunc.GetNearCrossSide(side2OnRoom, wall.point2To1Data, true, tempSideList);
                viewF.SetData(indexF2);
                goF.SetActive(true);

                GameObject goT = inputWallLengths[2];
                SetLengthHandleView viewT = goT.GetComponent<SetLengthHandleView>();
                WallSideData indexT2 = roomfunc.GetNearCrossSide(side2OnRoom, wall.point2To1Data, false, tempSideList);
                viewT.SetData(indexT2);
                goT.SetActive(true);

                viewF.otherdata = indexT2;
                viewT.otherdata = indexF2;
                viewF.ForT = true;
                viewT.ForT = false;
                viewF.isParallelPointList = tempSideList;
                viewT.isParallelPointList = tempSideList;
            }

            GameObject go = inputWallLengths[0];
            if (side1OnRoom == null && side2OnRoom == null)
            {
                WallSideData side = wall.point1To2Data;
                SetLengthHandleView view = go.GetComponent<SetLengthHandleView>();
                view.SetData(side);
                view.otherdata = null;
                go.SetActive(true);
            }
            else
            {
                go.SetActive(false);
            }
        }
        #endregion

    }

    private void LoadMode(ProductData productData)
    {
        GoodsVO vo = mainPageData.getGoods(productData.seekId);
        if (vo == null)
        {
            Debug.LogWarning("proxy.getGood(id) == null seekId = " + productData.seekId);
            return;
        }
    
        Vector3 size = mainPageData.getProduct(vo.seekId).size;
        Product product = mainPageData.getProduct(vo.seekId);
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
                    y = defaultSetting.DefaultHeight + productData.height - product.size.y;
                }
                break;
            case 4://地毯 
                {
                    y = -0.001f + productData.height;
                }
                break;
            case 5://天花板
                {
                    y = defaultSetting.DefaultHeight + productData.height - 0.001f;
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

        GameObject instanceObj;
        Vector3 pos;
        GoodsView goodsView;
        if (productData.type == 2)
        {
            instanceObj = prefabs.GetNewInstance_doorwhiteground().gameObject;
            //float width = size.z;
            instanceObj.transform.localScale = Vector2.right * size.x + Vector2.up * size.z;
            instanceObj.layer = LayerMask.NameToLayer("UI");
            AddProductGameObj(productData.id, instanceObj);

            productData.setData(instanceObj, size, y, true);
            pos = instanceObj.transform.localPosition;
            pos.z = -5;
            instanceObj.transform.localPosition = pos;
            goodsView = instanceObj.GetComponent<GoodsView>();
            goodsView.SetData(productData);
            if (productData.hide == true)
            {
                instanceObj.SetActive(false);
            }
            else
            {
                instanceObj.SetActive(true);
            }
            goodsViewList.Add(goodsView);
            return;
        }
        instanceObj = prefabs.GetNewQuad();
        //float width = size.z;
        instanceObj.transform.localScale = Vector2.right * size.x + Vector2.up * size.z;
        instanceObj.layer = LayerMask.NameToLayer("UI");
        AddProductGameObj(productData.id, instanceObj);

        productData.setData(instanceObj, size, y, true);
        pos = instanceObj.transform.localPosition;
        pos.z = -5;
        instanceObj.transform.localPosition = pos;
        goodsView = instanceObj.GetComponent<GoodsView>();
        goodsView.SetData(productData);
        if (productData.hide == true)
        {
            instanceObj.SetActive(false);
        }
        else
        {
            instanceObj.SetActive(true);
        }
        goodsViewList.Add(goodsView);
        SetTextureTool.SetTexture(instanceObj, vo.uri2D, vo.seekId);
        //AssetBoundleLoader loader = new AssetBoundleLoader(vo.modelUri, LoadDataType.assetBoundle, onLoaded, new object[2] { vo, product });
        //loader.Load();
    }

    //private void onLoaded(object obj)
    //{
    //    SimpleInnerLoader loader = obj as SimpleInnerLoader;
    //    object[] objs = loader.bringData as object[];
    //    GoodsVO vo = objs[0] as GoodsVO;
    //    ProductData product = objs[1] as ProductData;
    //    UnityEngine.Object Obj = loader.loadedData as UnityEngine.Object;
    //    if (Obj == null)
    //    {
    //        Debug.LogWarning(loader.uri);
    //        return;
    //    }

    //    Vector3 size = mainPageData.getProduct(machine.selectVO.seekId).size;
    //    GameObject instanceObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //    float width = size.z;
    //    instanceObj.transform.localScale = Vector2.right * size.x + Vector2.up * width;
    //    instanceObj.layer = LayerMask.NameToLayer("UI");
    //    AddGameObj(product.id, instanceObj);
    //    product.setData(instanceObj, true);
    //    Vector3 pos = instanceObj.transform.localPosition;
    //    pos.z = -1;
    //    instanceObj.transform.localPosition = pos;
    //    instanceObj.transform.parent = prefabs.parentTran_2D;
    //}

    //private void LoadSuccess(EventX e)
    //{
    //    AssetResource resource = e.target as AssetResource;
    //    AssetsManager.bindEventHandle(resource, LoadSuccess, false);
    //    object[] objs = resource.userData as object[];
    //    GoodsVO vo = objs[0] as GoodsVO;
    //    ProductData product = objs[1] as ProductData;
    //    GameObject Obj = resource.data as GameObject;
    //    if (Obj == null)
    //    {
    //        Debug.LogWarning(resource.url);
    //        return;
    //    }

    //    Vector3 size = mainPageData.getProduct(machine.selectVO.seekId).size;
    //    GameObject instanceObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //    float width = size.z;
    //    instanceObj.transform.localScale = Vector2.right * size.x + Vector2.up * width;
    //    instanceObj.layer = LayerMask.NameToLayer("UI");
    //    AddGameObj(product.id, instanceObj);
    //    product.setData(instanceObj, true);
    //    Vector3 pos = instanceObj.transform.localPosition;
    //    pos.z = -1;
    //    instanceObj.transform.localPosition = pos;
    //    instanceObj.transform.parent = prefabs.parentTran_2D;
    //}

    public void AddProductGameObj(int id, GameObject go)
    {
        if (products.ContainsKey(id) == false) products.Add(id, new List<GameObject>());
        products[id].Add(go);
    }

    public GoodsView getGoodsView(ProductData data)
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

        SetGameObjCountEquleList(data.wallList.Count, wallLines, prefabs.GetNewInstance_grey);

        SetGameObjCountEquleList(data.wallList.Count, wallsideLines, prefabs.GetNewInstance_black, 2);

        SetGameObjCountEquleList(data.wallList.Count, wallEndLines, prefabs.GetNewInstance_black, 2);

        SetGameObjCountEquleList(data.pointList.Count, wallPoints, prefabs.GetNewInstance_point);

        SetGameObjCountEquleList(data.wallList.Count, showLenLines, prefabs.GetNewInstance_line, 2);

        SetGameObjCountEquleList(data.wallList.Count, showLenPoints, prefabs.GetNewInstance_fork, 4);

        SetGameObjCountEquleList(data.wallList.Count, showWords, prefabs.GetNewInstance_text, 2);

        SetGameObjCountEquleList(data.roomList.Count, floors, prefabs.GetNewInstance_floor);

        SetGameObjCountEquleList(data.roomList.Count, areas, prefabs.GetNewInstance_area);

        SetGameObjCountEquleList(selectPackedProductdatas.Count, selectPackedproducts, prefabs.GetNewInstance_selectGoods);

        for (int i = 0; i < showLenLines.Count; i++)
        {
            showLenLines[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < showLenPoints.Count; i++)
        {
            showLenPoints[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < showWords.Count; i++)
        {
            showWords[i].gameObject.SetActive(false);
        }



        for (int i = 0; i < data.productDataList.Count; i++)
        {
            int id = data.productDataList[i].id;
            if (products.ContainsKey(id) == false)
            {
                products.Add(id, new List<GameObject>());
            }
        }
        goodsViewList.Clear();
        removeListId.Clear();
        foreach (int id in products.Keys)
        {
            hasIndexList.Clear();
            for (int i = 0; i < data.productDataList.Count; i++)
            {
                ProductData productData = data.productDataList[i];
                if (productData.id == id)
                {
                    hasIndexList.Add(i);
                    if (products.ContainsKey(id) && products[id].Count >= hasIndexList.Count)
                    {
                        GameObject go = products[id][hasIndexList.Count - 1];
                        Vector3 size = mainPageData.getProduct(productData.seekId).size;
                        GoodsVO vo = mainPageData.getGoods(productData.seekId);
                        ResourcesPool.DisposTexture(go);
                        SetTextureTool.SetTexture(go, vo.uri2D, vo.seekId);
                        Product product = mainPageData.getProduct(vo.seekId);
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
                                    y = defaultSetting.DefaultHeight + productData.height - product.size.y;
                                }
                                break;
                            case 4://地毯 
                                {
                                    y = productData.height + -0.001f;
                                }
                                break;
                            case 5://天花板
                                {
                                    y = defaultSetting.DefaultHeight + productData.height - 0.001f;
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
                        productData.setData(go, size, y, true);
                        GoodsView goodsView = go.GetComponent<GoodsView>();
                        goodsView.SetData(productData);
                        if (selectObjData == productData)
                        {
                            if (selectGoods == null) selectGoods = prefabs.GetNewInstance_selectGoods().gameObject;
                            selectGoods.transform.localRotation = go.transform.localRotation;
                            selectGoods.transform.localScale = go.transform.localScale;
                            selectGoods.transform.localPosition = go.transform.localPosition - Vector3.forward;
                            selectGoods.SetActive(true);
                            if(productData.hide == true) selectGoods.SetActive(false);
                        }
                        for (int k = 0; k < selectPackedProductdatas.Count; k++)
                        {
                            if (selectPackedProductdatas[k] == productData)
                            {
                                GameObject obj = selectPackedproducts[k];
                                //if (obj == null) obj = prefabs.GetNewInstance_selectGoods().gameObject;
                                obj.transform.localRotation = go.transform.localRotation;
                                obj.transform.localScale = go.transform.localScale;
                                obj.transform.localPosition = go.transform.localPosition - Vector3.forward;
                                obj.SetActive(true);
                                if (productData.hide == true) obj.SetActive(false);
                                break;
                            }
                        }

                        if (productData.hide == true)
                        {
                            go.SetActive(false);
                        }
                        else
                        {
                            go.SetActive(true);
                        }
                        goodsViewList.Add(goodsView);
                    }
                }
            }
            int minCount = hasIndexList.Count < products[id].Count ? hasIndexList.Count : products[id].Count;
            for (int i = minCount; i < products[id].Count; i++)
            {
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

        if ((selectObjData == null || selectObjData is ProductData == false) && selectGoods != null)
        {
            selectGoods.SetActive(false);
        }

        for (int i = 0; i < removeListId.Count; i++)
        {
            products.Remove(removeListId[i]);
        }

        if (inputWallLengths.Count == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject go = prefabs.GetNewInstance_inputWallLength().gameObject;
                go.AddComponent<SetLengthHandleView>();
                go.SetActive(false);
                inputWallLengths.Add(go);
            }            
        }
    }

    public void SetObjWithData(WallData walldata, GameObject wallbody, GameObject wallLeft, GameObject wallRight, GameObject wallEnd1, GameObject wallEnd2, out List<Vector2> v2s)
    {
        //walldata边线形成的 多边形 的 顶点集合
        v2s = wallfunc.GetWallVerticesPos(walldata);
        linefunc.SetLine(v2s[1], v2s[2], wallRight, prefabs.size);
        linefunc.SetLine(v2s[4], v2s[5], wallLeft, prefabs.size);

        if (data.GetNearestPoints(walldata.point1).Count == 1)
        {
            if (wallEnd1.activeSelf == false) wallEnd1.SetActive(true);
            linefunc.SetLine(v2s[1], v2s[5], wallEnd1, prefabs.size);
        }
        else
        {
            wallEnd1.SetActive(false);
        }
        if (data.GetNearestPoints(walldata.point2).Count == 1)
        {
            if (wallEnd2.activeSelf == false) wallEnd2.SetActive(true);
            linefunc.SetLine(v2s[2], v2s[4], wallEnd2, prefabs.size);
        }
        else
        {
            wallEnd2.SetActive(false);
        }

        Vector3 pos = v2s[0];
        float angle = linefunc.GetAngle(v2s[3] - v2s[0]);
        wallbody.transform.localPosition = new Vector3(pos.x, pos.y, wallbody.transform.localPosition.z);
        wallbody.transform.localRotation = Quaternion.Euler(Vector3.forward * (angle - 90));

        WallView view = wallbody.GetComponent<WallView>();
        if (view == null) { Debug.LogWarning("wallbody.GetComponent<PointView>() == null"); }
        view.SetData(walldata);

        List<Vector2> newV2s = wallfunc.GetWallVerticesLocalPos(v2s, walldata.width, prefabs.size);
        linefunc.SetMesh(newV2s, wallbody);

        if (selectObjData == walldata)
        {
            if (selectWall == null) selectWall = prefabs.GetNewInstance_selectWall().gameObject;
            linefunc.SetMesh(newV2s, selectWall);
            selectWall.transform.localPosition = wallbody.transform.localPosition - Vector3.forward;
            selectWall.transform.localRotation = wallbody.transform.localRotation;
            selectWall.SetActive(true);
        }
        if (selectObjData is ProductData && selectGoodsdisedWalldata == walldata)
        {
            if (selectDisedwall == null) selectDisedwall = prefabs.GetNewInstance_selectWall().gameObject;
            linefunc.SetMesh(newV2s, selectDisedwall);
            selectDisedwall.transform.localPosition = wallbody.transform.localPosition - Vector3.forward;
            selectDisedwall.transform.localRotation = wallbody.transform.localRotation;
            selectDisedwall.SetActive(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="c"> 左/右 边线方程参数c </param>
    private void SetShowObjWithData(WallData walldata, GameObject showLine, GameObject showStart, GameObject showEnd, GameObject words, Vector2 v21, Vector2 v22, float c)
    {
        Vector3 v2start = linefunc.GetDisPoint(walldata.a, walldata.b, c, v21.x, v21.y);
        Vector3 v2end= linefunc.GetDisPoint(walldata.a, walldata.b, c, v22.x, v22.y);
        float angle = linefunc.GetAngle(v2end- v2start);
        v2start.z = showStart.transform.localPosition.z;
        v2end.z = showEnd.transform.localPosition.z;
        showStart.transform.localPosition = v2start;
        showEnd.transform.localPosition = v2end;
        showStart.transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
        showEnd.transform.localRotation = Quaternion.Euler(Vector3.forward * angle);
        linefunc.SetLine(v2start, v2end, showLine, prefabs.size);
        Vector3 v3 = (v2start + v2end) / 2;
        v3.z = -3;
        words.transform.localPosition = v3;
        float dis = Vector2.Distance(v2start, v2end) * 1000;
        words.GetComponentInChildren<TextMesh>().text = "" + (Mathf.RoundToInt(dis) * ((int)defaultSetting.DefaultUnit / 1000f));

        showLine.SetActive(true);
        showStart.SetActive(true);
        showEnd.SetActive(true);
        words.SetActive(true);
    }

    public override void sleep()
    {
        base.sleep();
    }

}
