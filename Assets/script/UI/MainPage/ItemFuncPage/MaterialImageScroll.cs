using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class MaterialImageScroll : UGUIScrollView,IDragHandler {

    public int _cellWidth;
    public int _cellHeight;
    public int _cellWidthSpace;
    public int _cellHeightSpace;
    public Arrangement _arrangement = Arrangement.Vertical;
    public int _upperLimitIndex;
    public int _lowerLimitIndex;
    public int _maxPerLine;
    public RectTransform MaterialController;
    public Button materialAdd;
    public Button materialDelete;
    public Image cancelDelete;
    public RectTransform DraggingIcon;
    public List<Button> deleteList = new List<Button>();
    public Action<MaterialItemData> deleDropMaterial;
    // 特殊数据
    private MaterialItemData specialitem;
    // 一个空的材质球，
    private GameObject specialskin;
    //  添加 特殊 Item 数量
    private int AddItemPerfab = 1;
    private MaterialItemData specialitemBottom;
    // 一个空的材质球，
    private GameObject specialBottomskin;
    protected override void ResetPostion()
    {
        ContentRectTrans.anchoredPosition = new Vector2(0,-30); 
        CurrentIndex = 0;
    }
    protected override void ItemChildGameObject(GameObject obj = null)
    {
        base.ItemChildGameObject(obj);
        Button temp = UITool.GetUIComponent<Button>(obj.transform,"delete");
        deleteList.Add(temp);
    }
    protected override void Init()
    {
        ItemSkin = transform.Find("item").gameObject;
        MaterialController = UITool.GetUIComponent<RectTransform>(transform,"Controller");
        MaterialController.gameObject.SetActive(true);
        MaterialController.SetParent(ContentRectTrans.parent);
        MaterialController.localScale = Vector3.one;
        materialDelete = UITool.GetUIComponent<Button>(MaterialController,"Delete"); 
        materialDelete.onClick.AddListener(RemoveMaterial);
        cancelDelete = UITool.GetUIComponent<Image>(MaterialController, "Delete/cancelDelete");
        UDragScroll uscr = ItemSkin.AddComponent<UDragScroll>();
        uscr.scroRect = ScroRect;
        ItemSkin.AddComponent<MaterailImageItemFunc>();
        specialitem = new MaterialItemData("", "", "");
        specialitemBottom = new MaterialItemData("", "", "");
        specialitemBottom.special = true;
        specialitem.special = true;
        SetData(100, 130, _arrangement, 100, 100, 10, 30, 1);
        //for (int i = 0; i < 1; i++)
        //{
        //    Msgs.Add(new ItemData());
        //}
        //RefreshDisplay(Msgs,false,true);
        //skinClone.SetActive(false);
        //Display(Msgs);
    }

    protected override void ItemAddListion(UGUIItemFunction func)
    {
        base.ItemAddListion(func);
        MaterailImageItemFunc itemfun = func as MaterailImageItemFunc;
        itemfun.deleFun = DeleFun;
        itemfun.deleFollowMouse = FollowMouse;
        itemfun.deleSetDraggedPosition = SetDraggedPosition;
        itemfun.deleSetDraggingIcon = SetDraggingIcon;
        itemfun.deleEndDragCopy = EndDragCopy;
    }

    internal void FollowMouse(PointerEventData obj)
    {
        Vector3 temp = GlobalConfig.Canvas.worldCamera.ScreenToWorldPoint(obj.position);
        DraggingIcon.position = new Vector3(temp.x, temp.y, 0);
    }

    public void DeleFun(MaterailImageItemFunc itemFunc)
    {
        ItemData item = itemFunc.data;
        Msgs.Remove(item);
        if (Msgs.Count >= 6)
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * 6 + CellHeightSpace * (6));
            MaterialController.sizeDelta = new Vector2(GetLoaclPosByIndex(6).x, -GetLoaclPosByIndex(6).y + 60);
        }
        else
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * (Msgs.Count) + CellHeightSpace * (Msgs.Count));
            MaterialController.sizeDelta = new Vector2(GetLoaclPosByIndex(Msgs.Count+1).x, -GetLoaclPosByIndex(Msgs.Count+1).y + 60);
        }
        Debug.Log(MaterialController.sizeDelta);
        RefreshDisplay(null, false, true);

        dispatchEvent(new MaterialEvent(MaterialEvent.RemoveMaterial, item));
    }

    public void SetDraggedPosition(PointerEventData data)
    {
        MyTweenRectPosition myTween = MaterialController.GetComponent<MyTweenRectPosition>();
        if (myTween == null) myTween = MaterialController.gameObject.AddComponent<MyTweenRectPosition>();
        if (ScrollRectTrans.sizeDelta.y < MaterialController.sizeDelta.y)
        {
            MaterialController.sizeDelta = new Vector2(100, ScrollRectTrans.sizeDelta.y + 100);
        }
        else
        {
            MaterialController.sizeDelta = new Vector2(100, MaterialController.sizeDelta.y + 100);
        }
        myTween.from = Vector2.up * -30f;
        myTween.to = Vector2.up * 130f;
        myTween.duration = 1;
        if (data.delta.y > 0)
        {
            myTween.SetStartToCurrentValue();
            myTween.to = Vector2.up * 130f;
            myTween.PlayForward();
            return;
        }
        myTween.SetStartToCurrentValue();
        myTween.to = Vector2.up * -30;
        myTween.PlayForward();

    }

    public void SetDraggingIcon(MaterialItemData itemdata)
    {
        DraggingIcon = new GameObject("Material", typeof(RectTransform), typeof(RawImage)).GetComponent<RectTransform>();
        DraggingIcon.SetParent(GlobalConfig.UIParentObj.transform);
        DraggingIcon.SetAsLastSibling();
        DraggingIcon.localScale = Vector3.one;
        RawImage image = DraggingIcon.GetComponent<RawImage>();
        image.raycastTarget = false;
        SetTextureTool.SetTexture(image, itemdata.textureURL, itemdata.seekId);
        if (deleDropMaterial != null) deleDropMaterial(itemdata);
    }

    public void EndDragCopy()
    {
        if (DraggingIcon != null)
        {
            GameObject.DestroyImmediate(DraggingIcon.gameObject, true);
        }
    }

    public override Vector3 GetLoaclPosByIndex(int index)
    {
        //if (index == 0)
        //{
        //    return new Vector3(0,-30,0);
        //}
        float x = 0f;
        float y = 0f;
        switch (_arrangement)
        {
            case Arrangement.Horizontal:
                x = (index / MaxPerLine) * (CellWidth + CellWidthSpace);
                y = -(index % MaxPerLine) * (CellHeight + CellHeightSpace);
                break;
            case Arrangement.Vertical:
                x = (index % MaxPerLine) * (CellWidth + CellWidthSpace);
                y = -(index / MaxPerLine) * (CellHeight + CellHeightSpace) - CellHeightSpace;
                break;
        }

        return new Vector3(x + 30f, y);
        // return base.GetLoaclPosByIndex(index);
    }

    public override void SetContentSize(int length)
    {
        int lineCount = length / MaxPerLine ;
        switch (_arrangement)
        {
            case Arrangement.Horizontal:
                ContentRectTrans.sizeDelta = new Vector2(CellWidth * lineCount + CellWidthSpace * (lineCount - 1), ContentRectTrans.sizeDelta.y);
                break;
            case Arrangement.Vertical:
                ContentRectTrans.sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * lineCount + CellHeightSpace * (lineCount ));
                break;
        }
    }
    public void AddMaterial(List<MaterialItemData> items)
    {
        for (int i = 0; i < items.Count; i++)
        {
            Msgs.Add(items[i]);
        }
        RefreshDisplay(Msgs, false, true);
        
    }
    public override void RefreshDisplay(List<ItemData> data = null, bool restPos = false, bool isChange = false)
    {
        //base.RefreshDisplay(data, restPos, isChange);

        //if (!isAdd)
        //{
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
            SetContentSize(this.Msgs.Count+2);
        }
        int index = 0;
        if (specialskin == null)
        {
            specialskin = GetInstance();
            Debug.Log(specialskin);
            UITool.SetActionTrue(specialskin);
        }
        specialskin.transform.SetParent(ContentRectTrans);
        specialskin.transform.localPosition = GetLoaclPosByIndex(index);
        specialskin.transform.localScale = Vector3.one;
        specialskin.GetComponent<RectTransform>().SetSiblingIndex(index);
        MaterailImageItemFunc itemfunc = specialskin.GetComponent<MaterailImageItemFunc>();
        itemfunc.scroRect = ScroRect;
        itemfunc.data = specialitem;
        itemfunc.index = index;
        ItemAddListion(itemfunc);
        // ItemChildGameObject(specialskin);
        for (int i = 0; i < this.Msgs.Count; i++)
        {
            if ((i < CurrentIndex - UpperLimitIndex) && (CurrentIndex > LowerLimitIndex) && !isChange)
            {
                return;
            }
            skinClone = GetInstance();
            skinClone.transform.SetParent(ContentRectTrans);
            skinClone.transform.localPosition = GetLoaclPosByIndex(i+ AddItemPerfab);
            skinClone.transform.localScale = Vector3.one;
            skinClone.GetComponent<RectTransform>().SetSiblingIndex(i+ AddItemPerfab);
            MaterailImageItemFunc func = skinClone.GetComponent<MaterailImageItemFunc>();
            func.scroRect = ScroRect;
            func.data = this.Msgs[i];
            func.index = i+ AddItemPerfab;
            itemDic.Add(i + AddItemPerfab, func);
            ItemAddListion(func);
            ItemChildGameObject(skinClone);
        }
        if (specialBottomskin == null)
        {
            specialBottomskin = GetInstance();
            Debug.Log(specialskin);
            UITool.SetActionTrue(specialBottomskin);
        }
        specialBottomskin.transform.SetParent(ContentRectTrans);
        specialBottomskin.transform.localPosition = GetLoaclPosByIndex(this.Msgs.Count + 1);
        specialBottomskin.transform.localScale = Vector3.one;
        specialBottomskin.GetComponent<RectTransform>().SetSiblingIndex(this.Msgs.Count + 1);
        MaterailImageItemFunc itemBottomFunc = specialBottomskin.GetComponent<MaterailImageItemFunc>();
        itemBottomFunc.scroRect = ScroRect;
        itemBottomFunc.data = specialitemBottom;
        itemBottomFunc.index = index + 1;
        ItemAddListion(itemBottomFunc);

        // 容器的区域限制
        if (data != null || isChange == true)
        {
            int length = Msgs.Count;
            if (length >= 6)
            {
                ScrollRectTrans.sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * 6 + CellHeightSpace * (6));
                MaterialController.sizeDelta = new Vector2(GetLoaclPosByIndex(6).x, -GetLoaclPosByIndex(6).y + 60);
            }
            else
            {
                ScrollRectTrans.sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * (length+1) + CellHeightSpace * (length+1));
                MaterialController.sizeDelta = new Vector2(GetLoaclPosByIndex(length+1).x, -GetLoaclPosByIndex(length+1).y + 60);
            }
            //MaterialController.sizeDelta = new Vector2(GetLoaclPosByIndex(length).x, -GetLoaclPosByIndex(length).y + 60);
        }
    }

    bool isPress = false;
    private void RemoveMaterial()
    {
        isPress = !isPress;
        if (isPress)
        {
            foreach (Button item in deleteList)
            {
                
                item.gameObject.SetActive(isPress);

            }
            cancelDelete.gameObject.SetActive(isPress);
        }
        else
        {
            foreach (Button item in deleteList)
            {
                item.gameObject.SetActive(isPress);
            }
            cancelDelete.gameObject.SetActive(isPress);
        }
        Debug.Log("移除材质球");
    }
    public void OnDrag(PointerEventData eventData)
    {
        SetDraggedPosition(eventData);
    }
    
}
