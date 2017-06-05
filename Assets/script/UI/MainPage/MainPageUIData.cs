using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MainPageUIData : UIData<MainPageUIData>
{
    public OriginalInputData data
    {
        get {
            return OriginalInputData.Instance;
        }
    }
    public List<ProductData> productList = new List<ProductData>();
    //{
    //    get
    //    {
    //        return data.productList;
    //    }
    //}

    public MainPageUIData()
    {
        //"http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/8b7df9b0-28f5-4c88-a412-324284ccc923/model.assetBundle";//assetBundle";

        addTemplateData(0, new Vector2(-2, 2), new Vector2(2, 2), new Vector2(2, -2), new Vector2(-2, -2));
        addTemplateData(1, new Vector2(-2, 0), new Vector2(0, 0), new Vector2(0, 2), new Vector2(2, 2), new Vector2(2, -2), new Vector2(-2, -2));
        addTemplateData(2, new Vector2(-2, 2), new Vector2(2, 2), new Vector2(2, -2), new Vector2(0, -2), new Vector2(0, 0), new Vector2(-2, 0));
        addTemplateData(3, new Vector2(-2, 2), new Vector2(2, 2), new Vector2(2, 0), new Vector2(0, 0), new Vector2(0, -2), new Vector2(-2, -2));
        addTemplateData(4, new Vector2(-2, 2), new Vector2(0, 2), new Vector2(0, 0), new Vector2(2, 0), new Vector2(2, -2), new Vector2(-2, -2));
        addTemplateData(5, new Vector2(-2, 1), new Vector2(2, 1), new Vector2(2, -1), new Vector2(-2, -1));
        
    }

    public Dictionary<int, InputTemplateData> templateDic = new Dictionary<int, InputTemplateData>();
    public Dictionary<string, Product> productDic = new Dictionary<string, Product>();
    public Dictionary<string, GoodsVO> GoodsDic = new Dictionary<string, GoodsVO>();

    /// <summary>
    /// 《方案Id/tempId， 《报价id/tempId， PriceData》》
    /// </summary>
    public Dictionary<string, Dictionary<string, PriceData>> allPriceDic = new Dictionary<string, Dictionary<string, PriceData>>();

    public MaterialItemData defaultMaterialItem = new MaterialItemData();

    public bool GetCollected(string seekId)
    {
        return false;
    }

    public List<MaterialItemData> uimaterialList = new List<MaterialItemData>();

    public List<RenderEffectItemData> renderEffectItemData = new List<RenderEffectItemData>();

    public bool AddPriceData(PriceData priceData)
    {
        string targetId = priceData.targetId;
        if (allPriceDic.ContainsKey(targetId) == false)
        {
            allPriceDic.Add(targetId, new Dictionary<string, PriceData>());
        }
        string stringId = priceData.stringId;
        if (allPriceDic[targetId].ContainsKey(stringId) == false)
        {
            allPriceDic[targetId].Add(stringId, priceData);
            return true;
        }
        return false;
    }

    public bool RemovePriceData(string targetId, string stringId)
    {
        if (allPriceDic.ContainsKey(targetId) == false)
        {
            return false;
        }
        if (allPriceDic[targetId].ContainsKey(stringId) == false)
        {
            return false;
        }
        allPriceDic[targetId].Remove(stringId);
        return true;
    }
    public bool HasPriceData(string stringId)
    {
        foreach (Dictionary<string, PriceData> item in allPriceDic.Values)
        {
            if (item.ContainsKey(stringId))
            {
                return true;
            }
        }
        return false;
    }

    public PriceData GetPriceData(string stringId)
    {
        foreach (Dictionary<string, PriceData> item in allPriceDic.Values)
        {
            if (item.ContainsKey(stringId))
            {
                return item[stringId];
            }
        }
        return null;
    }

    public bool AddProduct(Product product)
    {
        if (productDic.ContainsKey(product.seekId) == true) productDic.Remove(product.seekId);
        productDic.Add(product.seekId, product);
        GoodsVO vo = new GoodsVO(GoodsDic.Count, product.modelTexture, product.top, product.assetBundlePath, product.attachType, 1, 0, product.seekId);
        if (GoodsDic.ContainsKey(vo.seekId) == true) GoodsDic.Remove(vo.seekId);
        GoodsDic.Add(vo.seekId, vo);
        return true;
    }

    //public Product getProduct(int goodsId)
    //{
    //    if (GoodsDic.ContainsKey(goodsId) == false)
    //    {
    //        return null;
    //    }
    //    return getProduct(GoodsDic[goodsId].seekId);
    //}
    public Product getProduct(string seekId)
    {
        if (productDic.ContainsKey(seekId) == false)
        {
            return null;
        }
        return productDic[seekId];
    }

    /// <summary>
    /// 获取到的数据不允许更改
    /// </summary>
    public GoodsVO getGoods(string seekId)
    {
        if (GoodsDic.ContainsKey(seekId) == false)
        {
            return null;
        }
        return GoodsDic[seekId];
    }

    private void addTemplateData(int id, params Vector2[] poss)
    {
        templateDic.Add(id, new InputTemplateData(id, poss));
    }

    public InputTemplateData GetData(int type)
    {
        if (templateDic.ContainsKey(type) == false) return null;
        return templateDic[type];
    }

    public List<Vector2> GetPosList(int type)
    {
        InputTemplateData data = GetData(type);
        if (data == null) return null;
        return data.offposList;
    }

}
