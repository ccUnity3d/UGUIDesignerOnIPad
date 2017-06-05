using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UndoHelper : Singleton<UndoHelper>
{
    private InputStateMachine machine
    {
        get {
            return InputStateMachine.Instance;
        } 
    }

    private CacheLocalSchemeManager cacheLocalSchemeManager
    {
        get
        {
            return CacheLocalSchemeManager.Instance;
        }
    }

    private JsonCacheManager jsonCacheManager
    {
        get
        {
            return JsonCacheManager.Instance;
        }
    }

    private MainPageUIData mainpageData
    {
        get {
            return MainPageUIData.Instance;
        }
    }

    private SchemePageController schemeControl
    {
        get {
            return SchemePageController.Instance;
        }
    }

    public UndoHelper()
    {

    }

    ///// <summary>
    ///// 输入数据（点 线 位置等）
    ///// </summary>
    //private OriginalInputData originalData = OriginalInputData.Instance;

    /// <summary>
    /// 备份的后退数据（做还原使用）//堆栈后进先出
    /// </summary>
    public List<TotalData> undoDatas = new List<TotalData>();

    /// <summary>
    /// 备份的前进数据（备份分前进，后退，和当前currentData）
    /// </summary>
    public Stack<TotalData> redoDatas = new Stack<TotalData>();

    /// <summary>
    /// 最近的一次不保存进列表 用临时变量存
    /// </summary>
    public TotalData currentData = new TotalData();

    public void CreatNewEnter()
    {
        OriginalProjectData OriginalProjectData = new OriginalProjectData();
        TotalData total = new TotalData();
        OriginalProjectData.data = total.data;
        OriginalProjectData.id = total.schemeManifest.id;
        OriginalProjectData.name = total.schemeManifest.name;
        OriginalProjectData.description = total.schemeManifest.description;
        OriginalProjectData.meta = total.schemeManifest.meta;
        //OriginalProjectData.tempId = total.schemeManifest.tempId;
        OriginalProjectData.isNew = true;
        SetCurrentData(OriginalProjectData, false);
        if (cacheLocalSchemeManager.isReady)
        {
            currentData.schemeManifest.tempId = cacheLocalSchemeManager.GetNewLocalcacheTempId();
        }
        else
        {
            currentData.schemeManifest.tempId = -1;
            cacheLocalSchemeManager.addEventListener(MyCacheEvent.loadReady, LoadReady);
        }
        //jsonCacheManager.SaveSchemeToLocal();
    }

    private void LoadReady(MyEvent obj)
    {
        cacheLocalSchemeManager.removeEventListener(MyCacheEvent.loadReady, LoadReady);
        if (string.IsNullOrEmpty(currentData.schemeManifest.id))
        {
            currentData.schemeManifest.tempId = CacheLocalSchemeManager.Instance.GetNewLocalcacheTempId();
            jsonCacheManager.SaveSchemeToLocal();
        }
    }

    /// <summary>
    /// 保存一次数据  使用方法：应该先使用保存 再更改变化
    /// </summary>
    public void save()
    {
        //Debug.Log("Saved");
        if (undoDatas.Count > 100)
        {
            undoDatas.RemoveAt(0);
        }

        currentData.data.ResetId();
        currentData.SaveId++;

        ///新保存的数据
        TotalData inputdata = new TotalData();
        inputdata.SaveId = currentData.SaveId;
        inputdata.schemeManifest = currentData.schemeManifest;

        for (int i = 0; i < currentData.data.wallList.Count; i++)
        {
            WallData wall = currentData.data.wallList[i];
            WallData newWall = new WallData();
            newWall.guid = wall.guid;
            newWall.height = wall.height;
            newWall.width = wall.width;
            Point p1 = getPointById(wall.point1.guid, inputdata.data);
            if (p1 == null)
            {
                p1 = new Point();
                p1.guid = wall.point1.guid;
                p1.pos = wall.point1.pos;
                inputdata.data.AddPoint(p1);
            }
            newWall.point1 = p1;
            Point p2 = getPointById(wall.point2.guid, inputdata.data);
            if (p2 == null)
            {
                p2 = new Point();
                p2.guid = wall.point2.guid;
                p2.pos = wall.point2.pos;
                inputdata.data.AddPoint(p2);
            }
            newWall.point2 = p2;
            WallSideData newSide1 = new WallSideData();
            newSide1.guid = wall.point1To2Data.guid;
            newSide1.forward = wall.point1To2Data.forward;
            MaterialData materialData = new MaterialData();
            materialData.textureURI = wall.point1To2Data.materialData.textureURI;
            materialData.id = wall.point1To2Data.materialData.id;
            materialData.guid = wall.point1To2Data.materialData.guid;
            materialData.seekId = wall.point1To2Data.materialData.seekId;
            materialData.color = wall.point1To2Data.materialData.color;
            materialData.tileSize_x = wall.point1To2Data.materialData.tileSize_x;
            materialData.tileSize_y = wall.point1To2Data.materialData.tileSize_y;
            materialData.rotation= wall.point1To2Data.materialData.rotation;
            materialData.offsetX = wall.point1To2Data.materialData.offsetX;
            materialData.offsetY = wall.point1To2Data.materialData.offsetY;
            newSide1.materialData = materialData;
            //newSide1.texture = wall.point1To2Data.texture;
            //newSide1.mesh = wall.point1To2Data.mesh;
            newSide1.targetWall = newWall;

            TBaseboard newTBaseboard  = new TBaseboard(true);
            newTBaseboard.guid = wall.point1To2Data.tBaseboard.guid;
            newTBaseboard.id = wall.point1To2Data.tBaseboard.id;
            newTBaseboard.hide = wall.point1To2Data.tBaseboard.hide;
            newTBaseboard.isT = wall.point1To2Data.tBaseboard.isT;
            newTBaseboard.type = wall.point1To2Data.tBaseboard.type;
            newTBaseboard.width = wall.point1To2Data.tBaseboard.width;
            newTBaseboard.height = wall.point1To2Data.tBaseboard.height;
            newTBaseboard.disRoot = wall.point1To2Data.tBaseboard.disRoot;
            if (wall.point1To2Data.tBaseboard.materialData != null)
            {
                MaterialData tMaterialData = new MaterialData();
                tMaterialData.textureURI = wall.point1To2Data.tBaseboard.materialData.textureURI;
                tMaterialData.id = wall.point1To2Data.tBaseboard.materialData.id;
                tMaterialData.guid = wall.point1To2Data.tBaseboard.materialData.guid;
                tMaterialData.seekId = wall.point1To2Data.tBaseboard.materialData.seekId;
                tMaterialData.color = wall.point1To2Data.tBaseboard.materialData.color;
                tMaterialData.tileSize_x = wall.point1To2Data.tBaseboard.materialData.tileSize_x;
                tMaterialData.tileSize_y = wall.point1To2Data.tBaseboard.materialData.tileSize_y;
                tMaterialData.rotation = wall.point1To2Data.tBaseboard.materialData.rotation;
                tMaterialData.offsetX = wall.point1To2Data.tBaseboard.materialData.offsetX;
                tMaterialData.offsetY = wall.point1To2Data.tBaseboard.materialData.offsetY;
                newTBaseboard.materialData = tMaterialData;//wall.point1To2Data.tBaseboard.materialData;
            }
            newSide1.tBaseboard = newTBaseboard;
            TBaseboard newTopBaseboard = new TBaseboard(true);
            newTopBaseboard.guid = wall.point1To2Data.topBaseboard.guid;
            newTopBaseboard.id = wall.point1To2Data.topBaseboard.id;
            newTopBaseboard.hide = wall.point1To2Data.topBaseboard.hide;
            newTopBaseboard.isT = wall.point1To2Data.topBaseboard.isT;
            newTopBaseboard.type = wall.point1To2Data.topBaseboard.type;
            newTopBaseboard.width = wall.point1To2Data.topBaseboard.width;
            newTopBaseboard.height = wall.point1To2Data.topBaseboard.height;
            newTopBaseboard.disRoot = wall.point1To2Data.topBaseboard.disRoot;
            if (wall.point1To2Data.topBaseboard.materialData != null)
            {
                MaterialData topMaterialData = new MaterialData();
                topMaterialData.textureURI = wall.point1To2Data.topBaseboard.materialData.textureURI;
                topMaterialData.id = wall.point1To2Data.topBaseboard.materialData.id;
                topMaterialData.guid = wall.point1To2Data.topBaseboard.materialData.guid;
                topMaterialData.seekId = wall.point1To2Data.topBaseboard.materialData.seekId;
                topMaterialData.color = wall.point1To2Data.topBaseboard.materialData.color;
                topMaterialData.tileSize_x = wall.point1To2Data.topBaseboard.materialData.tileSize_x;
                topMaterialData.tileSize_y = wall.point1To2Data.topBaseboard.materialData.tileSize_y;
                topMaterialData.rotation = wall.point1To2Data.topBaseboard.materialData.rotation;
                topMaterialData.offsetX = wall.point1To2Data.topBaseboard.materialData.offsetX;
                topMaterialData.offsetY = wall.point1To2Data.topBaseboard.materialData.offsetY;
                newTopBaseboard.materialData = topMaterialData;
            }
            newSide1.topBaseboard = newTopBaseboard;

            newWall.point1To2Data = newSide1;

            WallSideData newSide2 = new WallSideData();
            newSide2.guid = wall.point2To1Data.guid;
            newSide2.forward = wall.point2To1Data.forward;
            MaterialData materialData2 = new MaterialData();
            materialData2.textureURI = wall.point2To1Data.materialData.textureURI;
            materialData2.id = wall.point2To1Data.materialData.id;
            materialData2.guid = wall.point2To1Data.materialData.guid;
            materialData2.seekId = wall.point2To1Data.materialData.seekId;
            materialData2.color = wall.point2To1Data.materialData.color;
            materialData2.tileSize_x = wall.point2To1Data.materialData.tileSize_x;
            materialData2.tileSize_y = wall.point2To1Data.materialData.tileSize_y;
            materialData2.rotation = wall.point2To1Data.materialData.rotation;
            materialData2.offsetX = wall.point2To1Data.materialData.offsetX;
            materialData2.offsetY = wall.point2To1Data.materialData.offsetY;
            newSide2.materialData = materialData2;
            //newSide2.texture = wall.point2To1Data.texture;
            //newSide2.mesh = wall.point2To1Data.mesh;
            newSide2.targetWall = newWall;

            TBaseboard newTBaseboard2 = new TBaseboard(true);
            newTBaseboard2.guid = wall.point2To1Data.tBaseboard.guid;
            newTBaseboard2.id = wall.point2To1Data.tBaseboard.id;
            newTBaseboard2.hide = wall.point2To1Data.tBaseboard.hide;
            newTBaseboard2.isT = wall.point2To1Data.tBaseboard.isT;
            newTBaseboard2.type = wall.point2To1Data.tBaseboard.type;
            newTBaseboard2.width = wall.point2To1Data.tBaseboard.width;
            newTBaseboard2.height = wall.point2To1Data.tBaseboard.height;
            newTBaseboard2.disRoot = wall.point2To1Data.tBaseboard.disRoot;
            if (wall.point2To1Data.tBaseboard.materialData != null)
            {
                MaterialData tMaterialData2 = new MaterialData();
                tMaterialData2.textureURI = wall.point2To1Data.tBaseboard.materialData.textureURI;
                tMaterialData2.id = wall.point2To1Data.tBaseboard.materialData.id;
                tMaterialData2.guid = wall.point2To1Data.tBaseboard.materialData.guid;
                tMaterialData2.seekId = wall.point2To1Data.tBaseboard.materialData.seekId;
                tMaterialData2.color = wall.point2To1Data.tBaseboard.materialData.color;
                tMaterialData2.tileSize_x = wall.point2To1Data.tBaseboard.materialData.tileSize_x;
                tMaterialData2.tileSize_y = wall.point2To1Data.tBaseboard.materialData.tileSize_y;
                tMaterialData2.rotation = wall.point2To1Data.tBaseboard.materialData.rotation;
                tMaterialData2.offsetX = wall.point2To1Data.tBaseboard.materialData.offsetX;
                tMaterialData2.offsetY = wall.point2To1Data.tBaseboard.materialData.offsetY;
                newTBaseboard2.materialData = tMaterialData2;//wall.point2To1Data.tBaseboard.materialData;
            }
            newSide2.tBaseboard = newTBaseboard2;
            TBaseboard newTopBaseboard2 = new TBaseboard(true);
            newTopBaseboard2.guid = wall.point2To1Data.topBaseboard.guid;
            newTopBaseboard2.id = wall.point2To1Data.topBaseboard.id;
            newTopBaseboard2.hide = wall.point2To1Data.topBaseboard.hide;
            newTopBaseboard2.isT = wall.point2To1Data.topBaseboard.isT;
            newTopBaseboard2.type = wall.point2To1Data.topBaseboard.type;
            newTopBaseboard2.width = wall.point2To1Data.topBaseboard.width;
            newTopBaseboard2.height = wall.point2To1Data.topBaseboard.height;
            newTopBaseboard2.disRoot = wall.point2To1Data.topBaseboard.disRoot;
            if (wall.point2To1Data.topBaseboard.materialData != null)
            {
                MaterialData topMaterialData2 = new MaterialData();
                topMaterialData2.textureURI = wall.point2To1Data.topBaseboard.materialData.textureURI;
                topMaterialData2.id = wall.point2To1Data.topBaseboard.materialData.id;
                topMaterialData2.guid = wall.point2To1Data.topBaseboard.materialData.guid;
                topMaterialData2.seekId = wall.point2To1Data.topBaseboard.materialData.seekId;
                topMaterialData2.color = wall.point2To1Data.topBaseboard.materialData.color;
                topMaterialData2.tileSize_x = wall.point2To1Data.topBaseboard.materialData.tileSize_x;
                topMaterialData2.tileSize_y = wall.point2To1Data.topBaseboard.materialData.tileSize_y;
                topMaterialData2.rotation = wall.point2To1Data.topBaseboard.materialData.rotation;
                topMaterialData2.offsetX = wall.point2To1Data.topBaseboard.materialData.offsetX;
                topMaterialData2.offsetY = wall.point2To1Data.topBaseboard.materialData.offsetY;
                newTopBaseboard2.materialData = topMaterialData2;
            }
            newSide2.topBaseboard = newTopBaseboard2;

            newWall.point2To1Data = newSide2;
            
            inputdata.data.AddWall(newWall, true);
        }
        for (int i = 0; i < currentData.data.roomList.Count; i++)
        {
            RoomData room = currentData.data.roomList[i];
            RoomData newroom = new RoomData();
            newroom.pointList = new List<Point>();
            newroom.guid = room.guid;
            newroom.name = room.name;
            newroom.type = room.type;
            newroom.ceilingHeight = room.ceilingHeight;
            //newroom.floor = room.floor;
            //newroom.ceiling = room.ceiling;

            MaterialData materialData = new MaterialData();
            materialData.textureURI = room.floor.textureURI;
            materialData.id = room.floor.id;
            materialData.guid = room.floor.guid;
            materialData.seekId = room.floor.seekId;
            materialData.color = room.floor.color;
            materialData.tileSize_x = room.floor.tileSize_x;
            materialData.tileSize_y = room.floor.tileSize_y;
            materialData.rotation = room.floor.rotation;
            materialData.offsetX = room.floor.offsetX;
            materialData.offsetY = room.floor.offsetY;
            newroom.floor = materialData;

            MaterialData materialData2 = new MaterialData();
            materialData2.textureURI = room.ceiling.textureURI;
            materialData2.id = room.ceiling.id;
            materialData2.guid = room.ceiling.guid;
            materialData2.seekId = room.ceiling.seekId;
            materialData2.color = room.ceiling.color;
            materialData2.tileSize_x = room.ceiling.tileSize_x;
            materialData2.tileSize_y = room.ceiling.tileSize_y;
            materialData2.rotation = room.ceiling.rotation;
            materialData2.offsetX = room.ceiling.offsetX;
            materialData2.offsetY = room.ceiling.offsetY;
            newroom.ceiling = materialData2;

            for (int k = 0; k < room.sideList.Count; k++)
            {
                WallSideData side = room.sideList[k];
                WallSideData aimside = getSideById(side.guid, inputdata.data);
                if (aimside != null)
                {
                    newroom.sideList.Add(aimside);
                }
                else
                {
                    Debug.LogWarning("aimside == null");
                }
            }
            for (int k = 0; k < room.pointList.Count; k++)
            {
                Point point = room.pointList[k];
                Point aimpoint = getPointById(point.guid, inputdata.data);
                newroom.pointList.Add(aimpoint);
            }
            inputdata.data.roomList.Add(newroom);
        }

        for (int i = 0; i < currentData.data.productDataList.Count; i++)
        {
            ProductData product = currentData.data.productDataList[i];
            ProductData newproduct = new ProductData(product.id, product.position, product.rotate, product.seekId, product.height, product.type);
            newproduct.guid = product.guid;
            newproduct.hide = product.hide;
            newproduct.scale = product.scale;
            if (product.targetWall != null)
            {
                newproduct.targetWall = inputdata.data.GetWall(product.targetWall.guid);
            }
            else
            {
                newproduct.targetWall = null;
            }
            inputdata.data.productDataList.Add(newproduct);
        }

        for (int i = 0; i < currentData.data.maxAngleRooms.Count; i++)
        {
            RoomData room = currentData.data.maxAngleRooms[i];
            RoomData newroom = new RoomData();
            newroom.pointList = new List<Point>();
            newroom.guid = room.guid;
            newroom.name = room.name;
            newroom.type = room.type;
            newroom.ceilingHeight = room.ceilingHeight;
            //newroom.floor = room.floor;
            //newroom.ceiling = room.ceiling;

            MaterialData materialData = new MaterialData();
            materialData.textureURI = room.floor.textureURI;
            materialData.id = room.floor.id;
            materialData.guid = room.floor.guid;
            materialData.seekId = room.floor.seekId;
            materialData.color = room.floor.color;
            materialData.tileSize_x = room.floor.tileSize_x;
            materialData.tileSize_y = room.floor.tileSize_y;
            materialData.rotation = room.floor.rotation;
            materialData.offsetX = room.floor.offsetX;
            materialData.offsetY = room.floor.offsetY;
            newroom.floor = materialData;

            MaterialData materialData2 = new MaterialData();
            materialData2.textureURI = room.ceiling.textureURI;
            materialData2.id = room.ceiling.id;
            materialData2.guid = room.ceiling.guid;
            materialData2.seekId = room.ceiling.seekId;
            materialData2.color = room.ceiling.color;
            materialData2.tileSize_x = room.ceiling.tileSize_x;
            materialData2.tileSize_y = room.ceiling.tileSize_y;
            materialData2.rotation = room.ceiling.rotation;
            materialData2.offsetX = room.ceiling.offsetX;
            materialData2.offsetY = room.ceiling.offsetY;
            newroom.ceiling = materialData2;

            for (int k = 0; k < room.sideList.Count; k++)
            {
                WallSideData side = room.sideList[k];
                WallSideData aimside = getSideById(side.guid, inputdata.data);
                if (aimside != null)
                {
                    newroom.sideList.Add(aimside);
                }
                else
                {
                    Debug.LogWarning("aimside == null");
                }
            }
            for (int k = 0; k < room.pointList.Count; k++)
            {
                Point point = room.pointList[k];
                Point aimpoint = getPointById(point.guid, inputdata.data);
                newroom.pointList.Add(aimpoint);
            }
            inputdata.data.maxAngleRooms.Add(newroom);
        }

        for (int i = 0; i < currentData.data.preScanDatas.Count; i++)
        {
            PreScanData scanData = currentData.data.preScanDatas[i] as PreScanData;
            PreScanData newScanData = new PreScanData();
            newScanData.id = scanData.id;
            newScanData.position = scanData.position;
            newScanData.rotate = scanData.rotate;
            newScanData.imagemeta = scanData.imagemeta;
            inputdata.data.preScanDatas.Add(newScanData);
        }

        inputdata.data.defaultsettings = currentData.data.defaultsettings;

        undoDatas.Add(inputdata);

        redoDatas.Clear();

        //jsonCacheManager.SaveSchemeToLocal();

        Resources.UnloadUnusedAssets();
    }

    public void ResetSaveId()
    {
        int id = currentData.SaveId;
        for (int i = 0; i < undoDatas.Count; i++)
        {
            undoDatas[i].SaveId -= id;
        }
        TotalData[]  redoredos = redoDatas.ToArray();
        for (int i = 0; i < redoredos.Length; i++)
        {
            redoredos[i].SaveId -= id;
        }
        currentData.SaveId = 0;
    }

    public void SetCurrentData(OriginalProjectData project, bool saveToLocal = true)
    {
        //SetToLocal(currentData);
        undoDatas.Clear();
        redoDatas.Clear();
        currentData = new TotalData();
        currentData.SaveId = 0;
        currentData.data = project.data;
        currentData.schemeManifest.id = project.id;
        currentData.schemeManifest.templateId = project.templateId;
        currentData.schemeManifest.meta = project.meta;
        currentData.schemeManifest.name = project.name;
        currentData.schemeManifest.description = project.description;
        currentData.schemeManifest.tempId = project.tempId;
        currentData.schemeManifest.isNew = project.isNew;
        currentData.schemeManifest.version = project.version;

        currentData.schemeManifest.prices.Clear();
        currentData.schemeManifest.tempEffectMetas = project.tempEffectMetas;
        //是否为样板间
        currentData.schemeManifest.isTemplate = project.isTemplate;
        if (project.priceIdList != null)
        {
            currentData.schemeManifest.prices.AddRange(project.priceIdList);
            for (int i = 0; i < project.priceIdList.Count; i++)
            {
                string id = project.priceIdList[i];
                if (mainpageData.HasPriceData(id))
                {
                    continue;
                }
                int tempId;
                if (int.TryParse(id, out tempId))
                {
                    LoaderPool.CacheLoad(tempId, SimpleLoadDataType.JsonOffer, OnPriceLoaded);
                }
                else
                {
                    LoaderPool.CacheLoad(id, SimpleLoadDataType.JsonOffer, OnPriceLoaded);
                }
            }

        }

        //ClearView();
        RefreshView();
        if (saveToLocal == true)
        {
            jsonCacheManager.SaveSchemeToLocal();
        }

        schemeControl.dispatchEvent(new SchemeEvent(SchemeEvent.ChangeScheme));
        Resources.UnloadUnusedAssets();
    }

    private void OnPriceLoaded(object obj)
    {
        SimpleCacheLoader loader = obj as SimpleCacheLoader;
        if (loader.state == SimpleLoadedState.Failed)
        {
            //Debug.LogError(loader.uri);
            return;
        }
        string json = loader.loadedData.ToString();
        object jsonObj = MyJsonTool.FromJson(json);
        PriceData priceData = new PriceData();
        priceData.Deserialize(jsonObj as Dictionary<string, object>);
        mainpageData.AddPriceData(priceData);
    }

    public WallSideData getSideById(int id, OriginalInputData inputdata)
    {
        for (int i = 0; i < inputdata.wallList.Count; i++)
        {
            WallData wall = inputdata.wallList[i];
            if (wall.point1To2Data.guid == id)
            {
                return wall.point1To2Data;
            }
            if (wall.point2To1Data.guid == id)
            {
                return wall.point2To1Data;
            }
        }
        return null;
    }

    public Point getPointById(int id, OriginalInputData inputdata)
    {
        for (int i = 0; i < inputdata.pointList.Count; i++)
        {
            Point p = inputdata.pointList[i];
            if (p.guid == id)
            {
                return p;
            }
        }
        return null;
    }

    /// <summary>
    /// 还原一次数据
    /// </summary>
    public void undo()
    {
        if (machine.CurrentState != null)
        {
            if (machine.currentInputIs2D)
            {
                if (machine.CurrentState.GetType() != typeof(FreeState2D))
                {
                    machine.setState(FreeState2D.NAME);
                    return;
                }
            }
            else
            {
                if (machine.CurrentState.GetType() != typeof(FreeState3D))
                {
                    machine.setState(FreeState3D.NAME);
                    return;
                }
            }
        }
        if (undoDatas.Count == 0)
        {
            return;
        }

        DefaultSettings defaultsettings = currentData.data.defaultsettings;
        ///当前数据放进前进列表redo使用
        redoDatas.Push(currentData);
        ///取出上次操作数据还原到当前
        currentData = undoDatas[undoDatas.Count-1];
        undoDatas.RemoveAt(undoDatas.Count - 1);

        //还原：原始输入的基本数据（拐点和墙 贴图等）
        //originalData.SetToData(currentData);
        //还原：直接设置产生的直接数据（拐点与拐点的关系等）
        currentData.data.AfterSetToData();
        //还原：直接设置产生的附属数据（墙围成的房间， 地板范围等）
        //originalData.SetToData(currentData);
        //还原：附属数据里的输入部分（地板贴图等属性）
        //currentData.data.AfterSetToData();

        RefreshView();

        jsonCacheManager.SaveSchemeToLocal();
    }

    /// <summary>
    /// 前进一次数据
    /// </summary>
    public void redo()
    {
        if (redoDatas.Count == 0)
        {
            return;
        }
        ///当前数据放进前进列表undo使用
        undoDatas.Add(currentData);
        ///取出上次操作数据还原到当前
        currentData = redoDatas.Pop();

        //还原：原始输入的基本数据（拐点和墙 贴图等）
        //originalData.SetToData(currentData);
        //还原：直接设置产生的直接数据（拐点与拐点的关系等）
        currentData.data.AfterSetToData();

        //还原：直接设置产生的附属数据（墙围成的房间， 地板范围等）
        //originalData.SetToData(currentData);
        //还原：附属数据里的输入部分（地板贴图等属性）
        //currentData.data.AfterSetToData();

        RefreshView();

        jsonCacheManager.SaveSchemeToLocal();
    }
    private void RefreshView()
    {
        View2D.Instance.RefreshView();
        View3D.Instance.RefreshView();
    }
    public void ClearView()
    {
        OriginalProjectData projectData = new OriginalProjectData();
        projectData.data = new OriginalInputData();
        SetCurrentData(projectData, false);
        ResourcesPool.ClearPool();
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

}
