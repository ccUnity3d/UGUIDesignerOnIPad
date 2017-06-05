using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EditorSchemeState : MainPageState
{
    private SchemePageController schemeController
    {
        get
        {
            return SchemePageController.Instance;
        }
    }
    public const string Name = "EditorSchemeState";
    private RectTransform rectScheme;
    public override void Ready()
    {
        base.Ready();
        rectScheme = mainpage.SkinScheme.GetComponent<RectTransform>(); 
    }

    public override void enter()
    {
        base.enter();

        schemeController.OpenScheme();
    }
    public override void exit()
    {
        base.exit();
    }
}