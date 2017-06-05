using starling;

namespace foundation
{
    public abstract class AbstractState :DisplayObjectContainerX, IState
    {
        protected string _type;

        protected string _nextState;

        protected bool _resizeable = false;

        /// <summary>
        /// 是否已完成初始化;
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// 状态管理器; 
        /// </summary>

        protected StateMachine stateMachine;

        public StateMachine getStateMachine
        {
            get { return stateMachine; }
        }

        /// <summary>
        /// 当前状态名称;
        /// </summary>
        /// <param name="type"></param>

        public AbstractState()
        {
        }

        /// <summary>
        /// 是否初始化完成 
        /// </summary>

        public bool initialized
        {
            get
            {
                return _initialized;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>

        public virtual void initialize()
        {
            _initialized = true;
        }

        /**
         * 设置当前状态由哪个状态机管理 
         * @param value
         * 
         */
        public void setStateMachine(StateMachine value)
        {
            stateMachine = value;
        }

        /// <summary>
        /// 当前状态名称; 
        /// </summary>
        public string type
        {
            get
            {
                return _type;
            }
        }

        public string nextState
        {
            get
            {
                return _nextState;
            }
            set { _nextState = value; }
        }

        public virtual void sleep()
        {
            DebugX.Log("sleep:" + type);

            this.simpleDispatch(EventX.EXIT);
        }

        /// <summary>
        /// 进入当前状态; 
        /// </summary>

        public virtual void awaken()
        {
            DebugX.Log("awaken:" + type);
        }
    }
}
