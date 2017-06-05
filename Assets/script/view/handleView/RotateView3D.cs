using UnityEngine;
using System.Collections;

public class RotateView3D : HandleView {

    public override bool SetState(IStateMachine selectMachine)
    {
        selectMachine.setState(EditTypeOnSelect.Rotate);
        return true;
    }
}
