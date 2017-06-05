using UnityEngine;
using UnityEngine.UI;

public class SchemePage : UIPage<SchemePage>
{
    public RectTransform Rectscheme;
    #region scheme
    public RectTransform RectshemeNode;
    public InputField nameText;
    //public Text namePlaceholder;
    public InputField desText;
    //public Text desPlaceholder;
    public Button effectBut;
    public Image effectBG;
    public Text effectText;
    public Button detailBut;
    public Image detailBG;
    public Text detailText;
    public Button exitBut;
    public RectTransform ImageScrollView;
    public RectTransform OfferScrollView;
    #endregion

    #region offer
    public RectTransform createOffer;
    public Button createExit;
    public RectTransform createScrollView;
    public Button generaterExit;
    public RectTransform generaterScrollView;
    public RectTransform setMainOffer;
    public Button setMainExit;
    public RectTransform setMainScrollView;
    public Button offerExit;
    public Text setMainOfferTotalPrice;
    public Text offerTitle;
    
    //public Text offerName;
    public RectTransform createOfferTop;
    public RectTransform productPictureTitle;
    public RectTransform productModelTitle;
    public Button offerCreate;
    public Button productManifest;
    public Text productText;
    public Image productBG;
    public Button modelManifest;
    public Text modelText;
    public Image modelBG;
    public Text generateOfferTotalPrice;
    public Text createTotalNumber;
    public Text createOfferTotalPrice;
    public Text genertaTotalNumber;
    public Text schemeDesc;
    public GameObject tab1;
    public Button editor;
    public GameObject tab2;
    public Transform mask;
    public Button offerDelete;
    public Button offerAllSelect;
    public Text offerAllCancel;
    public Button finish;
    public Button offerGenerate;
    /// <summary>
    /// 删除单个产品报价
    /// </summary>
    public Button deletePrice;
    [Tooltip("回收")]
    public RectTransform content;
    public GridLayoutGroup gridLayout;
    public RectTransform offerProducePicture;
    public RectTransform offerProduceScrollView;
    public RectTransform produceLine;
    public RectTransform offerModel;
    public RectTransform offerModelScrollView;
    public RectTransform generateOffer;
    public Text total;
    public Button generateShare;
    public Button setMainGenerate;

    public Button ShareMain;
    public Button deleteOffer;
    public Button setOfferMain;
    #endregion

    #region effectImage
    public RectTransform RectEffect;
    public Button effectExit;
    public RectTransform effectScroll;
    #endregion

    protected override void Ready(Object arg1)
    {
        Rectscheme = skin.GetComponent<RectTransform>();
        #region schemeNode
        RectshemeNode = UITool.GetUIComponent<RectTransform>(Rectscheme, "ShemeNode");
        exitBut = UITool.GetUIComponent<Button>(RectshemeNode, "Exit");
        nameText = UITool.GetUIComponent<InputField>(RectshemeNode, "Top/schemeName/InputField");
        PlaceHolder("请输入方案名……", "Top/schemeName/InputField/Placeholder");
        desText = UITool.GetUIComponent<InputField>(RectshemeNode, "Top/DesText/InputField");
        PlaceHolder("请输入方案详情……", "Top/DesText/InputField/Placeholder");
        effectBut = UITool.GetUIComponent<Button>(RectshemeNode, "Middle/EffectButton");
        effectBG = UITool.GetUIComponent<Image>(effectBut.transform, "background");
        effectText = UITool.GetUIComponent<Text>(effectBut.transform, "name");
        detailBut = UITool.GetUIComponent<Button>(RectshemeNode, "Middle/DetailedButton");
        detailBG = UITool.GetUIComponent<Image>(detailBut.transform, "background");
        detailText = UITool.GetUIComponent<Text>(detailBut.transform, "name");
        ImageScrollView = UITool.GetUIComponent<RectTransform>(RectshemeNode, "Middle/ImageScrollView");
        OfferScrollView = UITool.GetUIComponent<RectTransform>(RectshemeNode, "Middle/OfferScrollView");
        #endregion

        #region CreateOffer
        createOffer = UITool.GetUIComponent<RectTransform>(Rectscheme, "CreateOffer");
        createExit = UITool.GetUIComponent<Button>(createOffer, "Exit");
        createScrollView = UITool.GetUIComponent<RectTransform>(createOffer, "Content/OfferProduce/ScrollView");
        productPictureTitle = UITool.GetUIComponent<RectTransform>(createOffer, "Content/OfferProduce/ProductPictureTitle");
        offerModel = UITool.GetUIComponent<RectTransform>(createOffer, "Content/OfferModel");
        productModelTitle = UITool.GetUIComponent<RectTransform>(createOffer, "Content/OfferModel/ProductModelTitle");
        offerModelScrollView = UITool.GetUIComponent<RectTransform>(createOffer, "Content/OfferModel/offerModelScrollView");
        offerCreate = UITool.GetUIComponent<Button>(createOffer, "Title/CreateOfferTop/CreateOfferBtn");
        productManifest = UITool.GetUIComponent<Button>(createOffer, "Title/CreateOfferTop/ProductManifest");
        productText = UITool.GetUIComponent<Text>(productManifest.transform, "Text");
        productBG = UITool.GetUIComponent<Image>(productManifest.transform, "BackGround");
        offerProducePicture = UITool.GetUIComponent<RectTransform>(createOffer, "Content/OfferProduce");
        offerProduceScrollView = UITool.GetUIComponent<RectTransform>(offerProducePicture, "ScrollView");
        produceLine = UITool.GetUIComponent<RectTransform>(offerProducePicture, "produceFrame");
        modelManifest = UITool.GetUIComponent<Button>(createOffer, "Title/CreateOfferTop/ModelManifest");
        modelText = UITool.GetUIComponent<Text>(modelManifest.transform, "Text");
        modelBG = UITool.GetUIComponent<Image>(modelManifest.transform, "BackGround");
        createTotalNumber = UITool.GetUIComponent<Text>(createOffer, "Title/CreateOfferTop/countNumber");
        createOfferTotalPrice = UITool.GetUIComponent<Text>(createOffer,"Title/CommonTitle/totalValue");
        Debug.Log("createOfferTotalPrice :"+createOfferTotalPrice.text);
        #endregion

        #region GenerateOffer
        generateOffer = UITool.GetUIComponent<RectTransform>(Rectscheme, "GenerateOffer");
        generaterExit = UITool.GetUIComponent<Button>(generateOffer, "Exit");
        generaterScrollView = UITool.GetUIComponent<RectTransform>(generateOffer, "GenerateScrollView");
        mask = UITool.GetUIComponent<Transform>(generaterScrollView, "Mask");
        tab2 = UITool.GetUIComponent<RectTransform>(generateOffer, "Title/CreateOfferTop/Tab2").gameObject;
        tab1 = UITool.GetUIComponent<RectTransform>(generateOffer, "Title/CreateOfferTop/Tab1").gameObject;
        editor = UITool.GetUIComponent<Button>(tab1.transform, "Editor");
        finish = UITool.GetUIComponent<Button>(tab2.transform, "Finish");
        offerAllSelect = UITool.GetUIComponent<Button>(tab2.transform, "AllSelectBtn");
        offerAllCancel = UITool.GetUIComponent<Text>(offerAllSelect.transform, "SelectName");
        offerGenerate = UITool.GetUIComponent<Button>(Rectscheme, "GenerateOffer/GenerateOffer");
        deletePrice = UITool.GetUIComponent<Button>(generateOffer, "Delete");
        total = UITool.GetUIComponent<Text>(generateOffer, "Title/CommonTitle/totalValue");
        genertaTotalNumber = UITool.GetUIComponent<Text>(generateOffer, "Title/CreateOfferTop/countNumber");
        createOfferTop = UITool.GetUIComponent<RectTransform>(generateOffer, "Title/CreateOfferTop");
        generateOfferTotalPrice = UITool.GetUIComponent<Text>(generateOffer, "Title/CommonTitle/totalValue");
        Debug.Log("generateOfferTotalPrice : "+ generateOfferTotalPrice.text);
        #endregion

        #region SetMainOffer
        setMainOffer = UITool.GetUIComponent<RectTransform>(Rectscheme, "SetMainOffer");
        setMainExit = UITool.GetUIComponent<Button>(setMainOffer, "Exit");
        content = UITool.GetUIComponent<RectTransform>(setMainOffer, "Content");
        gridLayout = content.GetComponent<GridLayoutGroup>();
        setMainScrollView = UITool.GetUIComponent<RectTransform>(setMainOffer, "Content/Produce/ScrollView");
        total = UITool.GetUIComponent<Text>(setMainOffer, "Title/CommonTitle/total");
        ShareMain = UITool.GetUIComponent<Button>(setMainOffer, "Content/SetOffer/share");
        deleteOffer = UITool.GetUIComponent<Button>(setMainOffer, "Content/SetOffer/deleteOffer");
        setOfferMain = UITool.GetUIComponent<Button>(setMainOffer, "Content/SetOffer/setOfferMain");
        offerExit = UITool.GetUIComponent<Button>(setMainOffer, "Exit");
        offerTitle = UITool.GetUIComponent<Text>(setMainOffer, "Title/CommonTitle/title");
        setMainOfferTotalPrice = UITool.GetUIComponent<Text>(setMainOffer, "Title/CommonTitle/total");
        Debug.Log("setMainOfferTotalPrice :"+ setMainOfferTotalPrice.text);
        #endregion
       
        #region EfffectImage
        RectEffect = UITool.GetUIComponent<RectTransform>(Rectscheme, "EfffectImage");
        effectExit = UITool.GetUIComponent<Button>(RectEffect, "Exit");
        effectScroll = UITool.GetUIComponent<RectTransform>(RectEffect, "SetMainPicture");
        #endregion
    }


    private void PlaceHolder(string words, string uri)
    {
        Text placeholder = UITool.GetUIComponent<Text>(RectshemeNode, uri);
        placeholder.text = words;
        Color color = placeholder.color;
        color.a = 0.5f;
        placeholder.color = color;
    }
}
