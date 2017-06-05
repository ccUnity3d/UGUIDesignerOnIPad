using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 方案数据
/// </summary>
[System.Serializable]
public class JsonProjectData
{
    public JsonMeta meta = new JsonMeta();
    public List<JsonModelData> data = new List<JsonModelData>();
    public List<JsonProductData> products = new List<JsonProductData>();


    [System.Serializable]
    public class JsonMeta
    {
        public string magic = "11111111a";
        public string version = "0.1";
        public string unit = "meter";
        public string keywords = "Homestyler Ipad designer editor";

        internal void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "magic":
                        magic = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "version":
                        version = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "unit":
                        unit = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "keywords":
                        keywords = MyJsonTool.getStringValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }

    #region ModelData
    [System.Serializable]
    public abstract class JsonModelData
    {
        public string Class;
        public string id = "";
        public int flag = 0;
        public List<string> parents = new List<string>();
        public List<string> children = new List<string>();
        public object userDefined = default(object);

        internal abstract void Deserialize(Dictionary<string, object> dic);
    }
    [System.Serializable]
    public class Floorplan : JsonModelData
    {
        public Floorplan()
        {
            Class = "hsw.model.Floorplan";
        }
        public string GUID = "8f3f4b94-c358-4a95-bef4-ac8601134896";
        public string active_camera = "3367";
        public string underlay = "2259";
        public float global_wall_height3d = 2.6f;
        public float globalWallWidth = 0.1524f;
        public string displayLengthUnit = "m";
        public float displayLengthPrecisionDigits = 2;
        public string displayAreaUnit = "m";
        public float displayAreaPrecisionDigits = 2;
        public int environmentColor = 6645093;

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "GUID":
                        GUID = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "active_camera":
                        active_camera = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "underlay":
                        underlay = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "global_wall_height3d":
                        global_wall_height3d = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "globalWallWidth":
                        globalWallWidth = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "displayLengthUnit":
                        displayLengthUnit = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "displayLengthPrecisionDigits":
                        displayLengthPrecisionDigits = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "displayAreaUnit":
                        displayAreaUnit = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "displayAreaPrecisionDigits":
                        displayAreaPrecisionDigits = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "environmentColor":
                        environmentColor = MyJsonTool.getIntValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Room : JsonModelData
    {
        public Room()
        {
            Class = "hsw.model.Room";
        }
        public string root = "2367";
        public string floorMaterial = "2667";
        public string pattern = "Horizontal";
        public float ceilingHeight3d = 2.6f;
        public string ceilingMaterial = "2668";

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "root":
                        root = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "floorMaterial":
                        floorMaterial = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "pattern":
                        pattern = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "ceilingHeight3d":
                        ceilingHeight3d = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ceilingMaterial":
                        ceilingMaterial = MyJsonTool.getStringValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class CoWall : JsonModelData
    {
        public CoWall()
        {
            Class = "hsw.model.CoWall";
        }
        public string edge = "2313";
        public bool reversed = true;
        public string next = "2368";
        public string prev = "2373";
        public List<string> moldings = new List<string>();
        public List<string> materials = new List<string>();

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "edge":
                        edge = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "reversed":
                        reversed = MyJsonTool.getBoolValue(dic, key);
                        break;
                    case "next":
                        next = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "prev":
                        prev = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "moldings":
                        moldings = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "materials":
                        materials = MyJsonTool.getStringListValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Wall : JsonModelData
    {
        public Wall()
        {
            Class = "hsw.model.Wall";
        }
        public string from = "2260";
        public string to = "2261";
        public string coedge = "2367";
        public float width = 0.1524f;
        public float height3d = 2.6f;
        public string wallType = "generic";
        public bool isLoadBearing = false;

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "from":
                        from = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "to":
                        to = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "coedge":
                        coedge = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "width":
                        width = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "height3d":
                        height3d = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "wallType":
                        wallType = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "isLoadBearing":
                        isLoadBearing = MyJsonTool.getBoolValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Point : JsonModelData
    {
        public Point()
        {
            Class = "hsw.model.Point";
        }
        public float x = -33.6227f;
        public float y = 0.5843f;

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "x":
                        x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "y":
                        y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Content : JsonModelData
    {
        public Content()
        {
            Class = "hsw.model.Content";
        }
        public float x = -33.2532f;
        public float y = 1.7f;
        public float z = 0;
        public float rotation = 270;
        public float XScale = 1;
        public float YScale = 1;
        public float ZScale = 1;
        public float XLength = 0.7547f;
        public float YLength = 0.5865f;
        public float ZLength = 0.6794f;
        public string seekId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string variationId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string type = "seating";
        public string unit = "cm";
        public string topView = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/Top.png";
        public string modelTexture = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model-fc.png";
        public string model3d = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model.gz.bobj";
        public string _host = "2313";

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "x":
                        x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "y":
                        y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "z":
                        z = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "rotation":
                        rotation = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XScale":
                        XScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YScale":
                        YScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZScale":
                        ZScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XLength":
                        XLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YLength":
                        YLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZLength":
                        ZLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "seekId":
                        seekId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "variationId":
                        variationId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "type":
                        type = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "unit":
                        unit = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "topView":
                        topView = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "modelTexture":
                        modelTexture = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "model3d":
                        model3d = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "_host":
                        _host = MyJsonTool.getStringValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Molding : JsonModelData
    {
        public Molding()
        {
            Class = "hsw.model.Molding";
        }
        public string seekId = "molding_baseboard_m3";
        public string type = "baseboard";
        public float height = 0.08f;
        public float offset = 0;
        public bool autoFit = true;
        public float thickness = 0.014f;
        public object shape = null;
        public List<string> materials = new List<string>();

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "seekId":
                        seekId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "type":
                        type = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "height":
                        height = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "offset":
                        offset = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "autoFit":
                        autoFit = MyJsonTool.getBoolValue(dic, key);
                        break;
                    case "thickness":
                        thickness = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "shape":
                        shape = MyJsonTool.getValue(dic, key);
                        break;
                    case "materials":
                        materials = MyJsonTool.getStringListValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Material : JsonModelData
    {
        public Material()
        {
            Class = "hsw.model.Material";
        }
        public string seekId = "local";
        public string textureURI = "res/contents/moldings/Baseboard_wood_white_diffuse_1.jpg";
        public int color = 16777215;
        public string name = "0001390001097";
        public List<string> categoryId = new List<string>();
        public float tileSize_x = 2.9683f;
        public float tileSize_y = 2.6f;

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "seekId":
                        seekId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "textureURI":
                        textureURI = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "color":
                        color = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "name":
                        name = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "categoryId":
                        categoryId = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "tileSize_x":
                        tileSize_x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "tileSize_y":
                        tileSize_y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Door : JsonModelData
    {
        public Door()
        {
            Class = "hsw.model.Door";
        }
        public float x = -33.2532f;
        public float y = 1.7f;
        public float z = 0;
        public float rotation = 270;
        public float XScale = 1;
        public float YScale = 1;
        public float ZScale = 1;
        public float XLength = 0.7547f;
        public float YLength = 0.5865f;
        public float ZLength = 0.6794f;
        public string seekId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string variationId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string type = "seating";
        public string unit = "cm";
        public string topView = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/Top.png";
        public string modelTexture = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model-fc.png";
        public string model3d = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model.gz.bobj";
        public string _host = "2313";

        public string profile = "M0.449863, 0 L0.449863, 2.1 L-0.449863, 2.1 L-0.449863, 0 L0.449863, 0";
        public int swing = 3;

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "x":
                        x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "y":
                        y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "z":
                        z = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "rotation":
                        rotation = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XScale":
                        XScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YScale":
                        YScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZScale":
                        ZScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XLength":
                        XLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YLength":
                        YLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZLength":
                        ZLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "seekId":
                        seekId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "variationId":
                        variationId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "type":
                        type = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "unit":
                        unit = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "topView":
                        topView = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "modelTexture":
                        modelTexture = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "model3d":
                        model3d = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "_host":
                        _host = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "profile":
                        profile = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "swing":
                        swing = MyJsonTool.getIntValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Window : JsonModelData
    {
        public Window()
        {
            Class = "hsw.model.Window";
        }
        public float x = -33.2532f;
        public float y = 1.7f;
        public float z = 0;
        public float rotation = 270;
        public float XScale = 1;
        public float YScale = 1;
        public float ZScale = 1;
        public float XLength = 0.7547f;
        public float YLength = 0.5865f;
        public float ZLength = 0.6794f;
        public string seekId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string variationId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string type = "seating";
        public string unit = "cm";
        public string topView = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/Top.png";
        public string modelTexture = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model-fc.png";
        public string model3d = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model.gz.bobj";
        public string _host = "2313";

        public string profile = "M0.449863, 0 L0.449863, 2.1 L-0.449863, 2.1 L-0.449863, 0 L0.449863, 0";
        public int swing = 3;

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "x":
                        x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "y":
                        y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "z":
                        z = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "rotation":
                        rotation = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XScale":
                        XScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YScale":
                        YScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZScale":
                        ZScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XLength":
                        XLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YLength":
                        YLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZLength":
                        ZLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "seekId":
                        seekId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "variationId":
                        variationId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "type":
                        type = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "unit":
                        unit = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "topView":
                        topView = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "modelTexture":
                        modelTexture = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "model3d":
                        model3d = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "_host":
                        _host = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "profile":
                        profile = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "swing":
                        swing = MyJsonTool.getIntValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Hole : JsonModelData
    {
        public Hole()
        {
            Class = "hsw.model.Hole";
        }
        public float x = -33.2532f;
        public float y = 1.7f;
        public float z = 0;
        public float rotation = 270;
        public float XScale = 1;
        public float YScale = 1;
        public float ZScale = 1;
        public float XLength = 0.7547f;
        public float YLength = 0.5865f;
        public float ZLength = 0.6794f;
        public string seekId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string variationId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string type = "seating";
        public string unit = "cm";
        public string topView = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/Top.png";
        public string modelTexture = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model-fc.png";
        public string model3d = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model.gz.bobj";
        public string _host = "2313";

        public string profile = "M0.449863, 0 L0.449863, 2.1 L-0.449863, 2.1 L-0.449863, 0 L0.449863, 0";
        public int swing = 3;
        public object paramData = null;

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "x":
                        x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "y":
                        y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "z":
                        z = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "rotation":
                        rotation = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XScale":
                        XScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YScale":
                        YScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZScale":
                        ZScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XLength":
                        XLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YLength":
                        YLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZLength":
                        ZLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "seekId":
                        seekId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "variationId":
                        variationId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "type":
                        type = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "unit":
                        unit = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "topView":
                        topView = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "modelTexture":
                        modelTexture = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "model3d":
                        model3d = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "_host":
                        _host = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "profile":
                        profile = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "swing":
                        swing = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "paramData":
                        paramData = MyJsonTool.getValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Group : JsonModelData
    {
        public Group()
        {
            Class = "hsw.model.Group";
        }
        public float x = -33.2532f;
        public float y = 1.7f;
        public float z = 0;
        public float rotation = 270;
        public float XScale = 1;
        public float YScale = 1;
        public float ZScale = 1;
        public float XLength = 0.7547f;
        public float YLength = 0.5865f;
        public float ZLength = 0.6794f;
        public string seekId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string variationId = "9026b8e9-5aac-4fa9-be7a-54ede2250ac4";
        public string type = "seating";
        public string unit = "cm";
        public string topView = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/Top.png";
        public string modelTexture = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model-fc.png";
        public string model3d = "http://s3.juran.cn/juran-prod-assets/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/model.gz.bobj";
        public string _host = "2313";

        public string profile = "M0.449863, 0 L0.449863, 2.1 L-0.449863, 2.1 L-0.449863, 0 L0.449863, 0";
        public int swing = 3;
        public object paramData = null;
        private List<string> members = new List<string>();

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "x":
                        x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "y":
                        y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "z":
                        z = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "rotation":
                        rotation = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XScale":
                        XScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YScale":
                        YScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZScale":
                        ZScale = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "XLength":
                        XLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YLength":
                        YLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZLength":
                        ZLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "seekId":
                        seekId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "variationId":
                        variationId = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "type":
                        type = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "unit":
                        unit = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "topView":
                        topView = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "modelTexture":
                        modelTexture = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "model3d":
                        model3d = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "_host":
                        _host = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "profile":
                        profile = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "swing":
                        swing = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "members":
                        members = MyJsonTool.getStringListValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Camera : JsonModelData
    {
        public Camera()
        {
            Class = "hsw.model.Camera";
        }
        public float x = -24.3101f;
        public float y = 5.1325f;
        public float target_x = -22.8543f;
        public float target_y = 4.6181f;
        public float horizontal_fov = 60f;
        public float z = 1.4f;
        public float pitch = -1.1102f;
        public string type = "firstperson";

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "x":
                        x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "y":
                        y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "z":
                        z = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "target_x":
                        target_x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "target_y":
                        target_y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "horizontal_fov":
                        horizontal_fov = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "pitch":
                        pitch = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "type":
                        type = MyJsonTool.getStringValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    [System.Serializable]
    public class Underlay : JsonModelData
    {
        public Underlay()
        {
            Class = "hsw.model.Underlay";
        }
        public float x = -24.3101f;
        public float y = 5.1325f;
        public float width = 2555.6957f;
        public float height = 1916.7717f;
        public string url = "http://3d.juran.cn/img/54981011498e905d7d01e3bd.img";
        public bool enabled = false;
        public bool show = false;

        internal override void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "Class":
                        Class = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "flag":
                        flag = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "parents":
                        parents = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "children":
                        children = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "userDefined":
                        userDefined = MyJsonTool.getValue(dic, key);
                        break;
                    case "x":
                        x = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "y":
                        y = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "width":
                        width = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "height":
                        height = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "url":
                        url = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "enabled":
                        enabled = MyJsonTool.getBoolValue(dic, key);
                        break;
                    case "show":
                        show = MyJsonTool.getBoolValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }
    #endregion
    [System.Serializable]
    public class JsonProductData
    {
        public string id = "4abd1bba-6d61-481e-9008-25295599e006";
        public string seekId = "4abd1bba-6d61-481e-9008-25295599e006";
        public string name = "VANILLA RVUG1";
        public string unit = "cm";
        public string contentType = "door/single swing door";
        public string entityType = "door";
        public string productType = "model";
        public string thumbnail = "http=//cf-hsm-prod-assets.homestyler.com/i/4abd1bba-6d61-481e-9008-25295599e006/resized/iso0_w160_h160.jpg";
        public List<string> imagesResize = new List<string>() { "http://juran-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/resized/iso_w160_h160.jpg" };
        public List<string> images = new List<string>() { "http=//cf-hsm-prod-assets.homestyler.com/i/4abd1bba-6d61-481e-9008-25295599e006/iso.jpg" };
        public string t = "";
        public List<string> categories = new List<string>() { "2afb28e9-3b85-4ce4-b826-1eff97886de4", "2ffc642e-0042-48b8-b3f0-3e9191011a0a" };
        public List<string> families = new List<string>();
        public string v = "Barausse";
        public string vu = "https=//hsm-prod-assets.s3.amazonaws.com/vendorlogos/temp-54858eeb-fe24-4073-aaf6-83536936b83a.jpg";
        public string productURL = "http=//www.barausse.com/portal/en/international/products/interior-doors/line-index/VANILLA/VANILLA%20RVUG1/";
        public string price = "490.00";
        public string sku = "zhibangchugui_014";
        public int status = 1;
        public float XLength = 95.4517f;
        public float YLength = 17.7819f;
        public float ZLength = 208.834f;
        public string modelTexture = "http=//hsm-prod-assets.s3.amazonaws.com/i/4abd1bba-6d61-481e-9008-25295599e006/model-fc.png";
        public string model3d = "http=//hsm-prod-assets.s3.amazonaws.com/i/4abd1bba-6d61-481e-9008-25295599e006/model.gz.bobj";
        public string top = "https=//hsm-prod-assets.s3.amazonaws.com/svg/swinging_door.svg";
        public List<string> variations = new List<string>();
        public string variationId = "";
        public bool isScalable = false;
        public List<string> bomcategoryId = new List<string>();
        public string snapPlaneUrl = "http://juran-prod-assets.s3.cn-north-1.amazonaws.com.cn/i/9026b8e9-5aac-4fa9-be7a-54ede2250ac4/plane.obj";
        public List<string> components = new List<string>();
        public float defaultHeight = 0;
        public bool hasPocket = true;
        public object snapPlaneMesh = null;
        public string profile = "M0.9,0 L0.9,2.2 L-0.9,2.2 L-0.9,0 L0.9,0";
        public object profileConfig = null;
        public float elevation = 0f;
        public string assemblyType = "wall_attachment";
        public List<string> products = new List<string>();
        public object productDataById = null;

        internal void Deserialize(Dictionary<string, object> dic)
        {
            foreach (string key in dic.Keys)
            {
                switch (key)
                {
                    case "id":
                        id = MyJsonTool.getStringValue(dic, key);
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
                    case "contentType":
                        contentType = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "entityType":
                        entityType = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "productType":
                        productType = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "thumbnail":
                        thumbnail = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "imagesResize":
                        imagesResize = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "images":
                        images = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "t":
                        t = MyJsonTool.getStringValue(dic, key);
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
                    case "price":
                        price = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "sku":
                        sku = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "status":
                        status = MyJsonTool.getIntValue(dic, key);
                        break;
                    case "XLength":
                        XLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "YLength":
                        YLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "ZLength":
                        ZLength = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "modelTexture":
                        modelTexture = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "model3d":
                        model3d = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "top":
                        top = MyJsonTool.getStringValue(dic, key);
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
                    case "bomcategoryId":
                        bomcategoryId = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "snapPlaneUrl":
                        snapPlaneUrl = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "components":
                        components = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "defaultHeight":
                        defaultHeight = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "hasPocket":
                        hasPocket = MyJsonTool.getBoolValue(dic, key);
                        break;
                    case "snapPlaneMesh":
                        snapPlaneMesh = MyJsonTool.getValue(dic, key);
                        break;
                    case "profile":
                        profile = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "profileConfig":
                        profileConfig = MyJsonTool.getValue(dic, key);
                        break;
                    case "elevation":
                        elevation = MyJsonTool.getFloatValue(dic, key);
                        break;
                    case "assemblyType":
                        assemblyType = MyJsonTool.getStringValue(dic, key);
                        break;
                    case "products":
                        products = MyJsonTool.getStringListValue(dic, key);
                        break;
                    case "productDataById":
                        productDataById = MyJsonTool.getValue(dic, key);
                        break;
                    default:
                        Debug.LogError("Json " + this.GetType().Name + " 里的 key = " + key + " 未实现！");
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 反序列化
    /// </summary>
    /// <param name="obj"></param>
    internal void Deserialize(Dictionary<string, object> dic)
    {
        foreach (string key in dic.Keys)
        {
            switch (key)
            {
                case "meta":
                    {
                        Dictionary<string, object> metaDic = dic["meta"] as Dictionary<string, object>;
                        meta.Deserialize(metaDic);
                    }
                    break;
                case "data":
                    {
                        object obj = dic["data"];
                        List<object> arr = obj as List<object>;
                        for (int i = 0; i < arr.Count; i++)
                        {
                            Dictionary<string, object> dici = arr[i] as Dictionary<string, object>;
                            JsonModelData modeldata = null;
                            if (dici.ContainsKey("Class"))
                            {
                                string typename = dici["Class"].ToString();
                                switch (typename)
                                {
                                    case "hsw.model.Floorplan":
                                        modeldata = new Floorplan();
                                        break;
                                    case "hsw.model.Room":
                                        modeldata = new Room();
                                        break;
                                    case "hsw.model.CoWall":
                                        modeldata = new CoWall();
                                        break;
                                    case "hsw.model.Wall":
                                        modeldata = new Wall();
                                        break;
                                    case "hsw.model.Point":
                                        modeldata = new Point();
                                        break;
                                    case "hsw.model.Content":
                                        modeldata = new Content();
                                        break;
                                    case "hsw.model.Molding":
                                        modeldata = new Molding();
                                        break;
                                    case "hsw.model.Material":
                                        modeldata = new Material();
                                        break;
                                    case "hsw.model.Door":
                                        modeldata = new Door();
                                        break;
                                    case "hsw.model.Camera":
                                        modeldata = new Camera();
                                        break;
                                    case "hsw.model.Underlay":
                                        modeldata = new Underlay();
                                        break;
                                    case "hsw.model.Window":
                                        modeldata = new Window();
                                        break;
                                    case "hsw.model.Hole":
                                        modeldata = new Hole();
                                        break;
                                    case "hsw.model.Group":
                                        modeldata = new Group();
                                        break;
                                    default:
                                        Debug.Log(typename + " == null");
                                        continue;
                                }
                                modeldata.Deserialize(dici);
                            }
                            else { Debug.Log(i + " ContainsKey Class == false"); }
                            data.Add(modeldata);
                        }
                    }
                    break;
                case "products":
                    {
                        List<object> arr = dic["products"] as List<object>;
                        for (int i = 0; i < arr.Count; i++)
                        {
                            JsonProductData productData = new JsonProductData();
                            Dictionary<string, object> dici = arr[i] as Dictionary<string, object>;
                            productData.Deserialize(dici);
                            products.Add(productData);
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
