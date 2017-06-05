using UnityEngine;
using System.Collections;
using System;

public class SelectStateMachine<T, ChildType> : BaseStateMachine<T, ChildType>, IStateMachine
    where T : IMyState, IReady
    where ChildType : IInstance, IStateMachine<T>, new()
{
    
    public override void Inject()
    {
        
    }

    public void Ready(GameObject optionSkin)
    {
        foreach (T item in stateDic.Values)
        {
            item.Ready(optionSkin);
        }
    }
}
