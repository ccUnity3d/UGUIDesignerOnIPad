using foundation;
using System.Collections.Generic;
using UnityEngine;

namespace clayui
{
    public abstract class ViewNode : EventDispatcher
    {
        protected ViewNode _parent;
        protected bool autoZIndex = true;

        internal int _firstDepth = int.MaxValue;
        internal int _zIndex = int.MaxValue;

        protected GameObject _skin;

        protected float _x = float.MaxValue;
        protected float _y = float.MaxValue;

        protected float _scaleY = float.MaxValue;
        protected float _scaleX = float.MaxValue;


        public int zIndex
        {
            get
            {

                if (_zIndex == int.MaxValue)
                {
                    return 0;
                }

                return _zIndex;
            }
        }

        protected List<ViewNode> children;

        public ViewNode()
        {
            children = new List<ViewNode>();
        }


        public float scaleX
        {
            get
            {
                if (_scaleX == float.MaxValue) return 1;
                return _scaleX;
            }
            set
            {
                if (_scaleX == value)
                {
                    return;
                }
                _scaleX = value;
                _updateScale();
            }
        }

        public float scaleY
        {
            get
            {
                if (_scaleX == float.MaxValue) return 1;

                return _scaleY;
            }
            set
            {
                if (_scaleY == value)
                {
                    return;
                }
                _scaleY = value;
                _updateScale();
            }
        }


        public float x
        {
            get
            {
                if (_x == float.MaxValue)
                {
                    return 0;
                }
                return _x;
            }
            set
            {
                if (_x == value)
                {
                    return;
                }
                _x = value;
                _updatePosition();
            }
        }

        public float y
        {
            get
            {
                if (_y == float.MaxValue)
                {
                    return 0;
                }
                return _y;
            }
            set
            {
                if (_y == value)
                {
                    return;
                }
                _y = value;
                _updatePosition();
            }
        }


        public ViewNode parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (_parent == value)
                {
                    return;
                }

                _parent = value;


                _updateParent();
            }
        }

        protected void _updateParent()
        {
            if (_parent == null)
            {
                if (_skin)
                {
                    _skin.SetActive(false);
                }
                return;
            }


            if (_skin != null && _parent.skin != null)
            {
                GameObject parentSkin = _parent.skin;
                if (parentSkin != null)
                {
                    _skin.transform.SetParent(parentSkin.transform);
                    _skin.transform.localPosition = Vector3.zero;
                    _skin.transform.localScale = new Vector3(1, 1, 1);
                    _skin.SetActive(isActive);

                    _updatePosition();
                    _updateScale();

                    foreach (ViewNode item in children)
                    {
                        item._updateParent();
                    }

                }
            }
        }


        public GameObject skin
        {
            get
            {
                return _skin;
            }

            set
            {

                if (_skin == value)
                {
                    return;
                }

                if (_skin != null)
                {
                    unbindComponent();
                }

                _skin = value;

                if (_skin != null)
                {

                    Vector3 pos = _skin.transform.localPosition;
                    if (_x == float.MaxValue)
                    {
                        _x = pos.x;
                    }
                    if (_y == float.MaxValue)
                    {
                        _y = pos.y;
                    }

                    pos = _skin.transform.localScale;
                    if (_scaleX == float.MaxValue)
                    {
                        _scaleX = pos.x;
                    }
                    if (_scaleY == float.MaxValue)
                    {
                        _scaleY = pos.y;
                    }

                    _updateParent();

                    bindComponent();

                    _skinCallLater();
                }
            }
        }

        protected virtual void _skinCallLater()
        {
        }


        protected void _updatePosition()
        {
            if (_skin != null)
            {
                Vector3 v = _skin.transform.localPosition;
                v.x = x;
                v.y = y;
                v.z = zIndex;
                _skin.transform.localPosition = v;
            }
        }


        protected void _updateScale()
        {
            if (_skin != null)
            {
                Vector3 v = _skin.transform.localScale;
                v.x = _scaleX;
                v.y = _scaleY;
                _skin.transform.localScale = v;
            }
        }


        protected virtual void unbindComponent()
        {
        }
        protected virtual void bindComponent()
        {
        }


        private bool isActive = true;
        public void setActive(bool value)
        {
            isActive = value;

            if (_skin != null)
            {
                _skin.SetActive(value);
            }
        }


        public void addChild(ViewNode node)
        {
            int index = children.IndexOf(node);
            if (index != -1)
            {
                children.RemoveAt(index);
            }
            this.addChildAt(node, children.Count);
        }

        public int addChildAt(ViewNode node, int index)
        {

            if (node == null)
            {
                return -1;
            }

            if (index >= children.Count)
            {
                index = children.Count;
                children.Add(node);
            }
            else
            {
                if (index < 0)
                {
                    index = 0;
                }
                children.Insert(index, node);
            }
            if (autoZIndex)
            {
                int len = children.Count;

                int depth = getFirstDepth();
                ViewNode item;
                for (int i = index; i < len; i++)
                {
                    item = children[i];
                    item._zIndex = depth - (i + 1);
                    item._updatePosition();
                }
            }

            node.parent = this;

            return index;
        }

        private int getFirstDepth()
        {
            if (_firstDepth == int.MaxValue && _skin != null)
            {
                int len = _skin.transform.childCount;
                float depth = 0;
                Transform item;
                for (int i = 0; i < len; i++)
                {
                    item = _skin.transform.GetChild(i);
                    depth = Mathf.Min(depth, item.localPosition.z);
                }
                _firstDepth = (int)depth - 1;
            }
            else
            {
                return 0;
            }

            return _firstDepth;
        }

        public bool removeChild(ViewNode node)
        {
            if (node == null)
            {
                return false;
            }

            int index = children.IndexOf(node);
            if (index == -1)
            {
                return false;
            }

            return removeChildAt(index) != null;
        }

        public ViewNode removeChildAt(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                return null;
            }
            ViewNode node = children[index];
            children.RemoveAt(index);
            node.parent = null;
            node.setActive(false);

            return node;
        }

        public ViewNode getChildAt(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                return null;
            }
            return children[index];
        }

        public bool containerChild(ViewNode node)
        {
            return children.Contains(node);
        }

        public int numbChidren()
        {
            return children.Count;
        }


        public void _dispose()
        {

            parent = null;
        }


        public GameObject FindChildGameObject(string path)
        {
            return _skin.transform.FindChild(path).gameObject;
        }

        public Transform FindChildTransform(string path)
        {
            return _skin.transform.FindChild(path);
        }
    }
}


