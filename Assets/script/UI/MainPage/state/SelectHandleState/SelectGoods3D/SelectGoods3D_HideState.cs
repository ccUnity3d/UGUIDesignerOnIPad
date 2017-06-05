using UnityEngine;
using System.Collections;

public class SelectGoods3D_HideState : SelectGoods3D_State
{
    public override void enter()
    {
        base.enter();
        save();
        machine.selectGoods3DState.target.hide = true;
        RefreshView();
        inputMachine.setState(FreeState3D.NAME);
    }

    public override void exit()
    {
        base.exit();
    }
}
