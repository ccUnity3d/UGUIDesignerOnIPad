using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OriginalInputData {
    
    public static OriginalInputData Instance
    {
        get {
            return UndoHelper.Instance.currentData.data;
        }
    }

    /// <summary>
    /// 设计师户型设计输入的所有点
    /// </summary>
    public List<Point> pointList = new List<Point>();
    /// <summary>
    /// 输入点产生的所有墙体的参数列表
    /// </summary>
    public List<WallData> wallList = new List<WallData>();
    public List<ProductData> productDataList = new List<ProductData>();
    public List<RoomData> roomList = new List<RoomData>();

    public List<Product> productList;

    public DefaultSettings defaultsettings = new DefaultSettings();

    /// <summary>
    /// 半高墙专用
    /// </summary>
    public List<RoomData> maxAngleRooms = new List<RoomData>();

    public List<ItemData> preScanDatas = new List<ItemData>();

    //public static explicit operator OriginalInputData(string json)
    //{
    //    OriginalInputData data = new OriginalInputData();
    //    object obj = MyJsonTool.FromJson(json);
    //    data.DeSerialize(obj as Dictionary<string, object>);
    //    return data;
    //}

    private bool _isOld = false;
    public bool isOld
    {
        get { return _isOld; }
        set { _isOld = value; }
    }

    public void DeSerialize(Dictionary<string, object> dic)
    {
        pointList.Clear();
        wallList.Clear();
        productDataList.Clear();
        roomList.Clear();
        productList = new List<Product>();
        preScanDatas.Clear();

        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "pointList":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            Point point = new Point();
                            point.DeSerialize(list[i] as Dictionary<string, object>);
                            pointList.Add(point);
                        }
                    }
                    break;
                case "wallList":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            WallData wall = new WallData();
                            wall.DeSerialize(list[i] as Dictionary<string, object>);
                            wallList.Add(wall);
                        }
                    }
                    break;
                case "productDataList":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            ProductData productdata = new ProductData();
                            productdata.DeSerialize(list[i] as Dictionary<string, object>);
                            productDataList.Add(productdata);
                        }
                    }
                    break;
                case "roomList":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            RoomData room = new RoomData();
                            room.DeSerialize(list[i] as Dictionary<string, object>);
                            roomList.Add(room);
                        }
                    }
                    break;
                case "productList":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        if (obj == null)
                        {
                            productList = null;
                            break; 
                        }
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            Product product = new Product();
                            if (isOld)
                            {
                                JsonProduct jsonData = new JsonProduct();
                                jsonData.DeSerialize(list[i] as Dictionary<string, object>);
                                jsonData.WriteToData(product);
                            }
                            else {
                                product.DeSerialize(list[i] as Dictionary<string, object>);
                            }
                            productList.Add(product);
                            //if(product)
                        }
                    }
                    break;
                case "defaultsettings":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        defaultsettings.DeSerialize(obj as Dictionary<string, object>);
                    }
                    break;
                case "maxAngleRooms":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            RoomData room = new RoomData();
                            room.DeSerialize(list[i] as Dictionary<string, object>);
                            maxAngleRooms.Add(room);
                        }
                    }                    
                    break;
                case "preScanDatas":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            PreScanData prescan = new PreScanData();
                            prescan.DeSerialize(list[i] as Dictionary<string, object>);
                            preScanDatas.Add(prescan);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        
        //恢复关联
        Dictionary<int, Point> pointDic = new Dictionary<int, Point>();
        Dictionary<int, WallData> wallDic = new Dictionary<int, WallData>();
        Dictionary<int, WallSideData> tempSideDic = new Dictionary<int, WallSideData>();
        for (int i = 0; i < pointList.Count; i++)
        {
            pointDic.Add(pointList[i].guid, pointList[i]);
        }
        for (int i = 0; i < wallList.Count; i++)
        {
            wallDic.Add(wallList[i].guid, wallList[i]);
            WallData wall = wallList[i];
            if (pointDic.ContainsKey(wall.point1Guid) == false)
            {
                Debug.LogError("wall.point1Guid = "+ wall.point1Guid + "对应的Point不存在");
            }
            else {
                wall.point1 = pointDic[wall.point1Guid];
            }
            if (pointDic.ContainsKey(wall.point2Guid) == false)
            {
                Debug.LogError("wall.point2Guid = " + wall.point2Guid + "对应的Point不存在");
            }
            else {
                wall.point2 = pointDic[wall.point2Guid];
            }
            wall.point1To2Data.targetWall = wall;
            wall.point2To1Data.targetWall = wall;
            tempSideDic.Add(wall.point1To2Data.guid, wall.point1To2Data);
            tempSideDic.Add(wall.point2To1Data.guid, wall.point2To1Data);
        }
        for (int i = 0; i < productDataList.Count; i++)
        {
            ProductData productData = productDataList[i];
            if (productData.targetWallGuid != -1)
            {
                if (wallDic.ContainsKey(productData.targetWallGuid))
                {
                    productData.targetWall = wallDic[productData.targetWallGuid];
                }
                else
                {
                    Debug.LogError("productData.targetWallGuid = "+ productData.targetWallGuid + "对应的Wall不存在");
                    productData.targetWall = null;
                }
            }
            else {
                productData.targetWall = null;
            }
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomData room = roomList[i];
            for (int k = 0; k < room.sideListGuids.Count; k++)
            {
                if (tempSideDic.ContainsKey(room.sideListGuids[k]) == false)
                {
                    Debug.LogError("room.sideListGuids[k] = " + room.sideListGuids[k] + "对应的WallSideData不存在");
                    continue;
                }
                room.sideList.Add(tempSideDic[room.sideListGuids[k]]);
            }
            room.pointList = new List<Point>();
            for (int k = 0; k < room.pointListGuids.Count; k++)
            {
                if (pointDic.ContainsKey(room.pointListGuids[k]) == false)
                {
                    Debug.LogError("room.pointListGuids[k] = " + room.pointListGuids[k] + "对应的Point不存在");
                    continue;
                }
                room.pointList.Add(pointDic[room.pointListGuids[k]]);
            }
        }

        for (int i = 0; i < maxAngleRooms.Count; i++)
        {
            RoomData room = maxAngleRooms[i];
            for (int k = 0; k < room.sideListGuids.Count; k++)
            {
                if (tempSideDic.ContainsKey(room.sideListGuids[k]) == false)
                {
                    Debug.LogError("room.sideListGuids[k] = " + room.sideListGuids[k] + "对应的WallSideData不存在");
                    continue;
                }
                room.sideList.Add(tempSideDic[room.sideListGuids[k]]);
            }
            room.pointList = new List<Point>();
            for (int k = 0; k < room.pointListGuids.Count; k++)
            {
                if (pointDic.ContainsKey(room.pointListGuids[k]) == false)
                {
                    Debug.LogError("room.pointListGuids[k] = " + room.pointListGuids[k] + "对应的Point不存在");
                    continue;
                }
                room.pointList.Add(pointDic[room.pointListGuids[k]]);
            }
        }

        MainPageUIData.Instance.productDic.Clear();
        MainPageUIData.Instance.GoodsDic.Clear();

        if (productList != null)
        {
            for (int i = 0; i < productList.Count; i++)
            {
                MainPageUIData.Instance.AddProduct(productList[i]);
            }
        }
    }

    ///// <summary>
    ///// UI材质球列表
    ///// </summary>
    //public List<MaterialItemData> uimaterialList = new List<MaterialItemData>() { };

    public void BeforetSerializeFieldDo()
    {
        ResetId();
        productList = new List<Product>();
        Dictionary<string, Product> dic = MainPageUIData.Instance.productDic;
        foreach (Product item in dic.Values)
        {
            productList.Add(item);
        }
        for (int i = 0; i < wallList.Count; i++)
        {
            wallList[i].BeforetSerializeFieldDo();
        }
        for (int i = 0; i < productDataList.Count; i++)
        {
            productDataList[i].BeforetSerializeFieldDo();
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].BeforetSerializeFieldDo();
        }
        for (int i = 0; i < maxAngleRooms.Count; i++)
        {
            maxAngleRooms[i].BeforetSerializeFieldDo();
        }
    }


    //辅助计算的
    private Dictionary<Point, List<Point>> neareatListDic = new Dictionary<Point, List<Point>>();
    private List<WallData> combineHelp = new List<WallData>();
    private Dictionary<WallData, List<ProductData>> productListDic = new Dictionary<WallData, List<ProductData>>();

    public void AddWall(WallData wall, bool whithoutChange = false)
    {
        //Debug.LogWarning("addWall " + wall.point1To2Data.id + " " + wall.point2To1Data.id);
        if (whithoutChange == false)
        {
            wall.point1To2Data.targetWall = wall;
            wall.point2To1Data.targetWall = wall;
            wall.point1To2Data.materialData = new MaterialData(-1, "3D/texture2D/wall3d.assetbundle");
            wall.point2To1Data.materialData = new MaterialData(-1, "3D/texture2D/wall3d.assetbundle");
            //Debug.LogWarning("AddWall " + wall.point1.guid+ " " + wall.point2.guid);
        }
        wallList.Add(wall);
    }

    public void AddPoint(Point point)
    {
        pointList.Add(point);
    }

    public WallData AddWall(Point point1, Point point2, float height, float width)
    {
        if (point1 == point2)
        {
            return null;
        }
        WallData wall = GetWall(point1, point2);
        if (wall != null)
        {
            return wall;
        }
        wall = new WallData();
        wall.point1 = point1;
        wall.point2 = point2;
        wall.height = height;
        wall.width = width;
        AddWall(wall);
        return wall;
    }

    public void ResetId()
    {
        int startId = 0;
        for (int i = 0; i < pointList.Count; i++)
        {
            pointList[i].guid = startId;
            startId++;
        }
        for (int i = 0; i < wallList.Count; i++)
        {
            wallList[i].guid = startId;
            startId++;

            wallList[i].point1To2Data.guid = startId;
            startId++;

            wallList[i].point2To1Data.guid = startId;
            startId++;
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].guid = startId;
            startId++;
        }

        for (int i = 0; i < productDataList.Count; i++)
        {
            productDataList[i].guid = startId;
            startId++;
        }

    }

    public List<ProductData> GetWallProdust(WallData walldata)
    {
        if (productListDic.ContainsKey(walldata) == false)
        {
            productListDic.Add(walldata, new List<ProductData>());
        }
        List<ProductData> list = productListDic[walldata];
        list.Clear();
        for (int i = 0; i < productDataList.Count; i++)
        {
            ProductData productData = productDataList[i];
            if (productData.type != 2)
            {
                continue;
            }
            if (productData.targetWall != walldata)
            {
                continue;
            }
            list.Add(productData);
        }
        return list;
    }

    public RoomData GetRoom(WallSideData side)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomData room = roomList[i];
            for (int k = 0; k < room.sideList.Count; k++)
            {
                if (side == room.sideList[k])
                {
                    return room;
                }
            }
        }
        return null;
    }

    public RoomData GetRoom(Point point1, Point point2)
    {
        WallData wall = GetWall(point1, point2);
        if (wall == null) return null;
        bool right = point1 == wall.point1;
        WallSideData side = right == true ? wall.point1To2Data : wall.point2To1Data;
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomData room = roomList[i];
            for (int k = 0; k < room.sideList.Count; k++)
            {
                if (side == room.sideList[k])
                {
                    return room;
                }
            }
        }
        return null;
    }

    public ProductData AddProduct(int goodsId, Vector3 world, float rotate, Product product, WallData targetWall, int type)
    {
        ProductData productdata = new ProductData(goodsId, world, rotate, product.seekId, product.defaultHeight, type);
        productdata.targetWall = targetWall;
        productDataList.Add(productdata);
        return productdata;
    }

    public WallData GetWall(Point target, Point item)
    {
        for (int i = 0; i < wallList.Count; i++)
        {
            if (wallList[i].equle(target, item))
            {
                return wallList[i];
            }
        }
        return null;
    }

    public WallData GetWall(int guid)
    {
        for (int i = 0; i < wallList.Count; i++)
        {
            if (wallList[i].guid == guid)
            {
                return wallList[i];
            }
        }
        return null;
    }

    public void RemoveWall(WallData wall)
    {
        if (wallList.Contains(wall) == false)
        {
            Debug.Log("wallList.Contains(wall) == false");
        }
        wallList.Remove(wall);
        if (pointList.Contains(wall.point1) == true)
        {
            List<Point> list = GetNearestPoints(wall.point1);
            if (list.Count == 0)
            {
                pointList.Remove(wall.point1);
            }
        }
        if (pointList.Contains(wall.point2) == true)
        {
            List<Point> list = GetNearestPoints(wall.point2);
            if (list.Count == 0)
            {
                pointList.Remove(wall.point2);
            }
        }
    }

    public void RemovePoint(Point target)
    {
        List<Point> list = GetNearestPoints(target);
        for (int i = 0; i < list.Count; i++)
        {
            Point item = list[i];
            WallData wall = GetWall(target, item);
            if (wall == null)
            {
                Debug.Log("GetWall(target, item) == null");
            }
            wallList.Remove(wall);

            List<Point> itemNearList = GetNearestPoints(item);
            if (itemNearList.Count == 0)
            {
                pointList.Remove(item);
            }            
        }
        pointList.Remove(target);
    }


    public void RemoveProduct(ProductData target)
    {
        productDataList.Remove(target);
        //
    }


    /// <summary>
    /// 合并点
    /// </summary>
    public void CombinePoint(Point delet, Point use, bool deletIs1 = true)
    {
        pointList.Remove(delet);
        List<Point> list = GetNearestPoints(delet);
        List<Point> useList = GetNearestPoints(use);
        for (int i = 0; i < list.Count; i++)
        {
            Point item = list[i];
            WallData wall = GetWall(delet, item);
            if (wall == null)
            {
                Debug.Log("GetWall(target, item)==null");
            }
            wallList.Remove(wall);
            if (use == item)
            {
                continue;
            }
            if (useList.Contains(item) == false)
            {
                if (deletIs1)
                {
                    AddWall(use, item, wall.height, wall.width);
                }
                else {
                    AddWall(item, use, wall.height, wall.width);
                }
            }
        }

        if (GetNearestPoints(use).Count == 0)
        {
            pointList.Remove(use);
        }
    }

    public List<WallData> CombinePointWall(Point point, WallData nearWall)
    {
        List<ProductData> productdatas = GetWallProdust(nearWall);

        combineHelp.Clear();
        WallData wall1 = GetWall(nearWall.point1, point);
        if (wall1 == null)
        {
            wall1 = AddWall(nearWall.point1, point, nearWall.height, nearWall.width);
        }
        combineHelp.Add(wall1);
        WallData wall2= GetWall(point, nearWall.point2);
        if (wall2 == null)
        {
            wall2 = AddWall(point, nearWall.point2, nearWall.height, nearWall.width);
        }
        combineHelp.Add(wall2);
        RemoveWall(nearWall);
        for (int i = 0; i < productdatas.Count; i++)
        {
            ProductData productdata = productdatas[i];
            Vector2 pos = GetVector2(productdata.position);
            if (PointOnWall(pos, wall1))
            {
                productdata.targetWall = wall1;
            }
            else
            {
                productdata.targetWall = wall2;
            }
        }

        return combineHelp;
    }
    //private bool PointOnWall(Point p, WallData wall)
    //{
    //    return PointOnWall(p.pos, wall);
    //}
    private bool PointOnWall(Vector2 v2, WallData wall)
    {
        float wallLen = Vector2.Distance(wall.point1.pos, wall.point2.pos);
        float len1 = Vector2.Distance(wall.point1.pos, v2);
        float len2 = Vector2.Distance(wall.point2.pos, v2);
        if (len1 + len2 - wallLen > 0.01f)
        {
            return false;
        }
        return true;
    }

    private Vector2 GetVector2(Vector3 position)
    {
        return Vector2.right * position.x + Vector2.up * position.z;
    }

    public List<Point> GetNearestPoints(Point target)
    {
        List<Point> list = getList(target);
        for (int i = 0; i < wallList.Count; i++)
        {
            if (wallList[i].point1 == target)
            {
                list.Add(wallList[i].point2);
            }
            if (wallList[i].point2 == target)
            {
                list.Add(wallList[i].point1);
            }
        }
        return list;
    }

    private List<Point> getList(Point target)
    {
        if(neareatListDic.ContainsKey(target) == false)
        {
            neareatListDic.Add(target, new List<Point>());
        }
        neareatListDic[target].Clear();
        return neareatListDic[target];
    }
    
    public void AfterSetToData()
    {

    }

    public int InWallProductCount()
    {
        int n = 0;
        for (int i = 0; i < productDataList.Count; i++)
        {
            if (productDataList[i].type == 2)
            {
                n++;
            }
        }
        return n;
    }

    public RoomData PointOnRoom(Point point)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].pointList.Contains(point)){
                return roomList[i];
            }
        }
        return null;
    }

    public RoomData WallSideOnRoom(WallSideData side)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].sideList.Contains(side))
            {
                return roomList[i];
            }
        }
        return null;
    }


    public bool Contins(Point target)
    {
        if (pointList.Contains(target) == true)
        {
            return true;
        }
        return false;
    }


    public bool Contins(WallData target)
    {
        if (wallList.Contains(target) == true)
        {
            return true;
        }
        return false;
    }


    public bool Contins(RoomData target)
    {
        if (roomList.Contains(target) == true)
        {
            return true;
        }
        return false;
    }

}
