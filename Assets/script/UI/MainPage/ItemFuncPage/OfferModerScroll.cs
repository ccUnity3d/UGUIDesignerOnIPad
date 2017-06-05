using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OfferModerScroll : UGUIScrollView {


    public int _cellWidthSpace;
    public int _cellHeightSpace;
    public Arrangement _arrangement = Arrangement.Vertical;
    public int _upperLimitIndex;
    public int _lowerLimitIndex;
    public int _maxPerLine;


    private MainPageUIData uiData
    {
        get
        {
            return MainPageUIData.Instance;
        }
    }

    protected override void Init()
    {
        ItemSkin = transform.Find("itemModel").gameObject;
        ItemSkin.AddComponent<OfferModerItemFunc>();
        UDragScroll uscr = ItemSkin.AddComponent<UDragScroll>();
        uscr.scroRect = ScroRect;
        SetData(994, 200, _arrangement, _upperLimitIndex, _lowerLimitIndex, _cellWidthSpace, _cellHeightSpace, 2);
        // 测试
        //for (int i = 0; i < 20; i++)
        //{
        //    msgs.Add(new ItemData());
        //}
        //Display(msgs);
    }
    public override void RefreshDisplay(List<ItemData> data = null, bool restPos = false, bool isChange = false)
    {
        Debug.Log("offerModelScroller" + data.Count);
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
            SetContentSize(this.Msgs.Count);
        }

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
            OfferModerItemFunc func = skinClone.GetComponent<OfferModerItemFunc>();
            func.scroRect = ScroRect;
            func.produce = uiData.getProduct(itemPrice.seekId);
            func.data = this.Msgs[i];
            func.index = i;
            itemDic.Add(i, func);
            ItemAddListion(func);
            ItemChildGameObject(skinClone);
        }
    }
    protected override void ItemAddListion(UGUIItemFunction func)
    {
        base.ItemAddListion(func);
    }


}
