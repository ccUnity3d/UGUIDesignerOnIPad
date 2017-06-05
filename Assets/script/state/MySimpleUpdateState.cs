using UnityEngine;
using System.Collections;

public class MySimpleUpdateState : MySimpleState {

    new public void Enter()
    {
        enter();
        MyTickManager.Add(update);
    }

    new public void Exit()
    {
        MyTickManager.Remove(update);
        exit();
    }

    public virtual void update()
    {

    }
    
}
