using UnityEngine;
using System.Collections;
using System;

public class SelectRoom_State : MySimpleState, IReady {

    public SelectRoom_StateMachine machine
    {
        get {
            return SelectRoom_StateMachine.Instance;
        }
    }
    protected InputStateMachine inputMachine
    {
        get {
            return InputStateMachine.Instance;
        }
    }
    protected RoomHelpFunc roomfunc
    {
        get {
            return RoomHelpFunc.Instance;
        }
    }
    protected UndoHelper undoHelper
    {
        get {
            return UndoHelper.Instance;
        }
    }
    /// <summary>
    /// 2D墙设置视图
    /// </summary>
    protected View2D view2D
    {
        get {
            return View2D.Instance;
        }
    }
    protected View3D view3D
    {
        get {
            return View3D.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
    }

    public override void exit()
    {
        base.exit();
    }

    public RoomData target
    {
        get
        {
            return machine.selectRoomState.target;
        }
    }

    protected OriginalInputData data
    {
        get
        {
            return OriginalInputData.Instance;
        }
    }

    protected void setState(string name)
    {
        machine.setState(name);
    }

    protected void RefreshView()
    {
        if (inputMachine.currentInputIs2D)
        {
            view2D.RefreshView();
        }
        else {
            view3D.RefreshView();
        }
    }

    public virtual void Ready(GameObject skin)
    {
        //throw new NotImplementedException();
    }
}
