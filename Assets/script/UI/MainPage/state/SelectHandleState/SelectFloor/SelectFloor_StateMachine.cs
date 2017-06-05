using UnityEngine;
using System.Collections;

public class SelectFloor_StateMachine : SelectStateMachine<SelectFloor_State, SelectFloor_StateMachine>, IStateMachine
{
    public SelectFloorState3D selectFloorState3D;

    public SelectFloor_StateMachine()
    {

    }

    public override void Inject()
    {
        addState(EditTypeOnSelect.Free, new SelectFloor_FreeState());
        addState(EditTypeOnSelect.Rotate, new SelectFloor_RotateState());
    }

}
