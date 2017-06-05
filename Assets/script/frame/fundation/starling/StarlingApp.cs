using foundation;
using UnityEngine;

namespace starling
{

    public class StarlingApp : BaseApp
    {
        private RenderSupport renderSupport;
        private StageX _stage;

        public StarlingApp():base()
        {
        
        }

        protected override void Start()
        {
            Debug.Log("start GUIRouter");

            if (renderSupport != null)
            {
                return;

            }
            renderSupport = new RenderSupport();
            base.Start();

            _stage = new StageX();
            sceneMachine.target = _stage;
        }

        public StageX stage
        {
            get { return _stage; }
        }


        // Update is called once per frame
        protected override void Update()
        {
            if (renderSupport == null)
            {
                return;
            }

            bool isResize = false;
            if (_stage.stageWidth != Screen.width)
            {
                _stage.stageWidth = Screen.width;
                isResize = true;
            }
            if (_stage.stageHeight != Screen.height)
            {
                _stage.stageHeight = Screen.height;
                isResize = true;
            }

            if (isResize)
            {
                ResizeMananger.getInstance().resize(Screen.width, Screen.height);
            }

            updateInput();

            //float deltaTime = 1.0f/60;
            //UnityEngine.Time.DeltaTime;
            TickManager.getInstance().tick();

            renderSupport.nextFrame();
            _stage.render(renderSupport, 1.0f);
            renderSupport.finishQuadBatch();
        }

        private DisplayObjectX _collidingObject = null;
        private DisplayObjectX _previousCollidingObject = null;
        private Vector2X mp = new Vector2X();

        private void updateInput()
        {
            if (_stage == null)
            {
                return;
            }
            bool isNew = false;
            MouseEventX mouseEvent;
            Event currentEvent = Event.current;
            if (currentEvent != null && currentEvent.button == 0)
            {
                Vector2 t = currentEvent.mousePosition;
                mp.setTo(t.x, t.y);

                _collidingObject = _stage.hitTest(mp);
                isNew = (_previousCollidingObject != _collidingObject);

                if (currentEvent.type == EventType.MouseDown)
                {
                    mouseEvent = new MouseEventX(MouseEventX.MOUSE_DOWN, mp);

                    if (_previousCollidingObject != null && isNew)
                    {
                        _previousCollidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_UP, mp));
                    }

                    if (_collidingObject != null && isNew)
                    {
                        _collidingObject.dispatchEvent(mouseEvent);
                    }

                    _stage.dispatchEvent(mouseEvent);
                }
                else if (currentEvent.type == EventType.MouseUp)
                {
                    mouseEvent = new MouseEventX(MouseEventX.MOUSE_UP, mp);
                    if (_collidingObject != null && isNew)
                    {
                        _collidingObject.dispatchEvent(mouseEvent);
                    }
                    _stage.dispatchEvent(mouseEvent);
                }
                else if (currentEvent.type == EventType.mouseMove)
                {
                    mouseEvent = new MouseEventX(MouseEventX.MOUSE_MOVE, mp);

                    if (_collidingObject != null && isNew == false)
                    {
                        _collidingObject.dispatchEvent(mouseEvent);
                    }

                    _stage.dispatchEvent(mouseEvent);
                }



                if (_collidingObject != _previousCollidingObject)
                {
                    if (_previousCollidingObject != null)
                    {
                        _previousCollidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_OUT, mp));
                    }
                    if (_collidingObject != null)
                    {
                        _collidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_OVER, mp));
                    }
                }
                else if (_collidingObject != null)
                {
                    _collidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_MOVE, mp));
                }

                _previousCollidingObject = _collidingObject;



                return;
            }

            //todo;
            int touchCount = Input.touchCount;

            if (touchCount < 1)
            {
                if (_previousCollidingObject != null)
                {
                    _previousCollidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_UP, null));
                    _previousCollidingObject = null;
                }
                return;
            }
            Touch touch = Input.touches[0];
            mp.setTo(touch.position.x, touch.position.y);
            _collidingObject = stage.hitTest(mp);

            isNew = (_previousCollidingObject != _collidingObject);

            switch (touch.phase)
            {
                case TouchPhase.Began:

                    if (_previousCollidingObject != null && isNew)
                    {
                        _previousCollidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_UP, mp));
                    }

                    if (_collidingObject != null && isNew)
                    {
                        _collidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_DOWN, mp));
                    }
                    break;


                case TouchPhase.Canceled:
                case TouchPhase.Ended:

                    if (_previousCollidingObject != null)
                    {
                        _previousCollidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_UP, mp));
                    }
                    break;

                case TouchPhase.Moved:

                    if (_collidingObject != null && isNew == false)
                    {
                        _collidingObject.dispatchEvent(new MouseEventX(MouseEventX.MOUSE_MOVE, mp));
                    }

                    break;
            }


            _previousCollidingObject = _collidingObject;
        }

    }
}