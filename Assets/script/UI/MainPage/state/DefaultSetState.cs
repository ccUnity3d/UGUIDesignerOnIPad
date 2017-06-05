using UnityEngine;
using System.Collections;

public class DefaultSetState : MainPageState
{
    public const string Name = "SetState";
    private GameObject go;
    private SettingPage setting { get { return SettingPage.Instance; } }

    private string setunitVaule;

    #region Setting
    public string defaultUnitVaule
    {
        get {
            return defaultSettings.DefaultUnit.ToString();
        }
    }
    public string defaultHeightValue
    {
        get
        {
            return (defaultSettings.DefaultHeight * (int)defaultSettings.DefaultUnit).ToString();
        }
    }
    public string defaultWidthValue
    {
        get {
            return (defaultSettings.DefaultWidth * (int)defaultSettings.DefaultUnit).ToString(); 
        }
    }

    private DefaultSettings defaultSettings
    {
        get {
            return DefaultSettings.Instance;
        }
    }
    
    public override void enter()
    {
        base.enter();
        if (inputMachine.currentInputIs2D)
        {
            inputMachine.setState(FreeState2D.NAME);
        }
        else {
            inputMachine.setState(FreeState3D.NAME);
        }
        if (setting.skin.activeSelf == false)
        {
            setting.skin.SetActive(true);

            setunitVaule = defaultUnitVaule;
            setting.measurementUnit.text = defaultUnitVaule;
            setting.wallHeightUnit.text = defaultUnitVaule;
            setting.wallWidthUnit.text = defaultUnitVaule;
            setting.wallHeightValue.text = defaultHeightValue;
            setting.wallWidthValue.text = defaultWidthValue;
            setting.measurementList.value = 3;
            for (int i = 0; i < setting.measurementList.options.Count; i++)
            {
                if (setting.measurementList.options[i].text == defaultSettings.DefaultUnit.ToString())
                {
                    setting.measurementList.value = i;
                    break;
                }
            }
        }
        else
        {
            setState(MainPageFreeState.Name);
        }
        //mainpage.Setting.gameObject.SetActive(true);
    }

    public override void Ready()
    {
        base.Ready();
        setting.SetData(mainpage.SkinSetting);

        #region Setting
        setting.wallHeigth.onClick.AddListener(OnWallHight);
        setting.wallWidth.onClick.AddListener(OnWallWidth);
        setting.measurementList.onValueChanged.AddListener(OnMeasurement);
        setting.SetCancel.onClick.AddListener(OnSetCancel);
        setting.SetOk.onClick.AddListener(OnSetOk);
        //mainpage.wallHightValue.text += KeyPageController.Instace.deleSetValue();
        #endregion
    }


    public override void exit()
    {
        setting.skin.SetActive(false);
        base.exit();
    }
   
    private void OnWallHight()
    {
        controller.OpenInput(defaultHeightValue, setting.wallHeightValue);
        //Debug.Log(setting.wallHeightValue.text);
       // defaultHeightValue = setting.wallHeightValue.text;
    }
    private void OnMeasurement(int Index)
    {
        string value = setting.measurementList.options[Index].text;
        setunitVaule = value;
        setting.measurementUnit.text = setunitVaule;
        setting.wallHeightUnit.text = setunitVaule;
        setting.wallWidthUnit.text = setunitVaule;
        Unit unit = Unit.mm;
        switch (setunitVaule)
        {
            case "m":
                unit = Unit.m;
                break;
            case "dm":
                unit = Unit.dm;
                break;
            case "cm":
                unit = Unit.cm;
                break;
            case "mm":
                unit = Unit.mm;
                break;
            case "ft":
                unit = Unit.ft;
                break;
            default:
                break;
        }
        setting.wallHeightValue.text = (defaultSettings.DefaultHeight * (int)unit).ToString();
        setting.wallWidthValue.text = (defaultSettings.DefaultWidth* (int)unit).ToString();
    }
    private void OnWallWidth()
    {
        controller.OpenInput(defaultWidthValue, setting.wallWidthValue);
    }

    private void OnSetCancel()
    {
        setting.measurementUnit.text = defaultUnitVaule;
        setting.wallHeightValue.text = defaultHeightValue;
        setting.wallWidthValue.text = defaultWidthValue;
        setting.rectSetting.gameObject.SetActive(false);
        KeyBoard.Instance.keyboard.gameObject.SetActive(false);
    }
    private void OnSetOk()
    {
        defaultSettings.DefaultUnit = Unit.mm;
        switch (setunitVaule)
        {
            case "m":
                defaultSettings.DefaultUnit = Unit.m;
                break;
            case "dm":
                defaultSettings.DefaultUnit = Unit.dm;
                break;
            case "cm":
                defaultSettings.DefaultUnit = Unit.cm;
                break;
            case "mm":
                defaultSettings.DefaultUnit = Unit.mm;
                break;
            case "ft":
                defaultSettings.DefaultUnit = Unit.ft;
                break;
            default:
                break;
        }
        string setWidthValue = setting.wallWidthValue.text;
        string setHeightValue = setting.wallHeightValue.text;
        float width;
        float heigth;
        float.TryParse(setWidthValue, out width);
        defaultSettings.DefaultWidth = width / (int)defaultSettings.DefaultUnit;
        bool isDefault = float.TryParse(setHeightValue, out heigth);
        defaultSettings.DefaultHeight = heigth / (int)defaultSettings.DefaultUnit;
        setting.rectSetting.gameObject.SetActive(false);
        KeyBoard.Instance.keyboard.gameObject.SetActive(false);
        RefreshView();
        //Debug.Log("确认");
    }
    #endregion
}
