using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class TemplatePage : UIPage<TemplatePage> {


    public Button temp1;
    public Button temp2;
    public Button temp3;
    public Button temp4;
    public Button temp5;
    public Button temp6;

    protected override void Ready(Object arg1)
    {
        RectTransform gridLayoutGroup = UITool.GetUIComponent<RectTransform>(skin.transform, "GridLayoutGroup");
        temp1 = UITool.GetUIComponent<Button>(gridLayoutGroup, "temp1");
        temp2 = UITool.GetUIComponent<Button>(gridLayoutGroup, "temp2");
        temp3 = UITool.GetUIComponent<Button>(gridLayoutGroup, "temp3");
        temp4 = UITool.GetUIComponent<Button>(gridLayoutGroup, "temp4");
        temp5 = UITool.GetUIComponent<Button>(gridLayoutGroup, "temp5");
        temp6 = UITool.GetUIComponent<Button>(gridLayoutGroup, "temp6");

    }
}
