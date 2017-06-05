using UnityEngine;
using System.Collections;
using System;

public class SelectWallSide_State : MySimpleState, IReady
{
    public SelectWallSide_StateMachine machine
    {
        get
        {
            return SelectWallSide_StateMachine.Instance;
        }
    }

    protected InputStateMachine inputMachine
    {
        get
        {
            return InputStateMachine.Instance;
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
    protected View2D view2D = View2D.Instance;
    protected View3D view3D = View3D.Instance;

    public override void enter()
    {
        base.enter();
    }

    public override void exit()
    {
        base.exit();
    }

    public WallSideData target
    {
        get
        {
            return machine.selectWallSideState3D.target;
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

    protected void Save()
    {
        undoHelper.save();
    }

    public virtual void Ready(GameObject skin)
    {
        //throw new NotImplementedException();
    }
}