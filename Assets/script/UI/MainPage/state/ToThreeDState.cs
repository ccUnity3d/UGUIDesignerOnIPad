using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ToThreeDState : MainPageState
{
    public const string Name = "ToThreeDState";
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
        if (inputMachine.currentInputIs2D == true)
        {
            cameraMachine.setState(CameraState3D.NAME);
            inputMachine.setState(FreeState3D.NAME);

            mainpage.menuGroup.cellSize = new Vector2(130, 56);
            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].gameObject.SetActive(false);
            }
            mainpage.material.gameObject.SetActive(true);
            mainpage.cameraView.gameObject.SetActive(true);
            mainpage.scheme.gameObject.SetActive(false);
        }
        mainpage.thereD.gameObject.SetActive(false);
        mainpage.twoD.gameObject.SetActive(true);
        //inputMachine.setState(FreeState3D.NAME);
        setState(MainPageFreeState.Name);
    }

    public override void exit()
    {
        base.exit();
    }
}