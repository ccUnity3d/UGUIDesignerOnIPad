using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class ImageFunc : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public ScrollRect scroRect;
    public ItemData data;
    public int index;
    //public RectTransform parent;

    public Action<int, int> OnChange;
    private bool isDown = false;
    private bool dragActive = false;
    private bool startDrag = false;
    private float beginTime = 0.0f;
    private float loapPressDelay = 0.5f;
    private bool moved = false;
    private Vector2 itemStartPos = Vector2.zero;
    /// <summary>
    /// 激活时候的信息（未激活 最后离开时候的信息）
    /// </summary>
    private Vector2 inputStartPos;
    private Vector2 offset = Vector2.zero;
    private RectTransform rectTran;
    private UScrollList uscroll;

    private RectTransform view;
    /// <summary>
    /// 报价
    /// </summary>
    private RectTransform offer;
    private ImageFunc dragToItem;
    private Collider2D []coll;
    private int count;
    private Vector2 viewDragActivePos;
    void Start()
    {
        view = this.transform.FindChild("view").GetComponent<RectTransform>();
        DrapButton button = view.gameObject.AddComponent<DrapButton>();
        button.onPointerDownDele = onPointerDown;
        button.onPointerUpDele = onPointerUp;
        button.onDragDele = onDrag;
        button.onPointerClickDele = onClick;
        uscroll = scroRect.GetComponent<UScrollList>();
        rectTran = this.GetComponent<RectTransform>();
        coll = new Collider2D[8];
        offer = this.transform.FindChild("view/Offer").GetComponent<RectTransform>();
        offer.GetComponent<Button>().onClick.AddListener(OpenOffer);
    }
    // Update is called once per frame
    void Update () {
        if (isDown == false || moved == true || dragActive == true) return;
        if (Time.time - beginTime > loapPressDelay)
        {
            scroRect.vertical = false;
            dragActive = true;
            itemStartPos = rectTran.anchoredPosition;
            dragToItem = null;
            viewDragActivePos = view.position;
        }

    }

    private void OpenOffer()
    {
        if (moved == true) return;
        Debug.Log("OpenOffer" + index);
    }
    private void onClick()
    {
        if (moved == true) return;
        Debug.Log("onClick"+index);
    }

    public void onPointerDown(PointerEventData eventData)
    {
        isDown = true;
        moved = false;
        inputStartPos = eventData.position;
        beginTime = Time.time;
    }

    public void onPointerUp(PointerEventData eventData)
    {
        isDown = false;
        scroRect.vertical = true;
        if (dragActive == false)
        {
            return;
        }
        if (dragToItem != null)
        {
            float time = 0.2f;
            Vector2 pos = uscroll.GetItemPos(dragToItem.index);
            GoToPos(rectTran, pos);
            MyCallLater.Add(time, laterChange);
        }
        else
        {
            Vector2 pos = uscroll.GetItemPos(index);
            GoToPos(rectTran, pos);
        }
        dragActive = false;
    }
    public void onDrag(PointerEventData eventData)
    {
        if (eventData.delta != Vector2.zero)
        {
            moved = true;
        }
        if (dragActive == true)
        {
            rectTran.SetSiblingIndex(100);
            SetDraggedPosition(eventData);
            return;
        }
    }
    private void laterChange()
    {
        ResetViewPos(rectTran);
        dragToItem.ResetViewPos(dragToItem.view);
        if (OnChange != null) OnChange(this.index, dragToItem.index);
    }

    private void ResetViewPos(RectTransform rect)
    {
        MyTweenRectPosition tween = rect.GetComponent<MyTweenRectPosition>();
        if (tween != null) tween.enabled = false;
        rect.anchoredPosition3D = Vector2.zero;
    }

    void SetDraggedPosition(PointerEventData data)
    {
        if (offset == data.position - inputStartPos)
        {
            return;
        }
        offset = data.position - inputStartPos;
        rectTran.anchoredPosition = itemStartPos + GetPos(offset);

        Vector2 org = new Vector2(rectTran.position.x + 1.8f, rectTran.position.y - 1.3f);

        count = Physics2D.OverlapPointNonAlloc(org, coll);

        if (count==0) return;

        bool hasOldFunc = false;

        for (int i = 0; i < count; i++)
        {
            if (coll[i].transform.parent == null) continue;
            if (coll[i].transform.parent == this.transform) continue;
            if (dragToItem != null && dragToItem.transform == coll[i].transform.parent)
            {
                hasOldFunc = true;
                break;
            }

            ImageFunc func = coll[i].transform.parent.GetComponent<ImageFunc>();

            if (func == null)  continue;

            if (dragToItem != null) dragToItem.goBack(dragToItem.view);

            dragToItem = func;
            dragToItem.GoToWorldPos(dragToItem.view, viewDragActivePos);
            func.transform.SetSiblingIndex(50);
            return;
        }

        if (hasOldFunc == false)
        {
            if (dragToItem != null)
            {
                dragToItem.goBack(dragToItem.view);
                dragToItem = null;
            }   
        }
    }

    private void GoToWorldPos(RectTransform rectTran, Vector2 pos)
    {
        MyTweenRectPosition mytween = rectTran.GetComponent<MyTweenRectPosition>();
        if (mytween == null) mytween = rectTran.gameObject.AddComponent<MyTweenRectPosition>();
        mytween.from = rectTran.position;
        mytween.to = pos;
        mytween.ResetToBeginning();
        mytween.worldSpace = true;
        mytween.style = Style.Once;
        mytween.moveType = MoveType.EaseInOut;
        mytween.duration = 0.5f;
        mytween.PlayForward();
        //停掉当前动画
        //动画 从当前位置 去到指定位置
    }

    private void goBack(RectTransform rectTran)
    {
        MyTweenRectPosition mytween = rectTran.GetComponent<MyTweenRectPosition>();
        if (mytween == null) mytween = rectTran.gameObject.AddComponent<MyTweenRectPosition>();
        mytween.from = rectTran.anchoredPosition3D;
        mytween.to = Vector2.zero;
        mytween.ResetToBeginning();
        mytween.worldSpace = false;
        mytween.style = Style.Once;
        mytween.moveType = MoveType.EaseInOut;
        mytween.duration = 0.5f;
        mytween.PlayForward();
        //停掉当前动画
        ///开始动画 从当前位置 返回到 本地坐标初始位置
    }

    private void GoToPos(RectTransform rectTran, Vector2 pos)
    {
        MyTweenRectPosition mytween = rectTran.GetComponent<MyTweenRectPosition>();
        if (mytween == null) mytween = rectTran.gameObject.AddComponent<MyTweenRectPosition>();
        mytween.from = rectTran.anchoredPosition3D;
        mytween.to = pos;
        mytween.ResetToBeginning();
        mytween.worldSpace = false;
        mytween.duration = 0.5f;
        mytween.PlayForward();
        //停掉当前动画
        ///开始动画 从当前位置 返回到 本地坐标初始位置
    }

    public Vector2 GetPos(Vector2 v2)
    {
        v2.x = v2.x * 2048f / Screen.width;
        v2.y = v2.y * 1536f / Screen.height;
        return v2;
    }

}
