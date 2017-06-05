using foundation;
using System.Collections.Generic;

namespace starling
{
    public class Image : Quad
    {
        private TextureX _texture;
        private Vector2X[] translatorUVS=null;
        private bool uvInvalid = false;

        public TextureX texture
        {
            get { return _texture; }
            set
            {
                _texture = value;
                uvInvalid = true;
            }
        }


        public Image(TextureX texture)
        {
            _texture = texture;

            RectangleX frame = _texture.frame;
            float width = frame != null ? frame.width : texture.width;
            float height = frame != null ? frame.height : texture.height;

            this.init(width, height);
        }


        public void readjustSize()
        {
            RectangleX frame = _texture.frame;
            float w = frame != null ? frame.width : texture.width;
            float h = frame != null ? frame.height : texture.height;

            vertices[0] = new Vector3X(0, 0);
            vertices[1] = new Vector3X(w, 0);
            vertices[2] = new Vector3X(w, h);
            vertices[3] = new Vector3X(0, h);
        }

        public override void render(RenderSupport support, float parentAlpha)
        {
            support.batchQuad(this, parentAlpha, _texture);
        }

        public static Vector3X[] helperVertices;
        internal override void copyVertexDataTo(List<Vector3X> bufferVertices, List<Vector2X> bufferUvs)
        {
            if (uvInvalid || translatorUVS==null)
            {
                translatorUVS = _texture.adjustUVs(uvs, translatorUVS);
            }
            helperVertices = _texture.adjustVertices(vertices);

            int len = helperVertices.Length;
            for (int i = 0; i < len; i++)
            {
                translatorVertices[i] = mSceneTransform.transformVector(helperVertices[i]);
            }
            bufferUvs.AddRange(translatorUVS);
            bufferVertices.AddRange(translatorVertices);
            
        }
    }
}
