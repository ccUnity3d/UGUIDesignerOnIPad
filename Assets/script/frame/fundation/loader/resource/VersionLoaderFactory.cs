using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class VersionLoaderFactory : ILoaderFactory
    {
        internal static string PRE_HASH = "";
        internal static int PRE_HASH_LEN = 0;
        private static Dictionary<string,string> packageMapping =new Dictionary<string, string>();
        public RFLoader getLoader(string url, string uri)
        {
            if (PRE_HASH_LEN > 5)
            {
                int index = url.IndexOf(PRE_HASH);
                if (index != -1)
                {
                    string localPath = url.Substring(index + PRE_HASH_LEN);

                    string formatedPath = AssetLoader.formatedURL(localPath);
                    if (isInPackage(formatedPath))
                    {
                        url = getLocal(localPath);
                    }
                    else if (!Application.isEditor)
                    {
                        DebugX.Log("本地无,从远程加载文件:{0}",url);
                    }
                }
            }
            return new AssetLoader(url, uri);
        }

        private string getLocal(string localPath)
        {
            return "";
        }

        public static void SetPreHashValue(string value)
        {
            PRE_HASH = value;
            PRE_HASH_LEN = value.Length;
        }


        public static bool isInPackage(string uri)
        {
            string hash = null;
            if (packageMapping.TryGetValue(uri, out hash))
            {
                return true;
            }
            return false;
        }

        public static void initPackageVersion(string v)
        {
            string[] keyList = v.Split('\n');

            string[] keyHash;
            foreach (string key in keyList)
            {
                keyHash = key.Split(':');

                if (keyHash.Length == 2)
                {
                    packageMapping.Add(keyHash[0],keyHash[1]);
                }
            }
        }
    }
}
