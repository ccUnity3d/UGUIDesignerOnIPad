using System.Collections.Generic;


namespace foundation
{
    using starling;
    /// <summary>
    ///  状态机
    ///  抽像不同事物的状态,让它在一定时间内只处于一个状态
    ///  如：场景
    /// </summary>
    public class StateMachine
    {
        protected Dictionary<string, IState> states;
        protected IState _currentState;
        protected object _target;

        private static StateMachine instance;
        /// <summary>
        /// 状态机;
        /// </summary>
        /// <param name="target">状态机控制的对像;</param>
        public StateMachine(object target = null)
        {
            states = new Dictionary<string, IState>();
            _target = target;
            instance = this;
        }

        public object target
        {
            get
            {
                return _target;
            }
            set { _target = value; }
        }


        public static void setCurrentState(string value)
        {
            instance.currentState = value;
        }
        public static string getCurrentState()
        {
            return instance.currentState;
        }

        /// <summary>
        /// 添加不同的状态; 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool addState(IState value)
        {
            if (states.ContainsKey(value.type))
            {
                return false;
            }
            value.setStateMachine(this);
            states.Add(value.type, value);
            //states[value.type] = value;
            return true;
        }

        /// <summary>
        ///  删除状态; 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool removeState(IState value)
        {
            if (hasState(value.type) == false)
            {
                return false;
            }
            value.setStateMachine(null);
            states.Remove(value.type);
            return true;
        }

        /// <summary>
        /// 是否存在状态; 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool hasState(string type)
        {
            return states.ContainsKey(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stati">没好办法 填什么都行</param>
        /// <returns></returns>
        public static T GetState<T>(string type) where T : IState
        {
            return (T)instance.getState(type);
        }
        /// <summary>
        /// 取得状态 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>		
        public IState getState(string type)
        {
            IState state;
            if (states.TryGetValue(type, out state))
            {
                return state;
            }
            return null;
        }

        /// <summary>
        /// 设置当前状态 
        /// </summary>
        public string currentState
        {
            get
            {
                if (_currentState == null)
                {
                    return "";
                }
                return _currentState.type;
            }
            set
            {
                IState newState = getState(value);
                if (newState == _currentState)
                {
                    return;
                }

                if (_currentState != null)
                {
                    _currentState.removeEventListener(EventX.EXIT, exitHandle);
                    _currentState.sleep();
                    _currentState = null;
                    if (_currentState is DisplayObjectX && target is DisplayObjectContainerX)
                    {
                        (target as DisplayObjectContainerX).removeChild(_currentState as DisplayObjectX);
                    }
                }

                _currentState = newState;

                if (_currentState != null)
                {
                    if (_currentState is DisplayObjectX && target is DisplayObjectContainerX)
                    {
                        (target as DisplayObjectContainerX).addChild(_currentState as DisplayObjectX);
                    }

                    IFacade facade = Facade.getInstance();
                    if (_currentState.initialized == false)
                    {
                        facade.autoInitialize(_currentState.type);
                        _currentState.initialize();
                    }

                    _currentState.addEventListener(EventX.EXIT, exitHandle);
                    _currentState.awaken();
                    facade.changeState(_currentState.type);
                }
            }

        
        }

        private void exitHandle(EventX e)
        {
            IState target = e.target as IState;
            target.removeEventListener(EventX.EXIT, exitHandle);

            if (target != _currentState)
            {
                return;
            }

            if (_currentState is DisplayObjectX && target is DisplayObjectContainerX)
            {
                (target as DisplayObjectContainerX).removeChild(_currentState as DisplayObjectX);
            }

            _currentState = null;
            if (string.IsNullOrEmpty(target.nextState) == false)
            {
                currentState = target.nextState;
            }
            else
            {
                currentState = null;
            }
        }
    }
}
