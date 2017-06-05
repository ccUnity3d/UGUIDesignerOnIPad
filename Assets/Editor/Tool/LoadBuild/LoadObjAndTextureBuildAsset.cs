using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LoadObjAndTextureBuildAsset {

    
    private static BuildLoader mono;

    [MenuItem("LoadAndBuildeTools/LoadAsset", false, 1)]
    public static void LoadAsset()
    {
        EditorSceneManager.OpenScene("Assets/scene/loadScene.unity", OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("LoadAndBuildeTools/BuildeAsset", false, 1)]
    public static void BuildeAsset()
    {
        AssetDatabase.Refresh();
        string buildFilter = "Assets/Prefabs/NewFolder/i/";
        string[] directories = Directory.GetDirectories(buildFilter);
        List<GameObject> objs = new List<GameObject>();
        for (int i = 0; i < directories.Length; i++)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(directories[i] + "/model.obj");
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(directories[i] + "/model.png");
            MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>(true);
            for (int k = 0; k < renders.Length; k++)
            {
                renders[k].sharedMaterial.mainTexture = texture;
            }
            objs.Add(go);
        }
        Selection.objects = objs.ToArray();
        Debug.LogWarning("SimpleBuilder.Start");
        SimpleBuilder.SimpleBuildeSelectAsset();
        Debug.LogWarning("SimpleBuilder.End");
        //while (LoadedStack.Count > 0)
        //{
        //    ModelData data = LoadedStack.Pop();
        //    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(data.textureUrl);
        //    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(data.modelUrl);
        //    Renderer[] renders = prefab.GetComponentsInChildren<Renderer>();
        //    for (int i = 0; i < renders.Length; i++)
        //    {
        //        renders[i].material.mainTexture = texture;
        //    }
        //    Debug.Log("加载完成" + data.modelUrl);

        //    Selection.objects = new Object[] { prefab };
        //    SimpleBuilder.SimpleBuildeSelectAsset();
        //}
    }




}
