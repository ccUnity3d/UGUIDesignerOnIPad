using UnityEngine;
using System.Collections;

public class SizeState : MainPageState
{

    public const string Name = "SizeState";
    public override void enter()
    {
        base.enter();
        mainpage.middleLine.gameObject.SetActive(true);
        inputMachine.setState(SetFirstPointState.NAME);
    }

    public override void exit()
    {
        mainpage.middleLine.gameObject.SetActive(false);
        base.exit();
    }
}
