namespace foundation
{
    public class Vector4X
    {

        public float x;
        public float y;
        public float z;
        public float w;
        public Vector4X(float x = 0.0f, float y = 0.0f, float z = 0.0f,float w=1.0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }


        public void setTo(float x = 0.0f, float y = 0.0f, float z = 0.0f,float w=1.0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public bool Equals(Vector4X value)
        {
            return x == value.x && y == value.y && z == value.z && w==value.w;
        }
    }
}
