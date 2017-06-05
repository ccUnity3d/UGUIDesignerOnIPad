using UnityEngine;
using System.Collections;

public class SelectPoint_DeletState : SelectPoint_State {

    public override void enter()
    {
        base.enter();
        if (target != null)
        {
            data.RemovePoint(target);
            roomfunc.ForceRefreshRoomData(data);
            RefreshView();
        }
        inputMachine.setState(FreeState2D.NAME);
    }

    public override void exit()
    {
        base.exit();
    }

}
