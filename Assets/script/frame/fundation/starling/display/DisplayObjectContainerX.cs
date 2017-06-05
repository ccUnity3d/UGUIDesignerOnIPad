using foundation;
using System;
using System.Collections.Generic;

namespace starling
{
    public class DisplayObjectContainerX : DisplayObjectX
    {
        private bool mTouchGroup = false;
        internal List<DisplayObjectX> mChildren = new List<DisplayObjectX>();

        public DisplayObjectContainerX()
        {

        }

        public List<DisplayObjectX> children
        {
            get { return mChildren; }
        }

        public virtual DisplayObjectX addChild(DisplayObjectX child)
        {
            addChildAt(child, numChildren);
            return child;
        }

        public virtual DisplayObjectX addChildAt(DisplayObjectX child, int index)
        {
            int numChildren = mChildren.Count;

            if (index >= 0 && index <= numChildren)
            {
                if (child.parent == this)
                {
                    setChildIndex(child, index); // avoids dispatching events
                }
                else
                {
                    child.removeFromParent();

                    // 'splice' creates a temporary object, so we avoid it if it's not necessary
                    if (index == numChildren)
                    {
                        mChildren.Add(child);
                    }
                    else
                    {
                        mChildren.Insert(index, child);
                    }

                    child.setParent(this);
                    //child.dispatchEventWith(EventX.ADDED, true);

                    if (stage != null)
                    {
                        DisplayObjectContainerX container = child as DisplayObjectContainerX;
                        if (container != null)
                        {
                            //递归上去;
                            //container.simpleDispatch(EventX.ADDED_TO_STAGE);
                        }
                        else
                        {
                            child.simpleDispatch(EventX.ADDED_TO_STAGE);
                        }
                    }
                }

                return child;
            }
            else
            {
                throw new RankException();
            }
        }

        public int getChildIndex(DisplayObjectX child)
        {
            return mChildren.IndexOf(child);
        }

        public DisplayObjectX getChildAt(int index)
        {
            if (index >= 0 && index < numChildren)
            {
                return mChildren[index];
            }
            else
            {
                throw new RankException("Invalid child index");
            }
        }

        public DisplayObjectX getChildByName(string name)
        {
            int numChildren = mChildren.Count;
            for (int i = 0; i < numChildren; ++i)
            {
                if (mChildren[i].name == name)
                {
                    return mChildren[i];
                }
            }
            return null;
        }

        public void setChildIndex(DisplayObjectX child, int index)
        {
            int oldIndex = getChildIndex(child);
            if (oldIndex == index) return;
            if (oldIndex == -1)
            {
                throw new ArgumentException("Not a child of this container");
            }

            mChildren.RemoveAt(oldIndex);

            index = Math.Min(numChildren, index);
            mChildren.Insert(index, child);
        }

        public DisplayObjectX removeChild(DisplayObjectX child, bool dispose = false)
        {
            int childIndex = getChildIndex(child);
            if (childIndex != -1) removeChildAt(childIndex, dispose);
            return child;
        }

        public void removeAllChildren()
        {
            while (numChildren > 0)
            {
                removeChildAt(0);
            }
        }

        public bool contains(DisplayObjectX child)
        {
            while (child != null)
            {
                if (child == this) return true;
                else child = child.parent;
            }
            return false;
        }

        public DisplayObjectX removeChildAt(int index, bool dispose = false)
        {
            if (index >= 0 && index < numChildren)
            {
                DisplayObjectX child = mChildren[index];
                child.simpleDispatch(EventX.REMOVED);

                if (stage != null)
                {
                    DisplayObjectContainerX container = child as DisplayObjectContainerX;
                    if (container != null)
                    {
                        container.broadcastEventWith(EventX.REMOVED_FROM_STAGE);
                    }
                    else
                    {
                        child.simpleDispatch(EventX.REMOVED_FROM_STAGE);
                    }
                }

                child.setParent(null);
                index = mChildren.IndexOf(child);
                if (index >= 0)
                {
                    mChildren.RemoveAt(index);
                }
                if (dispose)
                {
                    child.dispose();
                }

                return child;
            }
            else
            {
                throw new RankException("Invalid child index");
            }
        }

        /// <summary>
        ///  上抛事件;
        /// </summary>
        /// <param name="eventType"></param>
        private void broadcastEventWith(string eventType)
        {

        }

        public int numChildren
        {
            get { return mChildren.Count; }
        }


        public override RectangleX getBounds(DisplayObjectX targetSpace)
        {
            RectangleX resultRect = new RectangleX();


            int numChildren = mChildren.Count;

            if (numChildren == 0)
            {
                getTransformationMatrix(targetSpace, sHelperTransform);
                //todo;
                resultRect.setTo(0, 0, 0, 0);
            }
            else if (numChildren == 1)
            {
                resultRect = mChildren[0].getBounds(targetSpace);
            }
            else
            {
                float minX = float.MaxValue;
                float maxX = -float.MaxValue;
                float minY = float.MinValue;
                float maxY = -float.MinValue;

                for (int i = 0; i < numChildren; ++i)
                {
                    resultRect = mChildren[i].getBounds(targetSpace);

                    if (minX > resultRect.x) minX = resultRect.x;
                    if (maxX < resultRect.width) maxX = resultRect.width;
                    if (minY > resultRect.y) minY = resultRect.y;
                    if (maxY < resultRect.height) maxY = resultRect.height;
                }

                resultRect.setTo(minX, minY, maxX - minX, maxY - minY);
            }

            return resultRect;
        }


        public override DisplayObjectX hitTest(Vector2X localVector, bool forTouch = false)
        {
            if (forTouch && (visible == false || touchable == false))
            {
                return null;
            }
            float localX = localVector.x;
            float localY = localVector.y;
            int numChildren = mChildren.Count;

            DisplayObjectX child;
            DisplayObjectX target;
            Vector2X transformVector;
            for (int i = numChildren - 1; i >= 0; --i) // front to back!
            {
                child = mChildren[i];
                getTransformationMatrix(child, sHelperTransform);

                transformVector = sHelperTransform.transformVector(localVector);

                target = child.hitTest(transformVector);

                if (target != null)
                {
                    return forTouch && mTouchGroup ? this : target;
                }
            }

            return null;
        }

        public override void render(RenderSupport support, float parentAlpha)
        {
            float alpha = parentAlpha*this.alpha;
            int numChildren = mChildren.Count;


            for (int i = 0; i < numChildren; ++i)
            {
                DisplayObjectX child = mChildren[i];

                if (child.hasVisibleArea)
                {
                    child.sceneTransform = Transform2X.multiply(mSceneTransform, child.transform);
                    support.blendMode = child.blendMode;

//                    if (filter)
//                    {
//                        filter.render(child, support, alpha);
//                    }
//                    else
//                    {
                        child.render(support, alpha);
//                    }

                    support.blendMode = blendMode;
                }
            }
        }
    }
}