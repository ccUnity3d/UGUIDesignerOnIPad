using UnityEngine;
using System.Collections;
using System;

public class SelectGoods3D_StateMachine : SelectStateMachine<SelectGoods3D_State, SelectGoods3D_StateMachine> {

    public SelectGoodsState3D selectGoods3DState;

    public override void Inject()
    {
        addState(EditTypeOnSelect.Free, new SelectGoods3D_FreeState());
        addState(EditTypeOnSelect.Detail, new SelectGoods3D_DetailState());
        addState(EditTypeOnSelect.Copy, new SelectGoods3D_CopyState());

        addState(EditTypeOnSelect.MirroredX, new SelectGoods3D_MirroredXState());
        addState(EditTypeOnSelect.MirroredY, new SelectGoods3D_MirroredYState());
        addState(EditTypeOnSelect.DisFloor, new SelectGoods3D_DisFloorState());
        addState(EditTypeOnSelect.DisWall, new SelectGoods3D_DisWallState());
        //addState(EditTypeOnSelect.Replace, new SelectGoods3D_ReplaceState());


        addState(EditTypeOnSelect.Group, new SelectGoods3D_GroupState());
        addState(EditTypeOnSelect.Delet, new SelectGoods3D_DeletState());
        addState(EditTypeOnSelect.Collect, new SelectGoods3D_CollectState());
        addState(EditTypeOnSelect.Hide, new SelectGoods3D_HideState());

        addState(EditTypeOnSelect.Rotate, new SelectGoods3D_RotateState());
        addState(EditTypeOnSelect.MoveUp, new SelectGoods3D_MoveUpState());
    }

}
