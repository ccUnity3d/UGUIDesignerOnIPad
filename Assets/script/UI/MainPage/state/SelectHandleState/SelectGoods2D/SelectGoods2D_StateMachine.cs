using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectGoods2D_StateMachine : SelectStateMachine<SelectGoods2D_State, SelectGoods2D_StateMachine>, IStateMachine
{
    public SelectGoodsState2D selectGoodsState2D;
    
    public SelectGoods2D_StateMachine()
    {

    }

    public override void Inject()
    {
        addState(EditTypeOnSelect.Free,         new SelectGoods2D_FreeState());
        addState(EditTypeOnSelect.Detail,       new SelectGoods2D_DetailState());
        addState(EditTypeOnSelect.Copy,         new SelectGoods2D_CopyState());
        addState(EditTypeOnSelect.MirroredX,    new SelectGoods2D_MirroredXState());
        addState(EditTypeOnSelect.MirroredY,    new SelectGoods2D_MirroredYState());
        addState(EditTypeOnSelect.Replace,      new SelectGoods2D_ReplaceState());
        addState(EditTypeOnSelect.DisWall,      new SelectGoods2D_DisWallState());
        addState(EditTypeOnSelect.DisFloor,     new SelectGoods2D_DisFloorState());
        addState(EditTypeOnSelect.Group,        new SelectGoods2D_GroupState());
        addState(EditTypeOnSelect.Delet,        new SelectGoods2D_DeletState());
        addState(EditTypeOnSelect.Collect,      new SelectGoods2D_CollectState());
        addState(EditTypeOnSelect.Hide,         new SelectGoods2D_HideState());
        addState(EditTypeOnSelect.Rotate, new SelectGoods2D_RotateState());
    }
    
}
