using UnityEngine;
using System.Collections;
using System;

public class ExitState : MainPageState
{
    public const string Name = "ExitState";
    private SchemeManifest schemeManifest { get { return SchemeManifest.Instance; } }

    public override void enter()
    {
        base.enter();
        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(FreeState2D.NAME);
        }
        else {
            inputMachine.setState(FreeState3D.NAME);
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.Quit();
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            undoHelper.ClearView();
            RefreshView();
            GlobalConfig.running = false;
            SimpleLoader.CancelLoad();
            MyEventDispatcher.ClearAllMyEvent();
            MyCallLater.Add(0.1f, SendInfo);
        }
        else
        {
#if UNITY_EDITOR
            undoHelper.ClearView();
            RefreshView();
            GlobalConfig.running = false;
            SimpleLoader.CancelLoad();
            //UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }

    private void SendInfo()
    {
        MsgToIOS msg = new MsgToIOS();
        msg.code = "101003";
        UnityIOSMsg.sendToIOS(msg);
    }
    public override void Ready()
    {
        base.Ready();
    }
    public override void exit()
    {
        base.exit();
    }

}
