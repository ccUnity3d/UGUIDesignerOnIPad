using UnityEngine;
using System.Collections.Generic;

public class SelectRoom_StateMachine : SelectStateMachine<SelectRoom_State, SelectRoom_StateMachine>
{
    public SelectRoomState selectRoomState;
   
    /// <summary>
    /// 
    /// </summary>
    public string roomType = "";
    
    public SelectRoom_StateMachine()
    {

    }

    public override void Inject()
    {
        addState(EditTypeOnSelect.Free, new SelectRoom_FreeState());
        addState(EditTypeOnSelect.Delet, new SelectRoom_DeletState());
        addState(EditTypeOnSelect.SetRoomType, new SelectRoom_SetRoomTypeState());
    }
}

