using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 计算闭合的工具
/// </summary>
public class CaculatCloseFunc {

    private static CaculatCloseFunc instance;
    public static CaculatCloseFunc Instance
    {
        get {
            if (instance == null) instance = new CaculatCloseFunc();
            return instance;
        }
    }


    private List<List<Vector2>> closeList = new List<List<Vector2>>();

    private List<Vector2> myVertices = new List<Vector2>();
    private List<Edge> edgeList = new List<Edge>();

    private List<Edge> p1List = new List<Edge>();
    private List<Edge> p2List = new List<Edge>();

    private Dictionary<Vector2, List<Vector2>> searchableDic = new Dictionary<Vector2, List<Vector2>>();
    public List<List<Vector2>> GetCloseList(List<Vector2> vertices)
    {
        myVertices.Clear();
        closeList.Clear();
        edgeList.Clear();
        myVertices.InsertRange(0, vertices);

        for (int i = 0; i < myVertices.Count; i++)
        {
            Vector2 p1 = myVertices[i];
            Vector2 p2 = myVertices[(i + 1) % myVertices.Count];
            edgeList.Add(new Edge(p1, p2));
        }

        //for (int i = 0; i < myVertices.Count; i++)
        //{
        //    Vector2 p1 = myVertices[i];
        //    for (int k = i + 1; k < myVertices.Count; k++)
        //    {
        //        Vector2 p2 = myVertices[k];
        //        if (p1 == p2)
        //        {
        //            //Common(i, k);
        //            myVertices.RemoveAt(i);
        //            i--;
        //            break;
        //        }
        //    }
        //}

        for (int i = 0; i < myVertices.Count; i++)
        {
            Vector2 p = myVertices[i];
            for (int k = 0; k < edgeList.Count; k++)
            {
                Edge item = edgeList[k];
                if (p == item.p1 || p == item.p2)
                {
                    continue;
                }
                if (isOnEdge(p, item) == true)
                {
                    edgeList.RemoveAt(k);
                    k--;
                    OnEdge(p, item);
                }
            }
        }

        for (int i = 0; i < edgeList.Count; i++)
        {
            for (int k = i + 1; k < edgeList.Count; k++)
            {
                Edge edgei = edgeList[i];
                Edge edgek = edgeList[k];
                if (edgei.p1 == edgek.p2 || edgei.p2 == edgek.p1|| edgei.p1 == edgek.p1 || edgei.p2 == edgek.p2)
                {
                    continue;
                }
                Vector2 cross;
                if (tryGetCross(edgei, edgek, out cross))
                {
                    //顺序不能颠倒
                    edgeList.RemoveAt(k);
                    edgeList.RemoveAt(i);
                    i--;
                    //myVertices.Add(cross);
                    edgeList.Add(new Edge(edgei.p1, cross));
                    edgeList.Add(new Edge(cross, edgei.p2));
                    edgeList.Add(new Edge(edgek.p1, cross));
                    edgeList.Add(new Edge(cross, edgek.p2));
                    break;
                }
            }
        }


        for (int i = 0; i < edgeList.Count; i++)
        {
            for (int k = i + 1; k < edgeList.Count; k++)
            {
                if (edgeList[i].Equals(edgeList[k]) == true)
                {
                    edgeList.RemoveAt(i);
                    i--;
                    break;
                }
            }
        }

        SetCloseList();

        return closeList;
    }

    private void SetCloseList()
    {
        for (int i = 0; i < edgeList.Count; i++)
        {
            Edge item = edgeList[i];
            if (isInRoom(item.p1, item.p2) == false)
            {
                List<Vector2> list = GetCloseList(item.p1, item.p2);
                if (list != null)
                {
                    closeList.Add(list);
                }
            }
        }
    }

    private List<Vector2> GetCloseList(Vector2 p1, Vector2 p2)
    {
        searchableDic.Clear();
        List<Vector2> minAnglePointList = SearchMinAngleList(p1, p2);
        return minAnglePointList;
    }

    /// <summary>
    /// 逆时针角最小的点
    /// </summary>
    private List<Vector2> SearchMinAngleList(Vector2 p1, Vector2 p2)
    {
        AddFasle(p1, p2);
        List<Vector2> list = new List<Vector2>();
        list.Add(p2);
        LoopSearch(p1, list, p1, p2);
        list.Insert(0, p1);
        if (list.Count < 3)
        {
            return null;
        }
        ///判断围成多边形的内角和是否大于n*180  内角和(n-2)*180 外角和(n+2)*180;
        float totalAngle = GetTotalClockAngle(list);
        if (totalAngle > 180 * list.Count)
        {
            //Debug.LogTest("内角和"+ totalAngle + "大于n*180");
            return null;
        }
        return list;
    }

    private float GetTotalClockAngle(List<Vector2> list)
    {
        float totalAngle = 0;
        for (int i = 0; i < list.Count; i++)
        {
            Vector2 point1 = list[i];
            Vector2 point2 = list[(i + 1) % list.Count];
            Vector2 point3 = list[(i + 2) % list.Count];
            float angle = GetUnClockAngle(point1, point2, point3);
            totalAngle += angle;
        }
        return totalAngle;
    }

    private void LoopSearch(Vector2 start, List<Vector2> list, Vector2 p1, Vector2 p2)
    {
        Vector2 minAnglePoint;
        bool searched = SearchMinAnglePoint(p1, p2, out minAnglePoint);
        if (searched == true)
        {
            //Debug.LogTest("minAnglePoint " + minAnglePoint.id);
            AddFasle(p2, minAnglePoint);
        }
        else
        {
            //Debug.LogTest("minAnglePoint = null");
        }
        if (searched == false || list.Contains(minAnglePoint) == true)
        {
            //Debug.LogTest("remove " + list[list.Count-1].id);
            list.RemoveAt(list.Count - 1);
            if (list.Count == 0)
            {
                //Debug.LogTest("没有循环");
                return;
            }
            else if (list.Count == 1)
            {
                LoopSearch(start, list, p1, list[0]);
            }
            else
            {
                LoopSearch(start, list, list[list.Count - 2], list[list.Count - 1]);
            }
        }
        else
        {
            //AddFasle(pointTo, minAnglePoint);
            if (minAnglePoint == start)
            {
                //Debug.LogTest("有循环");
                return;
            }
            list.Add(minAnglePoint);
            //Debug.LogTest("add " + minAnglePoint.id);
            LoopSearch(start, list, p2, minAnglePoint);
        }
    }

    /// <summary>
    /// 逆时针最小角
    /// </summary>
    private bool SearchMinAnglePoint(Vector2 p1, Vector2 p2, out Vector2 nearest)
    {
        float minAngle = 400;
        bool searched = false;
        List<Vector2> nearList = new List<Vector2>();
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].p1 == p2)
            {
                nearList.Add(edgeList[i].p2);
            }
        }
        nearest = Vector2.zero;
        for (int i = 0; i < nearList.Count; i++)
        {
            Vector2 near = nearList[i];
            if (near == p1 || GetSearchable(p2, near) == false)
            {
                //Debug.LogTest("near " + near.id + " 不可搜索");
                continue;
            }
            float angle = GetUnClockAngle(p1, p2, near);
            //Debug.LogTest("angle "+ pointFrom.id + " " + pointTo.id + " " + near.id + " = " + angle);
            if (angle < minAngle)
            {
                searched = true;
                ////之前大意判断过后忘记赋值
                minAngle = angle;
                nearest = near;
            }
            //Debug.LogTest("nearest " + nearest.id);
        }
        return searched;
    }

    /// <summary>
    /// 逆时针到角
    /// </summary>
    private float GetUnClockAngle(Vector2 p1, Vector2 p2, Vector2 near)
    {
        float angle1 = GetAngle(p1 - p2);
        float angle2 = GetAngle(near - p2);
        if (angle2 < angle1)
        {
            angle2 = angle2 + 360;
        }
        return angle2 - angle1;
    }

    private float GetAngle(Vector2 direct)
    {
        if (direct.magnitude < 0.01f)
        {
            return 0;
        }
        float angle = Mathf.Acos(direct.x / direct.magnitude);
        if (direct.y < 0)
        {
            angle = 360 - angle * 180 / Mathf.PI;
        }
        else
        {
            angle = angle * 180 / Mathf.PI;
        }
        return angle;
    }

    private bool GetSearchable(Vector2 p1, Vector2 p2)
    {
        if (searchableDic.ContainsKey(p1) == false)
        {
            return true;
        }
        if (searchableDic[p1].Contains(p2) == false)
        {
            return true;
        }
        return false;
    }

    private void AddFasle(Vector2 p1, Vector2 p2)
    {
        if (searchableDic.ContainsKey(p1) == false)
        {
            searchableDic.Add(p1,new List<Vector2>());
        }
        if (searchableDic[p1].Contains(p2) == false)
        {
            searchableDic[p1].Add(p2);
        }
    }

    private bool isInRoom(Vector2 itemp1, Vector2 itemp2)
    {
        for (int k = 0; k < closeList.Count; k++)
        {
            List<Vector2> itemlist = closeList[k];
            for (int m = 0; m < itemlist.Count; m++)
            {
                Vector2 p1 = itemlist[m];
                Vector2 p2 = itemlist[(m + 1) % itemlist.Count];
                if (itemp1 == p1 && itemp2 == p2)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool tryGetCross(Edge edge1, Edge edge2, out Vector2 cross)
    {
        return GetTwoLineCrossPoint(edge1.p1, edge1.p2, edge2.p1, edge2.p2, out cross);
    }

    /// <summary>
    /// 两直线 a * x + b * y + c = 0 与 m * x + n * y + p = 0 的交点
    /// </summary>
    public bool GetTwoLineCrossPoint(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 cross)
    {
        if (GetTwoLineCrossPoint(GetA(p1, p2), GetB(p1, p2), GetC(p1, p2), GetA(p3, p4), GetB(p3, p4), GetC(p3, p4), out cross) == false)
        {
            return false;
        }
        if (PosOnEdge(cross, p1, p2) && PosOnEdge(cross, p3, p4))
        {
            return true;
        }
        return false; 
    }
    /// <summary>
    /// 两直线 a * x + b * y + c = 0 与 m * x + n * y + p = 0 的交点
    /// </summary>
    public bool GetTwoLineCrossPoint(float a, float b, float c, float m, float n, float p, out Vector2 pos)
    {
        if (Mathf.Abs(a * n - b * m) < 0.01f)
        {
            pos = Vector2.zero;
            //pos = GetDisPoint(a, b, c, pos.x, pos.y);
            //Debug.LogWarning("两直线平行 ，没有交点");
            return false;
        }
        pos.x = (b * p - c * n) / (a * n - b * m);
        pos.y = (c * m - a * p) / (a * n - b * m);
        return true;
    }
    
    /// <summary>
    /// 参数 a = y2 - y1;
    /// </summary>
    public float GetA(Vector2 p1, Vector2 p2)
    {
        return p2.y - p1.y;   
    }

    /// <summary>
    /// 参数 b = -(x2 - x1);
    /// </summary>
    public float GetB(Vector2 p1, Vector2 p2)
    {
        return -(p2.x - p1.x);
    }

    /// <summary>
    /// 参数 c = -x1 * y2 + x2 * y1;
    /// </summary>
    public float GetC(Vector2 p1, Vector2 p2)
    {
        return -p1.x * p2.y + p2.x * p1.y;
    }


    private bool isOnEdge(Vector2 p, Edge item)
    {
        float a = GetA(item.p1, item.p2);
        float b = GetB(item.p1, item.p2);
        float c = GetC(item.p1, item.p2);
        float dis = GetDis(a, b, c, p.x, p.y);
        if (dis > 0.06f)
        {
            return false;
        }
        Vector2 v2 = GetDisPoint(a, b, c, p.x, p.y);
        if (PosOnEdge(v2, item.p1, item.p2))
        {
            return true;
        }
        return false;
    }

    public bool PosOnEdge(Vector2 p, Vector2 p1, Vector2 p2)
    {
        if ((p.x - p1.x) * (p.x - p2.x) > 0)
        {
            return false;
        }
        if ((p.y - p1.y) * (p.y - p2.y) > 0)
        {
            return false;
        }
        return true;
    }
    /// <summary>
    /// 点(x0,y0)到直线A*x+B*y+C=0的垂点
    /// </summary>
    public Vector2 GetDisPoint(float A, float B, float C, float x0, float y0)
    {
        float x = 0;
        float y = 0;

        if (A == 0 && B == 0)
        {
            //Debug.LogWarning("直线方程 A==0 && B==0");
        }
        else
        {
            x = (B * B * x0 - A * B * y0 - A * C) / (A * A + B * B);
            y = (A * A * y0 - A * B * x0 - B * C) / (A * A + B * B);
        }
        return new Vector2(x, y);
    }

    /// <summary>
    /// 点(x0,y0)到直线A*x+B*y+C=0的距离
    /// </summary>
    public float GetDis(float A, float B, float C, float x0, float y0)
    {
        return Mathf.Abs(A * x0 + B * y0 + C) / Mathf.Sqrt(A * A + B * B);
    }

    private void OnEdge(Vector2 p, Edge item)
    {
        bool hasp1 = false;
        bool hasp2 = false;
        for (int i = 0; i < edgeList.Count; i++)
        {
            if (edgeList[i].Equals(p, item.p1))
            {
                hasp1 = true;
            }
            if (edgeList[i].Equals(p, item.p2))
            {
                hasp2 = true;
            }
        }
        if (hasp1 == false)
        {
            edgeList.Add(new Edge(item.p1, p));
        }
        if (hasp2 == false)
        {
            edgeList.Add(new Edge(p, item.p2));
        }
    }

    ///// <summary>
    ///// 只对边做处理 k > i;
    ///// </summary>
    //private void Common(int i, int k)
    //{
    //    if (k == i + 1)
    //    {
    //        RemoveEdge(i, k % (myVertices.Count));
    //        return;
    //    }       
    //}
    //private void RemoveEdge(int i, int k)
    //{
    //    for (int m = 0; m < edgeList.Count; m++)
    //    {
    //        if (edgeList[m].Equals(myVertices[i], myVertices[k]))
    //        {
    //            edgeList.RemoveAt(m);
    //        }
    //    }
    //}

    class Edge
    {
        public Vector2 p1;
        public Vector2 p2;

        public Edge(Vector2 point1, Vector2 point2)
        {
            p1 = point1; p2 = point2;
        }

        public Edge() : this(Vector2.zero, Vector2.zero)
        { }

        public bool Equals(Vector2 p1, Vector2 p2)
        {
            return ((this.p1 == p2) && (this.p2 == p1)) || ((this.p1 == p1) && (this.p2 == p2));
        }
        public bool Equals(Edge other)
        {
            return ((this.p1 == other.p2) && (this.p2 == other.p1)) || ((this.p1 == other.p1) && (this.p2 == other.p2));
        }

        public bool Contains(Vector2 p)
        {
            return this.p1 == p || this.p2 == p;
        }

        public Vector2 GetOtherP(Vector2 p)
        {
            if (this.p1 == p)
            {
                return this.p2;
            }

            return this.p1;
        }
    }
}
