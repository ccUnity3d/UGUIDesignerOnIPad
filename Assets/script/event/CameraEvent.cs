using foundation;

public class CameraEvent : MyEvent {

    public const string ResetCamera = "resetcamera";
    public const string ScaleChange = "ScaleChange";
    public const string RotateChange = "RotateChange";
    public const string PositionChange = "PositionChange";
    public const string ViewChange = "ViewChange";

    public CameraEvent(string type, object data = null) : base(type, data)
    {

    }

}
