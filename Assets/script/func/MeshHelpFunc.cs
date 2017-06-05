using UnityEngine;
using System.Collections.Generic;

public class MeshHelpFunc : Singleton<MeshHelpFunc> {

    private int[] EmtyArr = new int[] { };
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
        if (meshColli != null)
        {
            meshColli.sharedMesh = mesh;
        }
    }

    public Vector2 GetV2(float x, float y)
    {
        return Vector2.right * x + Vector2.up * y;
    }

    /// <summary>
    /// 设置圆环体内圈半径、环宽度 -- 使用mesh
    /// </summary>
    /// <param name="doorPath">圆环体</param>
    /// <param name="radius">圆环内圈半径</param>
    /// <param name="symmetrical">对称</param>
    public void SetDoubleSwingDoorMesh(GameObject doorPath, float radius, bool symmetrical = true)
    {
        //圆分割数
        int n = 15;

        float halfRadius = radius/2;

        float r1 = halfRadius;
        float r2 = halfRadius;
        if (symmetrical == false)
        {
            r1 = radius * 2 / 3;
            r2 = radius * 1 / 3;
        }

        //圆环宽度
        float w1 = r1 / 60f;
        //body宽度
        float w_1 = r1 / 30f;

        //圆环宽度
        float w2 = r2 / 60f;
        //body宽度
        float w_2 = r2 / 30f;

        tempUVs.Clear();
        tempv3s.Clear();
        triangles.Clear();

        float r1_1 = r1 - w1;

        for (int i = 0; i <= n; i++)
        {
            float angle = 1.5f * Mathf.PI + 0.5f * Mathf.PI / n * i;
            float x = r1_1 * Mathf.Cos(angle) - halfRadius;
            float y = r1_1 * Mathf.Sin(angle);
            tempUVs.Add(GetV2(x, y));
            tempv3s.Add(GetV2(x, y));
            x = r1 * Mathf.Cos(angle) - halfRadius;
            y = r1 * Mathf.Sin(angle);
            tempUVs.Add(GetV2(x, y));
            tempv3s.Add(GetV2(x, y));

            if (i + 1 > n)
            {
                break;
            }
            triangles.Add(2 * i);
            triangles.Add(2 * (i + 1));
            triangles.Add(2 * i + 1);

            triangles.Add(2 * i + 1);
            triangles.Add(2 * (i + 1));
            triangles.Add(2 * (i + 1) + 1);
        }

        //setBody
        {
            int startIndex = tempv3s.Count;

            tempUVs.Add(GetV2(-halfRadius, 0));
            tempv3s.Add(GetV2(-halfRadius, 0));

            tempUVs.Add(GetV2(-halfRadius, -r1));
            tempv3s.Add(GetV2(-halfRadius, -r1));

            tempUVs.Add(GetV2(-halfRadius + w_1, 0));
            tempv3s.Add(GetV2(-halfRadius + w_1, 0));

            tempUVs.Add(GetV2(-halfRadius + w_1, -r1));
            tempv3s.Add(GetV2(-halfRadius + w_1, -r1));

            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);

            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 3);
        }

        float r2_1 = r2 - w2;
        int Count = tempv3s.Count;
        for (int i = 0; i <= n; i++)
        {
            float angle = Mathf.PI + 0.5f * Mathf.PI / n * i;
            float x = r2_1 * Mathf.Cos(angle) + halfRadius;
            float y = r2_1 * Mathf.Sin(angle);
            tempUVs.Add(GetV2(x, y));
            tempv3s.Add(GetV2(x, y));
            x = r2 * Mathf.Cos(angle) + halfRadius;
            y = r2 * Mathf.Sin(angle);
            tempUVs.Add(GetV2(x, y));
            tempv3s.Add(GetV2(x, y));

            if (i + 1 > n)
            {
                break;
            }
            triangles.Add(Count + 2 * i);
            triangles.Add(Count + 2 * (i + 1));
            triangles.Add(Count + 2 * i + 1);

            triangles.Add(Count + 2 * i + 1);
            triangles.Add(Count + 2 * (i + 1));
            triangles.Add(Count + 2 * (i + 1) + 1);
        }

        //setBody
        {
            int startIndex = tempv3s.Count;

            tempUVs.Add(GetV2(halfRadius, 0));
            tempv3s.Add(GetV2(halfRadius, 0));

            tempUVs.Add(GetV2(halfRadius, -r2));
            tempv3s.Add(GetV2(halfRadius, -r2));

            tempUVs.Add(GetV2(halfRadius - w_2, 0));
            tempv3s.Add(GetV2(halfRadius - w_2, 0));

            tempUVs.Add(GetV2(halfRadius - w_2, -r2));
            tempv3s.Add(GetV2(halfRadius - w_2, -r2));

            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);

            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 3);
        }

        Mesh mesh = doorPath.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(tempv3s);
        mesh.SetUVs(0, tempUVs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshColli = doorPath.GetComponent<MeshCollider>();
        if (meshColli != null)
        {
            meshColli.sharedMesh = mesh;
        }
    }
    /// <summary>
    /// 设置圆环体内圈半径、环宽度 -- 使用mesh
    /// </summary>
    /// <param name="doorPath">圆环体</param>
    /// <param name="radius">圆环内圈半径</param>
    public void SetSingleSwingDoorMesh(GameObject doorPath, float radius)
    {
        //圆分割数
        int n = 15;
        //圆环宽度
        float w = radius / 60f;
        //body宽度
        float w_ = radius / 30f;

        tempUVs.Clear();
        tempv3s.Clear();
        triangles.Clear();

        float radius2 = radius - w;

        for (int i = 0; i < n; i++)
        {
            float angle = 1.5f * Mathf.PI +  0.5f * Mathf.PI / n * i;
            float x = radius2 * Mathf.Cos(angle) - radius / 2;
            float y = radius2 * Mathf.Sin(angle);
            tempUVs.Add(GetV2(x, y));
            tempv3s.Add(GetV2(x, y));
            x = radius * Mathf.Cos(angle) - radius / 2;
            y = radius * Mathf.Sin(angle);
            tempUVs.Add(GetV2(x, y));
            tempv3s.Add(GetV2(x, y));

            if (i + 1 >= n)
            {
                break;
            }
            triangles.Add(2 * i);
            triangles.Add(2 * ((i + 1) % n));
            triangles.Add(2 * i + 1);

            triangles.Add(2 * i + 1);
            triangles.Add(2 * ((i + 1) % n));
            triangles.Add(2 * ((i + 1) % n) + 1);
        }

        //setBody
        {
            int startIndex = tempv3s.Count;

            tempUVs.Add(GetV2(-radius / 2, 0));
            tempv3s.Add(GetV2(-radius / 2, 0));

            tempUVs.Add(GetV2(-radius / 2, -radius));
            tempv3s.Add(GetV2(-radius / 2, -radius));

            tempUVs.Add(GetV2(-radius / 2 + w_, 0));
            tempv3s.Add(GetV2(-radius / 2 + w_, 0));

            tempUVs.Add(GetV2(-radius / 2 + w_, -radius));
            tempv3s.Add(GetV2(-radius / 2 + w_, -radius));

            triangles.Add(startIndex + 0);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 2);

            triangles.Add(startIndex + 2);
            triangles.Add(startIndex + 1);
            triangles.Add(startIndex + 3);
        }

        Mesh mesh = doorPath.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(tempv3s);
        mesh.SetUVs(0, tempUVs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshColli = doorPath.GetComponent<MeshCollider>();
        if (meshColli != null)
        {
            meshColli.sharedMesh = mesh;
        }
    }

    //GameObject doorBody

    /// <summary>
    /// 设置圆环体内圈半径、环宽度 -- 使用mesh
    /// </summary>
    /// <param name="doorPath">圆环体</param>
    /// <param name="radius">圆环内圈半径</param>
    public void SetDoorBodyMesh(GameObject doorBody, float radius)
    {
        //圆环宽度
        float w = radius / 30f;

        tempUVs.Clear();
        tempv3s.Clear();
        triangles.Clear();

        tempUVs.Add(GetV2(-radius / 2, 0));
        tempv3s.Add(GetV2(-radius / 2, 0));

        tempUVs.Add(GetV2(-radius / 2, -radius));
        tempv3s.Add(GetV2(-radius / 2, -radius));

        tempUVs.Add(GetV2(-radius / 2 + w, 0));
        tempv3s.Add(GetV2(-radius / 2 + w, 0));

        tempUVs.Add(GetV2(-radius / 2 + w, -radius));
        tempv3s.Add(GetV2(-radius / 2 + w, -radius));

        triangles.Add(0);
        triangles.Add(1);
        triangles.Add(2);

        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(3);

        Mesh mesh = doorBody.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(tempv3s);
        mesh.SetUVs(0, tempUVs);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        MeshCollider meshColli = doorBody.GetComponent<MeshCollider>();
        if (meshColli != null)
        {
            meshColli.sharedMesh = mesh;
        }
    }
}
