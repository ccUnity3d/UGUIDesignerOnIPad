using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class SaveState : MainPageState
{
    private SaveScheme savaScheme { get { return SaveScheme.Instance; } }

    public SchemePageController schemePageControl
    {
        get
        {
            return SchemePageController.Instance;
        }
    }

    private Mode2DPrefabs prefabs
    {
        get {
            return Mode2DPrefabs.Instance;
        }
    }

    private CacheLocalSchemeManager cacheLocalSchemeManager
    {
        get
        {
            return CacheLocalSchemeManager.Instance;
        }
    }

    private JsonCacheManager jsonCacheManager
    {
        get {
            return JsonCacheManager.Instance;
        }
    }

    private SchemeManifest schemeManifest
    {
        get
        {
            return SchemeManifest.Instance;
        }
    }

    public override void Ready()
    {
        base.Ready();
        savaScheme.SetData(mainpage.saveScheme);
        savaScheme.cancel.onClick.AddListener(onCancel);
        savaScheme.ok.onClick.AddListener(onConfirm);
    }

    private void onCancel()
    {
        UITool.SetActionFalse(savaScheme.rectSave.gameObject);
    }
    private void onConfirm()
    {
        UITool.SetActionFalse(savaScheme.rectSave.gameObject);
    }
    public const string Name = "SaveState";
    public override void enter()
    {
        base.enter();
        //if (string.IsNullOrEmpty(SchemeManifest.Instance.name))
        //{
        //    UITool.SetActionTrue(mainpage.saveScheme.gameObject);
        //}
        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(FreeState2D.NAME);
        }
        else
        {
            inputMachine.setState(FreeState3D.NAME);
        }
        //临时
        //SaveOffer();
        //正式
        MyMono.MyStartCoroutine(Func, this);
    }

    //private void SaveOffer()
    //{
    //    TestPrice price = new TestPrice();
    //    price.prices = undoHelper.currentData.schemeManifest.prices;
    //    string json = MyJsonTool.ToJson(price);
    //    WriteToLocal(Application.dataPath + "/offer.json", json);
    //}

    private IEnumerator Func(object[] arg1)
    {
        yield return new WaitForEndOfFrame();

        OriginalProjectData data = new OriginalProjectData();
       
        //Camera camera3D = prefabs.mainCamera;
        //if (inputMachine.currentInputIs2D)
        //{
        //    view3D.RefreshView();
        //    prefabs.helpCamera.gameObject.SetActive(true);
        //    camera3D = prefabs.helpCamera;
        //}
        TouchCaptureScreen.Instance.AoutCaptureScreenImage();

        yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();

        Texture2D texture = TouchCaptureScreen.Instance.texture; //CaptureScreen.Instance.CaptureCamera(camera3D);

        //mainpage.image.texture = texture;

        string textureEncoding = "";
        if (texture != null)
        {
            byte[] bytes = texture.EncodeToJPG();
            GameObject.DestroyImmediate(texture, true);
            texture = null;
            Resources.UnloadUnusedAssets();
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
        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    Debug.Log(textureEncoding);
        //    WriteToLocal(Application.dataPath + "/OriginalProjectJson/textureEncoding/texture.txt", textureEncoding);
        //}
        schemeManifest.meta = data.meta = "data:image/jpeg;base64," + textureEncoding;
        originalInputData.BeforetSerializeFieldDo();
        data.data = originalInputData;
        //SchemeManifest schemeManifest = schemePageControl.schemeManifest;
        data.name = schemeManifest.name;
        data.description = schemeManifest.description;
        data.isNew = schemeManifest.isNew;
        data.id = schemeManifest.id;
        data.tempId = schemeManifest.tempId;
        data.priceIdList = schemeManifest.prices;
        data.version = schemeManifest.version;

        string json = MyJsonTool.ToJson(data);
        jsonCacheManager.AddSchemeCache(data, json);

        MsgToIOS msg = new MsgToIOS();
        msg.code = "101004";
        MsgToIOS.InfoToIOS info = new MsgToIOS.InfoToIOS();
        info.projectData = data;
        info.type = data.isNew == true ? 0 : 1;
        msg.info = info;
        if (info.type == 0)
        {
            UnityIOSMsg.sendToIOS(msg, IOSEvent.SetSchemeId, CreatScheme);
        }
        else
        {
            UnityIOSMsg.sendToIOS(msg);
            controller.dispatchEvent(new MyEvent(MySaveSchemeToShare.SaveSchemeToShare));
        }

        setState(MainPageFreeState.Name);
        undoHelper.ResetSaveId();
    }

    private void CreatScheme(MyEvent obj)
    {
        //UnityIOSMsg.removeListioner(IOSEvent.CreatScheme, CreatScheme);
        IOSEvent e = obj as IOSEvent;
        MsgFromIOS.InfoFromIOS info = (MsgFromIOS.InfoFromIOS)e.data;
        schemeManifest.id = info.projectId;
        schemeManifest.isNew = false;
        cacheLocalSchemeManager.RemoveSchemeCache(schemeManifest.tempId);
        jsonCacheManager.SaveSchemeToLocal();
        //抛方案本地变服务器的事件 用于临时方案临时报价列表-变成-服务器方案临时报价列表
        mainpageData.dispatchEvent(new MainPageUIDataEvent(MainPageUIDataEvent.SchemeIdChanged, info));
        controller.dispatchEvent(new MyEvent(MySaveSchemeToShare.SaveSchemeToShare));
        schemePageControl.dispatchEvent(new MyEvent (SchemeEvent.GenerateOffer));

    }


    public override void exit()
    {
        
        base.exit();
    }

    //[System.Serializable]
    //public class TestPrice
    //{
    //    public System.Collections.Generic.List<PriceData> prices;
    //}
}
