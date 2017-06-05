using UnityEngine;
using System.Collections;

public class SelectGoods2D_DeletState : SelectGoods2D_State
{
    public override void enter()
    {
        base.enter();
        save();
        if (target != null)
        {
            data.RemoveProduct(target);
            RefreshView();
        }
        inputMachine.setState(FreeState2D.NAME);
    }

    public override void exit()
    {
        base.exit();
    }
}
