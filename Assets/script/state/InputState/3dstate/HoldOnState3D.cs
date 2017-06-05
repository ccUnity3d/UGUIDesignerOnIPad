using UnityEngine;
using System.Collections;

public class HoldOnState3D : InputState
{
    public const string NAME = "HoldOnState3D";
    public HoldOnState3D() : base(NAME)
    {

    }

    public override void enter()
    {
        base.enter();
        RefreshView();
    }
}
