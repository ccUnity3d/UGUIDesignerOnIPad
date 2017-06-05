using UnityEngine;
using System.Collections;

public class MaterialItemData : ItemData {

    public static int start = 0;
    public bool special = false;
    public MaterialItemData()
    {
        id = start;
        start++;
    }
    public MaterialItemData(string id,string seekId, string textureURL) 
    {
        this.stringId = id;
        this.seekId  = seekId;
        this.textureURL = textureURL;
    }

    public string stringId = "0";
    public string seekId = "local";
    public string textureURL = "3D/texture2D/defaultfloor.assetbundle";


}
