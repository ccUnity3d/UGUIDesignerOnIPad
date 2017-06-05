using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SelectPoint_StateMachine : SelectStateMachine<SelectPoint_State, SelectPoint_StateMachine>
{
    public SelectPointState selectPointState;
   
    public SelectPoint_StateMachine()
    {

    }

    public override void Inject()
    {
        addState(EditTypeOnSelect.Free, new SelectPoint_FreeState());
        addState(EditTypeOnSelect.Delet, new SelectPoint_DeletState());
    }
}
