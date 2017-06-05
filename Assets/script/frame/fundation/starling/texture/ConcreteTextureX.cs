using UnityEngine;

namespace starling
{
    public class ConcreteTextureX:TextureX
    {
        private float mScale = 1;
        public ConcreteTextureX(Texture texture)
        {
            mNativeTexture = texture;

            mWidth = texture.width;
            mHeight = texture.height;
            
        }

        public override float height
        {
            get { return mHeight / mScale; }
        }

        public override float nativeHeight
        {
            get { return mHeight; }
        }

        public override float nativeWidth
        {
            get { return mWidth; }
        }

        public override float width
        {
            get { return mWidth / mScale; }
        }
    }
}
