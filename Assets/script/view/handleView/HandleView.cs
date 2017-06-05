using UnityEngine;
using System.Collections;
using System;

public class HandleView : MonoBehaviour {

    public virtual bool SetState(IStateMachine selectMachine)
    {
        return false;
        //throw new NotImplementedException();
    }

    public virtual bool moveable
    {
        get { return false; }
    }
}
