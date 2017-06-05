using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ToTwoDState : MainPageState
{
    public const string Name = "ToTwoDState";
    private CameraStateMachine cameraMachine
    {
        get
        {
            return CameraStateMachine.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        List<Button> buttons = mainpage.two_D_No_Menu;

        if (inputMachine.currentInputIs2D == false)
        {
            cameraMachine.setState(CameraState2D.NAME);
            inputMachine.setState(FreeState2D.NAME);
            mainpage.menuGroup.cellSize = new Vector2(550, 56);
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(true);
            }
            mainpage.material.gameObject.SetActive(false);
            mainpage.cameraView.gameObject.SetActive(false);
            mainpage.scheme.gameObject.SetActive(true);
        }
        mainpage.thereD.gameObject.SetActive(true);
        mainpage.twoD.gameObject.SetActive(false);
        inputMachine.setState(FreeState2D.NAME);
        setState(MainPageFreeState.Name);
    }

    public override void exit()
    {
        base.exit();
    }
}
