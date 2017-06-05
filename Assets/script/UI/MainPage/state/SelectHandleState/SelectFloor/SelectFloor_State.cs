using UnityEngine;
using System.Collections;
using System;

public class SelectFloor_State : MySimpleState, IReady
{
    public virtual void Ready(GameObject skin)
    {
        //throw new NotImplementedException();
    }

    protected SelectFloor_StateMachine machine
    {
        get
        {
            return SelectFloor_StateMachine.Instance;
        }
    }

    protected MaterialData target
    {
        get {
            return machine.selectFloorState3D.target;
        }
    }

    protected void setState(string name)
    {
        machine.setState(name);
    }

}
