using UnityEngine;
using System.Collections;

public class InnerLineState : MainPageState {

    public static string Name = "InnerLineState";
    private ToggleButton innerLine;

    public override void enter()
    {
        base.enter();
        if (innerLine.onDown == false)
        {
            setState(MainPageFreeState.Name);
            return;
        }

        //inputMachine.inputWallType = InputWallType.Side;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            inputMachine.setState(SetFirstPointState.NAME);
        }
        else {
            inputMachine.setState(SetInnerFirstPointState.NAME);
        }
    }

    public override void Ready()
    {
        base.Ready();
        innerLine = mainpage.innerLine;
        
    }

    public override void exit()
    {
        inputMachine.setState(FreeState2D.NAME);
        innerLine.onDown = false;
        base.exit();
    }
}
