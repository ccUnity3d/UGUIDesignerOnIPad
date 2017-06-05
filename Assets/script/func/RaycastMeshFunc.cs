using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastMeshFunc : Singleton<RaycastMeshFunc> {

    private LineHelpFunc lineFunc
    {
        get { return LineHelpFunc.Instance; }
    }
    private Mode2DPrefabs prefabs
    {
        get { return Mode2DPrefabs.Instance; }
    }

    /// <summary>
    /// 所有顶点的屏幕坐标
    /// </summary>
    private List<Vector2> verticelist = new List<Vector2>();

    /// <summary>
    /// 屏幕坐标点是否可以点到物体
    /// </summary>
    /// <param name="screenp">屏幕坐标</param>
    /// <param name="go">物体</param>
    public bool RaycastMesh(Vector2 screenp, GameObject go, out float hitdis)
    {
        MeshFilter[] meshfilters = go.GetComponentsInChildren<MeshFilter>();
        hitdis = float.MaxValue;

        for (int i = 0; i < meshfilters.Length; i++)
        {
            Mesh mesh = meshfilters[i].sharedMesh;
            Transform tran = meshfilters[i].transform;
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;

            verticelist.Clear();
            for (int k = 0; k < vertices.Length; k++)
            {
                Vector3 v3local = vertices[k];
                Vector3 v3world = tran.TransformPoint(v3local);
                Vector3 v3screen = prefabs.mainCamera.WorldToScreenPoint(v3world);
                verticelist.Add(v3screen);
            }

            int triggleCount = triangles.Length / 3;
            
            for (int k = 0; k < triggleCount; k++)
            {
                Vector3 p1 = verticelist[triangles[3 * k]];
                Vector3 p2 = verticelist[triangles[3 * k + 1]];
                Vector3 p3 = verticelist[triangles[3 * k + 2]];
                
                //如果是disfloor面片 <=0 会被忽略掉  如果是地毯 >0 会被忽略掉 所以这个过滤条件先关掉
                //int clockWise = lineFunc.Clockwise(p1, p2, p3);
                //if (clockWise <= 0)
                //{
                //    continue;
                //}

                bool attriggle = atTriggle(screenp, p1, p2, p3);
                if (attriggle)
                {
                    //返回距离 - 近似距离 点击到三角面的中心点 误差小  只有大的三角面误差较大
                    Vector3 worldp = prefabs.mainCamera.ScreenToWorldPoint(screenp);
                    Vector3 worldp1 = tran.TransformPoint(vertices[triangles[3 * k]]);
                    Vector3 worldp2 = tran.TransformPoint(vertices[triangles[3 * k + 1]]);
                    Vector3 worldp3 = tran.TransformPoint(vertices[triangles[3 * k + 2]]);
                    Vector3 triggleCenter = (worldp1 + worldp2 + worldp3) / 3;
                    float dis = Vector3.Distance(worldp, triggleCenter);
                    //float dis = 0;//TODO:
                    if (dis < hitdis)
                    {
                        hitdis = dis;
                    }
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 点在三角形上或内部
    /// </summary>
    /// <param name="p">点</param>
    /// <param name="p1">三角形点1</param>
    /// <param name="p2">三角形点2</param>
    /// <param name="p3">三角形点3</param>
    private bool atTriggle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        int wise1 = lineFunc.Clockwise(p1, p2, p);
        int wise2 = lineFunc.Clockwise(p2, p3, p);
        int wise3 = lineFunc.Clockwise(p3, p1, p);
        if (wise1 * wise2 == -1) return false;
        if (wise2 * wise3 == -1) return false;
        if (wise3 * wise1 == -1) return false;
        return true;
    }

}
