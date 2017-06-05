using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class MessageBox : Singleton<MessageBox>
{
    private MainPage mainpage
    {
        get {
            return MainPage.Instance;
        }
    }
    private GameObject skin;
    private RectTransform skinRectTrans;

    private Color32 panelImageBgColor = new Color32(239, 239, 239, 255);
    private Color32 titleTextColor = new Color32(89, 87, 87, 255);
    private Color32 buttonTextColor = new Color32(255, 255, 255, 255);

    private Text title;
    private Text message;
    private Button ok;
    private Button cancel;
    private Button close;
    private Text okText;
    private Text cancelText;


    private float panelSizeDelta_X = 600;
    private float panelSizeDelta_Y = 460;
    private float textWidth = 400;
    private float textHeight = 80;
    private float myButtonSize_X = 160;
    private float myButtonSize_Y = 68;
    private float offset_X = 40;
    private float offset_Y = 40;

    private Vector2 cellSize = new Vector2(160, 80);
    private Vector2 spacing = new Vector2(160, 0);

    private Action okDele;
    private Action cancelDele;
    private Action closeDele;

    public void ShowOkCancelClose(string title = null, string message = null,
        Action CancelAction = null, Action OkAction = null, Action CloseAction = null,
        string Cancel = "取消", string Ok = "确定"
        )
    {
        if (skin == null)
        {
            CreatSkin();
        }
        ok.gameObject.SetActive(true);
        cancel.gameObject.SetActive(true);
        close.gameObject.SetActive(true);

        cancelDele = CancelAction;
        okDele = OkAction;
        closeDele = CloseAction;

        this.title.text = title;
        this.message.text = message;
        this.okText.text = Ok;
        this.cancelText.text = Cancel;
        this.skin.SetActive(true);
    }

    public void ShowOkCancel(string title = null, string message = null,
        Action CancelAction = null, Action OkAction = null,
        string Cancel = "取消", string Ok = "确定")
    {
        if (skin == null)
        {
            CreatSkin();
        }
        ok.gameObject.SetActive(true);
        cancel.gameObject.SetActive(true);
        close.gameObject.SetActive(false);

        cancelDele = CancelAction;
        okDele = OkAction;
        closeDele = null;

        this.title.text = title;
        this.message.text = message;
        this.okText.text = Ok;
        this.cancelText.text = Cancel;
        this.skin.SetActive(true);
    }
    public void ShowOkClose(string title = null, string message = null, 
        Action OkAction = null, Action CloseAction = null,
        string Ok = null)
    {
        if (skin == null)
        {
            CreatSkin();
        }
        ok.gameObject.SetActive(true);
        cancel.gameObject.SetActive(false);
        close.gameObject.SetActive(true);
        
        cancelDele = null;
        okDele = OkAction;
        closeDele = CloseAction;

        this.title.text = title;
        this.message.text = message;
        this.okText.text = Ok;
        this.skin.SetActive(true);
    }
    public void ShowOk(string title = null, string message = null, 
        Action OkAction = null,
        string Ok = null)
    {
        if (skin == null)
        {
            CreatSkin();
        }
        ok.gameObject.SetActive(true);
        cancel.gameObject.SetActive(false);
        close.gameObject.SetActive(false);

        cancelDele = null;
        okDele = OkAction;
        closeDele = null;

        this.title.text = title;
        this.message.text = message;
        this.okText.text = Ok;
        this.skin.SetActive(true);
    }

    private void CreatSkin()
    {
        skin = new GameObject("MessageBox", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        skinRectTrans = skin.GetComponent<RectTransform>();
        Image panelImage = skin.GetComponent<Image>();
        //panelImage.sprite = Resources.Load<Sprite>("ButtonBG");
        panelImage.color = panelImageBgColor;
        skinRectTrans.SetParent(mainpage.skinRectTrans);
        skinRectTrans.anchoredPosition3D = Vector2.zero;
        skinRectTrans.sizeDelta = Vector2.right * panelSizeDelta_X + Vector2.up * panelSizeDelta_Y;
        skinRectTrans.localScale = Vector3.one;

        title = CreateText("title", skinRectTrans, titleTextColor, Vector2.right * textWidth + Vector2.up * textHeight);
        message = CreateText("message", skinRectTrans, titleTextColor, Vector2.right * textWidth + Vector2.up * textHeight);

        ok = CreateButton("ok", Resources.Load<Sprite>("ButtonBG"), 
            Vector2.right * myButtonSize_X + Vector2.up * myButtonSize_Y,
            out okText);
        okText.text = "确认";
        cancel = CreateButton("cancel", Resources.Load<Sprite>("ButtonBG"),
            Vector2.right * myButtonSize_X + Vector2.up * myButtonSize_Y, 
            out cancelText);
        cancelText.text = "取消";
        
        close = CreateButton("close", Resources.Load<Sprite>("Exit"),
            Vector2.right * offset_X + Vector2.up * offset_Y
            );

        GameObject layout = new GameObject("Layout", typeof(RectTransform), typeof(GridLayoutGroup));
        RectTransform buttonLayout = layout.GetComponent<RectTransform>();
        buttonLayout.SetParent(skinRectTrans);
        buttonLayout.anchorMin = Vector2.zero;
        buttonLayout.anchorMax = Vector2.one;
        buttonLayout.sizeDelta = Vector2.one * -60;
        buttonLayout.localScale = Vector3.one;
        buttonLayout.anchoredPosition3D = Vector3.zero;
        GridLayoutGroup group = layout.GetComponent<GridLayoutGroup>();
        group.cellSize = cellSize;
        group.spacing = spacing;
        group.startAxis = GridLayoutGroup.Axis.Horizontal;
        group.childAlignment = TextAnchor.LowerCenter;
        cancel.transform.SetParent(buttonLayout);
        cancel.transform.localScale = Vector3.one;
        cancel.transform.localPosition = Vector3.zero;
        ok.transform.SetParent(buttonLayout);
        ok.transform.localScale = Vector3.one;
        ok.transform.localPosition = Vector3.zero;


        RectTransform closeRectTrans = close.GetComponent<RectTransform>();
        SetAnchor(closeRectTrans, RectAnchor.UpperRight);

        ok.onClick.AddListener(OnClickOk);
        cancel.onClick.AddListener(OnClickCancel);
        close.onClick.AddListener(OnClickClose);

        SetAnchor(title, RectAnchor.UpperCenter);
        SetAnchor(message, RectAnchor.MiddleCenter);
    }

    private void OnClickOk()
    {
        Close();
        if (okDele != null) okDele();
    }
    private void OnClickCancel()
    {
        Close();
        if (cancelDele != null) cancelDele();
    }
    private void OnClickClose()
    {
        Close();
        if (closeDele != null) closeDele();
    }

    public Button CreateButton(string name, Sprite image, Vector2 sizeDelta, out Text text)
    {
        Button button = CreateButton(name, image, sizeDelta);
        RectTransform buttonRectTrans = button.GetComponent<RectTransform>();
        text = CreateText("Text", buttonRectTrans, buttonTextColor, Vector2.zero);
        return button;
    }
    public Button CreateButton(string name, Sprite sprite, Vector2 sizeDelta)
    {
        GameObject goButton = new GameObject(name, typeof(Image), typeof(Button));
        RectTransform buttonRectTrans = goButton.GetComponent<RectTransform>();
        buttonRectTrans.SetParent(skinRectTrans);
        Image image = goButton.GetComponent<Image>();
        image.sprite = sprite;
        Button button = goButton.GetComponent<Button>();
        buttonRectTrans.anchorMin = Vector2.zero;
        buttonRectTrans.anchorMax = Vector2.zero;
        buttonRectTrans.sizeDelta = sizeDelta;
        return button;
    }
    public Button CreateButton(string name, Sprite sprite)
    {
        GameObject goButton = new GameObject(name, typeof(Image), typeof(Button));
        RectTransform buttonRectTrans = goButton.GetComponent<RectTransform>();
        buttonRectTrans.SetParent(skinRectTrans);
        Image image = goButton.GetComponent<Image>();
        image.sprite = sprite;
        Button button = goButton.GetComponent<Button>();
        buttonRectTrans.anchorMin = Vector2.zero;
        buttonRectTrans.anchorMax = Vector2.zero;
        image.SetNativeSize();
        return button;
    }

    public Text CreateText(string name, RectTransform parentRect, Color32 color, Vector2 sizeData)
    {
        GameObject goText = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        RectTransform textRectTrans = goText.GetComponent<RectTransform>();
        Text text = goText.GetComponent<Text>();
        text.font = Resources.Load<Font>("simhei");
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 30;
        textRectTrans.SetParent(parentRect);
        textRectTrans.anchorMin = Vector2.zero;
        textRectTrans.anchorMax = Vector2.one;
        textRectTrans.sizeDelta = sizeData;
        textRectTrans.localScale = Vector3.one;
        //textRectTrans.anchoredPosition3D = Vector3.zero;
        return goText.GetComponent<Text>();
    }

    public void SetAnchor(Graphic graphic, RectAnchor anchor, float posX = 0, float posY = 0)
    {
        SetAnchor(graphic.GetComponent<RectTransform>(), anchor, posX, posY);
    }
    public void SetAnchor(RectTransform rectTran, RectAnchor anchor, float posX = 0,float posY =0)
    {
        switch (anchor)
        {
            case RectAnchor.UpperLeft:
                rectTran.pivot = Vector2.up;
                rectTran.anchorMin = Vector2.up;
                rectTran.anchorMax = Vector2.up;
                rectTran.anchoredPosition3D = Vector3.zero;
                rectTran.localScale = Vector3.one;
                break;
            case RectAnchor.UpperCenter:
                rectTran.pivot = Vector2.up+Vector2.right*0.5f;
                rectTran.anchorMin = Vector2.up + Vector2.right * 0.5f;
                rectTran.anchorMax = Vector2.up + Vector2.right * 0.5f;
                rectTran.anchoredPosition3D = Vector3.right * posX + Vector3.up * posY +Vector3.zero;
                rectTran.localScale = Vector3.one;

                break;
            case RectAnchor.UpperRight:
                rectTran.pivot = Vector2.one;
                rectTran.anchorMin = Vector2.up  + Vector2.right ;
                rectTran.anchorMax = Vector2.up  + Vector2.right ;
                rectTran.anchoredPosition3D = Vector3.right * posX + Vector3.up * posY + Vector3.zero;
                rectTran.localScale = Vector3.one;

                break;
            case RectAnchor.MiddleLeft:
                rectTran.pivot = Vector2.up * 0.5f;
                rectTran.anchorMin = Vector2.up * .5f ;
                rectTran.anchorMax = Vector2.up * .5f ;
                rectTran.anchoredPosition3D = Vector3.right * posX + Vector3.up * posY + Vector3.zero;
                rectTran.localScale = Vector3.one;

                break;
            case RectAnchor.MiddleCenter:
                rectTran.pivot = Vector2.one*.5f;
                rectTran.anchorMin = Vector2.up * 0.5f + Vector2.right * 0.5f;
                rectTran.anchorMax = Vector2.up * 0.5f + Vector2.right * 0.5f;
                rectTran.anchoredPosition3D = Vector3.right * posX + Vector3.up * posY + Vector3.zero;
                rectTran.localScale = Vector3.one;

                break;
            case RectAnchor.MiddleRight:
                rectTran.pivot = Vector2.up * .5f + Vector2.right ;
                rectTran.anchorMin = Vector2.up * .5f + Vector2.right ;
                rectTran.anchorMax = Vector2.up * .5f + Vector2.right ;
                rectTran.anchoredPosition3D = Vector3.right * posX + Vector3.up * posY + Vector3.zero;
                rectTran.localScale = Vector3.one;

                break;
            case RectAnchor.LowerLeft:
                rectTran.pivot = Vector2.zero;
                rectTran.anchorMin = Vector2.zero;
                rectTran.anchorMax = Vector2.zero;
                rectTran.anchoredPosition3D = Vector3.right * posX + Vector3.up * posY + Vector3.zero;
                rectTran.localScale = Vector3.one;

                break;
            case RectAnchor.LowerCenter:
                rectTran.pivot = Vector2.right*.5f;
                rectTran.anchorMin = Vector2.right* .5f;
                rectTran.anchorMax = Vector2.right * .5f;
                rectTran.anchoredPosition3D = Vector3.right * posX + Vector3.up * posY + Vector3.zero ;
                rectTran.localScale = Vector3.one;

                break;
            case RectAnchor.LowerRight:
                rectTran.pivot = Vector2.right;
                rectTran.anchorMin = Vector2.right ;
                rectTran.anchorMax = Vector2.right ;
                rectTran.anchoredPosition3D = Vector3.right * posX + Vector3.up * posY + Vector3.zero;
                rectTran.localScale = Vector3.one;

                break;
            default:
                break;
        }
    }
    
    public virtual void Close()
    {
        if (skin != null) skin.SetActive(false);
    }
   
}

public enum RectAnchor
{
    UpperLeft = 0,

    UpperCenter = 1,
    
    UpperRight = 2,
   
    MiddleLeft = 3,
    
    MiddleCenter = 4,
   
    MiddleRight = 5,
  
    LowerLeft = 6,
    
    LowerCenter = 7,
   
    LowerRight = 8
}