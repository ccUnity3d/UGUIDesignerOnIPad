namespace foundation
{
    public class Vector2X
    {
        public float x;
        public float y;

        public Vector2X(float x = 0.0f, float y = 0.0f)
        {
            this.x = x;
            this.y = y;
        }

        public void setTo(float x=0.0f, float y=0.0f)
        {
            this.x = x;
            this.y = y;
        }
    }
}
