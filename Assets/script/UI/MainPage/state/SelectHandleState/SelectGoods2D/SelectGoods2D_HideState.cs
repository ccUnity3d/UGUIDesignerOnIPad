using UnityEngine;
using System.Collections;

public class SelectGoods2D_HideState : SelectGoods2D_State
{
     
    public override void enter()
    {
        base.enter();
        save();
        target.hide = true;
        RefreshView();
        inputMachine.setState(FreeState2D.NAME);
    }

    public override void exit()
    {
        base.exit();
    }
}
