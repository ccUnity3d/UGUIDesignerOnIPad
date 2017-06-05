using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GenerateOfferItemFunc : UGUIItemFunction, IBeginDragHandler, IEndDragHandler
{
    private MainPage mainPage
    {
        get {
            return MainPage.Instance;
        }
    }

    public Product produce;
    // 产品图片
    public RawImage productImage;
    // 产品名字
    public Text productName;
    // 品牌
    public Text brand;
    // 产品编号
    public Text productNo;
    // 单价
    public Text unitPrice;
    // 数量
    public Text editorNumber;
    // 是否收藏
    public Toggle selectToggle;
    // 总价格
    public Text totalText;

    public bool isLeftMove;

    public bool isEditor;

    public MyTweenRectPosition myTween;

    public RectTransform itemRect;

    public Action<GenerateOfferItemFunc> deleteFun;

    public Action<GenerateOfferItemFunc> selectDeleteFunc;

    public Action<GenerateOfferItemFunc> deleteTween;

    public Action<GenerateOfferItemFunc> cancelDeleteFunc;

    private Button add;

    private Button reduce;

    private Button itemDelete;


    private ItemPrice itemData
    {
        get { return data as ItemPrice; }

    }

    private MyScrollRect scrollRect;
    protected override void Awake()
    {
        itemRect = this.GetComponent<RectTransform>();
        DrapButton button = this.gameObject.AddComponent<DrapButton>();
        button.onDragDele = onDrag;
        productImage = UITool.GetUIComponent<RawImage>(this.transform,"RawImage");
        productName = UITool.GetUIComponent<Text>(this.transform,"name");
        brand = UITool.GetUIComponent<Text>(this.transform,"brand");
        unitPrice = UITool.GetUIComponent<Text>(this.transform, "unitPrice");
        productNo = UITool.GetUIComponent<Text>(this.transform,"no");
        add = UITool.GetUIComponent<Button>(this.transform, "number/Editor/addBtn");
        add.onClick.AddListener(onAdd);
        reduce = UITool.GetUIComponent<Button>(this.transform, "number/Editor/reduceBtn");
        reduce.onClick.AddListener(onReduce);
        itemDelete = UITool.GetUIComponent<Button>(this.transform, "delete");
        itemDelete.onClick.AddListener(onItemDelete);
        editorNumber = UITool.GetUIComponent<Text>(this.transform, "number/Editor/numberText/number");
        selectToggle = UITool.GetUIComponent<Toggle>(this.transform, "total");
        selectToggle.onValueChanged.AddListener(onSelectToggle);
        totalText = UITool.GetUIComponent<Text>(this.transform, "total/Label");
    }
    private void onItemDelete()
    {
        if (deleteFun != null) deleteFun(this);
    }
    public override void Render()
    {
        base.Render();
        productImage.texture = mainPage.Logo;
        SetTextureTool.SetTexture(productImage, produce.tempTexture, "notLocal");

        productName.text = produce.name;
        brand.text = produce.vendor;
        productNo.text = produce.seekId;
        unitPrice.text = "";
        editorNumber.text = itemData.count.ToString();
        totalText.text = "";

        selectToggle.isOn = itemData.isSelect;

        //if (selectDeleteFunc != null) selectDeleteFunc(this);
        //itemData.isCollect = true;
        //selectToggle.isOn = isEditor;
    }
    void Start()
    {
        scrollRect = scroRect.GetComponent<MyScrollRect>();
    }
    private void onAdd()
    {
        Opertion(1);
    }
    private void onReduce()
    {
        Opertion(-1);
    }
    private void onSelectToggle(bool isTrue)
    {
        itemData.isSelect = isTrue;
        if (isTrue == true )
        {
            //itemData.isCollect = true;
            if (selectDeleteFunc != null) selectDeleteFunc(this);
            Debug.Log("显示删除"+index);
            return;
        }
        if (cancelDeleteFunc != null) cancelDeleteFunc(this);
        //itemData.isCollect = false;
        Debug.Log(isTrue);
    }
    private void Opertion(int num)
    {
        int temp = int.Parse(editorNumber.text);
        temp += num;
        if (temp <= 0)
        {
            editorNumber.text = 0.ToString();
            //itemData.isCollect = false;
            selectToggle.isOn = false;
            return;
        }
        editorNumber.text = temp.ToString();
        itemData.count = temp;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            scrollRect.vertical = false;
            return;
        }
        scrollRect.vertical = true;
    }
    private void onDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y) && isLeftMove)
        {
            myTween = this.GetComponent<MyTweenRectPosition>();
            if (myTween == null) myTween = this.gameObject.AddComponent<MyTweenRectPosition>();
            myTween.from = Vector2.up * itemRect.anchoredPosition.y;
            myTween.to = new Vector2(-280, itemRect.anchoredPosition.y);
            myTween.duration = 0.25f;
            if (eventData.delta.x > 0)
            {
                myTween.SetStartToCurrentValue();
                myTween.to = Vector2.up * itemRect.anchoredPosition.y;
                myTween.PlayForward();
                if (cancelDeleteFunc != null) cancelDeleteFunc(this);
                return;
            }
            if (deleteTween != null) deleteTween(this);
            myTween.SetStartToCurrentValue();
            myTween.to = new Vector2(-280, itemRect.anchoredPosition.y);
            myTween.PlayForward();
            //if (selectDeleteFunc != null) selectDeleteFunc(this);
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y))
        {
            scrollRect.vertical = false;
            return;
        }
        scrollRect.vertical = true;
    }
    
}
