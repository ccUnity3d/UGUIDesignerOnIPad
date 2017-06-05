using UnityEngine;
using foundation;
using System;

public class CameraControler : MyEventDispatcher
{
    private static CameraControler instance;
    public static CameraControler Instance
    {
        get
        {
            if (instance == null) instance = new CameraControler();
            return instance;
        }
    }

    public CameraStateMachine machine
    {
        get {
            return CameraStateMachine.Instance;
        }
    }

    public InputStateMachine inputMachine
    {
        get
        {
            return InputStateMachine.Instance;
        }
    }

    public Camera helpCamera;
    public Camera mainCamera;
    public Camera uiCamera;


    public void SetCameraData(Camera toCamera, Camera fromCamera)
    {
        toCamera.orthographic = fromCamera.orthographic;
        toCamera.orthographicSize = fromCamera.orthographicSize;
        toCamera.cullingMask = fromCamera.cullingMask;
        toCamera.transform.position = fromCamera.transform.position;
        toCamera.transform.rotation = fromCamera.transform.rotation;
        
    }

    internal void ResetAllCamera(Vector2 pos)
    {
        CameraState camerastate2D = machine.getState(CameraState2D.NAME);
        camerastate2D.ResetCameraData(pos);
        CameraState camerastate3D = machine.getState(CameraState3D.NAME);
        camerastate3D.ResetCameraData(pos);
        if (inputMachine.currentInputIs2D)
        {
            camerastate2D.ResetCamera(null);
        }
        else
        {
            camerastate3D.ResetCamera(null);
        }
    }
}
