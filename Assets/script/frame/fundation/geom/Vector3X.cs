using System;

namespace foundation
{
    public class Vector3X
    {

        public float x;
        public float y;
        public float z;
        public float w=1;

        public Vector3X(float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }


        public void setTo(float x = 0.0f, float y = 0.0f, float z = 0.0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool Equals(Vector3X value)
        {
            return x == value.x && y == value.y && z == value.z;
        }

        public Vector3X add(Vector3X pos)
        {

            this.x += pos.x;
            this.y += pos.y;
            this.z += pos.z;
            return this;
        }
    }
}
