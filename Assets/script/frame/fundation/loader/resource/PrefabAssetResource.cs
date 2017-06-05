using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class PrefabAssetResource : AssetResource
    {
        private static Dictionary<string,string> manifesMaping=new Dictionary<string, string>();
        private static Dictionary<string, AssetResource> resourceHash128Maping = new Dictionary<string, AssetResource>();
        public static void registMapping(string pathKey, string manifesKey)
        {
            manifesMaping.Add(pathKey,manifesKey);
        }

        public string manifesKey = "";
        private string manifesPrefix;
        private string dependKey;
        private bool isProgress = false;
        private AssetBundleManifest assetBundleManifest;
        public PrefabAssetResource(string url, string uri = null) : base(url, uri)
        {
            foreach (string key in manifesMaping.Keys)
            {
                if (url.IndexOf(key) != -1)
                {
                    manifesKey = manifesMaping[key];
                }
            }
        }

        protected override void _loadImp(int priority = 0, bool progress = false, uint retryCount = 0)
        {
            this.isProgress = progress;
            string manifesURI = "/" + manifesKey + "/";
            int len = manifesURI.Length;

            if (string.IsNullOrEmpty(manifesKey))
            {
                throw new Exception("不正常");
            }

            int index = url.IndexOf(manifesURI);
            manifesPrefix = url.Substring(0,index + len);

            dependKey = url.Substring(index + len);

            manifesURI = manifesKey + ".manifest";
            string manifesURL = manifesPrefix + manifesKey;
            AssetResource resource = AssetsManager.getResource(manifesURL, manifesURI, LoaderXDataType.MANIFEST);
            if (progress)
            {
                resource.addEventListener(EventX.PROGRESS, progressHandle);
            }
            AssetsManager.bindEventHandle(resource, manifesHandle);
            resource.load(3);

            return;
        }

        private void manifesHandle(EventX e)
        {
            AssetResource resource = e.target as AssetResource;
            resource.removeEventListener(EventX.PROGRESS, progressHandle);
            AssetsManager.bindEventHandle(resource, manifesHandle, false);
            if (e.type != EventX.COMPLETE)
            {
                _data = null;
                resourceComplete(e.type);
                return;
            }
            QueueLoader queueLoader = new QueueLoader();
            queueLoader.retryCount = 3;
            queueLoader.addEventListener(EventX.COMPLETE, dependsHandle);
            if (isProgress)
            {
                queueLoader.addEventListener(EventX.PROGRESS, progressHandle);
            }
            assetBundleManifest = resource.data as AssetBundleManifest;
            string[] dependencies = assetBundleManifest.GetAllDependencies(dependKey);

            //Hash128 hash = assetBundleManifest.GetAssetBundleHash(dependKey);
            string hashValue = null;
            string hashKey = null;
            AssetResource tempResource;
            foreach (string dependency in dependencies)
            {
                string url = manifesPrefix + dependency;
                string uri = manifesKey + "/" + dependency;
                //Debug.Log(dependency);
                hashValue = assetBundleManifest.GetAssetBundleHash(dependency).ToString();
                hashKey = manifesKey + "_" + hashValue;
                if (resourceHash128Maping.TryGetValue(hashKey, out tempResource)==false)
                {
                    tempResource = AssetsManager.getResource(url, uri, LoaderXDataType.ASSETBUNDLE);
                    resourceHash128Maping.Add(hashKey, tempResource);
                }
                /*else if (url != tempResource.url)
                {
                    Debug.Log("redirect:" + url + " to:" + tempResource.url);
                }*/

                tempResource.retain();
                queueLoader.add(tempResource);
            }
            queueLoader.start();
        }

        private void dependsHandle(EventX e)
        {
            QueueLoader queueLoader=e.target as QueueLoader;;
            queueLoader.removeEventListener(EventX.COMPLETE, dependsHandle);
            queueLoader.removeEventListener(EventX.PROGRESS, progressHandle);
            base._loadImp(0,isProgress);
        }

        override protected void loadComplete(EventX e)
        {
            loader = e.target as RFLoader;
            loader.removeEventListener(EventX.COMPLETE, loadComplete);
            loader.removeEventListener(EventX.PROGRESS, progressHandle);
            loader.removeEventListener(EventX.FAILED, failedLoad);
            bool b = true;
            string eventType = e.type;

            AssetBundle assetBundle = loader.parser(LoaderXDataType.ASSETBUNDLE) as AssetBundle;
            if (assetBundle != null)
            {
                switch (parserType)
                {
                        case LoaderXDataType.SPRITE:
                        _data = assetBundle.LoadAllAssets<Sprite>();
                        break;

                    default:
                        string[] names = dependKey.Split('.');
                        _data = assetBundle.LoadAsset(names[0]);
                        break;
                }
            }

            if (_data == null)
            {
                _data = "_data is null";
                eventType = EventX.FAILED;
            }

            if (b)
            {
                resourceComplete(eventType);
            }
        }
    }
}
