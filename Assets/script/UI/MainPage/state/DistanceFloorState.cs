using UnityEngine;
using System.Collections;

public class DistanceFloorState : MainPageState {

    public static string Name = "DistanceFloor";
    // 距离店面 高度
    private ToggleButton toggleButton;
    private OptionsPage optionsPage { get { return OptionsPage.Instance; } }

   
    public override void enter()
    {
        if (toggleButton.onDown == false )
        {
            setState(MainPageFreeState.Name);
            return;
        }
        Debug.Log(optionsPage.disFloorUnit.text);
      
        UITool.SetActionTrue(optionsPage.RectDisFloorValue.gameObject);
    }

    public override void exit()
    {
        UITool.SetActionFalse(optionsPage.RectDisFloorValue.gameObject);
        toggleButton.onDown = false;
    }

    public override void Ready()
    {
        base.Ready();
        toggleButton = optionsPage.disFloor;
    }
}
