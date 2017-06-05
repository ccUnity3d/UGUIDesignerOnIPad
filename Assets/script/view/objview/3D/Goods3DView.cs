using UnityEngine;
using System.Collections;

public class Goods3DView : ObjView
{
    public ProductData data;

    public override void SetData(ObjData data)
    {
        base.SetData(data);
        this.data = data as ProductData;
    }

    public override string GetState()
    {
        return SelectGoodsState3D.NAME;
    }

}
