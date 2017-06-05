using UnityEngine;

namespace foundation
{
    public class PathDefine
    {
        public static string configPath;
        public static string uiPath;

        public static string avatarPath;
        public static string scenePath;

        public static string texturePath;
        public static string effectPath;
        public static string soundPath;

        public static string commonPath;


        public static string LocalPrefix = "file://";
        public static string getLocal(string uri = "")
        {
            if (isWEB)
            {
                return "StreamingAssets/" + uri;
            }

            string prefix = LocalPrefix;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    prefix = "";
                    break;
            }
            string url = prefix + Application.streamingAssetsPath + "/" + uri;
            return url;
        }

        public static bool isWEB
        {
            get
            {
                return Application.platform == RuntimePlatform.WindowsWebPlayer ||
                       Application.platform == RuntimePlatform.OSXWebPlayer;
            }
        }

        private static string _platformFolderName;
        public static string platformFolderName
        {
            get
            {

                if (_platformFolderName != null)
                {
                    return _platformFolderName;
                }

                _platformFolderName = "WebPlayer";
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        _platformFolderName = "Android";
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        _platformFolderName = "iOS";
                        break;
                }
                return _platformFolderName;
            }
        }
    }
}
