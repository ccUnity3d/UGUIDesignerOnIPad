using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TouchCaptureScreen : Singleton<TouchCaptureScreen> {

    // Use this for initialization
    //public Camera camera;
    private float captureTime;
    private Vector2 beginTouch;
    private Vector2 endTouch;
    //private Vector2 beginTouch;
    //private Vector2 endTouch;
    
    Rect rect = new Rect(0, 0, Screen.width, Screen.height);
    public Texture2D texture;
    private Camera camera
    {
        get {
            return Mode2DPrefabs.Instance.mainCamera;
        }
    }

    void Start () {
      
        //beginTouch = Vector2.zero;
        //endTouch = new Vector2(1000,700);
        //AoutCaptureScreenImage();

    }
	
	// Update is called once per frame
	void Update () {
        //TouchCaptureScreenImage();
      
    }

    ////  通过手指截屏
    //public void TouchCaptureScreenImage()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        beginTouch = Input.mousePosition;
    //    }
    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        endTouch = Input.mousePosition;
    //        MyMono.MyStartCoroutine(TouchCaptureImage, this,  null);
    //        captureTime = 0;
    //    }
    //}

    // 自动截图 
    public void AoutCaptureScreenImage()
    {
        CameraClearFlags cameraClearFlags = camera.clearFlags;
        camera.clearFlags = CameraClearFlags.SolidColor;
        MyMono.MyStartCoroutine(AoutCaptureImage, this, camera);
        camera.clearFlags = cameraClearFlags;
    }
    
    //// 自动截图 根据相机
    //IEnumerator TouchCaptureImage(object[] datas)
    //{
    //    readTexture = new Texture2D((int)(endTouch.x - beginTouch.x), (int)(beginTouch.y - endTouch.y), TextureFormat.ARGB32, false);
    //    Rect rect = new Rect((int)beginTouch.x, Screen.height - (int)(Screen.height - endTouch.y), (int)(endTouch.x - beginTouch.x), (int)(beginTouch.y - endTouch.y));
    //    yield return new WaitForEndOfFrame();
    //    readTexture.ReadPixels(rect,0,0);
    //    readTexture.Apply();
    //    yield return readTexture;
    //    byte[] bytes = readTexture.EncodeToPNG();
    //    //File.WriteAllBytes(Application.dataPath+"/newTexture.png",bytes);
    //}
    IEnumerator AoutCaptureImage(object[] datas)
    {
        Mode2DPrefabs.Instance.uiCamera.enabled = false;
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();
        //Camera tempCamera = datas[0] as Camera;
        RenderTexture targetTexture = new RenderTexture(2048 / 4, 1536 / 4, -1);
        camera.targetTexture = targetTexture;
        Texture2D readTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.ARGB32, false);
        readTexture.ReadPixels(rect, 0, 0);
        yield return new WaitForEndOfFrame();
        readTexture.Apply();
        texture = GetTexture(readTexture, 2);
        camera.targetTexture = null;
        Mode2DPrefabs.Instance.uiCamera.enabled = true;

        yield return new WaitForSeconds(0.1f);
        yield return new WaitForEndOfFrame();
        GameObject.DestroyImmediate(targetTexture, true);
        GameObject.DestroyImmediate(readTexture, true);
        targetTexture = null;
        readTexture = null;
        Resources.UnloadUnusedAssets();
        //GC.Collect();
        //byte[] bytes = texture.EncodeToPNG();
        //File.WriteAllBytes(Application.dataPath + "/miniTexture.png", bytes);
    }

    private Texture2D GetTexture(Texture2D readTexture, int multi = 2)
    {
        Color[] colors = readTexture.GetPixels();
        Color[] newColors = new Color[readTexture.width / multi * readTexture.height / multi];
        Texture2D texture = new Texture2D(readTexture.width / multi, readTexture.height / multi);
        for (int i = 0; i < texture.width; i++)
        {
            for (int k = 0; k < texture.height; k++)
            {
                int index = k * texture.width + i;
                Color color0 = colors[k * multi * readTexture.width + i * multi];
                //Color color1 = colors[k * multi * readTexture.width + i * multi + 1];
                //Color color2 = colors[(k * multi + 1) * readTexture.width + i * multi];
                //Color color3 = colors[(k * multi + 1) * readTexture.width + i * multi + 1];
                //newColors[index] = (color0+ color1+ color2+ color3)/multi;
                newColors[index] = color0;
                //newColors[i].r = ;
                //newColors[i].g = ;
                //newColors[i].b = ;
                //newColors[i].a = ;
            }
        }
        texture.SetPixels(newColors);
        return texture;
    }
}
