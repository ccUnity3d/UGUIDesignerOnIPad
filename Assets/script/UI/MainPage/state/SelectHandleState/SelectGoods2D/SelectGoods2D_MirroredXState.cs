using UnityEngine;
using System.Collections;

public class SelectGoods2D_MirroredXState : SelectGoods2D_State
{
    public override void enter()
    {
        base.enter();
        save();
        target.scale.x = -target.scale.x;
        target.rotate = -target.rotate;
        RefreshView();
        setState(EditTypeOnSelect.Free);
    }

    public override void exit()
    {
        base.exit();
    }
}
