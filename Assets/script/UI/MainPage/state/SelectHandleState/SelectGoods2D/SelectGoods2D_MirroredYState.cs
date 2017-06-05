﻿using UnityEngine;
using System.Collections;

public class SelectGoods2D_MirroredYState : SelectGoods2D_State
{
    public override void enter()
    {
        base.enter();
        save();
        target.scale.z = -target.scale.z;
        target.rotate = 360 - target.rotate;
        RefreshView();
        setState(EditTypeOnSelect.Free);
    }

    public override void exit()
    {
        base.exit();
    }
}
