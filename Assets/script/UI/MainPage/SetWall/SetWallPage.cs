using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SetWallPage : UIPage<SetWallPage> {

    public RectTransform rectWallHeightOrShort;
    public RectTransform gridLayout;
    public Button lenghtButton;
    public RectTransform lengthRect;
    public Text lenghtText;
    public Text lenghtUnit;
    public Button widthButton;
    public RectTransform widthRect;
    public Text widthText;
    public Text widthUnit;
    public Button heightButton;
    public RectTransform heightRect;
    public Text heightText;
    public Text heightUnit;
    public Button wallTwoOK;
    protected override void Ready(Object arg1)
    {
        rectWallHeightOrShort = UITool.GetUIComponent<RectTransform>(skin.transform, "Wall");
        gridLayout = UITool.GetUIComponent<RectTransform>(rectWallHeightOrShort, "GridLayout");
        lengthRect = UITool.GetUIComponent<RectTransform>(rectWallHeightOrShort, "GridLayout/Length");
        lenghtButton = UITool.GetUIComponent<Button>(rectWallHeightOrShort, "GridLayout/Length/LengthBtn");
        lenghtUnit = UITool.GetUIComponent<Text>(rectWallHeightOrShort, "GridLayout/Length/LengthBtn/unit");
        widthRect = UITool.GetUIComponent<RectTransform>(rectWallHeightOrShort,"GridLayout/Width");
        widthButton = UITool.GetUIComponent<Button>(rectWallHeightOrShort, "GridLayout/Width/WidthBtn");
        widthUnit = UITool.GetUIComponent<Text>(rectWallHeightOrShort, "GridLayout/Width/WidthBtn/unit");
        heightRect = UITool.GetUIComponent<RectTransform>(rectWallHeightOrShort, "GridLayout/Height");
        heightButton = UITool.GetUIComponent<Button>(rectWallHeightOrShort, "GridLayout/Height/HeightBtn");
        heightUnit = UITool.GetUIComponent<Text>(rectWallHeightOrShort, "GridLayout/Height/HeightBtn/unit");
        lenghtText = UITool.GetUIComponent<Text>(lenghtButton.transform, "value");
        widthText = UITool.GetUIComponent<Text>(widthButton.transform, "value");
        heightText = UITool.GetUIComponent<Text>(heightButton.transform, "value");
        wallTwoOK = UITool.GetUIComponent<Button>(rectWallHeightOrShort, "GridLayout/Ok/OkBtn");
    }
}
