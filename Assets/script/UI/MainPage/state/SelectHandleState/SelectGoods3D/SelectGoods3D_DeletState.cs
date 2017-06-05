using UnityEngine;
using System.Collections;

public class SelectGoods3D_DeletState : SelectGoods3D_State
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
        inputMachine.setState(FreeState3D.NAME);
    }

    public override void exit()
    {
        base.exit();
    }
}
