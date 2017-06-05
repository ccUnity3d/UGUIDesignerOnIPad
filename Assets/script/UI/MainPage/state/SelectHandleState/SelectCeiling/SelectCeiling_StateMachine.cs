using UnityEngine;
using System.Collections;

public class SelectCeiling_StateMachine : SelectStateMachine<SelectCeiling_State, SelectCeiling_StateMachine>, IStateMachine
{
    public SelectCeilingState3D selectCeilingState3D;

    public SelectCeiling_StateMachine()
    {

    }

    public override void Inject()
    {
        addState(EditTypeOnSelect.Free, new SelectCeiling_FreeState());
        addState(EditTypeOnSelect.Rotate, new SelectCeiling_RotateState());
    }
}
