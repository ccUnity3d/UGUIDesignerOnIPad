using UnityEngine;

public class CloseState : InputState2D
{
    public const string NAME = "CloseState";
    public CloseState() : base(NAME)
    {

    }

    public override void enter()
    {
        base.enter();
    }

    public override void mUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            setState(FreeState2D.NAME);
        }
    }

    public override void exit()
    {
        base.exit();
    }

}