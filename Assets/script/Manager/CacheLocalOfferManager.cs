using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class CacheLocalOfferManager : Singleton<CacheLocalOfferManager> {

    static public readonly string LocalURL = Application.persistentDataPath + "/";

    static public readonly string URL =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER
                "file:///";
#elif UNITY_ANDROID   //安卓
                "jar:file://";
#elif UNITY_IPHONE  //iPhone
		        "file://";
#else
                string.Empty;
#endif

    /// <summary>
    /// 记录所有已缓存的报价的临时id
    /// </summary>
    public List<int> cacheOfferList = new List<int>();
    /// <summary>
    /// 在配置cacheOfferList加载完成前临时记录
    /// </summary>
    public List<int> tempList = new List<int>();
    /// <summary>
    /// 写文件不需要加上前缀
    /// </summary>
    /// 
    public bool isReady = false;
    public string cachePath
    {
        get
        {
            return LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Caches/Offer/Local/OfferCaches.catalog";
        }
    }
    /// <summary>
    /// 读文件需要加上前缀 URL
    /// </summary>
    public string loadcachePath
    {
        get
        {
            return URL + LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Caches/Offer/Local/OfferCaches.catalog";
        }
    }
    public void LoadCache()
    {
        isReady = false;
        cacheOfferList.Clear();
        FileInfo info = new FileInfo(cachePath);
        if (info.Exists == false)
        {
            isReady = true;
            return;
        }
        MyMono.MyStartCoroutine(loadText,this,loadcachePath);
    }
    IEnumerator loadText(object [] objs)
    {
        string url = objs[0].ToString() ;
        WWW www = new WWW(url);

        yield return www;

        if (string.IsNullOrEmpty(www.error) !=true )
        {
            Debug.LogError(www.error);
        }
        else
        {
            string text = www.text;
            /// www 读取 第一个字符读取错误，做一个处理
            if (text.Length > 0)
            {
                char num0 = text[0];
                if (num0 < 48 || num0 > 57)
                {
                    if (text.Length == 1)
                    {
                        text = "";
                    }
                    else {
                        text = text.Substring(1);
                    }
                }
            }
            else
            {
                //Debug.LogError(url + "加载到文件string.IsNullOrEmpty(text) == true");
            }
            string[] urls = text.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            int temp ;
            for (int i = 0; i < urls.Length; i++)
            {
                if (int.TryParse(urls[i], out temp))
                {
                    if (cacheOfferList.IndexOf(temp) == -1)
                    {
                        cacheOfferList.Add(temp);
                    }
                }
                else
                {
                    Debug.LogError("转换失败 ：" + temp );
                }
            }
        }
        bool changed = false;

        for (int i = 0; i < cacheOfferList.Count; i++)
        {
            string tempUrl = getWriteUrl(cacheOfferList[i]);
            FileInfo info = new FileInfo(tempUrl);
            if (info.Exists == false )
            {
                cacheOfferList.RemoveAt(i);
                i--;
                changed = true;
            }
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            if (cacheOfferList.IndexOf(tempList[i]) != -1)
            {
                cacheOfferList.Add(tempList[i]);
                changed = true;
            }
        }

        if (changed == true )
        {
            OnCacheChange();
        }

        isReady = true;

        this.dispatchEvent(new MyCacheEvent(MyCacheEvent.loadReady));
    }
    private void OnCacheChange()
    {
        FileInfo info = new FileInfo(cachePath);
        if (info.Exists == true)
        {
            info.Delete();
        }
        if (info.Directory.Exists == false)
        {
            info.Directory.Create();
        }
        using (StreamWriter writer = new StreamWriter(cachePath, true, System.Text.Encoding.UTF8))
        {
            for (int i = 0; i < cacheOfferList.Count; i++)
            {
                writer.WriteLine(cacheOfferList[i]);
            }
        }
    }

    /// <summary>
    /// 加载路径tempId
    /// </summary>
    public string getLoadUrl(int tempId)
    {
        string url = URL + LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Offer/Local/" + tempId + ".midf";
        return url;
    }

    /// <summary>
    /// 读写路径 tempId
    /// </summary>
    public string getWriteUrl(int tempId)
    {
        string url = LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Offer/Local/" + tempId + ".midf";
        return url;
    }

    public void AddOfferCache(int tempId, string json)
    {
        string url = getWriteUrl(tempId);
        Debug.LogWarning(url);
        FileInfo info = new FileInfo(url);
        if (info.Exists == true)
        {
            info.Delete();
        }
        if (info.Directory.Exists == false)
        {
            info.Directory.Create();
        }
        using (StreamWriter writer = new StreamWriter(url, true, System.Text.Encoding.UTF8))
        {
            writer.WriteLine(json);
        }
        if (isReady)
        {
            if (cacheOfferList.IndexOf(tempId) == -1)
            {
                cacheOfferList.Add(tempId);
                OnCacheChange();
            }
        }
        else
        {
            if (tempList.IndexOf(tempId) == -1) tempList.Add(tempId);
        }
    }
    public void RemoveOfferCache(int tempId)
    {
        string url = getWriteUrl(tempId);
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("save");
        //    //url = Path.Combine(cachePath, json);
        //    return;
        //}
        FileInfo info = new FileInfo(url);
        if (info.Exists == true)
        {
            info.Delete();
        }

        if (isReady)
        {
            if (cacheOfferList.IndexOf(tempId) != -1)
            {
                cacheOfferList.Remove(tempId);
                OnCacheChange();
            }
        }
        else
        {
            if (cacheOfferList.IndexOf(tempId) != -1)
            {
                tempList.Remove(tempId);
            }
        }

    }
    public void ClearOfferCache()
    {
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("Clear");
        //    return;
        //}
        for (int i = 0; i < cacheOfferList.Count; i++)
        {
            int tempId = cacheOfferList[i];
            string url = getWriteUrl(tempId);

            FileInfo info = new FileInfo(url);
            if (info.Exists == true)
            {
                info.Delete();
            }
        }
        cacheOfferList.Clear();
        FileInfo cacheinfo = new FileInfo(cachePath);
        if (cacheinfo.Exists == true)
        {
            cacheinfo.Delete();
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            int tempId = tempList[i];
            string url = getWriteUrl(tempId);

            FileInfo info = new FileInfo(url);
            if (info.Exists == true)
            {
                info.Delete();
            }
        }
        tempList.Clear();
    }
    public bool HasCached(int tempId)
    {
        if (isReady)
        {
            if (cacheOfferList.IndexOf(tempId) != -1)
            {
                return true;
            }
            else
            {
                string url = getWriteUrl(tempId);
                FileInfo info = new FileInfo(url);
                if (info.Exists == true)
                {
                    cacheOfferList.Add(tempId);
                    return true;
                }
            }
        }
        else
        {
            if (tempList.IndexOf(tempId) != -1)
            {
                return true;
            }
        }
        return false;
    }

    public int GetNewLocalcacheTempId()
    {
        int newtempId = -1;
        for (int i = 0; i < cacheOfferList.Count; i++)
        {
            if (cacheOfferList[i] > newtempId)
            {
                newtempId = cacheOfferList[i];
            }
        }
        newtempId++;
        return newtempId;
    }

}
