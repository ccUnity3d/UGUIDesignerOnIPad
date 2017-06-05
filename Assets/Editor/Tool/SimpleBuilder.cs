using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SimpleBuilder {
    
    public static readonly string PackEnd = ".assetbundle";

    [MenuItem("Assets/BuildeAsset", false, 0)]
    [MenuItem("Tools/BuildeSelectAsset", false, 1)]
    public static void SimpleBuildeSelectAsset()
    {
        BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

        BuildTarget target = BuildTarget.WebPlayer;
        System.Action<List<Object>, BuildTarget, string> action = null;
        bool cancel = SelectType("选中", ref target, ref action);

        if (cancel)
        {
            return;
        }
        Object[] objs = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);
        List<Object> selectList = new List<Object>();
        for (int i = 0; i < objs.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(objs[i]);
            if (path.IndexOf(".") != -1)
            {
                selectList.Add(objs[i]);
                continue;
            }
            if (path.IndexOf("Assets/Prefabs/") == -1)
            {
                Debug.LogWarning("所选文件" + path + "不在可打包目录Assets/Prefabs/");
                continue;
            }
            List<Object> list = FileTool.GetObjects(path.Replace("Assets/Prefabs/", ""), "*.*", Application.dataPath + "/Prefabs/");
            selectList.InsertRange(0, list);
        }
        if (selectList.Count == 0)
        {
            return;
        }

        action(selectList, target, "");

        EditorUserBuildSettings.SwitchActiveBuildTarget(activeBuildTarget);
    }

    [MenuItem("Tools/BuildeAllAsset", false, 1)]
    public static void SimpleBuildeAllAsset()
    {
        BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

        BuildTarget target = BuildTarget.WebPlayer;
        System.Action<List<Object>, BuildTarget, string> action = null;
        bool cancel = SelectType("所有", ref target, ref action);
        if (cancel)
        {
            return;
        }

        SetBuild(action, "UI/panel/", "*.prefab", Application.dataPath + "/Prefabs/", target);
        //SetBuild(action, "2D/texture2D/", "*.png", Application.dataPath + "/Prefabs/", target);
        SetBuild(action, "2D/prefab/", "*.prefab", Application.dataPath + "/Prefabs/", target);
        SetBuild(action, "3D/texture2D/", "*.jpg", Application.dataPath + "/Prefabs/", target);
        //SetBuild(action, "3D/prefab/", "*.prefab", Application.dataPath + "/Prefabs/", target);

        EditorUserBuildSettings.SwitchActiveBuildTarget(activeBuildTarget);
        //List<Object> uiPanels = FileTool.GetObjects("UI/panel/", "*.prefab", Application.dataPath + "/Prefabs/");
        //List<Object> modelTextures = FileTool.GetObjects("UI/texture2D/", "*.jpg", Application.dataPath + "/Prefabs/");
        //List<Object> view2DTextures = FileTool.GetObjects("2D/texture2D/", "*.png", Application.dataPath + "/Prefabs/");
        //List<Object> models = FileTool.GetObjects("2D/prefab/", "*.prefab", Application.dataPath + "/Prefabs/");
        //action(uiPanels, target, "UI/panel/");
        //action(modelTextures, target, "UI/texture2D/");
        //action(view2DTextures, target, "2D/texture2D/");
        //action(models, target, "2D/prefab/");
    }

    private static void SetBuild(System.Action<List<Object>, BuildTarget, string> action, string fold, string endings, string rootpath, BuildTarget target)
    {
        List<Object> objs = FileTool.GetObjects(fold, endings, rootpath);
        action(objs, target, fold);
    }

    public static bool SelectType(string type, ref BuildTarget target, ref System.Action<List<Object>, BuildTarget, string> action)
    {
        bool pack = EditorUtility.DisplayDialog("打包", "打包" + type + "资源", "确定", "取消");
        if (pack == false)
        {
            return true;
        }

        target = Application.platform == RuntimePlatform.IPhonePlayer ? BuildTarget.iOS : BuildTarget.WebPlayer;
        int playformIndex = EditorUtility.DisplayDialogComplex("打包", "打包" + type + "资源", "IOS", "Android", "Web");
        target = playformIndex == 0 ? BuildTarget.iOS : playformIndex == 1 ? BuildTarget.Android : BuildTarget.WebPlayer;

        bool build = EditorUtility.DisplayDialog("打包", "打包" + type + "资源", "SimpleBuild", "取消");
        if (build == false)
        {
            return true;
        }
        else
        {
            action = SimpleBuild;
        }
        //else {
        //    action = ManifestBuild;
        //}
        return false;
    }

    /// <summary>
    /// unity旧的打包方式 全部打包在一起
    /// </summary>
    /// <param name="SelectedObjs"></param>
    public static void SimpleBuild(List<Object> SelectedObjs, BuildTarget target, string targetFile)
    {
        //if (target == BuildTarget.iOS && Application.platform != RuntimePlatform.IPhonePlayer)
        //{
        //    Debug.LogError("当前平台不能打包为IOS资源");
        //    return;
        //}
        string playformPath = target == BuildTarget.iOS ? "IOS/" : target == BuildTarget.Android == true ? "Android/" : "Web/";
        //EditorUserBuildSettings.SwitchActiveBuildTarget(target);
        //UnityEditor.Sprites.Packer.RebuildAtlasCacheIfNeeded(target, true);
        for (int i = 0; i < SelectedObjs.Count; i++)
        {
            Object obj = SelectedObjs[i];
            string path = AssetDatabase.GetAssetPath(obj);
            if (targetFile == "") {
                targetFile = path.Replace("Assets/Prefabs/", "");
                targetFile = targetFile.Split(new string[] { obj.name + "." }, System.StringSplitOptions.RemoveEmptyEntries)[0];
            }
            string file = Application.dataPath + "/StreamingAssets/" + playformPath + targetFile;
            FileInfo fi = new FileInfo(file);
            DirectoryInfo di = fi.Directory;
            if (di.Exists == false)
            {
                Directory.CreateDirectory(di.FullName);
            }
            string targetPath = file + obj.name + PackEnd;
            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies, target))
            {
                Debug.Log(obj.name + target + "资源打包成功!");
            }
            else {
                Debug.Log(obj.name + target + "资源打包失败!");
            }
            AssetDatabase.Refresh();
        }
    }

    ///// <summary>
    ///// unity5.0以上新的打包方式 关联打包 depends完全分开靠索引完成组建
    ///// </summary>
    ///// <param name="SelectedObjs"></param>
    //public static void ManifestBuild(List<Object> SelectedObjs, BuildTarget target, string targetFold)
    //{
    //    if (target == BuildTarget.iOS && Application.platform != RuntimePlatform.IPhonePlayer)
    //    {
    //        Debug.LogError("当前平台不能打包为IOS资源");
    //        return;
    //    }
    //    AssetBundleBuild[] builds = new AssetBundleBuild[1];
    //    string playformPath = target == BuildTarget.iOS ? "IOS/" : target == BuildTarget.Android == true ? "Android/" : "Web/";
    //    for (int i = 0; i < SelectedObjs.Count; i++)
    //    {
    //        Object obj = SelectedObjs[i];
    //        string path = AssetDatabase.GetAssetPath(obj);
    //        if (targetFold == "")
    //        {
    //            targetFold = path.Replace("Assets/Prefabs/", "");
    //            targetFold = targetFold.Split(new string[] { obj.name + "." }, System.StringSplitOptions.RemoveEmptyEntries)[0];
    //        }
    //        string folder = Application.dataPath + "/StreamingAssets/" + playformPath + targetFold;
    //        FileInfo fi = new FileInfo(folder);
    //        DirectoryInfo di = fi.Directory;
    //        if (di.Exists == false)
    //        {
    //            Directory.CreateDirectory(di.FullName);
    //        }
    //        //string targetPath = folder + obj.name + ".assetbundle";

    //        string[] depends = AssetDatabase.GetDependencies(new string[] { path });
    //        AssetBundleBuild build = new AssetBundleBuild();
    //        build.assetBundleName = obj.name + PackEnd;
    //        build.assetNames = depends;
    //        builds[0] = build;

    //        string totalPath = folder + obj.name + PackEnd;
    //        try
    //        {
    //            BuildPipeline.BuildAssetBundles(folder, builds,BuildAssetBundleOptions.None, target);
    //        }
    //        catch (System.Exception e)
    //        {
    //            Debug.Log("失败:" + e.Message);
    //        }
    //        finally
    //        {
    //            Debug.Log("成功:" + totalPath);
    //        }
    //        string manifestPath = totalPath + ".manifest";
    //        if (File.Exists(manifestPath))
    //        {
    //            File.Delete(manifestPath);
    //        }
    //        string file = Path.GetDirectoryName(totalPath);
    //        file = Path.GetFileNameWithoutExtension(file);
    //        if (File.Exists(folder + file))
    //        {
    //            File.Delete(folder + file);
    //        }
    //        if (File.Exists(folder + file + ".manifest"))
    //        {
    //            File.Delete(folder + file + ".manifest");
    //        }

    //        AssetDatabase.Refresh();
    //    }

    //}    
}
