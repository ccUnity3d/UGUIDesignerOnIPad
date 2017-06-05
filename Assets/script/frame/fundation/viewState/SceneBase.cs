namespace foundation
{
    public abstract class SceneBase :AbstractState
    {
        protected IFacade facade;
        public SceneBase(string type) 
        {
            this._type = type;
        }

        public override void initialize()
        {
            facade = Facade.getInstance();
            if (this is IInjectable)
            {
                facade.inject(this);
            }

            base.initialize();
        }

        public override void awaken()
        {
            DebugX.Log("awaken:" + type);
        }
    }
}
