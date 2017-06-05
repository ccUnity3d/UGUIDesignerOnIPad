using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraViewPage : UIPage<CameraViewPage>
{
    public CameraViewScroll cameraViewScroll;
    public RectTransform RectCameraViewScroll;
    public RectTransform RectController;
    public Button cameraViewAdd;
    protected override void Ready(Object arg1)
    {
        base.Ready(arg1);
        RectController = skin.GetComponent<RectTransform>();
        RectCameraViewScroll = UITool.GetUIComponent<RectTransform>(skin.transform,"CameraViewScroll");
        cameraViewScroll = UITool.AddUIComponent<CameraViewScroll>(RectCameraViewScroll.gameObject);
        RectCameraViewScroll.anchoredPosition = Vector3.zero;
        cameraViewAdd = UITool.GetUIComponent<Button>(skin.transform, "CameraViewAdd");
        //cameraViewAdd = UITool.GetUIComponent<Button>(skin.transform, "CameraViewAdd");
    }
}
