using System;
using UnityEngine;

public class SelectWall_State : MySimpleState, IReady
{
    public SelectWall_StateMachine machine
    {
        get {
            return SelectWall_StateMachine.Instance;
        }
    }

    public object bringData;
    protected InputStateMachine inputMachine
    {
        get
        {
            return InputStateMachine.Instance;
        }
    }
    protected RoomHelpFunc roomfunc
    {
        get
        {
            return RoomHelpFunc.Instance;
        }
    }
    protected WallHelpFunc wallfunc
    {
        get
        {
            return WallHelpFunc.Instance;
        }
    }
    protected LineHelpFunc linefunc
    {
        get
        {
            return LineHelpFunc.Instance;
        }
    }
    protected UndoHelper undoHelper
    {
        get
        {
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

    protected DefaultSettings defaultSettings
    {
        get {
            return DefaultSettings.Instance;
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

    public WallData target
    {
        get
        {
            return machine.selectWallState.target;
        }
        set {
            machine.selectWallState.target = value;
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
