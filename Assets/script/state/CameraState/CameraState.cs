using UnityEngine;
using foundation;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CameraState : MyState {


    protected CameraStateMachine machine
    {
        get {
            return CameraStateMachine.Instance;
        }
    }
    protected InputStateMachine inputMachine
    {
        get
        {
            return InputStateMachine.Instance;
        }
    } 
    protected CameraControler cameraControl
    {
        get
        {
            return CameraControler.Instance;
        }
    }
    protected UndoHelper undoHelper
    {
        get {
            return UndoHelper.Instance;
        }
    }
    protected DefaultSettings settings
    {
        get {
            return DefaultSettings.Instance;
        }
    }

    protected Vector3 oldPos = Vector3.zero;
    protected CameraData defaultData;
    protected CameraData data;
    protected Camera target = Camera.main;
    //protected bool uiHit
    //{
    //    get
    //    {
    //        Vector3 v3;
    //        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
    //        {
    //            if (EventSystem.current == null) return false;
    //            return EventSystem.current.IsPointerOverGameObject();
    //        }

    //        if (Input.touchCount == 0)
    //        {
    //            return false;
    //        }
    //        v3 = Input.GetTouch(0).position;

    //        PointerEventData pointdata;
    //        if (MyStandaloneInputModule.current.tryGetMyMousePointerEventData(out pointdata) == false)
    //        {
    //            return false;
    //        }
    //        List<RaycastResult> list = new List<RaycastResult>();
    //        EventSystem.current.RaycastAll(pointdata, list);
    //        if (list.Count == 0)
    //        {
    //            return false;
    //        }
    //        return true;
    //    }
    //}

    public CameraState(string name)
    {
    }

    public override void enter()
    {
        base.enter();
        //cameraControl.SetCameraData(cameraControl.helpCamera, cameraControl.mainCamera);
        cameraControl.addEventListener(CameraEvent.ResetCamera, ResetCamera);
    }

    public virtual void ResetCameraData(MyEvent obj)
    {
        data.cullingMask = defaultData.cullingMask;
        data.orthographic = defaultData.orthographic;
        data.orthographicSize = defaultData.orthographicSize;
        data.cullingMask = defaultData.cullingMask;
        data.position = defaultData.position;
        data.rotation = defaultData.rotation;
    }

    public virtual void ResetCamera(MyEvent obj)
    {
        //defaultData.setCamera(cameraControl.mainCamera);
    }
    public virtual void ResetCameraData(Vector2 pos)
    {
        ResetCameraData(null);
    }

    public override void mUpdate()
    {
        base.mUpdate();
    }

    public override void mPhoneUpdate()
    {
        base.mPhoneUpdate();
    }

    public override void exit()
    {
        cameraControl.removeEventListener(CameraEvent.ResetCamera, ResetCamera);
        base.exit();
    }
    
}
