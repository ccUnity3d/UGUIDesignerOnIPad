using UnityEngine;

public class TestWallProxy
{
	private static TestWallProxy instance;
	public static TestWallProxy Instance
	{
		get{ 
			if (instance == null) {
				instance = new TestWallProxy ();
			}
			return instance;
		}
	}

	public Vector2[] realuv;
	public Vector2[] menuv;

	public TestWallProxy ()
	{
		realuv = new Vector2[4]{
			new Vector3(-5,0,0),
			new Vector3(-5,3,0),
			new Vector3(5,3,0),
			new Vector3(5,0,0)
		};
		menuv = new Vector2[5] {
			new Vector3 (1, 0, 0),
			new Vector3 (1, 2f, 0),
			new Vector3 (0, 2.95f, 0),
			new Vector3 (-1, 2f, 0),
			new Vector3 (-1, 0, 0)
		};

    }

    public void ResetMesh(Mesh mesh)
    {
        int len = realuv.Length + menuv.Length;
        Vector3[] vertices = new Vector3[len];
        Vector2[] uvs = new Vector2[len];
        for (int i = 0; i < len; i++)
        {
            Vector2 item;
            if (i < realuv.Length)
            {
                item = realuv[i];
                
            }
            else {
                item = menuv[i - realuv.Length];
            }
            vertices[i] = item;
            uvs[i] = item;
        }
        mesh.vertices = vertices;
        mesh.uv = uvs;
        
        Debug.Log("mesh.uv");
        for (int i = 0; i < mesh.uv.Length; i++)
        {
            Debug.Log(mesh.uv[i]);
        }

        mesh.triangles = TriangleSubdivision.TriangulatePolygon(uvs);

        Debug.Log("mesh.triangles");
        for (int i = 0; i < mesh.triangles.Length; i++)
        {
            Debug.Log(mesh.triangles[i]);
        }
    }
    
}


