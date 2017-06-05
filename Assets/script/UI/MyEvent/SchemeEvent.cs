using UnityEngine;
using System.Collections;
using foundation;

public class SchemeEvent :MyEvent {

    // 点击效果图
    public const string openEffectImage = "openEffectImage";
    // 添加 报价
    public const string AddOffer = "AddOffer";


    public const string openOffer= "openOffer";

    public const string ChangeScheme = "ChangeScheme";

    public const string SaveSchemeToGenerateOffer = "SaveSchemeToGenerateOffer";

    public const string GenerateOffer = "GenerateOffer";
    public SchemeEvent(string type,object data =null):base(type,data)
    {
        
    }
}
