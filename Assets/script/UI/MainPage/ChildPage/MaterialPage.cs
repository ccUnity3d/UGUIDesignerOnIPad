using UnityEngine;
using System.Collections;

public class MaterialPage : UIPage<MaterialPage> {

    public MaterialImageScroll materialScroll;
    public RectTransform RectController;

    protected override void Ready(Object arg1)
    {
        base.Ready(arg1);
        RectController = skin.GetComponent<RectTransform>();
        materialScroll = UITool.AddUIComponent<MaterialImageScroll>(skin);
    }
}
