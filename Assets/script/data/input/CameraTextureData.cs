using UnityEngine;

public class CameraTextureData
{
    public Camera camera;
    public RenderTexture texture;

    public CameraTextureData(Camera camera)
    {
        this.camera = camera;
        float value = Screen.height * 1.0f / Screen.width;
        //this.camera.rect = new Rect(0.5f - value / 2, 0, value, 1);
        texture = new RenderTexture(Screen.width/4, Screen.height/4, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        texture.name = "renderTexture";
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.anisoLevel = 0;
        texture.antiAliasing = 1;

        //一些设置
        this.camera.cullingMask = 1 << 0;
        this.camera.aspect = 1;//设置长宽比
        this.camera.clearFlags = CameraClearFlags.SolidColor;

        //使用透明
        this.camera.useOcclusionCulling = true;
        this.camera.backgroundColor = new Color(0, 0, 0, 0);

        //this.camera.targetTexture = texture;
        //RenderTexture.active = this.camera.targetTexture;
    }
}