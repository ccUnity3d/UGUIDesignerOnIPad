using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class SchemeManifestItemFunc : UGUIItemFunction, IBeginDragHandler, IEndDragHandler
{
    private ManifestOfferItem itemData
    {
        get { return data as ManifestOfferItem; }
    }

    public RawImage manifestImage;
    public Text manifestName;
    public RectTransform SetMainManifest;
    public Action<int, int> OnChange;
    private bool isDown = false;
    private bool dragActive = false;
    private bool startDrag = false;
    private float beginTime = 0.0f;
    private float longPressDelay = 0.5f;
    private bool moved = false;
    private Vector2 itemStartPos = Vector2.zero;
    private Vector2 inputStartPos;
    private Vector2 offset = Vector2.zero;
    // item 的 transfrom
    private RectTransform tranRect;
    // item 孩子 view 的 transform
    private RectTransform viewRect;
    // 当前拖拽的currentDragEffectImage与另一个EffectImage(dragToItems) 交互
    private SchemeManifestItemFunc dragToItem;
    // 碰撞到物体的数组
    private Collider2D[] coll;
    // 碰撞物体数量
    private int count;
    // 激活拖拽的位置
    private Vector2 viewDragActivePos;
    private SchemeManifestScroll schemeManifestScroll;
    protected override void Awake()
    {
    }
    void Start()
    {
        schemeManifestScroll = scroRect.GetComponent<SchemeManifestScroll>();
        if (itemData.special == true)
        {
            Button addManifest = transform.FindChild("view/AddManifest").GetComponent<Button>();
            manifestImage = UITool.GetUIComponent<RawImage>(this.transform, "view");
            manifestImage.enabled = false;
            addManifest.onClick.AddListener(AddManifestClick);
        }
        else
        {
            viewRect = UITool.GetUIComponent<RectTransform>(this.transform, "view");
            manifestImage = UITool.GetUIComponent<RawImage>(this.transform, "view");
            DrapButton effectBtn = manifestImage.gameObject.AddComponent<DrapButton>();
            effectBtn.onPointerDownDele = onPointerDown;
            effectBtn.onPointerUpDele = onPointerUp;
            effectBtn.onDragDele = onDrag;
            effectBtn.onPointerClickDele = onClick;
            tranRect = this.GetComponent<RectTransform>();
            tranRect.gameObject.SetActive(true);
            coll = new Collider2D [8];
            manifestName = UITool.GetUIComponent<Text>(this.transform, "view/Text");
            SetMainManifest = UITool.GetUIComponent<RectTransform>(this.transform, "view/ManifestImage");
        }
    }

    public override void Render()
    {
        base.Render();
        //string path = "" + itemData.priceDataId;
        //LoadOffer(path);进入报价后 PriceData
    }

    protected override void Update()
    {
        if (isDown == false || moved == true || dragActive == true) return;
        if (Time.time - beginTime > longPressDelay)
        {
            scroRect.vertical = false;
            dragActive = true;
            itemStartPos = tranRect.anchoredPosition;
            dragToItem = null;
            viewDragActivePos = viewRect.position;
        }
    }
    private void AddManifestClick()
    {
        schemeManifestScroll.dispatchEvent(new SchemeEvent(SchemeEvent.AddOffer));
    }
    private void onClick()
    {
        if (moved == true) return;
        schemeManifestScroll.ClickItem(itemData, this.gameObject);
    }
    public void onPointerDown(PointerEventData eventData)
    {
        isDown = true;
        moved = false;
        inputStartPos = eventData.position;
        beginTime = Time.time;
    }
    /// <summary>
    /// 当 手指抬起 执行
    /// </summary>
    /// <param name="eventData"></param>
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
            Vector2 pos = schemeManifestScroll.GetLoaclPosByIndex(dragToItem.index);
            GoToPos(tranRect, pos);
            MyCallLater.Add(time, laterChange);
        }
        else
        {
            Vector2 pos = schemeManifestScroll.GetLoaclPosByIndex(index);
            GoToPos(tranRect, pos);
        }
        dragActive = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {

    }
    public void onDrag(PointerEventData eventData)
    {
        if (eventData.delta != Vector2.zero)
        {
            moved = true;
        }
        if (dragActive == true)
        {
            tranRect.SetSiblingIndex(100);
            SetDraggedPosition(eventData);
            return;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
    private void SetDraggedPosition(PointerEventData data)
    {
        if (offset == data.position - inputStartPos)
        {
            return;
        }
        offset = data.position - inputStartPos;
        tranRect.anchoredPosition = itemStartPos + GetPos(offset);

        Vector2 org = new Vector2(tranRect.position.x + 1.8f, tranRect.position.y - 1.3f);

        count = Physics2D.OverlapPointNonAlloc(org, coll);

        if (count == 0 ) return;

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

            SchemeManifestItemFunc func = coll[i].transform.parent.GetComponent<SchemeManifestItemFunc>();

            if (func == null) continue;

            if (dragToItem != null) dragToItem.goBack(dragToItem.viewRect);

            dragToItem = func;
            dragToItem.GoToWorldPos(dragToItem.viewRect, viewDragActivePos);
            func.transform.SetSiblingIndex(50);
            return;
        }

        if (hasOldFunc == false)
        {
            if (dragToItem != null)
            {
                dragToItem.goBack(dragToItem.viewRect);
                dragToItem = null;
            }
        }
    }
    private void laterChange()
    {
        ResetViewPos(tranRect);
        dragToItem.ResetViewPos(dragToItem.viewRect);
        if (OnChange != null) OnChange(this.index, dragToItem.index);
    }
    private void ResetViewPos(RectTransform rect)
    {
        MyTweenRectPosition tween = rect.GetComponent<MyTweenRectPosition>();
        if (tween != null) tween.enabled = false;
        rect.anchoredPosition3D = Vector2.zero;
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
    // Update is called once per frame

}
