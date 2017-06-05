using UnityEngine;

public class GlobalConfig
{

    public GlobalConfig()
    {

    }

    public static bool isMyDebug = false;

    public static bool isNeedSDK = true;

    public static bool running = false;

    /*����˵��*/
    /*
     ���MainCamera�����������
     ����Screen�ķֱ�����α仯  ֻҪMainCamera��orthographicSize���� MainCamera��Ұheight��Ӧ�ĳ����߶ȴ�С�̶����䡣 
     * ���磺MainCamera��orthographicSize = 1����ô����Screen�ķֱ����Ƕ��٣�MainCamera��Ұ�߶ȶ���6.25��λ�ĳ�����С��
     */
    /*********/

    /// <summary>
    /// MainCamera������Size��Ӧ�����ĵ�λ����/
    /// </summary>
    public static float cameraSceneScale
    {
        get { return 1f; }
    }

    /// <summary>
    /// ����ui�ı�׼�ֱ��ʿ�ȣ�����Ӧ�ڴ˻����ϸı�
    /// </summary>
    public static float perfectWith = 960;
    /// <summary>
    /// ����ui��׼�ֱ��ʸ߶ȣ�����Ӧ�ڴ˻����ϸı�
    /// </summary>
    public static float perfectHeight = 540;

    private static GameObject _aimParentObj;

    public static GameObject UIObjInScene;

    /// <summary>
    /// ����ui��ê����
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
