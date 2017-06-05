using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildLoader : MonoBehaviour
{
    private Stack<ModelData> stack = new Stack<ModelData>();
    private int count;

    void Awake()
    {
        ModelData data = new ModelData();
        StartCoroutine(data.LoadTexture());
       //string url = "File://D:/GitProjects/mi-mobile-3d-tool/UGUIDesignerOnIPad/Assets/Prefabs/NewFolder/all_products.txt";
       //StartCoroutine(LoadTxt(url));
    }
    
    private IEnumerator LoadTxt(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        if (string.IsNullOrEmpty(www.error) == false)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string text = www.text;
            Debug.LogWarning(text);
            string[] urls = text.Split(new string[] {"\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            //for (int i = 0; i < urls.Length; i++)
            //{
            //    string Head = "http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/bb4f723e-c1e2-4ebb-b4b7-7824cd9d2dbe/model.obj"+ urls[i] + "/";
            //    ModelData data = new ModelData();
            //    data.modelUrl = Head + "model.obj";
            //    data.textureUrl = Head + "model.png";
            //    data.End = Load;
            //    data.mono = this;
            //    stack.Push(data);
            //}
            Load(3);
        }
    }

    private void Load(int count)
    {
        this.count = count;
        for (int i = 0; i < count; i++)
        {
            Load();
        }
    }

    void Load()
    {
        if (stack.Count == 0)
        {
            count--;
            if (count == 0)
            {
                Debug.Log("完成");
            }
            return;
        }
        ModelData data = stack.Pop();
        
        data.Load();
    }

}


public class ModelData
{
    public string modelUrl = "http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/b63a5fcb-607c-4aab-87ec-bbe89e9ce676/model.obj";
    public string textureUrl = "http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/b63a5fcb-607c-4aab-87ec-bbe89e9ce676/Top.png";//"http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/b63a5fcb-607c-4aab-87ec-bbe89e9ce676/model.png";
    public Action End;

    string serverUri = "http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/";
    string localUri = "D:/GitProjects/mi-mobile-3d-tool/UGUIDesignerOnIPad/Assets/Prefabs/NewFolder/";//file://D:/GitProjects/mi-mobile-3d-tool/UGUIDesignerOnIPad/
    internal BuildLoader mono;

    public void Load()
    {
        mono.StartCoroutine(LoadModel());
    }

    public IEnumerator LoadModel()
    {
        WWW www = new WWW(modelUrl);
        yield return www;
        if (string.IsNullOrEmpty(www.error) == false)
        {
            Debug.LogError(www.error);
            End();
        }
        else
        {
            string localUrl = modelUrl.Replace(serverUri, localUri);
            byte[] bytes = www.bytes;

            string folder = localUrl.Replace("model.obj", "");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(localUrl))
            {
                File.Delete(localUrl);
            }

            using (FileStream stream = File.Open(localUrl, FileMode.OpenOrCreate))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            //AssetDatabase.Refresh();
            mono.StartCoroutine(LoadTexture());
        }
    }
    public IEnumerator LoadTexture()
    {
        WWW www = new WWW(textureUrl);
        yield return www;
        if (string.IsNullOrEmpty(www.error) == false)
        {
            Debug.LogError(www.error);
            //End();
        }
        else
        {
            string localUrl = textureUrl.Replace(serverUri, localUri);
            //byte[] bytes = www.bytes;
            Texture2D pic = www.texture;
            byte[] bytes = pic.GetRawTextureData();
            string folder = Path.GetDirectoryName(localUrl);
            if (File.Exists(localUrl))
            {
                File.Delete(localUrl);
            }
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            using (FileStream stream = File.Open(localUrl, FileMode.OpenOrCreate))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
            //AssetDatabase.Refresh();
            //LoadedStack.Push(this);
            //End();
        }
    }
}