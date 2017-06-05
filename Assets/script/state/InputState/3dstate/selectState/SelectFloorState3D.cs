using UnityEngine;
using System.Collections;

public class SelectFloorState3D : SelectState3D
{
    public const string NAME = "SelectFloorState3D";
    public SelectFloorState3D() : base(NAME)
    {
        selectMachine = SelectGoods3D_StateMachine.Instance;
        selectFloor_StateMachine.selectFloorState3D = this;
    }

    private MaterialData _target;
    public MaterialData target
    {
        get { return _target; }
        set { _target = value; onTargetChange(_target); }
    }

    protected override GoodsType getGoodsType()
    {
        return GoodsType.Floor;
    }

    private SelectFloor_StateMachine selectFloor_StateMachine
    {
        get {
            return SelectFloor_StateMachine.Instance;
        }
    }

    public override void enter()
    {
        base.enter();
        target = (viewTarget as FloorView).data;
        optionsController.selectMachine = selectFloor_StateMachine;
        selectFloor_StateMachine.setState(EditTypeOnSelect.Free);
    }

    public override void mUpdate()
    {
        base.mUpdate();
    }

    public override void exit()
    {
        selectFloor_StateMachine.setState(EditTypeOnSelect.Free);
        base.exit();
    }
}
