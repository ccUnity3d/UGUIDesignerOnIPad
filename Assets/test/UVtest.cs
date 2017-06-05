using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UVtest : MonoBehaviour {
    private int[] EmtyArr = new int[] { };

    private List<Vector2> tempUVs = new List<Vector2>()
    {
        Vector2.up, Vector2.one
        ,Vector2.up*0.9f, Vector2.right + Vector2.up * 0.9f
        ,Vector2.up*0.1f, Vector2.right + Vector2.up * 0.1f
        ,Vector2.zero, Vector2.right
    };
    private List<int> tempTTriangles = new List<int>()
    {
         0,1,2//2,1,0//
        ,3,2,1//1,2,3//
        ,2,3,4//4,3,2//
        ,5,4,3//3,4,5//
        ,4,5,6//6,5,4//
        ,7,6,5//5,6,7//
    };
    private List<Vector3> tempv3s = new List<Vector3>();

    // Use this for initialization
    void Start () {
        tempv3s.Clear();
        tempv3s.Add(Vector2.up);
        tempv3s.Add(Vector2.one);
        tempv3s.Add(Vector2.up*0.9f);
        tempv3s.Add(Vector2.up * 0.9f + Vector2.right);
        tempv3s.Add(Vector2.up * 0.1f);
        tempv3s.Add(Vector2.up * 0.1f + Vector2.right);
        tempv3s.Add(Vector2.zero);
        tempv3s.Add(Vector2.right);
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        mesh.SetTriangles(EmtyArr, 0);
        mesh.SetVertices(tempv3s);
        mesh.SetUVs(0, tempUVs);
        mesh.SetTriangles(tempTTriangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float dis;
            bool hit = RaycastMeshFunc.Instance.RaycastMesh(Input.mousePosition, this.gameObject, out dis);
            Debug.LogWarning("hit == " + hit);
        }
    }
	
}
