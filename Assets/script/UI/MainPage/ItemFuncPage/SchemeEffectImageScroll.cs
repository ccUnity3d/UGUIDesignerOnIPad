using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SchemeEffectImageScroll : UGUIScrollView {

    public Arrangement _arrangement = Arrangement.Vertical;

    private SchemePageController schemeController { get { return SchemePageController.Instance; } }
    private GameObject view;
    protected override void Init()
    {
        ItemSkin = transform.Find("ImageItemSkin").gameObject;
        view = ItemSkin.transform.FindChild("GameObject/view").gameObject;
        UDragScroll uscr = view.AddComponent<UDragScroll>();
        uscr.scroRect = ScroRect;
        ItemSkin.AddComponent<SchemeEffectImageItemFunc>();
        SetData(672, 504, _arrangement,6,20,70,30,2);

    }
    public override void RefreshDisplay(List<ItemData> data = null, bool restPos = false, bool isChange = false)
    {
        //base.RefreshDisplay(data, restPos, isChange);
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
            SetContentSize(this.Msgs.Count+1);
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
            SchemeEffectImageItemFunc func = skinClone.GetComponent<SchemeEffectImageItemFunc>();
            func.scroRect = ScroRect;
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
    protected override void ResetPostion()
    {
        ContentRectTrans.anchoredPosition = Vector2.zero;
        CurrentIndex = 0;
    }

    protected override void ItemChildGameObject(GameObject obj = null)
    {
        view.GetComponent<UDragScroll>().scroRect = ScroRect;
        SchemeEffectImageItemFunc func = skinClone.GetComponent<SchemeEffectImageItemFunc>();
        func.OnChange = ItemIndexChange;
    }

    public void ItemIndexChange(int from, int to)
    {
        if (itemDic.ContainsKey(from) == false || itemDic.ContainsKey(to) == false)
        {
            Debug.LogWarning("ItemIndexChange Wrong ：index " + from + " or " + to + " 不存在！");
            return;
        }
        SchemeEffectImageItemFunc funcFrom = itemDic[from] as SchemeEffectImageItemFunc;
        SchemeEffectImageItemFunc funcTo = itemDic[to] as SchemeEffectImageItemFunc;
        funcFrom.index = to;
        funcTo.index = from;
        itemDic[from] = funcTo;
        itemDic[to] = funcFrom;

        ItemData fromdata = this.msgs[from];
        ItemData todata = this.msgs[to];
        this.msgs[from] = todata;
        this.msgs[to] = fromdata;

        RefreshDisplay();
    }
    
}
