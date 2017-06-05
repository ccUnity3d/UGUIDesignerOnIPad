using UnityEngine;
using System.Collections;

public class UndoState : MainPageState
{
    public const string Name = "UndoState";
    public override void enter()
    {
        base.enter();
        undoHelper.undo();
        setState(MainPageFreeState.Name);
    }

    public override void exit()
    {
        base.exit();
    }

}
