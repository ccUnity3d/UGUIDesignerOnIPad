using System.Collections.Generic;

public class SelectWall_HideState : SelectWall_State
{
    public override void enter()
    {
        base.enter();
        if (target != null)
        {
            target.hide = true;
            RefreshView();
            inputMachine.setState(FreeState2D.NAME);
            //machine.setState(EditTypeOnSelect.Free);
        }
    }

    public override void exit()
    {
        base.exit();
    }

}

