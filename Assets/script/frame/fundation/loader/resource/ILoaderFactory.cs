namespace foundation
{
    public interface ILoaderFactory
    {
        RFLoader getLoader(string url, string uri);
    }
}
