using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SchemeManifestScroll :UGUIScrollView {

    public Arrangement _arrangement = Arrangement.Vertical;
    // item 的 view
    private GameObject view;
    // additem 的 view
    private ManifestOfferItem specialitem;
    // 与 itemSkin 不同就是 它是一个添加按钮  ， 不需要绑定数据
    private int AddItemPerfab = 1;
    private GameObject specialskin;

    protected override void Init()
    {
        ItemSkin = transform.Find("ManifestItemSkin").gameObject;
        view = ItemSkin.transform.FindChild("view").gameObject;
        UDragScroll uscr = view.AddComponent<UDragScroll>();
        uscr.scroRect = ScroRect;
        ItemSkin.AddComponent<SchemeManifestItemFunc>();
        specialitem = new ManifestOfferItem("", "", "", "", "", "", 0);
        specialitem.special = true;
        SetData(672, 504, _arrangement, 6, 20, 70, 30, 2);
        // 初始化以后就隐藏 点击清单 在出现   
        UITool.SetActionFalse(this.gameObject);
    }
    protected override void ItemChildGameObject(GameObject obj = null)
    {
        view.GetComponent<UDragScroll>().scroRect = ScroRect;
        SchemeManifestItemFunc func = obj.GetComponent<SchemeManifestItemFunc>();
        func.OnChange = ItemIndexChange;
    }
    public void ItemIndexChange(int from, int to)
    {
        if (itemDic.ContainsKey(from) == false || itemDic.ContainsKey(to) == false)
        {
            Debug.LogWarning("ItemIndexChange Wrong ：index " + from + " or " + to + " 不存在！");
            return;
        }
        SchemeManifestItemFunc funcFrom = itemDic[from] as SchemeManifestItemFunc;
        SchemeManifestItemFunc funcTo = itemDic[to] as SchemeManifestItemFunc;
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

    public override void RefreshDisplay(List<ItemData> data = null, bool restPos = false, bool isChange = false)
    {
        //base.RefreshDisplay();
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
            SetContentSize(this.Msgs.Count + 2);
        }

        int index = 0;
        if (specialskin == null)
        {
            specialskin = GetInstance();
            GameObject addManifest = specialskin.transform.FindChild("view/AddManifest").gameObject;
            addManifest.SetActive(true);
            GameObject manifestImage = specialskin.transform.FindChild("view/ManifestImage").gameObject;
            manifestImage.SetActive(false);
            BoxCollider2D coll = specialskin.GetComponentInChildren<BoxCollider2D>();
            coll.enabled = false;
            GameObject text = specialskin.transform.FindChild("view/Text").gameObject;
            text.SetActive(false);
            coll.gameObject.SetActive(false);

        }//skinClone
        specialskin.transform.SetParent(ContentRectTrans);
        specialskin.transform.localPosition = GetLoaclPosByIndex(index);
        specialskin.transform.localScale = Vector3.one;
        specialskin.GetComponent<RectTransform>().SetSiblingIndex(index);
        SchemeManifestItemFunc itemfunc = specialskin.GetComponent<SchemeManifestItemFunc>();
        itemfunc.scroRect = ScroRect;
        itemfunc.data = specialitem;
        itemfunc.index = index;
        ItemAddListion(itemfunc);
        ItemChildGameObject(specialskin);
        for (int i = 0; i < this.Msgs.Count; i++)
        {
            if ((i < CurrentIndex - UpperLimitIndex) && (CurrentIndex > LowerLimitIndex) && !isChange)
            {
                return;
            }
            skinClone = GetInstance();
            skinClone.transform.SetParent(ContentRectTrans);
            skinClone.transform.localPosition = GetLoaclPosByIndex(i + AddItemPerfab);
            skinClone.transform.localScale = Vector3.one;
            skinClone.GetComponent<RectTransform>().SetSiblingIndex(i + AddItemPerfab);
            SchemeManifestItemFunc func = skinClone.GetComponent<SchemeManifestItemFunc>();
            func.scroRect = ScroRect;
            func.data = this.Msgs[i];
            func.index = i + AddItemPerfab;
            itemDic.Add(i + AddItemPerfab, func);
            ItemAddListion(func);
            ItemChildGameObject(skinClone);
        }
    }
    protected override void ResetPostion()
    {
        ContentRectTrans.anchoredPosition = Vector2.zero;
        CurrentIndex = 0;
    }
    public void ClickItem(ManifestOfferItem itemData, GameObject skin)
    {
        dispatchEvent(new SchemeEvent(SchemeEvent.openOffer, itemData));
    }

}
