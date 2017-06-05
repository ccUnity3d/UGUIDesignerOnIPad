namespace foundation
{
    public interface IListItemRender:IDataRenderer,IPoolable,IEventDispatcher
    {
        ChooseState chooseState { get; set; }
        int index
        {
            set;
            get;
        }
    }
}
