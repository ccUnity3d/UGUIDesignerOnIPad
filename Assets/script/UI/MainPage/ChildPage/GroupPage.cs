using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GroupPage : UIPage<GroupPage> {

    public RectTransform groupRectTransform;
    public InputField inputName;
    public InputField inputInfo;
    public Text name;
    public Text detailsText;
    public RectTransform collectGroup;
    public Button cancel;
    public Button ok;

    protected override void Ready(Object arg1)
    {
        groupRectTransform = UITool.GetUIComponent<RectTransform>(skin.transform, "Group");
        inputName = UITool.GetUIComponent<InputField>(skin.transform, "Group/InputName");
        name = UITool.GetUIComponent<Text>(skin.transform, "Group/InputName/Text");
        inputInfo = UITool.GetUIComponent<InputField>(skin.transform, "Group/InputInfo");
        detailsText = UITool.GetUIComponent<Text>(skin.transform, "Group/InputInfo/Text");
        collectGroup = UITool.GetUIComponent<RectTransform>(skin.transform, "Group/groupScrollView");
        cancel = UITool.GetUIComponent<Button>(skin.transform, "Group/Cancel");
        ok = UITool.GetUIComponent<Button>(skin.transform, "Group/Ok");
    }
}
