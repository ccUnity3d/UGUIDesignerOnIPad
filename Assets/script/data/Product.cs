using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Product
{
    public Product()
    {
    }

    /// <summary>
    /// 默认材质球使用
    /// </summary>
    public Product(string seekId, string stringId, string textureURL)
    {
        this.id = seekId;
        this.stringId = stringId;
        this.model.textureUrl = textureURL;
    }

    public List<string> retailers;//Product-
    public BoundingBox boundingBox;//Product-size
    public string modifiedTime;//Product-
    public string vendorUrl;//Product-vu
    public List<string> images;//Product-images
    public List<string> imagesResize;//Product-imagesResize
    public Ticket ticket;//Product-
    public List<string> groups;//Product-
    public List<string> families;//Product-families
    public string top;//Product-top
    public string vendor;//Product-v
    public string name;//Product-name
    public GoodsURL model;
    public int status;//Product-status
    public string productType;//Product-productType
    public string id;//Product-seekId
    public string tp;//Product-entityType//吸附类型 wall_attachment seating material_floor ceiling_attachment window door surface ceiling flooring hole Assembly
    public string zIndex;//Product-//-可能是离地高/shader参数
    public string tileSize;//Product-
    public List<Attributes1> attributes;//Product-
    public List<string> categories;//Product-categories
    public List<string> variations;//Product-variations

    public string seekId {
        get { return id; }
    }

    public string modelTexture {
        get { return model.textureUrl; }
    }

    private string _assetBundlePath;
    public string assetBundlePath
    {
        get{
            if (string.IsNullOrEmpty(_assetBundlePath) == true)
            {
                string path = model.modelUrl;//"4abd1bba-6d61-481e-9008-25295599e006"
                path = path.Replace("-fc.gz", ".assetbundle");
                path = path.Replace("midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/", "midea-products.oss-cn-shanghai.aliyuncs.com/");
                path = path.Replace(".gz.bobj", ".assetbundle");
                path = path.Replace("hsm-prod-assets.s3.amazonaws.com/i/", "midea-products.oss-cn-shanghai.aliyuncs.com/");
                _assetBundlePath = path;
            }
            return _assetBundlePath;
        }
    }

    private string _contentType;
    public string contentType
    {
        get {
            if (string.IsNullOrEmpty(_contentType))
            {
                ReadContentType();
            }
            return _contentType;
        }
    }

    private void ReadContentType()
    {
        _attachType = "";
        _contentType = "";
        _contentTypeBase = "";
        _contentTypeDetail = "";
        for (int i = 0; i < attributes.Count; i++)
        {
            if (attributes[i].name == "ContentType" && attributes[i].values.Count > 0)
            {
                _contentType = attributes[i].values[0].value;
                string[] arrs = _contentType.Split('/');
                _contentTypeBase = arrs[0];
                if (arrs.Length > 1)
                {
                    _contentTypeDetail = arrs[1];
                    string[] arr2s = _contentTypeDetail.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr2s.Length > 1) _attachType = arr2s[1];
                }
                else
                {
                    string[] arr2s = _contentType.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries);
                    if (arr2s.Length > 1)
                    {
                        _attachType = arr2s[1];
                    }
                    else
                    {
                        if (_contentType.IndexOf("floor-based") != -1)
                        {
                            _attachType = "floor-based";
                        }else if (_contentType.IndexOf("ceiling-attached") != -1)
                        {
                            _attachType = "ceiling-attached";
                        }else if (_contentType.IndexOf("wall-attached") != -1)
                        {
                            _attachType = "wall-attached";
                        }else if (_contentType.IndexOf("on top of others") != -1)
                        {
                            _attachType = "on top of others";
                        }
                    }
                }

                if (_contentTypeBase == "door" || _contentTypeBase == "window") _attachType = _contentTypeBase;
                break;
            }
        }
    }

    /// <summary>
    /// eg:in "door/single swing door" is "door"
    /// </summary>
    private string _contentTypeBase;
    public string contentTypeBase
    {
        get {
            if (string.IsNullOrEmpty(_contentTypeBase))
            {
                ReadContentType();
            }
            return _contentTypeBase;
        }
    }
    /// <summary>
    /// eg:in "door/single swing door" is "single swing door"
    /// </summary>
    private string _contentTypeDetail;
    public string contentTypeDetail
    {
        get
        {
            if (string.IsNullOrEmpty(_contentTypeDetail))
            {
                ReadContentType();
            }
            return _contentTypeDetail;
        }
    }

    private string _attachType;
    public string attachType
    {
        get
        {
            if (string.IsNullOrEmpty(_attachType))
            {
                ReadContentType();
            }
            return _attachType;
        }
    }

    private string _tempTexture;
    public string tempTexture
    {
        get
        {
            if (string.IsNullOrEmpty(_tempTexture))
            {
                _tempTexture = "";
                if (imagesResize.Count > 0)
                {
                    _tempTexture = imagesResize[0];
                }
                else
                {
                    _tempTexture = images[0].Replace("iso.jpg", "resized/iso_w160_h160.jpg");
                }
            }
            return _tempTexture;
        }
    }

    private float _defaultHeight = float.MinValue;
    public float defaultHeight
    {
        get
        {
            if (_defaultHeight == float.MinValue)
            {
                _defaultHeight = 0;
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (attributes[i].name == "defaultHeight" && attributes[i].free.Count > 0)
                    {
                        string attributeValue = attributes[i].free[0];
                        if (string.IsNullOrEmpty(attributeValue))
                        {
                            _defaultHeight = 0;
                            break;
                        }
                        _defaultHeight = float.Parse(attributeValue) / 100f;//单位转成米
                        break;
                    }
                }
            }
            return _defaultHeight;
        }
    }

    public Vector3 size
    {
        get
        {
            return boundingBox.getV3();
        }
    }
    
    /// <summary>
    /// 本地需求
    /// </summary>
    private string stringId;

    //public void WriteToProduct(JsonProduct product)
    //{
    //    product.id = id;
    //    product.seekId = model.id;
    //    product.thumbnail = model.vendorUrl;
    //    product.size = model.getSize;
    //    product.productType = model.productType;
    //    product.images = model.images;
    //    product.imagesResize = model.imagesResize;
    //    product.categories = model.categories;
    //    product.families = model.families;
    //    product.v = model.vendor;
    //    product.vu = model.vendorUrl;
    //    product.status = model.status;

    //    //亚马逊
    //    string top = model.top;//"http://midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/7e3e90d9-da17-459d-b7ed-14e681519d6a/Top.png";//
    //                           //top = top.Replace("midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/", "pms.3dshome.net/");
    //    product.top = top;

    //    //阿里云
    //    string path = model.model.modelUrl;//"4abd1bba-6d61-481e-9008-25295599e006"
    //    path = path.Replace("-fc.gz", ".assetbundle");
    //    path = path.Replace("midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/", "midea-products.oss-cn-shanghai.aliyuncs.com/");
    //    path = path.Replace(".gz.bobj", ".assetbundle");
    //    path = path.Replace("hsm-prod-assets.s3.amazonaws.com/i/", "midea-products.oss-cn-shanghai.aliyuncs.com/");
    //    product.model3d = path;//"http://midea-products.oss-cn-shanghai.aliyuncs.com/7e3e90d9-da17-459d-b7ed-14e681519d6a/model.assetbundle";//path//;

    //    product.modelTexture = model.model.textureUrl;//.Replace("midea-prod-assets.s3.cn-north-1.amazonaws.com.cn/", "midea-products.oss-cn-shanghai.aliyuncs.com/");
    //    product.entityType = model.tp;

    //    //product.components = model.components;
    //    //product.isScalable = model.isScalable;
    //    //product.variations = model.variations;
    //    //product.variationId = model.variationId;
    //    //product.defaultHeight = model.defaultHeight;
    //    //product.unit = model.unit;
    //    //product.contentType = model.contentType;
    //    //product.entityType = model.entityType;
    //    //product.productURL = model.productURL;
    //    if (model.attributes != null)
    //    {
    //        for (int i = 0; i < model.attributes.Count; i++)
    //        {
    //            Product.Attributes1 attributes1 = model.attributes[i];
    //            if (attributes1.name == "ContentType" && attributes1.values.Count > 0)
    //            {
    //                product.contentType = attributes1.values[0].value;
    //            }
    //        }
    //        if (product.contentType.StartsWith("window/") == true) product.defaultHeight = 1;//2;//
    //    }
    //    else
    //    {

    //    }
    //    product.name = model.name;
    //    product.retailers = model.retailers;
    //    product.modifiedTime = model.modifiedTime;
    //    product.ticket = model.ticket;
    //    product.groups = model.groups;
    //    product.zIndex = model.zIndex;
    //    product.tileSize = model.tileSize;
    //    product.attributes = model.attributes;
    //    product.variations = model.variations;
    //}
    public void DeSerialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "retailers":
                    retailers = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "boundingBox":
                    {
                        boundingBox = new BoundingBox();
                        object obj = MyJsonTool.getValue(dic, key);
                        boundingBox.Deserialize(obj as Dictionary<string, object>);
                    }
                    break;
                case "modifiedTime":
                    modifiedTime = MyJsonTool.getStringValue(dic, key);
                    break;
                case "vendorUrl":
                    vendorUrl = MyJsonTool.getStringValue(dic, key);
                    break;
                case "images":
                    images = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "imagesResize":
                    imagesResize = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "ticket":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        if (obj == null)
                        {
                            ticket = null;
                        }
                        else {
                            ticket = new Ticket();
                            ticket.Deserialize(obj as Dictionary<string, object>);
                        }
                    }
                    break;
                case "groups":
                    groups = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "families":
                    families = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "top":
                    top = MyJsonTool.getStringValue(dic, key);
                    break;
                case "vendor":
                    vendor = MyJsonTool.getStringValue(dic, key);
                    break;
                case "name":
                    name = MyJsonTool.getStringValue(dic, key);
                    break;
                case "model":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        model = new GoodsURL();
                        model.Deserialize(obj as Dictionary<string, object>);
                    }
                    break;
                case "status":
                    status = MyJsonTool.getIntValue(dic, key);
                    break;
                case "productType":
                    productType = MyJsonTool.getStringValue(dic, key);
                    break;
                case "id":
                    id = MyJsonTool.getStringValue(dic, key);
                    break;
                case "tp":
                    tp = MyJsonTool.getStringValue(dic, key);
                    break;
                case "zIndex":
                    zIndex = MyJsonTool.getStringValue(dic, key);
                    break;
                case "tileSize":
                    tileSize = MyJsonTool.getStringValue(dic, key);
                    break;
                case "attributes":
                    {
                        object obj = MyJsonTool.getValue(dic, key);
                        attributes = new List<Attributes1>();
                        List<object> list = obj as List<object>;
                        for (int i = 0; i < list.Count; i++)
                        {
                            Attributes1 attribute = new Attributes1();
                            attribute.Deserialize(list[i] as Dictionary<string, object>);
                            attributes.Add(attribute);
                        }
                    }
                    break;
                case "categories":
                    categories = MyJsonTool.getStringListValue(dic, key);
                    break;
                case "variations":
                    variations = MyJsonTool.getStringListValue(dic, key);
                    break;
                default:
                    Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                    break;
            }
        }
    }


    [System.Serializable]
    public class BoundingBox
    {
        public BoundingBox()
        { }
        public BoundingBox(Vector3 v3)
        {
            xLen = v3.x * 100;
            yLen = v3.z * 100;
            zLen = v3.y * 100;
        }

        public float xLen;
        public float yLen;
        public float zLen;

        private Vector3 v3;
        public Vector3 getV3()
        {
            if (v3 == default(Vector3))
            {
                v3 = Vector3.right * xLen + Vector3.up * zLen + Vector3.forward * yLen;
                v3 = v3 / 100;
            }
            return v3;
        }

        internal void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "xLen":
                        xLen = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "yLen":
                        yLen = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "zLen":
                        zLen = MyJsonTool.getFloatValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class Ticket
    {
        public string sku;

        internal void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "sku":
                        sku = MyJsonTool.getStringValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class GoodsURL
    {
        public string snapPlaneUrl;//Product-
        public string modelNormalizedUrl;//Product-
        public string textureUrl;//Product-modelTexture
        public string modelUrl;//Product-model3d
        public string modelBinaryUrl;//Product-

        internal void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "snapPlaneUrl":
                        snapPlaneUrl = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "modelNormalizedUrl":
                        modelNormalizedUrl = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "textureUrl":
                        textureUrl = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "modelUrl":
                        modelUrl = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "modelBinaryUrl":
                        modelBinaryUrl = MyJsonTool.getStringValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class Attributes1
    {
        public List<string> free;//Product-
        public string id;//Product-
        public string name;//Product-
        public List<AttributesValue> values;//Product-

        internal void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "free":
                        free = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "name":
                        name = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "values":
                        {
                            object obj = MyJsonTool.getValue(dic, key);
                            if (obj == null)
                            {
                                values = null;
                            }
                            else {
                                values = new List<AttributesValue>();
                                List<object> list = obj as List<object>;
                                for (int i = 0; i < list.Count; i++)
                                {
                                    AttributesValue value = new AttributesValue();
                                    value.Deserialize(list[i] as Dictionary<string, object>);
                                    values.Add(value);
                                }
                            }
                        }
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class AttributesValue
    {
        public string id;//Product-
        public string value;//Product-

        internal void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "value":
                        value = MyJsonTool.getStringValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
}

