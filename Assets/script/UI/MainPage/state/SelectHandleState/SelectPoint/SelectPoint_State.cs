using UnityEngine;
using System.Collections;
using System;

public class SelectPoint_State : MySimpleState , IReady{

    public SelectPoint_StateMachine machine
    {
        get {
            return SelectPoint_StateMachine.Instance;
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

    public Point target
    {
        get {
            return machine.selectPointState.target;
        }
    }

    protected OriginalInputData data
    {
        get
        {
            return OriginalInputData.Instance;
        }
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

    public virtual void Ready(GameObject optionSkin)
    {
        //throw new NotImplementedException();
    }
}
