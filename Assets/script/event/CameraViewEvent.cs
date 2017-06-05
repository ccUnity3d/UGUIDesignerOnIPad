using UnityEngine;
using System.Collections;

public class CameraViewEvent : MyEvent {

    // Use this for initialization
    public const string AddCameraView = "AddCameraView";
    public const string RemoveCameraView = "RemoveCameraView";

    public CameraViewEvent(string type, object data = null):base(type, data)
    {

    }
}
