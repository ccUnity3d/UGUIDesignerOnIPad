using UnityEngine;
using System.Collections;

public class SelectCeilingState3D : SelectState3D
{
    public const string NAME = "SelectCeilingState3D";
    public SelectCeilingState3D() : base(NAME)
    {
        selectMachine = SelectCeiling_StateMachine.Instance;
        selectCeiling_StateMachine.selectCeilingState3D = this;
    }

    private MaterialData _target;
    public MaterialData target
    {
        get { return _target; }
        set { _target = value; onTargetChange(_target); }
    }

    protected override GoodsType getGoodsType()
    {
        return GoodsType.Ceiling;
    }

    private SelectCeiling_StateMachine selectCeiling_StateMachine
    {
        get
        {
            return SelectCeiling_StateMachine.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        target = (viewTarget as CeilingView).data;
        optionsController.selectMachine = selectCeiling_StateMachine;
        selectCeiling_StateMachine.setState(EditTypeOnSelect.Free);
    }


    public override void mUpdate()
    {
        base.mUpdate();
    }

    public override void exit()
    {
        selectCeiling_StateMachine.setState(EditTypeOnSelect.Free); 
        base.exit();
    }
}
