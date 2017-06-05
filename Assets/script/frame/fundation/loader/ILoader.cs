namespace foundation
{
    public interface ILoader
    {
        void load();
        void cancel(bool fireEvent= true);
		
		object data
        {
            get;
        }
    }
}
