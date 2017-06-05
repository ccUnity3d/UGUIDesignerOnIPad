using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SchemeEffectImageItemFunc : UGUIItemFunction, IBeginDragHandler, IEndDragHandler
{
    private EffectImageItem itemData
    {
        get { return data as EffectImageItem; }
    }
    private MainPage mainPage
    {
        get {
            return MainPage.Instance;
        }
    }

    public RawImage effectImage;
    public Text effectName;
    public Button SetMainEffectImage;
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
    private SchemeEffectImageItemFunc dragToItem;
    // 碰撞到物体的数组
    private Collider2D[] coll;
    // 碰撞物体数量
    private int count;
    // 激活拖拽的位置
    private Vector2 viewDragActivePos;
    private SchemeEffectImageScroll schemeEffectImageScroll;

    private Text loadingProTime;
    private Image loadingProgressImage;

    private Texture2D texture;
    private DateTime startTime;
    private const int waitMinute
#if UNITY_IPHONE
        = 4;
#else
        = 1;
#endif
    private int startTimeTotalSecond;
    private bool clickable = false;


    protected override void Awake()
    {
        viewRect = UITool.GetUIComponent<RectTransform>(this.transform, "view");
        effectImage = UITool.GetUIComponent<RawImage>(this.transform, "GameObject/view");
        DrapButton effectBtn = effectImage.gameObject.AddComponent<DrapButton>();
        effectBtn.onPointerDownDele = onPointerDown;
        effectBtn.onPointerUpDele = onPointerUp;
        effectBtn.onDragDele = onDrag;
        effectBtn.onPointerClickDele = onClick;
        tranRect = this.GetComponent<RectTransform>();
        tranRect.gameObject.SetActive(true);
        coll = new Collider2D[8];
        effectName = UITool.GetUIComponent<Text>(this.transform, "Text");
       // SetMainEffectImage = UITool.GetUIComponent<Button>(this.transform, "view/mainEffectImage");
        loadingProTime = UITool.GetUIComponent<Text>(this.transform,"Text");
        loadingProgressImage = UITool.GetUIComponent<Image>(this.transform, "Loading");
    }
    void Start()
    {
        schemeEffectImageScroll = scroRect.GetComponent<SchemeEffectImageScroll>();

    }

    /// <summary>
    /// Awake后 Start前
    /// </summary>
    public override void Render()
    {
        MyMono.MyStopCoroutine(ShowEndTime, this);
        UITool.SetActive(loadingProgressImage, false);
        UITool.SetActive(loadingProTime, false);
        clickable = false;

        base.Render();
        effectImage.texture = mainPage.Logo;

        //正式
        //SetTextureTool.SetTexture(effectImage, itemData.path, "notLocal");

        //临时
        string[] strs = itemData.path.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries);
        if (strs.Length == 2)
        {
            Debug.Log("临时 strs.Length == 2");
            string path = strs[1];
            string[] strTimes = strs[0].Split('-');
            DateTime nowTime = DateTime.Now;
            startTime = new DateTime(int.Parse(strTimes[0]), int.Parse(strTimes[1]), int.Parse(strTimes[2])
                                   , int.Parse(strTimes[3]), int.Parse(strTimes[4]), int.Parse(strTimes[5]));
            int currentTotalSecond = nowTime.Hour * 3600 + nowTime.Minute * 60 + nowTime.Second;
            startTimeTotalSecond = startTime.Hour * 3600 + startTime.Minute * 60 + startTime.Second;
            if (
                nowTime.Year != startTime.Year
                || nowTime.Month != startTime.Month
                || nowTime.Day != startTime.Day
                || currentTotalSecond - startTimeTotalSecond > waitMinute*60
                )
            {
                LoaderPool.OutterLoad(path, SimpleLoadDataType.texture2D, onLoadedSetTexture, null);
                return;
            }
            
                            
            texture = null;
            LoaderPool.OutterLoad(path, SimpleLoadDataType.texture2D, onLoadedTexture, null);
            MyMono.MyStartCoroutine(ShowEndTime, this);
        }
        else
        {
            Debug.Log("临时 strs.Length == " + strs.Length);
        }

    }

    private void onLoadedSetTexture(object obj)
    {
        SimpleOutterLoader outloader = obj as SimpleOutterLoader;
        if (outloader.state == SimpleLoadedState.Failed)
        {
            return;
        }
        texture = outloader.loadedData as Texture2D;
        OnTimeOver();
    }

    private void onLoadedTexture(object obj)
    {
        SimpleOutterLoader outloader = obj as SimpleOutterLoader;
        if (outloader.state == SimpleLoadedState.Failed)
        {
            return;
        }
        texture = outloader.loadedData as Texture2D;
    }

    private IEnumerator ShowEndTime(params object[] obj)
    {
        DateTime nowTime = DateTime.Now;
        UITool.SetActive(loadingProgressImage, true);
        UITool.SetActive(loadingProTime, true);
        while (true)
        {
            nowTime = DateTime.Now;
            int currentTotalSecond = nowTime.Hour * 3600 + nowTime.Minute * 60 + nowTime.Second;
            if (
                nowTime.Year != startTime.Year
                || nowTime.Month != startTime.Month
                || nowTime.Day != startTime.Day
                || currentTotalSecond - startTimeTotalSecond > waitMinute * 60
                )
            {
                break;
            }
            float persent = (currentTotalSecond - startTimeTotalSecond) / (waitMinute * 60f);
            //显示进度 persent
            loadingProTime.text = string.Format("{0}%", Mathf.RoundToInt(persent * 100));
            loadingProgressImage.fillAmount = persent;

            yield return new WaitForSeconds(1f);
            if (GlobalConfig.running == false)
            {
                break;
            }
        }
        UITool.SetActive(loadingProgressImage, false);
        UITool.SetActive(loadingProTime, false);
        OnTimeOver();
    }

    private void OnTimeOver()
    {
        if (texture != null)
        {
            effectImage.texture = texture;
            clickable = true;
            Vector3 size = Vector3.right * 672 + Vector3.up * 504;
            float multi = (texture.height * 1f / texture.width);
            if (multi > 3f / 4)
            {
                size = Vector2.right * 504 / multi + Vector2.up * 504;
            }
            else
            {
                size = Vector2.right * 672 + Vector2.up * 672 * multi;
            }
            effectImage.rectTransform.sizeDelta = size;
        }
        else
        {
            Debug.LogWarning("加载出错！");
        }
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
    /// <summary>
    /// 当 手指按下 执行
    /// </summary>
    /// <param name="eventData"></param>
    private void onClick()
    {
        if (moved == true) return;
        if (clickable == false) return;
        schemeEffectImageScroll.dispatchEvent(new SchemeEvent(SchemeEvent.openEffectImage));
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
            Vector2 pos = schemeEffectImageScroll.GetLoaclPosByIndex(dragToItem.index);
            GoToPos(tranRect, pos);
            MyCallLater.Add(time, laterChange);
        }
        else
        {
            Vector2 pos = schemeEffectImageScroll.GetLoaclPosByIndex(index);
            GoToPos(tranRect, pos);
        }
        dragActive = false;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        throw new NotImplementedException();
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

        if (count == 0) return;

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

            SchemeEffectImageItemFunc func = coll[i].transform.parent.GetComponent<SchemeEffectImageItemFunc>();

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
}
