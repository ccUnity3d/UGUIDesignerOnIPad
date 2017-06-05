using UnityEngine;
using System.Collections;

public class RedoState : MainPageState
{
    public const string Name = "RedoState";
    public override void enter()
    {
        base.enter();
        undoHelper.redo();
        RefreshView();
        setState(MainPageFreeState.Name);
    }
   
    public override void exit()
    {
        base.exit();
    }
}
