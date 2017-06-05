using UnityEngine;
using System.Collections;

public class QueryState : MainPageState
{
    public const string Name = "QueryState";
    
    public override void enter()
    {
        base.enter();
        //Application.Quit();
        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(FreeState2D.NAME);
        }
        else
        {
            inputMachine.setState(FreeState3D.NAME);
        }
    }


    public override void exit()
    {
        base.exit();
    }

}
