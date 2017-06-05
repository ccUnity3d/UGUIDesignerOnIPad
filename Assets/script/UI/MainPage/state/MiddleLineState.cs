using UnityEngine;
using System.Collections;

public class MiddleLineState : MainPageState {

    public const string Name = "MiddleLineState";
    private ToggleButton middleLine;

    public override void enter()
    {
        base.enter();
        if (middleLine.onDown == false)
        {
            setState(MainPageFreeState.Name);
            return;
        }
        
        //inputMachine.inputWallType = InputWallType.Mid;
        inputMachine.setState(SetFirstPointState.NAME);
    }

    public override void Ready()
    {
        base.Ready();
        middleLine = mainpage.middleLine;
    }

    public override void exit()
    {
        inputMachine.setState(FreeState2D.NAME);
        middleLine.onDown = false;
        base.exit();
    }

}
