using UnityEngine;
using System.Collections;

public class HoldOnState2D : InputState2D {

    public const string NAME = "HoldOnState2D";

    public HoldOnState2D() : base(NAME)
    {

    }


    public override void enter()
    {
        base.enter();
        RefreshView();
    }
}
