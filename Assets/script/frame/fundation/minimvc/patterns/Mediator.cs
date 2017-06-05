using System;
using System.Reflection;

namespace foundation
{
    public class Mediator : Notifier, IMediator, IAsync
    {
        protected bool _ready = false;

        protected string _name;
        protected IPanel _view;
        protected IProxy _model;
        protected ASDictionary<string,Action<EventX>> _eventInterests;
        protected ASDictionary<string, Action<EventX>> modelEventInterests;

        protected Action<EventX> readyHandle;

        public Mediator(string mediatorName)
        {
            _name = mediatorName;

            _eventInterests=new ASDictionary<string, Action<EventX>>();
        }

        public bool addReayHandle(Action<EventX> handle)
        {

            if (_ready)
            {
                handle(new EventX(EventX.READY));
                return true;
            }

            readyHandle += handle;
            return true;
        }


        public bool isReady
        {
            get
            {
                return _ready;
            }
        }
        public ASDictionary<string, Action<EventX>> eventInterests
        {
            get { return _eventInterests; }
        }

        public bool startSync()
        {

            if (_view is IAsync)
            {
                IAsync asyncView = _view as IAsync;
                if (asyncView.isReady == false)
                {
                    asyncView.startSync();
                }
            }

            return true;
        }


        public string name
        {
            get { return _name; }
        }

        public void setView(IPanel value)
        {

            if (_view != null)
            {
                _view.removeEventListener(EventX.READY, preViewReadyHandler);
                bindSetViewEvent(_view, false);
            }

            _view = value;

            if (_view != null)
            {
                if (_view is IAsync)
                {
                    IAsync asyncView = _view as IAsync;
                    if (asyncView.isReady == false)
                    {
                        _view.addEventListener(EventX.READY, preViewReadyHandler);
                        return;
                    }
                }
                preViewReadyHandler(null);            //此句调用肯定会报错
            }
        }

        public IPanel getView()
        {
            return _view;
        }

        protected void preViewReadyHandler(EventX e)
        {
            if (e != null)
            {
                IPanel panel = e.target as IPanel;
                panel.removeEventListener(EventX.READY, preViewReadyHandler);
            }

            if (_view.isShow)
            {
                stageHandler(new EventX(EventX.ADDED_TO_STAGE));
            }
            bindSetViewEvent(_view, true);
           
            if (_model == null)
            {
                mediatorReadyHandle();
                return;
            }

            if (_model is IAsync)
            {
                IAsync asyncModel = _model as IAsync;
                if (asyncModel.isReady == false)
                {
                    _model.addEventListener(EventX.READY, preModelReadyHandle);
                    asyncModel.startSync();
                    return;
                }
            }
            mediatorReadyHandle();
        }


        protected void preModelReadyHandle(EventX e)
        {
            IProxy proxy = e.target as IProxy;
            proxy.removeEventListener(EventX.READY, preModelReadyHandle);
            if (proxy == _model)
            {
                mediatorReadyHandle();
            }
        }

        protected virtual void mediatorReadyHandle()
        {
            //DebugX.Log("mediator:{0} ready!",this.name);

            _ready = true;
            if (readyHandle != null)
            {
                readyHandle(new EventX(EventX.READY));
                readyHandle = null;
            }

            facade.simpleDispatch(EventX.MEDIATOR_READY, name);
        }

        protected void bindSetViewEvent(IPanel view, bool isBind)
        {
            if (isBind)
            {
                view.addEventListener(EventX.ADDED_TO_STAGE, stageHandler);
                view.addEventListener(EventX.REMOVED_FROM_STAGE, stageHandler);
            }
            else
            {
                view.removeEventListener(EventX.ADDED_TO_STAGE, stageHandler);
                view.removeEventListener(EventX.REMOVED_FROM_STAGE, stageHandler);
            }
        }

        protected void stageHandler(EventX e)
        {
            if (e.type == EventX.ADDED_TO_STAGE)
            {
                if (isCanAwaken())
                {
                    awaken();
                    facade.registerEvent(this.eventInterests,true);

                    if (_model != null)
                    {
                        facade.registerProxyEvent(this.modelEventInterests, _model, true);
                    }
                }
            }
            else if (e.type == EventX.REMOVED_FROM_STAGE)
            {
                facade.registerEvent(this.eventInterests,false);
                if (_model != null)
                {
                    facade.registerProxyEvent(this.modelEventInterests, _model, false);
                }
                sleep();
            }
        }

        /// <summary>
        /// 现在状态是否可让它唤醒 
        /// </summary>
        protected virtual bool isCanAwaken()
        {
            return true;
        }

        public IProxy getModel()
        {
            return _model;
        }


        public void setModel(IProxy value)
        {
            _model = value;
        }

        public virtual void execute(string eventType, object data = null)
        {
            if (isReady == false)
            {
                addReayHandle(
                    delegate(EventX e)
                    {
                        execute(eventType, data);
                    });
                startSync();
                return;
            }

            MethodInfo method=this.GetType().GetMethod(eventType);
            if (method!=null)
            {
                method.Invoke(this, new object[] {data});
            }
        }

        protected virtual void eventHandle(EventX e)
        {
            
        }

        public virtual void onRegister()
        {

        }

        public virtual void onRemove()
        {
        }

        public virtual void sleep()
        {
        }

        public virtual void awaken()
        {
        }

        public bool isShow
        {
            get { return _view.isShow; }
        }
    }
}
