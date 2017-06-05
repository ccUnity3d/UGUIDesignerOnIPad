using UnityEngine;
using System.Collections;
using System;

public class TemplateState : MainPageState
{
    public const string Name = "TemplateState";

    private TemplatePage template { get { return TemplatePage.Instance; } }


    private ToggleButton toggleButton;

    public override void Ready()
    {
        base.Ready();
        toggleButton = mainpage.template;
        template.SetData(mainpage.SkinTemplate);
        template.temp1.onClick.AddListener(OnTemp1);
        template.temp2.onClick.AddListener(OnTemp2);
        template.temp3.onClick.AddListener(OnTemp3);
        template.temp4.onClick.AddListener(OnTemp4);
        template.temp5.onClick.AddListener(OnTemp5);
        template.temp6.onClick.AddListener(OnTemp6);

        //template.temp1.onClick.AddListener(Close);
        //template.temp2.onClick.AddListener(Close);
        //template.temp3.onClick.AddListener(Close);
        //template.temp4.onClick.AddListener(Close);
        //template.temp5.onClick.AddListener(Close);
        //template.temp6.onClick.AddListener(Close);
    }

    private void Close()
    {
        template.skin.SetActive(false);
        MyTickManager.Remove(update);
    }

    public override void enter()
    {
        base.enter();
        //if (template.skin.activeSelf == false)
        //{
        //    template.skin.SetActive(true);
        //    MyTickManager.Add(update);
        //}
        //else
        //{
        //    setState(MainPageFreeState.Name);
        //}


        if (toggleButton.onDown == false )
        {
            setState(MainPageFreeState.Name);
            return;
        }
        template.skin.SetActive(true);
        MyTickManager.Add(update);
    }

    private void update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began && eventSystem.IsPointerOverGameObject() == false)
            {
                setState(MainPageFreeState.Name);
            }
        }
    }

    public override void exit()
    {
        MyTickManager.Remove(update);
        inputMachine.setState(FreeState2D.NAME);
        toggleButton.onDown = false;
        template.skin.SetActive(false);
        //if (machine.nextIsCurrent == false)
        //{
        //    toggleButton.onDown = false;
        //    template.skin.SetActive(false);
        //} 
        base.exit();
    }
    private void OnTemp6()
    {
        inputMachine.templateType = 5;
        inputMachine.setState(SetTempletState.NAME);
        Close();
        Debug.Log("template6");
    }

    private void OnTemp5()
    {
        inputMachine.templateType = 4;
        inputMachine.setState(SetTempletState.NAME);
        Close();
        Debug.Log("template5");
    }

    private void OnTemp4()
    {
        inputMachine.templateType = 3;
        inputMachine.setState(SetTempletState.NAME);
        Close();
        Debug.Log("template4");
    }

    private void OnTemp3()
    {
        inputMachine.templateType = 2;
        inputMachine.setState(SetTempletState.NAME);
        Close();
        Debug.Log("template3");
    }

    private void OnTemp2()
    {
        inputMachine.templateType = 1;
        inputMachine.setState(SetTempletState.NAME);
        Close();
        Debug.Log("template2");
    }

    private void OnTemp1()
    {
        inputMachine.templateType = 0;
        inputMachine.setState(SetTempletState.NAME);
        Close();
        Debug.Log("template1");
        //Debug.Log("template1" + Time.realtimeSinceStartup);
    }
    
}

