using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class OfferModerItemFunc : UGUIItemFunction {

    private MainPage mainPage
    {
        get
        {
            return MainPage.Instance;
        }
    }

    public Product produce;
    private ItemPrice itemData
    {
        get { return data as ItemPrice; }

    }
    private RawImage productImage;
    private Text modelName;
    private Text no;
    private Toggle collecte;
    private Transform tran;
    protected override void Awake()
    {
        tran = this.transform;
        productImage = UITool.GetUIComponent<RawImage>(tran, "modelImage");
        modelName = UITool.GetUIComponent<Text>(tran,"name");
        no = UITool.GetUIComponent<Text>(tran,"no");
        collecte = UITool.GetUIComponent<Toggle>(tran, "collecte");
        collecte.onValueChanged.AddListener(onSelectToggle);
    }

    public override void Render()
    {
        base.Render();
        productImage.texture = mainPage.Logo;
        SetTextureTool.SetTexture(productImage, produce.tempTexture, "notLocal");
        modelName.text = produce.name;
        no.text = produce.seekId;
        collecte.isOn = MainPageUIData.Instance.GetCollected(itemData.seekId);
    }
    private void onSelectToggle(bool isTrue)
    {
        //selectToggle.isOn = !selectToggle.isOn;
 
        Debug.Log(produce.seekId);
        if (isTrue == true)
        {
            MsgToIOS msg = new MsgToIOS();
            msg.code = "101008";
            MsgToIOS.InfoToIOS info = new MsgToIOS.InfoToIOS();

            info.goodsId = produce.seekId;
            info.collectType = 1;//(isTrue == true)? 1:0;
            info.image = produce.tempTexture;
            msg.info = info;
            UnityIOSMsg.sendToIOS(msg, IOSEvent.Collect, OnCollect);
            MainPageUIController.Instance.SetCollected(produce.seekId, true);
            //if (cancelAddOffer != null) cancelAddOffer(this);
            return;
        }
        //itemData.isCollect = false;
    }
    private void OnCollect(MyEvent obj)
    {
        IOSEvent e = obj as IOSEvent;
        MsgFromIOS.InfoFromIOS info = (MsgFromIOS.InfoFromIOS)e.data;
        switch (info.CollectType)
        {
            case "0":
                collecte.isOn = false;
                break;
            case "1":
                collecte.isOn = true;
                break;
            default:
                break;
        }
    }
}
