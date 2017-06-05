using UnityEngine;
using System.Collections;

public class SelectWall_WidthState : SelectWall_State {

    public override void enter()
    {
        base.enter();
        float length = float.Parse(machine.inputWidth) / 1000f;
        if (target.width != length)
        {
            machine.selectWallState.save();
            target.width = length;
            RefreshView();
        }
    }

    public override void exit()
    {
        base.exit();
    }

}
