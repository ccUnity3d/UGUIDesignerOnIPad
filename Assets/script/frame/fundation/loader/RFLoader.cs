
using System.Collections;

namespace foundation
{
    public abstract class RFLoader : EventDispatcher, ILoader, IDisposable
    {
        protected object _data;
        public object data
        {
            get
            {
                return _data;
            }
        }

        internal string hashVersion;

        public byte[] bytes
        {
            get { return (byte[])_data; }
        }

        protected string _id;
        public string id
        {
            get
            {
                return _id;
            }
        }

        protected string _url;
        public string url
        {
            get
            {
                return _url;
            }
        }

        protected AssetState _state;

        public bool autoRetry = false;
        public bool checkProgress = false;
        public uint autoTryLimit = 0;

        public RFLoader(string url, string id = null)
        {
            this._url = url;
            this._id = id;

            _state = AssetState.NONE;
            this.initialize();
        }

        protected virtual void initialize()
        {

        }

        virtual public void cancel(bool fireEvent = true)
        {
            if (fireEvent)
            {
                dispatchEvent(new EventX(EventX.FAILED));
            }
        }

        public abstract void stop();

        public virtual void load()
        {
            _state = AssetState.LOADING;
            //To Check the wrong url comes from
            //DebugX.Log("url"+url);
            BaseApp.instance.StartCoroutine(doLoad());
        }

        virtual protected IEnumerator doLoad()
        {
            yield return null;
        }

        public abstract object parser(LoaderXDataType parserType);
    }
}
