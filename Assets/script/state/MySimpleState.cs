using UnityEngine;
using System.Collections;
using System;

public class MySimpleState : MyState
{
    protected override bool needUpdate
    {
        get
        {
            return false;
        }
    }

}
