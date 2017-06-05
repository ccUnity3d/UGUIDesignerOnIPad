using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class SetTextureTool {

    private static SetTextureTool instance;
    public static SetTextureTool Instance
    {
        get {
            if (instance == null) instance = new SetTextureTool();
            return instance;
        }
    }

    protected ResourcesPool pool
    {
        get { return ResourcesPool.Instance; }
    }

    public static void SetTexture(GameObject go, string path, string isLocal)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("SetTexture(path = " + path+")");
            return;
        }
        Instance.setTexture(go, path, isLocal);
    }

    public void setTexture(GameObject go, string path, string isLocal)
    {
        if (isLocal == "local")
        {
            LoaderPool.InnerLoad(path, SimpleLoadDataType.texture2D, onLoaded, go);
        }
        else
        {
            LoaderPool.OutterLoad(path, SimpleLoadDataType.texture2D, onLoaded, go);
        }
    }

    private void onLoaded(object obj)
    {
        SimpleLoader loader = obj as SimpleLoader;
        Texture2D texture = loader.loadedData as Texture2D;
        RawImage image = loader.bringData as RawImage;
        if (image != null)
        {
            image.texture = texture;
            //Debug.LogWarning(texture.name);
            return;
        }
        GameObject go = loader.bringData as GameObject;
        if (go != null)
        {
            //Debug.LogWarning("GOGOGO" + texture.name + " " + texture.width + "/" + texture.height);
            //Vector3 size = Vector3.one * 0.01f;
            //size.x *= texture.width;
            //size.y *= texture.height;
            //go.transform.localScale = size;
            go.GetComponent<Renderer>().material.mainTexture = texture;
            return;
        }

    }

    public static void SetTexture(RawImage go, string path, string isLocal)
    {
        Instance.setTexture(go, path, isLocal);
    }
    public void setTexture(RawImage go, string path, string isLocal)
    {
        if (go == null)
        {
            Debug.LogWarning("setTexture RawImage go == null");
            return;
        }
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogWarning("setTexture path == null");
            return;
        }

        if (isLocal == "local")
        {
            LoaderPool.InnerLoad(path, SimpleLoadDataType.texture2D, onLoaded, go);
        }
        else
        {
            LoaderPool.OutterLoad(path, SimpleLoadDataType.texture2D, onLoaded, go);
        }
    }

}
