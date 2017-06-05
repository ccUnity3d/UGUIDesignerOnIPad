using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CameraViewScroll : UGUIScrollView  {

    //public RectTransform controllerAddDelet;
    public Action<int> onItemClick;
    public Button addItem;
    public Button showDelete;
    public Image cancelDelete;
    private UndoHelper undoHelper
    {
        get {
            return UndoHelper.Instance;
        }
    }

    protected override void Init()
    {
        ItemSkin = transform.Find("item").gameObject;

        viewRect.anchoredPosition = Vector2.right * 20;
        //controllerAddDelet = UITool.GetUIComponent<RectTransform>(transform, "Controller");
        //UITool.SetActionTrue(controllerAddDelet.gameObject);
        //controllerMove.SetParent(ContentRectTrans);
        //controllerMove.localScale = Vector3.one;
        //addItem = UITool.GetUIComponent<Button>(ScrollRectTrans, "Add");

        showDelete = UITool.GetUIComponent<Button>(ScrollRectTrans, "Delete");
        showDelete.onClick.AddListener(onDeleteCameraView);
        cancelDelete = UITool.GetUIComponent<Image>(ScrollRectTrans, "Delete/cancelDelete");

        UDragScroll uscr = ItemSkin.AddComponent<UDragScroll>();
        uscr.scroRect = ScroRect;
        ItemSkin.AddComponent<CamereViewItemFunc>();
        _Arranement = Arrangement.Vertical;
        SetData(320,240, _Arranement, 6,6,20,20);

        viewRect.SetAsFirstSibling();
    }

    bool isPress = false;

    public void CancelPress()
    {
        if (isPress == true)
        {
            isPress = false;
            RefreshDisplay(null, false, true);
        }
    }
    private void onDeleteCameraView()
    {
        isPress = !isPress;
        RefreshDisplay(null, false, true);
    }

    // 触发添加按钮 

    // 移除相机  视图
    protected override void ItemAddListion(UGUIItemFunction func)
    {
        base.ItemAddListion(func);
        CamereViewItemFunc itemfun = func as CamereViewItemFunc;
        itemfun.deleFun = DeleFun;
        //itemfun.deleFollowMouse = FollowMouse;
        //itemfun.deleSetDraggedPosition = SetDraggedPosition;
        //itemfun.deleSetDraggingIcon = SetDraggingIcon;
        //itemfun.deleEndDragCopy = EndDragCopy;
    }

    private void DeleFun(CamereViewItemFunc itemFunc)
    {
        undoHelper.save();

        int index = itemFunc.index;
        Msgs.RemoveAt(index);
        //if (Msgs.Count >= 3)
        //{
        //    ScrollRectTrans.sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * 6 + CellHeightSpace * (6));
        //    //controllerAddDelet.sizeDelta = new Vector2(GetLoaclPosByIndex(6).x, -GetLoaclPosByIndex(6).y + 60);
        //}
        //else
        //{
        //    ScrollRectTrans.sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * (Msgs.Count) + CellHeightSpace * (Msgs.Count));
        //    //controllerAddDelet.sizeDelta = new Vector2(GetLoaclPosByIndex(Msgs.Count + 1).x, -GetLoaclPosByIndex(Msgs.Count + 1).y + 60);
        //}
        ////MaterialController.sizeDelta = new Vector2(GetLoaclPosByIndex(Msgs.Count).x, -GetLoaclPosByIndex(Msgs.Count).y + 60);
        ////RefreshDisplay(Msgs, false, true);
        //Debug.Log(ScrollRectTrans.sizeDelta);
        RefreshDisplay(null, false, true);

        dispatchEvent(new CameraViewEvent(CameraViewEvent.RemoveCameraView, itemFunc.data));

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
            CamereViewItemFunc func = skinClone.GetComponent<CamereViewItemFunc>();
            func.scroRect = ScroRect;
            func.data = this.Msgs[i];
            func.index = i;
            func.deleClick = ClickItem;
            itemDic.Add(i, func);
            ItemAddListion(func);
            func.delete.gameObject.SetActive(isPress);
        }

        // 容器的区域限制
        if (data != null || isChange == true)
        {
            int length = Msgs.Count;
            if (length >= 3)
            {
                ScrollRectTrans.sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * 3 + CellHeightSpace * (2));
                //controllerAddDelet.sizeDelta = new Vector2(GetLoaclPosByIndex(6).x, -GetLoaclPosByIndex(6).y + 60);
            }
            else if (length == 0)
            {
                ScrollRectTrans.sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, 120);
                
            }
            else
            {
                ScrollRectTrans.sizeDelta = new Vector2(ContentRectTrans.sizeDelta.x, CellHeight * length + CellHeightSpace * length);
                //controllerAddDelet.sizeDelta = new Vector2(GetLoaclPosByIndex(length).x, -GetLoaclPosByIndex(length).y + 60);
            }
            //MaterialController.sizeDelta = new Vector2(GetLoaclPosByIndex(length).x, -GetLoaclPosByIndex(length).y + 60);
        }
    }

    private void ClickItem(int index)
    {
        if (onItemClick != null) onItemClick(index);
    }

    public void DeleteEnd()
    {
        if (isPress == true)
        {
            onDeleteCameraView();
        }
    }

    //public void SetDraggedPosition(PointerEventData data)
    //{
    //    MyTweenRectPosition myTween = controllerAddDelet.GetComponent<MyTweenRectPosition>();
    //    if (myTween == null) myTween = controllerAddDelet.gameObject.AddComponent<MyTweenRectPosition>();
    //    if (ScrollRectTrans.sizeDelta.y < controllerAddDelet.sizeDelta.y)
    //    {
    //        controllerAddDelet.sizeDelta = new Vector2(100, ScrollRectTrans.sizeDelta.y + 100);
    //    }
    //    else
    //    {
    //        controllerAddDelet.sizeDelta = new Vector2(100, controllerAddDelet.sizeDelta.y + 100);
    //    }
    //    myTween.from = Vector2.up * -30f;
    //    myTween.to = Vector2.up * 100f;
    //    myTween.duration = 1;
    //    if (data.delta.y > 0)
    //    {
    //        myTween.SetStartToCurrentValue();
    //        myTween.to = Vector2.up * 100f;
    //        myTween.PlayForward();
    //        return;
    //    }
    //    myTween.SetStartToCurrentValue();
    //    myTween.to = Vector2.up * -30;
    //    myTween.PlayForward();

    //}
}
