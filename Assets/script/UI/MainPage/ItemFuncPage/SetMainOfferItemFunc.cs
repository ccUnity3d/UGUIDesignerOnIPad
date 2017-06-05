using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SetMainOfferItemFunc : UGUIItemFunction {

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
   
    private RectTransform itemRect;
    private MyScrollRect scrollRect;

    // 取消添加单个报价到 清单
    public Action<CreateOfferItemFunc> cancelAddOffer;

    protected override void Awake()
    {
        itemRect = this.GetComponent<RectTransform>();
        productImage = UITool.GetUIComponent<RawImage>(itemRect, "RawImage");
        productName = UITool.GetUIComponent<Text>(itemRect, "name");
        brand = UITool.GetUIComponent<Text>(itemRect, "brand");
        no = UITool.GetUIComponent<Text>(itemRect, "no");
        unitPrice = UITool.GetUIComponent<Text>(itemRect, "unitPrice");
        number = UITool.GetUIComponent<Text>(itemRect, "numberText");
        totalPrice = UITool.GetUIComponent<Text>(this.transform, "total");
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
        productName.text = produce.name;
        brand.text = produce.vendor;
        no.text = produce.seekId;
        unitPrice.text = "";
        number.text = itemData.count.ToString();
        totalPrice.text = "";
    }
}
