using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MyCube : MonoBehaviour {
    //private Mesh mesh;
    void Start()
    {
        Vector3 v3 = transform.TransformPoint(Vector3.up);
        Debug.LogWarning(v3);
        //mesh = transform.GetComponent<MeshFilter>().mesh;
        //for (int i = 0; i < mesh.vertices.Length; i++)
        //{
        //    Debug.LogWarning(mesh.vertices[i]);
        //}
    }

    //public bool useMeshColli = false;
    //public Rect areaTo9 = Rect.MinMaxRect(0, 0, 1, 1);
    ///// <summary>
    ///// 默认材质
    ///// </summary>
    //public Material defaultMaterial = null;
    //public Vector3 size
    //{
    //    get {
    //        return _size;
    //    }
    //    set {
    //        _size = value;
    //        Resize();
    //    }
    //}

    //private Transform F;
    //private Transform B;
    //private Transform R;
    //private Transform L;
    //private Transform U;
    //private Transform D;

    //private Material[] materials = new Material[3];
    //private Vector3 _size = Vector3.one;
    //private int[] EmtyArr = new int[] { };
    //private UVS uVS = new UVS();
    //public Mesh mesh;
    //public GameObject prefab = null;

    //void Start()
    //{
    //    Creat("F", ref F);
    //    Creat("B", ref B, Vector3.up * 180);
    //    Creat("R", ref R, Vector3.up * 270);
    //    Creat("L", ref L, Vector3.up * 90);
    //    Creat("U", ref U, Vector3.right * 90);
    //    Creat("D", ref D, Vector3.right * 270);
    //    Resize();
    //}

    //private void Creat(string name, ref Transform tran, Vector3 angle = default(Vector3))
    //{
    //    GameObject go = GetObj();
    //    go.name = name;
    //    tran = go.transform;
    //    tran.SetParent(this.transform);
    //    tran.rotation = Quaternion.Euler(angle);
    //}

    //private GameObject GetObj()
    //{
    //    if (prefab == null) {
    //        prefab = GameObject.CreatePrimitive(PrimitiveType.Quad);
    //        prefab.transform.SetParent(this.transform);
    //        prefab.SetActive(false);
    //        if (useMeshColli == false) GameObject.DestroyImmediate(prefab.GetComponent<MeshCollider>(), true);
    //        mesh = prefab.GetComponent<MeshFilter>().mesh;
    //        for (int i = 0; i < mesh.vertices.Length; i++)
    //        {
    //            Debug.LogWarning(mesh.vertices[i]);
    //        }
    //    }
    //    GameObject go = GameObject.Instantiate(prefab);
    //    go.GetComponent<MeshFilter>().sharedMesh = mesh;
    //    go.SetActive(true);
    //    return go;
    //}

    //private void Resize()
    //{
    //    //mesh.SetTriangles(EmtyArr, 0);
    //    //uVS.SetUVs(areaTo9);
    //    //List<Vector3> vertices = uVS.getVertices();
    //    //mesh.SetVertices(vertices);
    //    //List<Vector2> uvs = uVS.getUVs();
    //    //mesh.SetUVs(0, uvs);
    //    //int[] triangles = uVS.getTriangles();
    //    //mesh.SetTriangles(triangles, 0);
    //    //mesh.RecalculateNormals();
    //    //mesh.RecalculateBounds();
    //    ResetPos();
    //}

    //private void ResetPos()
    //{
    //    F.localPosition = -Vector3.forward * size.z / 2;
    //    B.localPosition = Vector3.forward * size.z / 2;
    //    R.localPosition = Vector3.right * size.z / 2;
    //    L.localPosition = -Vector3.right * size.z / 2;
    //    U.localPosition = Vector3.up * size.z / 2;
    //    D.localPosition = -Vector3.up * size.z / 2;
    //}

    //public class UVS
    //{
    //    public List<float> xs = new List<float>();
    //    public List<float> ys = new List<float>();

    //    private List<Vector2> v2list = new List<Vector2>();
    //    private List<Vector3> v3list = new List<Vector3>();
    //    private List<int> list = new List<int>();

    //    public List<Vector2> getUVs()
    //    {
    //        return v2list;
    //    }

    //    public List<Vector3> getVertices()
    //    {
    //        return v3list;
    //    }

    //    public int[] getTriangles()
    //    {
    //        return list.ToArray();
    //    }

    //    public void SetUVs(Rect areaTo9)
    //    {
    //        int count = 4;
    //        bool left = areaTo9.x > 0;
    //        bool right = areaTo9.y >= areaTo9.x && areaTo9.y < 1;
    //        bool up = areaTo9.size.x > 0;
    //        bool down = areaTo9.size.y >= areaTo9.size.x && areaTo9.size.y < 1;
    //        xs.Add(0);
    //        if (left)
    //        {
    //            count += 2;
    //            xs.Add(areaTo9.x);
    //        }
    //        if (right)
    //        {
    //            count += 2;
    //            xs.Add(areaTo9.y);
    //        }
    //        xs.Add(1);
    //        int n = count / 2;
    //        ys.Add(0);
    //        if (up)
    //        {
    //            count += n;
    //            ys.Add(areaTo9.size.x);
    //        }
    //        if (down)
    //        {
    //            count += n;
    //            ys.Add(areaTo9.size.y);
    //        }
    //        ys.Add(1);

    //        v3list.Clear();
    //        v2list.Clear();
    //        list.Clear();
    //        for (int k = 0; k < ys.Count; k++)
    //        {
    //            for (int i = 0; i < xs.Count; i++)
    //            {
    //                v3list.Add(Vector3.right * (xs[i] - 0.5f) + Vector3.up * (ys[i] - 0.5f));
    //                v2list.Add(Vector2.right * xs[i] + Vector2.up * ys[i]);
    //                if (i + 1 < xs.Count && k + 1 < ys.Count)
    //                {
    //                    list.Add(xs.Count * (i + 1) + (k));
    //                    list.Add(xs.Count * (i) + (k + 1));
    //                    list.Add(xs.Count * (i) + (k));
    //                    list.Add(xs.Count * (i) + (k + 1));
    //                    list.Add(xs.Count * (i + 1) + (k));
    //                    list.Add(xs.Count * (i + 1) + (k + 1));
    //                }
    //            }              
    //        }

    //    }

    //}
}
