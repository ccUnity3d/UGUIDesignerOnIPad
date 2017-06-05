using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MainPageStateMachine : BaseStateMachine<MainPageState, MainPageStateMachine> {

    private static MainPageStateMachine instance;
    public override void Inject()
    {
        stateDic.Clear();

        addState(MainPageFreeState.Name, new MainPageFreeState());

        addState(ExitState.Name, new ExitState());
        addState(EditorSchemeState.Name, new EditorSchemeState());

        addState(OpenOtherSchemeState.Name, new OpenOtherSchemeState());
        addState(SaveState.Name, new SaveState());
        addState(UndoState.Name, new UndoState());
        addState(RedoState.Name, new RedoState());

        addState(TemplateState.Name, new TemplateState());
        addState(InnerLineState.Name, new InnerLineState());
        addState(MiddleLineState.Name, new MiddleLineState());
        addState(ShowState.Name, new ShowState());

        addState(MeasurementState.Name, new MeasurementState());
        addState(RenderState.Name, new RenderState());
        addState(OfferState.Name, new OfferState());
        addState(ShareState.Name, new ShareState());
        addState(CameraViewState.Name,new CameraViewState());

        addState(ToTwoDState.Name, new ToTwoDState());
        addState(ToThreeDState.Name, new ToThreeDState());
        addState(MaterialState.Name, new MaterialState());

        addState(QueryState.Name, new QueryState());
        addState(DefaultSetState.Name, new DefaultSetState());

        addState(AddGoodsState.Name, new AddGoodsState());



        addState(ParticularState.Name,new ParticularState());
        addState(DistanceFloorState.Name,new DistanceFloorState());
        addState(DistanceWallState.Name,new DistanceWallState());
        //addState(GroupState.Name, new GroupState());
    }

    public bool IsCurrentState(string name)
    {
        if (getState(name) == CurrentState && CurrentState !=null) return true;
        return false;
    }

    public void Ready()
    {
        foreach (MainPageState item in stateDic.Values)
        {
            item.Ready();
        }
    }
}
