using System;
using System.Collections;
using UnityEngine;

namespace foundation
{
    public class AssetLoader : RFLoader
    {
        protected WWW www;
        protected AssetBundle assetBundle;
        protected Texture2D texture2D;

        public AssetLoader(string url, string id = null) : base(url, id)
        {
        }

        public static string formatedURL(string url)
        {
            return url;
        }

        protected override IEnumerator doLoad()
        {
            www = new WWW(formatedURL(url));

            if (checkProgress)
            {
                TickManager.add(progress);
            }
            yield return www;

            if (checkProgress)
            {
                TickManager.remove(progress);
            }

            if (www == null)
            {
                _state = AssetState.FAILD;
                _data = null;
                string message = string.Format("加载文件失败:{0} 已被dispose", url);
                DebugX.Log(message);
                this.dispatchEvent(new EventX(EventX.FAILED, message));
            }
            else
            {
                if (string.IsNullOrEmpty(www.error))
                {
//                    if (www.isDone)
//                    {
                        _state = AssetState.READY;
                        _data = www.bytes;
                        this.simpleDispatch(EventX.COMPLETE, _data);
 //                   }
//                    else
//                    {
//                        _state = AssetState.FAILD;
//                        _data = null;
//                        string message = string.Format("加载文件失败:{0} 没有完成", url);
//                        DebugX.Log(message);
//                        this.dispatchEvent(new EventX(EventX.FAILED, message));
//                    }
                }
                else
                {
                    _state = AssetState.FAILD;
                    _data = null;
                    string message = string.Format("加载文件失败:{0} error:{1}", url, www.error);
                    DebugX.LogWarning(message);
                    this.simpleDispatch(EventX.FAILED, message);
                }
            }
        }


        public override object parser(LoaderXDataType parserType)
        {
            switch (parserType)
            {
                case LoaderXDataType.BYTES:
                    _data = www.bytes;
                    break;
                case LoaderXDataType.MANIFEST:
                    if (assetBundle == null)
                    {
                        assetBundle = www.assetBundle;
                    }
                    if (assetBundle != null)
                    {
                        _data = assetBundle.LoadAsset("AssetBundleManifest");
                    }
                    else
                    {
                        _data = null;
                        Debug.Log("No this Manisfest");
                    }
                    break;

                case LoaderXDataType.TEXTURE:
                    texture2D = www.texture;
                    if (www.assetBundle != null)
                    {
                        www.assetBundle.Unload(false);
                    }
                    _data = texture2D;
                    break;
                case LoaderXDataType.SOUND:
                    _data = www.audioClip;
                    break;
                case LoaderXDataType.ASSETBUNDLE:
                    if (assetBundle == null)
                    {
                        assetBundle = www.assetBundle;
                    }
                    _data = assetBundle;
                    break;
                
                case LoaderXDataType.SPRITE:
                    if (assetBundle == null)
                    {
                        assetBundle = www.assetBundle;
                    }
                    if (assetBundle != null)
                    {
                        _data = assetBundle.LoadAllAssets<Sprite>(); //assetBundle.LoadAll(typeof (Sprite));
                    }
                    break;

                case LoaderXDataType.AMF:
                    ByteArray bytes=new ByteArray(www.bytes);
                    if (bytes.BytesAvailable>0)
                    {
                        try
                        {
                            bytes.Inflate();
                        }
                        catch (Exception)
                        {
                            bytes.Position = 0;
                        }
                        _data = bytes.ReadObject();
                    }
                    else
                    {
                        _data = null;
                    }
                    break;
            }

            return _data;
        }

        public override void dispose()
        {
            if (www != null)
            {
                try
                {
                    www.Dispose();
                }
                catch (Exception)
                {
                }
                www = null;
            }
            if (texture2D !=null)
            {
                UnityEngine.Object.Destroy(texture2D);
                //texture2D = null;
            }

            if (assetBundle !=null)
            {
                assetBundle.Unload(true);
                assetBundle = null;
            }
            _data = null;
        }

        protected void progress()
        {
            if (www !=null)
            {
                this.dispatchEvent(new EventX(EventX.PROGRESS, www.progress));
            }
        }

        public override void stop()
        {
            
        }
    }
}

