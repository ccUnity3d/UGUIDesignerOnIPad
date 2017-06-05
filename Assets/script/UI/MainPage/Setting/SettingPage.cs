using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class SettingPage : UIPage<SettingPage> {

    #region Setting
    public RectTransform rectSetting;
    public Dropdown measurementList;
    public Text measurementUnit;
    public Text wallHeightUnit;
    public Text wallHeightValue;
    public Text wallWidthUnit;
    public Text wallWidthValue;
    public Button wallWidth;
    public Button wallHeigth;
    public Button SetCancel;
    public Button SetOk;
    #endregion
    protected override void Ready(Object arg1)
    {
        #region Setting
        rectSetting = skin.GetComponent<RectTransform>();
        measurementList = UITool.GetUIComponent<Dropdown>(rectSetting, "Setting/CenterLayout/Measurement/UnitDropdown");
        measurementUnit = UITool.GetUIComponent<Text>(rectSetting, "Setting/CenterLayout/Measurement/UnitDropdown/unit");
        wallWidth = UITool.GetUIComponent<Button>(rectSetting, "Setting/CenterLayout/WallWidth/InputWallWidth");
        wallHeigth = UITool.GetUIComponent<Button>(rectSetting, "Setting/CenterLayout/WallHight/InputWallHieght");
        wallHeightUnit = UITool.GetUIComponent<Text>(rectSetting, "Setting/CenterLayout/WallHight/InputWallHieght/unit");
        wallHeightValue = UITool.GetUIComponent<Text>(rectSetting, "Setting/CenterLayout/WallHight/InputWallHieght/inputValue");
        wallWidthUnit = UITool.GetUIComponent<Text>(rectSetting, "Setting/CenterLayout/WallWidth/InputWallWidth/unit");
        wallWidthValue = UITool.GetUIComponent<Text>(rectSetting, "Setting/CenterLayout/WallWidth/InputWallWidth/inputValue");
        SetCancel = UITool.GetUIComponent<Button>(rectSetting, "Setting/BottomLayout/Camcel");
        SetOk = UITool.GetUIComponent<Button>(rectSetting, "Setting/BottomLayout/Ok");
        #endregion
    }
}
