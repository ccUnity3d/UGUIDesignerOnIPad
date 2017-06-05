using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

public class ManifestBuilder {

    protected string exportPrefabFromPath = "";
    protected string exportPrefabToPath = Application.dataPath + "/StreamingAssets/" + "test";
    protected string extention = ".assetbundle";
    
    protected HashSet<string> bitmaps = new HashSet<string>();
    protected HashSet<string> shaders = new HashSet<string>();
    protected HashSet<string> materials = new HashSet<string>();
    protected HashSet<string> ttfs = new HashSet<string>();
    protected HashSet<string> fbxs = new HashSet<string>();
    protected HashSet<string> anims = new HashSet<string>();
    protected HashSet<string> prefabs = new HashSet<string>();
    protected HashSet<string> controllers = new HashSet<string>();

    protected Dictionary<string, HashSet<string>> toUnityPathMapping = new Dictionary<string, HashSet<string>>();
    protected Dictionary<string, int> prefabDirMaping = new Dictionary<string, int>();

    public ManifestBuilder()
    {
        toUnityPathMapping.Add("bitmap", bitmaps);
        //toUnityPathMapping.Add("shaders", shaders);
        //toUnityPathMapping.Add("materials", materials);
        toUnityPathMapping.Add("ttfs", ttfs);
        toUnityPathMapping.Add("fbxs", fbxs);
        toUnityPathMapping.Add("anims", anims);
        toUnityPathMapping.Add("prefabs", prefabs);
        toUnityPathMapping.Add("controllers", controllers);
    }
    
    //public bool exprotPrefabs(string FromPath, List<string> prefabPathList, List<BuildTarget> buildTargets)
    //{
    //    clearCache();
    //    DirectoryInfo directoryInfo = new DirectoryInfo(FromPath);
    //    string directoryName = directoryInfo.Name;

    //    int len = prefabPathList.Count;
    //    List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
    //    for (int i = 0; i < len; i++)
    //    {
    //        List<AssetBundleBuild> tempList = collectionPrefab(prefabPathList[i]);
    //        if (tempList.Count > 0)
    //        {
    //            builds.AddRange(tempList);
    //        }
    //    }

    //    //内部包含其它的prefab;
    //    int prefabCount = prefabs.Count;
    //    while (prefabCount > 0)
    //    {
    //        string[] tempPrefabs = prefabs.ToArray();
    //        prefabs.Clear();

    //        foreach (string item in tempPrefabs)
    //        {
    //            if (prefabPathList.IndexOf(item) != -1)
    //            {
    //                continue;
    //            }

    //            List<AssetBundleBuild> tempList = collectionPrefab(item);
    //            if (tempList.Count > 0)
    //            {
    //                builds.AddRange(tempList);
    //            }
    //        }

    //        prefabCount = prefabs.Count;
    //    }

    //    List<AssetBundleBuild> bitmapBuilds = new List<AssetBundleBuild>();

    //    foreach (string key in toUnityPathMapping.Keys)
    //    {
    //        HashSet<string> itemList = toUnityPathMapping[key];
    //        foreach (string item in itemList)
    //        {
    //            AssetBundleBuild build = new AssetBundleBuild();
    //            string fileName = Path.GetFileNameWithoutExtension(item);
    //            string assetBundleName = directoryName + "_" + key + "/" + fileName + ".unity3d";

    //            int hasIndex;
    //            if (prefabDirMaping.TryGetValue(assetBundleName, out hasIndex))
    //            {
    //                prefabDirMaping[assetBundleName] = hasIndex + 1;
    //                assetBundleName = directoryName + "_" + key + (hasIndex + 1) + "/" + fileName + ".unity3d";
    //            }
    //            else
    //            {
    //                prefabDirMaping.Add(assetBundleName, 0);
    //            }

    //            build.assetBundleName = assetBundleName;
    //            build.assetNames = new string[] { item };
    //            builds.Add(build);
    //        }
    //    }

    //    try
    //    {
    //        foreach (BuildTarget buildTarget in buildTargets)
    //        {
    //            string exportPlatformRootPath = Path.Combine(exportPrefabToPath,
    //                buildTarget.ToString() + "/" + directoryName);
    //            if (Directory.Exists(exportPlatformRootPath) == false)
    //            {
    //                Directory.CreateDirectory(exportPlatformRootPath);
    //            }

    //            BuildPipeline.BuildAssetBundles(exportPlatformRootPath, builds.ToArray(),
    //                BuildAssetBundleOptions.None, buildTarget);
    //        }

    //    }
    //    catch (Exception e)
    //    {
    //        Debug.Log("失败:" + e.Message);
    //        return false;
    //    }
    //    finally
    //    {
    //        clearCache();
    //    }

    //    return true;
    //}

    //protected List<AssetBundleBuild> collectionPrefab(string prefabPath)
    //{
    //    string[] depends = AssetDatabase.GetDependencies(new string[] { prefabPath });
    //    List<string> innerList = new List<string>();

    //    List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
    //    foreach (string depend in depends)
    //    {


    //        string extension = Path.GetExtension(depend).ToLower();
    //        switch (extension)
    //        {
    //            case ".jpg":
    //            case ".png":
    //                if (depend == prefabPath)
    //                {
    //                    innerList.Add(depend);
    //                }
    //                else
    //                {
    //                    bitmaps.Add(depend);
    //                }
    //                break;

    //            case ".shader":
    //                shaders.Add(depend);
    //                break;
    //            case ".dll":
    //                break;

    //            case ".cs":
    //                break;

    //            case ".ttf":
    //                ttfs.Add(depend);
    //                break;
    //            case ".mat":
    //                materials.Add(depend);
    //                break;
    //            case ".fbx":
    //                fbxs.Add(depend);
    //                break;

    //            case ".anim":
    //                anims.Add(depend);
    //                break;

    //            case ".controller":
    //                controllers.Add(depend);
    //                break;
    //            case ".prefab":
    //                if (depend != prefabPath)
    //                {
    //                    prefabs.Add(depend);
    //                }
    //                else
    //                {
    //                    innerList.Add(depend);
    //                }
    //                break;

    //            default:
    //                innerList.Add(depend);
    //                break;
    //        }
    //    }

    //    AssetBundleBuild build = new AssetBundleBuild();
    //    string fileName = Path.GetFileNameWithoutExtension(prefabPath);
    //    build.assetBundleName = fileName + ".unity3d";
    //    build.assetNames = innerList.ToArray();

    //    builds.Add(build);

    //    return builds;
    //}



    public bool exprotPrefabs(string FromPath, List<string> prefabPathList, List<BuildTarget> buildTargets)
    {
        clearCache();
        DirectoryInfo directoryInfo = new DirectoryInfo(FromPath);
        string directoryName = directoryInfo.Name;

        int len = prefabPathList.Count;
        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        for (int i = 0; i < len; i++)
        {
            string bundleName =  Path.GetFileNameWithoutExtension(prefabPathList[i]);
            List<AssetBundleBuild> tempList = collectionPrefab(prefabPathList[i], bundleName);
            if (tempList.Count > 0)
            {
                builds.AddRange(tempList);
            }
        }
        
        try
        {
            foreach (BuildTarget buildTarget in buildTargets)
            {
                string exportPlatformRootPath = Path.Combine(exportPrefabToPath,
                    buildTarget.ToString() + "/" + directoryName);
                if (Directory.Exists(exportPlatformRootPath) == false)
                {
                    Directory.CreateDirectory(exportPlatformRootPath);
                }

                BuildPipeline.BuildAssetBundles(exportPlatformRootPath, builds.ToArray(),
                    BuildAssetBundleOptions.None, buildTarget);
            }

        }
        catch (Exception e)
        {
            Debug.Log("失败:" + e.Message);
            return false;
        }
        finally
        {
            clearCache();
        }

        return true;
    }

    protected List<AssetBundleBuild> collectionPrefab(string prefabPath, string bundleName)
    {
        string[] depends = AssetDatabase.GetDependencies(new string[] { prefabPath });
        List<string> innerList = new List<string>();

        List<AssetBundleBuild> builds = new List<AssetBundleBuild>();
        innerList.AddRange(depends);


        AssetBundleBuild build = new AssetBundleBuild();
        string fileName = Path.GetFileNameWithoutExtension(prefabPath);
        build.assetBundleName = fileName + ".unity3d";
        build.assetNames = innerList.ToArray();

        builds.Add(build);

        return builds;
    }

    private void clearCache()
    {
        bitmaps.Clear();
        shaders.Clear();
        materials.Clear();
        prefabs.Clear();
        ttfs.Clear();
        fbxs.Clear();
        anims.Clear();
        controllers.Clear();
    }

}
