using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;



public class SchemePageController : UIController<SchemePageController>
{

    public SchemePage scheme;
    public RectTransform currentSchemeChildPage = null;
    public Stack<RectTransform> childPageStack = new Stack<RectTransform>();

    //private CacheSchemeManager cacheScheme { get { return CacheSchemeManager.Instance; } }

    private Color startColor;
    private Color currColor;

    private JsonCacheManager jsonCacheManager
    {
        get
        {
            return JsonCacheManager.Instance;
        }
    }
    protected UndoHelper undoHelper
    {
        get
        {
            return UndoHelper.Instance;
        }
    }
    /// <summary>
    /// 所有效果图
    /// </summary>
    public List<ItemData> effectImageList
    {
        get
        {
            List<ItemData> list = new List<ItemData>();
            for (int i = 0; i < schemeManifest.images.Count; i++)
            {
                EffectImageItem imageData = new EffectImageItem(schemeManifest.images[i]);
                list.Add(imageData);
            }
            return list;
        }
    }

    public List<ItemData> showEffectImage = new List<ItemData>();


    /// <summary>
    /// 所有报价清单
    /// </summary>
    public List<ItemData> offerList
    {
        get
        {
            List<ItemData> list = new List<ItemData>();
            for (int i = 0; i < schemeManifest.prices.Count; i++)
            {
                string priceId = schemeManifest.prices[i];
                ManifestOfferItem itemData = new ManifestOfferItem();
                itemData.priceDataId = priceId;
                list.Add(itemData);
            }

            return list;
        }
    }

    /// <summary>
    /// 所有物品清单
    /// </summary>
    public List<ItemData> productList
    {
        get
        {
            List<ItemData> list = GetInputProductDataList();
            return list;
        }
    }
    public MainPageStateMachine machine
    {
        get
        {
            return MainPageStateMachine.Instance;
        }
    }
    //public List<ItemData> renderEffectList;

    public SchemeManifest schemeManifest
    {
        get
        {
            return SchemeManifest.Instance;
        }
    }

    private Dictionary<string, int> helpDic = new Dictionary<string, int>();

    public OriginalInputData inputData
    {
        get { return OriginalInputData.Instance; }
    }

    private MainPage mainPage
    {
        get
        {
            return MainPage.Instance;
        }
    }

    private MainPageUIData uiData
    {
        get { return MainPageUIData.Instance; }
    }

    public SchemePageController()
    {
        panel = scheme = SchemePage.Instance;
    }
    public override void ready()
    {
        base.ready();
        mainScheme();
        inventoryOffer();
        effectImage();
        startColor = scheme.productText.color;
        currColor = scheme.modelText.color;
        UITool.SetActionFalse(scheme.modelBG.gameObject);

        scheme.createOfferTotalPrice.text = string.Format("参考总价：{0}", 998);
        scheme.generateOfferTotalPrice.text = string.Format("参考总价：{0}", 998);
        scheme.setMainOfferTotalPrice.text = string.Format("参考总价：{0}", 998);
        //scheme.productText.color = startColor;
        //EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        //SaveOffer();
    }

    //private void SaveOffer()
    //{
    //    string json = UnityEngine.JsonUtility.ToJson(schemeManifest);
    //    //cacheScheme.AddSchemeCache("",json); 
    //}
    /// <summary>
    /// 方案主页    里面包含  效果图和清单
    /// </summary>
    private void mainScheme()
    {
        mainPage.schemeName.text = schemeManifest.name;
        schemeImagesScroll = scheme.ImageScrollView.gameObject.AddComponent<SchemeEffectImageScroll>();
        schemeImagesScroll.addEventListener(SchemeEvent.openEffectImage, OpenEffectImage);
        schemeManifestScroll = scheme.OfferScrollView.gameObject.AddComponent<SchemeManifestScroll>();
        schemeManifestScroll.addEventListener(SchemeEvent.AddOffer, AddOffer);
        schemeManifestScroll.addEventListener(SchemeEvent.openOffer, OpenOffer);
        createOfferScroll = scheme.createScrollView.gameObject.AddComponent<CreateOfferScroll>();
        offerModerScroll = scheme.offerModelScrollView.gameObject.AddComponent<OfferModerScroll>();
        setMainOfferScroll = UITool.AddUIComponent<SetMainOfferScroll>(scheme.setMainScrollView.gameObject);
        editorOfferScroll = UITool.AddUIComponent<GeneraterOfferScroll>(scheme.generaterScrollView.gameObject);
        //scheme.OfferScrollView.gameObject.SetActive(false);
        fullScreenScroll = UITool.AddUIComponent<FullScreenScroll>(scheme.effectScroll.gameObject);
        scheme.exitBut.onClick.AddListener(onExit);
        scheme.effectBut.onClick.AddListener(onOpenEffect);
        scheme.detailBut.onClick.AddListener(onOpenDetail);
        scheme.nameText.text = schemeManifest.name;
        scheme.desText.text = schemeManifest.description;
        scheme.nameText.onValueChanged.AddListener(onIputSchemeName);
        scheme.desText.onValueChanged.AddListener(onIputSchemeDes);
        //schemeManifest.name = scheme.namePlaceholder.text;
        //schemeManifest.description = scheme.desPlaceholder.text;
        this.addEventListener(SchemeEvent.ChangeScheme, ChangeScheme);
        fullScreenScroll.addEventListener(IOSEvent.ShareImage, ShareImage);
        fullScreenScroll.addEventListener(MainPageUIDataEvent.SchemeEffectImageDeleted, SchemeEffectImageDeleted);
    }

    private void SchemeEffectImageDeleted(MyEvent obj)
    {
        string path = obj.data.ToString();
        schemeManifest.images.Remove(path);
        schemeImagesScroll.Display(effectImageList);
        fullScreenScroll.Display(effectImageList);
        if (schemeManifest.images.Count == 0)
        {
            onExit();
        }
    }

    private void ShareImage(MyEvent obj)
    {
        MsgToIOS msg = new MsgToIOS();
        msg.code = "101007";
        msg.info = new MsgToIOS.InfoToIOS();
        msg.info.shareType = 2;
        msg.info.templateId = schemeManifest.templateId;
        if (schemeManifest.tempEffectMetas.Count != 0)
        {
            msg.info.msg = schemeManifest.tempEffectMetas[0];
        }
        UnityIOSMsg.sendToIOS(msg);
    }

    private void ChangeScheme(MyEvent obj)
    {
        if (scheme.nameText == null || scheme.desText == null)
        {
            return;
        }
        scheme.nameText.text = schemeManifest.name + "";
        scheme.desText.text = schemeManifest.description + "";
    }
    private void onIputSchemeName(string value)
    {
        if (value.Length >= 10)
        {

            value = string.Format("{0}.", value.Substring(0, 10));
        }
        mainPage.schemeName.text = value;
        schemeManifest.name = value;
    }
    private void onIputSchemeDes(string value)
    {
        schemeManifest.description = value;
    }

    public void OpenEffectImage(MyEvent myEvent)
    {
        CommonChangeChildActive(scheme.RectEffect);
        fullScreenScroll.Display(effectImageList);
    }
    public void AddOffer(MyEvent myEvent)
    {
        Debug.Log("添加报价");
        isCreateOffer = false;
        OpenManifest();
    }

    /// <summary>
    /// 打开清单
    /// </summary>
    private void OpenManifest()
    {
        Debug.Log("打开清单");
        if (productList.Count == 0)
        {
            //MessageBox.instance.Show("当前没有报价，请添加报价", "", null,null,null, MessageBoxButton.OK);
            return;
        }
        CommonChangeChildActive(scheme.createOffer);
        createOfferScroll.Display(productList);
    }
    private void OnCreateOffer()
    {
        Debug.Log("创建 报价 数据 编辑用 还未保存");

        PriceData pricedata = new PriceData();

        List<ItemData> list = GetInputProductDataList();

        pricedata.priceList = list;
        GeneraterOfferScroll.ToggleGroup(true);
        OpenEditorOffer(pricedata);
    }
    /// <summary>
    /// 编辑报价
    /// </summary>
    /// <param name="pricedata"></param>
    public void OpenEditorOffer(PriceData pricedata)
    {
        Debug.Log("打开指定报价");
        UITool.SetActionFalse(scheme.tab2);
        UITool.SetActionTrue(scheme.tab1);
        UITool.SetActionTrue(scheme.offerGenerate.gameObject);
        UITool.SetActionFalse(scheme.deletePrice.gameObject);
        UITool.SetActionTrue(scheme.mask.gameObject);
        UITool.SetActionTrue(scheme.generaterExit.gameObject);
        editorOfferScroll.isLeftMove = true;
        for (int i = 0; i < pricedata.priceList.Count; i++)
        {
            ItemPrice item = pricedata.priceList[i] as ItemPrice;
            item.isSelect = true;
        }
        if (pricedata.priceList.Count == 0)
        {
            scheme.offerGenerate.interactable = false;
        }
        else
        {
            scheme.offerGenerate.interactable = true;
        }
        //设为当前报价
        currentEditorablePricedata = pricedata;
        //打开界面
        CommonChangeChildActive(scheme.generateOffer);
        //显示数据 可编辑
        editorOfferScroll.Display(currentEditorablePricedata.priceList);
        //itemGeneraterOffer = itemCreateOffer;
    }
    public void OpenFastEditorOffer(PriceData pricedata)
    {
        Debug.Log("打开指定报价快速编辑");
        UITool.SetActionFalse(scheme.tab1);
        UITool.SetActionTrue(scheme.tab2);
        UITool.SetActionFalse(scheme.offerGenerate.gameObject);
        UITool.SetActionTrue(scheme.deletePrice.gameObject);
        //editorOfferScroll.selectDeleteList.Clear();
        UITool.SetActionFalse(scheme.mask.gameObject);
        UITool.SetActionFalse(scheme.generaterExit.gameObject);
        isPress = false;
        scheme.offerAllCancel.text = "全选";
        editorOfferScroll.isLeftMove = false;
        for (int i = 0; i < pricedata.priceList.Count; i++)
        {
            ItemPrice item = pricedata.priceList[i] as ItemPrice;
            item.isSelect = false;
        }
        //GeneraterOfferScroll.ToggleGroup(false);

        //设为当前报价
        currentEditorablePricedata = pricedata;
        //打开界面
        CommonChangeChildActive(scheme.generateOffer);
        //显示数据 可编辑
        editorOfferScroll.Display(currentEditorablePricedata.priceList);


        //itemGeneraterOffer = itemCreateOffer;
    }
    private void OnGenerateOffer()
    {
        Debug.Log("生成报价");
        if (currentEditorablePricedata == null)
        {
            return;
        }

        if (undoHelper.currentData.SaveId != 0)
        {
            this.addEventListener(SchemeEvent.SaveSchemeToGenerateOffer, SaveSchemeToGenerateOffer);
            MessageBox.Instance.ShowOkCancelClose("生成报价单", "生成报价前请先保存方案",  null, SaveSchemeToOffer);
            return;
        }

        PriceData newpriceData = new PriceData();
        newpriceData.priceList = new List<ItemData>();
        newpriceData.isNew = true;
        newpriceData.tempId = jsonCacheManager.GetNewOfferTempId();
        newpriceData.targetServerId = schemeManifest.id;
        newpriceData.targetTempId = schemeManifest.tempId;
        for (int i = 0; i < currentEditorablePricedata.priceList.Count; i++)
        {
            ItemPrice item = currentEditorablePricedata.priceList[i] as ItemPrice;
            if (item.isSelect == false)
            {
                continue;
            }
            ItemPrice newitem = new ItemPrice();
            newitem.seekId = item.seekId;
            Product product = uiData.getProduct(newitem.seekId);
            newitem.image = product.tempTexture;
            newitem.productName = product.name;
            newitem.count = item.count;
            newpriceData.priceList.Add(newitem);
        }

        uiData.AddPriceData(newpriceData);
        jsonCacheManager.AddOfferCache(newpriceData);
        schemeManifest.prices.Add(newpriceData.tempId.ToString());

        schemeManifestScroll.Display(offerList);
        OpenOffer(newpriceData);
        //currentPricedata = newpriceData;
        //CommonChangeChildActive(scheme.setMainOffer);
        //setMainOfferScroll.Display(newpriceData.priceList);


        if (newpriceData.isTargetUnUpload == true)
        {
            //方案还未上传 一定无法生成报价
            return;
        }
        //上传报价
        MsgToIOS msg = new MsgToIOS();
        msg.code = "101101";
        msg.info = new MsgToIOS.InfoToIOS();
        msg.info.priceData = newpriceData;
        UnityIOSMsg.sendToIOS(msg, IOSEvent.SetOfferId, SetOfferId);

    }
    private void SaveSchemeToOffer()
    {
        this.dispatchEvent(new MyEvent(SchemeEvent.SaveSchemeToGenerateOffer));
    }
    private void SaveSchemeToGenerateOffer(MyEvent obj)
    {
  
        this.removeEventListeners(SchemeEvent.SaveSchemeToGenerateOffer);
        this.addEventListener(SchemeEvent.GenerateOffer,GenerateOffer);
        machine.setState(SaveState.Name);
        
    }
    private void GenerateOffer(MyEvent obj)
    {
        this.removeEventListener(SchemeEvent.GenerateOffer, GenerateOffer);
        Debug.Log("保存方案并且生成报价单");
        OnGenerateOffer();
    }
    private void OnShareMainOffer()
    {
        Debug.Log("分享主报价");
        //Debug.Log("报价分享");
        //101007 分享 title content photo HT5url collectTypr
        if (currentCannotEditorablePricedata.isTargetUnUpload == true)
        {
            Debug.LogWarning("请先上传方案");
            MessageBox.Instance.ShowOkCancelClose("", "请先上传方案", null,SaveToScheme);
            //提示：请先上传方案！
            return;
        }
        if (currentCannotEditorablePricedata.isUnUpload == true)
        {
            Debug.LogWarning("请先上传报价");
            MessageBox.Instance.ShowOkCancelClose("", "请先上传报价", null, BeforeSaveOffer);
            //提示：请先上传报价或者正在上传！
            return;
        }
        MsgToIOS msg = new MsgToIOS();
        msg.code = "101007";
        msg.info = new MsgToIOS.InfoToIOS();
        msg.info.shareType = 1;
        msg.info.priceData = currentCannotEditorablePricedata;
        UnityIOSMsg.sendToIOS(msg);
    }
   private void SaveToScheme()
    {
        this.addEventListener(MainPageUIDataEvent.SchemeIdChanged, OnSchemeIdChanged);
        machine.setState(SaveState.Name);
    }

    private void OnSchemeIdChanged(MyEvent obj)
    {
        this.removeEventListener(MainPageUIDataEvent.SchemeIdChanged, OnSchemeIdChanged);
        MsgFromIOS.InfoFromIOS info = obj.data as MsgFromIOS.InfoFromIOS;
        if (currentCannotEditorablePricedata.targetId == info.tempId.ToString())
        {
            currentCannotEditorablePricedata.targetServerId = info.projectId;
        }
        //MessageBox.instance.Show("请先上传报价", "", null, BeforeSaveOffer, null, MessageBoxButton.CancelOK);
        BeforeSaveOffer();
    }


    private void BeforeSaveOffer()
    {
        this.addEventListener(MainPageUIDataEvent.OfferIdChanged, OnOfferIdChanged);
        SaveOffer();
    }

    private void OnOfferIdChanged(MyEvent obj)
    {
        this.removeEventListener(MainPageUIDataEvent.OfferIdChanged, OnOfferIdChanged);
        MsgFromIOS.InfoFromIOS info = obj.data as MsgFromIOS.InfoFromIOS;
        if (currentCannotEditorablePricedata.stringId == info.tempId.ToString())
        {
            currentCannotEditorablePricedata.serverId = info.priceId;
        }

        MsgToIOS msg = new MsgToIOS();
        msg.code = "101007";
        msg.info = new MsgToIOS.InfoToIOS();
        msg.info.shareType = 1;
        msg.info.priceData = currentCannotEditorablePricedata;
        UnityIOSMsg.sendToIOS(msg);
    }

    private void SaveOffer()
    {
        MsgToIOS msg = new MsgToIOS();
        msg.code = "101101";
        msg.info = new MsgToIOS.InfoToIOS();
        msg.info.priceData = currentCannotEditorablePricedata;
        UnityIOSMsg.sendToIOS(msg, IOSEvent.SetOfferId, SetOfferId);
        //machine.setState(SaveState.Name);
    }
    private void OnSetMainGenerate()
    {
        if (isCreateOffer == true)
        {
            for (int i = 0; i < 4; i++)
            {
                onExit();
            }
        }
        for (int i = 0; i < 3; i++)
        {
            onExit();
        }
        currentEditorablePricedata.isSetMainOffer = true;
        schemeManifestScroll.Display(currentEditorablePricedata.priceList);
        //this.dispatchEvent(new MyEvent(MyEventSetMainOffer.setMainOffer));
        Debug.Log("设为主报价");
    }
    /// <summary>
    /// 打开指定报价
    /// </summary>
    /// <param name="pricedata"></param>
    public void OpenOffer(PriceData pricedata)
    {
        Debug.Log("打开指定报价");
        //设为当前报价
        currentCannotEditorablePricedata = pricedata;
        //打开界面
        CommonChangeChildActive(scheme.setMainOffer);

        //显示数据
        setMainOfferScroll.Display(currentCannotEditorablePricedata.priceList);
        //itemGeneraterOffer = itemCreateOffer;

    }
    /// <summary>
    /// 已经存在的报价， 打开查看详情
    /// </summary>
    public void OpenOffer(MyEvent myEvent)
    {
        //Debug.LogWarning("22222222222");
        SchemeEvent myevent = myEvent as SchemeEvent;
        ManifestOfferItem manifestOffer = myevent.data as ManifestOfferItem;
        string openPriceDataId = manifestOffer.priceDataId;
        PriceData priceData = uiData.GetPriceData(openPriceDataId);
        OpenOffer(priceData);
    }

    /// <summary>
    ///方案里面 清单报价
    /// </summary>
    private void inventoryOffer()
    {
        scheme.offerExit.onClick.AddListener(onExit);
        scheme.setMainExit.onClick.AddListener(onExit);
        scheme.createExit.onClick.AddListener(onExit);
        scheme.generaterExit.onClick.AddListener(onExit);
        // UITool.AddUIComponent<CreateOfferScroll>(scheme.createScrollView.gameObject);
        UITool.SetActionFalse(scheme.offerModel.gameObject);
        UITool.SetActionTrue(scheme.offerProducePicture.gameObject);
        scheme.offerCreate.onClick.AddListener(OnCreateOffer);
        scheme.productManifest.onClick.AddListener(OnProduct);
        scheme.modelManifest.onClick.AddListener(OnModel);
        scheme.deletePrice.onClick.AddListener(OnDeleteOffer);
        scheme.offerAllSelect.onClick.AddListener(OnAllSelectOffer);
        scheme.offerAllCancel.text = "全选";
        scheme.offerGenerate.onClick.AddListener(OnGenerateOffer);
        //scheme.generateShare.onClick.AddListener(OnGenerateShare);
        scheme.ShareMain.onClick.AddListener(OnShareMainOffer);
        scheme.deleteOffer.onClick.AddListener(onDeleteManiOffer);
        scheme.setOfferMain.onClick.AddListener(OnSetMainGenerate);
        scheme.editor.onClick.AddListener(onEditor);
        scheme.finish.onClick.AddListener(OnFinish);
        // 初始化 
        UITool.SetActionFalse(scheme.tab2);
        UITool.SetActionTrue(scheme.tab1);
        UITool.SetActionTrue(scheme.offerGenerate.gameObject);
        UITool.SetActionFalse(scheme.deletePrice.gameObject);
        UITool.SetActionTrue(scheme.mask.gameObject);
        editorOfferScroll.isLeftMove = true;
       // OnFinish();
    }

    private void OnFinish()
    {
        int totalProduct = 0;
        for (int i = 0; i < editorOfferScroll.Msgs.Count; i++)
        {
            ItemPrice item = editorOfferScroll.Msgs[i] as ItemPrice;
            
            if (item.isSelect == false)
            {
                editorOfferScroll.Msgs.RemoveAt(i);
                i--;
            }
            else
            {
                totalProduct += item.count;
            }
            
        }
        Debug.Log("共计使用{0}件物品"+totalProduct);
        scheme.genertaTotalNumber.text = string.Format("共计使用{0}件物品", totalProduct);
        //scheme.createTotalNumber.text = string.Format("共计使用{0}件物品", totalProduct);
        OpenEditorOffer(currentEditorablePricedata);
        onExit();
        onExit();
    }

    private void onEditor()
    {
        OpenFastEditorOffer(currentEditorablePricedata);
    }

    /// <summary>
    /// 方案里的 效果图
    /// </summary>
    private void effectImage()
    {
        scheme.effectExit.onClick.AddListener(onExit);
    }
    #region Scheme Onclick
    private void onExit()
    {
        if (childPageStack.Count > 0)
        {
            childPageStack.Pop().gameObject.SetActive(false);
        }
        if (childPageStack.Count > 0) childPageStack.Peek().gameObject.SetActive(true);
        if (isCreateOffer && scheme.RectshemeNode.gameObject.activeSelf)
        {
            UITool.SetActionFalse(scheme.RectshemeNode.gameObject);
        }
    }
    public void OpenScheme()
    {
        CommonChangeChildActive(scheme.RectshemeNode);
        scheme.nameText.text = schemeManifest.name;
        scheme.desText.text = schemeManifest.description;
        mainPage.schemeName.text = schemeManifest.name;
        schemeImagesScroll.Display(effectImageList);
        schemeManifestScroll.Display(offerList);

        // 从 name 文件中读取JSON

        // 文件存在 ,序列化

        // 读取数据 jons -> 对象（object）
    }

    public void ObjectToJsonData(string name)
    {
        schemeManifest.name = scheme.nameText.text;
        schemeManifest.description = scheme.desText.text;
        ManageResource.WriteSchemeJson(name, schemeManifest);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
    public void onOpenEffect()
    {
        UITool.SetActionTrue(scheme.effectBG.gameObject);
        UITool.SetActionFalse(scheme.detailBG.gameObject);
        scheme.effectText.color = startColor;
        scheme.detailText.color = currColor;
        UITool.SetActionFalse(scheme.OfferScrollView.gameObject);
        UITool.SetActionTrue(scheme.ImageScrollView.gameObject);
    }
    public void onOpenDetail()
    {
        UITool.SetActionFalse(scheme.effectBG.gameObject);
        UITool.SetActionTrue(scheme.detailBG.gameObject);
        scheme.effectText.color = currColor;
        scheme.detailText.color = startColor;
        UITool.SetActionFalse(scheme.ImageScrollView.gameObject);
        UITool.SetActionTrue(scheme.OfferScrollView.gameObject);
    }
    private bool isCreateOffer = false ;
    public void OnOpenOffer(bool isCreateOffer = false )
    {
        this.isCreateOffer = isCreateOffer;
        if (productList.Count == 0)
        {
            //MessageBox.instance.Show("当前没有报价，请添加报价", "", null, null, null, MessageBoxButton.OK);
            return;
        }
        Debug.Log("初次打开清单界面");
        CommonChangeChildActive(scheme.RectshemeNode);
        OpenManifest();
    }
    public void onDeleteManiOffer()
    {
        //TODO
        Debug.Log("删除 主报价");
    }
    public void CommonChangeChildActive(RectTransform rectTrans)
    {
        scheme.skin.SetActive(true);
        if (childPageStack.Count > 0) childPageStack.Peek().gameObject.SetActive(false);
        childPageStack.Push(rectTrans);
        childPageStack.Peek().gameObject.SetActive(true);
    }
    #endregion

    #region Offer Onclick
    

    private List<ItemData> GetInputProductDataList()
    {
        helpDic.Clear();
        for (int i = 0; i < inputData.productDataList.Count; i++)
        {
            string seekId = inputData.productDataList[i].seekId;
            int count;
            if (helpDic.TryGetValue(seekId, out count))
            {
                helpDic[seekId] = count + 1;
            }
            else
            {
                helpDic.Add(seekId, 1);
            }
        }
        List<ItemData> list = new List<ItemData>();
        foreach (string key in helpDic.Keys)
        {
            ItemPrice itemData = new ItemPrice();
            itemData.seekId = key;
            Product product = uiData.getProduct(itemData.seekId);
            itemData.image = product.tempTexture;
            itemData.productName = product.name;
            itemData.count = helpDic[key];
            list.Add(itemData);
        }
        return list;
    }

    private void OnProduct()
    {
        Debug.Log("ProductManifest");
        UITool.SetActionTrue(scheme.productBG.gameObject);
        UITool.SetActionFalse(scheme.modelBG.gameObject);
        UITool.SetActionTrue(scheme.productPictureTitle.gameObject);
        UITool.SetActionFalse(scheme.productModelTitle.gameObject);
        UITool.SetActionTrue(scheme.createScrollView.gameObject);
        UITool.SetActionFalse(scheme.offerModelScrollView.gameObject);
        scheme.productText.color = startColor;
        scheme.modelText.color = currColor;
        UITool.SetActionTrue(scheme.offerModel.gameObject);
        UITool.SetActionTrue(scheme.offerProducePicture.gameObject);
    }
    private void OnModel()
    {
        Debug.Log("ModelManifest");
        UITool.SetActionTrue(scheme.modelBG.gameObject);
        UITool.SetActionFalse(scheme.productBG.gameObject);
        UITool.SetActionTrue(scheme.productModelTitle.gameObject);
        UITool.SetActionFalse(scheme.productPictureTitle.gameObject);
        UITool.SetActionFalse(scheme.createScrollView.gameObject);
        UITool.SetActionTrue(scheme.offerModelScrollView.gameObject);
        scheme.modelText.color = startColor;
        scheme.productText.color = currColor;
        UITool.SetActionFalse(scheme.offerProducePicture.gameObject);
        UITool.SetActionTrue(scheme.offerModel.gameObject);
        offerModerScroll.Display(productList);
    }
    private void OnDeleteOffer()
    {
        GeneraterOfferScroll.DeleteSelete();
        GeneraterOfferScroll.ToggleGroup(false);
    }
    public bool isPress = false;
    private SchemeEffectImageScroll schemeImagesScroll;
    private SchemeManifestScroll schemeManifestScroll;
    private CreateOfferScroll createOfferScroll;
    private OfferModerScroll offerModerScroll;
    private GeneraterOfferScroll editorOfferScroll;
    private SetMainOfferScroll setMainOfferScroll;
    private FullScreenScroll fullScreenScroll;
    /// <summary>
    /// 编辑界面显示报价
    /// </summary>
    private PriceData currentCannotEditorablePricedata = null;
    /// <summary>
    /// 分享界面显示报价
    /// </summary>
    private PriceData currentEditorablePricedata = null;

    private void OnAllSelectOffer()
    {
        isPress = !isPress;
        if (isPress)
        {
            scheme.offerAllCancel.text = "取消全选";
            Debug.Log("全选");
            GeneraterOfferScroll.ToggleGroup(true);
        }
        else
        {
            scheme.offerAllCancel.text = "全选";
            Debug.Log("取消全选");
            GeneraterOfferScroll.ToggleGroup(false); 
        }
    }
   
    private void SetOfferId(MyEvent obj)
    {
        IOSEvent e = obj as IOSEvent;
        MsgFromIOS.InfoFromIOS info = e.data as MsgFromIOS.InfoFromIOS;

        //根据临时tempId找到数据 PriceData priceData = GetPriceData(info.tempId);
        PriceData priceData = uiData.GetPriceData(info.tempId.ToString());
        if (priceData == null)
        {
            
            return;
        }

        //删除本地临时缓存-未上传服务器的数据
        jsonCacheManager.RemoveOfferCache(priceData.tempId);
        //移除字典中按本地临时id存储的数据--为后面用新正式的id重新存入
        uiData.RemovePriceData(priceData.targetId, priceData.stringId);
        //移除当前数据的报价列表中的临时info.tempId
        schemeManifest.prices.Remove(info.tempId.ToString());

        // 给数据赋值服务器给的新id priceData.serverId = info.priceId;
        priceData.serverId = info.priceId;

        //存入服务器报价缓存
        jsonCacheManager.AddOfferCache(priceData);
        //重新存入字典 报价不会变动 下一次也可以直接使用 防止下次使用再去加载 
        uiData.AddPriceData(priceData);
        //添加当前数据的报价列表info.priceId
        schemeManifest.prices.Add(info.priceId);

        //抛报价本地变服务器的事件 此处用处不大  SetOfferId(MyEvent obj)本身就是监听的UnityIOSMsg的事件
        this.dispatchEvent(new MainPageUIDataEvent(MainPageUIDataEvent.OfferIdChanged, e.data));
    }

    private void OnGenerateDelete()
    {
        Debug.Log("删除生成报价");
    }
    #endregion

}
public class EffectImageItem : ItemData
{
    public EffectImageItem(string path)
    {
        this.path = path;
    }
    public string path;
}
public class ManifestOfferItem : ItemData
{
    public bool special = false;

    /// <summary>
    /// jietu
    /// </summary>
    public string texureEncoding;
    public string priceDataId;
    //public bool isSetMainOffer;
    //public string image;
    //public string name;
    ////  商品品牌
    //public string brand;
    //// 产品 编号
    //public string productNo;
    //public string price;
    //public int total;
    public ManifestOfferItem()
    { }
    public ManifestOfferItem(string image, string name, string brand, string productNo, string price, string number, int total)
    {
        //this.image = image;
        //this.name = name;
        //this.brand = brand;
        //this.productNo = productNo;
        //this.price = price;
        //this.number = number;
        //this.total = total;
    }
}

// 外部调用方法 MainpageUIController  onScheme()
//public void commonChangeSchemeActive(RectTransform rect)
//{
//    if (childPageStack.Count <=1 )
//    {
//        if (!childPageStack.Contains(rect))
//        {
//            childPageStack.Clear();
//            childPageStack.Push(rect);
//        }
//        childPageStack.Peek().gameObject.SetActive(true);
//        childPageStack.Push(scheme.RectshemeNode);
//        childPageStack.Peek().gameObject.SetActive(true);
//        return;
//    }
//    childPageStack.Peek().gameObject.SetActive(false);
//    childPageStack.Push(rect);
//    childPageStack.Peek().gameObject.SetActive(true);
//}
// 外部调用方法   MainpageUIController  onOffer()