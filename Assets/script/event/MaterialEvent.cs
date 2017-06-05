using UnityEngine;
using System.Collections;

public class MaterialEvent : MyEvent
{
    public const string AddMaterial = "AddMaterial";
    public const string RemoveMaterial = "RemoveMaterial";

    public MaterialEvent(string type, object data = null):base(type, data)
    {

    }
}
