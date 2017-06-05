using UnityEngine;
using System.Collections;

public class MainPageUIDataEvent : MyEvent {

    public const string SchemeIdChanged = "SchemeIdChanged";

    public const string SchemeEffectImageDeleted = "SchemeEffectImageDeleted";

    public const string OfferIdChanged = "OfferIdChanged";

    public MainPageUIDataEvent(string type, object data = null) : base(type, data)
    {

    }

}
