using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UScrollList : MonoBehaviour {
    /// <summary>
    /// 所有不使用的皮肤的存放处
    /// </summary>
    public Stack<GameObject> skinList = new Stack<GameObject>();
    /// <summary>
    /// 所有展示后的物品存放
    /// </summary>
    public Dictionary<int, ItemFunc> itemDic = new Dictionary<int, ItemFunc>();
    private MyScrollRect scroRect;
    private RectTransform contenttrans;
    internal bool itemActive;
    private Content content;
    private GameObject itemSkin;
    private ItemData[] msgs;
    private int LoadCount = 0;//最好大于三屏
    private int refreshDis = 6;//最好一屏左右 并且小于LoadCount/2；
    private int currentIndex = 0;
    private int oldRefreshIndex = 0;
    private RectTransform contentRect;
    public UScrollList()
    {
        ////
    }

    public void Awake()
    {
        scroRect = transform.gameObject.AddComponent<MyScrollRect>();
        scroRect.horizontal = false;
        scroRect.OnScrollDraged = OnScrollDraged;
        RectTransform baseRect = scroRect.GetComponent<RectTransform>();
        GameObject viewport = new GameObject("viewport");
        RectTransform viewtrans = viewport.AddComponent<RectTransform>();
        viewtrans.SetParent(transform,false);
        viewtrans.pivot = Vector2.up;
        viewtrans.anchorMin = Vector2.zero;
        viewtrans.anchorMax = Vector2.one;
        viewtrans.anchoredPosition3D = Vector3.zero;
        viewtrans.sizeDelta = Vector2.zero;
        viewport.AddComponent<CanvasRenderer>();
        viewport.AddComponent<Image>().raycastTarget = false;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        contenttrans = new GameObject("content").AddComponent<RectTransform>();
        contenttrans.SetParent(viewtrans,false);
        contenttrans.pivot = Vector2.up;
        contenttrans.anchorMin = Vector2.up;
        contenttrans.anchorMax = Vector2.up;
        contenttrans.anchoredPosition3D = Vector3.zero;
        contenttrans.sizeDelta = Vector2.zero;
        content = contenttrans.gameObject.AddComponent<Content>();
        content.scrollRect = scroRect;
        contentRect = content.contentTrans;
        scroRect.content = contentRect;

        // 设置布局
        SetData(420, 672, 30, 70, 6, 2, 20, refreshDis, content.itemPrefab);

        // 测试  数据
        ItemData[] data = new ItemData[20];

        Display(data);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="ItemH"></param>
    /// <param name="ItemW"></param>
    /// <param name="HorizontalDis"></param>
    /// <param name="VerticalDis"></param>
    /// <param name="ShowCount">prefab克隆的数量</param>
    /// <param name="maxPerLine">每行最大显示数量</param>
    /// <param name="LoadCount">预加载数量</param>
    /// <param name="itemSkin"></param>
    public void SetData(int ItemH, int ItemW, int HorizontalDis, int VerticalDis, int ShowCount,int maxPerLine, int LoadCount, int refreshDis, GameObject itemSkin)
    {
        content.cellHeigth = ItemH;
        content.cellWidth = ItemW;
        content.cellHeigthSpace = VerticalDis;
        content.cellWithSpace = HorizontalDis;
        content.ViewCount = ShowCount;
        this.itemSkin = itemSkin;
        this.LoadCount = LoadCount;
        this.refreshDis = refreshDis;
        content.maxPerLine = maxPerLine;
    }

    public Vector2 GetItemPos(int index)
    {
        return content.GetLocalPositionByIndex(index);
    }

    public void Display(ItemData[] msgs)
    {
        RefreshDisplay(msgs, true);
        //if(skinList.count>0)可以清除多余的skinList
    }

    public void RefreshDisplay(ItemData[] msgs = null, bool resetPos = false)
    {
        foreach (ItemFunc item in itemDic.Values)
        {
            skinList.Push(item.gameObject);
        }
        itemDic.Clear();

        if (resetPos == true) ResetPosition();
        if (msgs != null)
        {
            ResetHeight(msgs.Length);
            this.msgs = msgs;
        }

        for (int i = 0; i < this.msgs.Length; i++)
        {
            if (i < currentIndex - 6 || i > currentIndex + 20)
            {
                continue;
            }
            GameObject cloneSkin = GetInstance();
            UDragScroll uDrag = cloneSkin.GetComponent<UDragScroll>();
            ItemFunc func = cloneSkin.GetComponent<ItemFunc>();
            cloneSkin.transform.localPosition = content.GetLocalPositionByIndex(i);
            uDrag.scroRect = scroRect;
            func.OnChange = ItemIndexChange;
            func.scroRect = scroRect;
            func.data = this.msgs[i];
            func.index = i;
            itemDic.Add(i, func);
            cloneSkin.transform.SetParent(contentRect, false);
            cloneSkin.name = "" + func.index;
            cloneSkin.GetComponent<RectTransform>().SetSiblingIndex(i);
        }
    }
    private GameObject GetInstance()
    {
        if (skinList.Count > 0)
        {
            return skinList.Pop();
        }
        GameObject cloneSkin = GameObject.Instantiate(itemSkin);
        UDragScroll uDrag = cloneSkin.AddComponent<UDragScroll>();
        ItemFunc func = cloneSkin.AddComponent<ItemFunc>();

        GameObject itemview = cloneSkin.transform.FindChild("view").gameObject;
        GameObject itemOffer = cloneSkin.transform.FindChild("view/Offer").gameObject;
        UDragScroll viewScroll= itemview.AddComponent<UDragScroll>();
        viewScroll.scroRect = scroRect;
        UDragScroll offerScroll = itemOffer.AddComponent<UDragScroll>();
        offerScroll.scroRect = scroRect; 
        cloneSkin.SetActive(true);
        return cloneSkin;
    }

    public void ItemIndexChange(int from, int to)
    {
        if (itemDic.ContainsKey(from) == false || itemDic.ContainsKey(to) == false)
        {
            Debug.LogWarning("ItemIndexChange Wrong ：index " + from + " or " + to + " 不存在！");
            return;
        }
        ItemFunc funcFrom = itemDic[from];
        ItemFunc funcTo = itemDic[to];
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

    private void ResetPosition()
    {
        contentRect.anchoredPosition3D = Vector3.zero;
        currentIndex = 0;
    }

    private void ResetHeight(int length)
    {
        content.SetDataCount(length);
        //scroRect.onValueChanged.AddListener(content.MyValueChanged);
    }

    private void OnScrollDraged()
    {
        float value = contentRect.anchoredPosition3D.y / (content.cellHeigth + content.cellHeigthSpace);
        int index = 2 * (int)value;
        if (currentIndex != index)
        {
            currentIndex = index;
            if (currentIndex - oldRefreshIndex >= refreshDis || currentIndex - oldRefreshIndex <= -refreshDis)
            {
                oldRefreshIndex = currentIndex;
                LoadAndUnload();
            }
        }
    }

    private void LoadAndUnload()
    {
        //currentIndex
        Debug.Log("刷新");
    }

}
