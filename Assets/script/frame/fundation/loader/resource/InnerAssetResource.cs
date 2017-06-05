using UnityEngine;

namespace foundation
{
    public class InnerAssetResource: AssetResource
    {
        public InnerAssetResource(string url, string uri = null) : base(url, uri)
        {
            
        }

        protected override void _loadImp(int priority = 0, bool progress = false, uint retryCount = 0)
        {
            string eventType = EventX.COMPLETE;

            _data = Resources.Load(url) as Object;

            if (_data == null)
            {
                _data = "_data is null";
                eventType = EventX.FAILED;
            }


            resourceComplete(eventType);

        }

        public void setUrl(string path)
        {
            _url = path;
        }

    }
}