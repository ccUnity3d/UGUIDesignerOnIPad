using UnityEngine;
using System.Collections;

public class SelectGoods3D_DisFloorState : SelectGoods3D_State
{
    private RectTransform obj;
    private float min;
    private float max;
    private ToggleButton toggleButton;
    protected OptionsPage optionsPage
    {
        get
        {
            return OptionsPage.Instance;
        }
    }

    public override void Ready(GameObject skin)
    {
        base.Ready(skin);

        toggleButton = optionsPage.disFloor;
        obj = optionsPage.RectDisFloorValue;
    }

    public override void enter()
    {
        base.enter();

        if (toggleButton.onDown == false)
        {
            setState(EditTypeOnSelect.Free);
            return;
        }
        Debug.Log("离地");

        optionsPage.FloorSlider.wholeNumbers = false;
        min = optionsPage.FloorSlider.minValue = 0;
        max = optionsPage.FloorSlider.maxValue = 5 * (int)defaultSet.DefaultUnit;
        optionsPage.FloorSlider.value = target.height * (int)defaultSet.DefaultUnit;

        optionsPage.disFloorUnit.text = defaultSet.DefaultUnit.ToString();
        optionsPage.FloorTitle.text = optionsPage.disFloorValue.text = (target.height * (int)defaultSet.DefaultUnit).ToString();
        UITool.SetActionTrue(obj.gameObject);

        optionsPage.FloorSlider.onValueChanged.AddListener(onValueChanged);
        optionsPage.inputFloorValue.onClick.AddListener(onInputFloorValue);
        optionsPage.defaultFloorDis.onClick.AddListener(onDefaultFloorValue);

    }

    private void onValueChanged(float arg0)
    {
        int intValue = (int)(arg0 * (1000f / (int)defaultSet.DefaultUnit));
        float floatValue = intValue / (1000f / (int)defaultSet.DefaultUnit);
        optionsPage.disFloorValue.text = floatValue.ToString();
        optionsPage.FloorTitle.text = floatValue.ToString();
        target.height = floatValue / (int)defaultSet.DefaultUnit;
        //Debug.LogWarning("target.height = " + target.height);
        RefreshView();
    }

    private void onDefaultFloorValue()
    {
        //Debug.Log("距离地面的默认值");
        float value = targetProduct.defaultHeight * (int)defaultSet.DefaultUnit;
        optionsPage.FloorSlider.value = value;
        onValueChanged(value);
        RefreshView();
    }

    private void onInputFloorValue()
    {
        //Debug.Log("输入距离地面的值");
        mainPageContr.OpenInputWithLimit(optionsPage.disFloorValue.text, optionsPage.disFloorValue, min, max, OnFloorChangeL);
    }

    private void OnFloorChangeL()
    {
        float value = float.Parse(optionsPage.disFloorValue.text);
        optionsPage.FloorSlider.value = value;
        onValueChanged(value);
    }

    public override void exit()
    {
        UITool.SetActionFalse(obj.gameObject);
        toggleButton.onDown = false;
        mainPageContr.CloseInput();

        optionsPage.FloorSlider.onValueChanged.RemoveListener(onValueChanged);
        optionsPage.inputFloorValue.onClick.RemoveListener(onInputFloorValue);
        optionsPage.defaultFloorDis.onClick.RemoveListener(onDefaultFloorValue);
        base.exit();
    }
}
