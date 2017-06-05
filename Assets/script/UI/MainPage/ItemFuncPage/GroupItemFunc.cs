using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class GroupItemFunc : UGUIItemFunction
{
    private ProductData itemData
    {
        get {
            return data as ProductData;
        }
    }
    private MainPageUIData uiData
    {
        get {
            return MainPageUIData.Instance;
        }
    }

    private RawImage itemImage;
    private Transform tran;
    protected override void Awake()
    {
        tran = this.transform;
        itemImage = UITool.GetUIComponent<RawImage>(tran,"item");
    }
    public override void Render()
    {
        base.Render();
        if (itemData == null)
        {
            Debug.LogError("itemData == null");
            return;
        }
        Product product = uiData.getProduct(itemData.seekId);
        SetTextureTool.SetTexture(itemImage, product.tempTexture, "notlocal");
    }


}
