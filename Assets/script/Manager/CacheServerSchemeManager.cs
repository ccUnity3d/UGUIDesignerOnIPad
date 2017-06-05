using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


public class CacheServerSchemeManager : Singleton<CacheServerSchemeManager>
{

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

    public bool isReady = false;

    /// <summary>
    /// 记录所有已缓存的服务器数据库id
    /// </summary>
    public List<string> cacheSchemeList = new List<string>();

    /// <summary>
    /// 在配置cacheSchemeList加载完成前临时记录
    /// </summary>
    public List<string> tempList = new List<string>();

    public virtual string cachePath
    {
        get
        {
            return LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Caches/Scheme/Server/SchemeCaches.catalog";
        }
    }

    public string loadcachePath
    {
        get
        {
            return URL + LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Caches/Scheme/Local/SchemeCaches.catalog";
        }
    }

    /// <summary>
    /// 在打开方案时候加载 本地保存方案json
    /// </summary>
    public void LoadCache()
    {
        isReady = false;
        cacheSchemeList.Clear();

        FileInfo info = new FileInfo(cachePath);
        if (info.Exists == false)
        {
            isReady = true;
            return;
        }
        MyMono.MyStartCoroutine(LoadText, this, loadcachePath);
    }
    /// <summary>
    /// 加载缓存路径列表
    /// </summary>
    private IEnumerator LoadText(object[] objs)
    {
        string url = objs[0].ToString();
        WWW www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error) == false)
        {
            //返回一个错误消息，在下载期间如果产生了一个错误的话www.error !=null
        }
        else
        {
            string text = www.text;
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
            for (int i = 0; i < urls.Length; i++)
            {
                if (cacheSchemeList.IndexOf(urls[i]) == -1)
                {
                    cacheSchemeList.Add(urls[i]);
                }
            }

        }
        bool changed = false;
        for (int i = 0; i < cacheSchemeList.Count; i++)
        {
            string tempurl = cacheSchemeList[i];
            FileInfo info = new FileInfo(tempurl);
            if (info.Exists == false)
            {
                cacheSchemeList.RemoveAt(i);
                changed = true;
                i--;
            }
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            if (cacheSchemeList.IndexOf(tempList[i]) != -1)
            {
                cacheSchemeList.Add(tempList[i]);
                changed = true;
            }
        }
        if (changed == true)
        {
            OnCacheChange();
        }

        isReady = true;
        this.dispatchEvent(new MyCacheEvent(MyCacheEvent.loadReady));
    }

    /// <summary>
    /// 把方案Json 保存到这里
    /// </summary>
    /// <param name="url"></param>
    public void AddSchemeCache(string serverId, string json)
    {
        string url = getWriteUrl(serverId);
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("AddSchemeCache");
        //    //url = Path.Combine(cachePath, json);
        //    return;
        //}

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
            if (cacheSchemeList.IndexOf(serverId) == -1)
            {
                cacheSchemeList.Add(serverId);
                OnCacheChange();
            }
        }
        else
        {
            if (tempList.IndexOf(serverId) == -1) tempList.Add(serverId);
        }
    }
    /// <summary>
    /// 清理具体方案对应的JSON
    /// </summary>
    public void RemoveSchemeCache(string serverId)
    {
        string url = getWriteUrl(serverId);
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("RemoveSchemeCache");
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
            if (cacheSchemeList.IndexOf(serverId) != -1)
            {
                cacheSchemeList.Remove(serverId);
                OnCacheChange();
            }
        }
        else
        {
            if (cacheSchemeList.IndexOf(serverId) != -1)
            {
                tempList.Remove(serverId);
            }
        }

    }

    /// <summary>
    /// 清空 缓存
    /// </summary>
    public void ClearSchemeCache()
    {
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("Clear");
        //    return;
        //}
        for (int i = 0; i < cacheSchemeList.Count; i++)
        {
            string serverId = cacheSchemeList[i];
            string url = getWriteUrl(serverId);

            FileInfo info = new FileInfo(url);
            if (info.Exists == true)
            {
                info.Delete();
            }
        }
        cacheSchemeList.Clear();
        FileInfo cacheinfo = new FileInfo(cachePath);
        if (cacheinfo.Exists == true)
        {
            cacheinfo.Delete();
        }

        for (int i = 0; i < tempList.Count; i++)
        {
            string serverId = tempList[i];
            string url = getWriteUrl(serverId);

            FileInfo info = new FileInfo(url);
            if (info.Exists == true)
            {
                info.Delete();
            }
        }
        tempList.Clear();
    }

    public void OnCacheChange()
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
            for (int i = 0; i < cacheSchemeList.Count; i++)
            {
                writer.WriteLine(cacheSchemeList[i]);
            }
        }
    }

    public string getLoadUrl(string serverId)
    {
        string url = URL + LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Scheme/Server/" + serverId + ".midf";
        return url;
    }

    public string getWriteUrl(string serverId)
    {
        string url = LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Scheme/Server/" + serverId + ".midf";
        return url;
    }

    public bool HasCached(string serverId)
    {
        if (isReady)
        {
            if (cacheSchemeList.IndexOf(serverId) != -1)
            {
                return true;
            }
            else
            {
                string url = getWriteUrl(serverId);
                FileInfo info = new FileInfo(url);
                if (info.Exists == true)
                {
                    cacheSchemeList.Add(serverId);
                    return true;
                }
            }
        }
        else
        {
            if (tempList.IndexOf(serverId) != -1)
            {
                return true;
            }
        }
        return false;
    }
}

