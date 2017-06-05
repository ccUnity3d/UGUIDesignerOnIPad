using UnityEngine;
using System.Collections;
using System;

public class CeilingView : ObjView, IMaterialView
{
    public MaterialData data;

    public override void SetData(ObjData data)
    {
        base.SetData(data);
        this.data = data as MaterialData;
    }

    public override string GetState()
    {
        return SelectCeilingState3D.NAME;
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
