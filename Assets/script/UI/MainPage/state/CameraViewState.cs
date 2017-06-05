using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;

public class CameraViewState : MainPageState
{
    public const string Name = "CameraView";
    public List<ItemData> scandataList
    {
        get {
            return originalInputData.preScanDatas;
        }
    }
    private ToggleButton toggleButton;

    private CameraViewScroll cameraViewScroll;
    private CameraViewPage cameraViewPage { get { return CameraViewPage.Instance; } }
    private Mode2DPrefabs prefabs { get { return Mode2DPrefabs.Instance; } }
    private CameraState3D camera3DState;
    
    public override void enter()
    {
        base.enter();

        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(HoldOnState2D.NAME);
        }
        else
        {
            inputMachine.setState(HoldOnState3D.NAME);
        }

        if (toggleButton.onDown == false)
        {
            setState(MainPageFreeState.Name);
            return;
        }
        cameraViewPage.RectCameraViewScroll.sizeDelta = new Vector2(360, 0);
        cameraViewPage.skin.SetActive(true);
        UITool.SetActionTrue(cameraViewPage.RectCameraViewScroll.gameObject);
        UITool.SetActionFalse(cameraViewPage.cameraViewAdd.gameObject);
        cameraViewScroll.Display(scandataList);
        cameraViewScroll.onItemClick= OnItemClick;
        //cameraViewScroll.addEventListener(CameraViewEvent.RemoveCameraView, RemoveCameraView);
    }
    
    private void OnItemClick(int index)
    {
        PreScanData itemdata = scandataList[index] as PreScanData;
        prefabs.mainCamera.transform.position = itemdata.position;
        prefabs.mainCamera.transform.rotation = Quaternion.Euler(itemdata.rotate);
        camera3DState.follow = itemdata.follow;
        camera3DState.focaseP = itemdata.focaseP;
        //exit();
        Debug.Log("显示 相机视图");
    }

    public override void exit()
    {
        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(FreeState2D.NAME);
        }
        else
        {
            inputMachine.setState(FreeState3D.NAME);
        }
        toggleButton.onDown = false;
        inputMachine.setState(FreeState3D.NAME);
        cameraViewPage.skin.SetActive(false);
        base.exit();
    }
    public override void Ready()
    {
        base.Ready();
        toggleButton = mainpage.cameraView;
        //cameraViewPage.cameraViewAdd.onClick.AddListener(onCameraViewAdd);
        cameraViewPage.ChildPageSetData(mainpage.SkinCameraViewScroll);
        cameraViewScroll = cameraViewPage.cameraViewScroll;
        Button cameraViewAdd = UITool.GetUIComponent<Button>(cameraViewScroll.transform, "Add");
        cameraViewAdd.onClick.AddListener(AddCameraView);
        cameraViewPage.cameraViewAdd.onClick.AddListener(onCameraViewAdd);
        //cameraViewScroll.addEventListener(CameraViewEvent.RemoveCameraView, RemoveCameraView);

        camera3DState = CameraStateMachine.Instance.getState(CameraState3D.NAME) as CameraState3D;
    }

    //private void RemoveCameraView(MyEvent obj)
    //{
    //    Debug.Log("移除");
    //}

    private void AddCameraView()
    {
        Debug.Log("添加相机视图");

        cameraViewPage.cameraViewScroll.CancelPress();
        UITool.SetActionFalse(cameraViewPage.RectCameraViewScroll.gameObject);
        UITool.SetActionTrue(cameraViewPage.cameraViewAdd.gameObject);
        //UITool.SetActionTrue(cameraViewPage.cameraViewAdd.gameObject);
        //scandataList.Clear();
        //scandataList.Add(new PreScanData(Vector3.zero, Vector3.zero, ""));
        //cameraViewScroll.Display(scandataList);
        //cameraViewScroll.DeleteEnd();
    }
    private void onCameraViewAdd()
    {
        MyMono.MyStartCoroutine(Func,this);
    }

    private IEnumerator Func(object[] arg1)
    {
        yield return new WaitForEndOfFrame();

        PreScanData preScanData = new PreScanData();

        TouchCaptureScreen.Instance.AoutCaptureScreenImage();

        yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();

        Texture2D texture = TouchCaptureScreen.Instance.texture;

        string textureEncoding = "";

        if (texture != null)
        {
            byte[] bytes = texture.EncodeToJPG();
            GameObject.DestroyImmediate(texture, true);
            if (inputMachine.currentInputIs2D)
            {
                //prefabs.helpCamera.gameObject.SetActive(false);
            }
            textureEncoding = System.Convert.ToBase64String(bytes,
                    0,
                    bytes.Length,
                    Base64FormattingOptions.None
                    );
            Debug.LogWarning("截屏成功");
        }
        else
        {
            Debug.LogWarning("截屏失败");
        }

        preScanData.imagemeta =  textureEncoding;
        preScanData.id = scandataList.Count;
        preScanData.position = prefabs.mainCamera.transform.position;
        preScanData.rotate = prefabs.mainCamera.transform.rotation.eulerAngles;
        preScanData.focaseP = camera3DState.focaseP;
        preScanData.follow = camera3DState.follow;

        undoHelper.save();
        scandataList.Add(preScanData);

        cameraViewScroll.Display(scandataList);
        UITool.SetActionTrue(cameraViewPage.RectCameraViewScroll.gameObject);
        UITool.SetActionFalse(cameraViewPage.cameraViewAdd.gameObject);
    }


}
