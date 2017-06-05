using UnityEngine;
using UnityEngine.EventSystems;

public class App : MonoBehaviour {

    public bool showErrorOnGUI = true;
    public bool isDebug = false;
    private MyTickManager tickManager;
    private UGUIHitUI uguiHitUI;

    protected void Start()
    {
        uguiHitUI = UGUIHitUI.Instance;
        GlobalConfig.isMyDebug = isDebug;
        EventSystem.current.gameObject.AddComponent<MyStandaloneInputModule>();
        tickManager = MyTickManager.Instance;
        tickManager.add(TouchManager.Instance.update);
        //tickManager.add(ErrorDisplay.Instance.Update);

        Mode2DPrefabs mode2dprefab = Mode2DPrefabs.Instance;

        CacheModelManager.Instance.LoadCache();
        CacheServerSchemeManager.Instance.LoadCache();
        CacheLocalSchemeManager.Instance.LoadCache();
        CacheServerOfferManager.Instance.LoadCache();
        CacheLocalOfferManager.Instance.LoadCache();

        Application.targetFrameRate = 60;
        Application.runInBackground = true;
        Application.backgroundLoadingPriority = ThreadPriority.Normal;
        //防黑屏/
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        UIManager.Open(PageType.MainPage);

     
    }

    protected void Update()
    {
        if (uguiHitUI != null) uguiHitUI.RunIfUIIsHited();
        if (tickManager != null) tickManager.tick();
    }
    private int width = 0;
    private int height = 0;
    private void Adaption()
    {
        int w = Screen.width;
        int h = Screen.height;
        if (w == width && h == height)
        {
            return;
        }
        width = w;
        height = h;
    }



    private void OnApplicationFocus(bool onRunning)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return;
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            //if (FightManager.Instance.OnFight == true)
            //{
            //    if (onRunning == false)
            //    {
            //        BackShowExitGame();
            //    }
            //    else
            //    {
            //        if (GlobalConfig.heroVO.single == 1)
            //        {
            //            CallLater.Add(0.5f, showPauseTip);
            //        }
            //    }
            //}
        }
    }

    //void OnGUI()
    //{
    //    if (showErrorOnGUI == false) return;
    //    ErrorDisplay.Instance.OnGUI();
    //}

}
