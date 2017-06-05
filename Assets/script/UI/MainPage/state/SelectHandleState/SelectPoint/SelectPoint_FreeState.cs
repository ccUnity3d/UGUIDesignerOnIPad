using UnityEngine;
using System.Collections;

public class SelectPoint_FreeState : SelectPoint_State {

    //private Button button;

    public override void Ready(GameObject optionSkin)
    {
        base.Ready(optionSkin);
        //button = UITool.GetUIComponent<Button>(skin.transform, "");
    }

    public override void enter()
    {
        base.enter();
    }

    public override void exit()
    {
        base.exit();
    }
}
