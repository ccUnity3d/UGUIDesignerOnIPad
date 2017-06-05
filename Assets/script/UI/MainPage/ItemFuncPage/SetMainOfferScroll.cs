using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetMainOfferScroll : UGUIScrollView {

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
        ItemSkin.AddComponent<SetMainOfferItemFunc>();
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
            //记录当前图片
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
            SetMainOfferItemFunc func = skinClone.GetComponent<SetMainOfferItemFunc>();
            func.scroRect = ScroRect;
            func.produce = uiData.getProduct(itemPrice.seekId);
            func.data = this.Msgs[i];
            func.index = i;
            itemDic.Add(i, func); 
            ItemAddListion(func);
            ItemChildGameObject(skinClone);
        }

        //移除记录中剩余图片
        //释放未使用资源

    }
}
