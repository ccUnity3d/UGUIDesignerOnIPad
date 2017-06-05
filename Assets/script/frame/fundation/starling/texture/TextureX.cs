using foundation;
using UnityEngine;

namespace starling
{
    public abstract class TextureX
    {
        protected Texture mNativeTexture;

        protected float mWidth;
        protected float mHeight;

        public static TextureX fromNativeTexture(Texture texture)
        {
            ConcreteTextureX concreteTexture=new ConcreteTextureX(texture);

            return concreteTexture;
        }

        public static TextureX fromTexture(TextureX texture, RectangleX region = null, RectangleX frame = null)
        {
            return new SubTextureX(texture, region, frame);
        }

        public virtual Texture nativeTexture
        {
            get { return mNativeTexture; }
        }

        public virtual TextureX root
        {
            get { return null; }
        }

        public virtual RectangleX frame
        {
            get { return null; }
        }

        public bool repeat
        {
            get
            {
                return false;
            }
        }

        public abstract float width { get; }

        public abstract float height { get; }

        public abstract float nativeWidth { get; }

        public abstract float nativeHeight { get; }

        public virtual  float scale
        {
            get { return 1.0f; }
        }

        public bool mipMapping
        {
            get { return false; }
        }

        internal virtual Vector2X[] adjustUVs(Vector2X[] uvs,Vector2X[] result=null)
        {
            return uvs;
        }

        internal virtual Vector3X[] adjustVertices(Vector3X[] vertices,Vector3X[] result=null)
        {
            return vertices;
        }
    }
}
