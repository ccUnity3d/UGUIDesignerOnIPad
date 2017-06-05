using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mode管理器
/// </summary>
public class InputStateMachine : BaseStateMachine<InputState, InputStateMachine>
{
    public bool currentInputIs2D
    {
        get {
            return CurrentState is InputState2D;
        }
    }
    public InputStateMachine()
    {
    }

    //public InputWallType inputWallType = InputWallType.Mid;

    public Texture Texture;
    public List<SelectProductData> selectGoods = new List<SelectProductData>();
    public SelectProductData selectGood;
    public List<int> selectGoodsList = null;

    /// <summary>
    /// 模板画墙索引
    /// </summary>
    public int templateType;

    public override void Inject()
    {
        addState(HoldOnState2D.NAME, new HoldOnState2D());
        addState(CloseState.NAME, new CloseState());
        addState(FreeState2D.NAME, new FreeState2D());
        addState(PlaceGoodsState2D.NAME, new PlaceGoodsState2D());
        addState(SetFirstPointState.NAME, new SetFirstPointState());
        addState(SetOtherPointState.NAME, new SetOtherPointState());
        addState(SetTempletState.NAME, new SetTempletState());

        addState(SetInnerFirstPointState.NAME, new SetInnerFirstPointState());
        addState(SetInnerOtherPointState.NAME, new SetInnerOtherPointState());

        addState(MeasureDisState.NAME, new MeasureDisState());

        addState(SelectPointState.NAME, new SelectPointState());
        addState(SelectWallState.NAME, new SelectWallState());
        addState(SelectRoomState.NAME, new SelectRoomState());
        addState(SelectGoodsState2D.NAME, new SelectGoodsState2D());


        addState(HoldOnState3D.NAME, new HoldOnState3D());
        addState(FreeState3D.NAME, new FreeState3D());
        addState(PlaceGoodsState3D.NAME, new PlaceGoodsState3D());
        addState(SelectCeilingState3D.NAME, new SelectCeilingState3D());
        addState(SelectFloorState3D.NAME, new SelectFloorState3D());
        addState(SelectItemState3D.NAME, new SelectItemState3D());
        addState(SelectWallSideState3D.NAME, new SelectWallSideState3D());
        addState(SelectGoodsState3D.NAME, new SelectGoodsState3D());

        addState(SetMaterialState.NAME, new SetMaterialState());

    }

    public SelectState getSelectState(string stateName)
    {
        if (stateDic.ContainsKey(stateName) == false)
        {
            return null;
        }
        SelectState state = stateDic[stateName] as SelectState;
        return state;
    }

    public SelectState3D getSelectState3D(string stateName)
    {
        if (stateDic.ContainsKey(stateName) == false)
        {
            return null;
        }
        SelectState3D state = stateDic[stateName] as SelectState3D;
        return state;
    }
    
}
