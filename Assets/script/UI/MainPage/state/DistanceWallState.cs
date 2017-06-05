using UnityEngine;
using System.Collections;

public class DistanceWallState : MainPageState {

    public static string Name = "DistanceWall";
    private ToggleButton toggleButton;
    private OptionsPage optionsPage { get { return OptionsPage.Instance; } }
    private MainPageUIController mainController { get { return MainPageUIController.Instance; } }
    public override void enter()
    {
        Debug.Log(optionsPage.distanceWall.name);
        if (toggleButton.onDown == false )
        {
            setState(MainPageFreeState.Name);
            return;
        }
        UITool.SetActionTrue(optionsPage.distanceWall.gameObject);
     
        //UITool.SetActionTrue(optionsPage.RectDisWallsetValue.gameObject);
    }
    public override void exit()
    {
        toggleButton.onDown = false;
        mainController.CloseInput();
        UITool.SetActionFalse(optionsPage.distanceWall.gameObject);
        //UITool.SetActionFalse(optionsPage.RectDisWallsetValue.gameObject);
    }
    public override void Ready()
    {
        base.Ready();
        toggleButton = optionsPage.disWall;
    }
}
