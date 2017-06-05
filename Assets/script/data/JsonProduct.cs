using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
/// <summary>
/// 产品 （详情）
/// </summary>
public class JsonProduct
{
    public JsonProduct(){}
    public JsonProduct(string id, Vector3 size)
    {
        this.seekId = this.id = id;
        this.size = size;
        this.top = "2D/texture2D/" + id + ".assetbundle";
        this.thumbnail = "UI/texture2D/" + id + ".assetbundle";
        this.model3d = "3D/prefab/" + id + ".assetbundle";
    }
    public string id;// = "4abd1bba-6d61-481e-9008-25295599e006";
    public Vector3 size;// = new Vector3(0.95f, 2.09f, 0.18f);
    public float defaultHeight;// = 0;
    public string seekId;// = "4abd1bba-6d61-481e-9008-25295599e006";
    public string name;// = "VANILLA RVUG1";
    public string unit;// = "cm";
    public string productType;// = "model";
    public string contentType;// = "door/single swing door";

    /// <summary>
    /// 弃用的 请使用contentType
    /// </summary>
    public string entityType;// = "seating";//material_floor seating flooring ceiling surface window door wall_attachment ceiling_attachment    hole Assembly
    public string thumbnail;// = "http://cf-hsm-prod-assets.homestyler.com/i/4abd1bba-6d61-481e-9008-25295599e006/resized/iso0_w160_h160.jpg";
    public List<string> images = new List<string>();//{ "http://cf-hsm-prod-assets.homestyler.com/i/4abd1bba-6d61-481e-9008-25295599e006/iso.jpg" };
    public List<string> imagesResize = new List<string>();// { "http://cf-hsm-prod-assets.homestyler.com/i/4abd1bba-6d61-481e-9008-25295599e006/iso.jpg" };
    public List<string> categories = new List<string>();// { "2afb28e9-3b85-4ce4-b826-1eff97886de4", "2ffc642e-0042-48b8-b3f0-3e9191011a0a"};
    public List<string> families;
    public string v;// = "Barausse";
    public string vu;//= "https://hsm-prod-assets.s3.amazonaws.com/vendorlogos/temp-54858eeb-fe24-4073-aaf6-83536936b83a.jpg";
    public string productURL;// = "http://www.barausse.com/portal/en/international/products/interior-doors/line-index/VANILLA/VANILLA%20RVUG1/";
    public int status;// = 1;
    public string modelTexture;// = "http://hsm-prod-assets.s3.amazonaws.com/i/4abd1bba-6d61-481e-9008-25295599e006/model-fc.png";

    public string model3d;// = "http://hsm-prod-assets.s3.amazonaws.com/i/4abd1bba-6d61-481e-9008-25295599e006/model.gz.bobj";
    public string top;//= "https://hsm-prod-assets.s3.amazonaws.com/svg/swinging_door.svg";
    public List<string> variations;
    public string variationId;// = "";
    public bool isScalable;// = false;
    public List<string> components;



    //未知的接收到的数据
    public List<string> retailers;//经销商
    public string modifiedTime;//更改时间
    public Product.Ticket ticket;//目前全是1
    public List<string> groups;//打包后的组
    public string zIndex;//图层
    public string tileSize;//贴图缩放
    public List<Product.Attributes1> attributes;//乱七八糟的属性
    //新发现变量
    public string bomcategoryId;
    public SnapPlaneMesh snapPlaneMesh;
    public List<AssemblyItem> products;
    public Dictionary<string, JsonProduct> productDataById;
    public string price;
    public string sku;

    private string assetBundlePath;//= "";

    public string tempTexture
    {
        get {
            if (imagesResize.Count > 0)
            {
                return imagesResize[0];
            }
            return images[0].Replace("iso.jpg", "resized/iso_w160_h160.jpg");
        }
    }

    public string GetAssetBundlePath()
    {
        //if (string.IsNullOrEmpty(assetBundlePath))
        //{
        //    assetBundlePath = model3d.Replace(".gz.bobj", ".assetbundle");
        //}
        return model3d;
    }

    public void DeSerialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "id":
                    id = MyJsonTool.getStringValue(dic, key);
                    break;
                case "size":
                    size = MyJsonTool.getVector3(dic, key);
                    break;
                case "defaultHeight":
                    defaultHeight = MyJsonTool.getFloatValue(dic, key);
                    break;
                case "seekId":
                    seekId = MyJsonTool.getStringValue(dic, key);
                    break;
                case "name":
                    name = MyJsonTool.getStringValue(dic, key);
                    break;
                case "unit":
                    unit = MyJsonTool.getStringValue(dic, key);
                    break;
                case "productType":
                    productType = MyJsonTool.getStringValue(dic, key);
                    break;
                case "contentType":
                    contentType = MyJsonTool.getStringValue(dic, key);
                    break;
                case "entityType":
                    entityType = MyJsonTool.getStringValue(dic, key);
                    break;
                case "thumbnail":
                    thumbnail = MyJsonTool.getStringValue(dic, key);
                    break;
                case "images":
                    images = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "imagesResize":
                    imagesResize = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "categories":
                    categories = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "families":
                    families = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "v":
                    v = MyJsonTool.getStringValue(dic, key);
                    break;
                case "vu":
                    vu = MyJsonTool.getStringValue(dic, key);
                    break;
                case "productURL":
                    productURL = MyJsonTool.getStringValue(dic, key);
                    break;
                case "status":
                    status = MyJsonTool.getIntValue(dic, key);
                    break;
                case "modelTexture":
                    modelTexture = MyJsonTool.getStringValue(dic, key);
                    break;
                case "model3d":
                    {
                        //阿里云
                        string path = MyJsonTool.getStringValue(dic, key);
                        //alluserdata/youke001/Assetbundle/03e68276-b2f4-4a2a-ac71-c86b8690b618/model.assetbundle(Clone)
                        //"http://hsm-prod-assets.s3.amazonaws.com/i/4abd1bba-6d61-481e-9008-25295599e006/model.gz.bobj"
                        path = path.Replace("-fc.gz", ".assetbundle");
                        path = path.Replace("midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/", "midea-products.oss-cn-shanghai.aliyuncs.com/");

                        path = path.Replace(".gz.bobj", ".assetbundle");
                        path = path.Replace("hsm-prod-assets.s3.amazonaws.com/i/", "midea-products.oss-cn-shanghai.aliyuncs.com/");
                        //path = path.Replace("hsm-prod-assets.s3.amazonaws.com/i/", "pms.3dshome.net/");

                        model3d = path;
                    }
                    break;
                case "top":
                    {
                        //亚马逊
                        string path = MyJsonTool.getStringValue(dic, key);
                        //path = path.Replace("midea-products.oss-cn-shanghai.aliyuncs.com/", "pms.3dshome.net/");
                        this.top = path;
                    }
                    break;
                case "variations":
                    variations = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "variationId":
                    variationId = MyJsonTool.getStringValue(dic, key);
                    break;
                case "isScalable":
                    isScalable = MyJsonTool.getBoolValue(dic, key);
                    break;
                case "components":
                    components = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "retailers":
                    //retailers = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "modifiedTime":
                    //modifiedTime = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "ticket":
                    //ticket = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "groups":
                    //groups = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "zIndex":
                    //zIndex = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "tileSize":
                    //tileSize = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "attributes":
                    //attributes = MyJsonTool.getStringListValue(dic, key);
                    break;
                default:
                    Debug.LogWarning("Product缺少字段:" + key);
                    break;
            }
        }
    }

    
    internal void WriteToData(Product data)
    {
        data.retailers = this.retailers;
        data.boundingBox = new Product.BoundingBox(this.size);
        data.modifiedTime = this.modifiedTime;
        data.vendorUrl = this.vu;
        data.images = this.images;
        data.imagesResize = this.imagesResize;
        data.ticket = this.ticket;
        data.groups = this.groups;
        data.families = this.families;
        data.top = this.top;
        data.vendor = this.v;
        data.name = this.name;

        data.model = new Product.GoodsURL();
        //data.model.snapPlaneUrl = this.snapPlaneMesh;
        //data.model.modelNormalizedUrl = this.modelNormalizedUrl;
        //data.model.modelBinaryUrl = this.modelBinaryUrl;
        data.model.textureUrl = this.modelTexture;
        data.model.modelUrl = this.model3d;

        data.status = this.status;
        data.productType = this.productType;
        data.id = this.seekId;
        data.tp = this.entityType;
        data.zIndex = this.zIndex;
        data.tileSize = this.tileSize;
        
        data.attributes = this.attributes;
        if (data.attributes == null)
        {
            data.attributes = new List<Product.Attributes1>();
            Product.Attributes1 attributes1 = new Product.Attributes1();
            attributes1.name = "defaultHeight";
            attributes1.free = new List<string>() { this.defaultHeight.ToString()};
            data.attributes.Add(attributes1);
            Product.Attributes1 attributes2 = new Product.Attributes1();
            attributes2.name = "ContentType";
            attributes2.values = new List<Product.AttributesValue>();
            Product.AttributesValue itemvalue = new Product.AttributesValue();
            //itemvalue.value = this.contentType;//旧数据的contenttype不对 使用entitytype
            itemvalue.value = this.entityType;
            attributes2.values.Add(itemvalue);
            data.attributes.Add(attributes2);
        }
        
        data.categories = this.categories;
        data.variations = this.variations;
    }

}

[System.Serializable]
public class SnapPlaneMesh
{
    public MetaData metaData;
    public List<ContentData> geometries;
    public List<ContentData> materials;
    [SerializName]
    public ContentData serializName_object;//变量名object无法命名
}

[System.Serializable]
public class MetaData
{
    public float version;
    public string type;
    public string generator;
}

[System.Serializable]
public class ContentData {
    public string uuid;
    public string type;
    public float width;
    public float height;
    public int color;
    public int side;
    public string geometry;
    public string material;
    public Matrix4x4 matrix;
}

[System.Serializable]
public class AssemblyItem
{
    public string id;
    public Vector3 position;
    public float rotation;
}