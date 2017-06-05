using UnityEngine;
using System.Collections;
using System;

public class FloorView : ObjView, IMaterialView
{
    public MaterialData data;

    public override void SetData(ObjData data)
    {
        base.SetData(data);
        this.data = data as MaterialData;

        if (this.data.seekId == "local")
        {
            string uri = this.data.textureURI;
            //SimpleInnerLoader innerLoader = new SimpleInnerLoader(uri, SimpleLoadDataType.texture2D, onloaded, null);
        }
        else
        {
            string url = this.data.textureURI;
            //SimpleOutterLoader outLoader
        }
    }

    private void onloaded(object obj)
    {
        throw new NotImplementedException();
    }

    public override string GetState()
    {
        return SelectFloorState3D.NAME;
    }

    public MaterialData getdata()
    {
        return data;
    }

    public void setdata(MaterialData data)
    {
        this.data = data;
    }
}
