using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CreateOfferScroll : UGUIScrollView
{

    private MainPageUIData uiData
    {
        get
        {
            return MainPageUIData.Instance;
        }
    }

    private Arrangement _arrangement = Arrangement.Vertical;

    protected override void Init()
    {
        ItemSkin = transform.Find("itemPictures").gameObject;
        ItemSkin.AddComponent<CreateOfferItemFunc>();
        UDragScroll uscr = ItemSkin.AddComponent<UDragScroll>();
        uscr.scroRect = ScroRect;
        SetData(1980, 200, _arrangement, 6, 20);
        //Display(msgs);
    }

    public override void RefreshDisplay(List<ItemData> data = null, bool restPos = false, bool isChange = false)
    {
        foreach (UGUIItemFunction item in itemDic.Values)
        {
            item.gameObject.SetActive(false);
            skinList.Push(item.gameObject);
        }
        itemDic.Clear();
        if (restPos == true) ResetPostion();
        if (data != null)
        {
            this.Msgs = data;
        }
        if (data != null || isChange)
        {
            SetContentSize(this.Msgs.Count +1 );
        }
        int totalProduct = 0;
        for (int i = 0; i < this.Msgs.Count; i++)
        {
            if ((i < CurrentIndex - UpperLimitIndex) && (CurrentIndex > LowerLimitIndex) && !isChange)
            {
                return;
            }
            skinClone = GetInstance();
            skinClone.transform.SetParent(ContentRectTrans);
            skinClone.transform.localPosition = GetLoaclPosByIndex(i);
            skinClone.transform.localScale = Vector3.one;
            skinClone.GetComponent<RectTransform>().SetSiblingIndex(i);
            ItemPrice itemPrice = this.Msgs[i] as ItemPrice;
            totalProduct += itemPrice.count;
            CreateOfferItemFunc func = skinClone.GetComponent<CreateOfferItemFunc>();
            func.scroRect = ScroRect;
            func.produce = uiData.getProduct(itemPrice.seekId);
            func.data = this.Msgs[i];
            func.index = i;
            itemDic.Add(i, func);
            ItemAddListion(func);
            ItemChildGameObject(skinClone);
        }
        SchemePage.Instance.createTotalNumber.text = string.Format("共计使用{0}件物品", totalProduct);
       // SchemePage.Instance.genertaTotalNumber.text = string.Format("共计使用{0}件物品",totalProduct);
    }

    protected override void ItemAddListion(UGUIItemFunction func)
    {
        base.ItemAddListion(func);
        CreateOfferItemFunc itemFunc = func as CreateOfferItemFunc;
        itemFunc.cancelAddOffer = CancelAddOffer;
    }
    /// <summary>
    ///  是否添加收藏
    /// </summary>
    /// <param name="obj"></param>
    private void CancelAddOffer(CreateOfferItemFunc obj)
    {
        //msgs.RemoveAt(obj.index);
        //RefreshDisplay(null, false, true);
    }
}
