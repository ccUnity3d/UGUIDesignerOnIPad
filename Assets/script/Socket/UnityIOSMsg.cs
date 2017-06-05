using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Collections;

public class UnityIOSMsg : MonoBehaviour {

    public static UnityIOSMsg Instance;
    private static IMyEventDispatcher myEventDispatcher;
    public static UserInfo currentUser = new UserInfo();

    private MainPageStateMachine mainpageMachine
    {
        get {
            return MainPageStateMachine.Instance;
        }
    }
    private UndoHelper undoHelper
    {
        get
        {
            return UndoHelper.Instance;
        }
    }

    private CacheLocalSchemeManager localCache
    {
        get
        {
            return CacheLocalSchemeManager.Instance;
        }
    }

    private CacheServerSchemeManager serverCache
    {
        get
        {
            return CacheServerSchemeManager.Instance;
        }
    }

    private JsonCacheManager jsonCacheManager
    {
        get
        {
            return JsonCacheManager.Instance;
        }
    }

    private MainPageUIController mainPageUIController
    {
        get
        {
            return MainPageUIController.Instance;
        }
    }

    void Awake()
    {
        Instance = this;
        myEventDispatcher = new MyEventDispatcher();
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            TextAsset asset = Resources.Load<TextAsset>("addGoods");
            receiveFromIOSMsg(asset.text);
        }
    }
    
    public static void sendToIOS(MsgToIOS msg, string ListionType = "", Action<MyEvent> handle = null)
    {
        if (string.IsNullOrEmpty(ListionType) == false)
        {
            myEventDispatcher.addEventListener(ListionType, handle);
        }
        
        string json = MyJsonTool.ToJson(msg); 
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //if (msg.code == "101002" || msg.code == "101003")
            //{
            //    Mode2DPrefabs.Instance.mainCamera.enabled = false;
            //}
            Debug.Log("sendToIOS:" + json);
            sendToIOS(json);
        }
        else
        {
            Debug.Log("sendToIOS + " + json);
        }
    }

    public static void removeListioner(string ListionType, Action<MyEvent> handle)
    {
        myEventDispatcher.removeEventListener(ListionType, handle);
    }

    /// <summary>
    /// 向IOS发消息
    /// </summary>
    /// <param name="msg"></param>
    [DllImport("__Internal")]
    public static extern void sendToIOS(string msg);

    /// <summary>
    /// 接收IOS消息
    /// </summary>
    /// <param name="msg"></param>
    public void receiveFromIOSMsg(string msg)
    {
        Debug.LogWarning(msg);
        MsgFromIOS iosMsg = (MsgFromIOS)msg;
        receiveFromIOSMsg(iosMsg);
    }

    public void receiveFromIOSMsg(MsgFromIOS iosMsg)
    {
        switch (iosMsg.code)
        {
            case "201000":
                receiveUserInfo(iosMsg.info.userInfo);
                break;
            case "201001"://进入方案
                EnterDesign(iosMsg.info);
                break;
            case "201002"://选中商品列表信息
                SelectGoods(iosMsg.info.goodsArr);
                break;
            case "201003":
                break;
            case "201004":
                myEventDispatcher.dispatchEvent(new IOSEvent(IOSEvent.SetSchemeId, iosMsg.info));
                break;
            case "201008":
                myEventDispatcher.dispatchEvent(new IOSEvent(IOSEvent.SelectGoods, iosMsg.info));
                break;
            case "201101":
                myEventDispatcher.dispatchEvent(new IOSEvent(IOSEvent.SetOfferId, iosMsg.info));
                break;
            default:
                //Instance.;
                break;
        }
    }

    private void receiveUserInfo(object userInfo)
    {
        currentUser = new UserInfo();
        currentUser.DeSerialize(userInfo as Dictionary<string, object>);
    }

    private void SelectGoods(object data)
    {
        //List<MsgFromIOS.GoodsInfo> goodsList = new List<MsgFromIOS.GoodsInfo>();
        List<object> list = data as List<object>;
        List<Product> products = new List<Product>();
        List<SelectProductData> selectproducts = new List<SelectProductData>();
        for (int i = 0; i < list.Count; i++)
        {
            MsgFromIOS.GoodsInfo info = new MsgFromIOS.GoodsInfo();
            info.Deserialize(list[i] as Dictionary<string, object>);
            //goodsList.Add(info);
            //JsonProduct product = new JsonProduct();
            SelectProductData selctproduct = new SelectProductData();
            info.WriteToSelectProductData(selctproduct);
            //info.WriteToProduct(product);
            products.Add(info.model);
            selectproducts.Add(selctproduct);
        }
        int listionCount = myEventDispatcher.dispatchEvent(new IOSEvent(IOSEvent.SelectGoods, new object[] { products, selectproducts }));
        if (listionCount == 0)
        {
            Debug.LogWarning("未监听事件 IOSEvent.SelectGoods");
        }
    }

    private void EnterDesign(MsgFromIOS.InfoFromIOS info)
    {
        GlobalConfig.running = true;
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            info.msg = new List<object>() { "http://cf-hsm-prod-assets.homestyler.com/i/4abd1bba-6d61-481e-9008-25295599e006/iso.jpg" };
        }
        switch (info.enterType)
        {
            case "1":
                undoHelper.CreatNewEnter();
                mainpageMachine.setState(ToTwoDState.Name);
                break;
            case "2"://进入2D
            case "3"://进入3D
            case "4"://进入新建方案2D 预置指定模板
            case "5"://进入新建方案3D 预置指定模板
                mainPageUIController.dispatchEvent(new LoadEvent(LoadEvent.OpenLoadingPage));
                string id = info.projectId;
                int tempId = info.tempId; 
                if (string.IsNullOrEmpty(id) == true)
                {
                    if (localCache.HasCached(tempId))
                    {
                        LoaderPool.CacheLoad(tempId, SimpleLoadDataType.JsonScheme, OnCacheLoaded, info);
                        return;
                    }
                }
                else
                {
                    if (serverCache.HasCached(id))
                    {
                        LoaderPool.CacheLoad(id, SimpleLoadDataType.JsonScheme, OnCacheLoaded, info);
                        return;
                    }
                }

                LoaderPool.OutterLoad(info.json, SimpleLoadDataType.Json, onloaded, info);
                break;
            default:
                break;
        }
    }

    private void OnCacheLoaded(object obj)
    {
        SimpleCacheLoader loader = obj as SimpleCacheLoader;
        if (loader.state == SimpleLoadedState.Failed)
        {
            //Debug.LogError(loader.uri);
            return;
        }
        string json = loader.loadedData.ToString();
        object jsonData = MyJsonTool.FromJson(json);
        MsgFromIOS.InfoFromIOS info = loader.bringData as MsgFromIOS.InfoFromIOS;

        OriginalProjectData data = new OriginalProjectData();
        Dictionary<string, object> dic = jsonData as Dictionary<string, object>;

        ///在线端数据
        if (dic.ContainsKey("products"))
        {
            //data.DeSerialize(dic);
        }
        ///移动端新数据
        else if (dic.ContainsKey("version"))
        {
            data.DeSerialize(dic);
        }
        ///移动端旧数据 或 错误数据
        else
        {
            data.DeSerialize(dic);
        }

        data.tempEffectMetas.Clear();
        List<object> matelist = info.msg as List<object>;
        for (int i = 0; i < matelist.Count; i++)
        {
            data.tempEffectMetas.Add(matelist[i].ToString());
        }

        switch (info.enterType)
        {
            case "4":
            case "5":
                data.isNew = true;
                data.id = "";
                data.templateId = info.projectId;
                data.tempId = jsonCacheManager.GetNewSchemeTempId();
                //放到队列加载后
                //undoHelper.SetCurrentData(data);
                break;
            default:
                data.id = info.projectId;
                data.tempId = info.tempId;
                data.isNew = false;
                //放到队列加载后
                //undoHelper.SetCurrentData(data, false);
                break;
        }

        switch (info.enterType)
        {
            case "3":
            case "5":
                mainpageMachine.setState(ToThreeDState.Name);
                break;
            default:
                mainpageMachine.setState(ToTwoDState.Name);
                break;
        }

        float minX = 0;
        float maxX = 0;
        float minY = 0;
        float maxY = 0;
        for (int i = 0; i < data.data.pointList.Count; i++)
        {
            Vector2 v2 = data.data.pointList[i].pos;
            if (v2.x < minX)
            {
                minX = v2.x;
            }
            if (v2.x > maxX)
            {
                maxX = v2.x;
            }
            if (v2.y < minY)
            {
                minY = v2.y;
            }
            if (v2.y > maxY)
            {
                maxY = v2.y;
            }
        }
        Vector2 pos = Vector2.zero;
        pos.x = (minX + maxX) / 2;
        pos.y = (minY + maxY) / 2;
        CameraControler.Instance.ResetAllCamera(pos);


        QueueSimpleLoader queueloader = new QueueSimpleLoader(new object[] { info.enterType, data });
        for (int i = 0; i < data.data.productList.Count; i++)
        {
            Product product = data.data.productList[i];
            if (product.productType == "FloorTiles" || product.productType == "Wallpapers")
            {
                if (product.modelTexture.IndexOf("http://") == -1) continue;
                SimpleOutterLoader outLoader = LoaderPool.WaitOutterLoad(product.modelTexture, SimpleLoadDataType.texture2D, null, null);
                queueloader.AddQueueItem(outLoader);
            }
            else {
                switch (info.enterType)
                {
                    case "3":
                    case "5":
                        {
                            if (product.assetBundlePath.IndexOf("http://") == -1)
                            {
                                if (product.assetBundlePath.IndexOf("3D/") != 0 && product.modelTexture.IndexOf("3D/") != 0)
                                {
                                    Debug.LogErrorFormat("路径有误：seekid = {0} product.model3d = {1}", product.seekId, product.assetBundlePath);
                                }
                                continue;
                            }
                            SimpleOutterLoader outLoader = LoaderPool.WaitOutterLoad(product.assetBundlePath, SimpleLoadDataType.prefabAssetBundle, null, product, onloadedBforClone);
                            queueloader.AddQueueItem(outLoader);
                        }
                        break;
                    default:
                        {
                            if (product.assetBundlePath.IndexOf("http://") == -1)
                            {
                                if (product.assetBundlePath.IndexOf("3D/prefab") != 0) Debug.LogError("路径有误：" + product.assetBundlePath);
                                continue;
                            }
                            SimpleOutterLoader outLoader = LoaderPool.WaitOutterLoad(product.assetBundlePath, SimpleLoadDataType.prefabAssetBundle, null, product, onloadedBforClone);
                            queueloader.AddQueueItem(outLoader);
                        }
                        break;
                }
            }
        }
        if (queueloader.getCount != 0)
        {
            queueloader.addEventListener(LoadEvent.QueueProgress, OnAllProductProgress);
            queueloader.addEventListener(LoadEvent.QueueComplete, OnLoadedAllProductCache);
        }
        else
        {
            mainPageUIController.dispatchEvent(new LoadEvent(LoadEvent.CloseLoadingPage));
            switch (info.enterType)
            {
                case "4":
                case "5":
                    data.isTemplate = true;
                    undoHelper.SetCurrentData(data);
                    break;
                default:
                    undoHelper.SetCurrentData(data, false);
                    break;
            }
        }
    }
    
    private void onloaded(object obj)
    {
        SimpleOutterLoader loader = obj as SimpleOutterLoader;
        if (loader.state == SimpleLoadedState.Failed)
        {
            //Debug.LogError(loader.uri);
            return;
        }
        string json = loader.loadedData.ToString();
        object jsonData = MyJsonTool.FromJson(json);
        MsgFromIOS.InfoFromIOS info = loader.bringData as MsgFromIOS.InfoFromIOS;

        OriginalProjectData data = new OriginalProjectData();
        Dictionary<string, object>  dic = jsonData as Dictionary<string, object>;

        ///在线端数据
        if (dic.ContainsKey("products"))
        {
            //data.DeSerialize(dic);
        }
        ///移动端新数据
        else if (dic.ContainsKey("version"))
        {
            data.DeSerialize(dic);
        }
        ///移动端旧数据 或 错误数据
        else
        {
            data.DeSerialize(dic);
        }

        switch (info.enterType)
        {
            case "4":
            case "5":
                data.isTemplate = true;
                data.isNew = true;
                data.templateId = info.projectId;
                data.id = "";
                data.tempId = jsonCacheManager.GetNewSchemeTempId();
                break;
            default:
                data.id = info.projectId;
                data.tempId = info.tempId;
                data.isNew = false;
                break;
        }
        //data.templateId = info.templateId;
        data.tempEffectMetas.Clear();
        List<object> matelist = info.msg as List<object>;
        for (int i = 0; i < matelist.Count; i++)
        {
            data.tempEffectMetas.Add(matelist[i].ToString());
        }

        //放到队列加载后
        //undoHelper.SetCurrentData(data);

        switch (info.enterType)
        {
            case "3":
            case "5":
                mainpageMachine.setState(ToThreeDState.Name);
                break;
            default:
                mainpageMachine.setState(ToTwoDState.Name);
                break;
        }

        float minX = 0;
        float maxX = 0;
        float minY = 0;
        float maxY = 0;
        for (int i = 0; i < data.data.pointList.Count; i++)
        {
            Vector2 v2 = data.data.pointList[i].pos;
            if (v2.x < minX)
            {
                minX = v2.x;
            }
            if (v2.x > maxX)
            {
                maxX = v2.x;
            }
            if (v2.y < minY)
            {
                minY = v2.y;
            }
            if (v2.y > maxY)
            {
                maxY = v2.y;
            }
        }
        Vector2 pos = Vector2.zero;
        pos.x = (minX + maxX) / 2;
        pos.y = (minY + maxY) / 2;
        CameraControler.Instance.ResetAllCamera(pos);

        QueueSimpleLoader queueloader = new QueueSimpleLoader(data);
        for (int i = 0; i < data.data.productList.Count; i++)
        {
            Product product = data.data.productList[i];
            if (product.assetBundlePath.IndexOf("http://") == -1) continue;//if (string.IsNullOrEmpty(product.model3d)) continue;
            SimpleOutterLoader outLoader = LoaderPool.WaitOutterLoad(product.assetBundlePath, SimpleLoadDataType.prefabAssetBundle, null, product, onloadedBforClone);
            queueloader.AddQueueItem(outLoader);
        }
        if (queueloader.getCount != 0)
        {
            queueloader.addEventListener(LoadEvent.QueueProgress, OnAllProductProgress);
            queueloader.addEventListener(LoadEvent.QueueComplete, OnLoadedAllProduct);
        }
        else
        {
            undoHelper.SetCurrentData(data);
        }
    }

    private void onloadedBforClone(GameObject prefab, object bringData)
    {
        if (prefab == null) return;
        BoxCollider boxColli = prefab.AddComponent<BoxCollider>();
        Product product = bringData as Product;
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
        if (product.contentType.StartsWith("door/folding"))
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


    private void OnAllProductProgress(MyEvent obj)
    {
        float progress = (float)obj.data;
        Debug.Log("queueloader Progress = " + progress);
        mainPageUIController.dispatchEvent(new LoadEvent(LoadEvent.RefreshLoadingPage, progress));
    }

    private void OnLoadedAllProduct(MyEvent obj)
    {
        Debug.Log("queueloader Complete");
        mainPageUIController.dispatchEvent(new LoadEvent(LoadEvent.CloseLoadingPage));

        QueueSimpleLoader queueloader = obj.data as QueueSimpleLoader;
        OriginalProjectData data = queueloader.bringData as OriginalProjectData;
        undoHelper.SetCurrentData(data);
    }

    private void OnLoadedAllProductCache(MyEvent obj)
    {
        Debug.Log("queueloader Complete");
        mainPageUIController.dispatchEvent(new LoadEvent(LoadEvent.CloseLoadingPage));

        QueueSimpleLoader queueloader = obj.data as QueueSimpleLoader;
        object[] objs = queueloader.bringData as object[];
        string enterType = objs[0].ToString();
        OriginalProjectData data = objs[1] as OriginalProjectData;
        switch (enterType)
        {
            case "4":
            case "5":
                undoHelper.SetCurrentData(data);
                break;
            default:
                undoHelper.SetCurrentData(data, false);
                break;
        }
    }

    /// <summary>
    /// 接收服务器消息
    /// </summary>
    /// <param name="msg"></param>
    private void receiveFromServerMsg(Dictionary<string, object> msg)
    {
        UnityServerMsg serverData = (UnityServerMsg)msg;
        switch (serverData.code)
        {
            case "201000":
                break;
            case "201001":
                break;
            case "201002":
                break;
            case "201003":
                break;
            default:
                
                break;
        }
    }

    public static void addEventListener(string type, Action<MyEvent> listener)
    {
        if (myEventDispatcher == null)
        {
            Debug.LogError("UnityIOSMsg myEventDispatcher == null");
            return;
        }

        myEventDispatcher.addEventListener(type, listener);
    }

    #region 设计与IOS协议
    //101000 
    //101001 启动成功
    //101002 打开商城
    //101003 关闭设计
    //101004 保存方案   type：0.新的  1.旧的 json：
    //101005 **反馈字符串
    //101006 渲染 渲染编辑数据也是json格式
    //101007 分享  shareType projectData/priceData/
    //101008 收藏 collectType goodsid
    //101009 打开方案列表
    //101010 设为主报价 - 设计id 报价id
    //101011 删除报价 - 设计id 报价id

    //101101 上传/生成报价 priceData
    //101102 渲染效果图 rendersettings


    //201000 推送用户信息
    //201001 进入方案 1.新建设计 2.2D 3.3D  （2、3会有方案json）
    //201002 返回商品数组信息
    //201003 
    //201004 projectId tempId
    //201005 
    //201006
    //2101007 
    //201008 成功 collectType goodsid

    //201101 生成成功 priceId tempId
    //201102 渲染成功 effectTextureId
    #endregion

}
