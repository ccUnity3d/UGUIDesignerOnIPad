using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputState : MyState
{
    public object targetBasedata;

    protected InputStateMachine machine
    {
        get {
            return InputStateMachine.Instance;
        }
    }
    protected Mode2DPrefabs prefabs
    {
        get {
            return Mode2DPrefabs.Instance;
        }
    }
    protected OriginalInputData data
    {
        get
        {
            return OriginalInputData.Instance;
        }
    }
    protected DefaultSettings defaultSetting
    {
        get {
            return DefaultSettings.Instance;
        }
    }
    protected UndoHelper undoHelper
    {
        get
        {
            return UndoHelper.Instance;
        }
    }
    protected CameraStateMachine cameraMachion
    {
        get {
            return CameraStateMachine.Instance;
        }
    }
    protected TouchManager touchManager
    {
        get {
            return TouchManager.Instance;
        }
    }
    protected ResourcesPool pool
    {
        get {
            return ResourcesPool.Instance;
        }
    }
    protected MainPageUIData mainpagedata
    {
        get {
            return MainPageUIData.Instance;
        }
    }
    protected MainPageStateMachine mianpageMachine
    {
        get {
            return MainPageStateMachine.Instance;
        }
    }
    protected MainPageUIController mainPageController
    {
        get {
            return MainPageUIController.Instance;
        }
    }
    protected OptionsController optionsController
    {
        get
        {
            return OptionsController.Instance;
        }
    }

    /// <summary>
    /// 2D墙设置视图
    /// </summary>
    protected View2D view2D
    {
        get {
            return View2D.Instance;
        }
    }
    protected View3D view3D
    {
        get {
            return View3D.Instance;
        }
    }

    protected Camera inputCamera = Camera.main;

    /// <summary>
    /// 鼠标图标
    /// </summary>
    protected Transform cursor;
    //protected bool uiHit
    //{
    //    get {
    //        if (GlobalConfig.isMyDebug == true)
    //        {
    //            #if !UNITY_ANDROID && !UNITY_IPHONE  //安卓  
    //                    return EventSystem.current.IsPointerOverGameObject();
    //            #else
    //                    if (Input.touchCount == 0)
    //                    {
    //                        return false;
    //                    }

    //                    PointerEventData pointdata;
    //                    if (MyStandaloneInputModule.current.tryGetMyMousePointerEventData(out pointdata) == false)
    //                    {
    //                        return false;
    //                    }
    //                    List<RaycastResult> list = new List<RaycastResult>();
    //                    EventSystem.current.RaycastAll(pointdata, list);
    //                    if (list.Count == 0)
    //                    {
    //                        return false;
    //                    }
    //                    return true;
    //            #endif
    //        }
    //        else {
    //            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
    //            {
    //                return EventSystem.current.IsPointerOverGameObject();
    //            }

    //            if (Input.touchCount == 0)
    //            {
    //                return false;
    //            }

    //            PointerEventData pointdata;
    //            if (MyStandaloneInputModule.current.tryGetMyMousePointerEventData(out pointdata) == false)
    //            {
    //                return false;
    //            }
    //            List<RaycastResult> list = new List<RaycastResult>();
    //            EventSystem.current.RaycastAll(pointdata, list);
    //            if (list.Count == 0)
    //            {
    //                return false;
    //            }
    //            return true;
    //        }
    //    }
    //}

    public InputState(string name)
    {
    }

    private int oldCount = 0;

    public override void enter() { base.enter(); }
    public override void mUpdate() { base.mUpdate(); }
    public override void mPhoneUpdate(){base.mPhoneUpdate();}

    public override void exit() { base.exit(); }

    protected Vector3 GetScreenToWorldPos(Vector3 pos)
    {
        return inputCamera.ScreenToWorldPoint(pos);
    }


    protected void setState(string name)
    {
        machine.setState(name);
    }

    protected SelectState getSelectState(string name)
    {
        return machine.getSelectState(name);
    }

    protected SelectState3D getSelectState3D(string name)
    {
        return machine.getSelectState3D(name);
    }

    protected virtual void RefreshView()
    {
        if (machine.currentInputIs2D)
        {
            view2D.RefreshView();
        }
        else {
            view3D.RefreshView();
        }
    }
}
