using UnityEngine;
using System.Collections;

public class IOSEvent : MyEvent {

    public const string SelectGoods = "selectgods";
    public const string Collect = "Collect";
    public const string SetSchemeId = "SetSchemeId";
    public const string SetOfferId = "SetOfferId";
    public const string ShareImage = "ShareImage";

    public IOSEvent(string type, object data = null) : base(type, data)
    {

    }

}
