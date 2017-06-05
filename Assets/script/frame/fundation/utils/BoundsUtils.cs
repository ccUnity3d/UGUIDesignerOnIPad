using UnityEngine;
using System.Collections;

/// <summary>
/// 获取物体 Bounds 范围
/// </summary>
public class BoundsUtils
{
    /// <summary>
    /// 在世界坐标中真正渲染的部分的范围
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Bounds GetRendererBounds(GameObject obj) 
    { 
        Component[] components = obj.GetComponentsInChildren<Renderer>(false); 
        Bounds bounds = new Bounds(); 

        for (int i=0; i<components.Length; i++) 
        {
            if (i == 0)
            {
                bounds = (components[i] as Renderer).bounds;
            }
            else
            {
                bounds.Encapsulate((components[i] as Renderer).bounds);
            }
        } 
        return bounds; 
    } 
	
    public static string PrintRendererBounds(GameObject obj) 
    { 
        Bounds bounds = GetRendererBounds(obj); 
		//Debug.Log(obj.name+" renderer size: ("+bounds.size.x+", "+bounds.size.y+", "+bounds.size.z+")");
        return obj.name+" renderer size: ("+bounds.size.x+", "+bounds.size.y+", "+bounds.size.z+")"; 
    }

    /// <summary>
    /// 在世界坐标中整个物体完整的范围（不管镜头有没有渲染的、或者被挡住的部分，全算进去）
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string PrintMeshBounds(GameObject obj)
    {
        Bounds bounds = GetMeshBounds(obj);
        return obj.name + " mesh size: (" + bounds.size.x + ", " + bounds.size.y + ", " + bounds.size.z + ")";
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Bounds GetMeshBounds(GameObject obj)
    {
        Component[] components = obj.GetComponentsInChildren<MeshFilter>(false);
        MeshFilter meshFilter;
        Bounds childBounds;
        Bounds bounds = new Bounds();

        for (int i = 0; i < components.Length; i++)
        {
            meshFilter = components[i] as MeshFilter;
            childBounds = meshFilter.sharedMesh.bounds;
            childBounds.size = new Vector3(childBounds.size.x * meshFilter.transform.localScale.x,
                                            childBounds.size.y * meshFilter.transform.localScale.y,
                                            childBounds.size.z * meshFilter.transform.localScale.z);

            if (i == 0)
            {
                bounds = childBounds;
            }
            else
            {
                bounds.Encapsulate(childBounds);
            }
        }
        return bounds;
    }

    /// <summary>
    /// 获得物体在屏幕上的显示范围
    /// 此方法的返回的 Rect 的 起始坐标（0 点）在屏幕左上角
    /// </summary>
    /// <param name="rendererBounds"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Rect BoundsToScreenRect(Bounds rendererBounds, Camera camera)
    {
        //原点
        Vector3 origin = camera.WorldToScreenPoint(new Vector3(rendererBounds.min.x, rendererBounds.max.y, 0.0f));//rendererBounds.center.z
        //范围
        Vector3 extents = camera.WorldToScreenPoint(new Vector3(rendererBounds.max.x, rendererBounds.min.y, 0.0f));

        return new Rect(origin.x, Screen.height - origin.y, extents.x - origin.x, origin.y - extents.y);
    }

    /// <summary>
    /// 获得物体在屏幕上的显示范围
    /// 此方法的返回的 Rect 的 起始坐标（0 点）在屏幕左上角
    /// </summary>
    /// <param name="b"></param>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static Rect BoundsToScreenRect2(Bounds b, Camera camera)
    {
        Rect ScrRect = new Rect(0, 0, 0, 0);
        Vector3[] points = new Vector3[8];
        Vector3[] screenPos = new Vector3[8];

        points[0] = new Vector3(b.min.x, b.min.y, b.min.z);
        points[1] = new Vector3(b.max.x, b.min.y, b.min.z);
        points[2] = new Vector3(b.max.x, b.max.y, b.min.z);
        points[3] = new Vector3(b.min.x, b.max.y, b.min.z);
        points[4] = new Vector3(b.min.x, b.min.y, b.max.z);
        points[5] = new Vector3(b.max.x, b.min.y, b.max.z);
        points[6] = new Vector3(b.max.x, b.max.y, b.max.z);
        points[7] = new Vector3(b.min.x, b.max.y, b.max.z);

        Bounds screenBounds = new Bounds();
        for (int i = 0; i < 8; i++)
        {
            screenPos[i] = camera.WorldToScreenPoint(points[i]);
            if (i == 0)
            {
                screenBounds = new Bounds(screenPos[0], Vector3.zero);
            }
            screenBounds.Encapsulate(screenPos[i]);
        }

        ScrRect.xMin = screenBounds.min.x;
        ScrRect.yMin = screenBounds.min.y;
        ScrRect.xMax = screenBounds.max.x;
        ScrRect.yMax = screenBounds.max.y;
        return ScrRect;
    }

    /// <summary>
    /// 设置、修改物体的 layer，包括它的所有子、孙级
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    public static void setLayerWithChild(GameObject go, int newLayer = 0)
    {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = newLayer;
        }
    }

    /// <summary>
    /// 设置、修改物体的 sortingLayer，包括它的所有子、孙级
    /// </summary>
    /// <param name="go"></param>
    /// <param name="newSortingLayer">SortingLayer类的常量值（默认-1为不更改）</param>
    public static void setSortingLayerWithChild(GameObject go, int newSortingLayer = 0)
    {
        foreach (Renderer trans in go.GetComponentsInChildren<Renderer>(true))
        {
            trans.sortingLayerID = newSortingLayer;
        }
    }

    /// <summary>
    /// 设置、修改物体的 SortingLayerOrder，包括它的所有子、孙级
    /// </summary>
    /// <param name="go"></param>
    /// <param name="newSortingLayerOrder">SortingLayerOrder类的指定枚举的常量值</param>
    public static void setSortingLayerOrderWithChild(GameObject go,int newSortingLayerOrder = 0)
    {
        foreach (Renderer trans in go.GetComponentsInChildren<Renderer>(true))
        {
            trans.sortingOrder = newSortingLayerOrder;
        }
    }
}
