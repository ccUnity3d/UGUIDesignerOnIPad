using UnityEngine;
using System.Collections;
using System;

public class ShareState : MainPageState
{
    public const string Name = "ShareState";

    private SchemeManifest schemeManifest
    {
        get
        {
            return SchemeManifest.Instance;
        }
    }

    private JsonCacheManager jsonCacheManager
    {
        get {
            return JsonCacheManager.Instance;
        }
    }

    private Mode2DPrefabs prefabs
    {
        get {
            return Mode2DPrefabs.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        if (undoHelper.currentData.SaveId != 0)
        {
            //请先上传方案
            Debug.LogWarning("分享 是否保存方案");
            MessageBox.Instance.ShowOkCancelClose("分享方案", "是否保存方案",UnSaveShare, SaveSchemeToShare);
        }
        else if (schemeManifest.isUnUpLoaded == true)
        {
            if (string.IsNullOrEmpty(schemeManifest.templateId) == true)
            {
                //请先上传方案
                Debug.LogWarning("分享 是否保存方案");
                MessageBox.Instance.ShowOkCancelClose("分享方案", "请先上传方案", null, SaveSchemeToShare);
            }
            else {

                OriginalProjectData originalProjectData = jsonCacheManager.GetCurrentOriginalProjectData();
                originalProjectData.id = schemeManifest.templateId;
                //101007 分享 title content photo HT5url collectTypr
                MsgToIOS msg = new MsgToIOS();
                msg.code = "101007";
                msg.info = new MsgToIOS.InfoToIOS();
                msg.info.shareType = 0;
                msg.info.projectData = originalProjectData;
                msg.info.projectData.meta = originalProjectData.meta;
                UnityIOSMsg.sendToIOS(msg);
            }
        }
        else {

            Share();
        }

        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(FreeState2D.NAME);
        }
        else
        {
            inputMachine.setState(FreeState3D.NAME);
        }
    }

    private void UnSaveShare()
    {
        if (schemeManifest.isUnUpLoaded == false)
        {
            Share();
            return;
        }

        if (string.IsNullOrEmpty(schemeManifest.templateId) == true)
        {
            Debug.LogWarning("本地方案不保存无法分享");
        }
        else {

            OriginalProjectData originalProjectData = jsonCacheManager.GetCurrentOriginalProjectData();
            originalProjectData.id = schemeManifest.templateId;
            //101007 分享 title content photo HT5url collectTypr
            MsgToIOS msg = new MsgToIOS();
            msg.code = "101007";
            msg.info = new MsgToIOS.InfoToIOS();
            msg.info.shareType = 0;
            msg.info.projectData = originalProjectData;
            msg.info.projectData.meta = originalProjectData.meta;
            UnityIOSMsg.sendToIOS(msg);
        }
    }

    private void Share()
    {
        OriginalProjectData originalProjectData = jsonCacheManager.GetCurrentOriginalProjectData();
        //101007 分享 title content photo HT5url collectTypr
        MsgToIOS msg = new MsgToIOS();
        msg.code = "101007";
        msg.info = new MsgToIOS.InfoToIOS();
        msg.info.shareType = 0;
        msg.info.projectData = originalProjectData;
        msg.info.projectData.meta = originalProjectData.meta;
        UnityIOSMsg.sendToIOS(msg);
    }

    //private IEnumerator SaveSchemeBeforeShare(object[] arg1)
    //{
    //    TouchCaptureScreen.Instance.AoutCaptureScreenImage();
    //    yield return new WaitForSeconds(0.01f);
    //    yield return new WaitForEndOfFrame();
    //    yield return new WaitForSeconds(0.01f);
    //    yield return new WaitForEndOfFrame();
    //    Texture2D texture = TouchCaptureScreen.Instance.texture; //CaptureScreen.Instance.CaptureCamera(camera3D);
    //    string textureEncoding = "";
    //    if (texture != null)
    //    {
    //        byte[] bytes = texture.EncodeToJPG();
    //        if (inputMachine.currentInputIs2D)
    //        {
    //            //prefabs.helpCamera.gameObject.SetActive(false);
    //        }
    //        textureEncoding = System.Convert.ToBase64String(bytes,
    //                0,
    //                bytes.Length,
    //                Base64FormattingOptions.None
    //                );
    //        Debug.LogWarning("截屏成功");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("截屏失败");
    //    }
    //    string meta = "data:image/jpeg;base64," + textureEncoding;


    //    //101007 分享 title content photo HT5url collectTypr
    //    MsgToIOS msg = new MsgToIOS();
    //    msg.code = "101007";
    //    msg.info = new MsgToIOS.InfoToIOS();
    //    msg.info.shareType = 0;
    //    msg.info.projectData = jsonCacheManager.GetCurrentOriginalProjectData();
    //    msg.info.projectData.meta = meta;
    //    UnityIOSMsg.sendToIOS(msg);
    //}

    private void SaveSchemeToShare()
    {
        //Debug.Log("进入保存");
        if (schemeManifest.isUnUpLoaded == false)
        {
            setState(SaveState.Name);
            Share();
            return;
        }

        controller.addEventListener(MySaveSchemeToShare.SaveSchemeToShare, ShareScheme);
        setState(SaveState.Name);
    }
    private void ShareScheme(MyEvent obj)
    {
        controller.removeEventListener(MySaveSchemeToShare.SaveSchemeToShare, ShareScheme);
        Debug.Log("ShareScheme");
        setState(ShareState.Name);
    }
    public override void exit()
    {
        base.exit();
    }
}
