using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ShowOrHide : UIPage<ShowOrHide>
{


    #region Show
    public RectTransform RectShow;
    public Toggle Showsize;
    public Toggle Showtop;
    public Toggle showObject;
    public Toggle halfHighWall;
    public Text showObjectName;
    #endregion

    protected override void Ready(Object arg1)
    {
        base.Ready(arg1);

        RectShow = skin.GetComponent<RectTransform>();
        Showsize = UITool.GetUIComponent<Toggle>(RectShow, "GridLayout/Size/size");
        Showtop = UITool.GetUIComponent<Toggle>(RectShow, "GridLayout/Top/top");
        showObject = UITool.GetUIComponent<Toggle>(RectShow, "GridLayout/Show/Object");
        halfHighWall = UITool.GetUIComponent<Toggle>(RectShow, "GridLayout/HalfHighWall/HalfHighWall");
        showObjectName = UITool.GetUIComponent<Text>(showObject.transform, "name");
        Showsize.isOn = true;
        Showtop.isOn = true;
        halfHighWall.isOn = false;
    }
}
