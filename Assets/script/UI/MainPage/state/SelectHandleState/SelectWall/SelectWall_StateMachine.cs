using UnityEngine;
using System.Collections.Generic;

public class SelectWall_StateMachine : SelectStateMachine<SelectWall_State, SelectWall_StateMachine>
{
    public SelectWallState selectWallState;
   
    public string inputWidth = "";
    public string inputLength = "";
    
    /// <summary>
    /// 0.自身长度  1.room1的下一个 2.room1的上一个 3.room2的下一个 4.room2.的上一个
    /// </summary>
    public int OnRoomNearWallIndex = 0;

    public SelectWall_StateMachine()
    {

    }

    public override void Inject()
    {
        addState(EditTypeOnSelect.Free, new SelectWall_FreeState());
        addState(EditTypeOnSelect.Delet, new SelectWall_DeletState());
        addState(EditTypeOnSelect.Hide, new SelectWall_HideState());
        addState(EditTypeOnSelect.WallWidth, new SelectWall_WidthState());
        addState(EditTypeOnSelect.WallLength, new SelectWall_LengthState());
    }
}

