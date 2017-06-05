namespace foundation
{

    public interface IMessageFactory
    {
        AbstractMessage create(int type);
    }
}
