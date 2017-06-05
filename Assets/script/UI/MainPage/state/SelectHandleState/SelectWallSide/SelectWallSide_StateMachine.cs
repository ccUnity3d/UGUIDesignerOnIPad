using UnityEngine;
using System.Collections;
using System;

public class SelectWallSide_StateMachine : SelectStateMachine<SelectWallSide_State, SelectWallSide_StateMachine>, IStateMachine
{
    public SelectWallSideState3D selectWallSideState3D;

    public SelectWallSide_StateMachine()
    {

    }

    public override void Inject()
    {
        addState(EditTypeOnSelect.Free, new SelectWallSide_FreeState());
        addState(EditTypeOnSelect.Rotate, new SelectWallSide_RotateState());
        addState(EditTypeOnSelect.UseToAllSide, new SelectWallSide_UseToAllSideState());
        addState(EditTypeOnSelect.SetTLine, new SelectWallSide_TLineState());
        addState(EditTypeOnSelect.SetTopLine, new SelectWallSide_TopLineState());
    }

    
}
