using UnityEngine;
using System.Collections;
using System;

public class SelectWallSide_TopLineState : SelectWallSide_State
{
    private OptionsPage optionsPage { get { return OptionsPage.Instance; } }
    private MainPageUIController mainController { get { return MainPageUIController.Instance; } }
    public override void Ready(GameObject skin)
    {
        base.Ready(skin);
       
        optionsPage.optionOne.onClick.AddListener(onOptionOne);
        optionsPage.optionTwo.onClick.AddListener(onOptionTwo);
        optionsPage.optionThere.onClick.AddListener(onOptionTwo);
        optionsPage.optionFour.onClick.AddListener(onOptionFour);
        optionsPage.optionFive.onClick.AddListener(onOptionFive);
        //optionsPage.optionSix.onClick.AddListener(onOptionSix);
        //optionsPage.optionSeven.onClick.AddListener(onOptionSeven);
        optionsPage.setValue1.onClick.AddListener(onSetValue1);
        optionsPage.setValue2.onClick.AddListener(onSetValue2);
        optionsPage.auto.onClick.AddListener(onAuto);
    }
    public override void enter()
    {
        base.enter();
        UITool.SetActionTrue(optionsPage.wall4optionList.gameObject);
    }

    public override void exit()
    {
        base.exit();
        UITool.SetActionFalse(optionsPage.wall4optionList.gameObject);
    }

    
   
    private void onOptionOne()
    {
        Debug.Log("onOptionOne");
        if (target.topBaseboard.hide == true) return;
        Save();
        //target.topBaseboard.index = -1;
        target.topBaseboard.hide = true;
        RefreshView();
    }
    private void onOptionTwo()
    {
        Debug.Log("onOptionTwo");
        if (target.topBaseboard.index == 0 && target.topBaseboard.hide == false) return;
        Save();
        target.topBaseboard.index = 0;
        target.topBaseboard.hide = false;
        RefreshView();
    }
    private void onOptionThere()
    {
        Debug.Log("onOptionThere");
        if (target.topBaseboard.index == 1 && target.topBaseboard.hide == false) return;
        Save();
        target.topBaseboard.index = 1;
        target.topBaseboard.hide = false;
        RefreshView();
    }
    private void onOptionFour()
    {
        Debug.Log("onOptionFour");
        if (target.topBaseboard.index == 2 && target.topBaseboard.hide == false) return;
        Save();
        target.topBaseboard.index = 2;
        target.topBaseboard.hide = false;
        RefreshView();
    }
    private void onOptionFive()
    {
        Debug.Log("onOptionFive");
        if (target.topBaseboard.index == 3 && target.topBaseboard.hide == false) return;
        Save();
        target.topBaseboard.index = 3;
        target.topBaseboard.hide = false;
        RefreshView();
    }
    //private void onOptionSix()
    //{
    //    Debug.Log("onOptionSix");
    //    target.topBaseboard.hide = false;
    //}
    //private void onOptionSeven()
    //{
    //    Debug.Log("onOptionSeven");
    //}
    private void onSetValue1()
    {
        Debug.Log("onSetValue1");
        mainController.OpenInputWithLimit(optionsPage.wallValue1.text, optionsPage.wallValue1, 0, float.MaxValue, deleChangeWallValue1);
    }

    private void onSetValue2()
    {
        Debug.Log("onSetValue2");
        mainController.OpenInputWithLimit(optionsPage.wallValue2.text, optionsPage.wallValue2, 0, float.MaxValue, deleChangeWallValue2);
    }
    private void onAuto()
    {
        Debug.Log("onAuto");
    }


    private void deleChangeWallValue1()
    {
        Save();
        float height = float.Parse(optionsPage.wallValue1.text);
        target.topBaseboard.height = height;
        RefreshView();
    }

    private void deleChangeWallValue2()
    {
        Save();
        float disRoot = float.Parse(optionsPage.wallValue2.text);
        target.topBaseboard.disRoot = disRoot;
        RefreshView();
    }

}
