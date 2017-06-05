using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// SimpleOutterLoader 缓存
/// </summary>
public class CacheModelManager : Singleton<CacheModelManager> {

    static public readonly string LocalURL = Application.persistentDataPath + "/";

    static public readonly string URL =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_WEBPLAYER
                "file://";
#elif UNITY_ANDROID   //安卓
                "jar:file://";
#elif UNITY_IPHONE  //iPhone
		        "file://";
#else
                string.Empty;
#endif

    public List<string> cacheList = new List<string>();

    public List<string> tempAdd = new List<string>();

    public string cachePath
    {
        get {
            return LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Caches/modelCaches.catalog";
        } 
    }

    public string loadcachePath
    {
        get
        {
            return URL + LocalURL + "alluserdata/" + UnityIOSMsg.currentUser.uuid + "/Caches/Scheme/Local/SchemeCaches.catalog";
        }
    }

    private bool isReady = false;

    public void LoadCache()
    {
        isReady = false;
        cacheList.Clear();
        tempAdd.Clear();
        MyMono.MyStartCoroutine(LoadTxt, this, loadcachePath);
    }

    private IEnumerator LoadTxt(object[] objs)
    {
        string url = objs[0].ToString();
        WWW www = new WWW(url);
        yield return www;
        cacheList.Clear();
        if (string.IsNullOrEmpty(www.error) == false)
        {
            //Debug.LogError(www.error);
        }
        else
        {
            string text = www.text;
            if (text.Length > 0)
            {
                char num0 = text[0];
                if (num0 < 32 || num0 > 126)
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
            Debug.LogWarning(text);
            string[] urls = text.Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < urls.Length; i++)
            {
                if (cacheList.IndexOf(urls[i]) == -1) cacheList.Add(urls[i]);
                else { Debug.LogError("重复缓存url：" + urls[i]); }
            }
        }
        isReady = true;

        bool changed = false;
        for (int i = 0; i < cacheList.Count; i++)
        {
            FileInfo info = new FileInfo(cacheList[i]);
            if (info.Exists == false)
            {
                cacheList.RemoveAt(i);
                changed = true;
                i--;
            }
        }
        for (int i = 0; i < tempAdd.Count; i++)
        {
            if (cacheList.IndexOf(tempAdd[i]) == -1)
            {
                cacheList.Add(tempAdd[i]);
                changed = true;
            }
        }

        if (changed == true)
        {
            OnListChange();
        }
    }

    public void AddCache(string url, byte[] bytes)
    {
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("save:" + url);
        //    return;
        //}

        FileInfo t = new FileInfo(url);
        if (t.Exists)
        {
            File.Delete(url);
        }
        else if (t.Directory.Exists == false)
        {
            t.Directory.Create();
        }

        Debug.Log("save:" + url);
        using (FileStream stream = File.Open(url, FileMode.OpenOrCreate))
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        if (isReady == true)
        {
            if (HaseCached(url) == false)
            {
                cacheList.Add(url);
                OnListChange();
            }
        }
        else
        {
            if (tempAdd.IndexOf(url) != -1) tempAdd.Add(url);
        }
    }

    public void ClearCache()
    {
        if (isReady == true)
        {
            for (int i = 0; i < cacheList.Count; i++)
            {
                string url = cacheList[i];
                FileInfo file = new FileInfo(url);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            cacheList.Clear();
        }
        else
        {
            for (int i = 0; i < tempAdd.Count; i++)
            {
                string url = tempAdd[i];
                FileInfo file = new FileInfo(url);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            tempAdd.Clear();
        }

        {
            string url = cachePath;
            FileInfo file = new FileInfo(url);
            if (file.Exists)
            {
                file.Delete();
            }
        }
        //GC.Collect();
    }

    /// <summary>
    /// 移除指定缓存
    /// </summary>
    public void RemoveCache(string url)
    {
        FileInfo t = new FileInfo(url);
        if (t.Exists)
        {
            t.Delete();
        }

        if (isReady == true)
        {
            if (HaseCached(url))
            {
                cacheList.Remove(url);
                OnListChange();
            }
        }
        else
        {
            if (tempAdd.IndexOf(url) != -1) tempAdd.Remove(url);
        }
    }

    public bool HaseCached(string url)
    {
        if (isReady == true)
        {
            if (cacheList.IndexOf(url) != -1)
            {
                return true;
            }
            FileInfo info = new FileInfo(url);
            if (info.Exists)
            {
                cacheList.Add(url);
                return true;
            }
        }
        else
        {
            FileInfo t = new FileInfo(url);
            if (t.Exists)
            {
                if (tempAdd.IndexOf(url) == -1)
                {
                    tempAdd.Add(url);
                }
                return true;
            }
            else
            {
                if (tempAdd.IndexOf(url) != -1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void OnListChange()
    {
        string url = cachePath;
        //if (Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.Log("saveChanged:" + url);
        //    return;
        //}

        FileInfo file = new FileInfo(url);
        if (file.Exists)
        {
            //file.Length;字节长度//1-M = 1024-kb，1-kb = 1024-byte;
            File.Delete(url);
        }
        else if (file.Directory.Exists == false)
        {
            file.Directory.Create();
        }

        Debug.Log("saveChanged:" + url);
        using (StreamWriter writer = new StreamWriter(url, true, System.Text.Encoding.UTF8))
        {
            for (int i = 0; i < cacheList.Count; i++)
            {
                writer.WriteLine(cacheList[i]);
            }
        }
    }
}
