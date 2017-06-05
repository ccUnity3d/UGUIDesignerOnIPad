using foundation;

namespace starling
{
    public class StageX : DisplayObjectContainerX
    {
        private int mWidth=50;
        private int mHeight=50;

        public int stageWidth
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        public int stageHeight
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

        public StageX()
        { 
        }

        private int _mouseX = 0;
        public int mouseX
        {
            get { return _mouseX; }
        }

        private int _mouseY = 0;
        public int mouseY
        {
            get { return _mouseY; }
        }

        private int _deltaMouseX = 0;
        public int deltaMouseX
        {
            get { return _deltaMouseX; }
        }

        private int _deltaMouseY = 0;
        public int deltaMouseY
        {
            get { return _deltaMouseY; }
        }

        public void updateMousePosition(Vector2X mousePos, Vector2X deltaPos)
        {
            _mouseX = (int)mousePos.x;
            _mouseY = (int)mousePos.y;
            _deltaMouseX = (int)deltaPos.x;
            _deltaMouseY = (int)deltaPos.y;

            //string message = string.Format("mx:{0},my:{1},dx:{2},dy:{3}", _mouseX, _mouseY, _deltaMouseX, _deltaMouseY);
            //Debug.Log(message);
        }


        public override void render(RenderSupport support, float parentAlpha)
        {
            float aspect = (float)stageWidth/ stageHeight;

            mSceneTransform.a = 2.0f/ stageWidth*aspect;
            mSceneTransform.d = -2.0f/stageHeight;

            mSceneTransform.tx = -aspect;
            mSceneTransform.ty = 1;

            base.render(support,parentAlpha);

            //Camera
        }
    }
}