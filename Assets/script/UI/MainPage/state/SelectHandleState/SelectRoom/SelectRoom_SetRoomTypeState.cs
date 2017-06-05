using UnityEngine;
using System.Collections;

public class SelectRoom_SetRoomTypeState : SelectRoom_State
{
    public override void enter()
    {
        base.enter();
        if (target != null)
        {
            if (target.type != machine.roomType)
            {
                undoHelper.save();
                target.type = machine.roomType;
                RefreshView();
            }
        }
        setState(EditTypeOnSelect.Free);
    }

    public override void exit()
    {
        base.exit();
    }
}
