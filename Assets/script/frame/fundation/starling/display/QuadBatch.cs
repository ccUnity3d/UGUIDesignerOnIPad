using foundation;
using System.Collections.Generic;
using UnityEngine;

namespace starling
{
    public class QuadBatch
    {
        public const int MAX_NUM_QUADS = 1000;

        private GameObject go;
        private Shader _shader;
        private Texture _texture;

        private int mNumQuads;

        private List<Vector3X> vertices;
        private List<Vector2X> uvs;
        private List<Color32> colors; 

        internal int numQuads
        {
            get { return mNumQuads; }
        }

        public QuadBatch()
        {
            vertices=new List<Vector3X>();
            uvs=new List<Vector2X>();
            colors=new List<Color32>();
        }

        internal bool isStateChange(Quad quad, float parentAlpha, int numQuads= 1,TextureX texture=null)
        {
            if (mNumQuads == 0) return false;
            else if (mNumQuads + numQuads > MAX_NUM_QUADS)
            {
                return true;
            }

            if (_texture !=texture.nativeTexture)
            {
                return true;
            }

            return false;
        }

        public void render(int drawCall,RenderSupport renderSupport)
        {
            if (mNumQuads == 0)
            {
                return;
            }

            MeshFilter meshFilter;
            MeshRenderer renderer;
            Mesh mesh;
            if (go == null)
            {
                go = new GameObject("crl");
                go.layer = LayerMask.NameToLayer("UI");
                meshFilter = go.AddComponent<MeshFilter>();
                renderer = go.AddComponent<MeshRenderer>();

                go.transform.parent = renderSupport.uiRoot.transform;

                go.transform.localPosition=Vector3.zero;
            }
            else
            {
                meshFilter = go.GetComponent<MeshFilter>();
                renderer = go.GetComponent<MeshRenderer>();
            }

            mesh = meshFilter.mesh;
            //mesh.Clear();

            int len = vertices.Count;
            Vector3[] verticeList=new Vector3[len];
            Vector3X tempV3;
            for (int i = 0; i < len; i++)
            {
                tempV3=vertices[i];
                verticeList[i] = new Vector3(tempV3.x, tempV3.y, tempV3.z);
            }
            len = uvs.Count;
            Vector2[] uvList = new Vector2[len];
            Vector2X tempV2;
            for (int i = 0; i < len; i++)
            {
                tempV2 = uvs[i];
                uvList[i] =new Vector2(tempV2.x,tempV2.y);
            }
            mesh.vertices = verticeList;
            mesh.uv = uvList;
            mesh.colors32= colors.ToArray();

            mesh.triangles=createIndices(vertices.Count);


            Material material=renderer.material;
            material.renderQueue = drawCall+6000;
            material.shader = _shader;
            material.color=new Color(1,1,1,1);
            if (_texture != null)
            {
                material.mainTexture = _texture;
            }
        }



        public int[] createIndices(int vertexCount)
        {
            int indexCount = (vertexCount >> 1)*3;
            int[] rv = new int[indexCount];
            int index = 0;

            for (int i = 0; i < vertexCount; i += 4)
            {
                rv[index++] = i;
                rv[index++] = i + 1;
                rv[index++] = i + 2;

                rv[index++] = i + 2;
                rv[index++] = i + 3;
                rv[index++] = i;
            }

            return rv;
        }

        internal void reset()
        {
            mNumQuads = 0;

            vertices.Clear();
            uvs.Clear();
            colors.Clear();
        }

        internal void addQuad(Quad quad, float parentAlpha,TextureX texture)
        {
            if (mNumQuads == 0)
            {
                this._shader = quad.shader;
                if (texture != null)
                {
                    this._texture = texture.nativeTexture;
                }
            }

            quad.copyVertexDataTo(vertices,uvs);

            for (int i = 0; i < 4; i++)
            {
                colors.Add(quad.color);
            }

            mNumQuads++;
        }
    }
}
