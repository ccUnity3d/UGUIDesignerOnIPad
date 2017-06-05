using UnityEngine;
using System.Collections;
using System;

public class SelectFloor_RotateState : SelectFloor_State {

    private OptionsPage optionsPage { get { return OptionsPage.Instance; } }
    private MainPageUIController mainController { get { return MainPageUIController.Instance; } }
    protected View3D view3D { get { return View3D.Instance; } }
    private int num90;
    private int angle45;

    public override void Ready(GameObject skin)
    {
        base.Ready(skin);
    }

    public override void enter()
    {
        base.enter();
        if (optionsPage.rotation.onDown == false)
        {
            setState(EditTypeOnSelect.Free);
            return;
        }
        UITool.SetActionTrue(optionsPage.RectRotation.gameObject);
        num90 = Get90Count(target.rotation, out angle45);
        optionsPage.rotationSlider.value = angle45;
        optionsPage.sliderValue.text = angle45.ToString();
        optionsPage.rotationValue.text = angle45.ToString();
        optionsPage.rotationDefault.onClick.AddListener(onRotationDefault);
        optionsPage.rotationAngle.onClick.AddListener(onRotationAngel);
        optionsPage.rotationSlider.onValueChanged.AddListener(onRotationSliderChange);
        optionsPage.inputRotation.onClick.AddListener(onInputRotation);
    }

    public override void exit()
    {
        optionsPage.rotationDefault.onClick.RemoveListener(onRotationDefault);
        optionsPage.rotationAngle.onClick.RemoveListener(onRotationAngel);
        optionsPage.rotationSlider.onValueChanged.RemoveListener(onRotationSliderChange);
        optionsPage.inputRotation.onClick.RemoveListener(onInputRotation);
        UITool.SetActionFalse(optionsPage.RectRotation.gameObject);
        mainController.CloseInput();
        optionsPage.rotation.onDown = false;
        base.exit();
    }

    private void onInputRotation()
    {
        //Debug.Log("输入旋转值");
        mainController.OpenInput(optionsPage.rotationValue.text, optionsPage.rotationValue, OnRotationChange);
    }

    private void OnRotationChange()
    {
        float value = float.Parse(optionsPage.rotationValue.text);
        angle45 = (int)Mathf.Clamp(value, -45, 45);
        optionsPage.rotationSlider.value = angle45;
        onRotationSliderChange(angle45);
    }

    private void onRotationSliderChange(float arg0)
    {
        //Debug.Log("sliderChange");
        angle45 = (int)arg0;
        optionsPage.rotationValue.text = angle45.ToString();
        optionsPage.sliderValue.text = angle45.ToString();
        target.rotation = num90 * 90 + angle45;
        if (target.rotation < 0)
        {
            target.rotation += 360;
        }
        view3D.RefreshView();
    }

    private void onRotationAngel()
    {
        //Debug.Log("旋转90度");
        num90 += 1;
        if (num90 > 3)
        {
            num90 = 0;
        }
        target.rotation = num90*90 + angle45;
        if (target.rotation < 360)
        {
            target.rotation += 360;
        }
        view3D.RefreshView();
    }

    private void onRotationDefault()
    {
        //Debug.Log("默认旋转角度");
        num90 = 0;
        optionsPage.rotationSlider.value = 0;
        onRotationSliderChange(0);
    }

    private int Get90Count(float rotation, out int angle)
    {
        int count = 0;
        if (rotation <= 45)
        {
            angle = (int)rotation;
        }
        else if (rotation >= 315)
        {
            angle = (int)rotation - 360;
        }
        else {
            while (Mathf.Abs(rotation) > 45)
            {
                rotation -= 90;
                count++;
            }
            angle = (int)rotation;
        }
        return count;
    }

}
