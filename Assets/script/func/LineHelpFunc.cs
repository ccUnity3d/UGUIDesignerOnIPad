using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LineHelpFunc {

    private static LineHelpFunc instance;
    public static LineHelpFunc Instance
    {
        get {
            if (instance == null) instance = new LineHelpFunc();
            return instance;
        }
    }

    private int[] EmtyArr = new int[] { };
    private List<int> intlist = new List<int>();

    public float GetAngle(Vector2 direct)
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
    /// <summary>
    /// 弧度角
    /// </summary>
    /// <param name="direct"></param>
    /// <returns></returns>
    public float GetRadian(Vector2 direct)
    {
        if (direct.magnitude < 0.01f)
        {
            return 0;
        }
        float angle = Mathf.Acos(direct.x / direct.magnitude);
        if (direct.y < 0)
        {
            angle = 2 * Mathf.PI - angle;
        }
        return angle;
    }

    /// <summary>
    /// 夹角
    /// </summary>
    public float GetCrossAngle(Vector2 v1, Vector2 v2)
    {
        float angle = Mathf.Acos(Vector2.Dot(v1, v2) / (v1.magnitude * v2.magnitude));
        return angle * 180f / Mathf.PI;
    }
    public bool GetCross(WallData line1, WallData line2)
    {
        return GetCross(line1, line2.point1, line2.point2.pos);
    }
    public bool GetCross(WallData line, Point currentPoint, Vector2 mousepos)
    {
        if (line.GetDis(currentPoint.pos) < 0.01f || line.GetDis(mousepos) < 0.01f)
        {
            return false;
        }
        return CheckTwoLineCrose(line.point1.pos, line.point2.pos, currentPoint.pos, mousepos);
    }

    /// <summary>
    /// 点(x0,y0)到直线A*x+B*y+C=0的距离
    /// </summary>
    public float GetDis(float A, float B, float C, float x0, float y0)
    {
        return Mathf.Abs(A * x0 + B * y0 + C) / Mathf.Sqrt(A * A + B * B);
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
            //Debug.Log("直线方程 A==0 && B==0");
        }
        else
        {
            /*      
            if (A == 0)
            {
                x = x0;
                y = -C / B;
            }
            else if (B == 0)
            {
                x = -A / C;
                y = y0;
            }
            *///以上均满足 下面的公式set
            x = (B * B * x0 - A * B * y0 - A * C) / (A * A + B * B);
            y = (A * A * y0 - A * B * x0 - B * C) / (A * A + B * B);
        }

        return new Vector2(x, y);

    }

    public float GetClockAngle(Vector2 pointFrom, Vector2 pointTo, Vector2 near)
    {
        float angle1 = GetAngle(pointFrom - pointTo);
        float angle2 = GetAngle(near - pointTo);
        if (angle2 < angle1)
        {
            angle2 = angle2 + 360;
        }
        return angle2 - angle1;
    }

    public bool IsParallel(float a, float b, float m, float n)
    {
        if (a == 0 && b == 0)
        {
            return false;
        }
        if (m == 0 && n == 0)
        {
            return false;
        }

        while (Mathf.Abs(a) < 1 && Mathf.Abs(b) < 1)
        {
            a *= 100;
            b *= 100;
        }
        while (Mathf.Abs(m) < 1 && Mathf.Abs(n) < 1)
        {
            m *= 100;
            n *= 100;
        }        
        return Mathf.Abs(a * n - b * m) < 0.1f;
    }

    public float GetArea(List<Vector2> v2s, out Vector2 center)
    {
        float area = 0;

        //计算中心点用
        center = Vector2.zero;
        float totalAbsArea = 0;

        for (int i = 0; i < v2s.Count; i++)
        {
            Vector2 p1 = v2s[i];
            Vector2 p2 = v2s[(i + 1) % v2s.Count];
            float deltaArea = p1.x * p2.y - p2.x * p1.y;
            area += deltaArea;

            //计算中心点用
            totalAbsArea += Mathf.Abs(deltaArea);
            center += deltaArea * (p1 + p2) / 3f;
        }
        //计算中心点用
        center = -center / totalAbsArea;

        return 0.5f * Mathf.Abs(area);
    }

    /// <summary>
    /// 顺时针是负的
    /// </summary>
    /// <param name="v2s"></param>
    /// <returns></returns>
    public float GetArea(List<Vector2> v2s)
    {
        float area = 0;
        
        for (int i = 0; i < v2s.Count; i++)
        {
            Vector2 p1 = v2s[i];
            Vector2 p2 = v2s[(i + 1) % v2s.Count];
            float deltaArea = p1.x * p2.y - p2.x * p1.y;
            area += deltaArea;
        }
       
        return 0.5f * area;
    }

    public Vector2 GetTwoLineCrossPoint(WallData wall1, WallData wall2, Vector2 pos)
    {
        return GetTwoLineCrossPoint(wall1.a, wall1.b, wall1.c, wall2.a, wall2.b, wall2.c, pos);
    }
    /// <summary>
    /// 两直线 a * x + b * y + c = 0 与 m * x + n * y + p = 0 的交点
    /// </summary>
    public Vector2 GetTwoLineCrossPoint(float a, float b, float c, float m, float n, float p, Vector2 pos)
    {
        if (Mathf.Abs(a * n - b * m) < 0.01f)
        {
            pos = GetDisPoint(a, b, c, pos.x, pos.y);
            //Debug.LogWarning("两直线平行 ，没有交点");
            return pos;
        }
        float x = (b * p - c * n) / (a * n - b * m);
        float y = (c * m - a * p) / (a * n - b * m);
        return new Vector2(x, y);
    }


    public void SetLine(Vector2 v21, Vector2 v22, GameObject obj, float size)
    {
        Vector3 v3 = v21; v3.z = 0;
        Vector2 direct = v22 - v21;
        float angle = GetAngle(direct);
        float len = direct.magnitude / size;
        float an = (angle - 90);
        obj.transform.localRotation = Quaternion.Euler(Vector3.forward * an);
        v3.z = obj.transform.localPosition.z;
        obj.transform.localPosition = v3;

        Mesh mesh = obj.GetComponent<MeshFilter>().mesh;
        List<Vector3> list = new List<Vector3>() { new Vector3(-0.5f,0), new Vector3(0.5f,len), new Vector3(0.5f,0), new Vector3(-0.5f, len)};
        mesh.SetVertices(list);
        List<Vector2> v2list = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, len + 0.5f), new Vector2(1, 0), new Vector2(0, len + 0.5f) };
        mesh.SetUVs(0, v2list);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public List<List<Vector2>> SetMesh(List<Vector2> v2s, GameObject wallbody, float offsetX = 0, float offsetY = 0, float angle = 0, float tileSize_x = 1, float tileSize_y = 1, bool useCross = false)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < v2s.Count; i++)
        {
            vertices.Add(v2s[i]);
        }
        List<List<Vector2>> closelist;
        int[] triangles = MyTriangleSubdivision.MyTriangulatePolygon(v2s, ref vertices, ref uvs, out closelist, useCross);//TriangleSubdivision.TriangulatePolygon(v2s);
        if (triangles == null)
        {
            Debug.LogWarning("triangles == null 不是简单多边形");
        }
        float Angle = angle * Mathf.PI / 180f;
        for (int i = 0; i < uvs.Count; i++)
        {
            float x = uvs[i].x;
            float y = uvs[i].y;
            Vector2 newV2 = Vector2.zero;
            newV2.x = tileSize_x * (x * Mathf.Cos(Angle) - y * Mathf.Sin(Angle) + offsetX);
            newV2.y = tileSize_y * (x * Mathf.Sin(Angle) + y * Mathf.Cos(Angle) + offsetY);
            uvs[i] = newV2;
        }

        Mesh mesh = wallbody.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        //mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        PolygonCollider2D polygon = wallbody.GetComponent<PolygonCollider2D>();
        if (polygon != null) polygon.SetPath(0, v2s.ToArray());

        return closelist;
    }

    public void SetMesh3D(List<Vector2> v2s, GameObject gameObject, float offsetX = 0, float offsetY = 0, float angle = 0, float tileSize_x = 1, float tileSize_y = 1, bool useCross = false, bool isFloor = true)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < v2s.Count; i++)
        {
            vertices.Add(v2s[i]);
        }
        List<List<Vector2>> closelist;
        int[] triangles = MyTriangleSubdivision.MyTriangulatePolygon(v2s, ref vertices, ref uvs, out closelist, true);//TriangleSubdivision.TriangulatePolygon(v2s);
        if (triangles == null)
        {
            Debug.LogWarning("triangles == null 不是简单多边形");
        }
        float Angle = angle * Mathf.PI / 180f;
        for (int i = 0; i < uvs.Count; i++)
        {
            float x = uvs[i].x;
            float y = uvs[i].y;
            Vector2 newV2 = Vector2.zero;
            newV2.x = tileSize_x * (x * Mathf.Cos(Angle) - y * Mathf.Sin(Angle) + offsetX);
            newV2.y = tileSize_y * (x * Mathf.Sin(Angle) + y * Mathf.Cos(Angle) + offsetY);
            uvs[i] = newV2;
        }

        if (isFloor == false)
        {
            intlist.Clear();
            for (int i = 0; i < triangles.Length; i++)
            {
                intlist.Add(triangles[triangles.Length - 1 - i]);
            }
            for (int i = 0; i < intlist.Count; i++)
            {
                triangles[i] = intlist[i];
            }
        }
        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = Vector3.right * vertices[i].x + Vector3.forward * vertices[i].y;
        }

        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        //mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider meshColli = gameObject.GetComponentInChildren<MeshCollider>();
        if (meshColli != null)
        {
            Mesh newMesh = new Mesh();
            newMesh.SetTriangles(EmtyArr, 0);
            newMesh.SetVertices(vertices);
            newMesh.SetUVs(0, uvs);
            newMesh.SetTriangles(triangles, 0);
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            meshColli.sharedMesh = newMesh;
        }
    }
    /// <summary>
    /// mesh计算 不合并同位置点（门窗专用）
    /// </summary>
    public void SetMesh(float v2Len, float height, GameObject body, List<Vector2> pointList, List<Vector2> upcrossList = default(List<Vector2>), float offsetX = 0, float offsetY = 0, float angle = 0, float tileSize_x = 1, float tileSize_y = 1)//pointList 挖洞点列表
    {
        if ((pointList == null || pointList.Count == 0) && (upcrossList == default(List<Vector2>) || upcrossList.Count == 0))
        {
            SetMesh(v2Len, height, body, offsetX, offsetY, angle, tileSize_x, tileSize_y);
            return;
        }
        List<Vector2> v2s = new List<Vector2>();
        v2s.Add(Vector2.zero);
        v2s.Add(Vector2.up * height);
        if (upcrossList != default(List<Vector2>))
        {
            for (int i = 0; i < upcrossList.Count; i++)
            {
                v2s.Add(upcrossList[i]);
            }
        }
        v2s.Add(Vector2.right * v2Len + Vector2.up * height);
        v2s.Add(Vector2.right * v2Len);
        for (int i = 0; i < pointList.Count; i++)
        {
            v2s.Add(pointList[i]);
        }
        
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < v2s.Count; i++)
        {
            vertices.Add(v2s[i]);
        }
        List<List<Vector2>> closelist;
        int[] triangles = MyTriangleSubdivisionDoor.MyTriangulatePolygon(v2s, ref vertices, ref uvs, out closelist, false);

        float Angle = angle * Mathf.PI / 180f;
        for (int i = 0; i < uvs.Count; i++)
        {
            float x = uvs[i].x;
            float y = uvs[i].y;
            Vector2 newV2 = Vector2.zero;
            //纹理 的缩放旋转平移
            newV2.x = tileSize_x * (x * Mathf.Cos(Angle) - y * Mathf.Sin(Angle) + offsetX);
            newV2.y = tileSize_y * (x * Mathf.Sin(Angle) + y * Mathf.Cos(Angle) + offsetY);
            uvs[i] = newV2;
        }

        Mesh mesh = body.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        //mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider meshColli = body.GetComponentInChildren<MeshCollider>(true);
        if (meshColli != null)
        {
            meshColli.convex = false;
            Mesh newMesh = new Mesh();
            newMesh.SetTriangles(EmtyArr, 0);
            newMesh.SetVertices(vertices);
            newMesh.SetUVs(0, uvs);
            newMesh.SetTriangles(triangles, 0);
            newMesh.RecalculateNormals();
            newMesh.RecalculateBounds();
            meshColli.sharedMesh = newMesh;
        }
    }
    /// <summary>
    /// mesh计算 合并同位置点
    /// </summary>
    public void SetMesh(float v2Len, float height, GameObject body, float offsetX = 0, float offsetY = 0, float angle = 0, float tileSize_x = 1, float tileSize_y = 1)
    {
        List<Vector2> v2s = new List<Vector2>();
        v2s.Add(Vector2.zero);
        v2s.Add(Vector2.up * height);
        v2s.Add(Vector2.right * v2Len + Vector2.up * height);
        v2s.Add(Vector2.right * v2Len);
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < v2s.Count; i++)
        {
            vertices.Add(v2s[i]);
        }
        List<List<Vector2>> closelist;
        int[] triangles = MyTriangleSubdivision.MyTriangulatePolygon(v2s, ref vertices, ref uvs, out closelist, true);

        float Angle = angle * Mathf.PI / 180f;
        for (int i = 0; i < uvs.Count; i++)
        {
            float x = uvs[i].x;
            float y = uvs[i].y;
            Vector2 newV2 = Vector2.zero;
            //纹理 的缩放旋转平移
            newV2.x = tileSize_x * (x * Mathf.Cos(Angle) - y * Mathf.Sin(Angle) + offsetX);
            newV2.y = tileSize_y * (x * Mathf.Sin(Angle) + y * Mathf.Cos(Angle) + offsetY);
            uvs[i] = newV2;
        }
        Mesh mesh = body.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);
        //mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        BoxCollider colli = body.GetComponent<BoxCollider>();
        if (colli != null)
        {
            colli.center = Vector3.right * v2Len / 2 + Vector3.up * height / 2;
            colli.size = Vector3.right * v2Len + Vector3.up * height;
        }
        else
        {
            MeshCollider meshColli = body.GetComponentInChildren<MeshCollider>(true);
            if (meshColli != null)
            {
                meshColli.convex = false;
                Mesh newMesh = new Mesh();
                newMesh.SetTriangles(EmtyArr, 0);
                newMesh.SetVertices(vertices);
                newMesh.SetUVs(0, uvs);
                newMesh.SetTriangles(triangles, 0);
                newMesh.RecalculateNormals();
                newMesh.RecalculateBounds();
                meshColli.sharedMesh = newMesh;
            }
        }
    }

    /// <summary>
    /// 与SetMesh显示面相反
    /// </summary>
    public void SetMesh2(float v2Len, float height, GameObject body, float offsetX = 0, float offsetY = 0, float angle = 0, float tileSize_x = 1, float tileSize_y = 1)
    {
        List<Vector2> v2s = new List<Vector2>();
        v2s.Add(Vector2.zero);
        v2s.Add(Vector2.up * height);
        v2s.Add(Vector2.right * v2Len + Vector2.up * height);
        v2s.Add(Vector2.right * v2Len);
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        for (int i = 0; i < v2s.Count; i++)
        {
            vertices.Add(v2s[i]);
        }
        List<List<Vector2>> closelist;
        int[] triangles = MyTriangleSubdivision.MyTriangulatePolygon(v2s, ref vertices, ref uvs, out closelist, true);

        float Angle = angle * Mathf.PI / 180f;
        for (int i = 0; i < uvs.Count; i++)
        {
            float x = uvs[i].x;
            float y = uvs[i].y;
            Vector2 newV2 = Vector2.zero;
            //纹理 的缩放旋转平移
            newV2.x = tileSize_x * (x * Mathf.Cos(Angle) - y * Mathf.Sin(Angle) + offsetX);
            newV2.y = tileSize_y * (x * Mathf.Sin(Angle) + y * Mathf.Cos(Angle) + offsetY);
            uvs[i] = newV2;
        }

        Mesh mesh = body.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        OtherSide(triangles);
        mesh.SetTriangles(triangles, 0);
        //mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        BoxCollider colli = body.GetComponent<BoxCollider>();
        if (colli != null)
        {
            colli.center = Vector3.right * v2Len / 2 + Vector3.up * height / 2;
            colli.size = Vector3.right * v2Len + Vector3.up * height;
        }
    }

    List<int> temp = new List<int>();
    private void OtherSide(int[] triangles)
    {
        temp.Clear();
        for (int i = 0; i < triangles.Length; i++)
        {
            temp.Add(triangles[i]);
        }
        for (int i = 0; i < triangles.Length; i++)
        {
            int m = i / 3;
            int n = i % 3;
            triangles[i] = temp[3 * m + 2 - n];
        }
        return;
    }

    /// <summary>
    /// 1顺时针 0共线 -1逆时针
    /// </summary>
    public int Clockwise(Vector2 pos1, Vector2 pos2, Vector2 hitPoint)
    {
        float multiplicationCross = (pos1.x - pos2.x) * (hitPoint.y - pos2.y) - (pos1.y - pos2.y) * (hitPoint.x - pos2.x);
        if (multiplicationCross > 0)
        {
            return 1;
        }
        if (multiplicationCross < 0)
        {
            return -1;
        }

        return 0;

    }


    /// <summary>  
    /// 判断直线2的两点是否在直线1的两边。  
    /// </summary>  
    /// <param name="line1">直线1</param>  
    /// <param name="line2">直线2</param>  
    /// <returns></returns>  
    private bool CheckCrose(Vector2 line1str, Vector2 line1end, Vector2 line2str, Vector2 line2end)
    {
        Vector2 v1 = Vector2.zero;
        Vector2 v2 = Vector2.zero;
        Vector2 v3 = Vector2.zero;

        v1.x = line2str.x - line1end.x;
        v1.y = line2str.y - line1end.y;

        v2.x = line2end.x - line1end.x;
        v2.y = line2end.y - line1end.y;

        v3.x = line1str.x - line1end.x;
        v3.y = line1str.y - line1end.y;

        return (CrossMul(v1, v3) * CrossMul(v2, v3) < 0);

    }

    /// <summary>  
    /// 判断两条线段是否相交。  
    /// </summary>  
    /// <param name="line1">线段1</param>  
    /// <param name="line2">线段2</param>  
    /// <returns>相交返回真，否则返回假。</returns>  
    private bool CheckTwoLineCrose(Vector2 line1str, Vector2 line1end, Vector2 line2str, Vector2 line2end)
    {
        return CheckCrose(line1str, line1end, line2str, line2end) && CheckCrose(line2str, line2end, line1str, line1end);
    }
    /// <summary>  
    /// 计算两个向量的叉乘的值。  
    /// </summary>  
    /// <param name="pt1"></param>  
    /// <param name="pt2"></param>  
    /// <returns></returns>  
    private float CrossMul(Vector2 pt1, Vector2 pt2)
    {
        return pt1.x * pt2.y - pt1.y * pt2.x;
    }

    /// <summary>
    /// 是否平行
    /// </summary>
    /// <param name="item"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsTwoLineParallel(WallData item, WallData target)
    {
        return IsTwoLineParallel(item.a, item.b, target.a, target.b);
    }

    public bool IsTwoLineParallel(float a, float b, float m, float n)
    {
        if (Mathf.Abs(a * n - b * m) < 0.01f)
        {
            //Debug.LogWarning("两直线平行 ，没有交点");
            return true;
        }
        return false;
    }


    private List<Vector2> tempUVs = new List<Vector2>();
    private List<Vector3> tempv3s = new List<Vector3>();
    private List<int> triangles = new List<int>();

    /// <summary>
    /// 设置圆环体内圈半径、环宽度 -- 使用mesh
    /// </summary>
    /// <param name="Obj">圆环体</param>
    /// <param name="radius">圆环内圈半径</param>
    public void SetRadiusMesh(GameObject Obj, float radius2)
    {
        //圆分割数
        int n = 50;
        //圆环宽度
        float w = 0.08f;

        tempUVs.Clear();
        tempv3s.Clear();
        triangles.Clear();

        float radius = radius2 - w;

        for (int i = 0; i < n; i++)
        {
            float angle = 2 * Mathf.PI / n * i;
            float x = radius * Mathf.Cos(angle);
            float y = radius * Mathf.Sin(angle);
            tempUVs.Add(GetV2(x, y));
            tempv3s.Add(GetV2(x, y));
            x = radius2 * Mathf.Cos(angle);
            y = radius2 * Mathf.Sin(angle);
            tempUVs.Add(GetV2(x, y));
            tempv3s.Add(GetV2(x, y));

            triangles.Add(2 * i);
            triangles.Add(2 * ((i + 1) % n));
            triangles.Add(2 * i + 1);

            triangles.Add(2 * i + 1);
            triangles.Add(2 * ((i + 1) % n));
            triangles.Add(2 * ((i + 1) % n) + 1);
        }


        Mesh mesh = Obj.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(tempv3s);
        mesh.SetUVs(0, tempUVs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshColli = Obj.GetComponent<MeshCollider>();
        if (meshColli != null) {
            meshColli.sharedMesh = mesh;
        }
    }

    public Vector2 GetV2(float x,float y)
    {
        return Vector2.right * x + Vector2.up * y;
    }

}
