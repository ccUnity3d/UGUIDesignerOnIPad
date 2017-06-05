using System;
using foundation;

namespace starling
{
    public class TouchSprite:DisplayObjectContainerX
    {
        private static int MIN_DISTANCE_SQ=16;
        private static DisplayObjectX _draggingTarget =null;
		/**
		 * 是否正在拖动过程中;
		 * @return
		 *
		 */		
		public static bool isDragging{
		    get { return _draggingTarget != null; }
		}
        private float mx0;
		private float my0;
		private float mx;
		private float my;

        protected float _width;
		protected float _height;

        private bool _isDragging = false;

        public float draggingDistanceX=0;
		public float draggingDistanceY=0;
		public float scrollSpeed=5;
		

        private StageX _stage;
        public TouchSprite()
        {
            this.addEventListener(MouseEventX.MOUSE_DOWN, touchHandle);
            this.addEventListener(MouseEventX.MOUSE_UP, touchHandle);
        }

        private void touchHandle(EventX e)
        {
            Vector2X p = (Vector2X) e.data;
            float stageX=p.x;
            float stageY=p.y;

            switch (e.type)
            {
                case MouseEventX.MOUSE_DOWN:
                    mx = mx0 = stageX;
                    my = my0 = stageY;

                    _stage = stage;

                    _stage.addEventListener(MouseEventX.MOUSE_MOVE, touchMoveHandle);
                    _stage.addEventListener(MouseEventX.MOUSE_UP, touchHandle);
                    break;

                case MouseEventX.MOUSE_UP:

                    _stage.removeEventListener(MouseEventX.MOUSE_MOVE, touchMoveHandle);
                    _stage.removeEventListener(MouseEventX.MOUSE_UP, touchHandle);

                    if (_isDragging)
                    {
                        touchEnd(stageX - mx, stageY - my, stageX - mx0, stageY - my0);
                        if (hasEventListener(TouchEventX.TOUCH_END))
                        {
                            this.dispatchEvent(new TouchEventX(TouchEventX.TOUCH_END, stageX - mx, stageY - my, stageX - mx0, stageY - my0));
                        }
                        _isDragging = false;
                        CallLater.Add(touchEndHandle);
                    }

                    break;
                    
            }
        }

        private void touchEndHandle()
        {
            if (_draggingTarget == this)
            {
                _draggingTarget = null;
            }
        }


        private void touchMoveHandle(EventX e)
        {
            Vector2X p = (Vector2X)e.data;
            float stageX = p.x;
            float stageY = p.y;

            float dx = stageX - mx0;
            float dy = stageY - my0;

            if (!_isDragging)
            {
                if (Math.Abs(dx) < draggingDistanceX || Math.Abs(dy) < draggingDistanceY || (dx * dx + dy * dy) < MIN_DISTANCE_SQ) return;
                _isDragging = true;
                _draggingTarget = this;
                CallLater.Remove(touchEndHandle);

                touchStart(dx, dy);//这里面可能会有需求再变为不可拖;

                if (hasEventListener(TouchEventX.TOUCH_START))
                {
                    this.dispatchEvent(new TouchEventX(TouchEventX.TOUCH_START, 0, 0, dx, dy));
                }
            }

            touchMoving(stageX - mx, stageY - my, dx, dy);
            if (hasEventListener(TouchEventX.TOUCH_START))
            {
                this.dispatchEvent(new TouchEventX(TouchEventX.TOUCH_START, stageX - mx, stageY - my, dx, dy));
            }

            mx = stageX;
            my = stageY;
        }


        protected virtual void touchStart(float dx, float dy)
        {
      
        }
        protected virtual void touchMoving(float speedX, float speedY, float dx, float dy)
        {
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        protected virtual void touchEnd(float speedX, float speedY, float dx, float dy)
        {
            
        }


        public override float width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    updateSize();
                }
            }
        }

        public override float height
        {
            get { return _height; }
            set
            {
                if (_height != value)
                {
                    _height = value;
                    updateSize();
                }
            }
        }


        public void setSize(float width, float height){
			if(_width != width){
				_width=width;
			}
			if(_height != height){
				_height=height;
			}
            updateSize();
		}

        protected virtual void updateSize()
        {
      
        }
    }
}
