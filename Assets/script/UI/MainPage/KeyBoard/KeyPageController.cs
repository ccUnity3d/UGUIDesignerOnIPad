using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class KeyPageController : UIController<KeyPageController>
{
    public KeyBoard keyboard;
    public Text Text;
    public Action onChange;
    private string number;
    public string Number
    {
        get { return number; }
        set {
            number = value;
        }
    }
    public KeyPageController()
    {
        panel = keyboard = KeyBoard.Instance;
    }
    public override void ready()
    {
        base.ready();
        keyboard.one.onClick.AddListener(onOne);
        keyboard.two.onClick.AddListener(onTwo);
        keyboard.there.onClick.AddListener(onThere);
        keyboard.four.onClick.AddListener(onFour);
        keyboard.five.onClick.AddListener(onFive);
        keyboard.six.onClick.AddListener(onSix);
        keyboard.seven.onClick.AddListener(onSeven);
        keyboard.eight.onClick.AddListener(onEight);
        keyboard.nine.onClick.AddListener(onNine);
        keyboard.zero.onClick.AddListener(onZero);
        keyboard.cancel.onClick.AddListener(onCancel);
        keyboard.point.onClick.AddListener(onPoint);
        keyboard.exit.onClick.AddListener(onColse);
    }
    public void onOne()
    {
        if (Number == "0") Number = "";
        Number += "1";
        Text.text = Number;
        if(onChange!=null) onChange();
    }
    public void onTwo()
    {
        if (Number == "0") Number = "";
        Number += "2";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onThere()
    {
        if (Number == "0") Number = "";
        Number += "3";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onFour()
    {
        if (Number == "0") Number = "";
        Number += "4";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onFive()
    {
        if (Number == "0") Number = "";
        Number += "5";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onSix()
    {
        if (Number == "0") Number = "";
        Number += "6";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onSeven()
    {
        if (Number == "0") Number = "";
        Number += "7";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onEight()
    {
        if (Number == "0") Number = "";
        Number += "8";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onNine()
    {
        if (Number == "0") Number = "";
        Number += "9";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onZero()
    {
        if (Number != "0")
        {
            Number += "0";
            Text.text = Number;
            if (onChange != null) onChange();
        }
    }
    public void onCancel()
    {
        Number = "0";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    public void onPoint()
    {
        Number += ".";
        Text.text = Number;
        if (onChange != null) onChange();
    }
    private void onColse()
    {
        UITool.SetActionFalse(keyboard.keyboard.gameObject);
    }
    public string getNumber()
    {
        return this.number;
    }

    public void Open()
    {
        KeyBoard.Instance.keyboard.gameObject.SetActive(true);
        onCancel();
    }

    public void Close()
    {
        KeyBoard.Instance.keyboard.gameObject.SetActive(false);
    }
}
