using System;
using System.Collections.Generic;
using foundation;

namespace starling
{
    public abstract class DisplayObjectX : EventDispatcher
    {
        public  static Transform2X sHelperTransform=new Transform2X();
        private static List<DisplayObjectX> sAncestors= new List<DisplayObjectX>();

        protected bool _disposed = false;
        private float mX;
        private float mY;
        private float mPivotX;
        private float mPivotY;
        private float mScaleX;
        private float mScaleY;

        private float mRotation;
        private float mAlpha;
        private bool mVisible;
        private bool mTouchable;
        private string mBlendMode;
        private string mName;

        private DisplayObjectContainerX mParent;

        private bool mOrientationChanged = true;

        internal Transform2X mTransform;
        internal Transform2X mSceneTransform;
        internal Transform2X mSceneInvertTransform;


        public DisplayObjectX()
        {

            mX = mY = mPivotX = mPivotY = 0.0f;
            mRotation = 0;
            mScaleX = mScaleY = mAlpha = 1.0f;
            mVisible = true;
            mTouchable = false;
            mBlendMode = BlendModeX.AUTO;

            mTransform = new Transform2X();

            mSceneTransform = new Transform2X();
            mSceneInvertTransform=new Transform2X();
        }

        public bool hasVisibleArea
        {
            get { return mAlpha != 0.0 && mVisible && mScaleX != 0.0 && mScaleY != 0.0; }
        }

        public float x
        {
            get { return mX; }

            set
            {
                if (mX != value)
                {
                    mX = value;
                    mOrientationChanged = true;
                }
            }
        }


        public Transform2X getTransformationMatrix(DisplayObjectX targetSpace,Transform2X resultMatrix = null) {
            DisplayObjectX commonParent;
            DisplayObjectX currentObject;

            if (resultMatrix != null)
            {
                resultMatrix.identity();
            }
            else
            {
                resultMatrix = new Transform2X();
            }

            if (targetSpace == this)
            {
                return resultMatrix;
            }
            else if (targetSpace == mParent || (targetSpace == null && mParent == null))
            {
                resultMatrix.copyFrom(transform);
                return resultMatrix;
            }
            else if (targetSpace == null || targetSpace == stage)
            {
                // targetCoordinateSpace 'null' represents the target space of the base object.
                // -> move up from this to base

                currentObject = this;
                while (currentObject != targetSpace)
                {
                    resultMatrix = Transform2X.multiply(resultMatrix, currentObject.transform);
                    currentObject = currentObject.mParent;
                }

                return resultMatrix;
            }
            else if (targetSpace.mParent == this) // optimization
            {
                targetSpace.getTransformationMatrix(this, resultMatrix);
                resultMatrix.invert();

                return resultMatrix;
            }

            // 1. find a common parent of this and the target space
            commonParent = findCommonParent(this, targetSpace);
            // 2. move up from this to common parent

            currentObject = this;
            while (currentObject != commonParent)
            {
                resultMatrix = Transform2X.multiply(resultMatrix, currentObject.transform);
                currentObject = currentObject.mParent;
            }

            if (commonParent == targetSpace)
            {
                return resultMatrix;
            }
            // 3. now move up from target until we reach the common parent

            sHelperTransform.identity();
            currentObject = targetSpace;
            while (currentObject != commonParent)
            {
                sHelperTransform = Transform2X.multiply(sHelperTransform, currentObject.transform);
                currentObject = currentObject.mParent;
            }

            // 4. now combine the two matrices

            sHelperTransform.invert();
            resultMatrix = Transform2X.multiply(resultMatrix, sHelperTransform);

            return resultMatrix;
        }

        private DisplayObjectX findCommonParent(DisplayObjectX object1, DisplayObjectX object2)
        {
            DisplayObjectX currentObject = object1;

            while (currentObject != null)
            {
                sAncestors.Add(currentObject);
                currentObject = currentObject.mParent;
            }

            currentObject = object2;
            while (currentObject != null && sAncestors.IndexOf(currentObject) == -1)
            {
                currentObject = currentObject.mParent;
            }

            sAncestors.Clear();

            if (currentObject != null)
            {
                return currentObject;
            }

            throw new ArgumentException("Object not connected to target");
        }


        public virtual RectangleX getBounds(DisplayObjectX targetSpace)
        {
            throw new NotImplementedException();
        }

        public virtual DisplayObjectX hitTest(Vector2X localVector, bool forTouch = false)
        {
            if (!forTouch && (!mVisible || !mTouchable))
            {
                return null;
            }

            // otherwise, check bounding box
            if (getBounds(this).Contains(localVector))
            {
                return this;
            }
            else return null;
        }

        public void removeFromParent(bool dispose = false)
        {
            if (mParent != null)
            {
                mParent.removeChild(this, dispose);
            }
            else if (dispose)
            {
                this.dispose();
            }
        }

        public float y
        {
            get { return mY; }

            set
            {
                if (mY != value)
                {
                    mY = value;
                    mOrientationChanged = true;
                }
            }
        }

        internal void setParent(DisplayObjectContainerX value)
        {
            DisplayObjectX ancestor = value;
            while (ancestor != this && ancestor != null)
            {
                ancestor = ancestor.mParent;
            }
            if (ancestor == this)
            {
                throw new ArgumentException(
                    "An object cannot be added as a child to itself or one of its children (or children's children, etc.)");
            }
            else
            {
                mParent = value;
            }
        }

        public float scaleX
        {
            get { return mScaleX; }

            set
            {
                if (mScaleX != value)
                {
                    mScaleX = value;
                    mOrientationChanged = true;
                }
            }
        }

        public float scaleY
        {
            get { return mScaleY; }

            set
            {
                if (mScaleY != value)
                {
                    mScaleY = value;
                    mOrientationChanged = true;
                }
            }
        }

        public float pivotX
        {
            get { return mPivotX; }

            set
            {
                if (mPivotX != value)
                {
                    mPivotX = value;
                    mOrientationChanged = true;
                }
            }
        }

        public float pivotY
        {
            get { return mPivotY; }

            set
            {
                if (mPivotY != value)
                {
                    mPivotY = value;
                    mOrientationChanged = true;
                }
            }
        }

        public float rotation
        {
            set
            {
                value = MathExtendUtils.normalizeAngle(value);
                if (mRotation != value)
                {
                    mRotation = value;
                    mOrientationChanged = true;
                }
            }
            get { return mRotation; }
        }



        public float alpha
        {
            get { return mAlpha; }
            set { mAlpha = value < 0.0f ? 0.0f : (value > 1.0f ? 1.0f : value); }
        }

        public bool visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        public bool touchable
        {
            get { return mTouchable; }
            set { mTouchable = value; }
        }

        public string name
        {
            get { return mName; }
            set { mName = value; }
        }

        public string blendMode
        {
            get { return mBlendMode; }
            set { mBlendMode = value; }
        }

        public virtual float width
        {
            get { return getBounds(mParent).width; }
            set
            {
                scaleX = 1.0f;
                float actualSize = width;
                if (actualSize != 0.0) scaleX = value / actualSize;
            }
        }

        public virtual float height
        {
            get { return getBounds(mParent).height; }
            set
            {
                scaleX = 1.0f;
                float actualSize = height;
                if (actualSize != 0.0) scaleX = value / actualSize;
            }
        }

        public DisplayObjectContainerX parent
        {
            get { return mParent; }
        }

        public StageX stage
        {
            get
            {
                DisplayObjectX currentObject = this;
                while (currentObject.mParent != null)
                {
                    currentObject = currentObject.mParent;
                }
                return currentObject as StageX;
            }
        }

        public Transform2X transform
        {
            get
            {
                if (mOrientationChanged)
                {
                    mOrientationChanged = false;

                    mTransform.identity();
                    if (mRotation == 0.0f)
                    {
                        mTransform.a = mScaleX;
                        mTransform.d = mScaleY;

                        mTransform.tx = mX - mPivotX * mScaleX;
                        mTransform.ty = mY - mPivotY * mScaleY;
                    }
                    else
                    {
                        double angle = Math.PI*(mRotation% 360d)/180d;
                        float cos = Convert.ToSingle(Math.Cos(angle));
                        float sin = Convert.ToSingle(Math.Sin(angle));
                        float a = mScaleX*cos;
                        float b = mScaleX*sin;
                        float c = mScaleY*-sin;
                        float d = mScaleY*cos;
                        float tx = mX - mPivotX*a - mPivotY*c;
                        float ty = mY - mPivotX*b - mPivotY*d;

                        mTransform.a = a;
                        mTransform.c = c;
                        mTransform.b = b;
                        mTransform.d = d;

                        mTransform.tx = tx;
                        mTransform.ty = ty;

                    }

                }
                return mTransform;
            }
        }

        public Transform2X sceneTransform
        {
            get { return mSceneTransform; }
            internal set
            {
                mSceneTransform = value;
                mSceneInvertTransform = null;
            }
        }

        public virtual void render(RenderSupport support, float parentAlpha)
        {
        }
    }
}