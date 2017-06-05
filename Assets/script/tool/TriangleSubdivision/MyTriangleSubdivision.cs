using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/// <summary>
/// Triangulation by Ear Clipping(耳切法处理多边形三角划分）
/// 内部做了点线有处理 点重合合并 线交叉拆分
/// </summary>
public class MyTriangleSubdivision
{
    private static MyTriangleSubdivision instance;
    private static MyTriangleSubdivision Instance
    {
        get {
            if (instance == null) instance = new MyTriangleSubdivision();
            return instance;
        }
    }

    private CaculatCloseFunc closeFunc = CaculatCloseFunc.Instance;

    private List<Triangle> templist = new List<Triangle>();
    private List<Vector2> tempVertices = new List<Vector2>();
    private Dictionary<int, int> tempDic = new Dictionary<int, int>();

    /// <summary>
    /// 新版本
    /// </summary>
    public static int[] MyTriangulatePolygon(List<Vector2> Vertices, ref List<Vector3> vertices, ref List<Vector2> uvs, out List<List<Vector2>> closelist, bool useCross = false)
    {
        //if (useCross == false)
        //{
        //    return Instance.myTriangulatePolygon2(Vertices);
        //}
        return Instance.myTriangulatePolygon(Vertices, ref vertices, ref uvs, out closelist);
    }

    protected virtual int[] myTriangulatePolygon(List<Vector2> vectors, ref List<Vector3> vertices, ref List<Vector2> uvs, out List<List<Vector2>> closelist)
    {
        templist.Clear();
        tempVertices.Clear();
        ///闭合列表 列表
        closelist = closeFunc.GetCloseList(vectors);
        for (int i = 0; i < closelist.Count; i++)
        {
            tempDic.Clear();
            List<Vector2> list = closelist[i];
            List<Triangle> triangleList = TriangulatePolygon(list);
            if (triangleList == null) {
                //Debug.Log("TriangulatePolygon(list) == null");
                continue;
            }
            for (int k = 0; k < list.Count; k++)
            {
                int containIndex = -1;
                for (int m = 0; m < tempVertices.Count; m++)
                {
                    if (tempVertices[m] == list[k])
                    {
                        containIndex = m;
                        break;
                    }
                }
                if (containIndex == -1)
                {
                    containIndex = tempVertices.Count;
                    tempVertices.Add(list[k]);
                }
                tempDic.Add(k, containIndex);
            }
            for (int k = 0; k < triangleList.Count; k++)
            {
                triangleList[k].p1 = tempDic[triangleList[k].p1];
                triangleList[k].p2 = tempDic[triangleList[k].p2];
                triangleList[k].p3 = tempDic[triangleList[k].p3];
            }
            tempVertices.InsertRange(tempVertices.Count, list);
            templist.InsertRange(templist.Count, triangleList);
        }
        int[] arr = new int[3 * templist.Count];
        for (int i = 0; i < templist.Count; i++)
        {
            arr[3 * i] = templist[i].p1;
            arr[3 * i + 1] = templist[i].p2;
            arr[3 * i + 2] = templist[i].p3;
        }

        uvs.Clear();
        uvs.InsertRange(0, tempVertices);
        vertices.Clear();
        for (int i = 0; i < tempVertices.Count; i++)
        {
            vertices.Add(tempVertices[i]);
        }
        return arr;
    }

    /////旧版本
    private int[] myTriangulatePolygon2(List<Vector2> Vertices)
    {
        List<Triangle> list = TriangulatePolygon(Vertices);
        if (list == null) {
            //Debug.Log("TriangulatePolygon(list) == null");
            return null;
        }
        int[] arr = new int[3 * list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            arr[3 * i] = list[i].p1;
            arr[3 * i + 1] = list[i].p2;
            arr[3 * i + 2] = list[i].p3;
        }
        return arr;
    }

    /// <summary>
    /// 简单多边形：每个顶点被两条边所共享，而边的所有交点都是顶点
    /// 即把一个非简单多边形分割成多个简单多边形
    /// </summary>
    /// <param name="Vertices"> 逆时针点 </param>
    private List<Triangle> TriangulatePolygon(List<Vector2> Vertices)
    {
        ///已经被切割掉的三角形
        List<Triangle> TriangleList = new List<Triangle>();

        ///去掉耳朵后的剩余点
        List<int> lastPoint = new List<int>();

        /////凸点
        //List<int> tuPoint = new List<int>();
        /////凹点
        //List<int> ouPoint = new List<int>();
        /////耳朵
        //List<int> erPoint = new List<int>();
        /////挖掉的耳朵
        //List<int> cutPoint = new List<int>();

        int len = Vertices.Count;
        for (int i = 0; i < len; i++)
        {
            lastPoint.Add(i);
        }

        while (lastPoint.Count > 3)
        {
            //tuPoint.Clear();
            //ouPoint.Clear();
            //erPoint.Clear();
            Triangle triangle = GetErTriangle(lastPoint, Vertices);
            if (triangle == null)
            {
                //triangle.p1 = 1;
                Debug.LogWarning("lastPoint.Count > 3 && triangle == null");
                return null;
            }
            else {
                lastPoint.Remove(triangle.p2);
                TriangleList.Add(triangle);
            }

        }
        TriangleList.Add(new Triangle(lastPoint[0], lastPoint[1], lastPoint[2]));
        //int[] arr = new int[3 * TriangleList.Count];
        //for (int i = 0; i < TriangleList.Count; i++)
        //{
        //    arr[3 * i] = TriangleList[i].p1;
        //    arr[3 * i + 1] = TriangleList[i].p2;
        //    arr[3 * i + 2] = TriangleList[i].p3;
        //}
        return TriangleList;
    }

    private static Triangle GetErTriangle(List<int> lastPoint, List<Vector2> Vertices)
    {
        for (int i = 0; i < lastPoint.Count; i++)
        {
            int p0 = lastPoint[i < 1 ? lastPoint.Count - 1 : i - 1];
            int p1 = lastPoint[i];
            int p2 = lastPoint[(i + 1) % lastPoint.Count];

            if (LineHelpFunc.Instance.Clockwise(Vertices[p0], Vertices[p1], Vertices[p2]) == 1)
            {
                Triangle triangle = new Triangle(p0, p1, p2);
                bool itemIsEr = true;
                for (int j = 0; j < lastPoint.Count; j++)
                {
                    int p = lastPoint[j];
                    if (p == p0 || p == p1 || p == p2)
                    {
                        continue;
                    }
                    if (Contains(triangle, Vertices[p], Vertices) == true)
                    {
                        itemIsEr = false;
                    }
                }
                if (itemIsEr == true)
                {
                    return triangle;
                }
            }
        }

        return null;
    }


    /// <summary>
    /// 三角形内部是否包含点
    /// </summary>
    private static bool Contains(Triangle triangle, Vector2 vector2, List<Vector2> Vertices)
    {
        if (LineHelpFunc.Instance.Clockwise(Vertices[triangle.p1], Vertices[triangle.p2], vector2) == -1)
        {
            return false;
        }
        if (LineHelpFunc.Instance.Clockwise(Vertices[triangle.p2], Vertices[triangle.p3], vector2) == -1)
        {
            return false;
        }
        if (LineHelpFunc.Instance.Clockwise(Vertices[triangle.p3], Vertices[triangle.p1], vector2) == -1)
        {
            return false;
        }
        return true;
    }

    class Triangle
    {
        public int p1;
        public int p2;
        public int p3;
        public Triangle(int point1, int point2, int point3)
        {
            p1 = point1; p2 = point2; p3 = point3;
        }
    }

}
