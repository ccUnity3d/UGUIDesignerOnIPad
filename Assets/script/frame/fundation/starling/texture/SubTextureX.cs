using foundation;
using UnityEngine;

namespace starling
{
    public class SubTextureX:TextureX
    {
        private RectangleX mFrame;
        private TextureX mParent;

        private Transform2X mTransform;

        public SubTextureX(TextureX parentTexture, RectangleX region, RectangleX frame = null)
        {
            if (region == null)
            {
                region = new RectangleX(0, 0, parentTexture.width, parentTexture.height);
            }

            mParent = parentTexture;
            mFrame = frame !=null ? frame.clone() : null;

            mWidth = region.width;
            mHeight = region.height;


            mTransform=new Transform2X();

            mTransform.scale(region.width / mParent.width,
                                        region.height / mParent.height);
            mTransform.translate(region.x / mParent.width,
                                            -1-region.y / mParent.height);
        }

        public Transform2X transform
        {
            get { return mTransform; }
        }

        public override Texture nativeTexture
        {
            get { return mParent.nativeTexture; } 
        }

        public override TextureX root
        {
            get { return mParent.root; }
        }

        public override float height
        {
            get { return mHeight; }
        }

        public override float width
        {
            get { return mWidth; }
        }


        public override float nativeHeight
        {
            get { return mHeight* scale; }
        }

        public override float nativeWidth
        {
            get { return mWidth*scale; }
        }

        public override float scale
        {
            get { return mParent.scale; }
        }

        public TextureX parent
        {
            get { return mParent; }
        }
        public override RectangleX frame
        {
            get { return mFrame; }
        }

        private Transform2X _transform2X=new Transform2X();
        internal override Vector2X[] adjustUVs(Vector2X[] uv, Vector2X[] result = null)
        {
            SubTextureX texture = this;
            _transform2X.identity();
            while (texture !=null)
            {
                _transform2X= Transform2X.multiply(_transform2X,texture.transform);
                texture = texture.parent as SubTextureX;
            }

            Vector2X item;

            if (result == null)
            {
                result = new Vector2X[4];
            }
            for (int i = 0; i < 4; i++)
            {
                item = uv[i];
                result[i] =_transform2X.transformVector(item);
            }

            return result;
        }


        internal override Vector3X[] adjustVertices(Vector3X[] vertices, Vector3X[] result=null)
        {
            float deltaRight = mFrame.width + mFrame.x - mWidth;
            float deltaBottom = mFrame.height + mFrame.y - mHeight;
            if (result == null)
            {
                result = new Vector3X[4];
            }
            float[] deltaX = new float[] {-mFrame.x,-deltaRight,-deltaRight,-mFrame.x};
            float[] deltaY = new float[] { -mFrame.y, -mFrame.y, -deltaBottom,-deltaBottom };

            Vector3X temp;
            for (int i = 0; i < 4; i++)
            {
                temp = vertices[i];
                result[i]=new Vector3X(temp.x+deltaX[i],temp.y+deltaY[i]);
            }

            return result;
        }
    }
}
