using foundation;
using System;

namespace starling
{
    public class Scroller : TouchSprite
    {
        public static int barSize = 5;
        public static uint clr = 0x999999;
        protected static EventX scrollEvent = new EventX(EventX.SCROLL);

        /// <summary>
        /// 衰减参数(阻力) 
        /// </summary>
        protected float _decay = 0.93f;
        protected float _mouseDecay = 0.5f;

        /// <summary>
        /// 速度 
        /// </summary>
        protected float _velocityX = 0.0f;
        protected float _velocityY = 0.0f;
        protected bool _mouseDown = false;

        protected static float SPEED_SPRINGNESS = 0.4f;
        protected static float BOUNCING_SPRINGESS = 0.6f;
        protected static float MIN = 0.5f;

        public int miniSize = 10;

        protected Bar horizontalBar;
        protected Bar verticalBar;
        protected IScrollable _content;
        protected DisplayObjectX _contentView;

        protected int _contentFullWidth;
        protected int _contentFullHeight;
        protected int maxScrollWidth;
        protected int maxScrollHeight;
        private int maxHorizontalWidth;
        private int maxVerticalHeight;

        protected RectangleX _scrollRect;

        protected bool _horizontalEnabled;
        protected bool _verticalEnabled;

        public Scroller(Direction direction)
        {
            name = "Scroller";

            horizontalBar = new Bar(Direction.HORIZONTAL);
            verticalBar = new Bar(Direction.VERTICAL);

            if (direction == Direction.HORIZONTAL)
            {
                _horizontalEnabled = true;
                draggingDistanceX = 10;
            }
            else
            {
                _verticalEnabled = true;
                draggingDistanceY = 10;
            }

            horizontalBar.alpha = 0;
            verticalBar.alpha = 0;

            base.addChild(horizontalBar);
            base.addChild(verticalBar);

            _scrollRect = new RectangleX(0, 0, 1, 1);

            touchable = true;
        }

        public bool horizontalEnabled
        {
            set { _horizontalEnabled = value; }
        }

        public bool verticalEnabled
        {
            set { _verticalEnabled = value; }
        }

        protected override void updateSize()
        {

            verticalBar.x = _width - barSize - 2;
            horizontalBar.y = _height - barSize - 2;

            _scrollRect.width = _width;
            _scrollRect.height = _height;

            if (_contentFullWidth == 0)
            {
                _contentFullWidth = (int) _width;
            }
            if (_contentFullHeight == 0)
            {
                _contentFullHeight = (int) _height;
            }

            invalidate();
            //this.scrollRect = _scrollRect;
        }


        public IScrollable content
        {
            set
            {
                if (_content == value) return;

                if (_content != null)
                {
                    _content.scroller = null;
                    this.removeChild(_content.getScrollView());
                    _content.removeEventListener(EventX.RESIZE, contentResizeHandle);
                }

                _content = value;

                if (_content != null)
                {
                    _content.scroller = this;
                    _contentView = _content.getScrollView();
                    base.addChildAt(_contentView, 0);
                    _content.addEventListener(EventX.RESIZE, contentResizeHandle);
                    contentResizeHandle(null);
                }
                else
                {
                    _contentView = null;
                }
            }
            get { return _content; }
        }


        /**
		 * 内容大小变化 
		 * @param event
		 * 
		 */

        protected void contentResizeHandle(EventX e)
        {
            int newW = _content.fullWidth;
            int newH = _content.fullHeight;
            if (_contentFullWidth == newW && _contentFullHeight == newH) return;

            _contentFullWidth = newW;
            _contentFullHeight = newH;

            invalidate();
        }

        public void invalidate()
        {
            int vw = (int) _width;
            int vh = (int) _height;

            int fw = _contentFullWidth;
            int fh = _contentFullHeight;


            maxScrollWidth = vw - fw;
            maxScrollHeight = vh - fh;



            float aw = vw/fw;
            float ah = vh/fh;


            int sw = (int) (vw*aw);
            int sh = (int) (vh*ah);

            if (sw < miniSize)
            {
                sw = miniSize;
            }
            if (sh < miniSize)
            {
                sh = miniSize;
            }

            horizontalBar.size = sw;
            verticalBar.size = sh;

            maxVerticalHeight = vh - sh - barSize;
            maxHorizontalWidth = vw - sw - barSize;

            horizontalBar.max = vw;
            ;
            verticalBar.max = vh;
        }


        public float scrollContentX
        {
            set
            {
                float ax = value/maxScrollWidth;

                //CallLater.add(setAlpha, horizontalBar, 1);
                horizontalBar.position = (int) (ax*maxHorizontalWidth);
                _content.scrollX = value;

                if (hasEventListener(EventX.SCROLL))
                {
                    this.dispatchEvent(scrollEvent);
                }
            }
            get { return _content.scrollX; }
        }

        public float scrollContentY
        {
            set
            {
                float ay = value/maxScrollHeight;
                //CallLater.add(setAlpha, verticalBar, 1);
                verticalBar.position = (int) (ay*maxVerticalHeight);
                _content.scrollY = value;

                if (hasEventListener(EventX.SCROLL))
                {
                    this.dispatchEvent(scrollEvent);
                }
            }
        }

        public float scrollPercentHorizontal
        {
            set
            {
                int scrollContent = (int) (_width - _content.fullWidth);
                if (scrollContent < 0)
                {
                    scrollContentX = scrollContent*value;
                }
                else
                {
                    scrollContentX = 0;
                }
                //CallLater.remove(setAlpha);
            }
        }

        public float scrollPercentVertical
        {
            set
            {
                int scrollContent = (int) (_height - _content.fullHeight);
                if (scrollContent < 0)
                {
                    scrollContentY = scrollContent*value;
                }
                else
                {
                    scrollContentY = 0;
                }

                //CallLater.remove(setAlpha);
            }
        }

        /**
		 * 取得当前在第几屏; 
		 * @param direction
		 * @return 
		 * 
		 */

        public int getSceneIndex(Direction direction)
        {
            if (direction == Direction.HORIZONTAL)
            {
                int v = (int) Math.Round(_content.scrollX/_width);
                return Math.Abs(v);
            }
            else
            {
                int v = (int) Math.Round(_content.scrollY/_height);
                return Math.Abs(v);
            }
        }

        protected override void touchStart(float dx, float dy)
        {
            _mouseDown = true;

            if (_horizontalEnabled)
            {
                TickManager.add(enterHandleX);
            }
            if (_verticalEnabled)
            {
                TickManager.add(enterHandleY);
            }
        }

        /**
		 * 
		 * @param value
		 * @param vertical 是否垂直距离
		 * 
		 */

        protected override void touchMoving(float speedX, float speedY, float dx, float dy)
        {

            float old;
            //float bouncing = 1.0f;
            if (_horizontalEnabled)
            {
                old = _content.scrollX;
                if (old > 0)
                {
                    speedX *= (_width - old)/_width;
                }
                else if (old + _contentFullWidth < _width)
                {
                    speedX *= (_contentFullWidth + old)/_width;
                }
                scrollContentX = old + speedX;
                _velocityX += speedX*SPEED_SPRINGNESS;
            }
            if (_verticalEnabled)
            {
                old = _content.scrollY;
                if (old > 0)
                {
                    speedY *= (_height - old)/_height;
                }
                else if (old + _contentFullHeight < _height)
                {
                    speedY *= (_contentFullHeight + old)/_height;
                }
                scrollContentY = old + speedY;
                _velocityY += speedY*SPEED_SPRINGNESS;
            }
        }


        /**
		 *  
		 * @param value
		 * @param positive 是否正方向;
		 * 
		 */

        protected override void touchEnd(float speedX, float speedY, float dx, float dy)
        {

            _mouseDown = false;
        }

        protected void enterHandleX()
        {
            if (_mouseDown)
            {
                _velocityX *= _mouseDecay;
            }
            else
            {
                _velocityX *= _decay;
            }
            if (Math.Abs(_velocityX) < MIN)
            {
                _velocityX = 0;
            }

            float bouncing = 0;
            if (!_mouseDown)
            {
                float x = _content.scrollX;
                if (x > 0 || _contentFullWidth <= _width)
                {
                    bouncing = -x*BOUNCING_SPRINGESS;
                }
                else if (x + _contentFullWidth < _width)
                {
                    bouncing = (_width - _contentFullWidth - x)*BOUNCING_SPRINGESS;
                }
                if (Math.Abs(bouncing) < MIN)
                {
                    bouncing = 0;
                }
                scrollContentX = x + _velocityX + bouncing;
                if (_velocityX == 0 && bouncing == 0)
                {
                    //CallLater.add(setAlpha,horizontalBar,0);
                    TickManager.remove(enterHandleX);
                    return;
                }
            }
        }

        public void endTween()
        {
            if (_horizontalEnabled)
            {
                TickManager.remove(enterHandleX);
            }
            if (_verticalEnabled)
            {
                TickManager.remove(enterHandleY);
            }
        }

        protected void enterHandleY()
        {
            if (_mouseDown)
            {
                _velocityY *= _mouseDecay;
            }
            else
            {
                _velocityY *= _decay;
            }
            if (Math.Abs(_velocityY) < MIN)
            {
                _velocityY = 0;
            }

            float bouncing = 0;
            if (!_mouseDown)
            {
                float y = _content.scrollY;
                if (y > 0 || _contentFullHeight <= _height)
                {
                    bouncing = -y*BOUNCING_SPRINGESS;
                }
                else if (y + _contentFullHeight < _height)
                {
                    bouncing = (_height - _contentFullHeight - y)*BOUNCING_SPRINGESS;
                }
                if (Math.Abs(bouncing) < MIN)
                {
                    bouncing = 0;
                }
                //trace(_velocityY ,bouncing);
                scrollContentY = y + _velocityY + bouncing;
                if (_velocityY == 0 && bouncing == 0)
                {
                    //CallLater.add(setAlpha,verticalBar,0);
                    TickManager.remove(enterHandleY);
                }
            }
        }

        public void setControllBarVisible(Direction direction, bool visible)
        {

            switch (direction)
            {
                case Direction.HORIZONTAL:
                    horizontalBar.visible = visible;
                    break;
                case Direction.VERTICAL:
                    verticalBar.visible = visible;
                    break;
                case Direction.ANY:
                    horizontalBar.visible = verticalBar.visible = visible;
                    break;
            }
        }

        protected void setAlpha(DisplayObjectX v, float alpha)
        {
            v.alpha = alpha;
        }

        /*
		override public DisplayObjectX addChild(DisplayObjectX child)
        {
			throw new NotSupportedException("只能设置content");
		}
		
		override public DisplayObjectX addChildAt(DisplayObjectX child, int index){
			throw new NotSupportedException("只能设置content");
		}*/
    }
}
