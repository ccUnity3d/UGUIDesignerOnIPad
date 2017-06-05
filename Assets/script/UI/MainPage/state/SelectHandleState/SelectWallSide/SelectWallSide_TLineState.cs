using UnityEngine;
using System.Collections;

public class SelectWallSide_TLineState : SelectWallSide_State
{

   private OptionsPage optionsPage { get { return OptionsPage.Instance; } }
    private MainPageUIController mainController { get { return MainPageUIController.Instance; } }
    public override void Ready(GameObject skin)
    {
        base.Ready(skin);
        optionsPage.option1.onClick.AddListener(onOption1);
        optionsPage.option2.onClick.AddListener(onOption2);
        optionsPage.option3.onClick.AddListener(onOption3);
        optionsPage.option4.onClick.AddListener(onOption4);
        optionsPage.option5.onClick.AddListener(onOption5);
        //optionsPage.option6.onClick.AddListener(onOption6);
        optionsPage.setValue.onClick.AddListener(onSetValue);
    }
    public override void enter()
    {
        base.enter();
        UITool.SetActionTrue(optionsPage.wall2optionList.gameObject);
    }
    public override void exit()
    {
        base.exit();
        UITool.SetActionFalse(optionsPage.wall2optionList.gameObject);
    }
   
    private void onOption1()
    {
        Debug.Log("onOption1");
        if (target.tBaseboard.hide == true) return;
        Save();
        //target.tBaseboard.index = -1;
        target.tBaseboard.hide = true;
        RefreshView();
    }
    private void onOption2()
    {
        Debug.Log("onOption2");
        if (target.tBaseboard.index == 0 && target.tBaseboard.hide == false) return;
        Save();
        target.tBaseboard.index = 0;
        target.tBaseboard.hide = false;
        RefreshView();
    }
    private void onOption3()
    {
        Debug.Log("onOption3");
        if (target.tBaseboard.index == 1 && target.tBaseboard.hide == false) return;
        Save();
        target.tBaseboard.index = 1;
        target.tBaseboard.hide = false;
        RefreshView();
    }
    private void onOption4()
    {
        Debug.Log("onOption4");
        if (target.tBaseboard.index == 2 && target.tBaseboard.hide == false) return;
        Save();
        target.tBaseboard.index = 2;
        target.tBaseboard.hide = false;
        RefreshView();
    }
    private void onOption5()
    {
        Debug.Log("onOption5");
        if (target.tBaseboard.index == 3 && target.tBaseboard.hide == false) return;
        Save();
        target.tBaseboard.index = 3;
        target.tBaseboard.hide = false;
        RefreshView();
    }
    //private void onOption6()
    //{
    //    Debug.Log("onOption6");
    //    target.tBaseboard.hide = false;
    //}

    /// <summary>
    /// 踢脚线高度
    /// </summary>
    private void onSetValue()
    {
        Debug.Log("onSetValue");
        mainController.OpenInputWithLimit(optionsPage.wallValue.text, optionsPage.wallValue, 0, float.MaxValue, deleChangeWallValue);
    }

    private void deleChangeWallValue()
    {
        Save();
        float height = float.Parse(optionsPage.wallValue.text);
        target.tBaseboard.height = height;
        RefreshView();
    }

}
