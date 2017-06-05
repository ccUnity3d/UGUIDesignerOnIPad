using UnityEngine;

public class GlobalConfig
{

    public GlobalConfig()
    {

    }

    public static bool isMyDebug = false;

    public static bool isNeedSDK = true;

    public static bool running = false;

    /*额外说明*/
    /*
     如果MainCamera是正交相机：
     无论Screen的分辨率如何变化  只要MainCamera的orthographicSize不变 MainCamera视野height对应的场景高度大小固定不变。 
     * 例如：MainCamera的orthographicSize = 1，那么无论Screen的分辨率是多少，MainCamera视野高度都是6.25单位的场景大小。
     */
    /*********/

    /// <summary>
    /// MainCamera的正交Size对应场景的单位长度/
    /// </summary>
    public static float cameraSceneScale
    {
        get { return 1f; }
    }

    /// <summary>
    /// 制作ui的标准分辨率宽度，自适应在此基础上改变
    /// </summary>
    public static float perfectWith = 960;
    /// <summary>
    /// 制作ui标准分辨率高度，自适应在此基础上改变
    /// </summary>
    public static float perfectHeight = 540;

    private static GameObject _aimParentObj;

    public static GameObject UIObjInScene;

    /// <summary>
    /// 所有ui的锚物体
    /// </summary>
    public static GameObject UIParentObj
    {
        get
        {
            if (_aimParentObj == null)
            {
                _aimParentObj = GameObject.Find("UI/Canvas/Anchor").gameObject;
            }
            return _aimParentObj;
        }
    }
    private static Canvas canvas;
    public static Canvas Canvas
    {
        get
        {
            if (canvas == null)
            {
                canvas = GameObject.Find("UI/Canvas").GetComponent<Canvas>();
            }
            return canvas;
        }
    }

}
