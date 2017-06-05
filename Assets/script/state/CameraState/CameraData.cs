using System;
using UnityEngine;
public class CameraData
{
    public CameraData(int cullingMask, Vector3 position, Vector3 angle, bool orthographic = false, float orthographicSize = 0)
    {
        this.orthographic = orthographic;
        this.orthographicSize = orthographicSize;
        this.cullingMask = cullingMask;
        this.position = position;
        this.rotation = Quaternion.Euler(angle);
    }

    public bool orthographic;
    public float orthographicSize;
    public int cullingMask;

    public Vector3 position;
    public Quaternion rotation;

    //暂时未有重置位置的操作 还未投入使用
    public Vector3 forcaseP;
    public bool follow;

    public void setCamera(Camera camera)
    {
        camera.orthographic = orthographic;
        camera.orthographicSize = orthographicSize;
        camera.cullingMask = cullingMask;
        camera.transform.position = position;
        camera.transform.rotation = rotation;
    }

    public void ReadCamera(Camera camera)
    {
        orthographic = camera.orthographic;
        orthographicSize = camera.orthographicSize;
        cullingMask = camera.cullingMask;
        position = camera.transform.position;
        rotation = camera.transform.rotation;
    }
}
