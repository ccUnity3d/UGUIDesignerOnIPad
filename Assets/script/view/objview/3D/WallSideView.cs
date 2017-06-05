using UnityEngine;
using System.Collections;
using System;

public class WallSideView : ObjView, IMaterialView {

    public WallSideData data;

    public override void SetData(ObjData data)
    {
        base.SetData(data);
        this.data = data as WallSideData;
    }

    public override string GetState()
    {
        return SelectWallSideState3D.NAME;
    }

    public MaterialData getdata()
    {
        return data.materialData;
    }

    public void setdata(MaterialData data)
    {
        this.data.materialData = data;
    }
}
