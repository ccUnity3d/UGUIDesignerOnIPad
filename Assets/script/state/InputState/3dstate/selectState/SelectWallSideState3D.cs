using UnityEngine;
using System.Collections;

public class SelectWallSideState3D : SelectState3D
{
    public const string NAME = "SelectWallSideState3D";
    public SelectWallSideState3D() : base(NAME)
    {
        selectMachine = SelectWallSide_StateMachine.Instance;
        selectWallSide_StateMachine.selectWallSideState3D = this;
    }

    private WallSideData _target;
    public WallSideData target
    {
        get { return _target; }
        set { _target = value; onTargetChange(_target); }
    }

    protected override GoodsType getGoodsType()
    {
        return GoodsType.WallSide;
    }

    private SelectWallSide_StateMachine selectWallSide_StateMachine
    {
        get
        {
            return SelectWallSide_StateMachine.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        target = (viewTarget as WallSideView).data;
        optionsController.selectMachine = selectWallSide_StateMachine;
        selectWallSide_StateMachine.setState(EditTypeOnSelect.Free);
    }

    public override void mUpdate()
    {
        base.mUpdate();
    }

    public override void exit()
    {
        selectWallSide_StateMachine.setState(EditTypeOnSelect.Free);
        base.exit();
    }
}
