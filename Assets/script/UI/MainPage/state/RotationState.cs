using UnityEngine;
using System.Collections;

public class RotationState : MainPageState {

    public static string Name = "Rotation";

    private ToggleButton toggleButton;

    private OptionsPage optionPage { get { return OptionsPage.Instance; } }

    public override void enter()
    {
        if (toggleButton.onDown == false )
        {
            setState(EditTypeOnSelect.Free);
            UITool.SetActionFalse(optionPage.RectRotation.gameObject);
            return;
        }
        UITool.SetActionTrue(optionPage.RectRotation.gameObject);
    }
    public override void exit()
    {
        toggleButton.onDown = false;
        UITool.SetActionFalse(optionPage.RectRotation.gameObject);
    }
    public override void Ready()
    {
        base.Ready();
        toggleButton = optionPage.rotation;
    }
}
