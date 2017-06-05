using foundation;

namespace clayui
{
    public interface IListItemRender:IDataRenderer
    {
        int index
        {
            set;
            get;
        }

        float x
        {
            get;
            set;
        }

        float y
        {
            get;
            set;
        }
    }
}
