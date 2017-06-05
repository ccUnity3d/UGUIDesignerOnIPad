using UnityEngine;
using System.Collections;

public class MyCacheEvent : MyEvent
{
    public MyCacheEvent(string type) :base(type)
    {

    }
    public static readonly string loadReady = "loadReady";
}
