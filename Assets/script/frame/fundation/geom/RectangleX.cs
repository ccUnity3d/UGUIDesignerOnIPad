using System;
using UnityEngine;

namespace foundation
{
    public class RectangleX
    {
        public float x=0.0f;
        public float y=0.0f;

        public float width = 0.0f;
        public float height = 0.0f;

        public float xMax
        {
            get { return x + width; }
        }

        public float yMax
        {
            get { return y + height; }
        }

        public RectangleX(float x=0, float y=0, float width=0, float height=0)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public RectangleX clone()
        {
            return new RectangleX(x,y,width,height);
        }


        public override string ToString()
        {
            return string.Format("rect: {0} {1} {2} {3}", x, y, width, height);
        }

        public bool Contains(Vector2X point)
        {
            if (point.x >= this.x && point.x < this.xMax && point.y >= (double)this.y)
                return point.y < this.yMax;
            return false;
        }

        public void setTo(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        internal Rect getRect()
        {
            return new Rect(x,y,width,height);
        }
    }
}
