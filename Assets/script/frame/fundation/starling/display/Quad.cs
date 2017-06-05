using foundation;
using System.Collections.Generic;
using UnityEngine;

namespace starling
{
    public class Quad : DisplayObjectX
    {
        private Color _color=new Color(1,1,1,1);

        protected Vector3X[] vertices;
        internal Vector3X[] translatorVertices;
        internal Vector2X[] uvs;

        private Shader _shader;

        public Color color
        {
            get { return _color; }
            set { _color = value; }
        }

        private bool _tinted = false;

        public bool tinted
        {
            get { return _tinted; }
        }

        internal Shader shader
        {
            get { return _shader; }
        }

        public static int indexer;

        public Quad(float w, float h):this()
        {
            init(w,h);
        }

        protected Quad() { 
            name = "a" + indexer++;
            _shader = Shader.Find("UI/Default");
        }

        protected void init(float w, float h)
        {
           
            vertices = new Vector3X[4];
            translatorVertices = new Vector3X[4];

            uvs = new Vector2X[4];

            vertices[0] = new Vector3X(0, 0);
            vertices[1] = new Vector3X(w, 0);
            vertices[2] = new Vector3X(w, h);
            vertices[3] = new Vector3X(0, h);


            uvs[0] = new Vector2X(0, 0);
            uvs[1] = new Vector2X(1, 0);
            uvs[2] = new Vector2X(1, -1);
            uvs[3] = new Vector2X(0, -1);

            
        }

        public override RectangleX getBounds(DisplayObjectX targetSpace)
        {
            RectangleX resultRect =new RectangleX();
            if (targetSpace == this) // optimization
            {
                resultRect.setTo(0.0f, 0.0f, vertices[2].x, vertices[2].y);
            }
            else if (targetSpace == parent && rotation == 0.0) // optimization
            {
                float scaleX = this.scaleX;
                float scaleY = this.scaleY;
                Vector3X v = vertices[2];

                resultRect.setTo(x - pivotX*scaleX, y - pivotY*scaleY,v.x*scaleX, v.y*scaleY);
                if (scaleX < 0)
                {
                    resultRect.width *= -1;
                    resultRect.x -= resultRect.width;
                }
                if (scaleY < 0)
                {
                    resultRect.height *= -1;
                    resultRect.y -= resultRect.height;
                }
            }
            else
            {
                getTransformationMatrix(targetSpace, sHelperTransform);

                float minX = float.MaxValue;
                float maxX = -float.MaxValue;
                float minY = float.MinValue;
                float maxY = -float.MinValue;

                int len = vertices.Length;
                for (int i = 0; i < len; i++)
                {
                    Vector3X v = sHelperTransform.transformVector(vertices[i]);

                    if (minX > v.x) minX = v.x;
                    if (maxX < v.x) maxX = v.x;
                    if (minY > v.y) minY = v.y;
                    if (maxY < v.y) maxY = v.y;
                }

                resultRect.setTo(minX, minY, maxX - minX, maxY - minY);
            }

            return resultRect;
        }

        public override void render(RenderSupport support, float parentAlpha)
        {
            int len = vertices.Length;
            for (int i = 0; i < len; i++)
            {
                translatorVertices[i] = mSceneTransform.transformVector(vertices[i]);
            }
            support.batchQuad(this, parentAlpha,null);
        }

        internal virtual void copyVertexDataTo(List<Vector3X> bufferVertices, List<Vector2X> bufferUvs)
        {
            bufferVertices.AddRange(translatorVertices);
            bufferUvs.AddRange(uvs);
        }
    }
}
