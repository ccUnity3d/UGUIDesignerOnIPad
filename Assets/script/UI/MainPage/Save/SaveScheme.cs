using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SaveScheme : UIPage<SaveScheme>
{

    #region ui
    public RectTransform rectSave;
    public InputField schemeName;
    public InputField description;
    public Button cancel;
    public Button ok;
    #endregion

    protected override void Ready(Object arg1)
    {
        rectSave = skin.GetComponent<RectTransform>();
        schemeName = UITool.GetUIComponent<InputField>(rectSave, "SaveScheme/scheme/name/InputField");
        description = UITool.GetUIComponent<InputField>(rectSave, "SaveScheme/description/content/InputField");
        cancel = UITool.GetUIComponent<Button>(rectSave, "SaveScheme/cancel");
        ok = UITool.GetUIComponent<Button>(rectSave, "SaveScheme/ok");
        base.Ready(arg1);
    }


}
