using UnityEngine;
using System.Collections;
using System;

public class SelectCeiling_State : MySimpleState, IReady
{
    public virtual void Ready(GameObject skin)
    {
        //throw new NotImplementedException();
    }


    protected SelectCeiling_StateMachine machine
    {
        get
        {
            return SelectCeiling_StateMachine.Instance;
        }
    }

    protected MaterialData target
    {
        get
        {
            return machine.selectCeilingState3D.target;
        }
    }

    protected void setState(string name)
    {
        machine.setState(name);
    }

}
