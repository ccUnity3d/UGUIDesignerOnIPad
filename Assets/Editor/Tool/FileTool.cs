using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class FileTool {

    //private static string RootPath = "Assets/Prefab/assetBundle/";

    public static FileInfo[] GetFileInfo(string baseUri, string endings, string RootPath = "Assets/Prefab/assetBundle/")
    {
        string str = RootPath + baseUri;
        DirectoryInfo dirInfo = new DirectoryInfo(str);
        FileInfo[] fileInfoArr = dirInfo.GetFiles(endings, SearchOption.AllDirectories);
        return fileInfoArr;
    }

    public static List<UnityEngine.Object> GetObjects(string baseUri, string endings, string RootPath = "Assets/Prefab/assetBundle/")
    {
        List<UnityEngine.Object> list = new List<UnityEngine.Object>();
        FileInfo[] fileInfoArr = GetFileInfo(baseUri, endings, RootPath);
        for (int i = 0; i < fileInfoArr.Length; i++)
        {
            string fileFullPath = fileInfoArr[i].FullName.Replace("\\", "/");
            fileFullPath = RootPath + fileFullPath.Split(new string[] { RootPath }, StringSplitOptions.None)[1];
            string relativePath = "Assets" + fileFullPath.Split(new string[] { Application.dataPath }, StringSplitOptions.None)[1];
            UnityEngine.Object buildObj = AssetDatabase.LoadMainAssetAtPath(relativePath);
            if (buildObj != null)
            {
                UnityEngine.Object obj = buildObj as UnityEngine.Object;
                if (obj != null)
                {
                    list.Add(obj);
                }
            }
        }
        return list;
    }

    public static List<UnityEngine.Object> GetGameObjects(string[] baseUris, string endings, string RootPath = "Assets/Prefab/assetBundle/")
    {
        List<UnityEngine.Object> list = new List<UnityEngine.Object>();
        for (int i = 0; i < baseUris.Length; i++)
        {
            List<UnityEngine.Object> temp = GetObjects(baseUris[i], endings, RootPath);
            for (int j = 0; j < temp.Count; j++)
            {
                list.Add(temp[j]);
            }
        }
        return list;
    }

    public static List<T> GetComponent<T>(string[] baseUri, string endings, string RootPath = "Assets/Prefab/assetBundle/") where T : Component
    {
        List<T> list = new List<T>();

        for (int m = 0; m < baseUri.Length; m++)
        {
            List<UnityEngine.Object> temp = GetObjects(baseUri[m], endings);
            for (int i = 0; i < temp.Count; i++)
            {
                T[] ParticleSystems = (temp[i] as GameObject).GetComponentsInChildren<T>(true);
                if (ParticleSystems != null && ParticleSystems.Length != 0)
                {
                    for (int j = 0; j < ParticleSystems.Length; j++)
                    {
                        list.Add(ParticleSystems[j]);
                    }
                }
            }
        }
        
        return list;
    }


    public static void PackAssetToPath(object obj, string assetFullPath,string name)
    {
        try
        {
            string path=assetFullPath + name + ".asset";

            if(File.Exists(path)){
                File.Delete(path);
            }

            FileStream fs = new FileStream(path, FileMode.Create);
            
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(fs, obj);

            UnityEngine.Object uobj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));

            Selection.objects = new UnityEngine.Object[] { uobj };

            Debug.Log("生成 Asset 文件：" + assetFullPath + name + ".asset");
        }
        catch (System.Exception e)
        {
            Debug.Log("生成文件失败！" + assetFullPath + name + ".asset" + "，原因：" + e);
        }

    }


}
