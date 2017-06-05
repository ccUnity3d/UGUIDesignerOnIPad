using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FullScreenScroll : UGUIScrollView {

    private Arrangement arragement = Arrangement.Horizontal;
    private MyTweenRectPosition tween;
    private bool isScrollEnd = false;
    private Vector2 inputDic;
    private SchemePage schemePage { get { return SchemePage.Instance; } }
    private Image currPoint;
    private Transform rectTranEffectImage;
    public Button setMainBtn;
    public Button delete;
    public Button effectShareMain;

    public Dictionary<int, Image> pointDic = new Dictionary<int, Image>();

    private SchemeManifest schemeManifest
    {
        get {
            return SchemeManifest.Instance;
        }
    }

    protected override void Init()
    {
        ScroRect.movementType = UnityEngine.UI.ScrollRect.MovementType.Clamped;
        ItemSkin = transform.Find("item").gameObject;
        ItemSkin.AddComponent<UDragScroll>();
        ItemSkin.AddComponent<FullImageItemFunc>();
        rectTranEffectImage = this.transform.parent;
        setMainBtn = UITool.GetUIComponent<Button>(rectTranEffectImage, "setMainbtn");
        delete = UITool.GetUIComponent<Button>(rectTranEffectImage, "delete");
        effectShareMain = UITool.GetUIComponent<Button>(rectTranEffectImage, "share");
        RectTransform arrayPoint = UITool.GetUIComponent<RectTransform>(rectTranEffectImage, "ScrollPoint");


        SetData(2048,1536, arragement);
        ScroRect.OnScrollDragedStart = OnScrollDragedStart;
        ScroRect.OnScrollDragedEnd = OnScrollDragedEnd;
        setMainBtn.onClick.AddListener(OnEffectSetMain);
        delete.onClick.AddListener(OnEffectDelete);
        effectShareMain.onClick.AddListener(OnEffectShare);

        int i = 0;
        pointDic.Clear();
        Image[] Images = arrayPoint.GetComponentsInChildren<Image>(true);
        foreach (Image item in Images)
        {
            if (!pointDic.ContainsValue(item))
            {
                pointDic.Add(i, item);
            }
            i++;
        }
        currPoint = pointDic[0];

    }

    public override void RefreshDisplay(List<ItemData> data = null, bool restPos = false, bool isChange = false)
    {
        base.RefreshDisplay(data, restPos, isChange);
        foreach (Image item in pointDic.Values)
        {
            item.color = Color.white;
            item.gameObject.SetActive(false);
        }
        if (Msgs.Count > 1)
        {
            for (int i = 0; i < Msgs.Count; i++)
            {
                if (pointDic.ContainsKey(i) == false) {
                    break;
                }
                if (restPos == true)
                {
                    if (i == 0)
                    {
                        pointDic[i].color = Color.black;
                    }
                }
                //else
                //{
                //    currPoint
                //}
                pointDic[i].gameObject.SetActive(true);
            }
        }
        
    }

    /// <summary>
    /// data 不能直接使用 this.Msgs
    /// </summary>
    public override void Display(List<ItemData> data)
    {
        this.Msgs.Clear();
        for (int i = 0; i < 6; i++)
        {
            if (data.Count <= i)
            {
                break;
            }
            this.Msgs.Add(data[i]);
        }
        base.Display(this.Msgs);
    }
    
    private void OnScrollDragedStart(PointerEventData eventData)
    {
        inputDic = eventData.delta;
        if (tween != null && tween.enabled == true)
        {
            tween.enabled = false;
        }
    }

    private void OnScrollDragedEnd(PointerEventData eventData)
    {
        if (tween == null) tween = ContentRectTrans.gameObject.AddComponent<MyTweenRectPosition>();
        tween.from = ContentRectTrans.anchoredPosition;
        tween.to = ResetPos(eventData);
        tween.duration = 0.5f;
        tween.moveType = MoveType.Linear;
        tween.PlayForward();
    }
    public Vector2 ResetPos(PointerEventData eventData)
    {
        Vector2 pos = ContentRectTrans.anchoredPosition;
        // 判断水平滑动 是否越界
        if (pos.x>=0) return Vector2.zero;

        switch (_Arranement)
        {
            case Arrangement.Horizontal:
                {
                    int temp = Mathf.FloorToInt(pos.x / (CellWidthSpace + CellWidth));

                    if (inputDic.x > 0) temp++;

                    // 改变当前点的颜色
                    currPoint.color = Color.white;
                    pointDic[Mathf.Abs(temp)].color = Color.black;
                    currPoint = pointDic[Mathf.Abs(temp)];

                    return new Vector2((temp) * CellWidth, 0);
                }
        }
        return Vector2.zero;
    }

    protected override void ItemAddListion(UGUIItemFunction func)
    {
        base.ItemAddListion(func);
        FullImageItemFunc itemFunc = func as FullImageItemFunc;
        //itemFunc.onRestFiexdPos = ResetFixedPos;
    }
    #region Effect Onclick
    private void OnEffectSetMain()
    {
        Debug.Log("将效果图设置为主图");
        //this.dispatchEvent(new IOSEvent(IOSEvent.SetMainImage));

    }
    private void OnEffectDelete()
    {

        //正式
        //this.dispatchEvent(new IOSEvent(IOSEvent.DeletImage));

        //临时
        if (this.Msgs.Count == 0)
        {
            return;
        }
        EffectImageItem imageItem = this.Msgs[0] as EffectImageItem;
        dispatchEvent(new MainPageUIDataEvent(MainPageUIDataEvent.SchemeEffectImageDeleted, imageItem.path));
    }
    private void OnEffectShare()
    {
        Debug.Log("效果图分享");
        this.dispatchEvent(new IOSEvent(IOSEvent.ShareImage));
    }

    #endregion

}
