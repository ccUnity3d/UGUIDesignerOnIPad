using UnityEngine;
using System.Collections;

public class GoodsView : ObjView {

    public ProductData data;
   
    public override void SetData(ObjData data)
    {
        base.SetData(data);
        this.data = data as ProductData;
    }

    public override string GetState()
    {
        return SelectGoodsState2D.NAME;
    }



}
