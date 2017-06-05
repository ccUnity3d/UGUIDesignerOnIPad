using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreateOfferItemFunc : UGUIItemFunction {


    public Product produce;

    private ItemPrice itemData
    {
        get { return data as ItemPrice; }
    }
    private MainPage mainPage
    {
        get {
            return MainPage.Instance;
        }
    }

    private RawImage productImage;
    private Text productName;
    private Text brand;
    private Text no;
    // 单价
    private Text unitPrice;
    private Text number;
    private Text totalPrice;
    private Toggle selectToggle;
 
    private RectTransform itemRect;

    private MyScrollRect scrollRect; 
    
    // 取消添加单个报价到 清单
    public Action<CreateOfferItemFunc> cancelAddOffer;

    protected override void Awake()
    {
        itemRect = this.GetComponent<RectTransform>();
        productImage = UITool.GetUIComponent<RawImage>(itemRect,"RawImage");
        productName = UITool.GetUIComponent<Text>(itemRect,"name");
        brand = UITool.GetUIComponent<Text>(itemRect,"brand");
        no = UITool.GetUIComponent<Text>(itemRect,"no");
        unitPrice = UITool.GetUIComponent<Text>(itemRect, "unitPrice");
        number = UITool.GetUIComponent<Text>(itemRect,"numberText");
        selectToggle = UITool.GetUIComponent<Toggle>(itemRect, "total");
        selectToggle.onValueChanged.AddListener(onSelectToggle);
        totalPrice = UITool.GetUIComponent<Text>(this.transform,"total/Label");
    }
    void Start()
    {
        scrollRect = scroRect.GetComponent<MyScrollRect>();
    }
    public override void Render()
    {
        base.Render();
        productImage.texture = mainPage.Logo;
        SetTextureTool.SetTexture(productImage, produce.tempTexture, "notLocal");
        //Debug.Log(produceData.name);
        productName.text = produce.name;
        brand.text = produce.vendor;
        no.text = produce.seekId;
        unitPrice.text = "";
        number.text = itemData.count.ToString();
        totalPrice.text = "";
        selectToggle.isOn = MainPageUIData.Instance.GetCollected(produce.seekId);//itemData.seekId;
    }
    private void onSelectToggle(bool isTrue)
    {
        //selectToggle.isOn = !selectToggle.isOn;
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
            if (cancelAddOffer != null) cancelAddOffer(this);
            return;
        }
        //itemData.isCollect = false;
    }
    private void OnCollect(MyEvent obj)
    {
        //UnityIOSMsg.removeListioner(IOSEvent.Collect, OnCollect);
        IOSEvent e = obj as IOSEvent;
        MsgFromIOS.InfoFromIOS info = (MsgFromIOS.InfoFromIOS)e.data;
        //if (info.result == "0")
        //{
        //    return;
        //}
        switch (info.CollectType)
        {
            case "0":
                selectToggle.isOn = false;
                break;
            case "1":
                selectToggle.isOn = true;
                break;
            default:
                break;
        }
    }
}
