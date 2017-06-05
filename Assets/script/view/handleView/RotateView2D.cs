using UnityEngine;
using System.Collections;

public class RotateView2D : HandleView {
    public override bool SetState(IStateMachine selectMachine)
    {
        selectMachine.setState(EditTypeOnSelect.Rotate);
        return true;
    }
}
