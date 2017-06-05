using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 三角分割
/// </summary>
public class TriangleSubdivision
{
    private static float multiple = 10000;
    public static int[] TriangulatePolygon(Vector2[] XZofVertices)
    {
        int VertexCount = XZofVertices.Length;
        float xmin = XZofVertices[0].x;
        float ymin = XZofVertices[0].y;
        float xmax = xmin;
        float ymax = ymin;

        for (int i = 1; i < VertexCount; i++)
        {
            if (XZofVertices[i].x < xmin)
            {
                xmin = XZofVertices[i].x;
            }
            else if (XZofVertices[i].x > xmax)
            {
                xmax = XZofVertices[i].x;
            }

            if (XZofVertices[i].y < ymin)
            {
                ymin = XZofVertices[i].y;
            }
            else if (XZofVertices[i].y > ymax)
            {
                ymax = XZofVertices[i].y;
            }
        }

        float dx = xmax - xmin;
        float dy = ymax - ymin;
        float dmax = (dx > dy) ? dx : dy;
        float xmid = (xmax + xmin) * 0.5f;
        float ymid = (ymax + ymin) * 0.5f;

        Vector2[] ExpandedXZ = new Vector2[3 + VertexCount];

        for (int ii1 = 0; ii1 < VertexCount; ii1++)
        {
            ExpandedXZ[ii1] = XZofVertices[ii1];
        }

        //外接三角形的三个点
        ExpandedXZ[VertexCount] = new Vector2((xmid - multiple * 2 * dmax), (ymid - multiple * dmax));
        ExpandedXZ[VertexCount + 1] = new Vector2(xmid, (ymid + multiple * 2 * dmax));
        ExpandedXZ[VertexCount + 2] = new Vector2((xmid + multiple * 2 * dmax), (ymid - multiple * dmax));

        List<Triangle> TriangleList = new List<Triangle>();
        TriangleList.Add(new Triangle(VertexCount, VertexCount + 1, VertexCount + 2));

        for (int ii1 = 0; ii1 < VertexCount; ii1++)
        {
            List<Edge> Edges = new List<Edge>();

            for (int ii2 = 0; ii2 < TriangleList.Count; ii2++)
            {
                Triangle item = TriangleList[ii2];
                if (TriangulatePolygonSubFunc_InCircle(ExpandedXZ[ii1], ExpandedXZ[item.p1], ExpandedXZ[item.p2], ExpandedXZ[item.p3]))
                {
                    Edges.Add(new Edge(item.p1, item.p2));
                    Edges.Add(new Edge(item.p2, item.p3));
                    Edges.Add(new Edge(item.p3, item.p1));
                    TriangleList.RemoveAt(ii2);
                    ii2--;
                }
            }

            if (ii1 >= VertexCount) { continue; }

            for (int ii2 = Edges.Count - 2; ii2 >= 0; ii2--)
            {
                for (int ii3 = Edges.Count - 1; ii3 >= ii2 + 1; ii3--)
                {
                    if (Edges[ii2].Equals(Edges[ii3]))
                    {
                        Edges.RemoveAt(ii3);
                        Edges.RemoveAt(ii2);
                        ii3--;
                        continue;
                    }
                }
            }

            for (int ii2 = 0; ii2 < Edges.Count; ii2++)
            {
                TriangleList.Add(new Triangle(Edges[ii2].p1, Edges[ii2].p2, ii1));
            }

            Edges.Clear();
            Edges = null;
        }

        //大于点集外围的点 外接三角形的三个确认点
        for (int ii1 = TriangleList.Count - 1; ii1 >= 0; ii1--)
        {
            if (TriangleList[ii1].p1 >= VertexCount || TriangleList[ii1].p2 >= VertexCount || TriangleList[ii1].p3 >= VertexCount)
            {
                TriangleList.RemoveAt(ii1);
            }
        }

        //不在点集区域内的面
        for (int ii3 = 0; ii3 < TriangleList.Count; ii3++)
        {
            Triangle item = TriangleList[ii3];

            if (TriangleInPolygonOuter(XZofVertices, XZofVertices[item.p1], XZofVertices[item.p2], XZofVertices[item.p3]))
            {
                TriangleList.RemoveAt(ii3);
                ii3--;
            }
        }

        TriangleList.TrimExcess();

        int[] Triangles = new int[3 * TriangleList.Count];

        for (int ii1 = 0; ii1 < TriangleList.Count; ii1++)

        {

            Triangles[3 * ii1] = TriangleList[ii1].p1;

            Triangles[3 * ii1 + 1] = TriangleList[ii1].p2;

            Triangles[3 * ii1 + 2] = TriangleList[ii1].p3;

        }

        return Triangles;

    }


    //float.Epsilon这个值为无限接近于0的值，我再pc上正常，在ios端就不能用了，我打印出直接输出0，大家可以模拟一个值，0.0000000001，或者什么   趋近于0，但不等于0就行了，</span>
    static bool TriangulatePolygonSubFunc_InCircle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float minvalue = 1 / float.MaxValue;
        if (Mathf.Abs(p1.y - p2.y) < minvalue && Mathf.Abs(p2.y - p3.y) < minvalue)
        {
            return false;
        }

        float m1, m2, mx1, mx2, my1, my2, xc, yc;

        if (Mathf.Abs(p2.y - p1.y) < minvalue)
        {
            m2 = -(p3.x - p2.x) / (p3.y - p2.y);
            mx2 = (p2.x + p3.x) * 0.5f;
            my2 = (p2.y + p3.y) * 0.5f;
            xc = (p2.x + p1.x) * 0.5f;
            yc = m2 * (xc - mx2) + my2;
        }
        else if (Mathf.Abs(p3.y - p2.y) < minvalue)
        {
            m1 = -(p2.x - p1.x) / (p2.y - p1.y);
            mx1 = (p1.x + p2.x) * 0.5f;
            my1 = (p1.y + p2.y) * 0.5f;
            xc = (p3.x + p2.x) * 0.5f;
            yc = m1 * (xc - mx1) + my1;
        }
        else
        {
            m1 = -(p2.x - p1.x) / (p2.y - p1.y);
            m2 = -(p3.x - p2.x) / (p3.y - p2.y);
            mx1 = (p1.x + p2.x) * 0.5f;
            mx2 = (p2.x + p3.x) * 0.5f;
            my1 = (p1.y + p2.y) * 0.5f;
            my2 = (p2.y + p3.y) * 0.5f;
            xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
            yc = m1 * (xc - mx1) + my1;
        }

        float dx = p2.x - xc;
        float dy = p2.y - yc;
        float rsqr = dx * dx + dy * dy;
        dx = p.x - xc;
        dy = p.y - yc;
        double drsqr = dx * dx + dy * dy;
        return (drsqr <= rsqr);
    }

    static bool TriangleInPolygonOuter(Vector2[] pList, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        Vector2 centrePoint = new Vector2((p1.x + p2.x + p3.x) / 3, (p1.y + p2.y + p3.y) / 3);
        int crossNum = 0;

        for (int i = 0; i < pList.Length; i++)
        {
            if (IsPointInLine(centrePoint.x, centrePoint.y, pList[i].x, pList[i].y, pList[(i + 1) % pList.Length].x, pList[(i + 1) % pList.Length].y) == 0)
            {
                crossNum = crossNum + 1;
                continue;
            }
            else if (IsPointInLine(centrePoint.x, centrePoint.y, pList[i].x, pList[i].y, pList[(i + 1) % pList.Length].x, pList[(i + 1) % pList.Length].y) == 2)
            {
                crossNum = 1;
                break;
            }
        }

        if ((crossNum % 2) == 0)
        {
            return true;
        }

        crossNum = 0;
        return false;
    }

    //0   在外  1  在内   2  边上

    static int IsPointInLine(float x, float y, float x1, float y1, float x2, float y2)
    {
        float maxY = y1;
        float minY = y2;

        if (y1 > y2)
        {
            maxY = y1;
            minY = y2;
        }
        else {
            maxY = y2;
            minY = y1;
        }

        float averageX = (x1 + x2) / 2;
        float averageY = (y1 + y2) / 2;

        if (y == averageY && x == averageX)
        {
            return 2;
        }

        if (y < maxY && y > minY)
        {
            if (x > (x1 + (x2 - x1) * (y - y1) / (y2 - y1)))
            {
                return 0;
            }
        }
        return 1;
    }


    struct Triangle
    {
        public int p1;
        public int p2;
        public int p3;
        public Triangle(int point1, int point2, int point3)
        {
            p1 = point1; p2 = point2; p3 = point3;
        }
    }

    class Edge
    {
        public int p1;
        public int p2;

        public Edge(int point1, int point2)
        {
            p1 = point1; p2 = point2;
        }

        public Edge() : this(0, 0)
        { }

        public bool Equals(Edge other)
        {
            return ((this.p1 == other.p2) && (this.p2 == other.p1)) || ((this.p1 == other.p1) && (this.p2 == other.p2));
        }
    }
}