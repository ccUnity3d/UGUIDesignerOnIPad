using UnityEngine;
using System.Collections;
using System;

public class SimpleOutterLoader : SimpleLoader {

    static public readonly string serverURL = "http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/";
    static public readonly string LocalWriteURL = Application.persistentDataPath + "/";
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER
    static public readonly string LocalURL = "file:///" + Application.persistentDataPath + "/";
#elif UNITY_ANDROID   //安卓
    static public readonly string LocalURL = "jar:file://" + Application.persistentDataPath + "/";
#elif UNITY_IPHONE  //iPhone
	static public readonly string LocalURL =  "file://" + Application.persistentDataPath + "/";
#else
    static public readonly string LocalURL =string.Empty;
#endif
    public string httpUrl;
    public override string url
    {
        get {
            if (localHas)
            {
                return localloadurl;
            }
            return httpUrl;
        }
    }

    public string localloadurl
    {
        get
        {
            return LocalURL + uri;
        }
    }

    public string localWriteurl
    {
        get
        {
            return LocalWriteURL + uri;
        }
    }

    public override string keyUrl
    {
        get
        {
            return httpUrl;
        }
    }


    private CacheModelManager cacheModelManager
    {
        get
        {
            return CacheModelManager.Instance;
        }
    }

    private bool localHas = true;
    private string souceType = "";
    public Action<GameObject, object> OnLoadprefabBeforClone;

    public SimpleOutterLoader(string url, SimpleLoadDataType type, Action<object> onloaded, object bringData, Action<GameObject, object> onloadedBforClone = null)
    {
        this.httpUrl = url;
        this.type = type;
        this.onloaded = onloaded;
        this.bringData = bringData;
        this.OnLoadprefabBeforClone = onloadedBforClone;
        state = SimpleLoadedState.None;
        this.uri = getUri(httpUrl);
    }

    private string getUri(string httpUrl)
    {
        string[] strs = httpUrl.Split('/');
        if (strs.Length < 2)
        {
            Debug.LogError("httpUrl = " + httpUrl + " httpUrl.Split('/').Length < 2 " + httpUrl);
            return "";
        }
        string resource = strs[strs.Length - 1];
        string fold = strs[strs.Length - 2] + "/";
        if (fold == "resized/")
        {
            fold = strs[strs.Length - 3] + "/" + fold;
        }

        string[] str2s = resource.Split('.');
        if (str2s.Length < 2)
        {
            Debug.LogWarning("resource.Split('.').Length < 2 " + httpUrl);
            return "";
        }
        souceType = str2s[str2s.Length - 1];
        switch (souceType)
        {
            case "png":
            case "jpg":
                fold = "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Texture2D/" + fold;
                break;
            case "assetbundle":
                fold = "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Assetbundle/" + fold;
                break;
            case "midf":
                fold = "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/OutMidf/" + fold;
                //fold = "Scheme/Server/";
                break;
            default:
                break;
        }
        return fold + resource;
    }

    public override void StartLoad()
    {
        base.StartLoad();
        localHas = cacheModelManager.HaseCached(localWriteurl);
        MyMono.MyStartCoroutine(load, this, null);
    }


    protected override void stopLoad()
    {
        MyMono.MyStopCoroutine(load, this, null);
        base.stopLoad();
    }


    //private void LoadInPool(object data)
    //{
    //    //Debug.LogWarning("LoadInPool:"+uri);
    //    ResourcesPool.LoadPoolData poolData = data as ResourcesPool.LoadPoolData;
    //    if (poolData is ResourcesPool.ErrorData)
    //    {
    //        ResourcesPool.ErrorData errorData = poolData as ResourcesPool.ErrorData;
    //        errorData.errorCount++;
    //        if (errorData.errorCount > errorData.MaxErrorCount)
    //        {
    //            return;
    //        }
    //    }
    //    switch (poolData.type)
    //    {
    //        case ResourcesPool.PoolDataType.Json:
    //            loadedData = poolData.resouce;
    //            break;
    //        case ResourcesPool.PoolDataType.texture2D:
    //            loadedData = poolData.resouce;
    //            break;
    //        case ResourcesPool.PoolDataType.Prefab:
    //            ResourcesPool.PrefabData prefabData = poolData as ResourcesPool.PrefabData;
    //            if (canceled == false && juseEndReturn == false) loadedData = prefabData.GetNew();
    //            break;
    //        case ResourcesPool.PoolDataType.Byte:
    //        default:
    //            loadedData = null;
    //            Debug.LogWarning("没有此类型的池！");
    //            break;
    //    }

    //    if (canceled == false && onloaded != null)
    //    {
    //        onloaded(this);
    //    }
    //}
    private IEnumerator load(object[] arg1)
    {
        //加载成功 但是加载到的数据不对
        bool loadedIsWrong = false;
        string path = url;
        path = path.Replace("midea-products.oss-cn-shanghai.aliyuncs.com/", "pms.3dshome.net/");
        Debug.LogWarning("OutterLoad:" + path);
        www = new WWW(path);

        if (checkProgress)
        {
            MyTickManager.Add(Progress);
        }
        yield return www;
        //yield return new WaitForSeconds(2);//模拟慢网速
        if (checkProgress)
        {
            MyTickManager.Remove(Progress);
        }
        //if (www == null)
        //{
        //    state = SimpleLoadedState.Failed;
        //    loadedData = null;
        //    string message = string.Format("加载文件失败:{0}", url);
        //    Debug.Log(message);
        //}
        //else {
        if (string.IsNullOrEmpty(www.error))
        {
            state = SimpleLoadedState.Success;
            switch (type)
            {
                case SimpleLoadDataType.prefabAssetBundle:

                    string realName = System.IO.Path.GetFileNameWithoutExtension(url);
                    AssetBundle bundle = www.assetBundle;
                    if (bundle != null)
                    {
                        UnityEngine.Object[] objs = bundle.LoadAllAssets();
                        for (int i = 0; i < objs.Length; i++)
                        {
                            if (objs[i].GetType() == typeof(GameObject))
                            {
                                if (objs[i].name == realName)
                                {
                                    GameObject data = objs[i] as GameObject;
                                    data.name = uri;
                                    if (OnLoadprefabBeforClone != null) OnLoadprefabBeforClone(data, bringData);
                                    ResourcesPool.PrefabData prefab = resourcePool.addPrefab(httpUrl, data);
                                    if (canceled == false && juseEndReturn == false) loadedData = prefab.GetNew();
                                    //loadedData = objs[i];
                                    //resourcePool.addPrefab(url, loadedData);
                                }
                            }
                        }
                        if (localHas == false && string.IsNullOrEmpty(uri) == false)
                        {
                            SaveToLocal(www.bytes);
                        }
                        bundle.Unload(false);
                    }
                    else
                    {
                        loadedIsWrong = true;
                        Debug.LogError("加载到的资源不是AssetBundle！path = " + www.url);
                    }
                    break;
                case SimpleLoadDataType.texture2D:
                    if (localHas == false && string.IsNullOrEmpty(uri) == false)
                    {
                        //Texture2D texture = new Texture2D(100,100);
                        //(byte[])texture
                        //try
                        //{
                        SaveToLocal(www.bytes);
                        //}
                        //catch (Exception e)
                        //{
                        //    Debug.LogWarning(e);
                        //}
                        //
                    }
                    if (www.assetBundle != null)
                    {
                        loadedData = www.assetBundle.mainAsset as Texture2D; ;
                        resourcePool.addTexture(httpUrl, loadedData);
                        www.assetBundle.Unload(false);
                    }
                    else
                    {
                        loadedData = www.texture;
                        resourcePool.addTexture(httpUrl, loadedData);
                    }
                    break;
                case SimpleLoadDataType.Json:
                    string json = System.Text.Encoding.UTF8.GetString(www.bytes);
                    int index = json.IndexOf("{");
                    if (index != 0)
                    {
                        json = json.Substring(index);
                    }
                    //同一个Json可能会变化 不记录上次的 因为有可能是过时的
                    //if(localHas == false && string.IsNullOrEmpty(uri) == false) SaveToLocal(www.bytes);
                    loadedData = json;
                    break;
                case SimpleLoadDataType.Byte:
                default:
                    loadedData = null;
                    string message = string.Format("加载文件类型：{0} 失败:", type);
                    Debug.Log(message);
                    break;
            }
        }
        else
        {
            state = SimpleLoadedState.Failed;
            loadedData = null;
            string message = string.Format("加载文件失败:{0}", www.url);
            Debug.Log(message + " Error:" + www.error);
            error = true;
        }
        www.Dispose();
        //Debug.LogWarning("OnLoaded");
        if (loadedIsWrong == false)
        {
            OnLoaded();
        }
        else {
            resourcePool.LoadErrorData(this);
        }
    }

    private void SaveToLocal(byte[] bytes)
    {
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("save:" + localWriteurl);
        //    return;
        //}
        cacheModelManager.AddCache(localWriteurl, bytes);
    }
    
    //private void progress()
    //{
    //    if (www != null)
    //    {
    //        //this.dispatchEvent(new EventX(EventX.PROGRESS, www.progress));
    //    }
    //}
}
