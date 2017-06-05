using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JsonCacheManager : Singleton<JsonCacheManager>
{
    private CacheLocalSchemeManager cacheLocalSchemeManager
    {
        get
        {
            return CacheLocalSchemeManager.Instance;
        }
    }

    private CacheServerSchemeManager cacheServerSchemeManager
    {
        get
        {
            return CacheServerSchemeManager.Instance;
        }
    }

    private UndoHelper undoHelper
    {
        get {
            return UndoHelper.Instance;
        }
    }

    private CacheLocalOfferManager cacheLocalOfferManager
    {
        get
        {
            return CacheLocalOfferManager.Instance;
        }
    }

    private CacheServerOfferManager cacheServerOfferManager
    {
        get
        {
            return CacheServerOfferManager.Instance;
        }
    }
    private OriginalInputData originalInputData
    {
        get {
            return OriginalInputData.Instance;
        }
    }

    public void AddSchemeCache(OriginalProjectData data, string json = null)
    {
        if (string.IsNullOrEmpty(json))
        {
            json = MyJsonTool.ToJson(data);
        }
        if (data.isUnUpload == true)
        {
            cacheLocalSchemeManager.AddSchemeCache(data.tempId, json);
            //Debug.Log("data.isUnUpload == true");
        }
        else
        {
            cacheServerSchemeManager.AddSchemeCache(data.id, json);
            //Debug.Log("data.isUnUpload == false");
        }
    }


    public void SaveSchemeToLocal()
    {
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("SaveToLocal");
        //    return;
        //}
        //Debug.Log("SaveToLocal");
        OriginalProjectData OriginalProjectData = GetCurrentOriginalProjectData();
        AddSchemeCache(OriginalProjectData);
    }

    public OriginalProjectData GetCurrentOriginalProjectData()
    {
        originalInputData.BeforetSerializeFieldDo();
        OriginalProjectData originalProjectData = new OriginalProjectData();
        originalProjectData.data = originalInputData;
        originalProjectData.id = undoHelper.currentData.schemeManifest.id;
        originalProjectData.tempId = undoHelper.currentData.schemeManifest.tempId;
        originalProjectData.name = undoHelper.currentData.schemeManifest.name;
        originalProjectData.description = undoHelper.currentData.schemeManifest.description;
        originalProjectData.meta = undoHelper.currentData.schemeManifest.meta;
        originalProjectData.isNew = undoHelper.currentData.schemeManifest.isNew;
        originalProjectData.version = undoHelper.currentData.schemeManifest.version;

        originalProjectData.priceIdList = new List<string>();
        originalProjectData.priceIdList.AddRange(undoHelper.currentData.schemeManifest.prices);
        return originalProjectData;
    }


    /// <summary>
    /// 生成报价、加载报价后缓存、
    /// </summary>
    /// <param name="data"></param>
    /// <param name="json"></param>
    public void AddOfferCache(PriceData data, string json = null)
    {
        if (string.IsNullOrEmpty(json))
        {
            json = MyJsonTool.ToJson(data);
        }
        if (data.isUnUpload == true)
        {
            cacheLocalOfferManager.AddOfferCache(data.tempId, json);
            //Debug.Log("AddOfferCache data.isUnUpload == true");
        }
        else
        {
            cacheServerOfferManager.AddOfferCache(data.serverId, json);
            //Debug.Log("AddOfferCache data.isUnUpload == false");
        }
    }

    /// <summary>
    /// 删除报价-注意PriceData serverId的赋值 顺序
    /// </summary>
    /// <param name="data"></param>
    /// <param name="json"></param>
    public void RemoveOfferCache(PriceData data)
    {
        if (data.isUnUpload == true)
        {
            RemoveOfferCache(data.tempId);
        }
        else
        {
            RemoveOfferCache(data.serverId);
        }
    }

    /// <summary>
    /// 删除本地临时报价
    /// </summary>
    /// <param name="tempId"></param>
    public void RemoveOfferCache(int tempId)
    {
        cacheLocalOfferManager.RemoveOfferCache(tempId);
        //Debug.Log("RemoveOfferCache data.isUnUpload == true");
    }
    /// <summary>
    /// 删除服务器缓存报价
    /// </summary>
    /// <param name="serverId"></param>
    public void RemoveOfferCache(string serverId)
    {
        cacheServerOfferManager.RemoveOfferCache(serverId);
        //Debug.Log("RemoveOfferCache data.isUnUpload == false");
    }


    public int GetNewSchemeTempId()
    {
        return cacheLocalSchemeManager.GetNewLocalcacheTempId();
    }

    public int GetNewOfferTempId()
    {
        return cacheLocalOfferManager.GetNewLocalcacheTempId();
    }
}
