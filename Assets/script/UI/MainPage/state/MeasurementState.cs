using UnityEngine;
using System.Collections;

public class MeasurementState : MainPageState {

    public const string Name = "MeasurementState";
    private ToggleButton measurement;

    public override void enter()
    {
        base.enter();
        if (measurement.onDown == false)
        {
            setState(MainPageFreeState.Name);
            return;
        }
        inputMachine.setState(MeasureDisState.NAME);
    }

    public override void Ready()
    {
        base.Ready();
        measurement = mainpage.measurement;
    }

    public override void exit()
    {
        inputMachine.setState(FreeState2D.NAME);
        measurement.onDown = false;
        base.exit();
    }
}
