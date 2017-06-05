using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GeneraterOfferScroll : UGUIScrollView {

    SchemePageController schemePage { get { return SchemePageController.Instance; } }
    private MainPageUIData uiData
    {
        get
        {
            return MainPageUIData.Instance;
        }
    }
    private Arrangement _arrangement = Arrangement.Vertical;
    private List<GameObject> numberText;
    private List<GameObject> editorNumber;
    public List<ItemData> selectDeleteList = new List<ItemData>();
    private List<Toggle> toggleList = new List<Toggle>();
    private MyTweenRectPosition currentTween;
    // 当前删除itemRect
    private RectTransform currentItemRect;

    public bool isLeftMove = true;

    public bool isEditor = false;
    protected override void Init()
    {
        ItemSkin = transform.Find("itemPictures").gameObject;
        UDragScroll us = ItemSkin.AddComponent<UDragScroll>();
        schemePage.scheme.mask.SetAsLastSibling();
        us.scroRect = ScroRect;
        ItemSkin.AddComponent<GenerateOfferItemFunc>();
        SetData(1980, 200, _arrangement, 6, 20);
        AddDeleteEvent(DeleteAllSelectItem);
        AddToggleEvent(ToggleOffOrOn);
    }
    protected override void ItemAddListion(UGUIItemFunction func)
    {
        base.ItemAddListion(func);
        GenerateOfferItemFunc itemFunc = func as GenerateOfferItemFunc;
        itemFunc.deleteFun = DeleFunc;
        itemFunc.selectDeleteFunc = SelectDelete;
        itemFunc.deleteTween = DeleteTween;
        itemFunc.cancelDeleteFunc = CancelDelete;
    }
    protected override void ItemChildGameObject(GameObject obj = null)
    {
        base.ItemChildGameObject(obj);
       Toggle Temp = UITool.GetUIComponent<Toggle>(obj.transform, "total");
        toggleList.Add(Temp);
    }
    public override void RefreshDisplay(List<ItemData> data = null, bool restPos = false, bool isChange = false)
    {
      
        foreach (UGUIItemFunction item in itemDic.Values)
        {
            item.gameObject.SetActive(false);
            skinList.Push(item.gameObject);
        }
        //}
        itemDic.Clear();
        if (restPos == true) ResetPostion();
        if (data != null)
        {
            this.Msgs = data;
        }
        if (data != null || isChange)
        {
            SetContentSize(this.Msgs.Count +1);
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
            GenerateOfferItemFunc func = skinClone.GetComponent<GenerateOfferItemFunc>();
            func.scroRect = ScroRect;
            ItemAddListion(func);
            func.produce = uiData.getProduct(itemPrice.seekId);
            func.data = this.Msgs[i];
            func.index = i;
            func.isLeftMove = isLeftMove;
            itemDic.Add(i, func);
            
            ItemChildGameObject(skinClone);
           
        }
        SchemePage.Instance.genertaTotalNumber.text = string.Format("共计使用{0}件物品", totalProduct);
    }
    private GenerateOfferItemFunc offerItemFunc;
    private void DeleFunc(GenerateOfferItemFunc obj)
    {
        this.offerItemFunc = obj;
        MessageBox.Instance.ShowOkCancel("","你确定要删除已选中商品吗？", CancelAction, OkAction );
    }
    private void OkAction()
    {
        msgs.Remove(offerItemFunc.data);
        RefreshDisplay(null, false, true);
    }
    private void CancelAction()
    {
        offerItemFunc.myTween.SetStartToCurrentValue();
        offerItemFunc.myTween.to = Vector2.up * offerItemFunc.itemRect.anchoredPosition.y;
        offerItemFunc.myTween.PlayForward();
    }
    private void SelectDelete(GenerateOfferItemFunc obj)
    {
        Debug.Log("添加要删除的产品"+obj.index);
        if (!selectDeleteList.Contains(obj.data))
        {
            selectDeleteList.Add(obj.data);
            Debug.Log("选择删除的项目： " + obj.index);
        }
        
    }
    private void DeleteTween(GenerateOfferItemFunc obj)
    {
        if (obj.myTween == null) return;

        if (currentTween == null) { currentTween = obj.myTween; currentItemRect = obj.itemRect ; return; }

        currentTween.SetStartToCurrentValue();
        currentTween.to = Vector2.up * currentItemRect.anchoredPosition.y;
        currentTween.PlayForward();
        currentTween = obj.myTween;
        currentItemRect = obj.itemRect;
    }
    private void CancelDelete(GenerateOfferItemFunc obj)
    {
        if (selectDeleteList.Contains(obj.data))
        {
            selectDeleteList.Remove(obj.data);
            Debug.Log("选择取消删除的项目： " + obj.index);
        }
        //RefreshDisplay();
        schemePage.isPress = false;
        schemePage.scheme.offerAllCancel.text = "全选";
    }

    public  void DeleteAllSelectItem()
    {
        foreach (ItemData obj in selectDeleteList)
        {
            msgs.Remove(obj);
        }
        RefreshDisplay(null, false, true);
    }
    public void ToggleOffOrOn(bool isOn)
    {
        if (isOn)
        {
            for (int i = 0; i < toggleList.Count; i++)
            {
                toggleList[i].isOn = isOn;
            }
            return;
        }
        for (int i = 0; i < toggleList.Count; i++)
        {
            toggleList[i].isOn = false;
        }
        //toggleGroup.SetAllTogglesOff();
    }

    public delegate void AllDelete();
    public delegate void ToggleOff(bool toggle);
    private static  event AllDelete DeleteEvent;
    private static event ToggleOff toggleEvent;
    private void AddDeleteEvent(AllDelete func )
    {
        DeleteEvent += func;
    }
    public void AddToggleEvent(ToggleOff func)
    {
        toggleEvent += func;
    }
    private  void RemoveDeleteEvent(AllDelete func)
    {
        if (DeleteEvent != null)
        {
            DeleteEvent -= func;
        }
    }
    private void RemoveToggleEvent(ToggleOff func)
    {
        if (toggleEvent != null)
        {
            toggleEvent -= func;
        }
    }
    // 编辑要删除的报价
    public static void DeleteSelete()
    {
        if (DeleteEvent != null)
        {
            DeleteEvent();
        }
    }
    public static void ToggleGroup(bool isOn)
    {
        if (toggleEvent != null)
        {
            toggleEvent(isOn);
        }
    }

}
