using UnityEngine;
using System.Collections;

public class SelectGoods2D_CopyState : SelectGoods2D_State
{
    private Vector3 offset = new Vector3(1, 0, -1);
    public override void enter()
    {
        base.enter();
        save();
        data.AddProduct(target.id, target.position + offset, target.rotate, targetProduct, null, target.type);
        RefreshView();
        setState(EditTypeOnSelect.Free);
    }

    public override void exit()
    {
        base.exit();
    }
}
